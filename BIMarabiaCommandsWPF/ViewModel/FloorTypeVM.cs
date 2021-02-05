using BIMarabiaCommandsWPF.RevitHelper;
using BIMarabiaCommandsWPF.ViewModel.Commands;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BIMarabiaCommandsWPF.Model;

namespace BIMarabiaCommandsWPF.ViewModel
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    /// <summary>
    /// The FloorType ViewModel which controls the FloorType View.
    /// </summary>
    public class FloorTypeVM : INotifyPropertyChanged
    {
        private FloorType selectedFloorType;

        /// <summary>
        /// SelectedFloorType property "The selected floor type by the user".
        /// </summary>
        public FloorType SelectedFloorType
        {
            get { return selectedFloorType; }
            set
            {
                if (value != null)
                {
                    selectedFloorType = value;
                    OnPropertyChanged(nameof(SelectedFloorType));
                }
            }
        }

        /// <summary>
        /// The FloorTypes Observable Collection "List" property.
        /// </summary>
        public ObservableCollection<FloorType> FloorTypes { get; set; }

        /// <summary>
        /// The SelectWallsCommand property.
        /// </summary>
        public SelectWallsCommand SelectWallsCommand { get; set; }

        public FloorTypeVM()
        {
            // Initializing the SelectWallsCommand property.
            SelectWallsCommand = new SelectWallsCommand(this);

            // Initializing the FloorTypes property with the revit floor types.
            FloorTypes = new ObservableCollection<FloorType>(ReadFloorTypes());
        }

        /// <summary>
        /// The method that retrieves the available floor types from Revit.
        /// </summary>
        private List<FloorType> ReadFloorTypes()
        {
            // Filtering the revit document to collect all floor types
            return new FilteredElementCollector(RevitBase.Document)
                        .OfClass(typeof(FloorType))
                        .Cast<FloorType>()
                        .ToList();
        }

        /// <summary>
        /// The method that allows the user to select walls and creates the floor.
        /// </summary>
        public void SelectWallsAndCreateFloor()
        {
            // Initialize the wall selection filter
            WallSelectionFilter wallSelectionFilter = new WallSelectionFilter();

            // Select the Walls
            IList<Reference> references = RevitBase.UIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, wallSelectionFilter);

            if (references != null && references.Count > 0)
            {
                // Initialize the original curves list
                List<Curve> originalWallCurves = new List<Curve>();

                // Initialize the List of original offset distances
                List<double> originalOffsets = new List<double>();

                // Initialize the list of levels ids
                List<ElementId> levelsIds = new List<ElementId>();

                foreach (var r in references)
                {
                    // Get the wall element
                    Wall wall = RevitBase.Document.GetElement(r) as Wall;

                    if (wall != null)
                    {
                        // Add the required offset to the list by dividing the wall width by 2
                        originalOffsets.Add(wall.Width / 2.0);

                        // Add the wall curve to the original curves list
                        originalWallCurves.Add((wall.Location as LocationCurve).Curve);

                        // Add wall level id to the levels ids list
                        levelsIds.Add(wall.LevelId);
                    }
                }

                if (levelsIds.All(lvl => lvl == levelsIds[0]))
                {
                    // Get the contiguous curves with offsets tuple.
                    var contiguousCurvesWithOffsetsTuple = CurveHelper.GetContiguousCurvesWithOffsets(originalWallCurves, originalOffsets);

                    // Get the contiguous curves.
                    List<Curve> contiguousCurves = contiguousCurvesWithOffsetsTuple.curves;

                    // Get the contiguous curves.
                    List<double> contiguousOffsets = contiguousCurvesWithOffsetsTuple.offsets;

                    // Create a curve loop offset to the contiguous curves.
                    CurveLoop floorCurves = CurveLoop.CreateViaOffset(CurveLoop.Create(contiguousCurves), contiguousOffsets, new XYZ(0.0, 0.0, 1.0));

                    // Initialize the curve array
                    CurveArray curveArray = new CurveArray();

                    // Append the floor curves in the curve array
                    foreach (var c in floorCurves)
                    {
                        curveArray.Append(c);
                    }

                    // Get the walls level
                    Level level = RevitBase.Document.GetElement(levelsIds[0]) as Level;

                    using (Transaction transaction = new Transaction(RevitBase.Document, "Create floor from Walls"))
                    {
                        transaction.Start();

                        try
                        {
                            //if (SelectedFloorType == null) SelectedFloorType = FloorTypes.First();

                            // Create the floor
                            RevitBase.Document.Create.NewFloor(curveArray, SelectedFloorType, level, false);

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            // Asssign the error message to the message parameter
                            RevitBase.Message = e.Message;

                            // Roll back model changes
                            transaction.RollBack();

                            // Assign result to be failed
                            RevitBase.Result = Result.Failed;
                        }
                    }
                }
                else
                    // Assign result to be failed
                    RevitBase.Result = Result.Failed;
            }
            else
                // Assign result to be failed
                RevitBase.Result = Result.Failed;

            // Return result

        }

        /// <summary>
        /// The event handler which handles the changing in a property value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The method which invokes the event handler if a property changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that has been changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The method for closing the FloorType window by raising the SelectWallsClicked event.
        /// </summary>
        public void CloseWindowToSelectWalls()
        {
            RaiseSelectWallsClickedEvent();
        }

        /// <summary>
        /// The SelectWallsClicked Event Handler.
        /// </summary>
        public event EventHandler SelectWallsClicked;

        /// <summary>
        /// The method that raises "invokes" the SelectWallsClicked event.
        /// </summary>
        private void RaiseSelectWallsClickedEvent()
        {
            SelectWallsClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
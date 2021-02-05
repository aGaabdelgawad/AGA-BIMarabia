using BIMarabiaCommands.RevitHelper;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIMarabiaCommands
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    [TransactionAttribute(TransactionMode.Manual)]
    public class CreateFloorFromWallsContiguous : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Initialize the result.
            Result result = Result.Succeeded;

            // Get the active ui document.
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;

            // Get the document.
            Document document = uIDocument.Document;

            // Initialize the wall selection filter.
            WallSelectionFilter wallSelectionFilter = new WallSelectionFilter();

            // Prompt the user to select the walls.
            IList<Reference> references = uIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, wallSelectionFilter);

            if (references != null && references.Count > 0)
            {
                // Initialize the original curves list.
                List<Curve> originalWallCurves = new List<Curve>();

                // Initialize the List of original offset distances.
                List<double> originalOffsets = new List<double>();

                // Initialize the list of levels ids.
                List<ElementId> levelsIds = new List<ElementId>();

                foreach (var r in references)
                {
                    // Get the wall element.
                    Wall wall = document.GetElement(r) as Wall;

                    if (wall != null)
                    {
                        // Add the required offset to the list by dividing the wall width by 2.
                        originalOffsets.Add(wall.Width / 2.0);

                        // Add the wall curve to the original curves list.
                        originalWallCurves.Add((wall.Location as LocationCurve).Curve);

                        // Add wall level id to the levels ids list.
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

                    // Initialize the curve array.
                    CurveArray curveArray = new CurveArray();

                    // Append the floor curves in the curve array.
                    foreach (var c in floorCurves)
                    {
                        curveArray.Append(c);
                    }

                    // Get the walls level.
                    Level level = document.GetElement(levelsIds[0]) as Level;

                    // Name the floor type.
                    string floorTypeName = "Generic 300mm";

                    // Collect the floor type to be used.
                    FloorType floorType = new FilteredElementCollector(document)
                        .OfClass(typeof(FloorType))
                        .First<Element>(
                        e => e.Name.Equals(floorTypeName)) as FloorType;

                    using (Transaction transaction = new Transaction(document, "Create floor from Walls"))
                    {
                        // Start the transaction.
                        transaction.Start();

                        try
                        {
                            // Create the floor.
                            document.Create.NewFloor(curveArray, floorType, level, false);

                            // Commit the transaction and model changes.
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            // Asssign the error message to the message parameter.
                            message = e.Message;

                            // Roll back model changes.
                            transaction.RollBack();

                            // Assign result to be failed.
                            result = Result.Failed;
                        }
                    }
                }
                else
                    // Assign result to be failed.
                    result = Result.Failed;
            }
            else
                // Assign result to be failed.
                result = Result.Failed;

            // Return result.
            return result;
        }
    }
}
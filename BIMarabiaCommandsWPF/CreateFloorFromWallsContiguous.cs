using BIMarabiaCommandsWPF.View;
using BIMarabiaCommandsWPF.ViewModel;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMarabiaCommandsWPF.Model;

namespace BIMarabiaCommandsWPF
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreateFloorFromWallsContiguous : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Assign the UIDocument, Document and Result to the RevitBase properties
            RevitBase.UIDocument = commandData.Application.ActiveUIDocument;
            RevitBase.Document = commandData.Application.ActiveUIDocument.Document;
            RevitBase.Result = Result.Succeeded;

            // Initialize the FloorType window "view" and FloorType view model
            FloorTypeWindow floorTypeWindow = new FloorTypeWindow();
            FloorTypeVM floorTypeVM = new FloorTypeVM();

            // Register the event that is responsible for closing the window to select the walls
            floorTypeVM.SelectWallsClicked += (sender, args) =>
            {
                floorTypeWindow.Close();
            };

            // Assign the FloorType view model to the FloorType window "view"
            floorTypeWindow.DataContext = floorTypeVM;

            // Show the FloorType window "view"
            floorTypeWindow.ShowDialog();

            // Assign the RevitBase message property to the ref error message
            message = RevitBase.Message;

            // Return the RevitBase result property
            return RevitBase.Result;
        }
    }
}
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace BIMarabiaCommands
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class GetElementInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the active ui document.
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;

            // Get the document.
            Document document = uIDocument.Document;

            try
            {
                // Prompt the user to select an element.
                Reference selectedElement = uIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if (selectedElement != null)
                {
                    // Get the selected element from the document.
                    Element element = document.GetElement(selectedElement.ElementId);

                    // Get the type of the selected element.
                    ElementType elementType = document.GetElement(element.GetTypeId()) as ElementType;

                    // Show the element information through task dialog.
                    TaskDialog.Show("Element Details",
                        $"ID: {element.Id}{Environment.NewLine}" +
                        $"Instance: {element.Name}{Environment.NewLine}" +
                        $"Type: {elementType.Name}{Environment.NewLine}" +
                        $"Family: {elementType.FamilyName}{Environment.NewLine}" +
                        $"Category: {element.Category.Name}");

                    // Return succeeded result.
                    return Result.Succeeded;
                }
                else
                {
                    // Return failed result.
                    return Result.Failed;
                }
            }
            catch (Exception e)
            {
                // Assign the exception "error" message to the error message of Revit.
                message = e.Message;

                // Return failed result.
                return Result.Failed;
            }
        }
    }
}

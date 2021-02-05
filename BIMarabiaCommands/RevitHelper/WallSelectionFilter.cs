using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace BIMarabiaCommands.RevitHelper
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return (BuiltInCategory)GetCategoryIdAsInteger(elem) == BuiltInCategory.OST_Walls;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }

        /// <summary>
        /// Method to get the category id as integer of the element.
        /// </summary>
        /// <param name="element">A candidate element in selection operation.</param>
        /// <returns>The category id as integer.</returns>
        public int GetCategoryIdAsInteger(Element element)
        {
            return element?.Category?.Id?.IntegerValue ?? -1;
        }
    }
}
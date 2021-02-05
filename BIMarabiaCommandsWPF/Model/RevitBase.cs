using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMarabiaCommandsWPF.Model
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    /// <summary>
    /// Revit Base represents the revit model.
    /// </summary>
    public static class RevitBase
    {
        /// <summary>
        /// Revit active ui document.
        /// </summary>
        public static UIDocument UIDocument { get; set; }

        /// <summary>
        /// Revit document.
        /// </summary>
        public static Document Document { get; set; }

        /// <summary>
        /// Revit result enum.
        /// </summary>
        public static Result Result { get; set; }

        /// <summary>
        /// Revit error message.
        /// </summary>
        public static string Message { get; set; }
    }
}
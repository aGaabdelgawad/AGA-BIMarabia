using Autodesk.Revit.DB;
using System;
using System.Windows.Input;

namespace BIMarabiaCommandsWPF.ViewModel.Commands
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    /// <summary>
    /// The select walls command class.
    /// </summary>
    public class SelectWallsCommand : ICommand
    {
        /// <summary>
        /// The owner view model.
        /// </summary>
        public FloorTypeVM VM { get; set; }

        public SelectWallsCommand(FloorTypeVM vm)
        {
            // Assign the view model to the view model property.
            VM = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            // Return whether the parameter "selected floor type" is floor type or not.
            return (parameter as FloorType) != null;
        }

        public void Execute(object parameter)
        {
            // Close the "floor type" window.
            VM.CloseWindowToSelectWalls();

            // Invoke the "select walls and create floor" method.
            VM.SelectWallsAndCreateFloor();
        }
    }
}
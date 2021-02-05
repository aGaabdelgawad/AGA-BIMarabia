using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BIMarabiaCommands
{
    // This project copyrights is for Ahmed Gamal Abdel Gawad,
    // LinkedIn: https://www.linkedin.com/in/aGaabdelgawad/
    // Lectures: https://www.youtube.com/playlist?list=PLgmra2bOLNrdY-dJseru1pByMc4ye5xSo
    // This project is made for Introduction to Revit API using C# workshop,
    // The workshop was held in Cooperation with BIMarabia.

    public class BIMarabiaPlugin : IExternalApplication
    {
        // The assembly of the project.
        static readonly Assembly assembly = Assembly.GetExecutingAssembly();

        // The name of the tab to be added to Revit Ribbon.
        static readonly string tabName = "BIMarabia Plugin";

        public Result OnShutdown(UIControlledApplication application)
        {
            // Return succeeded result.
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Create the tab.
                application.CreateRibbonTab(tabName);

                // Create the "element" panel.
                RibbonPanel elementPanel = application.CreateRibbonPanel(tabName, "Element");

                // Create the "element" button.
                PushButton elementPushButton = CreateButton("elementButton", "Get Element Info", typeof(GetElementInfo).FullName, $"{nameof(BIMarabiaCommands)}.Resources.Info.png", elementPanel, "Get full information about element.");

                // Create the "floor" panel.
                RibbonPanel floorPanel = application.CreateRibbonPanel(tabName, "Floor");

                // Create the "floor" button.
                PushButton floorPushButton = CreateButton("floorButton", "Floor from Walls", typeof(CreateFloorFromWallsContiguous).FullName, $"{nameof(BIMarabiaCommands)}.Resources.Floor.png", floorPanel, "Create floor by selecting walls.");
            }
            catch (Exception e)
            {
                // Show the user an error message through task dialog.
                TaskDialog.Show("Error", e.Message);

                // Retun failed result.
                return Result.Failed;
            }

            // Return succeeded result.
            return Result.Succeeded;
        }

        /// <summary>
        /// Method to create the button with its required information,
        /// and add it to a certain panel.
        /// </summary>
        /// <param name="buttonName">The name of the button to be created.</param>
        /// <param name="buttonText">The text to be shown on the button in Revit UI.</param>
        /// <param name="className">The name of the class of the external command to be executed when clicking the button.</param>
        /// <param name="resource">The path of the image resource for the button.</param>
        /// <param name="panel">The panel where the button to be added.</param>
        /// <param name="description">The additional description to be shown as a tool tip for the button.</param>
        /// <returns>The push button which has been created.</returns>
        public PushButton CreateButton(string buttonName, string buttonText, string className, string imageResource, RibbonPanel panel, string description = null)
        {
            // Create the main information "data" about the button.
            PushButtonData buttonData = new PushButtonData(buttonName, buttonText, assembly.Location, className);

            // Create and add the button to the panel.
            PushButton pushButton = panel.AddItem(buttonData) as PushButton;

            // Add a tool tip description if the user sent it.
            if (description != null) pushButton.ToolTip = description;

            // Initialize the image.
            BitmapImage img = new BitmapImage();

            // Initialize a stream and assign the embedded resource to it.
            Stream stream = assembly.GetManifestResourceStream(imageResource);

            // Assign the resource stream to the image stream source.
            try
            {
                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();
            }
            catch (Exception e)
            {
                // Show an error message through task dialog.
                TaskDialog.Show("Create Button", e.Message);
            }

            // Assign the image to the large image of the button.
            pushButton.LargeImage = img;

            // Return the button that hase been created.
            return pushButton;
        }
    }
}
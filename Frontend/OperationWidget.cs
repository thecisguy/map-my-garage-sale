using System;
using System.Reflection;
using System.IO;
using Gtk;

namespace Frontend
{
    public class OperationWidget
    {
        #region Constructor
        public OperationWidget()
        {
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Creates a VBox.
        /// </summary>
        /// <returns>The operation widget.</returns>
        /// <param name="iconFilePath">Icon file path.</param>
        /// <param name="labelText">Label text.</param>
        public Widget CreateOperationWidget(string iconResourceName, string labelText)
        {
            VBox box = new VBox(false, 0);

            Stream imageStream = getSpecifiedResourceStream(iconResourceName);
            using (Gtk.Image image = new Gtk.Image (imageStream))
            {
                Label label = new Label(labelText);

                box.PackStart(image, false, false, 3);
                box.PackStart(label, false, false, 3);

                image.Show();
                label.Show();
            }
            return box;
        }

        /// <summary>
        /// Returns a stream from the passed in Resource name
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        private Stream getSpecifiedResourceStream(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return stream;
        }
        #endregion
    }
}


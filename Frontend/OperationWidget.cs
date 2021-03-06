/* OperationWidget.cs
 *
 * Contains functions for initializing custom Widget objects.
 *
 * Copyright (C) 2014 - Blake Lowe, Jordan Polaniec
 *
 * This file is part of Map My Garage Sale.
 * 
 * Map My Garage Sale is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * Map My Garage Sale is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Map My Garage Sale. If not, see <http://www.gnu.org/licenses/>.
 */
using Gtk;
using System;
using System.Reflection;
using System.IO;

namespace Frontend
{
    public class OperationWidget
    {
        #region Private Methods

        /// <summary>
        /// Creates a Widget using the passed in resources.
        /// </summary>
        /// <returns>The operation widget.</returns>
        /// <param name="iconFilePath">Icon file path.</param>
        /// <param name="labelText">Label text.</param>
        public Widget CreateOperationWidget(string iconResourceName, string labelText)
        {
            VBox box = new VBox(false, 0);

            //get the resource into memory
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


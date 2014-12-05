/* NewStandTemplateWindow.cs
 *
 * User Interface for creating a new Stand to drag onto the Grid.
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

namespace Frontend
{
    public partial class NewStandTemplateWindow : Gtk.Window
    {
        #region Private Members
        private NodeStore templateStore;
        #endregion

        #region Constructor
        public NewStandTemplateWindow(NodeStore store) : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            this.templateStore = store;
            cancelBtn.Hide();  //for now they can just click 'X'
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Make sure all fields are properly filled in
        /// </summary>
        private bool Validate()
        {
            bool retVal = false;
            string name = nameEntry.Text.Trim();
            Gdk.Color color = standColorButton.Color;
            int width = 0;
            int height = 0;

            if (widthEntry.Text.Trim().Length > 0 && heightEntry.Text.Trim().Length > 0)
            {
                width = Convert.ToInt32(widthEntry.Text.Trim());
                height = Convert.ToInt32(heightEntry.Text.Trim());
                if (name.Length > 0 && width > 0 && height > 0)
                {
                    retVal = true;
                }
            }
            else
            {
                return retVal;
            }
            return retVal;
        }

        /// <summary>
        /// Converts a passed in Gdk.Color object to Cairo color.
        /// </summary>
        /// <returns>The cairo color.</returns>
        /// <param name="gColor">G color.</param>
        private Cairo.Color ToCairoColor(Gdk.Color gColor)
        {
            Cairo.Color color = new Cairo.Color(
                                    (double)gColor.Red / ushort.MaxValue,
                                    (double)gColor.Green / ushort.MaxValue,
                                    (double)gColor.Blue / ushort.MaxValue, 1);
            return color;
        }

        /// <summary>
        /// Clear out the current values in the stand.
        /// </summary>
        private void clearValues()
        {
            nameEntry.Text = string.Empty;
            widthEntry.Text = string.Empty;
            heightEntry.Text = string.Empty;
            standColorButton.Color = new Gdk.Color(0,0,0);
        }
       
        #endregion

        #region Control Events
        protected void okButton_OnClick (object sender, EventArgs e)
        {
            if (Validate())
            {
                //TODO - Create new Stand object - need an api call to do this
                //using static id for now
                Cairo.Color color = ToCairoColor(standColorButton.Color);
                //Stand newStand = new Stand(0, nameEntry.Text.Trim(), color, Convert.ToInt32(widthEntry.Text.Trim()), Convert.ToInt32(heightEntry.Text.Trim()));
                //templateStore.AddNode(newStand);
                clearValues();
            }
            else
            {
                using(MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                    string.Format("Did you fill in all the fields?")))
                {
                    md.Run();
                    md.Destroy();
                }
            }
        }

        /// <summary>
        /// Destroys the dialog
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void cancelButton_OnClick (object sender, EventArgs e)
        {
            this.Destroy();
        }

        /// <summary>
        /// Opens a new dialog for tracking mouse coordinates and forming a new stand.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void drawStandShapeBtn_OnClick (object sender, EventArgs e)
        {
            //open new window with smaller grid for drawing
            throw new NotImplementedException ();
        }
        #endregion
    }
}


/* NewStandDialog.cs
 *
 * User Inteface for creating a new Stand object.
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

using System;
using Gtk;
using Cairo;

namespace Frontend
{
    public partial class NewStandDialog : Gtk.Dialog 
    {
        #region Private Members
        private const string STR_NAME_HINT = "Name: ";
        private const string STR_NAMEENTRY_TOOLTIP = "The name of the Stand object to be created.";
        private const string STR_WIDTH_HINT = "Width (in px): ";
        private const string STR_HEIGHT_HINT = "Height (in px): ";
        private const string STR_DIALOG_TITLE = "Create New Stand";
        #endregion

        #region Constructor
        public NewStandDialog()
        {
            Build();
            SetupUI();
           
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Complete setup of UI.
        /// </summary>
        private void SetupUI()
        {
            //window properties
            this.Title = STR_DIALOG_TITLE;

            //stand name UI and color picker
            HBox nameBox = new HBox(false, 0);
            Label nameLabel = new Label(STR_NAME_HINT);
            Entry nameEntry = new Entry();
            nameEntry.TooltipText = STR_NAMEENTRY_TOOLTIP;
            nameBox.Add(nameLabel);
            nameBox.Add(nameEntry);


            ColorSelection standColorSelector = new ColorSelection();
            //standColorSelector.ColorChanged
            nameBox.Add(standColorSelector);
            nameBox.ShowAll();

            //width UI
            HBox widthBox = new HBox(false, 0);
            Label widthLabel = new Label(STR_WIDTH_HINT);
            widthLabel.SetAlignment(0.0f, 0.5f);
            Entry widthEntry = new Entry();
            widthBox.PackStart(widthLabel, false, false, 0);
            widthBox.PackStart(widthEntry, false, false, 0);
            widthBox.ShowAll();

            //height UI
            HBox heightBox = new HBox(false, 0);
            Label heightLabel = new Label(STR_HEIGHT_HINT);
            Entry heightEntry = new Entry();
            heightBox.PackStart(heightLabel, false, false, 0);
            heightBox.PackStart(heightEntry, false, false, 0);
            heightBox.ShowAll();

            vBoxRoot.Add(nameBox);
            vBoxRoot.Add(widthBox);
            vBoxRoot.Add(heightBox);

        }


        #endregion


        #region Control Events

        #endregion
    }
}


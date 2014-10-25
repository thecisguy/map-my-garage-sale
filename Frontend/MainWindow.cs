/* MainWindow.cs
 *
 * Contains the startup and core user interface functionality.
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
using Frontend;
using csapi;

public partial class MainWindow: Gtk.Window
{	
    #region Private Members
    //UI text 
    public const string STR_NEWMAP_TOOLTIP = "Create a new map.";
    public const string STR_NEWMAP_BUTTON = "New Map";
    public const string STR_SAVEMAP_TOOLTIP = "Save the current map.";
    public const string STR_SAVEMAP_BUTTON = "Save Sale";
    public const string STR_OPENMAP_TOOLTIP =  "Open an existing Sale";
    public const string STR_OPENMAP_BUTTON = "Open Sale";
    public const string STR_NEWSTAND_TOOLTIP = "Create a new Stand";
    public const string STR_NEWSTAND_BUTTON = "New Stand";
    public const string STR_ROTATESTAND_TOOLTIP = "Rotate the selected stand 90 degrees clockwise.";
    public const string STR_ROTATESTAND_BUTTON = "Rotate Stand 90°";
    public const string STR_DELETESTAND_TOOLTIP = "Delete the selected Stand permanently";
    public const string STR_DELETESTAND_BUTTON = "Delete selected Stand";
    public const string STR_REMOVESTAND_TOOLTIP = "Remove the selected Stand from the Map";
    public const string STR_REMOVESTAND_BUTTON = "Remove selected Stand";
    public const string STR_INSERTEXISTINGSTAND_BUTTON = "Insert Existing Stand";
    public const string STR_INSERTEXISTINGSTAND_TOOLTIP = "Insert an existing stand at the selected cell on the map.";
    public const string STR_RENAMESTAND_BUTTON = "Rename selected Stand";
    public const string STR_RENAMESTAND_TOOLTIP = "Rename the selected Stand.";
    public const string STR_TOGGLEGRID_BUTTON = "Display Grid";
    public const string STR_STANDFRAME_TOOLTIP = "Contains all user created Stands for placement";
    public const string STR_STANDFRAME_LABEL = "Stands";
    public const string STR_WINDOWTITLE = "Map my Garage Sale";

    //UI resource paths
    public const string RES_ADDSTAND_ICON = "Frontend.Assets.addstandicon.png";
    public const string RES_NEWFILE_ICON = "Frontend.Assets.newfileicon.png";
    public const string RES_ROTATE_ICON = "Frontend.Assets.rotateicon.png";
    public const string RES_SAVE_ICON = "Frontend.Assets.saveicon.png";

    #endregion

    #region Constructor
	public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
		Build ();
        SetupUI();

		//testing
		//EngineAPI.deselectStand ();
	}
    #endregion


    #region Private Methods

    /// <summary>
    /// Completes UI initialization.
    /// </summary>
    private void SetupUI()
    {
        this.Title = STR_WINDOWTITLE;

        /*
         * Below is setup that I am unable to configure via the Designer and as such it is much easier to initialize and set
         * objects and properties than doing this in the Build() since that is machine generated code. - JPolaniec
         */

        //New, Save and Open Widget setup
        OperationWidget operationWidget = new OperationWidget();

        //New Map button
        Widget mapWidget = operationWidget.CreateOperationWidget (RES_NEWFILE_ICON, STR_NEWMAP_BUTTON);
        mapWidget.Show ();
        Gtk.Button newMapButton = new Gtk.Button (mapWidget);
        newMapButton.Clicked += newMapButton_Clicked;
        newMapButton.TooltipText = STR_NEWMAP_TOOLTIP;
        newMapButton.Show ();
        hboxNSO.PackStart (newMapButton, false, false, 3);

        //Save button
        Widget saveWidget = operationWidget.CreateOperationWidget (RES_SAVE_ICON, STR_SAVEMAP_BUTTON);
        saveWidget.Show ();
        Gtk.Button saveButton = new Gtk.Button (saveWidget);
        saveButton.Clicked += saveButton_Clicked;
        saveButton.TooltipText = STR_SAVEMAP_TOOLTIP;
        saveButton.Show ();
        hboxNSO.PackStart (saveButton, false, false, 3);

        //Open button
        FileChooserButton fileChooserButton = new FileChooserButton (STR_OPENMAP_BUTTON, FileChooserAction.Open);
        fileChooserButton.TooltipText = STR_OPENMAP_TOOLTIP;
        //TODO - fileChooserButton.Filter = we need to set a file extension for our saved sales!
        hboxNSO.PackEnd(fileChooserButton, false, false, 0);

        VSeparator middleSeparator = new VSeparator ();
        hboxStand.Add (middleSeparator);

        //Stand options Widget setup
        //New Stand button
        Widget newStandWidget = operationWidget.CreateOperationWidget (RES_NEWFILE_ICON, STR_NEWSTAND_BUTTON);
        newStandWidget.Show ();
        Gtk.Button newStandButton = new Gtk.Button (newStandWidget);
        newStandButton.Clicked += newStandButton_Clicked;
        newStandButton.TooltipText = STR_NEWSTAND_TOOLTIP;
        newStandButton.Show ();
        hboxStand.PackStart (newStandButton, false, false, 6);

        //Rotate button
        Widget rotateWidget = operationWidget.CreateOperationWidget (RES_ROTATE_ICON, STR_ROTATESTAND_BUTTON);
        rotateWidget.Show ();
        Gtk.Button rotateButton = new Gtk.Button (rotateWidget);
        rotateButton.Clicked += rotateButton_Clicked;
        rotateButton.TooltipText = STR_ROTATESTAND_TOOLTIP;
        rotateButton.Show();
        hboxStand.PackStart(rotateButton, false, false, 3);

        //Create a box to hold both the rotate button and existing button
        VBox rotateAndExistingBox = new VBox (false, 6);

        //Insert existing stand
        Gtk.Button insertExistingButton = new Gtk.Button (STR_INSERTEXISTINGSTAND_BUTTON);
        insertExistingButton.Show ();
        insertExistingButton.Clicked += insertExistingButton_Clicked;
        insertExistingButton.TooltipText = STR_INSERTEXISTINGSTAND_TOOLTIP;
        rotateAndExistingBox.PackStart (insertExistingButton, false, false, 3);

        //Remove selected stand
        Gtk.Button removeStandButton = new Gtk.Button (STR_REMOVESTAND_BUTTON);
        removeStandButton.Show ();
        removeStandButton.Clicked += removeStandButton_Clicked;
        removeStandButton.TooltipText = STR_REMOVESTAND_TOOLTIP;
        rotateAndExistingBox.PackStart (removeStandButton, false, false, 3);
        hboxStand.PackStart (rotateAndExistingBox, false, false, 3);

        /* Removing for now.
        VSeparator endSeparator = new VSeparator ();
        hboxStand.PackEnd (endSeparator, false, false, 0);
        */

        //Create a box to hold both the delete and rename buttons
        VBox deleteAndRenameBox = new VBox (false, 6);

        //Delete Widget setup
        Button deleteStandButton = new Button (STR_DELETESTAND_BUTTON);
        deleteStandButton.Show ();
        deleteStandButton.Clicked += deleteStandButton_Clicked;
        deleteStandButton.TooltipText = STR_DELETESTAND_TOOLTIP;
        deleteAndRenameBox.PackStart (deleteStandButton, false, false, 3);

        //Rename Widget setup
        Button renameStandButton = new Button (STR_RENAMESTAND_BUTTON);
        renameStandButton.Show ();
        renameStandButton.Clicked += renameStandButton_Clicked;
        renameStandButton.TooltipText = STR_RENAMESTAND_TOOLTIP;
        deleteAndRenameBox.PackStart (renameStandButton, false, false, 3);

        hboxRename.PackEnd (deleteAndRenameBox, false, false, 3);

        //Insert Grid
        CairoGraphic.drawGrid();  //draws the grid to an image file for now.  This will probably change once core is hooked up.
        Image g = new Image ("grid.png"); //file created from CairoGraphic draw call.
        g.Show ();
        vboxGrid.Add (g);

        //ToggleButton for Grid on/off
        ToggleButton gridToggleButton = new ToggleButton (STR_TOGGLEGRID_BUTTON);
        gridToggleButton.Show ();
        hboxToggle.PackStart (gridToggleButton, false, false, 3);

        //Stands Frame
        Gtk.Frame StandFrame = new Gtk.Frame();
        StandFrame.ExposeEvent += OnStandFrameExposeEvent;
        StandFrame.TooltipText = STR_STANDFRAME_TOOLTIP;
        StandFrame.CanFocus = true;
        StandFrame.ShadowType = ((Gtk.ShadowType)(1));
        StandFrame.BorderWidth = ((uint)(1));
        StandFrame.Show();
        MainTable.Attach(StandFrame, 2, 3, 2, 5);
    }

    #endregion

	#region Control Events

    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
        
    protected void saveButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void newMapButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void newStandButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void rotateButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void insertExistingButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void removeStandButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void deleteStandButton_Clicked(object sender, EventArgs e)
    {
    }

    protected void renameStandButton_Clicked(object sender, EventArgs e)
    {
    }
       
    protected void OnStandFrameExposeEvent (object o, ExposeEventArgs args)
    {
        //Testing drawing a stand - a rectangle in Cairo

        Gtk.Frame standFrame = (Gtk.Frame)o;
        CairoGraphic cairoGraphic = new CairoGraphic();
        OperationWidget operationWidget = new OperationWidget();

        using (Context context = Gdk.CairoHelper.Create(standFrame.GdkWindow))
        {
            cairoGraphic.DrawCurvedRectangle(context, standFrame.Allocation.X + 15, standFrame.Allocation.Y + 25, 75, 60);
        }
       
        VBox stand = operationWidget.createStand(cairoGraphic);
        stand.Show();
    }
    #endregion
}

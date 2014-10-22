using System;
using Gtk;
using Cairo;
using Frontend;
using csapi;

public partial class MainWindow: Gtk.Window
{	
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
        this.Title = "Map my Garage Sale - <FileName>";

        /*
         * Below is setup that I am unable to configure via the Designer and as such it is much easier to initialize and set
         * objects and properties than doing this in the Build() since that is machine generated code. - JPolaniec
         */

        //New, Save and Open Widget setup
        OperationWidget operationWidget = new OperationWidget();

        //New Map button
        Widget mapWidget = operationWidget.CreateOperationWidget ("Frontend.Assets.newfileicon.png", "New Sale");
        mapWidget.Show ();
        Gtk.Button newMapButton = new Gtk.Button (mapWidget);
        newMapButton.TooltipText = "Design a new map.";
        newMapButton.Show ();
        hboxNSO.PackStart (newMapButton, false, false, 3);

        //Save button
        Widget saveWidget = operationWidget.CreateOperationWidget ("Frontend.Assets.saveicon.png", "Save Sale");
        saveWidget.Show ();
        Gtk.Button saveButton = new Gtk.Button (saveWidget);
        saveButton.TooltipText = "Save the current map.";
        saveButton.Show ();
        hboxNSO.PackStart (saveButton, false, false, 3);

        //Open button
        FileChooserButton fileChooserButton = new FileChooserButton ("Open Sale", FileChooserAction.Open);
        fileChooserButton.TooltipText = "Open an existing Sale";
        //TODO - fileChooserButton.Filter = we need to set a file extension for our saved sales!
        hboxNSO.PackEnd(fileChooserButton, false, false, 0);

        VSeparator middleSeparator = new VSeparator ();
        hboxStand.Add (middleSeparator);

        //Stand options Widget setup
        //New Stand button
        Widget newStandWidget = operationWidget.CreateOperationWidget ("Frontend.Assets.addstandicon.png", "New Stand");
        newStandWidget.Show ();
        Gtk.Button newStandButton = new Gtk.Button (newStandWidget);
        newStandButton.TooltipText = "Create a new stand.";
        newStandButton.Show ();
        hboxStand.PackStart (newStandButton, false, false, 6);

        //Rotate button
        Widget rotateWidget = operationWidget.CreateOperationWidget ("Frontend.Assets.rotateicon.png", "Rotate Stand 90°");
        rotateWidget.Show ();
        Gtk.Button rotateButton = new Gtk.Button (rotateWidget);
        rotateButton.TooltipText = "Rotate the selected stand 90 degrees clockwise.";
        rotateButton.Show();
        hboxStand.PackStart(rotateButton, false, false, 3);

        //Create a box to hold both the rotate button and existing button
        VBox rotateAndExistingBox = new VBox (false, 6);

        //Insert existing stand
        Gtk.Button insertExistingButton = new Gtk.Button ("Insert Existing Stand");
        insertExistingButton.Show ();
        insertExistingButton.TooltipText = "Insert an existing stand at the selected cell on the map.";
        rotateAndExistingBox.PackStart (insertExistingButton, false, false, 3);

        //Remove selected stand
        Gtk.Button removeStandButton = new Gtk.Button ("Remove Selected Stand");
        removeStandButton.Show ();
        removeStandButton.TooltipText = "Remove the selected stand from the Map";
        rotateAndExistingBox.PackStart (removeStandButton, false, false, 3);
        hboxStand.PackStart (rotateAndExistingBox, false, false, 3);

        /* Removing for now.
        VSeparator endSeparator = new VSeparator ();
        hboxStand.PackEnd (endSeparator, false, false, 0);
        */

        //Create a box to hold both the delete and rename buttons
        VBox deleteAndRenameBox = new VBox (false, 6);

        //Delete Widget setup
        Button deleteStandButton = new Button ("Delete Selected Stand");
        deleteStandButton.Show ();
        deleteStandButton.TooltipText = "Deletes the current Stand in the Stands panel permanently.";
        deleteAndRenameBox.PackStart (deleteStandButton, false, false, 3);

        //Rename Widget setup
        Button renameStandButton = new Button ("Rename Selected Stand");
        renameStandButton.Show ();
        renameStandButton.TooltipText = "Rename the Selected Stand in the Stands Panel below.";
        deleteAndRenameBox.PackStart (renameStandButton, false, false, 3);

        hboxRename.PackEnd (deleteAndRenameBox, false, false, 3);

        //Insert Grid
        CairoGraphic.drawGrid();  //draws the grid to an image file for now.  This will probably change once core is hooked up.
        Image g = new Image ("grid.png"); //file created from CairoGraphic draw call.
        g.Show ();
        vboxGrid.Add (g);

        //ToggleButton for Grid on/off
        ToggleButton gridToggleButton = new ToggleButton ("Display GRID");
        gridToggleButton.Show ();
        hboxToggle.PackStart (gridToggleButton, false, false, 3);
    }

    #endregion

	#region Control Events
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
    #endregion
}

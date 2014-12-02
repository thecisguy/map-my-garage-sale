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
using Frontend.Map;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
    #region Private Members
    //UI text 
    private const string STR_NEWMAP_TOOLTIP = "Create a new Map.";
    private const string STR_NEWMAP_BUTTON = "New Map";
    private const string STR_SAVEMAP_TOOLTIP = "Save the current Map.";
    private const string STR_SAVEMAP_BUTTON = "Save Sale";
    private const string STR_OPENMAP_TOOLTIP =  "Open an existing Map";
    private const string STR_OPENMAP_BUTTON = "Open Sale";
    private const string STR_NEWSTAND_TOOLTIP = "Create a new Stand";
    private const string STR_NEWSTAND_BUTTON = "New Stand";
    private const string STR_ROTATESTAND_TOOLTIP = "Rotate the selected stand 90 degrees clockwise.";
    private const string STR_ROTATESTAND_BUTTON = "Rotate Stand 90°";
    private const string STR_DELETESTAND_TOOLTIP = "Delete the selected Stand permanently";
    private const string STR_DELETESTAND_BUTTON = "Delete Stand Template";
    private const string STR_REMOVESTAND_TOOLTIP = "Remove the selected Stand from the Map";
    private const string STR_REMOVESTAND_BUTTON = "Remove selected Stand";
    private const string STR_RENAMESTAND_BUTTON = "Rename selected Stand";
    private const string STR_RENAMESTAND_TOOLTIP = "Rename the selected Stand.";
    private const string STR_TOGGLEGRID_BUTTON = "Display Grid";
    private const string STR_BACKDROP_BUTTON = "Change Backdrop...";
    private const string STR_BACKDROP_TOOLTIP = "Change the backdrop shown behind the map design area.";
    private const string STR_STANDFRAME_TOOLTIP = "Contains all user created Stands for placement";
    private const string STR_STANDFRAME_LABEL = "Stands";
    private const string STR_WINDOWTITLE = "Map my Garage Sale - ";
    private const string STR_DIRTYMARK = "*";
    private const string STR_NEWMAP_DIALOG_TITLE = "Create new Map";
    private const string STR_METADATA_LABEL = "<Selected Stand metadata>";

    //UI resource paths
    private const string RES_ADDSTAND_ICON = "Frontend.Assets.addstandicon.png";
    private const string RES_NEWFILE_ICON = "Frontend.Assets.newfileicon.png";
    private const string RES_ROTATE_ICON = "Frontend.Assets.rotateicon.png";
    private const string RES_SAVE_ICON = "Frontend.Assets.saveicon.png";

    //selected stand constants
    private const string KEY_PREVIOUS_STAND_ORIGIN_X = "key_previous_stand_origin_x";
    private const string KEY_PREVIOUS_STAND_ORIGIN_Y = "key_previous_stand_origin_y";
    private const string KEY_PREVIOUS_STAND_HEIGHT = "key_previous_stand_height";
    private const string KEY_PREVIOUS_STAND_WIDTH = "key_previous_stand_width";
    private const string KEY_CURRENT_STAND_ORIGIN_X = "key_current_stand_origin_x";
    private const string KEY_CURRENT_STAND_ORIGIN_Y = "key_current_stand_origin_y";
    private const string KEY_CURRENT_STAND_HEIGHT = "key_current_stand_height";
    private const string KEY_CURRENT_STAND_WIDTH = "key_current_stand_width";
    private const string KEY_CURRENT_STAND_START_DRAG_X = "key_current_stand_start_drag_x";
    private const string KEY_CURRENT_STAND_START_DRAG_Y = "key_current_stand_start_drag_y";

    //UI members
    private HBox hboxToggle;
    private Frame StandFrame;
    private DrawingArea Grid;
    private NodeView view;
    private NodeStore store;
    private bool isMousePressed = false;
    private Dictionary<string, int> selectedStandInformation;
    private bool isStandSelected = false;
    private int DrawType;

    /// <summary>
    /// Maintains a list of all stands.  Design for how this works is a WIP.
    /// </summary>
    private AppState AppState;

    //Drag and Drop target values
    private enum TargetType {
        String,
        RootWindow
    };

    private static TargetEntry [] target_table = new TargetEntry [] {
        new TargetEntry ("STRING", 0, (uint) TargetType.String ),
        new TargetEntry ("text/plain", 0, (uint) TargetType.String),
        new TargetEntry ("application/x-rootwindow-drop", 0, (uint) TargetType.RootWindow)
    };

    #endregion

    #region Constructor
	public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
		Build ();
        SetupUI();
        AppState = new AppState();
	}
    #endregion

    #region Private Methods

    /// <summary>
    /// Completes UI initialization.
    /// </summary>
    private void SetupUI()
    {
        this.Title = STR_WINDOWTITLE;
        this.Icon = new Gdk.Pixbuf("Assets/icon.ico");
        selectedStandInformation = new Dictionary<string, int>();
        selectedStandInformation.Add(KEY_PREVIOUS_STAND_ORIGIN_X, 0);
        selectedStandInformation.Add(KEY_PREVIOUS_STAND_ORIGIN_Y, 0);
        selectedStandInformation.Add(KEY_PREVIOUS_STAND_HEIGHT, 0);
        selectedStandInformation.Add(KEY_PREVIOUS_STAND_WIDTH, 0);
        selectedStandInformation.Add(KEY_CURRENT_STAND_ORIGIN_X, 0);
        selectedStandInformation.Add(KEY_CURRENT_STAND_ORIGIN_Y, 0);
        selectedStandInformation.Add(KEY_CURRENT_STAND_HEIGHT, 0);
        selectedStandInformation.Add(KEY_CURRENT_STAND_WIDTH, 0);
        selectedStandInformation.Add(KEY_CURRENT_STAND_START_DRAG_X, 0);
        selectedStandInformation.Add(KEY_CURRENT_STAND_START_DRAG_Y, 0);

        /**
         * Below is setup that I am unable to configure via the Designer and as such it is much easier to initialize and set
         * objects and properties than doing this in the Build() since that is machine generated code. - JPolaniec
         */

        #region File Operation Buttons
        //New, Save and Open Widget setup
        OperationWidget operationWidget = new OperationWidget();


        //New Map button
        Widget mapWidget = operationWidget.CreateOperationWidget (RES_NEWFILE_ICON, STR_NEWMAP_BUTTON);
        mapWidget.Show ();
        Gtk.Button newMapButton = new Gtk.Button (mapWidget);
        newMapButton.Clicked += newMapButton_Clicked;
        newMapButton.TooltipText = STR_NEWMAP_TOOLTIP;
        newMapButton.Show ();
        hboxNSO.PackStart (newMapButton, false, false, 0);

        //Save button
        Widget saveWidget = operationWidget.CreateOperationWidget (RES_SAVE_ICON, STR_SAVEMAP_BUTTON);
        saveWidget.Show ();
        Gtk.Button saveButton = new Gtk.Button (saveWidget);
        saveButton.Clicked += saveButton_Clicked;
        saveButton.TooltipText = STR_SAVEMAP_TOOLTIP;
        saveButton.Show ();
        hboxNSO.PackStart(saveButton, false, false, 0);

        //Open button
        FileChooserButton fileChooserButton = new FileChooserButton (STR_OPENMAP_BUTTON, FileChooserAction.Open);
        fileChooserButton.TooltipText = STR_OPENMAP_TOOLTIP;
        FileFilter mmgsFileFilter = new FileFilter();
        mmgsFileFilter.Name = "Maps";
        mmgsFileFilter.AddPattern("*.mmgs");
        FileFilter allFileFilter = new FileFilter();
        allFileFilter.Name = "All Files";
        allFileFilter.AddPattern("*");
        fileChooserButton.AddFilter(mmgsFileFilter);
        fileChooserButton.AddFilter(allFileFilter);
        fileChooserButton.FileSet += fileChooserButton_FileSet;
        hboxNSO.PackStart(fileChooserButton, false, false, 10);

        #endregion

        VSeparator middleSeparator = new VSeparator ();
        hboxStand.Add (middleSeparator);

        #region Stand Buttons

        //New Stand button
        Widget newStandWidget = operationWidget.CreateOperationWidget (RES_ADDSTAND_ICON, STR_NEWSTAND_BUTTON);
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
        VBox rotateBox = new VBox (false, 6);

        //Remove selected stand
        Gtk.Button removeStandButton = new Gtk.Button (STR_REMOVESTAND_BUTTON);
        removeStandButton.Show ();
        removeStandButton.Clicked += removeStandButton_Clicked;
        removeStandButton.TooltipText = STR_REMOVESTAND_TOOLTIP;
        rotateBox.PackStart (removeStandButton, false, false, 3);
        hboxStand.PackStart (rotateBox, false, false, 3);

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

        hboxStand.PackEnd (deleteAndRenameBox, false, false, 3);
        #endregion

        #region Grid and Grid Buttons and Labels

        // Toggle
        this.hboxToggle = new Gtk.HBox ();
        this.hboxToggle.Name = "hboxToggle";
        this.hboxToggle.Spacing = 6;
        this.MainTable.Add (this.hboxToggle);
        Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.MainTable [this.hboxToggle]));
        w6.TopAttach = ((uint)(5));
        w6.BottomAttach = ((uint)(6));
        w6.XOptions = ((Gtk.AttachOptions)(2));
        w6.YOptions = ((Gtk.AttachOptions)(2));

        //ToggleButton for Grid on/of
        ToggleButton gridToggleButton = new ToggleButton (STR_TOGGLEGRID_BUTTON);
        gridToggleButton.Clicked += new EventHandler(gridToggleButton_OnClicked);
        gridToggleButton.Show ();
        hboxToggle.PackStart (gridToggleButton, false, false, 3);

        //progress bar
        //progressbar1.Hide();

        //Change backdrop button
        Button backdropButton = new Button(STR_BACKDROP_BUTTON);
        backdropButton.Clicked += new EventHandler(backdropButton_OnClicked);
        backdropButton.Show();
        hboxToggle.PackStart(backdropButton, false, false,3);

        InitializeGrid(true);

        #endregion

        #region Stand Templates
        //Stands Frame
        StandFrame = new Gtk.Frame();
        StandFrame.TooltipText = STR_STANDFRAME_TOOLTIP;
        StandFrame.CanFocus = true;
        StandFrame.ShadowType = ((Gtk.ShadowType)(1));
        StandFrame.BorderWidth = ((uint)(1));
        StandFrame.Show();
       
        MainTable.Attach(StandFrame, 3,4,2,5);

        InitializeStandTemplates(true);
        StandFrame.Add(view);

        #endregion

        #region MenuBar setup
        MenuItem menuItem = new MenuItem("File");
        Menu menu = new Menu();

        AccelGroup accelGroup = new AccelGroup();
        AddAccelGroup(accelGroup);

        ImageMenuItem openImageItem = new ImageMenuItem(Stock.Open, accelGroup);
        openImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.o, Gdk.ModifierType.ControlMask, AccelFlags.Visible));

        ImageMenuItem saveImageItem = new ImageMenuItem(Stock.Save, accelGroup);
        saveImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.s, Gdk.ModifierType.ControlMask, AccelFlags.Visible));

        MenuItem backdropImageItem = new MenuItem("Change Backdrop");
        backdropImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.s, Gdk.ModifierType.ShiftMask, AccelFlags.Visible));

        ImageMenuItem exitImageItem = new ImageMenuItem(Stock.Quit, accelGroup);
        exitImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.q, Gdk.ModifierType.ControlMask, AccelFlags.Visible));

        menu.Append(openImageItem);
        menu.Append(saveImageItem);
        menu.Append(new SeparatorMenuItem());
        menu.Append(backdropImageItem);
        menu.Append(new SeparatorMenuItem());
        menu.Append(exitImageItem);
        menuItem.Submenu = menu;
        menubar.Append(menuItem);
        Gtk.Table.TableChild menuChild = ((Gtk.Table.TableChild)(this.MainTable [this.menubar]));
        menuChild.TopAttach = ((uint)(0));
        menuChild.BottomAttach = ((uint)(1));
        menuChild.RightAttach = ((uint)(3));
        #endregion
       
    }

    /// <summary>
    /// Draws the grid.
    /// </summary>
    private void InitializeGrid(bool isFirstTime = false)
    {

        if (isFirstTime)
        {
            Grid = new DrawingArea();

            Grid.ExposeEvent += new ExposeEventHandler(GridExposeEvent);
            Grid.DragMotion += new DragMotionHandler(GridDragMotion);
            Grid.DragDataReceived += new DragDataReceivedHandler(GridDragDataReceived);
            Grid.DragDrop += new DragDropHandler(GridDragDrop);
            Grid.MotionNotifyEvent += new MotionNotifyEventHandler(GridMotionNotifyEvent);

            Gtk.Drag.DestSet(Grid, DestDefaults.Drop | DestDefaults.Motion, target_table, Gdk.DragAction.Copy);
            Grid.AddEvents((int)
                (Gdk.EventMask.ButtonPressMask
                    | Gdk.EventMask.ButtonReleaseMask
                    | Gdk.EventMask.KeyPressMask
                    | Gdk.EventMask.PointerMotionMask));
            Grid.KeyPressEvent += new KeyPressEventHandler(GridKeyPress);
            Grid.ButtonPressEvent += new ButtonPressEventHandler(GridButtonPress);
            Grid.ButtonReleaseEvent += new ButtonReleaseEventHandler(GridButtonRelease);

            MainTable.Add(Grid);

            Gtk.Table.TableChild gridChild = ((Gtk.Table.TableChild)(this.MainTable[this.Grid]));
            gridChild.TopAttach = ((uint)(2));
            gridChild.BottomAttach = ((uint)(5));
            gridChild.LeftAttach = ((uint)(0));
            gridChild.RightAttach = ((uint)(3));
            gridChild.XOptions = ((Gtk.AttachOptions)(4));
            gridChild.YOptions = ((Gtk.AttachOptions)(4));
        }

        //defaults
        CairoGrid.Height = 400u;
        CairoGrid.Width = 600u;

        try{
            uint height = EngineAPI.getMainGridHeight();
            uint width = EngineAPI.getMainGridWidth();
            CairoGrid.Height = height;
            CairoGrid.Width = width;
        }catch(MissingMethodException me)
        {
            //TODO-
            Console.WriteLine("Missing method exception hit: \n" + me.Message + "\n\n" + me.StackTrace);
        }
        catch(System.Security.SecurityException se)
        {
            Console.WriteLine("SecurityException hit: \n" + se.StackTrace);
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception while loading grid: " + e.Message + "\n\n" + e.StackTrace);
        }

        DrawType = (int)Enumerations.DrawType.InitialGridDraw;
    }

    /// <summary>
    /// Initializes the stand templates.
    /// </summary>
    private void InitializeStandTemplates(bool isFirstTime = false)
    {
        if (isFirstTime)
        {
            view = new NodeView();

            Gtk.Drag.SourceSet(view, Gdk.ModifierType.Button1Mask, target_table, Gdk.DragAction.Copy);
            view.DragDataGet += new Gtk.DragDataGetHandler(StandTemplateSourceDragDataGet);
            view.DragBegin += new Gtk.DragBeginHandler(StandTemplateSourceDragDataBegin);
            view.DragEnd += new Gtk.DragEndHandler(StandTemplateSourceDragDataEnd);
            view.NodeSelection.Changed += new System.EventHandler(StandTemplateNodeSelectionChanged);
            view.KeyReleaseEvent += new KeyReleaseEventHandler(StandTemplateNodeSelectionNodeChanged);

            Gtk.CellRendererText editableCell = new Gtk.CellRendererText();
            editableCell.Editable = true;
            editableCell.Edited += new EditedHandler(CellEditedHandler);

            view.AppendColumn("ID", new Gtk.CellRendererText(), "text", 0);
            view.AppendColumn("Icon", new Gtk.CellRendererPixbuf(), "pixbuf", 1);
            view.AppendColumn("Name", editableCell, "text", 2);
        }
        view.NodeStore = LoadStandTemplates();
        view.ShowAll();
    }

    private NodeStore LoadStandTemplates()
    {
        //get stand templates from API
        if(store != null && view.NodeStore != null)
        {
            view.NodeStore.Clear();
            store.Clear();
        }
        else
        {
            store = new NodeStore(typeof(Stand));
        }

        int numStandTemplates = EngineAPI.getNumTemplates();
        for (int i = 0; i < numStandTemplates; i++)
        {
            store.AddNode(new Stand(i, EngineAPI.getSTName(i), EngineAPI.getColorOfST(i))); //create stand in ui
        }

        //static
//        store.AddNode(new Stand(0, "Toys", new,  40, 70));
//        store.AddNode(new Stand(1, "Movies", new Cairo.Color(.67854,.78561,.94151,1), 100, 90));
//        store.AddNode(new Stand(2, "Books and CDs", new Cairo.Color(.228,.15611,.7561,1), 75, 90));
//        store.AddNode(new Stand(3, "Vintage Action Figures", new Cairo.Color(.9843,.78561,.94151,1), 60, 40));
        //end static testing
        return store;
    }
   
    private void RefreshUI(string fileName = "")
    {
        this.Title = STR_WINDOWTITLE + fileName;
        InitializeGrid(false);
        InitializeStandTemplates(false);
    }

    #endregion

	#region Control Events

    #region Grid Events

    #region Grid Drag and Drop

    protected void GridDragDataReceived(object o, DragDataReceivedArgs args)
    {
        Console.WriteLine("Drag Data Received at: " + args.X + ", " + args.Y);
        Stand stand = new Stand(args.SelectionData.Text);
        Console.WriteLine(isStandSelected);
        if (EngineAPI.canApplyGrabbedStand((long)args.Y, (long)args.X) && isStandSelected)
        {
            EngineAPI.doApplyGrabbedStand();

            DrawType = (int)Enumerations.DrawType.StandDraw;
            Grid.QueueDraw();
        }
        else
        {
            using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Unable to apply Stand here."))
            {
                md.Run();
                md.Destroy();
            }
            Grid.QueueDraw();
        }

        isStandSelected = false;
    }

    protected void GridDragDrop(object o, DragDropArgs args)
    {
        Console.WriteLine("Grid Drag Drop: " + args.X + ", " + args.Y);
    }

    protected void GridDragMotion(object o, DragMotionArgs args)
    {
        Console.WriteLine("Grid Drag Motion: " + args.X + ", " + args.Y);
    }

    protected void GridMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        if (isMousePressed)
        {
            Console.WriteLine("Grid motion with mouse at:" + args.Event.X + ", " + args.Event.Y);

        }
    }

    protected void GridKeyPress(object o, KeyPressEventArgs args)
    {
        Console.WriteLine("Key press: " + args.Event.Key.ToString());
    }

    protected void GridButtonPress(object o, ButtonPressEventArgs args)
    {
        //left mouse button
        if (args.Event.Button == 1)
        {
            isMousePressed = true;
            //did user click on a stand
            long currentSelectedStandOriginX = 0;
            long currentSelectedStandOriginY = 0;
            Console.WriteLine("press"); 
            if (EngineAPI.selectStand((uint)args.Event.Y, (uint)args.Event.X, out currentSelectedStandOriginY, out currentSelectedStandOriginX))
            {
               
                isStandSelected = true;
                selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y] = (int)currentSelectedStandOriginY;
                selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X] = (int)currentSelectedStandOriginX;

                //store drag points
                selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_X] = (int)args.Event.X;
                selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_Y] = (int)args.Event.Y;

                CairoStand.Width = (int)EngineAPI.getSelectedStandWidth();
                CairoStand.Height = (int)EngineAPI.getSelectedStandHeight();

                selectedStandInformation[KEY_CURRENT_STAND_WIDTH] = CairoStand.Width;
                selectedStandInformation[KEY_CURRENT_STAND_HEIGHT] = CairoStand.Height;
                metadataStatusBar.Push(0, "Name: " + EngineAPI.getSelectedStandName() + " Height: " + CairoStand.Height + " | Width: " + CairoStand.Width);

                DrawType = (int)Enumerations.DrawType.StandSelected;
                Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], CairoStand.Width, CairoStand.Height);
                if (CairoGrid.BackdropPath.Length > 0)
                {
                    DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                    Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], CairoStand.Width, CairoStand.Height);
                }
            }
            else
            {
                Console.WriteLine("stand not selected");
                if (isStandSelected)
                {
                    if (selectedStandInformation[KEY_CURRENT_STAND_HEIGHT] > 0 && selectedStandInformation[KEY_CURRENT_STAND_WIDTH] > 0)
                    {
                        isStandSelected = false;
                        DrawType = (int)Enumerations.DrawType.StandUnselected;
                        EngineAPI.deselectStand();
                        metadataStatusBar.Push(0, "No Stand selected.");
                        Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_WIDTH], selectedStandInformation[KEY_CURRENT_STAND_HEIGHT]);
                        if (CairoGrid.BackdropPath.Length > 0)
                        {
                            DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                            Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_WIDTH], selectedStandInformation[KEY_CURRENT_STAND_HEIGHT]);
                        }
                    }
                }
            }
        }
    }

    protected void GridButtonRelease(object o, ButtonReleaseEventArgs args)
    {
        //left mouse button

        //check to see that the stand has been moved a significant amount

        if (args.Event.Button == 1 && isStandSelected)
        {
            bool checkMoveX = args.Event.X > selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_X] + 10 || args.Event.X < selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_X] - 10;
            bool checkMoveY = args.Event.Y > selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_Y] + 10 || args.Event.Y < selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_Y] - 10;

            Console.WriteLine("checkmovex: " + checkMoveX);
            Console.WriteLine("checkmovey: " + checkMoveY);
            Console.WriteLine("selected stand height (test):" + EngineAPI.getSelectedStandHeight());
            if (checkMoveX && checkMoveY)
            {
                EngineAPI.grabSelectedStand();
                bool canApplyStand = EngineAPI.canApplyGrabbedStand((long)args.Event.Y, (long)args.Event.X);
                Console.WriteLine(args.Event.X + ", " + args.Event.Y);
                Console.WriteLine("after apply is:" + canApplyStand);
                if (canApplyStand)
                {
                    EngineAPI.doApplyGrabbedStand();

                    //update keys
                    selectedStandInformation[KEY_PREVIOUS_STAND_ORIGIN_X] = selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X];
                    selectedStandInformation[KEY_PREVIOUS_STAND_ORIGIN_Y] = selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y];
                    selectedStandInformation[KEY_PREVIOUS_STAND_WIDTH] = selectedStandInformation[KEY_CURRENT_STAND_WIDTH];
                    selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] = selectedStandInformation[KEY_CURRENT_STAND_HEIGHT];

                    long x, y;
                    try
                    {
                        if (EngineAPI.selectStand(checked((uint)args.Event.Y), checked((uint)args.Event.X), out y, out x))
                        {
                            //we now have stand at new coords as well as the origin points
                            selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X] = (int)x;
                            selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y] = (int)y;
                            selectedStandInformation[KEY_CURRENT_STAND_WIDTH] = (int)EngineAPI.getSelectedStandWidth();
                            selectedStandInformation[KEY_CURRENT_STAND_HEIGHT] = (int)EngineAPI.getSelectedStandHeight();

                            DrawType = (int)Enumerations.DrawType.ExistingStandRedraw;
//                            Grid.QueueDrawArea(selectedStandInformation[KEY_PREVIOUS_STAND_ORIGIN_X], selectedStandInformation[KEY_PREVIOUS_STAND_ORIGIN_Y], selectedStandInformation[KEY_PREVIOUS_STAND_WIDTH], selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT]);
//                            Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_WIDTH], selectedStandInformation[KEY_CURRENT_STAND_HEIGHT]);
                            Grid.QueueDraw();
                        }
                    }
                    catch (OverflowException)
                    {
                        //assume the user dragged the stand outside of the grid and remove it
                        Console.WriteLine("overflow exception caught");
                        EngineAPI.removeGrabbedStand();
                    }
                    isStandSelected = false;
                }
                else
                {
                    using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Unable to apply Stand here."))
                    {
                        md.Run();
                        md.Destroy();
                    }
                    Grid.QueueDraw();

                   // EngineAPI.deselectStand();
                   // isStandSelected = false;
                }
            }
        }
        isMousePressed = false;
    }

    #endregion

    #region Grid Drawing
    protected void GridExposeEvent(object o, ExposeEventArgs args)
    {
        using (Context context = Gdk.CairoHelper.Create(args.Event.Window))
        {
            switch (DrawType)
            {
                case (int)Enumerations.DrawType.StandDraw:
                    {
                        //CairoGrid.DrawTile(context, new PointD(k + args.Event.Region.Clipbox.Left, i + args.Event.Region.Clipbox.Top));
                        if (CairoGrid.BackdropPath.Length > 0)
                        {
                            CairoGrid.DrawBackdrop(context);
                        }
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.ExistingStandRedraw:
                    {
                        if (CairoGrid.BackdropPath.Length > 0)
                        {
                            CairoGrid.DrawBackdrop(context);
                        }
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.StandSelected:
                    {
                        //redraw the tiles and to maintain visibility of the Stand and grid lines
//                        CairoStand.DrawHighlight(context, args.Event.Region.Clipbox.X + 3, args.Event.Region.Clipbox.Y + 3);
//                        for (int i = 0; i < args.Event.Region.Clipbox.Height; i+=3)
//                        {
//                            for (int k = 0; k < args.Event.Region.Clipbox.Width; k+=3)
//                            {
//                                CairoGrid.DrawTile(context, new PointD(k + args.Event.Region.Clipbox.Left, i + args.Event.Region.Clipbox.Top));
//                            }
//                        }
                        if (CairoGrid.BackdropPath.Length > 0)
                        {
                            CairoGrid.DrawBackdrop(context);
                        }
                        CairoGrid.DrawGrid(context);

                        //just going to update statusbar for now
                        //metadataStatusBar.Push(0, "Height: " + EngineAPI.getSelectedStandHeight() + " | Width: " + EngineAPI.getSelectedStandWidth());
                        break;
                    }
                case (int)Enumerations.DrawType.StandUnselected:
                    {
                        for (int i = 0; i < selectedStandInformation[KEY_CURRENT_STAND_HEIGHT]; i++)
                        {
                            for (int k = 0; k < selectedStandInformation[KEY_CURRENT_STAND_WIDTH]; k+=5)
                            {
                                CairoGrid.DrawTile(context, new PointD(k + selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], i + selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y]));
                            }
                            progressbar1.PulseStep += i / selectedStandInformation[KEY_CURRENT_STAND_HEIGHT];
                        }
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.InitialGridDraw:
                    {
                        CairoGrid.BackdropPath = string.Empty;
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.GridLinesDraw:
                    {
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.GridLinesNoDraw:
                    {
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.BackdropChangeDraw:
                    {
                        CairoGrid.DrawBackdrop(context);
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Default option on expose");
                        break;
                    }
            }
        }

    }
    #endregion

    /// <summary>
    /// Determines whether or not to draw the actual grid lines on the mapping area.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected void gridToggleButton_OnClicked(object sender, EventArgs e)
    {
        if (CairoGrid.DrawLines)
        {
            CairoGrid.DrawLines = false;
            DrawType = (int)Enumerations.DrawType.GridLinesNoDraw;
            Grid.QueueDraw();
            if (CairoGrid.BackdropPath.Length > 0)
            {
                DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                Grid.QueueDraw();
            }
        }
        else
        {
            CairoGrid.DrawLines = true;
            DrawType = (int)Enumerations.DrawType.GridLinesDraw;
            Grid.QueueDraw();
            if (CairoGrid.BackdropPath.Length > 0)
            {
                DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                Grid.QueueDraw();
            }
        }
    }

    protected void backdropButton_OnClicked(object sender, EventArgs e)
    {
        FileChooserDialog backDropDialog = null;
        const string STR_DIALOG_SAVE_BUTTON_TEXT = "Open";
        const string STR_DIALOG_CANCEL_BUTTON_TEXT = "Cancel";
        string fileName = string.Empty; //holds user entered file name
        ResponseType response = ResponseType.None;

        try
        {
            backDropDialog = new FileChooserDialog(STR_NEWMAP_DIALOG_TITLE, this, FileChooserAction.Save);
            backDropDialog.AddButton(STR_DIALOG_SAVE_BUTTON_TEXT, ResponseType.Ok);
            backDropDialog.AddButton(STR_DIALOG_CANCEL_BUTTON_TEXT, ResponseType.Cancel);
            FileFilter mmgsFilter = new FileFilter();
            mmgsFilter.Name = "PNG";
            mmgsFilter.AddPattern("*.png");
            backDropDialog.AddFilter(mmgsFilter);
            backDropDialog.Run();
            response = (ResponseType)backDropDialog.Run();
            fileName = backDropDialog.Filename;
        }
        finally
        {
            if (backDropDialog != null)
            {
                backDropDialog.Destroy(); //cleanup
            }
        }

        switch (response)
        {
            case ResponseType.Ok:
                {
                    if (System.IO.File.Exists(fileName))
                    {
                        CairoGrid.BackdropPath = fileName;
                        DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                        Grid.QueueDraw();
                    }
                    else
                    {
                        using(MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                            string.Format("Backdrop file not found.")))
                        {
                            md.Run();
                            md.Destroy();
                        }
                    }
                    break;
                }
            default:
                {
                    Console.WriteLine("Cancel clicked on back drop button dialog.");
                    break;
                }
        }
       
    }
    #endregion

    #region MainWindow Events

    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

    /// <summary>
    /// When the user presses escape, deselect the currently selected stand in the grid if there is one
    /// </summary>
    /// <param name="o">O.</param>
    /// <param name="args">Arguments.</param>
    protected void MainWindow_OnKeyPress (object o, KeyPressEventArgs args)
    {
        if(args.Event.Key.Equals("Escape"))
        {
            Console.WriteLine("Esc hit");
        }
    }

    #endregion
        
    #region File Operation Button Events
    protected void saveButton_Clicked(object sender, EventArgs e)
    {
        if (AppState.IsUIDirty)
        {
            //TODO - Save map - need call
        }
    }

    protected void newMapButton_Clicked(object sender, EventArgs e)
    {
        AppState.IsUIDirty = true;

        FileChooserDialog newMapSaveDialog = null;
        const string STR_DIALOG_SAVE_BUTTON_TEXT = "Save";
        const string STR_DIALOG_CANCEL_BUTTON_TEXT = "Cancel";
        string fileName = string.Empty; //holds user entered file name
        ResponseType response = ResponseType.None;

        try
        {
            newMapSaveDialog = new FileChooserDialog(STR_NEWMAP_DIALOG_TITLE, this, FileChooserAction.Save);
            newMapSaveDialog.AddButton(STR_DIALOG_SAVE_BUTTON_TEXT, ResponseType.Ok);
            newMapSaveDialog.AddButton(STR_DIALOG_CANCEL_BUTTON_TEXT, ResponseType.Cancel);
            FileFilter mmgsFilter = new FileFilter();
            mmgsFilter.Name = "Map";
            mmgsFilter.AddPattern("*.mmgs");
            newMapSaveDialog.AddFilter(mmgsFilter);
            response = (ResponseType)newMapSaveDialog.Run();
            fileName = newMapSaveDialog.Filename;
        }
        finally
        {
            if (newMapSaveDialog != null)
            {
                newMapSaveDialog.Destroy(); //cleanup
            }
        }

        switch (response)
        {
            case ResponseType.Ok:
                {
                    //TODO - Need an api call to complete this
                    Console.WriteLine("Done saving new Map - " + fileName + ".  Working to implement Saving to engine.");
                    System.IO.File.Create(fileName); //always overwrites.  empty dummy file 
                    if (System.IO.File.Exists(fileName))
                    {
                        string[] splits = fileName.Split(new string[]{ @"\" }, StringSplitOptions.None);
                        RefreshUI(splits[splits.Length - 1]); //get just the name not the path
                    }
                    break;
                }
            default:
                {
                    Console.WriteLine("Cancel clicked on New Map dialog.");
                    break;
                }
        }
    }

    /// <summary>
    /// Users selects a file from the File dialog
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected void fileChooserButton_FileSet(object sender, EventArgs e)
    {
        FileChooserButton btn = (FileChooserButton)sender;
        if (System.IO.File.Exists(btn.Filename))
        {
            string fileName = btn.Filename;
            EngineAPI.loadUserFile(fileName);
            string[] splits = fileName.Split(new string[]{ @"\" }, StringSplitOptions.None);
            RefreshUI(splits[splits.Length - 1]); //get just the name not the path
        }
    }

    #endregion

    #region Stand Button Events
    protected void newStandButton_Clicked(object sender, EventArgs e)
    {
        NewStandTemplateWindow newStandWindow = new NewStandTemplateWindow(store);
        newStandWindow.Show();
    }

    /// <summary>
    /// Rotate the selected stand 90 degrees clockwise if there is one in the grid
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected void rotateButton_Clicked(object sender, EventArgs args)
    {
        if (isStandSelected)
        {
            EngineAPI.rotateSelectedStand(true);

            //update keys
            selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] = selectedStandInformation[KEY_CURRENT_STAND_HEIGHT];
            selectedStandInformation[KEY_PREVIOUS_STAND_WIDTH] = selectedStandInformation[KEY_CURRENT_STAND_WIDTH];
            selectedStandInformation[KEY_CURRENT_STAND_HEIGHT] = (int)EngineAPI.getSelectedStandHeight();
            selectedStandInformation[KEY_CURRENT_STAND_WIDTH] = (int)EngineAPI.getSelectedStandWidth();

            CairoStand.Height = CairoStand.Width; //flip for rotation
            CairoStand.Width = CairoStand.Height;
            DrawType = (int)Enumerations.DrawType.ExistingStandRedraw;
            Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_WIDTH] + selectedStandInformation[KEY_CURRENT_STAND_HEIGHT], selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] + selectedStandInformation[KEY_CURRENT_STAND_WIDTH]);
            if (CairoGrid.BackdropPath.Length > 0)
            {
                DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_WIDTH] + selectedStandInformation[KEY_CURRENT_STAND_HEIGHT], selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] + selectedStandInformation[KEY_CURRENT_STAND_WIDTH]);
            }
        }
        else
        {
            using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                                         string.Format("Please select a Stand in the mapping area first.")))
            {
                md.Run();
                md.Destroy();
            }
        }
    }

    protected void removeStandButton_Clicked(object sender, EventArgs args)
    {
        if (isStandSelected)
        {
            EngineAPI.removeSelectedStand();
            DrawType = (int)Enumerations.DrawType.ExistingStandRedraw;
            Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_WIDTH] + selectedStandInformation[KEY_CURRENT_STAND_HEIGHT], selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] + selectedStandInformation[KEY_CURRENT_STAND_WIDTH]);
            isStandSelected = false;
            if (CairoGrid.BackdropPath.Length > 0)
            {
                DrawType = (int)Enumerations.DrawType.BackdropChangeDraw;
                Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_WIDTH] + selectedStandInformation[KEY_CURRENT_STAND_HEIGHT], selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] + selectedStandInformation[KEY_CURRENT_STAND_WIDTH]);
            }
        }
        else
        {
            using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                string.Format("Please select a Stand in the mapping area first.")))
            {
                md.Run();
                md.Destroy();
            }
        }
    }

    /// <summary>
    /// Deletes a stand template.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void deleteStandButton_Clicked(object sender, EventArgs args)
    {
        //TODO - need an api call to do this engine side - currently static
        Stand node = (Stand)view.NodeSelection.SelectedNode;
        if (node != null)
        {
            using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.YesNo, false,
                string.Format("Are you sure you want to delete " + node.Name + "?")))
            {
                int response =  md.Run();
                switch (response)
                {
                    case (int)ResponseType.Yes:
                        {
                            //delete the stand
                            view.NodeStore.RemoveNode(node);
                            break;
                        }
                    default:
                        {
                            //don't delete the stand
                            break;
                        }
                }
                md.Destroy();
            }
        }
    }

    protected void renameStandButton_Clicked(object sender, EventArgs args)
    {
        ITreeNode selectedNode = view.NodeSelection.SelectedNode;
        if (selectedNode != null)
        {
            Stand stand = (Stand)view.NodeSelection.SelectedNode;
            EngineAPI.setSTName(stand.StandID, stand.Name);
        }
    }       

    #endregion

    #region Stand Template Events
    protected void StandTemplateNodeSelectionChanged(object sender, EventArgs args)
    {
        NodeSelection selectedNode = (NodeSelection)sender;
        Stand node = (Stand)selectedNode.SelectedNode; 
        if (node != null)
        {
            metadataStatusBar.Push(0, "ID: " + node.StandID + " | " + node.Name + " | Color: (" + node.Color.R + ", " + node.Color.G + ", " + node.Color.B + ")");
            metadataStatusBar.TooltipText = "ID: " + node.StandID + " | " + node.Name + " | Color: (" + node.Color.R + ", " + node.Color.G + ", " + node.Color.B + ")";
        }
    }

    protected void StandTemplateNodeSelectionNodeChanged(object sender, KeyReleaseEventArgs args)
    {
        if (args.Event.KeyValue == 65293)
        {
            ITreeNode selectedNode = view.NodeSelection.SelectedNode;
            if (selectedNode != null)
            {
                Stand stand = (Stand)view.NodeSelection.SelectedNode;
                EngineAPI.setSTName(stand.StandID, stand.Name);
            }
        }
    }

    protected void StandTemplateSourceDragDataBegin(object sender, Gtk.DragBeginArgs args)
    {
        Console.WriteLine("Stand now being dragged.");
        NodeSelection selectedNode = (NodeSelection)((NodeView)sender).NodeSelection;
        Stand node = (Stand)selectedNode.SelectedNode;

        EngineAPI.grabNewStand(node.StandID);
        isStandSelected = true;
    }

    protected void StandTemplateSourceDragDataGet(object sender, Gtk.DragDataGetArgs args)
    {
        Console.WriteLine("Stand Drag data get");
        ITreeNode selectedNode = view.NodeSelection.SelectedNode;
        if (selectedNode != null)
        {
            Stand stand = (Stand)view.NodeSelection.SelectedNode;
            args.SelectionData.Text = stand.getPropertyString();
        }

    }

    protected void StandTemplateSourceDragDataEnd(object sender, Gtk.DragEndArgs args)
    {
        Console.WriteLine("Stand no longer being dragged.");
    }

    protected void CellEditedHandler(object sender, Gtk.EditedArgs args)
    {
        Stand node = (Stand)store.GetNode(new Gtk.TreePath(args.Path));
        node.Name = args.NewText;
    }

    #endregion

    #endregion
}

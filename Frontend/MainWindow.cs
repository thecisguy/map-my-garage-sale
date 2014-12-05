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
using Cairo;
using csapi;
using Frontend;
using Frontend.Map;
using Gtk;
using System;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{
    #region Private Members

    //UI text
    private const string STR_NEWMAP_TOOLTIP = "Create a new Map.";
    private const string STR_NEWMAP_BUTTON = "New Map";
    private const string STR_SAVEMAP_TOOLTIP = "Save the current Map.";
    private const string STR_SAVEMAP_BUTTON = "Save Sale";
    private const string STR_OPENMAP_TOOLTIP = "Open an existing Map";
    private const string STR_OPENMAP_BUTTON = "Open Sale";
    private const string STR_NEWSTAND_TOOLTIP = "Create a new Stand";
    private const string STR_NEWSTAND_BUTTON = "New Stand";
    private const string STR_ROTATESTAND_TOOLTIP = "Rotate the selected stand 90 degrees clockwise.";
    private const string STR_ROTATESTAND_BUTTON = "Rotate Stand 90°";
    private const string STR_DELETESTAND_TOOLTIP = "Delete the selected Stand Template permanently";
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
    private const string STR_WINDOWTITLE = "Map my Garage Sale - New*";
    private const string STR_NEWMAP_DIALOG_TITLE = "Create new Map";
    private const string STR_OPENMAP_DIALOG_TITLE = "Open Existing Map";
    private const string STR_MENU_BAR_NAME = "File";
    private const string STR_MENU_BAR_CHANGEBACKDROP_ITEM = "Change Backdrop";

    //UI resource paths
    private const string RES_ADDSTAND_ICON = "Frontend.Assets.addstandicon.png";
    private const string RES_NEWFILE_ICON = "Frontend.Assets.newfileicon.png";
    private const string RES_ROTATE_ICON = "Frontend.Assets.rotateicon.png";
    private const string RES_SAVE_ICON = "Frontend.Assets.saveicon.png";
    private const string RES_APPLICATION_ICON = "Assets/icon.ico";

    //selected stand keys
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
    private Frame StandFrame;
    private DrawingArea Grid;
    private NodeView StandTemplateNodeView;
    private NodeStore store;
    private Dictionary<string, int> selectedStandInformation;
    private bool isStandSelected = false;
    private int DrawType;
    private string curFileName = "untitled.mmgs";

    //Drag and Drop target values
    private enum TargetType
    {
        String,
        RootWindow}

    ;

    private static TargetEntry[] target_table = new TargetEntry []
    {
        new TargetEntry("STRING", 0, (uint)TargetType.String),
        new TargetEntry("text/plain", 0, (uint)TargetType.String),
        new TargetEntry("application/x-rootwindow-drop", 0, (uint)TargetType.RootWindow)
    };


    #endregion

    #region Constructor

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        SetupUI();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Completes UI initialization.
    /// </summary>
    private void SetupUI()
    {
        this.Title = STR_WINDOWTITLE;
        this.Icon = new Gdk.Pixbuf(RES_APPLICATION_ICON);
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
        Widget mapWidget = operationWidget.CreateOperationWidget(RES_NEWFILE_ICON, STR_NEWMAP_BUTTON);
        mapWidget.Show();
        Gtk.Button newMapButton = new Gtk.Button(mapWidget);
        newMapButton.Clicked += newMapButton_Clicked;
        newMapButton.TooltipText = STR_NEWMAP_TOOLTIP;
        newMapButton.Show();
        hboxNSO.PackStart(newMapButton, false, false, 0);

        //Save button
        Widget saveWidget = operationWidget.CreateOperationWidget(RES_SAVE_ICON, STR_SAVEMAP_BUTTON);
        saveWidget.Show();
        Gtk.Button saveButton = new Gtk.Button(saveWidget);
        saveButton.Clicked += saveButton_Clicked;
        saveButton.TooltipText = STR_SAVEMAP_TOOLTIP;
        saveButton.Show();
        hboxNSO.PackStart(saveButton, false, false, 0);

        //Open button
        FileChooserButton fileChooserButton = new FileChooserButton(STR_OPENMAP_BUTTON, FileChooserAction.Open);
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

        VSeparator middleSeparator = new VSeparator();
        hboxStand.Add(middleSeparator);

        #region Stand Buttons

        //New Stand button
        Widget newStandWidget = operationWidget.CreateOperationWidget(RES_ADDSTAND_ICON, STR_NEWSTAND_BUTTON);
        newStandWidget.Show();
        Gtk.Button newStandButton = new Gtk.Button(newStandWidget);
        newStandButton.Clicked += newStandButton_Clicked;
        newStandButton.TooltipText = STR_NEWSTAND_TOOLTIP;
        newStandButton.Show();
        hboxStand.PackStart(newStandButton, false, false, 6);

        //Rotate button
        Widget rotateWidget = operationWidget.CreateOperationWidget(RES_ROTATE_ICON, STR_ROTATESTAND_BUTTON);
        rotateWidget.Show();
        Gtk.Button rotateButton = new Gtk.Button(rotateWidget);
        rotateButton.Clicked += rotateButton_Clicked;
        rotateButton.TooltipText = STR_ROTATESTAND_TOOLTIP;
        rotateButton.Show();
        hboxStand.PackStart(rotateButton, false, false, 3);

        //Create a box to hold both the rotate button and existing button
        VBox rotateBox = new VBox(false, 6);

        //Remove selected stand
        Gtk.Button removeStandButton = new Gtk.Button(STR_REMOVESTAND_BUTTON);
        removeStandButton.Show();
        removeStandButton.Clicked += removeStandButton_Clicked;
        removeStandButton.TooltipText = STR_REMOVESTAND_TOOLTIP;
        rotateBox.PackStart(removeStandButton, false, false, 3);

        //Delete Widget setup
        Button deleteStandButton = new Button(STR_DELETESTAND_BUTTON);
        deleteStandButton.Show();
        deleteStandButton.Clicked += deleteStandButton_Clicked;
        deleteStandButton.TooltipText = STR_DELETESTAND_TOOLTIP;
        rotateBox.PackStart(deleteStandButton, false, false, 3);

        hboxStand.PackStart(rotateBox, false, false, 3);

        #endregion

        #region Grid and Grid Buttons and Labels

        //Add Toggle grid box
        HBox hboxToggle = new HBox();
        hboxToggle.Spacing = 6;
        MainTable.Add(hboxToggle);
        Table.TableChild w6 = ((Table.TableChild)(MainTable[hboxToggle]));
        w6.TopAttach = ((uint)(5));
        w6.BottomAttach = ((uint)(6));
        w6.XOptions = ((Gtk.AttachOptions)(2));
        w6.YOptions = ((Gtk.AttachOptions)(2));

        //ToggleButton for Grid on/of
        ToggleButton gridToggleButton = new ToggleButton(STR_TOGGLEGRID_BUTTON);
        gridToggleButton.Clicked += new EventHandler(gridToggleButton_OnClicked);
        gridToggleButton.Show();
        hboxToggle.PackStart(gridToggleButton, false, false, 3);

        //Change backdrop button
        Button backdropButton = new Button(STR_BACKDROP_BUTTON);
        backdropButton.Clicked += new EventHandler(backdropButton_OnClicked);
        backdropButton.Show();
        hboxToggle.PackStart(backdropButton, false, false, 3);

        InitializeGrid(true);

        #endregion

        #region Stand Templates
        //Stands Frame
        StandFrame = new Frame();
        StandFrame.TooltipText = STR_STANDFRAME_TOOLTIP;
        StandFrame.CanFocus = true;
        StandFrame.ShadowType = ((Gtk.ShadowType)(1));
        StandFrame.BorderWidth = ((uint)(1));
        StandFrame.Show();
        MainTable.Attach(StandFrame, 3, 4, 2, 5);

        InitializeStandTemplates(true);
        StandFrame.Add(StandTemplateNodeView);

        #endregion

        #region MenuBar setup
        MenuItem menuItem = new MenuItem(STR_MENU_BAR_NAME);
        Menu menu = new Menu();

        AccelGroup accelGroup = new AccelGroup();
        AddAccelGroup(accelGroup);

        ImageMenuItem newImageItem = new ImageMenuItem(Stock.New, accelGroup);
        newImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.n, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
        newImageItem.Activated += new EventHandler(NewImageItem_OnActivated);

        ImageMenuItem openImageItem = new ImageMenuItem(Stock.Open, accelGroup);
        openImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.o, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
        openImageItem.Activated += new EventHandler(OpenImageItem_OnActivated);

        ImageMenuItem saveImageItem = new ImageMenuItem(Stock.Save, accelGroup);
        saveImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.s, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
        saveImageItem.Activated += new EventHandler(SaveImageItem_OnActivated);

        MenuItem backdropImageItem = new MenuItem(STR_MENU_BAR_CHANGEBACKDROP_ITEM);
        backdropImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.s, Gdk.ModifierType.ShiftMask, AccelFlags.Visible));
        backdropImageItem.Activated += new EventHandler(BackdropImageItem_OnActivated);

        ImageMenuItem exitImageItem = new ImageMenuItem(Stock.Quit, accelGroup);
        exitImageItem.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.q, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
        exitImageItem.Activated += new EventHandler(ExitImageItem_OnActivated);

        menu.Append(newImageItem);
        menu.Append(openImageItem);
        menu.Append(saveImageItem);
        menu.Append(new SeparatorMenuItem());
        menu.Append(backdropImageItem);
        menu.Append(new SeparatorMenuItem());
        menu.Append(exitImageItem);
        menuItem.Submenu = menu;
        menubar.Append(menuItem);
        Table.TableChild menuChild = ((Table.TableChild)(MainTable[menubar]));
        menuChild.TopAttach = ((uint)(0));
        menuChild.BottomAttach = ((uint)(1));
        menuChild.RightAttach = ((uint)(3));
        #endregion
       
    }

    /// <summary>
    /// Setup and draw the grid.
    /// </summary>
    private void InitializeGrid(bool isFirstTime = false)
    {
        if (isFirstTime)
        {
            Grid = new DrawingArea();

            Grid.ExposeEvent += new ExposeEventHandler(GridExposeEvent);
            Grid.DragDataReceived += new DragDataReceivedHandler(GridDragDataReceived);
            Gtk.Drag.DestSet(Grid, DestDefaults.Drop | DestDefaults.Motion, target_table, Gdk.DragAction.Copy);
            Grid.AddEvents((int)
                (Gdk.EventMask.ButtonPressMask
            | Gdk.EventMask.ButtonReleaseMask
            | Gdk.EventMask.KeyPressMask
            | Gdk.EventMask.PointerMotionMask));
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

        try
        {
            uint height = EngineAPI.getMainGridHeight();
            uint width = EngineAPI.getMainGridWidth();
            CairoGrid.Height = height;
            CairoGrid.Width = width;

            //catch these so we can softly fail an app load where for some reason the engine is unable to be seen by the client
        }
        catch (MissingMethodException me)
        {
            Console.WriteLine("Missing method exception hit: \n" + me.Message + "\n\n" + me.StackTrace);
        }
        catch (System.Security.SecurityException se)
        {
            Console.WriteLine("SecurityException hit: \n" + se.StackTrace);
        }
        catch (Exception e)
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
            StandTemplateNodeView = new NodeView();

            Gtk.Drag.SourceSet(StandTemplateNodeView, Gdk.ModifierType.Button1Mask, target_table, Gdk.DragAction.Copy);
            StandTemplateNodeView.DragDataGet += new Gtk.DragDataGetHandler(StandTemplateSourceDragDataGet);
            StandTemplateNodeView.DragBegin += new Gtk.DragBeginHandler(StandTemplateSourceDragDataBegin);
            StandTemplateNodeView.NodeSelection.Changed += new System.EventHandler(StandTemplateNodeSelectionChanged);
            StandTemplateNodeView.KeyReleaseEvent += new KeyReleaseEventHandler(StandTemplateNodeSelectionNodeChanged);

            Gtk.CellRendererText editableCell = new Gtk.CellRendererText();
            editableCell.Editable = true;
            editableCell.Edited += new EditedHandler(CellEditedHandler);

            StandTemplateNodeView.AppendColumn("ID", new Gtk.CellRendererText(), "text", 0);
            StandTemplateNodeView.AppendColumn("Icon", new Gtk.CellRendererPixbuf(), "pixbuf", 1);
            StandTemplateNodeView.AppendColumn("Name", editableCell, "text", 2);
        }
        StandTemplateNodeView.NodeStore = LoadStandTemplates();
        StandTemplateNodeView.ShowAll();
    }

    /// <summary>
    /// Get all Stand Templates from the engine
    /// </summary>
    /// <returns>The stand templates.</returns>
    private NodeStore LoadStandTemplates()
    {
        //get stand templates from engine
        if (store != null && StandTemplateNodeView.NodeStore != null)
        {
            StandTemplateNodeView.NodeStore.Clear();
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
        return store;
    }

    /// <summary>
    /// Refreshs the map area and stand templates
    /// </summary>
    /// <param name="fileName">File name.</param>
    private void RefreshUI(string fileName = "")
    {
        this.Title = STR_WINDOWTITLE + fileName;
        InitializeGrid(false);
        InitializeStandTemplates(false);
    }

    /// <summary>
    /// Open an existing mmgs file.
    /// </summary>
    private void OpenMap()
    {
        FileChooserDialog openMapSaveDialog = null;
        const string STR_DIALOG_SAVE_BUTTON_TEXT = "Open";
        const string STR_DIALOG_CANCEL_BUTTON_TEXT = "Cancel";
        string fileName = string.Empty; //holds user entered file name
        ResponseType response = ResponseType.None;

        try
        {
            openMapSaveDialog = new FileChooserDialog(STR_OPENMAP_DIALOG_TITLE, this, FileChooserAction.Open);
            openMapSaveDialog.AddButton(STR_DIALOG_SAVE_BUTTON_TEXT, ResponseType.Ok);
            openMapSaveDialog.AddButton(STR_DIALOG_CANCEL_BUTTON_TEXT, ResponseType.Cancel);
            FileFilter mmgsFilter = new FileFilter();
            mmgsFilter.Name = "Map";
            mmgsFilter.AddPattern("*.mmgs");
            openMapSaveDialog.AddFilter(mmgsFilter);
            response = (ResponseType)openMapSaveDialog.Run();
            fileName = openMapSaveDialog.Filename;
        }
        finally
        {
            if (openMapSaveDialog != null)
            {
                openMapSaveDialog.Destroy(); //cleanup
            }
        }

        switch (response)
        {
            case ResponseType.Ok:
                {
                    //Need an api call to complete this
                    System.IO.File.Create(fileName); //always overwrites.  empty dummy file 
                    if (System.IO.File.Exists(fileName))
                    {
                        string[] splits = fileName.Split(new string[]{ @"\" }, StringSplitOptions.None);
                        EngineAPI.loadUserFile(fileName);
                        RefreshUI(splits[splits.Length - 1]); //get just the name not the path
                    }
                    this.curFileName = fileName;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void NewMap()
    {
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
                    //Need an api call to complete this
                    System.IO.File.Create(fileName); //always overwrites.  empty dummy file 
                    if (System.IO.File.Exists(fileName))
                    {
                        string[] splits = fileName.Split(new string[]{ @"\" }, StringSplitOptions.None);
                        EngineAPI.saveUserFile(fileName);
                        RefreshUI(splits[splits.Length - 1]); //get just the name not the path
                    }
                    this.curFileName = fileName;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void SaveMap()
    {
        EngineAPI.saveUserFile(this.curFileName);
    }

    /// <summary>
    /// Change the backdrop on the mapping area.
    /// </summary>
    private void ChangeBackdrop()
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
                        DrawType = (int)Enumerations.DrawType.GridRedraw;
                        Grid.QueueDraw();
                    }
                    else
                    {
                        using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
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
                    break;
                }
        }
    }

    #endregion

    #region Control Events

    #region Menubar Events

    /// <summary>
    /// Create New map .
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void NewImageItem_OnActivated(object sender, EventArgs args)
    {
        NewMap();
    }

    /// <summary>
    /// Open existing map
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void OpenImageItem_OnActivated(object sender, EventArgs args)
    {
        OpenMap();
    }

    /// <summary>
    /// Save map.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void SaveImageItem_OnActivated(object sender, EventArgs args)
    {
        SaveMap();
    }

    /// <summary>
    /// Change backdrop
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void BackdropImageItem_OnActivated(object sender, EventArgs args)
    {
        ChangeBackdrop();
    }

    /// <summary>
    /// Exit the application.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void ExitImageItem_OnActivated(object sender, EventArgs args)
    {
        Application.Quit();
    }

    #endregion

    #region Grid Events

    #region Grid Drag and Drop

    /// <summary>
    /// Drag data received from the NodeView (Stand Template)
    /// </summary>
    /// <param name="o">O.</param>
    /// <param name="args">Arguments.</param>
    protected void GridDragDataReceived(object sender, DragDataReceivedArgs args)
    {
        Stand stand = new Stand(args.SelectionData.Text);

        //can Stand be placed here
        if (EngineAPI.canApplyGrabbedStand((long)args.Y, (long)args.X) && isStandSelected)
        {
            EngineAPI.doApplyGrabbedStand();
        }
        else
        {
            using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Unable to apply Stand here."))
            {
                md.Run();
                md.Destroy();
            }
        }
        DrawType = (int)Enumerations.DrawType.GridRedraw;
        Grid.QueueDraw();
        isStandSelected = false;
    }

    protected void GridButtonPress(object sender, ButtonPressEventArgs args)
    {
        //left mouse button
        if (args.Event.Button == 1)
        {
            long currentSelectedStandOriginX = 0;
            long currentSelectedStandOriginY = 0;

            //did user click on a stand
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

                DrawType = (int)Enumerations.DrawType.GridRedraw;
                Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], CairoStand.Width, CairoStand.Height);
            }
            else
            {
                if (isStandSelected)
                {
                        isStandSelected = false;
                        DrawType = (int)Enumerations.DrawType.GridRedraw;
                        EngineAPI.deselectStand();
                        metadataStatusBar.Push(0, "No Stand selected.");
                        Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], 
                            selectedStandInformation[KEY_CURRENT_STAND_WIDTH], selectedStandInformation[KEY_CURRENT_STAND_HEIGHT]);
                }
            }
        }
    }

    protected void GridButtonRelease(object sender, ButtonReleaseEventArgs args)
    {
        //left mouse button
        //check to see that the stand has been moved a significant amount
        if (args.Event.Button == 1 && isStandSelected)
        {
            bool checkMoveX = args.Event.X > selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_X] + 10 || args.Event.X < selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_X] - 10;
            bool checkMoveY = args.Event.Y > selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_Y] + 10 || args.Event.Y < selectedStandInformation[KEY_CURRENT_STAND_START_DRAG_Y] - 10;

            if (checkMoveX && checkMoveY)
            {
                EngineAPI.grabSelectedStand();
                bool canApplyStand = EngineAPI.canApplyGrabbedStand((long)args.Event.Y, (long)args.Event.X);
                Console.WriteLine(canApplyStand);
                if (canApplyStand)
                {
                    EngineAPI.doApplyGrabbedStand();

                    //update keys
                    selectedStandInformation[KEY_PREVIOUS_STAND_ORIGIN_X] = selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X];
                    selectedStandInformation[KEY_PREVIOUS_STAND_ORIGIN_Y] = selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y];
                    selectedStandInformation[KEY_PREVIOUS_STAND_WIDTH] = selectedStandInformation[KEY_CURRENT_STAND_WIDTH];
                    selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] = selectedStandInformation[KEY_CURRENT_STAND_HEIGHT];

                    //we now have stand at new coords as well as the origin points for it post-selection
                    selectedStandInformation[KEY_CURRENT_STAND_WIDTH] = (int)EngineAPI.getSelectedStandWidth();
                    selectedStandInformation[KEY_CURRENT_STAND_HEIGHT] = (int)EngineAPI.getSelectedStandHeight();

                    DrawType = (int)Enumerations.DrawType.GridRedraw;
                    Grid.QueueDraw();
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
                }
            }
        }
    }

    #endregion

    #region Grid Drawing

    /// <summary>
    /// Mission Critical method for drawing.  Originally, many different enums with switch statements were used and partial redraws of the grid were calculated based on user
    /// movement in the mapping area.  However, after the design decision was made to have a 3 pixel per Tile ratio instead of a 1 pixel per Tile ratio the performance of drawing the grid
    /// in the application ceased to be an issue.  Draw performance was now at acceptable levels when the Grid was fully redrawn when necessary rather than calculating clips of it to redraw, 
    /// which often left artifacting.
    /// 
    /// Past versions of this method can be viewed at https://github.com/thecisguy/map-my-garage-sale, but it has been greatly simplified since.
    /// 
    /// This is still where all drawing physically takes place.
    /// 
    /// Fired when the drawable is exposed to the UI (window created, queued up manually, resized/minimized dialog)
    /// </summary>
    /// <param name="o">O.</param>
    /// <param name="args">Arguments.</param>
    protected void GridExposeEvent(object sender, ExposeEventArgs args)
    {
        //create a context from the event window - in this case the drawing area
        using (Context context = Gdk.CairoHelper.Create(args.Event.Window))
        {
            //draw the backdrop first if set
            if (CairoGrid.BackdropPath.Length > 0)
            {
                CairoGrid.DrawBackdrop(context);
            }

            switch (DrawType)
            {
                case (int)Enumerations.DrawType.GridRedraw:
                    {
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                case (int)Enumerations.DrawType.InitialGridDraw:
                    {
                        CairoGrid.BackdropPath = string.Empty;
                        CairoGrid.DrawGrid(context);
                        break;
                    }
                default:
                    {
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
            DrawType = (int)Enumerations.DrawType.GridRedraw;
            Grid.QueueDraw();
            if (CairoGrid.BackdropPath.Length > 0)
            {
                DrawType = (int)Enumerations.DrawType.GridRedraw;
                Grid.QueueDraw();
            }
        }
        else
        {
            CairoGrid.DrawLines = true;
            DrawType = (int)Enumerations.DrawType.GridRedraw;
            Grid.QueueDraw();
            if (CairoGrid.BackdropPath.Length > 0)
            {
                DrawType = (int)Enumerations.DrawType.GridRedraw;
                Grid.QueueDraw();
            }
        }
    }

    protected void backdropButton_OnClicked(object sender, EventArgs e)
    {
        ChangeBackdrop();       
    }

    #endregion

    #region MainWindow Events

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    /// <summary>
    /// When the user presses escape, deselect the currently selected stand in the grid if there is one
    /// </summary>
    /// <param name="o">O.</param>
    /// <param name="args">Arguments.</param>
    protected void MainWindow_OnKeyPress(object sender, KeyPressEventArgs args)
    {
        if (args.Event.Key.Equals("Escape") && isStandSelected)
        {
            EngineAPI.deselectStand();
            isStandSelected = false;
        }
    }

    #endregion

    #region File Operation Button Events

    protected void saveButton_Clicked(object sender, EventArgs e)
    {
        SaveMap();
    }

    protected void newMapButton_Clicked(object sender, EventArgs e)
    {
        NewMap();
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
            this.curFileName = fileName;
            string[] splits = fileName.Split(new string[]{ @"\" }, StringSplitOptions.None);
            RefreshUI(splits[splits.Length - 1]); //get just the name not the path
        }
    }

    #endregion

    #region Stand Button Events

    protected void newStandButton_Clicked(object sender, EventArgs e)
    {
        NewStandTemplateWindow newStandWindow = new NewStandTemplateWindow(StandTemplateNodeView.NodeStore);
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
            DrawType = (int)Enumerations.DrawType.GridRedraw;
            Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], 
                selectedStandInformation[KEY_CURRENT_STAND_WIDTH] + selectedStandInformation[KEY_CURRENT_STAND_HEIGHT], 
                selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] + selectedStandInformation[KEY_CURRENT_STAND_WIDTH]);
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
            DrawType = (int)Enumerations.DrawType.GridRedraw;
            Grid.QueueDrawArea(selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_X], selectedStandInformation[KEY_CURRENT_STAND_ORIGIN_Y], 
                selectedStandInformation[KEY_CURRENT_STAND_WIDTH] + selectedStandInformation[KEY_CURRENT_STAND_HEIGHT], 
                selectedStandInformation[KEY_PREVIOUS_STAND_HEIGHT] + selectedStandInformation[KEY_CURRENT_STAND_WIDTH]);
            isStandSelected = false;
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
        //need an api call to do this engine side - currently static
        Stand node = (Stand)StandTemplateNodeView.NodeSelection.SelectedNode;
        if (node != null)
        {
            using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.YesNo, false,
                                          string.Format("Are you sure you want to delete " + node.Name + "?")))
            {
                int response = md.Run();
                switch (response)
                {
                    case (int)ResponseType.Yes:
                        {
                            //delete the stand
                            StandTemplateNodeView.NodeStore.RemoveNode(node);
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
        ITreeNode selectedNode = StandTemplateNodeView.NodeSelection.SelectedNode;
        if (selectedNode != null)
        {
            Console.WriteLine(selectedNode);
            Stand stand = (Stand)StandTemplateNodeView.NodeSelection.SelectedNode;
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
        //65293 is Enter key (not numpad one)
        if (args.Event.KeyValue == 65293)
        {
            ITreeNode selectedNode = StandTemplateNodeView.NodeSelection.SelectedNode;
            if (selectedNode != null)
            {
                Stand stand = (Stand)StandTemplateNodeView.NodeSelection.SelectedNode;
                EngineAPI.setSTName(stand.StandID, stand.Name);
            }
        }
    }

    /// <summary>
    /// A value in the Stand Template 'Name' cell has changed.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Arguments.</param>
    protected void CellEditedHandler(object sender, EditedArgs args)
    {
        Stand node = (Stand)StandTemplateNodeView.NodeStore.GetNode(new TreePath(args.Path));
        node.Name = args.NewText;
    }

    protected void StandTemplateSourceDragDataBegin(object sender, DragBeginArgs args)
    {
        NodeSelection selectedNode = (NodeSelection)((NodeView)sender).NodeSelection;
        Stand node = (Stand)selectedNode.SelectedNode;

        EngineAPI.grabNewStand(node.StandID);
        isStandSelected = true;
    }

    protected void StandTemplateSourceDragDataGet(object sender, DragDataGetArgs args)
    {
        ITreeNode selectedNode = StandTemplateNodeView.NodeSelection.SelectedNode;
        if (selectedNode != null)
        {
            Stand stand = (Stand)StandTemplateNodeView.NodeSelection.SelectedNode;
            args.SelectionData.Text = stand.getPropertyString();
        }
    }

    #endregion

    #endregion
}

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
    private const string STR_DELETESTAND_BUTTON = "Delete selected Stand";
    private const string STR_REMOVESTAND_TOOLTIP = "Remove the selected Stand from the Map";
    private const string STR_REMOVESTAND_BUTTON = "Remove selected Stand";
    private const string STR_RENAMESTAND_BUTTON = "Rename selected Stand";
    private const string STR_RENAMESTAND_TOOLTIP = "Rename the selected Stand.";
    private const string STR_TOGGLEGRID_BUTTON = "Display Grid";
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

    //UI members
    private HBox standsBox;
    private global::Gtk.HBox hboxToggle;
    private global::Gtk.Label metadataLabel;
    private Gtk.Frame StandFrame;
    private DrawingArea Grid;
    private NodeView view;
    private NodeStore store;
    private bool isRedraw = false;
    private bool isMousePressed = false;
    private List<CairoMouse> clicks;
    private int DrawType;


    /// <summary>
    /// Maintains a list of all stands.  Design for how this works is a WIP.
    /// </summary>
    private AppState AppState;

    enum TargetType {
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

        /**
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
        FileFilter mmgsFileFilter = new FileFilter();
        mmgsFileFilter.Name = "Maps";
        mmgsFileFilter.AddPattern("*.mmgs");
        FileFilter allFileFilter = new FileFilter();
        allFileFilter.Name = "All Files";
        allFileFilter.AddPattern("*");
        fileChooserButton.AddFilter(mmgsFileFilter);
        fileChooserButton.AddFilter(allFileFilter);
        fileChooserButton.FileSet += fileChooserButton_FileSet;
        hboxNSO.PackEnd(fileChooserButton, false, false, 0);

        VSeparator middleSeparator = new VSeparator ();
        hboxStand.Add (middleSeparator);

        //Stand options Widget setup
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

        //More ui stuff outside of stetic generation - this seems to be MUCH more stable than making changes in the designer

        // Toggle
        this.hboxToggle = new Gtk.HBox ();
        this.hboxToggle.Name = "hboxToggle";
        this.hboxToggle.Spacing = 6;
        this.MainTable.Add (this.hboxToggle);
        Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.MainTable [this.hboxToggle]));
        w6.TopAttach = ((uint)(5));
        w6.BottomAttach = ((uint)(6));
        w6.XOptions = ((Gtk.AttachOptions)(4));
        w6.YOptions = ((Gtk.AttachOptions)(4));

        // Metadata label
        this.metadataLabel = new global::Gtk.Label ();
        this.metadataLabel.Name = "metadataLabel";
        this.metadataLabel.LabelProp = STR_METADATA_LABEL;
        this.metadataLabel.Justify = ((Gtk.Justification)(1));
        this.MainTable.Add (this.metadataLabel);
        Gtk.Table.TableChild w4 = ((Gtk.Table.TableChild)(this.MainTable [this.metadataLabel]));
        w4.TopAttach = ((uint)(5));
        w4.BottomAttach = ((uint)(6));
        w4.LeftAttach = ((uint)(1));
        w4.RightAttach = ((uint)(3));
        w4.XOptions = ((Gtk.AttachOptions)(4));
        w4.YOptions = ((Gtk.AttachOptions)(4));

        //Grid VBox;
        this.vboxGrid = new Gtk.VBox ();
        this.vboxGrid.Name = "vboxGrid";
        this.vboxGrid.Spacing = 6;
        this.MainTable.Add (this.vboxGrid);
        Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.MainTable [this.vboxGrid]));
        w5.TopAttach = ((uint)(2));
        w5.BottomAttach = ((uint)(5));
        w5.RightAttach = ((uint)(3));
        w5.XOptions = ((Gtk.AttachOptions)(4));

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

        //ToggleButton for Grid on/of
        ToggleButton gridToggleButton = new ToggleButton (STR_TOGGLEGRID_BUTTON);
        gridToggleButton.Clicked += new EventHandler(gridToggleButton_OnClicked);
        gridToggleButton.Show ();
        hboxToggle.PackStart (gridToggleButton, false, false, 3);

        //Stands Frame
        StandFrame = new Gtk.Frame();
        StandFrame.TooltipText = STR_STANDFRAME_TOOLTIP;
        StandFrame.CanFocus = true;
        StandFrame.ShadowType = ((Gtk.ShadowType)(1));
        StandFrame.BorderWidth = ((uint)(1));
        StandFrame.Show();

        standsBox = new HBox(true, 1);
        standsBox.Show();
        //StandFrame.Add(standsBox);
        MainTable.Attach(StandFrame, 3, 4, 2, 5);

        InitializeGrid();
        InitializeStandTemplates();
    }

    /// <summary>
    /// Draws the grid.
    /// </summary>
    private void InitializeGrid()
    {
        //testing
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

        //TODO - load engine data here prior to showing so we aren't leaking exposes everywhere?
         //this has to set a static prop for a switch in event_expose

        uint height = 400u;
        uint width = 750u;

        CairoGrid.Height = height;
        CairoGrid.Width = width;

        try{
            height = EngineAPI.getMainGridHeight();
            width = EngineAPI.getMainGridWidth();
        }catch(MissingMethodException me)
        {
            //catch this temporarily 
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
        vboxGrid.Add(Grid); 
        vboxGrid.ShowAll();//expose fired here for grid

        //endtesting
    }

    private void InitializeStandTemplates()
    {
        //testing nodeview
        store = new NodeStore(typeof(Stand));
        store.AddNode(new Stand(0, "Toys", new Cairo.Color(.49584,.78561,.94151,1),  40, 70));
        store.AddNode(new Stand(1, "Movies", new Cairo.Color(.67854,.78561,.94151,1), 100, 90));
        store.AddNode(new Stand(2, "Books and CDs", new Cairo.Color(.228,.15611,.7561,1), 75, 90));
        store.AddNode(new Stand(3, "Vintage Action Figures", new Cairo.Color(.9843,.78561,.94151,1), 60, 40));
        view = new NodeView(store);

        Gtk.Drag.SourceSet(view, Gdk.ModifierType.Button1Mask, target_table, Gdk.DragAction.Copy);
        view.DragDataGet += new Gtk.DragDataGetHandler(StandTemplateSourceDragDataGet);
        view.DragBegin += new Gtk.DragBeginHandler(StandTemplateSourceDragDataBegin);
        view.DragEnd += new Gtk.DragEndHandler(StandTemplateSourceDragDataEnd);
        view.NodeSelection.Changed += new System.EventHandler(StandTemplateNodeSelectionChanged);

        view.AppendColumn("ID", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Icon", new Gtk.CellRendererPixbuf(), "pixbuf", 1);
        view.AppendColumn("Name", new Gtk.CellRendererText(), "text", 2);
//        view.AppendColumn("Height", new Gtk.CellRendererText(), "text", 3);
//        view.AppendColumn("Width", new Gtk.CellRendererText(), "text", 4);

        view.ShowAll();
        //end testing

        StandFrame.Add(view);
    }

    protected void GridDragDataReceived(object o, DragDataReceivedArgs args)
    {
        Console.WriteLine("Drag Data Received at: " + args.X + ", " + args.Y);
        Stand stand = new Stand(args.SelectionData.Text);

        //       bool canApplyStand = EngineAPI.canApplyGrabbedStand((uint)args.X, (uint)args.Y);
        //        if (canApplyStand)
        //        {
        //            EngineAPI.doApplyGrabbedStand();
        //            grid.DrawGrid();
        //      }
        //       else
        //      {
        //          using (MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Unable to apply Stand here."))
        //           {
        //               md.Run();
        //               md.Destroy();
        //           }
        //        }

        CairoStand.Height = stand.Height;
        CairoStand.Width = stand.Width;
        CairoStand.Color = stand.Color;
        DrawType = (int)Enumerations.DrawType.NewStandDraw;
        int upLeftX = args.X - (CairoStand.Width / 2);  //TODO - these will be values from the selected stand
        int upLeftY = args.Y - (CairoStand.Height / 2);
        Grid.QueueDrawArea(upLeftX, upLeftY, CairoStand.Width, CairoStand.Height);
    }

    protected void GridDragDrop(object o, DragDropArgs args)
    {
        Console.WriteLine("Grid Drag Drop: " + args.X + ", " + args.Y);
    }

    protected void GridExposeEvent(object o, ExposeEventArgs args)
    {
        Console.WriteLine("Grid expose fired.");

        switch (DrawType)
        {
            case (int)Enumerations.DrawType.NewStandDraw:
                {
                    using (Context context = Gdk.CairoHelper.Create(args.Event.Window))
                    {
                        //EngineAPI.doApplyGrabbedStand();

                        //starting from upper left corner, increment the upLeft value width * height times and redraw grid
                        for (int i = 0; i < args.Event.Region.Clipbox.Height; i++)
                        {
                            for (int k = 0; k < args.Event.Region.Clipbox.Width; k++)
                            {
                                CairoGrid.DrawTile(context, new PointD(k + args.Event.Region.Clipbox.Left, i + args.Event.Region.Clipbox.Top));
                            }
                        }
                    }
                    break;
                }
            case (int)Enumerations.DrawType.InitialGridDraw:
                {
                    using (Context context = Gdk.CairoHelper.Create(args.Event.Window))
                    {
                        CairoGrid.BackdropPath = "testbackdrop.png";
                        CairoGrid.DrawGrid(context);
                    }
                    break;
                }
            default:
                {
                    Console.WriteLine("Default option on expose");
                    break;
                }
        }
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
            clicks.Add(new CairoMouse(new Point(Convert.ToInt32(args.Event.X), Convert.ToInt32(args.Event.Y))));
        }
    }

    protected void GridKeyPress(object o, KeyPressEventArgs args)
    {
        Console.WriteLine("Key press: " + args.Event.Key.ToString());
    }

    protected void GridButtonPress(object o, ButtonPressEventArgs args)
    {
        //TODO - did user click on a stand?
            //if yes, draw a highlighting line around the coords received by engine
        //EngineAPI.selectStand((uint)args.Event.X, (uint)args.Event.Y);

        Console.WriteLine("Button press: " + args.Event.Button.ToString());
        isMousePressed = true;
        if (clicks != null)
        {
            clicks.Clear();
        }
        else
        {
            clicks = new List<CairoMouse>();
        }
    }

    protected void GridButtonRelease(object o, ButtonReleaseEventArgs args)
    {
        //TODO - did user click outside of a stand?  deselect currently selected stand.  
        //TODO - Need a CairoSelector class to keep track of selected stand rectangle coords for redraws
        Console.WriteLine("Button release: " + args.Event.Button.ToString());
        isMousePressed = false;

        //this should not need to be done
//        if (clicks.Count > 0)
//        {
//            DrawType = (int)Enumerations.DrawType.NewStandDraw;
//            foreach (CairoMouse m in clicks)
//            {
//                Grid.QueueDrawArea(m.Point.X, m.Point.Y, 5, 5);
//            }
//        }
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
            newMapSaveDialog.Run();
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
                    //TODO - Save map data.  Need an api call to complete this
                    Console.WriteLine("Done saving new Map - " + fileName + ".  Working to implement Saving of object.");
                    break;
                }
            default:
                {
                    Console.WriteLine("Cancel clicked on New Map dialog.");
                    break;
                }
        }
    }

    protected void newStandButton_Clicked(object sender, EventArgs e)
    {
        NewStandTemplateDialog newStandDialog = null;
        ResponseType response = ResponseType.None;

        try
        {
            newStandDialog = new NewStandTemplateDialog(store);
            response = (ResponseType)newStandDialog.Run();
        }
        finally
        {
            if(newStandDialog != null)
            {
                newStandDialog.Destroy(); //cleanup
            }
        }

        switch (response)
        {
            case ResponseType.Ok: //set on the button's properties in the dialog
                {
                    Console.WriteLine("Done creating Stand.  Working to implement backend creation of new Stand object.");
                    //TODO - create stand in engine
                    break;
                }
            default:
                {
                    Console.WriteLine("Cancel clicked on New Stand dialog.");
                    break;
                }
        }
    }

    /// <summary>
    /// Rotate the selected stand 90 degrees clockwise if there is one in the grid
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected void rotateButton_Clicked(object sender, EventArgs args)
    {
        //EngineAPI.rotateSelectedStand(true);
    }

    protected void removeStandButton_Clicked(object sender, EventArgs args)
    {
        //EngineAPI.removeSelectedStand();
    }

    protected void deleteStandButton_Clicked(object sender, EventArgs args)
    {
        //TODO - need an api call to do this engine side - waiting on implementation
    }

    protected void renameStandButton_Clicked(object sender, EventArgs args)
    {
        ITreeNode selectedNode = view.NodeSelection.SelectedNode;
        if (selectedNode != null)
        {
            Stand stand = (Stand)view.NodeSelection.SelectedNode;
            //TODO - need an api call to do this - waiting on implementation
        }
    }        

    protected void StandTemplateNodeSelectionChanged(object sender, EventArgs args)
    {
        NodeSelection selectedNode = (NodeSelection)sender;
        Stand node = (Stand)selectedNode.SelectedNode;
        metadataLabel.Text = "Stand ID: " + node.StandID + " | Name: " + node.Name + " | Width: " + node.Width + "px | Height: " + node.Height + "px";
    }

    protected void StandTemplateSourceDragDataBegin(object sender, Gtk.DragBeginArgs args)
    {
        Console.WriteLine("Stand now being dragged.");
        isRedraw = true;
        //EngineAPI.grabNewStand(0);
    }

    protected void StandTemplateSourceDragDataGet(object sender, Gtk.DragDataGetArgs args)
    {
        Console.WriteLine("Stand Drag data get");
        ITreeNode selectedNode = view.NodeSelection.SelectedNode;
        if (selectedNode != null)
        {
            Stand stand = (Stand)view.NodeSelection.SelectedNode;
            args.SelectionData.Text = stand.getPropertyString();
            Console.WriteLine(args.SelectionData.Text);
        }

    }

    protected void StandTemplateSourceDragDataEnd(object sender, Gtk.DragEndArgs args)
    {
        Console.WriteLine("Stand no longer being dragged.");
    }

    void GridDragDropHandler(object o, DragDropArgs args)
    {
        Console.WriteLine("Grid Drag Dropped");
        Console.WriteLine("Stand dropped at (" + args.X + ", " + args.Y + ")");
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
            //TODO - refresh UI
        }
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
            EngineAPI.deselectStand();
        }
    }

    /// <summary>
    /// Determines whether or not to draw the actual grid lines on the mapping area.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    protected void gridToggleButton_OnClicked(object sender, EventArgs e)
    {
//        if (gridDrawingArea.DrawLines)
//        {
//            gridDrawingArea.DrawLines = false;
//        }
//        else
//        {
//            gridDrawingArea.DrawLines = true;
//        }
    }
    #endregion
}

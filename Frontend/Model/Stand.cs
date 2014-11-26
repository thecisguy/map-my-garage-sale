/* Stand.cs
 *
 * Contains the business properties for a Stand object and is used for display in the NodeView (treeview).
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
using Gdk;
using Cairo;

namespace Frontend
{
    [Gtk.TreeNode (ListOnly=true)]
    public class Stand: Gtk.TreeNode 
    {   
        [Gtk.TreeNodeValue (Column=0)]
        public int StandID {get; set;}

        [Gtk.TreeNodeValue (Column=1)]
        public string Name {get; set;}

        [Gtk.TreeNodeValue (Column=2)]
        public int Height {get; set;}

        [Gtk.TreeNodeValue (Column=3)]
        public int Width { get; set;}

        [Gtk.TreeNodeValue (Column=4)]
        public Pixbuf ThumbNail{get; set;}

        public Cairo.Color Color{get; set;}

        public Stand (int id, string name, Cairo.Color color, int width, int height)
        {
            this.StandID = id;
            this.Name = name;
            this.Color = color;
            this.Width = width;
            this.Height = height;
        }

        public Stand(string propertyString)
        {
            string[] properties = propertyString.Split(new string[]{";"}, StringSplitOptions.None);
            this.StandID = Convert.ToInt32(properties [0]);
            this.Name = properties[1];
            this.Color = new Cairo.Color(Convert.ToDouble(properties[2]), Convert.ToDouble(properties[3]), Convert.ToDouble(properties[4]), Convert.ToDouble(properties[5]));
            this.Width = Convert.ToInt32(properties [6]);
            this.Height = Convert.ToInt32(properties[7]);
        }

        public string getPropertyString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append (this.StandID.ToString () + ";");
            builder.Append (this.Name + ";");
            builder.Append (this.Color.R + ";" + this.Color.G + ";" + this.Color.B  + ";" + this.Color.A  + ";");
            builder.Append (this.Width.ToString () +";");
            builder.Append (this.Height.ToString ());
            //TODO - icon as base64?
            return builder.ToString ();
        }




    }
}


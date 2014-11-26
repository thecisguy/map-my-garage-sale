﻿/* CairoStand.cs
 *
 * Logic for drawing a Stand on the Grid drawingarea.    
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
using Gtk;
using System;

namespace Frontend.Map
{
    public static class CairoStand
    {
        public static int Height { get; set; }
        public static int Width { get; set; }
        public static Cairo.Color Color;

        #region Drawing Methods

        /// <summary>
        /// Draw a Stand on the context using the passed in Stand's information
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="X">X.</param>
        /// <param name="Y">Y.</param>
        /// <param name="stand">Stand.</param>
        public static void Draw(Context context, double X, double Y)
        {
            context.MoveTo(X, Y);
            context.SetSourceRGBA(Color.R, Color.G, Color.B, Color.A);
            context.LineWidth = 2;
            context.Rectangle(new Rectangle(X, Y, Width, Height));
            context.Fill();
            context.Stroke();
        }
        #endregion
    }
}


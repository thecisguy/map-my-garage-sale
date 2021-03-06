﻿/* CairoGrid.cs
 *
 * Logic for drawing the main grid area used for map designs.
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
    public static class CairoGrid
    {
        #region Properties

        public static uint Height { get; set; }
        public static uint Width { get; set; }
        public static string BackdropPath = string.Empty;
        public static bool DrawLines = true;

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Takes in width, height and calls draw method.  The Height and Width are incremented by 3 instead of a 1:1 ratio due to performance issues when drawing to the grid.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static void DrawGrid(Context context)
        {

            for (int countHeight = 0; countHeight < Height; countHeight+=3)
            {
                for (int countWidth = 0; countWidth < Width; countWidth+=3)
                {
                    DrawTile(context, new PointD(countWidth, countHeight));
                }
            }
            if (DrawLines)
            {
                DrawGridLines(context);
            }
        }

        /// <summary>
        /// Scales the created pixbuf to match the grid's dimensions and then draws it behind the mapping area via Cairo.ImageSurface.  
        /// </summary>
        /// <param name="context">Context.</param>
        public static void DrawBackdrop(Context context)
        {
            //conversion to signed integer needed for scaling
            using (Gdk.Pixbuf pb = new Gdk.Pixbuf(BackdropPath).ScaleSimple(Convert.ToInt32(Width), Convert.ToInt32(Height), Gdk.InterpType.Bilinear))
            {
                try
                {
                    pb.Save("temp", "png");

                    //paint the image and then the tiles
                    using (ImageSurface surface = new ImageSurface("temp"))
                    {
                        context.SetSource(surface);
                        context.Paint();
                    }
                }
                catch (Exception)
                {
                    using (MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                                                  string.Format("Unable to save a scaled copy of the backdrop image")))
                    {
                        md.Run();
                        md.Destroy();
                    }
                }
            }
        }

        #region Grid Lines

        /// <summary>
        /// Draws actual grid lines on the mapping area.
        /// </summary>
        public static void DrawGridLines(Context context)
        {
            //vertical grid lines
            for (int countHeight = 0; countHeight < Height; countHeight++)
            {
                for (int countWidth = 0; countWidth < Width; countWidth += 50)
                {
                    DrawVerticalLine(context, new PointD(countWidth, countHeight));
                }
                context.ClosePath();
                context.Stroke();
            }

            //horizontal grid lines
            for (int countWidth = 0; countWidth < Width; countWidth++)
            {
                for (int countHeight = 0; countHeight < Height; countHeight += 50)
                {
                    DrawHorizontalLine(context, new PointD(countWidth, countHeight));
                }
                context.ClosePath();
                context.Stroke();
            }
        }

        /// <summary>
        /// Draws a line from the current point to 5 pixels past that point
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="point">Point.</param>
        public static void DrawVerticalLine(Context context, PointD point)
        {
            context.Antialias = Antialias.Default;
            context.SetSourceRGBA(0.9, 0.5, 0.1, 0.5);
            context.LineCap = LineCap.Square;
            context.LineWidth = 0.2;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X + 5, point.Y);
        }

        /// <summary>
        /// Draws a line from the current point to 5 pixels beneath that point.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="point">Point.</param>
        public static void DrawHorizontalLine(Context context, PointD point)
        {
            context.Antialias = Antialias.Default;
            context.SetSourceRGBA(0.9, 0.5, 0.1, 0.5);
            context.LineCap = LineCap.Square;
            context.LineWidth = 0.2;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X, point.Y + 5);
        }

        #endregion

        #region Draw Tiles

        /// <summary>
        /// Takes in color and starting point and draws a line.  This line is a Tile that the engine has loaded.
        /// The current point + 3 pixels is used to ensure adequate performance of drawing.  
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="point">Point.</param>
        public static void DrawTile(Context context, PointD point)
        {
            Cairo.Color color = EngineAPI.getColorOfTile((uint)point.Y, (uint)point.X); 

            context.Antialias = Antialias.None;
            context.SetSourceRGBA(color.R, color.G, color.B, 0.9);
            context.LineCap = LineCap.Round;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X + 3, point.Y + 3);
            context.ClosePath();
            context.Stroke();
        }

        #endregion

        #endregion
    }
}


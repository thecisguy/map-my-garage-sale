/* CairoGraphic.cs
 *
 * Contains helper methods for drawing Cairo graphics.
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
    public class CairoGraphic : DrawingArea, IDisposable
	{

        #region Public Properties
        public double X {get; set;}
        public double Y {get; set;}
        public double Height { get; set; }
        public double Width { get; set; }
        public Gdk.Window Window { get; set; }
        #endregion

		public CairoGraphic () 
		{
        }

        public CairoGraphic(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            DrawCurvedRectangle(args.Window, this.X + 15, this.Y + 25, this.Width, this.Height);
            return true;
        }


        private double min (params double[] arr)
        {
            int minp = 0;
            for (int i = 1; i < arr.Length; i++)
            {
                if(arr[i] < arr[minp])
                {
                    minp = i;
                }
            }
            return minp;
        }

        /// <summary>
        /// Draws a curved rectangle using the passed in Gdk.Window.  This 'paints' to 'this' since we
        /// are inheriting from DrawingArea
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void DrawCurvedRectangle(Gdk.Window window, double x, double y, double width, double height)
        {
            using (Cairo.Context context = Gdk.CairoHelper.Create(window))
            {
                    context.Save();
                    context.MoveTo(x, y + height / 2);
                    context.CurveTo(x, y, x, y, x + width / 2, y);
                    context.CurveTo(x + width, y, x + width, y, x + width, y + height / 2);
                    context.CurveTo(x + width, y + height, x + width, y + height, x + width / 2, y + height);
                    context.CurveTo(x, y + height, x, y + height, x, y + height / 2);
                    context.SetSourceRGBA(0.1, 0.6, 1, 1);
                    context.FillPreserve();
                    context.SetSourceRGBA(0.2, 0.8, 1, 1);
                    context.LineWidth = 5;
                    context.Stroke();
                    //context.Restore();
            }
        }
		/// <summary>
		/// This method is basically a placeholder to draw a grid image while I work to draw a fully implemented
		/// Grid using the engine.
		/// </summary>
		public static void drawGrid ()
		{
			ImageSurface surface = new ImageSurface(Format.ARGB32, 650, 400);
			Context cr = new Context(surface);
			cr.SetSourceRGB (0.9, 0.6, 0.2);

			//horizontal lines
			cr.MoveTo (0, 80);
			cr.LineTo (650, 80);
			cr.Stroke ();

			cr.MoveTo (0, 200);
			cr.LineTo (650, 200);
			cr.Stroke ();

			cr.MoveTo (0, 320);
			cr.LineTo (650, 320);
			cr.Stroke ();

			//vertical lines
			cr.MoveTo (215, 0);
			cr.LineTo (215, 400);
			cr.Stroke ();

			cr.MoveTo (415, 0);
			cr.LineTo (415, 400);
			cr.Stroke ();

			Rectangle borderRect = new Rectangle (0, 0, 650, 400);
			cr.Rectangle (borderRect);
			cr.SetSourceRGB (0, 0, 0);
			cr.Stroke ();

			surface.WriteToPng("grid.png");
			surface.Dispose (); //dump surface properly
		}



		public virtual void Dispose(bool disposing)
		{
			if (disposing) 
			{
				//nothing yet since there are no unmanaged resources
			}
		}
	}

}


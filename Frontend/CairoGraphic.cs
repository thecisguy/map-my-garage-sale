using System;
using Gtk;
using Cairo;

namespace Frontend
{
	public class CairoGraphic : DrawingArea
	{
		public CairoGraphic () 
		{}

		public static void draw ()
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
		}
	}
}


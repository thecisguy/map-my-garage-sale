using Cairo;
using Gtk;
using System;

namespace Frontend
{
    public class CairoGrid : DrawingArea, IDisposable
    {
        public CairoGrid()
        {
        }

        /// <summary>
        /// Takes in width, height and calls draw method
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void DrawGrid(Gdk.Window window, uint width, uint height)
        {
            Gdk.Pixbuf pb = new Gdk.Pixbuf("testbackdrop.png");
            pb = pb.ScaleSimple(600, 400, Gdk.InterpType.Bilinear);
            pb.Save("temp", "png");

            //paint the image and then the tiles
            using (ImageSurface surface = new ImageSurface("temp"))
            {

                using (Context context = Gdk.CairoHelper.Create(window))
                {
                    context.Source = new Pattern(surface);
                    context.Paint();

                    for (int countHeight = 0; countHeight < height; countHeight++)
                    {
                        for (int countWidth = 0; countWidth < width; countWidth++)
                        {
                            DrawTile(context, new PointD(countWidth, countHeight));
                        }
                    }
                }

            }
 

        }

        /// <summary>
        /// Takes in color and starting point and draws a line.  This line is a
        /// Tile that the engine has loaded.  
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="point">Point.</param>
        private void DrawTile(Context context, PointD point)
        {

            Cairo.Color color = new Cairo.Color(0.8, 0.8, 0.8, 0.1); //spoofing color no engine call yet

            context.Antialias = Antialias.None;
            context.LineWidth = 4;
            context.SetSourceRGBA(color.R, color.G, color.B, color.A);
            context.LineCap = LineCap.Square;
            context.MoveTo(point);

            PointD padPoint = new PointD(point.X + 10, point.Y + 10);
            context.LineTo(padPoint);
            context.Stroke();
        }

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            DrawGrid(args.Window, 600u, 500u); 
            return true;
        }
    }
}


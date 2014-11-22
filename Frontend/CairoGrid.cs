using Cairo;
using csapi;
using Gtk;
using System;

namespace Frontend
{
    public class CairoGrid : DrawingArea, IDisposable
    {
        public uint Height { get; set; }
        public uint Width { get; set; }
        public string BackdropPath { get; set;}

        public CairoGrid()
        {}

        #region Drawing Methods

        /// <summary>
        /// Takes in width, height and calls draw method
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void DrawGrid(Gdk.Window window)
        {
            if (BackdropPath.Length > 0)
            {

                //conversion to signed integer needed for scaling
                using (Gdk.Pixbuf pb = new Gdk.Pixbuf("testbackdrop.png").ScaleSimple(Convert.ToInt32(Width), Convert.ToInt32(Height), Gdk.InterpType.Bilinear))
                {
                    try
                    {
                        pb.Save("temp", "png");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Unable to save a scaled copy of the backdrop image");
                    }

                    //paint the image and then the tiles
                    using (ImageSurface surface = new ImageSurface("temp"))
                    {
                        using (Context context = Gdk.CairoHelper.Create(window))
                        {
                            context.SetSource(new SurfacePattern(surface));
                            context.Paint();
                            Draw(window);
                        }
                    }
                }
            }
            else
            {
                Draw(window);  
            }
        }

        private void Draw(Gdk.Window window)
        {
            using (Context context = Gdk.CairoHelper.Create(window))
            {
                for (int countHeight = 0; countHeight < Height; countHeight++)
                {
                    for (int countWidth = 0; countWidth < Width; countWidth++)
                    {
                        DrawTile(context, new PointD(countWidth, countHeight));
                    }
                }
                context.ClosePath();
                context.Stroke();
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
            Cairo.Color color = EngineAPI.getColorOfTile((uint) point.Y,(uint) point.X); //spoofing color no engine call yet
Console.WriteLine(point.Y + ":" + point.X + ":" + color.R + ":" + color.G + ":" + color.B + ":" + color.A); 

            context.Antialias = Antialias.None;
            context.SetSourceRGBA(color.R, color.G, color.B, color.A);
            context.LineCap = LineCap.Square;
            context.LineTo(point.X, point.Y);
        }

        #endregion

        #region Control Events

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            DrawGrid(args.Window); 
            return true;
        }

        #endregion
    }
}


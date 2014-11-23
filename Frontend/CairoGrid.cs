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
        public string BackdropPath { get; set; }
        private Gdk.Window window;
        public bool DrawLines = true;

        public CairoGrid()
        {
        }

        #region Drawing Methods

        /// <summary>
        /// Takes in width, height and calls draw method
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void DrawGrid()
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
                            Draw(context);

                            if (DrawLines)
                            {
                                DrawGridLines(context);
                            }
                        }
                    }
                }
            }
            else
            {
                Draw();  
            }
        }

        /// <summary>
        /// Used when user wants a backdrop.  Saves creating a new context for no reason.
        /// </summary>
        /// <param name="context">Context.</param>
        private void Draw(Context context)
        {
            for (int countHeight = 0; countHeight < Height; countHeight++)
            {
                for (int countWidth = 0; countWidth < Width; countWidth++)
                {
                    DrawTile(context, new PointD(countWidth, countHeight));
                }
                context.ClosePath();
                context.Stroke();
            }   
        }


        /// <summary>
        /// Overload for when user wants no backdrop.  
        /// </summary>
        private void Draw()
        {
            using(Context context = Gdk.CairoHelper.Create(window))
            {
                for (int countHeight = 0; countHeight < Height; countHeight++)
                {
                    for (int countWidth = 0; countWidth < Width; countWidth++)
                    {
                        DrawTile(context, new PointD(countWidth, countHeight));
                    }
                    context.ClosePath();
                    context.Stroke();
                }   
            }
        }

        /// <summary>
        /// Draws actual grid lines on the mapping area
        /// </summary>
        private void DrawGridLines(Context context)
        {
            //vertical grid lines
            for (int countHeight = 0; countHeight < Height; countHeight++)
            {
                for (int countWidth = 0; countWidth < Width; countWidth+= 50)
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

        private void DrawVerticalLine(Context context, PointD point)
        {
            context.Antialias = Antialias.Default;
            context.SetSourceRGBA(0.9, 0.5, 0.1, 0.9);
            context.LineCap = LineCap.Square;
            context.LineWidth = 0.3;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X + 5, point.Y);
        }

        private void DrawHorizontalLine(Context context, PointD point)
        {
            context.Antialias = Antialias.Default;
            context.SetSourceRGBA(0.9, 0.5, 0.1, 0.9);
            context.LineCap = LineCap.Square;
            context.LineWidth = 0.3;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X, point.Y+5);
        }

        /// <summary>
        /// Takes in color and starting point and draws a line.  This line is a
        /// Tile that the engine has loaded.  
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="point">Point.</param>
        private void DrawTile(Context context, PointD point)
        {
            Cairo.Color color = new Cairo.Color(0, 0, 0, 0.1); //EngineAPI.getColorOfTile((uint)point.Y, (uint)point.X); //spoofing color no engine call yet

            context.Antialias = Antialias.None;
            context.SetSourceRGBA(color.R, color.G, color.B, color.A);
            context.LineCap = LineCap.Square;
            context.LineTo(point.X, point.Y);
        }

        #endregion

        #region Control Events

        protected override bool OnExposeEvent(Gdk.EventExpose args)
        {
            window = args.Window;
            DrawGrid(); 
            return true;
        }

        #endregion
    }
}


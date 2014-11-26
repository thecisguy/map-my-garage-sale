using Cairo;
using csapi;
using Gtk;
using System;

namespace Frontend
{
    public static class CairoGrid 
    {
        public static uint Height { get; set; }
        public static uint Width { get; set; }
        public static string BackdropPath = string.Empty;
        public static bool DrawLines = true;
        public static ImageSurface mainSurface;


        #region Drawing Methods

        /// <summary>
        /// Takes in width, height and calls draw method
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static void DrawGrid(Context context)
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
                            context.SetSource(surface);
                            context.Paint();
                            Draw(context);

                            if (DrawLines)
                            {
                                DrawGridLines(context);
                            }
                    }
                }
            }
            else
            {
                Draw(context); 
                if (DrawLines)
                {
                    DrawGridLines(context);
                }
            }
        }

        /// <summary>
        /// Used when user wants a backdrop.  Saves creating a new context for no reason.
        /// </summary>
        /// <param name="context">Context.</param>
        public static void Draw(Context context)
        {
            //context.SetSource(mainSurface);

                for (int countHeight = 0; countHeight < Height; countHeight++)
                {
                    for (int countWidth = 0; countWidth < Width; countWidth++)
                    {
                        DrawTile(context, new PointD(countWidth, countHeight));
                    }
                }

        }


        /// <summary>
        /// Draws actual grid lines on the mapping area
        /// </summary>
        public static void DrawGridLines(Context context)
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

        public static void DrawVerticalLine(Context context, PointD point)
        {
            context.Antialias = Antialias.Default;
            context.SetSourceRGBA(0.9, 0.5, 0.1, 0.9);
            context.LineCap = LineCap.Square;
            context.LineWidth = 0.3;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X + 5, point.Y);
        }

        public static void DrawHorizontalLine(Context context, PointD point)
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
        private static void DrawTile(Context context, PointD point)
        {
            Cairo.Color color1 = new Cairo.Color(0, 0, 0, 1);

            Cairo.Color color = color1;//EngineAPI.getColorOfTile((uint) point.Y,(uint) point.X); //spoofing color no engine call yet
//Console.WriteLine(point.Y + ":" + point.X + ":" + color.R + ":" + color.G + ":" + color.B + ":" + color.A); 

            context.Antialias = Antialias.None;
            context.SetSourceRGBA(color.R, color.G, color.B, color.A);
            context.LineCap = LineCap.Round;
            context.MoveTo(point.X, point.Y);
            context.LineTo(point.X, point.Y);
            context.ClosePath();
            context.Stroke();

        }

        #endregion
    }
}


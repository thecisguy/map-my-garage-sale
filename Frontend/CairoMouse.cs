using System;
using Gtk;
using Cairo;

namespace Frontend
{
    public class CairoMouse
    {
        public Point Point;

        public CairoMouse(Point point)
        {
            this.Point = point;
        }

        public void Draw(Context context)
        {
            context.MoveTo(this.Point.X, this.Point.Y);
            context.SetSourceRGBA(.65846, .159641, .684, 1);
            context.LineWidth = 1;
            context.LineTo(this.Point.X, this.Point.Y);
            context.Stroke();

        }
    }
}


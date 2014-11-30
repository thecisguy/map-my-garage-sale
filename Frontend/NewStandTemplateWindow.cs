using System;

namespace Frontend
{
    public partial class NewStandTemplateWindow : Gtk.Window
    {
        public NewStandTemplateWindow() :
            base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
    }
}


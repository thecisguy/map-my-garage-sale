using System;
using Gtk;

namespace Frontend
{
    public partial class NewStandTemplateWindow : Gtk.Window
    {
        #region Private Members
        private NodeStore templateStore;
        #endregion

        #region Constructor
        public NewStandTemplateWindow(NodeStore store) : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            this.templateStore = store;

            cancelBtn.Hide();  //for now they can just click 'X'
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Make sure all fields are properly filled in
        /// </summary>
        private bool Validate()
        {
            bool retVal = false;
            string name = nameEntry.Text.Trim();
            Gdk.Color color = standColorButton.Color;
            int width = 0;
            int height = 0;

            if (widthEntry.Text.Trim().Length > 0 && heightEntry.Text.Trim().Length > 0)
            {
                width = Convert.ToInt32(widthEntry.Text.Trim());
                height = Convert.ToInt32(heightEntry.Text.Trim());
                if (name.Length > 0 && width > 0 && height > 0)
                {
                    retVal = true;
                }
            }
            else
            {
                return retVal;
            }


            return retVal;
        }

        private Cairo.Color ToCairoColor(Gdk.Color gColor)
        {
            Cairo.Color color = new Cairo.Color(
                                    (double)gColor.Red / ushort.MaxValue,
                                    (double)gColor.Green / ushort.MaxValue,
                                    (double)gColor.Blue / ushort.MaxValue, 1);
            return color;
        }

        private void clearValues()
        {
            nameEntry.Text = string.Empty;
            widthEntry.Text = string.Empty;
            heightEntry.Text = string.Empty;
            standColorButton.Color = new Gdk.Color(0,0,0);
        }
       
        #endregion

        #region Control Events
        protected void okButton_OnClick (object sender, EventArgs e)
        {
            if (Validate())
            {
                //TODO - Create new Stand object - need an api call to do this
                //using static id for now
                Cairo.Color color = ToCairoColor(standColorButton.Color);
                Stand newStand = new Stand(0, nameEntry.Text.Trim(), color, Convert.ToInt32(widthEntry.Text.Trim()), Convert.ToInt32(heightEntry.Text.Trim()));
                templateStore.AddNode(newStand);
                clearValues();
                //this.Destroy();
            }
            else
            {
                using(MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false,
                    string.Format("Did you fill in all the fields?")))
                {
                    md.Run();
                    md.Destroy();
                }
            }
        }

        protected void cancelButton_OnClick (object sender, EventArgs e)
        {
            this.Destroy();
        }

        protected void drawStandShapeBtn_OnClick (object sender, EventArgs e)
        {
            throw new NotImplementedException ();
        }
        #endregion
    }
}


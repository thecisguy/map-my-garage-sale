using System;
using Gtk;

namespace Frontend
{
	public class OperationWidget
	{
		public OperationWidget ()
		{
		}

		/// <summary>
		/// Creates a VBox.
		/// </summary>
		/// <returns>The operation widget.</returns>
		/// <param name="iconFilePath">Icon file path.</param>
		/// <param name="labelText">Label text.</param>
		public static Widget CreateOperationWidget(string iconFilePath, string labelText)
		{
			VBox box = new VBox (false, 0);

			Gtk.Image image = new Gtk.Image (iconFilePath);
			Label label = new Label (labelText);

			box.PackStart (image, false, false, 3);
			box.PackStart (label, false, false, 3);

			image.Show ();
			label.Show ();

			return box;
		}
	}
}


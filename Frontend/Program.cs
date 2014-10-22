using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cairo;
using Glade;
using Gtk;

namespace Frontend
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.ShowAll ();
			Application.Run ();
		}
	}
}

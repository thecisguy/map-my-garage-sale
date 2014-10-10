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
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static Cairo.Color getColorOfTile(uint row, uint column);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void DebugPrintMonoInfo(System.Object o);


		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();

			win.ShowAll ();

//			uint row = 5;
//			uint column = 7;
//			Cairo.Color nc = getColorOfTile(row, column);
//			DebugPrintMonoInfo(nc);
//			Console.WriteLine();
//			Console.WriteLine("Printing Color Info:");
//			Console.WriteLine("Value of Red: " + nc.R);
//			Console.WriteLine("Value of Green: " + nc.G);
//			Console.WriteLine("Value of Blue: " + nc.B);
//			Console.WriteLine("Value of Alpha: " + nc.A);
			Application.Run ();
		}
	}
}

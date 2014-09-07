using System;
using System.Runtime.CompilerServices;
using Cairo;

class MonoMain {

	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	extern static Cairo.Color
		getColorOfTile(uint row, uint column);

	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	extern static void DebugPrintMonoInfo(Object o);

	static void Main() {
		Console.WriteLine ("Hello, World!");

		Cairo.Color c = new Color(0.5, 0.5, 0.5);
		DebugPrintMonoInfo(c);

		double d = 2.6;
		DebugPrintMonoInfo(d);
/*
		Cairo.Color nc = getColorOfTile(4u, 3u);
		Console.WriteLine();
		Console.WriteLine("Printing Color Info:");
		Console.WriteLine("Value of Red: " + nc.R);
		Console.WriteLine("Value of Green: " + nc.G);
		Console.WriteLine("Value of Blue: " + nc.B);
*/
	}
}

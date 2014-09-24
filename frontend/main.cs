using System;
using System.Runtime.CompilerServices;
using Cairo;
using api;

class MonoMain {

	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	extern static void DebugPrintMonoInfo(Object o);

	static void Main() {
		uint row = 5;
		uint column = 7;
		Cairo.Color nc = EngineAPI.getColorOfTile(row, column);
		DebugPrintMonoInfo(nc);
		Console.WriteLine();
		Console.WriteLine("Printing Color Info:");
		Console.WriteLine("Value of Red: " + nc.R);
		Console.WriteLine("Value of Green: " + nc.G);
		Console.WriteLine("Value of Blue: " + nc.B);
		Console.WriteLine("Value of Alpha: " + nc.A);
	}
}
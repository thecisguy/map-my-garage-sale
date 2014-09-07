using System;
using System.Runtime.CompilerServices;
using Cairo;

class MonoMain {

	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	extern static Cairo.Color
		getColorOfTile(uint row, uint column);

	static void Main() {
		Console.WriteLine ("Hello, World!");
		//Cairo.Color nc = getColorOfTile(4u, 3u);
	}
}

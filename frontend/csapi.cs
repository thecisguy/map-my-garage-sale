using System;
using System.Runtime.CompilerServices;
using Cairo;

namespace api {

	class EngineAPI {

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static double[] getColorOfTileRaw(uint row, uint column);

		public static Cairo.Color getColorOfTile(uint row, uint column) {
			double[] data = getColorOfTileRaw(row, column);
			return new Color(data[0], data[1], data[2], data[3]);
		}
	}
}
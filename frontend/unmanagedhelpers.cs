using System;
using System.Runtime.CompilerServices;
using Cairo;

namespace api {

	class EngineAPI {

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void DebugPrintMonoInfo(Object o);

		static Cairo.Color createColor(double r, double g,
		                               double b, double a) {
			return new Color(r, g, b, a);
		}
	}
}
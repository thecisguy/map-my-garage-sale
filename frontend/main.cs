/* main.cs
 *
 * Contains the main method of the frontend, which is invoked
 * by a mono API call made by the engine.
 *
 * Copyright (C) 2014 - Blake Lowe, Jordan Polaniec
 *
 * This file is part of Map My Garage Sale.
 * 
 * Map My Garage Sale is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * Map My Garage Sale is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Map My Garage Sale. If not, see <http://www.gnu.org/licenses/>.
 */

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
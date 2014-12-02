/* csapi.cs
 *
 * Contains the C# end of the engine-frontend communictation API.
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

namespace csapi {

	public class EngineAPI {

/************** Raw Methods ****************************************/

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static double[] getColorOfTileRaw(uint row, uint column);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static long[] selectStandRaw(uint row, uint column);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void deselectStandRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void rotateSelectedStandRaw(bool clockwise);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void removeSelectedStandRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void mirrorSelectedStandRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void grabNewStandRaw(int st_id);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static bool canApplyGrabbedStandRaw(
					uint row, uint column);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void doApplyGrabbedStandRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void removeGrabbedStandRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void grabSelectedStandRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static uint getMainGridHeightRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static uint getMainGridWidthRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static void loadUserFileRaw(string filename);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static uint getSelectedStandHeightRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static uint getSelectedStandWidthRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static int getNumTemplatesRaw();

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		extern static double[] getColorOfSTRaw(int st_id);

/***************** API Methods ***************************************/

		public static Cairo.Color getColorOfTile(uint row, uint column) {
			double[] data = getColorOfTileRaw(row, column);
			Color test = new Color(data[0], data[1], data[2], data[3]);
			return test;
		}

		public static bool selectStand(uint row, uint column) {
			long[] data = selectStandRaw(row, column);
			return data[0] == 1;
		}

		public static bool selectStand(uint row, uint column,
				out long originrow, out long origincolumn) {
			long[] data = selectStandRaw(row, column);
			originrow = data[1];
			origincolumn = data[2];
			return data[0] == 1;
		}

		public static void deselectStand() {
			deselectStandRaw();
		}

		public static void rotateSelectedStand(bool clockwise) {
			rotateSelectedStandRaw(clockwise);
		}

		public static void removeSelectedStand() {
			removeSelectedStandRaw();
		}

		public static void mirrorSelectedStand() {
			mirrorSelectedStandRaw();
		}

		public static void grabNewStand(int st_id) {
			grabNewStandRaw(st_id);
		}

		public static bool canApplyGrabbedStand(uint row, uint column) {
			return canApplyGrabbedStandRaw(row, column);
		}

		public static void doApplyGrabbedStand() {
			doApplyGrabbedStandRaw();
		}

		public static void removeGrabbedStand() {
			removeGrabbedStandRaw();
		}

		public static void grabSelectedStand() {
			grabSelectedStandRaw();
		}

		public static uint getMainGridHeight() {
			return getMainGridHeightRaw();
		}

		public static uint getMainGridWidth() {
			return getMainGridWidthRaw();
		}

		public static void loadUserFile(string filename) {
			loadUserFileRaw(filename);
		}

		public static uint getSelectedStandHeight() {
			return getSelectedStandHeightRaw();
		}

		public static uint getSelectedStandWidth() {
			return getSelectedStandWidthRaw();
		}

		public static int getNumTemplates() {
			return getNumTemplatesRaw();
		}

		public static Cairo.Color getColorOfST(int st_id) {
			double[] data = getColorOfSTRaw(st_id);
			Color test = new Color(data[0], data[1], data[2], data[3]);
			return test;
		}
	}
}
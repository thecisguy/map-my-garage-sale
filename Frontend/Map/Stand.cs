﻿/* Stand.cs
 *
 * Represents a Stand object in the UI.
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

using Cairo;
using System;
using Gtk;

namespace Frontend.Map
{
    public class Stand : DrawingArea, IDisposable, IMap
    {
        #region Properties
        public int StandKeyId { get; set; }
        public int StandInstanceId { get; set; }
        public string StandName { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Color ForeGround;
        #endregion


        #region Constructors
        public Stand()
        {
        }

        public Stand(string standName, float width, float height, Color foreGround)
        {

        }
        #endregion
    }
}


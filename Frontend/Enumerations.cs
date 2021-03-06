﻿/* Enumerations.cs
 *
 * This classes only job is to contain all the possible types of drawing that can occur in the expose event of the mapping area.
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

namespace Frontend
{
    public static class Enumerations
    {
        //many more used to be here.  See Grid.Expose_Event() implementation comments.
        public enum DrawType
        {
            InitialGridDraw,
            GridRedraw
        }
    }
}


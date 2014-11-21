/* global.h
 * 
 * Declares global variables and useful macros.
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

#ifndef GLOBAL_H
#define GLOBAL_H

// important mono types
#include <mono/jit/jit.h>

/* For mono typedefs: this header contains (usually stdint.h-style)
 * aliases for C# primitives.
 */
#include <mono/utils/mono-publib.h>

/* Color constants are defined at the base level as integers between
 * 0-255 (HTML style). This makes it easier to modify the values. We
 * divide them by 255.0 to get the double in the range of 0.0 - 1.0
 * that Cairo expects.
 */

#define TILE_EMPTY_RED_HTML	225
#define TILE_EMPTY_GREEN_HTML   225
#define TILE_EMPTY_BLUE_HTML	200

#define TILE_EMPTY_RED			(TILE_EMPTY_RED_HTML / 255.0)
#define TILE_EMPTY_GREEN		(TILE_EMPTY_GREEN_HTML / 255.0)
#define TILE_EMPTY_BLUE			(TILE_EMPTY_BLUE_HTML / 255.0)

#define TILE_EMPTY_ALPHA 0.3

#endif

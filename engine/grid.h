/* grid.h
 *
 * Declares the Tile and Grid structures
 * and the methods used to interact with them.
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

#ifndef GRID_H
#define GRID_H

#include <stdbool.h>
#include "global.h"

typedef struct tile *tile;
typedef struct grid *grid;
typedef struct stand *stand;
typedef struct stand_template *stand_template;

/* The stand-like type can represent either a stand or a
 * stand_template, and should be used to pass these types to
 * functions that don't care which one they get.
 */
enum stand_type {STAND, STAND_TEMPLATE};
typedef union stand_like {
	struct {
		enum stand_type type;
	} stand_proto;
	struct {
		enum stand_type type;
		stand s;
	} stand_stand;
	struct {
		enum stand_type type;
		stand_template st;
	} stand_st;
} stand_like;

struct tile {
	tile up;
	tile right;
	tile left;
	tile down;
	uint32_t row;
	uint32_t column;

	stand_like stand;
};

struct grid {
	tile origin;
	uint32_t height;
	uint32_t width;
	tile *lookup;
};

/* Allocates and initializes a new Grid.
 * 
 * As all Grids are rectangular, this method accepts width
 * and height parameters for its dimensions.
 *
 * The resulting Grid will have width columns and height rows.
 * 
 * Returns NULL if space could not be allocated.
 */
grid new_grid(uint32_t width, uint32_t height);

/* Allocates and initializes a new Grid which is a clone of
 * an existing one.
 * 
 * Returns NULL if space could not be allocated.
 */
grid clone_grid(grid g);

/* Deallocates a Grid. */
void del_grid(grid g);

/* Convenience method for the lookup table.
 * Returns the Tile located at the specified coordinates.
 */
tile grid_lookup(grid g, uint32_t row, uint32_t column);

/* Rotates a grid 90 degrees */
void rotate_grid(grid g, bool clockwise);

/* Reflects a Grid horizontally by swapping its columns. */
void mirror_grid(grid g);

#endif

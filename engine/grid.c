/* grid.c
 *
 * Defines routines for constructing and deleting Grids.
 *
 * Grids are one of the primary data types in the program, because
 * they represent the user's main editing area as well as the 'space'
 * that Stands occupy.
 *
 * A Grid is made up of a potentially large amount of Tiles.
 * Each Tile represents a single square on the Grid. The overall
 * Grid is a rectangular matrix of Tiles.
 *
 * A Grid can be traversed by first accessing its origin Tile,
 * its top-left corner tile. Each Tile contains pointers to its
 * adjacent tiles.
 *
 * When referring to a specific Tile in a grid, the column of the Tile
 * is the number of columns to the right of the origin Tile it is, and
 * its row is the number of rows below the origin Tile it is.
 * The coordinates should be expressed as [row, column].
 * For example, in the following Grid:
 *
 * A B C
 * D E F
 * G H I
 *
 * A is the origin Tile, it is at [0, 0].
 * C is at [0, 2].
 * H is at [2, 1].
 *
 * All of the memory occupied by a Grid exists on the heap.
 * You must destroy a Grid by calling its destructor to avoid
 * leaking it.
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

#include <stdlib.h>
#include <assert.h>
#include <stdbool.h>
#include "grid.h"
#include "global.h"

static tile new_tile(uint32_t row, uint32_t column);
static void rebuild_lookup(grid g);
static void reset_origin(grid g);

/* Creates a new tile on the heap, initializing its pointers to NULL */
static tile new_tile(uint32_t row, uint32_t column) {
	tile nt = malloc(sizeof(struct tile));
	if (!nt)
		goto out_nt;

	nt->up = NULL;
	nt->down = NULL;
	nt->left = NULL;
	nt->right = NULL;

	nt->row = row;
	nt->column = column;

	nt->s = NULL;

	return nt;

out_nt:
	return NULL;
}

grid new_grid(uint32_t width, uint32_t height) {
	// test invariants
	assert(width > 0);
	assert(height > 0);

	/* For error handling later if an allocation fails.
	 * It holds references to the rightmost Tile in each
	 * row of the Grid. This will allow us to deallocate our
	 * constucted rows, as well as the portion of the row we
	 * are currently working on.
	 */
	tile rightends[height];
	for (uint32_t i = 0; i < height; i++)
		rightends[i] = NULL;

	// allocate space for the struct grid and
	// initialize its fields
	grid ng = malloc(sizeof(struct grid));
	if (!ng)
		goto out_ng;
	ng->origin = NULL;
	ng->height = height;
	ng->width = width;

	ng->lookup = malloc(sizeof(tile *) * height * width);
	if (!ng->lookup)
		goto out_lookup;

	// Now we descend through the graph.
	// Each pass through this loop completes a single row.
	for (uint32_t j = 0; j < height; j++) {
		tile above;
		tile farleft;

		// create the first tile in the current row so our
		// inner loop has something to work with
		if (j == 0) {
			// create origin tile
			ng->origin = new_tile(j, 0);
			if (!ng->origin)
				goto out_tiles;
			rightends[j] = ng->origin;
			ng->lookup[0] = ng->origin;
			above = NULL;
			farleft = ng->origin;
		} else {
			tile newrow = new_tile(j, 0);
			if (!newrow)
				goto out_tiles;
			ng->lookup[j * width] = newrow;
			rightends[j] = newrow;
			farleft->down = newrow;
			newrow->up = farleft;
			above = farleft->right;
			farleft = newrow;
		}

		// this inner loop constructs all of the columns in the row,
		// extending from the tile we just created.
		tile t = farleft;
		for (uint32_t i = 1; i < width; i++, rightends[j] = t) {
			tile n = new_tile(j, i);
			ng->lookup[j * width + i] = n;
			if (!n)
				goto out_tiles;
			n->up = above;
			if (above != NULL)
				above->down = n;
			t->right = n;
			n->left = t;
			t = n;
			if (above != NULL)
				above = above->right;
		}
	}

	return ng;

// Error handling routines:
out_tiles:;
	/* If we jumped here, then we need to delete every tile
	 * structure we allocated, and the lookup table.
	 * We accomplish this by traversing rightends, deleting every
	 * row we completed as well as whatever portion of the current
	 * row we have finished.
	 *
	 * Like the above routine, we move across the grid top-down
	 * in the outer loop. However, in the inner loop, we move across
	 * each row from right to left.
	*/
	uint32_t endi = 0;
	while (rightends[endi] != NULL && endi < height) {
		tile rend = rightends[endi];
		while (rend != NULL) {
			tile l = rend->left;
			free(rend);
			rend = l;
		}
		endi++;
	}
	free(ng->lookup);

out_lookup:;
	// If we jumped here, we need only free the struct grid.
	free(ng);

out_ng:;
       // If we jumped here, there is nothing for us to free.
       return NULL;
}

void del_grid(grid g) {
	assert(g);
	
	/* we need to free every tile, then the lookup table,
	 * then the struct grid itself
	 * in the outer loop, we scan top-down
	 */
	tile l = g->origin;
	while (l != NULL) {
		tile below = l->down;

		// in the inner loop, we scan left-right
		tile now = l;
		while (now != NULL) {
			tile next = now->right;
			free(now);
			now = next;
		}

		l = below;
	}
	
	// now we just need to free the rest
	free(g->lookup);
	free(g);
}

tile grid_lookup(grid g, uint32_t row, uint32_t column) {
	// test invariants
	assert(g);
	assert(row < g->height);
	assert(column < g->width);

	return g->lookup[row * g->width + column];
}

void rotate_grid(grid g, bool clockwise) {
	assert(g);
	
	// iterate through each tile in the grid
	uint64_t num_tiles = g->width * g->height;
	uint64_t i = 0;
	for(tile *t = g->lookup; i < num_tiles; t++, i++) {
		tile cur = *t;

		// rotation of the entire grid can be achieved
		// by rotating each individual tile

		if (clockwise) {
			tile oldup = cur->up;
			cur->up = cur->left;
			cur->left = cur->down;
			cur->down = cur->right;
			cur->right = oldup;
		} else {
			tile oldup = cur->up;
			cur->up = cur->right;
			cur->right = cur->down;
			cur->down = cur->left;
			cur->left = oldup;
		}
	}

	// now we need to update the Grid structure's members
	uint32_t temp = g->height;
	g->height = g->width;
	g->width = temp;

	reset_origin(g);

	rebuild_lookup(g);
}

/* Rebuilds all lookup data in a grid.
 * This includes the lookup table as well as the coordinates
 * stored in each Tile.
 */
static void rebuild_lookup(grid g) {
	assert(g);
	
	// we need to go over each tile
	// begin by scanning top-down
	
	uint32_t row = 0;
	uint32_t column = 0;
	tile l = g->origin;
	while (l) {
		tile below = l->down;

		// in the inner loop, we scan left-right, and modify the
		// members of the Tile and Grid structures
		tile now = l;
		while (now) {
			now->row = row;
			now->column = column;

			g->lookup[row * g->width + column] = now;

			now = now->right;
			column++;
		}

		l = below;
		row++;
		column = 0;
	}
}

/* Finds the true origin tile, starting from the apparent origin
 * and moving northeast. Sets the origin pointer to the found Tile.
 */
static void reset_origin(grid g) {
	assert(g);
	
	tile t = g->origin;
	while(t->left)
		t = t->left;
	while(t->up)
		t = t->up;
	g->origin = t;
}

grid clone_grid(grid g) {
	grid cg = new_grid(g->width, g->height);
	if (!cg)
		goto out_cg;

	// copy stand references
	uint64_t num_tiles = g->width * g->height;
	tile *old, *new;
	uint64_t ti;
	for (old = g->lookup, new = cg->lookup, ti = 0;
	     ti < num_tiles; old++, new++, ti++) {
		(*new)->s = (*old)->s;
	}

	return cg;

	out_cg:;
		return NULL;
}

void mirror_grid(grid g) {
	assert(g);
	
	// iterate through each tile in the grid
	uint64_t num_tiles = g->width * g->height;
	uint64_t i = 0;
	for(tile *t = g->lookup; i < num_tiles; t++, i++) {
		tile cur = *t;

		// reflection of the entire grid can be achieved
		// by flipping the left-right pointers of each
		// individual tile

		tile temp = cur->left;
		cur->left = cur->right;
		cur->right = temp;
	}

	// now we need to update the Grid structure's members
	reset_origin(g);
	rebuild_lookup(g);
}
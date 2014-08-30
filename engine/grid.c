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
 * its top-right corner tile. Each Tile contains pointers to its
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
 * C is at [0, 3].
 * H is at [3, 2].
 *
 * All of the memory occupied by a Grid exists on the heap.
 * You must destroy a Grid by calling its destructor to avoid
 * leaking it.
 */

#include <stdlib.h>
#include <assert.h>
#include "grid.h"

static tile new_tile(void);

/* Creates a new tile on the heap, initializing its pointers to NULL */
static tile new_tile(void) {
	tile nt = malloc(sizeof(struct tile));
	if (nt == NULL)
		goto out_nt;

	nt->up = NULL;
	nt->down = NULL;
	nt->left = NULL;
	nt->right = NULL;

	return nt;

out_nt:
	return NULL;
}

grid new_grid(unsigned int width, unsigned int height) {
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
	for (int i = 0; i < height; i++)
		rightends[i] = NULL;

	// allocate space for the struct grid and
	// initialize its fields
	grid ng = malloc(sizeof(struct grid));
	if (ng == NULL)
		goto out_ng;
	ng->origin = NULL;
	ng->height = height;
	ng->width = width;

	// Now we descend through the graph.
	// Each pass through this loop completes a single row.
	for (int j = 0; j < height; j++) {
		tile above;
		tile farleft;

		// create the first tile in the current row so our
		// inner loop has something to work with
		if (j == 0) {
			// create origin tile
			rightends[j] = ng->origin;
			ng->origin = new_tile();
			if (ng->origin == NULL)
				goto out_tiles;
			above = NULL;
			farleft = ng->origin;
		} else {
			tile newrow = new_tile();
			if (newrow == NULL)
				goto out_tiles;
			rightends[j] = newrow;
			farleft->down = newrow;
			newrow->up = farleft;
			above = farleft->right;
			farleft = newrow;
		}

		// this inner loop constructs all of the columns in the row,
		// extending from the tile we just created.
		tile t = farleft;
		for (int i = 1; i < width; i++, rightends[j] = t) {
			tile n = new_tile();
			if (n == NULL)
				goto out_tiles;
			n->up = above;
			above->down = n;
			t->right = n;
			n->left = t;
			t = n;
			if (j != 0)
				above = above->right;
		}
	}

	return ng;

// Error handling routines:
out_tiles:;
	/* If we jumped here, then we need to delete every tile
	 * structure we allocated and the grid structure itself.
	 * We accomplish this by traversing rightends, deleting every
	 * row we completed as well as whatever portion of the current
	 * row we have finished.
	 *
	 * Like the above routine, we move across the grid top-down
	 * in the outer loop. However, in the inner loop, we move across
	 * each row from right to left.
	*/
	int endi = 0;
	while (rightends[endi] != NULL && endi < height) {
		tile rend = rightends[endi];
		while (rend != NULL) {
			tile l = rend->left;
			free(rend);
			rend = l;
		}
		endi++;
	}
	free(ng);
out_ng:;
       // If we jumped here, there is nothing for us to free.
       return NULL;
}

/* grid.h
 *
 * Declares the Tile and Grid structures
 * and the methods used to interact with them.
 */

#ifndef GRID_H
#define GRID_H

typedef struct tile *tile;
typedef struct grid *grid;
typedef struct stand *stand;

struct tile {
	tile up;
	tile right;
	tile left;
	tile down;
	uint32_t row;
	uint32_t column;

	stand s;
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

/* Conveninece method for the lookup table.
 * Returns the Tile located at the specified coordinates.
 */
tile grid_lookup(grid g, uint32_t row, uint32_t column);

#endif

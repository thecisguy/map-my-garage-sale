/* grid.h
 *
 * Declares the Tile and Grid structures
 * and the methods used to interact with them.
 */

typedef struct tile *tile;
typedef struct grid *grid;

struct tile {
	tile up;
	tile right;
	tile left;
	tile down;
	unsigned int row;
	unsigned int column;
};

struct grid {
	tile origin;
	unsigned int height;
	unsigned int width;
	tile *lookup;
};

/* Allocates and initializes a new Grid.
 * 
 * As all Grids are rectangular, this method accepts width
 * and height parameters for its dimensions.
 *
 * The resulting Grid will have width columns and height rows.
 */
grid new_grid(unsigned int width, unsigned int height);

/* Deallocates a Grid. */
void del_grid(grid g);

/* Conveninece method for the lookup table.
 * Returns the Tile located at the specified coordinates.
 */
tile grid_lookup(grid g, unsigned int row, unsigned int column);
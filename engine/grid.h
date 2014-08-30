/* grid.h
 *
 * Declares the Tile and Grid structures
 * and the methods used to interact with them.
 */

struct tile {
	struct tile *up;
	struct tile *right;
	struct tile *left;
	struct tile *down;
};

struct grid {
	struct tile *origin;
	unsigned int height;
	unsigned int width;
};

typedef struct tile *tile;
typedef struct grid *grid;

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

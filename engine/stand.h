/* stand.h
 *
 * Declares the Stand structure
 * and the methods used to interact with it.
 */

#ifndef STAND_H
#define STAND_H

#include "grid.h"
#include <stdbool.h>
#include <stdlib.h>

typedef struct stand_template *stand_template;

struct stand {
	// basic info
	grid source;
	char *name;

	// owning grid
	grid g;

	// color info
	double red;
	double green;
	double blue;
	double alpha;
};

stand new_stand(stand_template t, double red,
		double green, double blue, double alpha);

typedef struct application_node *application_node;

bool do_apply(stand s, application_node n);
bool can_apply(stand s, grid g);

bool rotateCW(grid g);
bool rotateCCW(grid g);
bool mirror(grid g);

#endif
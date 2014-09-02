#include "grid.h"
#include <stdbool.h>
#include <stdlib.h>

struct stand {
	grid g;
	
	// color info
	double red;
	double green;
	double blue;
	double alpha;
}

stand new_stand(double red, double green, double blue, double alpha) {
	stand ns = malloc(sizeof(struct stand));
	if (ns == NULL)
		goto out_ns;

	ns->red = red;
	ns->green = green;
	ns->blue = blue;
	ns->alpha = alpha;

	ns->g = NULL;

out_ns:;
       return ns;
}

bool apply(stand s, grid g);

bool rotateCW(grid g);
bool rotateCCW(grid g);
bool mirror(grid g);

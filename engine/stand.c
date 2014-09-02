#include "grid.h"
#include <stdbool.h>

struct stand {
	grid g;
	
	// color info
	double red;
	double green;
	double blue;
	double alpha;
}

bool apply(stand s, grid g);

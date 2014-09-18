/* stand.c
 *
 * Defines routines for construsting and using Stands.
 *
 * Stands represent physical item displays in the real world.
 * They occupy space on Grids by applying themselves onto Tiles.
 * They also carry color information, which is used to draw them.
 *
 * When examining a Grid, each Stand can be uniqely identified by its
 * pointer. Stands may not overlap one another.
 *
 * Stands can be constructed from pre-existing Stand Templates. Stand
 * Templates include a small Grid over which its shape is applied. This
 * Grid is only large enough to serve as the minimum bounding-
 * box of its shape. In addition, new Stand Templates can be defined by
 * scanning a Grid for a Stand, and recreating it upon a new Grid. Again,
 * the new Grid must be the minimum boundung box of its shape.
 *
 * Stands and Stand Templates exist on the heap, and must be destroyed
 * to aviod leaking them.
 */

#include <stdbool.h>
#include <stdlib.h>
#include "global.h"
#include "grid.h"
#include "stand.h"

struct stand_template {
	grid t;

	char *name;
}

stand new_stand(stand_template t, double red,
		double green, double blue, double alpha) {
	stand ns = malloc(sizeof(struct stand));
	if (ns == NULL)
		goto out_ns;

	ns->name = malloc(strlen(t->name) + 1);
	if (ns->name == NULL)
		goto out_name;
	strcpy(ns->name, t->name);

	ns->g = NULL;
	ns->red = red;
	ns->green = green;
	ns->blue = blue;
	ns->alpha = alpha;

	ns->source = stand_template_copy(t);

	return ns;

out_name:;
	free(ns)
out_ns:;
	return NULL;
}

struct application_node {
	tile t;
	struct application_node *next;
}
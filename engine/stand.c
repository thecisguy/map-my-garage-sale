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
 * box of its shape, plus enough added rows and columns to make the template
 * a square. In addition, new Stand Templates can be defined by
 * scanning a Grid for a Stand, and recreating it upon a new Grid. Again,
 * the new Grid must be the minimum boundung box of its shape.
 *
 * Stands and Stand Templates exist on the heap, and must be destroyed
 * to aviod leaking them.
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

#include <stdbool.h>
#include <stdlib.h>
#include <string.h>
#include "global.h"
#include "grid.h"
#include "stand.h"

struct stand_template {
	grid t;

	char *name;
};

stand new_stand(stand_template tem, double red,
		double green, double blue, double alpha) {
	stand ns = malloc(sizeof(struct stand));
	if (!ns)
		goto out_ns;

	ns->name = malloc(strlen(tem->name) + 1);
	if (!ns->name)
		goto out_name;
	strcpy(ns->name, tem->name);

	ns->g = NULL;
	ns->red = red;
	ns->green = green;
	ns->blue = blue;
	ns->alpha = alpha;

	ns->source = clone_grid(tem->t);
	if (!ns->source)
		goto out_source;

	return ns;
	
out_source:;
	free(ns->name);
out_name:;
	free(ns);
out_ns:;
	return NULL;
}

struct application_node {
	tile t;
	struct application_node *next;
};
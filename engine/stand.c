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
 * Stands are constructed from pre-existing Stand Templates. Stand
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
#include <assert.h>
#include "global.h"
#include "grid.h"
#include "stand.h"

struct stand_template {
	grid t;

	char *name;
};

typedef struct application_node {
	tile t;
	struct application_node *next;
} *application_node;

struct application_data {
	int64_t row;
	int64_t column;
	grid g;
	struct application_node *head;
};

static void del_application_list(application_node n);
static void del_application_data(application_data appd);

stand new_stand(stand_template tem, double red,
		double green, double blue, double alpha) {
	assert(tem);
	
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
	ns->appd = NULL;

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

/* Frees the memory allocated by an application list. */
static void del_application_list(application_node n) {
	while (n) {
		application_node next = n->next;
		free(n);
		n = next;
	}
}

/* Frees the memory allocated by application data.
 * 
 * Note: this function does not set the pointer in any owning
 * Stand (if one exists) to NULL. This is the caller's responsibility.
 */
static void del_application_data(application_data appd) {
	assert(appd);

	del_application_list(appd->head);
	free(appd);
}

/* Checks the applicability of Stand s onto Grid g at the specified
 * coordinates, and prepares data which can be immediately consumed
 * by do_apply if successful.
 * 
 * The Stand will be applied such that the origin tile of its source Grid
 * lies overtop of the given coordinates.
 * 
 * The specified coordinates CAN specify an off-the-grid location, though
 * it is illegal to specify a set of coordinates such that any tile of the
 * Stand lies off the Grid.
 * 
 * If the generation of the application data fails for any reason, this
 * function will return false.
 */
bool can_apply(restrict stand s, restrict grid g,
               int64_t row, int64_t column) {
	assert(s);
	assert(g);
	
	application_node head = NULL;
	application_node tail = NULL;
	
	for (uint32_t cur_row = 0; cur_row < s->source->height; cur_row++) {
		for (uint32_t cur_column = 0; cur_column < s->source->width;
		     cur_column++) {
			tile from = grid_lookup(s->source, cur_row, cur_column);
			if (!from->s) // stand does not occupy this tile
				continue;
			int64_t target_row = row + cur_row;
			int64_t target_column = column + cur_column;
			if (target_row < 0 || target_row >= g->height
			    || target_column < 0 || target_column >= g->width)
				goto out_fail; // target tile is off the grid
			tile to = grid_lookup(g, target_row, target_column);
			if (to->s) // another stand occupies this tile
				goto out_fail;
			
			// stand CAN be applied here
			if (!head) {
				head = (application_node)
					malloc(sizeof(struct application_node));
				if (!head) // out of mem
					goto out_fail;
				tail = head;
				head->t = to;
			} else {
				tail->next = (application_node)
					malloc(sizeof(struct application_node));
				if (!tail->next) // out of mem
					goto out_fail;
				tail = tail->next;
				tail->t = to;
			}
		}
	}

	application_data out =
		(application_data) malloc(sizeof(struct application_data));
	if (!out)
		goto out_fail;

	if (s->appd)
		del_application_data(s->appd);

	out->row = row;
	out->column = column;
	out->head = head;
	out->g = g;
	s->appd = out;
	return true;

	out_fail:
		del_application_list(head);
		return false;
}

/* Actually applies a Stand onto a Grid.
 * 
 * Before calling this function, you must make a call to can_apply,
 * which checks the applicability of the Stand at the desired coordinates.
 * (this function does nothing if can_apply has not been run since the
 * last call to do_apply)
 * 
 * This function uses data produced by can_apply which contains placement
 * information to do its work, which is why the Stand to be applied is the only
 * necessary parameter.
 */
void do_apply(stand s) {
	assert(s);
	
	if (!s->appd)
		return;

	application_node n = s->appd->head;
	while (n) {
		n->t->s = s;
		n = n->next;
	}

	s->row = s->appd->row;
	s->column = s->appd->column;
	s->g = s->appd->g;

	del_application_data(s->appd);
	s->appd = NULL;
}
 /* Removes a Stand from the Grid it is applied to. */
void remove_stand(stand s) {
	assert(s);
	
	// we use the Stand's source dimensions to restrict the
	// search space on the applied Grid
	uint32_t end_row = s->row + s->source->height;
	uint32_t end_column = s->column + s->source->width;
	for (uint32_t row = s->row; row < end_row; row++) {
		for (uint32_t column = s->column; column < end_column; column++) {
			tile t = grid_lookup(s->g, row, column);
			if (t->s == s)
				t->s = NULL;
		}
	}
}
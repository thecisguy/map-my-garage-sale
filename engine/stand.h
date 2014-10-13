/* stand.h
 *
 * Declares the Stand structure
 * and the methods used to interact with it.
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

#ifndef STAND_H
#define STAND_H

#include "grid.h"
#include <stdbool.h>
#include <stdlib.h>

typedef struct stand_template *stand_template;
typedef struct application_node *application_node;

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

	// applicable list, added by can_apply and removed by do_apply
	application_node list;
};

stand new_stand(stand_template t, double red,
		double green, double blue, double alpha);

void do_apply(stand s);
bool can_apply(stand s, grid g, int64_t row, int64_t column);

bool rotateCW(grid g);
bool rotateCCW(grid g);
bool mirror(grid g);

#endif

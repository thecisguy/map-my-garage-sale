/* save_n_load.c
 * 
 * This file contains definitions for the functions responsible
 * for saving and loading the program from a file.
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

#include <stdio.h>
#include <string.h>
#include <stdbool.h>
#include <ctype.h>
#include <inttypes.h>

#include "grid.h"
#include "stand.h"
#include "save_n_load.h"
#include "capi.h"

static void scan_whitespace(FILE *f);
static bool read_stand_templates(FILE *f, struct stand_template **st);
static grid read_grid(FILE *f, uint32_t height, uint32_t width, void *stand);

bool load_file(FILE *f) {
	int c;
	char filetype[5];
	for (int i = 0; i < 4; i++) {
		// did we get EOF?
		if (!(c = fgetc(f))) return false;
		filetype[i] = (char) c;
	}
	filetype[4] = '\0';
	// wrong filetype?
	if (strcmp("MMGS", filetype) != 0) return false;

	(void) fgetc(f); //skip next colon
	int file_version = 0;
	while ((c = fgetc(f)) && c != ';') {
		// non-numeric character?
		if (!isdigit(c)) return false;
		file_version = file_version * 10 + (c - '0');
	}

	// new data, to be moved if successful
	int new_num_templates;
	struct stand_template *new_st_arr = NULL;

	scan_whitespace(f);
	while (c) {
		char blockname[101];
		blockname[0] = c;
		int i = 0;
		while (++i < 100 && (c = fgetc(f)) && c != '(' && c != '[') {
			blockname[i] = c;
		}
		blockname[i] = '\0';
		if (i == 100 && !(c == '(' || c == '['))
			return false;
		ungetc(c, f);
		
		if (strcmp("standtemplates", blockname) == 0) {
			new_num_templates =
				read_stand_templates(f, &new_st_arr);
			if (!new_num_templates)
				goto out_fail;
		} else if (strcmp("stands", blockname) == 0) {
			// go forth and parse
		} else if (strcmp("maingrid", blockname) == 0) {

		} else {
			// unrecognized block
			return false;
		}

		scan_whitespace(f);
	}
	return true;

out_fail:;
	 if (new_st_arr) {
		 for (int i = 0; i < new_num_templates; i++) {
			free(new_st_arr[i].name);
			del_grid(new_st_arr[i].t);
		 }
	 }
	 return false;
}

static void scan_whitespace(FILE *f) {
	int c;
	while((c = fgetc(f)) && isspace(c));
	ungetc(c, f);
}


/* Reads the standtemplates block.
 * 
 * Requires a FILE * and the location of where to store the stand_template
 * array, allocated on the heap.
 * 
 * Returns the number of templates read, or 0 if the read failed,
 * in which case st will be set to NULL.
 */
static bool read_stand_templates(FILE *f, struct stand_template **st) {
	int num_templates;
	int scan_val = fscanf(f, "[%i](", &num_templates);
	if (scan_val == EOF || scan_val < 1)
		return false;
	struct stand_template *new_stand_templates =
		(struct stand_template *) malloc(
		sizeof(struct stand_template) * num_templates);
	if (!new_stand_templates)
		goto out_templates;

	int templates_i = 0;
	int c;
	char *name;
	while ((c = fgetc(f)) != ')') {
		if (isspace(c)) continue;
		int name_len = 0;
		do {
			name_len = name_len * 10 + (c - '0');
		} while ((c = fgetc(f)) != ':');
		
		name = (char *) malloc(sizeof(char) * (name_len + 1));
		if (!name)
			goto out_name;
		for (int i = 0; i < name_len; i++) {
			name[i] = fgetc(f);
		}
		name[name_len] = '\0';

		uint32_t height;
		uint32_t width;
		scan_val = 
			fscanf(f, ":%" SCNu32 ":%" SCNu32 ":", &height, &width);
		if (scan_val == EOF || scan_val < 2)
			goto out_new_source;

		stand_template t = &new_stand_templates[templates_i++];

		grid new_source = read_grid(f, height, width, (void *) t);
		if (!new_source)
			goto out_new_source;

		t->name = name;
		t->t = new_source;
		
	}
	return true;

	out_new_source:;
		free(name);
	out_name:;
		free(new_stand_templates);
	out_templates:;
		*st = NULL;
		return false;
}

static grid read_grid(FILE *f, uint32_t height, uint32_t width, void *stand) {
	grid ng = new_grid(height, width);
	if (!ng)
		goto out_ng;

	int c;
	// exploits the row-major order of the lookup table
	tile *t = ng->lookup;
	while ((c = fgetc(f)) && c != ';') {
		if (isspace(c)) continue;
		if (c == '0') {
			// nothing to do here, as tiles' stand pointers
			// are NULL by default
		} else if (c == 'S') {
			(*t)->stand = stand;
		} else {
			// unrecognized char
			goto out_fail;
		}
		t++;
	}
	if (c == EOF)
		goto out_fail;

	return ng;


out_fail:;
	del_grid(ng);
out_ng:;
	return NULL;
}

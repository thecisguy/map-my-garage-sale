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
static grid read_grid(FILE *f, uint32_t height,
                      uint32_t width, stand_like stand);
static bool read_stands(FILE *f, stand **s);
static void print_stand_templates(FILE *f);
static void print_stands(FILE *f);
static void print_grid(FILE *f, grid g);

bool load_file(FILE *f) {
	int c;
	char filetype[5];
	for (int i = 0; i < 4; i++) {
		// did we get EOF?
		if ((c = fgetc(f)) == EOF) return false;
		filetype[i] = (char) c;
	}
	filetype[4] = '\0';
	// wrong filetype?
	if (strcmp("MMGS", filetype) != 0) return false;

	(void) fgetc(f); //skip next colon
	int file_version = 0;
	while ((c = fgetc(f)) != EOF && c != ';') {
		// non-numeric character?
		if (!isdigit(c)) return false;
		file_version = file_version * 10 + (c - '0');
	}
	
	// new data, to be moved if successful
	int32_t new_num_templates = 0;
	struct stand_template *new_st_arr = NULL;
	int32_t new_num_stands = 0;
	stand *new_stand_arr = NULL;
	grid new_main_grid = NULL;

	scan_whitespace(f);
	while ((c = fgetc(f)) != EOF) {
		char blockname[101];
		blockname[0] = c;
		int i = 0;
		while (++i < 100 && (c = fgetc(f)) != EOF
		       && c != '(' && c != '[') {
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
			new_num_stands =
				read_stands(f, &new_stand_arr);
			if (!new_num_stands)
				goto out_fail;
		} else if (strcmp("maingrid", blockname) == 0) {
			if (!(c = fgetc(f)) || c != '(')
				goto out_fail;
			uint32_t new_height;
			uint32_t new_width;
			int scan_val =
				fscanf(f, "%" SCNu32 ":%" SCNu32,
						&new_width, &new_height);
			if (scan_val == EOF || scan_val < 2)
				goto out_fail;
			scan_whitespace(f);
			if (!(c = fgetc(f)) || c != ')')
				goto out_fail;

			if (!(new_main_grid = new_grid(new_width, new_height)))
				goto out_fail;
		} else {
			// unrecognized block
			goto out_fail;
		}

		scan_whitespace(f);
	}

	// apply new stands
	if (!new_main_grid)
		goto out_fail;
	for (int i = 0; i < new_num_stands; i++) {
		stand cur = new_stand_arr[i];
		bool ok = can_apply(cur, new_main_grid, cur->row, cur->column);
		if (!ok)
			goto out_fail;
		do_apply(cur);
	}

	// copy other data
	if (new_st_arr)
		main_templates = new_st_arr;
	if (main_grid) {
		tile *t = main_grid->lookup;
		uint64_t len = main_grid->height * main_grid->width;
		for (uint64_t i = 0; i < len; i++) {
			if ((*t)->stand.stand_stand.s)
				del_stand((*t)->stand.stand_stand.s);
			t++;
		}
		del_grid(main_grid);
	}
	main_grid = new_main_grid;

	//cleanup
	free(new_stand_arr); // only removes the container, the stands inside
	                     // are safely in the grid
	
	return true;

out_fail:;
	 if (new_st_arr) {
		 for (int i = 0; i < new_num_templates; i++) {
			free(new_st_arr[i].name);
			del_grid(new_st_arr[i].t);
		 }
		 free(new_st_arr);
	 }
	 if (new_stand_arr) {
		 for (int i = 0; i < new_num_stands; i++) {
			 del_stand(new_stand_arr[i]);
		 }
		 free(new_stand_arr);
	 }
	 if (new_main_grid) {
		 del_grid(new_main_grid);
	 }
	 return false;
}

static void scan_whitespace(FILE *f) {
	int c;
	while((c = fgetc(f)) != EOF && isspace(c));
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
	int32_t num_templates;
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
	while ((c = fgetc(f)) != EOF && c != ')') {
		if (isspace(c)) continue;
		int name_len = 0;
		do {
			name_len = name_len * 10 + (c - '0');
		} while ((c = fgetc(f)) != EOF && c != ':');
		
		name = (char *) malloc(sizeof(char) * (name_len + 1));
		if (!name)
			goto out_name;
		for (int i = 0; i < name_len; i++) {
			name[i] = fgetc(f);
		}
		name[name_len] = '\0';
		
		uint8_t red;
		uint8_t green;
		uint8_t blue;
		uint8_t alpha;
		uint32_t height;
		uint32_t width;
		scan_val = 
			fscanf(f, ":%" SCNu8 ":%" SCNu8 ":%" SCNu8 ":%" SCNu8
				":%" SCNu32 ":%" SCNu32 ":",
				&red, &green, &blue, &alpha, &height, &width);
		if (scan_val == EOF || scan_val < 6)
			goto out_new_source;

		stand_template t = &new_stand_templates[templates_i++];
		stand_like tl;
		tl.stand_proto.type = STAND_TEMPLATE;
		tl.stand_st.st = t;

		grid new_source = read_grid(f, height, width, tl);
		if (!new_source)
			goto out_new_source;

		t->name = name;
		t->t = new_source;
		t->red = red / 255.0;
		t->green = green / 255.0;
		t->blue = blue / 255.0;
		t->alpha = alpha / 255.0;
		
	}
	*st = new_stand_templates;
	return num_templates;

	out_new_source:;
		free(name);
	out_name:;
		free(new_stand_templates);
	out_templates:;
		*st = NULL;
		return 0;
}

/* Reads the stands block.
 * 
 * Requires a FILE * and the location of where to store the stand
 * array, allocated on the heap.
 * 
 * Returns the number of stands read, or 0 if the read failed,
 * in which case st will be set to NULL.
 */
static bool read_stands(FILE *f, stand **stand_arr) {
	int32_t num_stands;
	int scan_val = fscanf(f, "[%i](", &num_stands);
	if (scan_val == EOF || scan_val < 1)
		return false;
	stand *new_stands =
		(stand *) malloc(
		sizeof(stand) * num_stands);
	if (!new_stands)
		goto out_stands;

	int stands_i = 0;
	int c;
	char *name;
	stand s = NULL;
	while ((c = fgetc(f)) != EOF && c != ')') {
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

		uint8_t red;
		uint8_t green;
		uint8_t blue;
		uint8_t alpha;
		uint32_t height;
		uint32_t width;
		scan_val = 
			fscanf(f, ":%" SCNu8 ":%" SCNu8 ":%" SCNu8 ":%" SCNu8
				":%" SCNu32 ":%" SCNu32 ":",
				&red, &green, &blue, &alpha, &height, &width);
		if (scan_val == EOF || scan_val < 6)
			goto out_new_source;

		s = (stand) malloc(sizeof(struct stand));
		if (!s)
			goto out_new_stand;
		stand_like sl;
		sl.stand_proto.type = STAND;
		sl.stand_stand.s = s;
		
		grid new_source = read_grid(f, height, width, sl);
		if (!new_source)
			goto out_new_source;

		uint64_t row;
		uint64_t column;
		scan_val = fscanf(f, "%" SCNu64 ":%" SCNu64 ";", &row, &column);
		if (scan_val == EOF || scan_val < 2)
			goto out_new_source;

		s->name = name;
		s->source = new_source;
		s->red = red / 255.0;
		s->green = green / 255.0;
		s->blue = blue / 255.0;
		s->alpha = alpha / 255.0;
		s->row = row;
		s->column = column;
		new_stands[stands_i++] = s;
	}
	*stand_arr = new_stands;
	return num_stands;

	out_new_source:;
		free(s);
	out_new_stand:;
		free(name);
	out_name:;
		while (--stands_i >= 0) {
			del_stand(new_stands[stands_i]);
		}
		free(new_stands);
	out_stands:;
		*stand_arr = NULL;
		return 0;
}

static grid read_grid(FILE *f, uint32_t height,
	              uint32_t width, stand_like stand) {
	grid ng = new_grid(height, width);
	if (!ng)
		goto out_ng;

	int c;
	// exploits the row-major order of the lookup table
	tile *t = ng->lookup;
	while ((c = fgetc(f)) != EOF && c != ';' && c != ':') {
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

#define FILE_VERSION 1

bool save_file(FILE *f) {
	fprintf(f, "MMGS:%i;\n\n", FILE_VERSION);

	print_stand_templates(f);
	print_stands(f);

	fprintf(f, "maingrid(\n%" PRIu32 ":%" PRIu32 "\n)\n\n",
			main_grid->width, main_grid->height);

	return true;
}

static void print_stand_templates(FILE *f) {
	if (num_main_templates < 1)
		return;

	fprintf(f, "standtemplates[%" PRIi32 "](\n", num_main_templates);

	for (int32_t i = 0; i < num_main_templates; i++) {
		stand_template tt = main_templates + i;
		grid tgrid = tt->t;
		fprintf(f, "%zu:%s:%" PRIu8 ":%" PRIu8 ":%" PRIu8 ":%" PRIu8
			":%" PRIu32 ":%" PRIu32 ":\n",
				strlen(tt->name), tt->name,
				(uint8_t) (tt->red * 255.0),
				(uint8_t) (tt->green * 255.0),
				(uint8_t) (tt->blue * 255.0),
				(uint8_t) (tt->alpha * 255.0),
				tgrid->width, tgrid->height);
		print_grid(f, tgrid);
		fprintf(f, ";\n\n");
	}

	fprintf(f, ")\n\n");
}

static void print_grid(FILE *f, grid g) {
	// exploits the row-major order of the lookup table
	tile *t = g->lookup;
	uint64_t len = g->height * g->width;
	for (uint64_t i = 0; i < len; i++) {
		if ((*t)->stand.stand_proto.type == STAND) {
			if ((*t)->stand.stand_stand.s) {
				fprintf(f, "S");
			} else {
				fprintf(f, "0");
			}
		} else {
			if ((*t)->stand.stand_st.st) {
				fprintf(f, "S");
			} else {
				fprintf(f, "0");
			}
		}
		t++;
	}
}

static void print_stands(FILE *f) {
}

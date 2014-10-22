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

#include "save_n_load.h"

#define SCAN_WHITESPACE while(c = fgetc(f) && isspace(c))

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
	while (c = fgetc(f) && c != ';') {
		// non-numeric character?
		if (!isdigit(c)) return false;
		file_version = file_version * 10 + (c - '0');
	}

	SCAN_WHITESPACE;
	while (c) {
		char blockname[101];
		blockname[0] = c;
		int i = 0;
		while (++i < 100 && (c = fgetc(f)) && c != '(') {
			blockname[i] = c;
		}
		blockname[i] = '\0';
		
		if (strcmp("standtemplates", blockname) == 0) {
			// parse these
		} else if (strcmp("stands", blockname) == 0) {
			// go forth and parse
		} else if (strcmp("maingrid", blockname) == 0) {
			// gotta parse 'em all
		} else {
			// unrecognized block
			return false;
		}

		SCAN_WHITESPACE;
	}
	
	//grid new_main_grid;
}
/* main.c
 * 
 * This file contains the engine's main method, which serves as
 * the entry point for the entire application.
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

#include <stdlib.h>
#include <assert.h>

#include "global.h"
#include "grid.h"
#include "stand.h"
#include "capi.h"

int main(int argc, char* argv[]) {
	// initialize Mono runtime
	const char *filename = "frontend.exe";
	
	initialize_engine();
	initialize_mono(filename);
	return execute_frontend(argc, argv);
}
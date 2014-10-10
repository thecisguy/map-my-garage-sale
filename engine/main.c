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

#include <mono/jit/jit.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/object.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <stdlib.h>
#include <assert.h>

#include "global.h"
#include "grid.h"
#include "stand.h"
#include "capi.h"

grid main_grid;
MonoDomain *main_domain;
MonoAssembly *main_assembly;

int 
main(int argc, char* argv[]) {
	// initialize Mono runtime
	const char *filename = "frontend.exe";
	
	int retval;
	mono_config_parse (NULL);
	main_domain = mono_jit_init (filename);
	main_assembly = mono_domain_assembly_open(main_domain, filename);
	if (!main_assembly)
		exit (2);

	// initialize globals
	main_grid = new_grid(100u, 100u);

	// register internal calls
	register_api_functions();

	// run main method
	mono_jit_exec(main_domain, main_assembly, argc, argv);
	retval = mono_environment_exitcode_get();
	mono_jit_cleanup(main_domain);
	return retval;
}
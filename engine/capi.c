/* capi.h
 * 
 * This file contains definitions for the functions of the engine end
 * of the engine-frontend API.
 * 
 * This file can be considered the counterpart to csapi.cs.
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
#include <assert.h>

#include "global.h"
#include "grid.h"
#include "stand.h"
#include "capi.h"

static grid main_grid;
static stand selected_stand = NULL;
static MonoDomain *main_domain;
static MonoAssembly *main_assembly;

static MonoArray *get_color_of_tile(uint32_t row, uint32_t column);
static void debug_print_mono_info(MonoObject *obj);
static void register_api_functions(void);
static void select_stand(uint32_t row, uint32_t column);
static void deselect_stand(void);

static MonoArray *get_color_of_tile(uint32_t row, uint32_t column) {
	
	double red, blue, green, alpha;
	stand s = grid_lookup(main_grid, row, column)->s;
	if (s) {
		red = s->red;
		blue = s->blue;
		green = s->green;
		alpha = s->alpha;
	} else {
		red = TILE_EMPTY_RED;
		green = TILE_EMPTY_GREEN;
		blue = TILE_EMPTY_BLUE;
		alpha = TILE_EMPTY_ALPHA;
	}

	MonoArray *data = mono_array_new(main_domain, mono_get_double_class(), 4);
	mono_array_set(data, double, 0, red);
	mono_array_set(data, double, 1, green);
	mono_array_set(data, double, 2, blue);
	mono_array_set(data, double, 3, alpha);
	
	return data;
}

static void debug_print_mono_info(MonoObject *obj) {
	MonoClass *cl = mono_object_get_class(obj);
	MonoImage *im = mono_class_get_image(cl);
	printf("MonoClass Name: %s\n", mono_class_get_name(cl));

	void *v = NULL;
	MonoMethod *me = mono_class_get_methods(cl, &v);
	while(me != NULL) {
		printf("\tMonoMethod Name: %s\n", mono_method_get_name(me));
		me = mono_class_get_methods(cl, &v);
	}
	
	printf("MonoImage Name: %s\n", mono_image_get_name(im));
	printf("MonoImage File Name: %s\n", mono_image_get_filename(im));
}

void initialize_engine(void) {
	// initialize globals
	main_grid = new_grid(100u, 100u);
}

static void register_api_functions(void) {
	mono_add_internal_call("csapi.EngineAPI::getColorOfTileRaw",
	                       get_color_of_tile);
	mono_add_internal_call("MonoMain::DebugPrintMonoInfo",
	                       debug_print_mono_info);
	mono_add_internal_call("csapi.EngineAPI::selectStandRaw",
	                       select_stand);
	mono_add_internal_call("csapi.EngineAPI::deselectStandRaw",
	                       deselect_stand);
}

void initialize_mono(const char *filename) {
	mono_config_parse (NULL);
	main_domain = mono_jit_init (filename);
	main_assembly = mono_domain_assembly_open(main_domain, filename);
	assert(main_assembly);

	// register internal calls
	register_api_functions();
}

int execute_frontend(int argc, char* argv[]) {
	// run main method of frontend
	mono_jit_exec(main_domain, main_assembly, argc, argv);
	int retval = mono_environment_exitcode_get();
	mono_jit_cleanup(main_domain);
	return retval;
}

/* Selects a Stand from the given coordinates.
 * 
 * This corresponds to the user "clicking" a Stand in the frontend.
 * The indicated Stand will be used in all future calls to *_selected_*
 * methods until this method is called again.
 * 
 * If the coordinates of a blank tile are passed in, selected_stand
 * will be set to NULL. (This is desirable, as the user will probably
 * click on a blank tile when attempting to "deselect" a Stand.)
 */
static void select_stand(uint32_t row, uint32_t column) {
	selected_stand = grid_lookup(main_grid, row, column)->s;
}

/* Manually deselects the selected_stand.
 * 
 * For when deselecting the Stand is desired, e.g. the user clicks "off" the
 * Grid, presses Esc, etc.
 */
static void deselect_stand(void) {
	selected_stand = NULL;
}
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
#include <stdlib.h>
#include <time.h>
#include <inttypes.h>
#include <string.h>

#include "global.h"
#include "grid.h"
#include "stand.h"
#include "capi.h"
#include "save_n_load.h"

grid main_grid;
static stand selected_stand = NULL;
static stand grabbed_stand = NULL;
static MonoDomain *main_domain;
static MonoAssembly *main_assembly;
struct stand_template *main_templates = NULL;
int num_main_templates = 0;

static MonoArray *get_color_of_tile(uint32_t row, uint32_t column);
static void debug_print_mono_info(MonoObject *obj);
static void register_api_functions(void);
static MonoArray *select_stand(uint32_t row, uint32_t column);
static void deselect_stand(void);
static void rotate_selected_stand(mono_bool clockwise);
static void remove_selected_stand(void);
static void mirror_selected_stand(void);
static void grab_new_stand(int st_num);
static mono_bool can_apply_grabbed_stand(int64_t row, int64_t column);
static void do_apply_grabbed_stand(void);
static void remove_grabbed_stand(void);
static void grab_selected_stand(void);
static uint32_t get_main_grid_height(void);
static uint32_t get_main_grid_width(void);
static void load_user_file(MonoString *ufile);
static void set_selected_stand_name(MonoString *newname);
static MonoString *get_selected_stand_name(void);
static uint32_t get_selected_stand_height(void);
static uint32_t get_selected_stand_width(void);
static int32_t get_num_templates(void);
static MonoArray *get_color_of_st(int32_t st_id);
static void set_st_name(int32_t st_id, MonoString *newname);
static MonoString *get_st_name(int32_t st_id);
static void save_user_file(MonoString *ufile);

static MonoArray *get_color_of_tile(uint32_t row, uint32_t column) {
	
	double red, blue, green, alpha;
	stand s = grid_lookup(main_grid, row, column)->stand.stand_stand.s;
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
	main_grid = new_grid(400u, 400u);

	srand(time(NULL));
	
	FILE *def = fopen("default_sale.mmgs", "r");
	bool load_success = load_file(def);
	assert(load_success);
	//save_file(stdout);
	fclose(def);
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
	mono_add_internal_call("csapi.EngineAPI::rotateSelectedStandRaw",
	                       rotate_selected_stand);
	mono_add_internal_call("csapi.EngineAPI::removeSelectedStandRaw",
	                       remove_selected_stand);
	mono_add_internal_call("csapi.EngineAPI::mirrorSelectedStandRaw",
	                       mirror_selected_stand);
	mono_add_internal_call("csapi.EngineAPI::grabNewStandRaw",
	                       grab_new_stand);
	mono_add_internal_call("csapi.EngineAPI::canApplyGrabbedStandRaw",
	                       can_apply_grabbed_stand);
	mono_add_internal_call("csapi.EngineAPI::doApplyGrabbedStandRaw",
	                       do_apply_grabbed_stand);
	mono_add_internal_call("csapi.EngineAPI::removeGrabbedStandRaw",
	                       remove_grabbed_stand);
	mono_add_internal_call("csapi.EngineAPI::grabSelectedStandRaw",
	                       grab_selected_stand);
	mono_add_internal_call("csapi.EngineAPI::getMainGridHeightRaw",
	                       get_main_grid_height);
	mono_add_internal_call("csapi.EngineAPI::getMainGridWidthRaw",
	                       get_main_grid_width);
	mono_add_internal_call("csapi.EngineAPI::loadUserFileRaw",
	                       load_user_file);
	mono_add_internal_call("csapi.EngineAPI::setSelectedStandNameRaw",
	                       set_selected_stand_name);
	mono_add_internal_call("csapi.EngineAPI::getSelectedStandNameRaw",
	                       get_selected_stand_name);
	mono_add_internal_call("csapi.EngineAPI::getSelectedStandHeightRaw",
	                       get_selected_stand_height);
	mono_add_internal_call("csapi.EngineAPI::getSelectedStandWidthRaw",
	                       get_selected_stand_width);
	mono_add_internal_call("csapi.EngineAPI::getNumTemplatesRaw",
	                       get_num_templates);
	mono_add_internal_call("csapi.EngineAPI::getColorOfSTRaw",
	                       get_color_of_st);
	mono_add_internal_call("csapi.EngineAPI::getSTNameRaw",
	                       get_st_name);
	mono_add_internal_call("csapi.EngineAPI::setSTNameRaw",
	                       set_st_name);
	mono_add_internal_call("csapi.EngineAPI::saveUserFileRaw",
	                       save_user_file);
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
 *
 * Returns true if selected_stand was set to a stand, false if
 * it was set to NULL (Tile was empty).
 */
static MonoArray *select_stand(uint32_t row, uint32_t column) {
	selected_stand = grid_lookup(main_grid, row, column)->
		stand.stand_stand.s;
	MonoArray *data = mono_array_new(main_domain,
			mono_get_int64_class(), 3);
	mono_array_set(data, int64_t, 0,
			(int64_t) (selected_stand ? true : false));
	mono_array_set(data, int64_t, 1,
		selected_stand ? selected_stand->row : 0);
	mono_array_set(data, int64_t, 2,
		selected_stand ? selected_stand->column : 0);
	return data;
}

/* Manually deselects the selected_stand.
 * 
 * For when deselecting the Stand is desired, e.g. the user clicks "off" the
 * Grid, presses Esc, etc.
 */
static void deselect_stand(void) {
	selected_stand = NULL;
}

/* Rotates the selected Stand in the specified direction */
static void rotate_selected_stand(mono_bool clockwise) {
	assert(selected_stand);
	rotate_stand(selected_stand, (bool) clockwise);
}

/* Removes the selected Stand */
static void remove_selected_stand(void) {
	assert(select_stand);
	del_stand(selected_stand);
	selected_stand = NULL;
}

/* Mirrors the selected Stand */
static void mirror_selected_stand(void) {
	assert(select_stand);
	mirror_stand(selected_stand);
}

/* Creates a Stand from a Stand Template, and grabs it */
static void grab_new_stand(int32_t st_num) {
	assert(main_templates);
	assert(st_num < num_main_templates && st_num >= 0);
	grabbed_stand = new_stand(main_templates + st_num);
}

/* Checks the applicability of the grabbed stand onto the Main Grid
 * at the specified coordinates.
 */
static mono_bool can_apply_grabbed_stand(int64_t row, int64_t column) {
	if (!grabbed_stand) return (mono_bool) false;
	printf("engine got row: %" PRIi64 "\n", row);
	printf("engine got column: %" PRIi64 "\n", column);
	return (mono_bool) can_apply(grabbed_stand, main_grid, row, column);
}

/* Actually applies the grabbed stand.
 * can_apply_grabbed_stand must have been previously called.
 */
static void do_apply_grabbed_stand(void) {
	do_apply(grabbed_stand);
	selected_stand = grabbed_stand;
	grabbed_stand = NULL;
}

/* Deletes the grabbed stand.
 */
static void remove_grabbed_stand(void) {
	if (!grabbed_stand) return;
	del_stand(grabbed_stand);
	grabbed_stand = NULL;
}

/* Grabs the selected stand, by first lifting it from the Main Grid.
 */
static void grab_selected_stand(void) {
	if (!selected_stand) return;
	remove_stand(selected_stand);
	selected_stand->g = NULL;
	if (grabbed_stand)
		remove_grabbed_stand();
	grabbed_stand = selected_stand;
	selected_stand = NULL;
}

/* Returns height of Main Grid */
static uint32_t get_main_grid_height(void) {
	return main_grid->height;
}

/* Returns height of Main Grid */
static uint32_t get_main_grid_width(void) {
	return main_grid->width;
}

/* Loads a file from user input in the frontend */
static void load_user_file(MonoString *ufile) {
	char *filename = mono_string_to_utf8(ufile);
	FILE *userfile = fopen(filename, "r");
	assert(userfile);
	load_file(userfile);
	fclose(userfile);
	mono_free(filename);
}

/* Sets the Selected Stand's name to the given string */
static void set_selected_stand_name(MonoString *newname) {
	assert(selected_stand);
	char *mononame = mono_string_to_utf8(newname);
	// we duplicate this because the string from mono
	// requires mono_free, which doesn't jive with our other code
	char *cname = (char *) malloc(sizeof(char) * (strlen(mononame) + 1));
	if (!cname)
		goto out_mononame;
	strcpy(cname, mononame);

	if (selected_stand->name)
		free(selected_stand->name);
	selected_stand->name = cname;
	
	out_mononame:
		mono_free(mononame);
}

/* Returns the height of the Selected Stand's source grid */
static uint32_t get_selected_stand_height(void) {
	assert(selected_stand);
	return selected_stand->source->height;
}

/* Returns the width of the Selected Stand's source grid */
static uint32_t get_selected_stand_width(void) {
	assert(selected_stand);
	return selected_stand->source->width;
}

/* Returns the number of known Stand Templates */
static int32_t get_num_templates(void) {
	return num_main_templates;
}

/* Returns the color of the given stand template */
static MonoArray *get_color_of_st(int32_t st_id) {
	assert(st_id < num_main_templates);
	
	double red, blue, green, alpha;
	stand_template s = main_templates + st_id;
	red = s->red;
	blue = s->blue;
	green = s->green;
	alpha = s->alpha;

	MonoArray *data = mono_array_new(main_domain, mono_get_double_class(), 4);
	mono_array_set(data, double, 0, red);
	mono_array_set(data, double, 1, green);
	mono_array_set(data, double, 2, blue);
	mono_array_set(data, double, 3, alpha);
	
	return data;
}

/* Sets the given stand template's name to the given string */
static void set_st_name(int32_t st_id, MonoString *newname) {
	assert(st_id < num_main_templates);
	char *mononame = mono_string_to_utf8(newname);
	// we duplicate this because the string from mono
	// requires mono_free, which doesn't jive with our other code
	char *cname = (char *) malloc(sizeof(char) * (strlen(mononame) + 1));
	if (!cname)
		goto out_mononame;
	strcpy(cname, mononame);

	stand_template st = main_templates + st_id;
	
	if (st->name)
		free(st->name);
	st->name = cname;

	out_mononame:
		mono_free(mononame);
}

/* Return the Selected Stand's name as a MonoString */
static MonoString *get_selected_stand_name(void) {
	assert(selected_stand);
	return mono_string_new(main_domain, selected_stand->name);
}
/* Return the given stand template's name as a MonoString */
static MonoString *get_st_name(int32_t st_id) {
	assert(st_id < num_main_templates);
	return mono_string_new(main_domain, (main_templates + st_id)->name);
}

/* Saves a file from user input in the frontend */
static void save_user_file(MonoString *ufile) {
	char *filename = mono_string_to_utf8(ufile);
	FILE *userfile = fopen(filename, "w");
	assert(userfile);
	printf("saving to %s\n", filename);
	save_file(userfile);
	fclose(userfile);
	mono_free(filename);
}
#include <mono/jit/jit.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/object.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <stdlib.h>
#include <assert.h>

#include "grid.h"
#include "global.h"
#include "stand.h"

static MonoArray *get_color_of_tile(MonoObject *o,
                                     uint32_t row, uint32_t column);
static void debug_print_mono_info(MonoObject *obj); 

grid main_grid;
static MonoDomain *main_domain;
static MonoAssembly *main_assembly;

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
	mono_add_internal_call("api.EngineAPI::getColorOfTileRaw",
	                       get_color_of_tile);
	mono_add_internal_call("MonoMain::DebugPrintMonoInfo",
	                       debug_print_mono_info);

	// run main method
	mono_jit_exec(main_domain, main_assembly, argc, argv);
	retval = mono_environment_exitcode_get();
	mono_jit_cleanup(main_domain);
	return retval;
}


// TODO: find out why we are passed an object reference even though we
// declared the function static in unmanagedhelpers.cs
static MonoArray *get_color_of_tile(MonoObject *o,
                                     uint32_t row, uint32_t column) {
	
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

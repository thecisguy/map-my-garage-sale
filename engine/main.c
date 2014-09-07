#include <mono/jit/jit.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <stdlib.h>

#include "grid.h"
#include "global.h"

static MonoObject *get_color_of_tile(unsigned int row, unsigned int column);

grid main_grid;
static MonoDomain *main_domain;
static MonoAssembly *main_assembly;

int 
main(int argc, char* argv[]) {
	// initialize Mono runtime
	const char *file = "main.exe";
	int retval;
	mono_config_parse (NULL);
	main_domain = mono_jit_init (file);
	main_assembly = mono_domain_assembly_open(main_domain, file);
	if (!main_assembly)
		exit (2);

	// initialize globals
	main_grid = new_grid(100u, 100u);

	// register internal calls
	mono_add_internal_call("MonoMain::getColorOfTile", get_color_of_tile);

	// run main method
	mono_jit_exec(main_domain, main_assembly, argc, argv);
	retval = mono_environment_exitcode_get();
	mono_jit_cleanup(main_domain);
	return retval;
}

static MonoObject *get_color_of_tile(unsigned int row, unsigned int column) {
	MonoImage *image = mono_assembly_get_image(main_assembly);
	MonoClass *cairo_color = mono_class_from_name(image, "Cairo", "Color");
	MonoMethod *cairo_color_ctor =
		mono_class_get_method_from_name(cairo_color, ".ctor", 4);
	
	MonoObject *new_color = mono_object_new(main_domain, cairo_color);
	mono_runtime_object_init(new_color);

	void *cairo_color_ctor_args[4];
	for (int i = 0; i < 4; i++) {
		cairo_color_ctor_args[i] = malloc(sizeof(double));
		*((double *)(cairo_color_ctor_args[i])) = 0.5; 
	}

	mono_runtime_invoke (cairo_color_ctor, new_color,
	                     cairo_color_ctor_args, NULL);

	return new_color;
}
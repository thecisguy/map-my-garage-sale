#include <mono/jit/jit.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/mono-config.h>
#include <stdlib.h>

int 
main(int argc, char* argv[]) {
	MonoDomain *domain;
	const char *file = "main.exe";
	int retval;

	// load default Mono config
	mono_config_parse (NULL);

	// load mono domain
	domain = mono_jit_init (file);

	// load mono assembly
	MonoAssembly *assembly;
	assembly = mono_domain_assembly_open (domain, file);

	if (!assembly)
		exit (2);

	// run main method
	mono_jit_exec (domain, assembly, argc, argv);
	
	retval = mono_environment_exitcode_get ();
	
	mono_jit_cleanup (domain);
	return retval;
}


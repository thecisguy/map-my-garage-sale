#ifndef GLOBAL_H
#define GLOBAL_H

/* For mono typedefs: this header contains (usually stdint.h-style)
 * aliases for C# primitives.
 */
#include <mono/utils/mono-publib.h>

#include "grid.h"

/* Color constants are defined at the base level as integers between
 * 0-255 (HTML style). This makes it easier to modify the values. We
 * divide them by 255.0 to get the double in the range of 0.0 - 1.0
 * that Cairo expects.
 */

#define TILE_EMPTY_RED_HTML		225
#define TILE_EMPTY_GREEN_HTML   225
#define TILE_EMPTY_BLUE_HTML	200

#define TILE_EMPTY_RED			(TILE_EMPTY_RED_HTML / 255.0)
#define TILE_EMPTY_GREEN		(TILE_EMPTY_GREEN_HTML / 255.0)
#define TILE_EMPTY_BLUE			(TILE_EMPTY_BLUE_HTML / 255.0)

#define TILE_EMPTY_ALPHA 1.0

extern grid main_grid;

#endif
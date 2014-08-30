#include <stdlib.h>
#include <stdio.h>

struct tile {
	struct tile *up;
	struct tile *right;
	struct tile *left;
	struct tile *down;
};

struct grid {
	struct tile *origin;
	unsigned int height;
	unsigned int width;
};

typedef struct tile *tile;
typedef struct grid *grid;

tile new_tile(void) {
	tile nt = malloc(sizeof(struct tile));
	if (nt == NULL)
		goto out_nt;

	nt->up = NULL;
	nt->down = NULL;
	nt->left = NULL;
	nt->right = NULL;

	return nt;

out_nt:
	return NULL;
}

grid new_grid(unsigned int width, unsigned int height) {
	if (width < 1 || height < 1) {
		fprintf(stderr, "error in new_grid, bad params: [%u,%u]",
				width, height);
	}

	tile rightends[height]; // for error handling
	for (int i = 0; i < height; i++)
		rightends[i] = NULL;

	grid ng = malloc(sizeof(struct grid));
	if (ng == NULL)
		goto out_ng;
	ng->origin = NULL;
	ng->height = height;
	ng->width = width;

	for (int j = 0; j < height; j++) {
		tile above;
		tile farleft;
		if (j == 0) {
			// create first tile
			rightends[j] = ng->origin;
			ng->origin = new_tile();
			if (ng->origin == NULL)
				goto out_tiles;
			above = NULL;
			farleft = ng->origin;
		} else {
			tile newrow = new_tile();
			if (newrow == NULL)
				goto out_tiles;
			rightends[j] = newrow;
			farleft->down = newrow;
			newrow->up = farleft;
			above = farleft->right;
			farleft = newrow;
		}

		tile t = farleft;
		for (int i = 1; i < width; i++, rightends[j] = t) {
			tile n = new_tile();
			if (n == NULL)
				goto out_tiles;
			n->up = above;
			above->down = n;
			t->right = n;
			n->left = t;
			t = n;
			if (j != 0)
				above = above->right;
		}
	}

	return ng;

out_tiles:;
	int endi = 0;
	while (rightends[endi] != NULL && endi < height) {
		tile rend = rightends[endi];
		while (rend != NULL) {
			tile l = rend->left;
			free(rend);
			rend = l;
		}
		endi++;
	}
	free(ng);
out_ng:;
	return NULL;
}

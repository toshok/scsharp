
#include <stdio.h>

#define FT_FREETYPE_H <freetype/freetype.h>
#include FT_FREETYPE_H

FT_Library aLibrary;
FT_Face aface;

int
main (int argc, char **argv)
{
  if (0 != FT_Init_FreeType (&aLibrary))
    printf ("FT_Init_FreeType failed\n");

  if (0 != FT_New_Face (aLibrary, argv[1], 0, &aface))
    printf ("FT_New_Face failed\n");

  printf ("face: %s\n", aface->family_name);
  printf ("style: %s\n", aface->style_name);
}

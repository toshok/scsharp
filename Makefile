
all:
	$(MAKE) -C Starcraft all

MCSFLAGS= -debug
MCS= gmcs

Starcraft_dll_FILES= GRP.cs Util.cs MPQ.cs TBL.cs Game.cs Builtins.cs Painter.cs CursorAnimator.cs Palette.cs BIN.cs UIScreen.cs MainMenu.cs UIPainter.cs Race.cs SwooshPainter.cs ScoreScreen.cs GameScreen.cs CHK.cs ImagesDat.cs GlobalResources.cs IScriptBin.cs IScriptRunner.cs SpritesDat.cs Sprite.cs SpriteManager.cs SMK.cs
Starcraft_dll_REFS= -pkg:gtk-sharp-2.0

dump_grp_exe_FILES = BMP.cs dump-grp.cs
dump_grp_exe_REFS= -r:Starcraft.dll

dump_map_exe_FILES = TGA.cs dump-map.cs
dump_map_exe_REFS= -r:Starcraft.dll

dump_tileset_exe_FILES = BMP.cs dump-tileset.cs
dump_tileset_exe_REFS= -r:Starcraft.dll

animate_grp_exe_FILES = animate-grp.cs
animate_grp_exe_REFS= -r:Starcraft.dll -pkg:gtk-sharp-2.0

run_animation_exe_FILES = run-animation.cs
run_animation_exe_REFS= -r:Starcraft.dll -pkg:gtk-sharp-2.0

run_smk_exe_FILES = TGA.cs run-smk.cs
run_smk_exe_REFS= -r:Starcraft.dll -pkg:gtk-sharp-2.0

dump_tbl_exe_FILES = dump-tbl.cs
dump_tbl_exe_REFS= -r:Starcraft.dll

starcraft_exe_FILES = starcraft.cs
starcraft_exe_REFS= -r:Starcraft.dll -pkg:gtk-sharp-2.0

all: dump-map.exe dump-grp.exe dump-tileset.exe animate-grp.exe dump-tbl.exe starcraft.exe font-info run-animation.exe run-smk.exe

dump-grp.exe: Starcraft.dll $(dump_grp_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_grp_exe_FILES) $(dump_grp_exe_REFS)

dump-map.exe: Starcraft.dll $(dump_map_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_map_exe_FILES) $(dump_map_exe_REFS)

dump-tileset.exe: Starcraft.dll $(dump_tileset_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_tileset_exe_FILES) $(dump_tileset_exe_REFS)

animate-grp.exe: Starcraft.dll $(animate_grp_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(animate_grp_exe_FILES) $(animate_grp_exe_REFS)

run-animation.exe: Starcraft.dll $(run_animation_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(run_animation_exe_FILES) $(run_animation_exe_REFS)

run-smk.exe: Starcraft.dll $(run_smk_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(run_smk_exe_FILES) $(run_smk_exe_REFS)

dump-tbl.exe: Starcraft.dll $(dump_tbl_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_tbl_exe_FILES) $(dump_tbl_exe_REFS)

starcraft.exe: Starcraft.dll $(starcraft_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(starcraft_exe_FILES) $(starcraft_exe_REFS)

font-info: font-info.c
	$(CC) $(shell pkg-config --cflags freetype2) font-info.c -o font-info $(shell pkg-config --libs freetype2)

Starcraft.dll: $(Starcraft_dll_FILES)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ $(Starcraft_dll_FILES) $(Starcraft_dll_REFS)

clean:
	rm -f *.exe *.mdb *.dll font-info
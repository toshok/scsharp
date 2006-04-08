MCSFLAGS= -debug
MCS= gmcs

dump_grp_exe_FILES = BMP.cs dump-grp.cs
dump_grp_exe_REFS= -r:../Starcraft/Starcraft.dll

dump_map_exe_FILES = TGA.cs dump-map.cs
dump_map_exe_REFS= -r:../Starcraft/Starcraft.dll

dump_tileset_exe_FILES = BMP.cs dump-tileset.cs
dump_tileset_exe_REFS= -r:../Starcraft/Starcraft.dll

animate_grp_exe_FILES = animate-grp.cs
animate_grp_exe_REFS= -r:../Starcraft/Starcraft.dll -pkg:gtk-sharp-2.0

run_animation_exe_FILES = run-animation.cs
run_animation_exe_REFS= -r:../Starcraft/Starcraft.dll -r:../Starcraft/Starcraft.Gui.dll -r:../sdldotnet/bin/assemblies/SdlDotNet.dll -r:../sdldotnet/lib/Tao.Sdl.dll

run_smk_exe_FILES = TGA.cs run-smk.cs
run_smk_exe_REFS= -r:../Starcraft/Starcraft.dll -r:../Starcraft/Starcraft.Gui.dll -pkg:gtk-sharp-2.0

dump_tbl_exe_FILES = dump-tbl.cs
dump_tbl_exe_REFS= -r:../Starcraft/Starcraft.dll

font_foo_exe_FILES = font-foo.cs
font_foo_exe_REFS= -r:../Starcraft/Starcraft.dll -r:../Starcraft/Starcraft.Gui.dll -r:../sdldotnet/bin/assemblies/SdlDotNet.dll -r:../sdldotnet/lib/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

all: dump-map.exe dump-grp.exe dump-tileset.exe animate-grp.exe dump-tbl.exe font-info run-animation.exe run-smk.exe font-foo.exe

dump-grp.exe: $(dump_grp_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_grp_exe_FILES) $(dump_grp_exe_REFS)

dump-map.exe: $(dump_map_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_map_exe_FILES) $(dump_map_exe_REFS)

dump-tileset.exe: $(dump_tileset_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_tileset_exe_FILES) $(dump_tileset_exe_REFS)

animate-grp.exe: $(animate_grp_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(animate_grp_exe_FILES) $(animate_grp_exe_REFS)

run-animation.exe: $(run_animation_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(run_animation_exe_FILES) $(run_animation_exe_REFS)

run-smk.exe: $(run_smk_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(run_smk_exe_FILES) $(run_smk_exe_REFS)

dump-tbl.exe: $(dump_tbl_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(dump_tbl_exe_FILES) $(dump_tbl_exe_REFS)

font-foo.exe: $(font_foo_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(font_foo_exe_FILES) $(font_foo_exe_REFS)

starcraft.exe: $(starcraft_exe_FILES)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $(starcraft_exe_FILES) $(starcraft_exe_REFS)

font-info: font-info.c
	$(CC) $(shell pkg-config --cflags freetype2) font-info.c -o font-info $(shell pkg-config --libs freetype2)

Starcraft.dll: $(Starcraft_dll_FILES)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ $(Starcraft_dll_FILES) $(Starcraft_dll_REFS)

clean:
	rm -f *.exe *.mdb *.dll font-info
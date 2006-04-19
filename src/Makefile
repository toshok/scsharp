MCSFLAGS= -debug
MCS= gmcs

PROGRAM=starcraft.exe
PROGRAM_REFS= -r:../SCSharp/SCSharp.Gui.dll -r:../SCSharp/SCSharp.dll -r:../sdldotnet/bin/assemblies/SdlDotNet.dll -r:../sdldotnet/lib/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

all: $(PROGRAM)

$(PROGRAM): $(LIBRARY)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ @$(PROGRAM_SOURCES) $(PROGRAM_REFS)

clean:
	$(RM) -f $(PROGRAM) $(PROGRAM).mdb

PROGRAM_SOURCES=$(PROGRAM).sources

program_frag=.deps/$(PROGRAM).makefrag

$(program_frag): $(PROGRAM_SOURCES)
	@echo Creating $@ ...
	@sed 's,^,$(PROGRAM): ,' $< > $@

-include $(program_frag)

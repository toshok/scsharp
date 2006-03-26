MCSFLAGS= -debug
MCS= gmcs

PROGRAM=starcraft.exe
PROGRAM_REFS= -r:../Starcraft/Starcraft.Gui.dll -r:../Starcraft/Starcraft.dll -pkg:gtk-sharp-2.0

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

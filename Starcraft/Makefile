MCSFLAGS= -debug
MCS= gmcs

LIBRARY=Starcraft.dll
LIBRARY_REFS= -pkg:gtk-sharp-2.0

PROGRAM=starcraft.exe
PROGRAM_REFS= -r:Starcraft.dll -pkg:gtk-sharp-2.0

all: $(PROGRAM)

$(PROGRAM): $(LIBRARY)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ @$(PROGRAM_SOURCES) $(PROGRAM_REFS)

$(LIBRARY):
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(LIBRARY_SOURCES) $(LIBRARY_REFS)


clean:
	$(RM) -f $(PROGRAM) $(LIBRARY) $(PROGRAM).mdb $(LIBRARY).mdb

LIBRARY_SOURCES=$(LIBRARY).sources
PROGRAM_SOURCES=$(PROGRAM).sources

program_frag=.deps/$(PROGRAM).makefrag
library_frag=.deps/$(LIBRARY).makefrag

$(program_frag): $(PROGRAM_SOURCES)
	@echo Creating $@ ...
	@sed 's,^,$(PROGRAM): ,' $< > $@

$(library_frag): $(LIBRARY_SOURCES)
	@echo Creating $@ ...
	@sed 's,^,$(LIBRARY): ,' $< > $@

-include $(program_frag)
-include $(library_frag)
MCSFLAGS= -debug
MCS= gmcs

LIBRARY1=Starcraft.dll
LIBRARY1_REFS=

LIBRARY2=Starcraft.Gui.dll
LIBRARY2_REFS= -r:Starcraft.dll -r:../sdldotnet/bin/assemblies/SdlDotNet.dll -r:../sdldotnet/lib/Tao.Sdl.dll -r:System.Drawing.dll

LIBRARY2_RES=/resource:Starcraft.Gui/credits.txt,credits.txt

all: $(LIBRARY2)

$(LIBRARY2): $(LIBRARY1)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(LIBRARY2_SOURCES) $(LIBRARY2_RES) $(LIBRARY2_REFS)

$(LIBRARY1):
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(LIBRARY1_SOURCES) $(LIBRARY1_RES) $(LIBRARY1_REFS)


clean:
	$(RM) -f $(LIBRARY2) $(LIBRARY1) $(LIBRARY2).mdb $(LIBRARY1).mdb

LIBRARY1_SOURCES=$(LIBRARY1).sources
LIBRARY2_SOURCES=$(LIBRARY2).sources

library2_frag=.deps/$(LIBRARY2).makefrag
library1_frag=.deps/$(LIBRARY1).makefrag

$(library2_frag): $(LIBRARY2_SOURCES)
	@echo Creating $@ ...
	@sed 's,^,$(LIBRARY2): ,' $< > $@

$(library1_frag): $(LIBRARY1_SOURCES)
	@echo Creating $@ ...
	@sed 's,^,$(LIBRARY1): ,' $< > $@

-include $(library2_frag)
-include $(library1_frag)
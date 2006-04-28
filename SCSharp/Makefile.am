MCSFLAGS= -debug -d:DEBUG
MCS=gmcs

SCSHARPDLL=SCSharp.dll
SCSHARPDLL_REFS=

SCSHARPUIDLL=SCSharp.UI.dll
SCSHARPUIDLL_REFS= -r:SCSharp.dll -r:../sdldotnet-bin/SdlDotNet.dll -r:../sdldotnet-bin/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

SCSHARPUIDLL_RES=SCSharp.UI/credits.txt
# XXX should autogenerate the next one from the previous one
SCSHARPUIDLL_RESFLAGS=/resource:SCSharp.UI/credits.txt,credits.txt

all-local: $(SCSHARPDLL) $(SCSHARPUIDLL)

clean-local:
	-rm -f *.dll

$(SCSHARPUIDLL): $(SCSHARPDLL)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPUIDLL_SOURCE_LIST) $(SCSHARPUIDLL_RESFLAGS) $(SCSHARPUIDLL_REFS)

$(SCSHARPDLL):
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPDLL_SOURCE_LIST) $(SCSHARPDLL_RESFLAGS) $(SCSHARPDLL_REFS)

SCSHARPDLL_SOURCE_LIST=$(SCSHARPDLL).sources
SCSHARPUIDLL_SOURCE_LIST=$(SCSHARPUIDLL).sources

library2_frag=.deps/$(SCSHARPUIDLL).makefrag
library1_frag=.deps/$(SCSHARPDLL).makefrag

$(library2_frag): $(SCSHARPUIDLL_SOURCE_LIST)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPUIDLL): ,' $< > $@

$(library1_frag): $(SCSHARPDLL_SOURCE_LIST)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPDLL): ,' $< > $@

-include $(library2_frag)
-include $(library1_frag)

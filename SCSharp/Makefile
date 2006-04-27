MCSFLAGS= -debug -d:DEBUG
MCS= gmcs

SCSHARPDLL=SCSharp.dll
SCSHARPDLL_REFS=

SCSHARPUIDLL=SCSharp.UI.dll
SCSHARPUIDLL_REFS= -r:SCSharp.dll -r:../sdldotnet-bin/SdlDotNet.dll -r:../sdldotnet-bin/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

SCSHARPUIDLL_RES=SCSharp.UI/credits.txt
# XXX should autogenerate the next one from the previous one
SCSHARPUIDLL_RESFLAGS=/resource:SCSharp.UI/credits.txt,credits.txt

all: $(SCSHARPUIDLL)

$(SCSHARPUIDLL): $(SCSHARPDLL)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPUIDLL_SOURCES) $(SCSHARPUIDLL_RESFLAGS) $(SCSHARPUIDLL_REFS)

$(SCSHARPDLL):
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPDLL_SOURCES) $(SCSHARPDLL_RESFLAGS) $(SCSHARPDLL_REFS)


clean:
	$(RM) -f $(SCSHARPUIDLL) $(SCSHARPDLL) $(SCSHARPUIDLL).mdb $(SCSHARPDLL).mdb

dist:
	@mkdir $(DISTDIR)
	@mkdir $(DISTDIR)/SCSharp.Mpq
	@mkdir $(DISTDIR)/SCSharp.UI
	@mkdir $(DISTDIR)/SCSharp.Util
	@cp Makefile $(SCSHARPDLL_SOURCES) $(SCSHARPUIDLL_SOURCES) $(DISTDIR)
	@for i in `cat $(SCSHARPDLL_SOURCES)`; do cp $$i $(DISTDIR)/$$i; done
	@for i in `cat $(SCSHARPUIDLL_SOURCES)`; do cp $$i $(DISTDIR)/$$i; done
	@for i in $(SCSHARPUIDLL_RES); do cp $$i $(DISTDIR)/$$i; done

SCSHARPDLL_SOURCES=$(SCSHARPDLL).sources
SCSHARPUIDLL_SOURCES=$(SCSHARPUIDLL).sources

library2_frag=.deps/$(SCSHARPUIDLL).makefrag
library1_frag=.deps/$(SCSHARPDLL).makefrag

$(library2_frag): $(SCSHARPUIDLL_SOURCES)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPUIDLL): ,' $< > $@

$(library1_frag): $(SCSHARPDLL_SOURCES)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPDLL): ,' $< > $@

-include $(library2_frag)
-include $(library1_frag)

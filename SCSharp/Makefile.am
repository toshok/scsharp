MCSFLAGS= -debug -d:DEBUG
MCS=gmcs

SCSHARPDLL=SCSharp.dll
SCSHARPDLL_REFS=-r:ICSharpCode.SharpZipLib

SCSHARPSMKDLL=SCSharp.Smk.dll
SCSHARPSMKDLL_REFS=-r:SCSharp.dll -r:../sdldotnet-bin/Tao.Sdl.dll -r:System.Drawing.dll

SCSHARPUIDLL=SCSharp.UI.dll
SCSHARPUIDLL_REFS= -r:SCSharp.dll -r:SCSharp.Smk.dll -r:../sdldotnet-bin/SdlDotNet.dll -r:../sdldotnet-bin/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

SCSHARPUIDLL_RES=SCSharp.UI/credits.txt
# XXX should autogenerate the next one from the previous one
SCSHARPUIDLL_RESFLAGS=/resource:SCSharp.UI/credits.txt,credits.txt

all-local: $(SCSHARPDLL) $(SCSHARPSMKDLL) $(SCSHARPUIDLL)

clean-local:
	-rm -f *.dll

dist-hook:
	mkdir $(distdir)/SCSharp.Mpq
	mkdir $(distdir)/SCSharp.Mpq.Smk
	mkdir $(distdir)/SCSharp.UI
	mkdir $(distdir)/SCSharp.Util
	cp -p $(SCSHARPDLL_SOURCE_LIST) $(SCSHARPSMKDLL_SOURCE_LIST) $(SCSHARPUIDLL_SOURCE_LIST) $(distdir)
	for i in `cat $(SCSHARPDLL_SOURCE_LIST)`; do cp -p $$i $(distdir)/$$i; done
	for i in `cat $(SCSHARPSMKDLL_SOURCE_LIST)`; do cp -p $$i $(distdir)/$$i; done
	for i in `cat $(SCSHARPUIDLL_SOURCE_LIST)`; do cp -p $$i $(distdir)/$$i; done
	for i in $(SCSHARPUIDLL_RES); do cp -p $$i $(distdir)/$$i; done

$(SCSHARPUIDLL): $(SCSHARPDLL)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPUIDLL_SOURCE_LIST) $(SCSHARPUIDLL_RESFLAGS) $(SCSHARPUIDLL_REFS)

$(SCSHARPDLL):
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPDLL_SOURCE_LIST) $(SCSHARPDLL_RESFLAGS) $(SCSHARPDLL_REFS)

$(SCSHARPSMKDLL):
	$(MCS) $(MCSFLAGS) -target:library -out:$@ @$(SCSHARPSMKDLL_SOURCE_LIST) $(SCSHARPSMKDLL_RESFLAGS) $(SCSHARPSMKDLL_REFS)

SCSHARPDLL_SOURCE_LIST=$(SCSHARPDLL).sources
SCSHARPSMKDLL_SOURCE_LIST=$(SCSHARPSMKDLL).sources
SCSHARPUIDLL_SOURCE_LIST=$(SCSHARPUIDLL).sources

library3_frag=.deps/$(SCSHARPSMKDLL).makefrag
library2_frag=.deps/$(SCSHARPUIDLL).makefrag
library1_frag=.deps/$(SCSHARPDLL).makefrag

$(library3_frag): $(SCSHARPSMKDLL_SOURCE_LIST)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPSMKDLL): ,' $< > $@

$(library2_frag): $(SCSHARPUIDLL_SOURCE_LIST)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPUIDLL): ,' $< > $@

$(library1_frag): $(SCSHARPDLL_SOURCE_LIST)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(SCSHARPDLL): ,' $< > $@

-include $(library3_frag)
-include $(library2_frag)
-include $(library1_frag)

MCSFLAGS= -debug
MCS= gmcs

PROGRAM=starcraft.exe
PROGRAM_REFS= -r:../SCSharp/SCSharp.UI.dll -r:../SCSharp/SCSharp.dll -r:../sdldotnet-bin/SdlDotNet.dll -r:../sdldotnet-bin/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

EXTRA_DIST=starcraft.exe.config-example mono.sh

all-local: $(PROGRAM)

clean-local:
	-rm -f $(PROGRAM)

dist-hook:
	cp -p $(PROGRAM_SOURCE_LIST) $(distdir)
	for i in `cat $(PROGRAM_SOURCE_LIST)`; do cp -p $$i $(distdir)/$$i; done

$(PROGRAM): $(LIBRARY)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ @$(PROGRAM_SOURCE_LIST) $(PROGRAM_REFS)

PROGRAM_SOURCE_LIST=$(PROGRAM).sources

program_frag=.deps/$(PROGRAM).makefrag

$(program_frag): $(PROGRAM_SOURCE_LIST)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(PROGRAM): ,' $< > $@

-include $(program_frag)

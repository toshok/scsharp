MCSFLAGS= -debug
MCS= gmcs

PROGRAM=starcraft.exe
PROGRAM_REFS= -r:../SCSharp/SCSharp.UI.dll -r:../SCSharp/SCSharp.dll -r:../sdldotnet-bin/SdlDotNet.dll -r:../sdldotnet-bin/Tao.Sdl.dll -r:System.Drawing.dll -r:System.Configuration.dll

EXTRA_DIST=starcraft.exe.config-example mono.sh

all: $(PROGRAM)

$(PROGRAM): $(LIBRARY)
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ @$(PROGRAM_SOURCES) $(PROGRAM_REFS)

clean:
	$(RM) -f $(PROGRAM) $(PROGRAM).mdb

dist:
	@mkdir $(DISTDIR)
	@cp Makefile $(PROGRAM_SOURCES) $(EXTRA_DIST) $(DISTDIR)
	@for i in `cat $(PROGRAM_SOURCES)`; do cp $$i $(DISTDIR)/$$i; done

PROGRAM_SOURCES=$(PROGRAM).sources

program_frag=.deps/$(PROGRAM).makefrag

$(program_frag): $(PROGRAM_SOURCES)
	@echo Creating $@ ...
	@-mkdir .deps
	@sed 's,^,$(PROGRAM): ,' $< > $@

-include $(program_frag)

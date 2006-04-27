VERSION=0.0000004
DISTDIR=scsharp-src-$(VERSION)
RELEASEDIR=scsharp-linux-$(VERSION)

SUBDIRS = SCSharp StormLib src

all::

all clean::
	@for dir in $(SUBDIRS); do \
		echo "Making $@ in $$dir"; \
		$(MAKE) -C $$dir $@; \
	done

dist:: dist-local dist-recurse dist-finish

dist-local:
	@rm -rf $(DISTDIR).tar.gz
	@rm -rf $(DISTDIR)
	@mkdir $(DISTDIR)
	@mkdir $(DISTDIR)/sdldotnet-bin
	@cp Makefile RELEASE_README ChangeLog $(DISTDIR)
	@cp sdldotnet-bin/*.dll sdldotnet-bin/*.dll.config $(DISTDIR)/sdldotnet-bin

dist-recurse:
	@for dir in $(SUBDIRS); do \
		echo "Making dist in $$dir"; \
		$(MAKE) DISTDIR="../$(DISTDIR)/$$dir" -C $$dir dist; \
	done

dist-finish:
	tar -cvzf $(DISTDIR).tar.gz $(DISTDIR)
	rm -rf $(DISTDIR)

#dll configs?
release: all
	-mkdir $(RELEASEDIR)
	@cp src/starcraft.exe $(RELEASEDIR)
	@cp src/starcraft.exe.config-example $(RELEASEDIR)/starcraft.exe.config
	@cp SCSharp/*.dll $(RELEASEDIR)
	@cp StormLib/libStorm.so $(RELEASEDIR)
	@cp sdldotnet/bin/examples/SdlDotNet.dll $(RELEASEDIR)
	@cp sdldotnet/bin/examples/Tao.Sdl.dll $(RELEASEDIR)
	@cp sdldotnet/bin/examples/Tao.Sdl.dll.config $(RELEASEDIR)
	@cp RELEASE_README $(RELEASEDIR)/README
	tar -cvzf $(RELEASEDIR).tar.gz $(RELEASEDIR)
	rm -rf $(RELEASEDIR)


SUBDIRS= docs SCSharp src Tests

if ENABLE_STORMLIB
SUBDIRS += StormLib
endif

EXTRA_DIST= HACKING RELEASE_README

install:
	@echo \"make install\" doesn\'t work yet.  try again later.

dist-hook:
	mkdir -p $(distdir)/sdldotnet-bin
	cp sdldotnet-bin/* $(distdir)/sdldotnet-bin

#dll configs?
RELEASEDIR=$(PACKAGE)-$(VERSION)
release: all
	-mkdir $(RELEASEDIR)
	@cp src/scsharp.exe $(RELEASEDIR)
	@cp src/scsharp.exe.config-example $(RELEASEDIR)/scsharp.exe.config
	@cp SCSharp/*.dll $(RELEASEDIR)
	@cp sdldotnet-bin/*.dll $(RELEASEDIR)
	@cp sdldotnet-bin/*.dll.config $(RELEASEDIR)
	@cp NEWS $(RELEASEDIR)/NEWS
	@cp RELEASE_README $(RELEASEDIR)/README
	@cp AUTHORS $(RELEASEDIR)
	tar -cvjf $(RELEASEDIR).tar.bz2 $(RELEASEDIR)
	rm -rf $(RELEASEDIR)

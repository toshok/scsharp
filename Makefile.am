
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
	@cp AUTHORS $(RELEASEDIR)
	tar -cvjf $(RELEASEDIR).tar.bz2 $(RELEASEDIR)
	rm -rf $(RELEASEDIR)

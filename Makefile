VERSION=0.0000002
RELEASEDIR=scsharp-linux-$(VERSION)

SUBDIRS = Starcraft src

all clean:
	@for dir in $(SUBDIRS); do \
		echo "Making $@ in $$dir"; \
		$(MAKE) -C $$dir $@; \
	done

#dll configs?
release: all
	-mkdir $(RELEASEDIR)
	@cp src/starcraft.exe $(RELEASEDIR)
	@cp src/starcraft.exe.config-example $(RELEASEDIR)/starcraft.exe.config
	@cp Starcraft/*.dll $(RELEASEDIR)
	@cp StormLib/libStorm.so $(RELEASEDIR)
	@cp sdldotnet/bin/examples/SdlDotNet.dll $(RELEASEDIR)
	@cp sdldotnet/bin/examples/Tao.Sdl.dll $(RELEASEDIR)
	@cp sdldotnet/bin/examples/Tao.Sdl.dll.config $(RELEASEDIR)
	@cp RELEASE_README $(RELEASEDIR)/README
	tar -cvzf $(RELEASEDIR).tar.gz $(RELEASEDIR)
	rm -rf $(RELEASEDIR)
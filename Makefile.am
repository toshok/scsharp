
SUBDIRS= SCSharp StormLib src

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

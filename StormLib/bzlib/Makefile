VERSION = 1.0.3
MAJOR=$(shell echo $(VERSION) | sed -e "s/\..*$$//")
MINOR=$(shell echo $(VERSION) | sed -e "s/^[^\.]*\.0*\([0-9]\+\).*$$/\1/")
VERSADD = -Wl,--major-image-version=$(MAJOR) -Wl,--minor-image-version=$(MINOR) 
SHELL=/bin/sh
RESDIR=./res
VPATH=. $(RESDIR)

# To assist in cross-compiling
CC=gcc
AR=ar
RANLIB=ranlib
LDFLAGS= -Wl,-s $(VERSADD)
EXE_EXT=.exe
DLL_EXT=.dll
LN=cp -fp
  
  # Suitably paranoid flags to avoid bugs in gcc-2.7
BIGFILES = -D_LARGEFILE_SOURCE=1 -D_LARGEFILE64_SOURCE=1 -D_FILE_OFFSET_BITS=64
CFLAGS = -D__GW32__ -Wall -Winline -O3 -mms-bitfields  -fno-exceptions \
 -fomit-frame-pointer -march=i386 -mcpu=i686 -fno-strength-reduce $(BIGFILES) \
 -idirafter e:/progra~1/gnuwin32/lib -idirafter h:/glibc/include
LIBS = -Le:/progra~1/gnuwin32/lib -lgw32c -lole32 -luuid -liberty
  
# Where you want it installed when you do 'make install'
PREFIX = c:/progra~1/Bzip2

OBJS= blocksort.o  \
      huffman.o    \
      crctable.o   \
      randtable.o  \
      compress.o   \
      decompress.o \
      bzlib.o

DLLOBJS = $(OBJS) bzip-dllversion.o

.SUFFIXES:
.SUFFIXES: .c .o .exe .rc .dll

# Rule to make compiled resource (Windows)
.rc.o:
	windres --include-dir $(RESDIR) -i $< -o $@

all: libbz2.a bzip2$(DLL_EXT) bzip2 bzip2recover test

bzip2: libbz2.a bzip2.o bzip2-res.o
	$(CC) $(CFLAGS) $(LDFLAGS) -o bzip2 bzip2.o bzip2-res.o \
		./libbz2.a $(LIBS)

bzip2recover: bzip2recover.o bzip2recover-res.o
	$(CC) $(CFLAGS) $(LDFLAGS) -o bzip2recover bzip2recover.o \
		bzip2recover-res.o $(LIBS)

libbz2.a: $(OBJS)
	rm -f libbz2.a
	$(AR) cq libbz2.a $(OBJS)
	@if ( test -f $(RANLIB) -o -f /usr/bin/ranlib -o \
		-f /bin/ranlib -o -f /usr/ccs/bin/ranlib ) ; then \
		echo $(RANLIB) libbz2.a ; \
		$(RANLIB) libbz2.a ; \
	fi

bzip2$(DLL_EXT): $(DLLOBJS)
	o2dll -o $@ -l libbz2 $(DLLOBJS) -version-info 0:0:0 

check: test
test: bzip2
	@cat words1
	./bzip2 -1  < sample1.ref > sample1.rb2
	./bzip2 -2  < sample2.ref > sample2.rb2
	./bzip2 -3  < sample3.ref > sample3.rb2
	./bzip2 -d  < sample1.bz2 > sample1.tst
	./bzip2 -d  < sample2.bz2 > sample2.tst
	./bzip2 -ds < sample3.bz2 > sample3.tst
	cmp sample1.bz2 sample1.rb2 
	cmp sample2.bz2 sample2.rb2
	cmp sample3.bz2 sample3.rb2
	cmp sample1.tst sample1.ref
	cmp sample2.tst sample2.ref
	cmp sample3.tst sample3.ref
	@cat words3

install: bzip2 bzip2recover
	if ( test ! -d $(PREFIX)/bin ) ; then mkdir -p $(PREFIX)/bin ; fi
	if ( test ! -d $(PREFIX)/lib ) ; then mkdir -p $(PREFIX)/lib ; fi
	if ( test ! -d $(PREFIX)/man ) ; then mkdir -p $(PREFIX)/man ; fi
	if ( test ! -d $(PREFIX)/man/man1 ) ; then mkdir -p $(PREFIX)/man/man1 ; fi
	if ( test ! -d $(PREFIX)/include ) ; then mkdir -p $(PREFIX)/include ; fi
	cp -fp bzip2$(EXE_EXT) $(PREFIX)/bin/bzip2$(EXE_EXT)
	cp -fp bzip2$(DLL_EXT) $(PREFIX)/bin/bzip2$(DLL_EXT)
	cp -fp bzip2$(EXE_EXT) $(PREFIX)/bin/bunzip2$(EXE_EXT)
	cp -fp bzip2$(EXE_EXT) $(PREFIX)/bin/bzcat$(EXE_EXT)
	cp -fp bzip2recover$(EXE_EXT) $(PREFIX)/bin/bzip2recover$(EXE_EXT)
	chmod a+x $(PREFIX)/bin/bzip2$(EXE_EXT)
	chmod a+x $(PREFIX)/bin/bzip2$(DLL_EXT)
	chmod a+x $(PREFIX)/bin/bunzip2$(EXE_EXT)
	chmod a+x $(PREFIX)/bin/bzcat$(EXE_EXT)
	chmod a+x $(PREFIX)/bin/bzip2recover$(EXE_EXT)
	cp -fp bzip2.1 $(PREFIX)/man/man1
	chmod a+r $(PREFIX)/man/man1/bzip2.1
	cp -fp bzlib.h $(PREFIX)/include
	chmod a+r $(PREFIX)/include/bzlib.h
	cp -fp libbz2.a $(PREFIX)/lib
	cp -fp libbz2$(DLL_EXT).a $(PREFIX)/lib
	cp -fp bzip2.lib $(PREFIX)/lib
	cp -fp bzip2-bcc.lib $(PREFIX)/lib
	cp -fp bzip2.def $(PREFIX)/lib
	chmod a+r $(PREFIX)/lib/libbz2.a
	chmod a+r $(PREFIX)/lib/libbz2$(DLL_EXT).a
	chmod a+r $(PREFIX)/lib/bzip2.lib
	chmod a+r $(PREFIX)/lib/bzip2-bcc.lib
	chmod a+r $(PREFIX)/lib/bzip2.def
	cp -fp bzgrep $(PREFIX)/bin/bzgrep
	$(LN) $(PREFIX)/bin/bzgrep $(PREFIX)/bin/bzegrep
	$(LN) $(PREFIX)/bin/bzgrep $(PREFIX)/bin/bzfgrep
	chmod a+x $(PREFIX)/bin/bzgrep
	cp -fp bzmore $(PREFIX)/bin/bzmore
	$(LN) $(PREFIX)/bin/bzmore $(PREFIX)/bin/bzless
	chmod a+x $(PREFIX)/bin/bzmore
	cp -fp bzdiff $(PREFIX)/bin/bzdiff
	$(LN) $(PREFIX)/bin/bzdiff $(PREFIX)/bin/bzcmp
	chmod a+x $(PREFIX)/bin/bzdiff
	cp -fp bzgrep.1 bzmore.1 bzdiff.1 $(PREFIX)/man/man1
	chmod a+r $(PREFIX)/man/man1/bzgrep.1
	chmod a+r $(PREFIX)/man/man1/bzmore.1
	chmod a+r $(PREFIX)/man/man1/bzdiff.1
	echo ".so man1/bzgrep.1" > $(PREFIX)/man/man1/bzegrep.1
	echo ".so man1/bzgrep.1" > $(PREFIX)/man/man1/bzfgrep.1
	echo ".so man1/bzmore.1" > $(PREFIX)/man/man1/bzless.1
	echo ".so man1/bzdiff.1" > $(PREFIX)/man/man1/bzcmp.1

clean: 
	rm -f *.o libbz2*.a bzip2$(EXE_EXT) bzip2recover$(EXE_EXT) bzip2$(DLL_EXT) \
	sample1.rb2 sample2.rb2 sample3.rb2 \
	sample1.tst sample2.tst sample3.tst	 \
	libbz2*.lib 

blocksort.o: blocksort.c
	@cat words0
	$(CC) $(CFLAGS) -c blocksort.c
huffman.o: huffman.c
	$(CC) $(CFLAGS) -c huffman.c
crctable.o: crctable.c
	$(CC) $(CFLAGS) -c crctable.c
randtable.o: randtable.c
	$(CC) $(CFLAGS) -c randtable.c
compress.o: compress.c
	$(CC) $(CFLAGS) -c compress.c
decompress.o: decompress.c
	$(CC) $(CFLAGS) -c decompress.c
bzlib.o: bzlib.c
	$(CC) $(CFLAGS) -c bzlib.c
bzip2.o: bzip2.c
	$(CC) $(CFLAGS) -c bzip2.c
bzip2recover.o: bzip2recover.c
	$(CC) $(CFLAGS) -c bzip2recover.c


distclean: clean
	rm -f manual.ps manual.html manual.pdf

DISTNAME=bzip2-1.0.3
dist: check manual
	rm -f $(DISTNAME)
	$(LN) . $(DISTNAME)
	tar cvf $(DISTNAME).tar \
	   $(DISTNAME)/blocksort.c \
	   $(DISTNAME)/huffman.c \
	   $(DISTNAME)/crctable.c \
	   $(DISTNAME)/randtable.c \
	   $(DISTNAME)/compress.c \
	   $(DISTNAME)/decompress.c \
	   $(DISTNAME)/bzlib.c \
	   $(DISTNAME)/bzip2.c \
	   $(DISTNAME)/bzip2recover.c \
	   $(DISTNAME)/bzlib.h \
	   $(DISTNAME)/bzlib_private.h \
	   $(DISTNAME)/Makefile \
	   $(DISTNAME)/LICENSE \
	   $(DISTNAME)/bzip2.1 \
	   $(DISTNAME)/bzip2.1.preformatted \
	   $(DISTNAME)/bzip2.txt \
	   $(DISTNAME)/words0 \
	   $(DISTNAME)/words1 \
	   $(DISTNAME)/words2 \
	   $(DISTNAME)/words3 \
	   $(DISTNAME)/sample1.ref \
	   $(DISTNAME)/sample2.ref \
	   $(DISTNAME)/sample3.ref \
	   $(DISTNAME)/sample1.bz2 \
	   $(DISTNAME)/sample2.bz2 \
	   $(DISTNAME)/sample3.bz2 \
	   $(DISTNAME)/dlltest.c \
	   $(DISTNAME)/manual.html \
	   $(DISTNAME)/manual.pdf \
	   $(DISTNAME)/manual.ps \
	   $(DISTNAME)/README \
	   $(DISTNAME)/README.COMPILATION.PROBLEMS \
	   $(DISTNAME)/README.XML.STUFF \
	   $(DISTNAME)/CHANGES \
	   $(DISTNAME)/bzip2.def \
	   $(DISTNAME)/libbz2.dsp \
	   $(DISTNAME)/dlltest.dsp \
	   $(DISTNAME)/makefile.msc \
	   $(DISTNAME)/Y2K_INFO \
	   $(DISTNAME)/unzcrash.c \
	   $(DISTNAME)/spewG.c \
	   $(DISTNAME)/mk251.c \
	   $(DISTNAME)/bzdiff \
	   $(DISTNAME)/bzdiff.1 \
	   $(DISTNAME)/bzmore \
	   $(DISTNAME)/bzmore.1 \
	   $(DISTNAME)/bzgrep \
	   $(DISTNAME)/bzgrep.1 \
	   $(DISTNAME)/Makefile-libbz2_so \
	   $(DISTNAME)/bz-common.xsl \
	   $(DISTNAME)/bz-fo.xsl \
	   $(DISTNAME)/bz-html.xsl \
	   $(DISTNAME)/bzip.css \
	   $(DISTNAME)/entities.xml \
	   $(DISTNAME)/manual.xml \
	   $(DISTNAME)/format.pl \
	   $(DISTNAME)/xmlproc.sh
	gzip -v $(DISTNAME).tar

# For rebuilding the manual from sources on my SuSE 9.1 box

MANUAL_SRCS= 	bz-common.xsl bz-fo.xsl bz-html.xsl bzip.css \
		entities.xml manual.xml 

manual: manual.html manual.ps manual.pdf

manual.ps: $(MANUAL_SRCS)
	./xmlproc.sh -ps manual.xml

manual.pdf: $(MANUAL_SRCS)
	./xmlproc.sh -pdf manual.xml

manual.html: $(MANUAL_SRCS)
	./xmlproc.sh -html manual.xml

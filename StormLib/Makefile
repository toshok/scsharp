CPP_SOURCES:= \
GfxDecode.cpp \
SCommon.cpp \
SCompression.cpp \
SFileCompactArchive.cpp \
SFileCreateArchiveEx.cpp \
SFileExtractFile.cpp \
SFileFindFile.cpp \
SFileOpenArchive.cpp \
SFileOpenFileEx.cpp \
SFileReadFile.cpp \
SListFile.cpp \
StormPortLinux.cpp \
wave/wave.cpp \
huffman/huffman.cpp

C_SOURCES = \
pklib/implode.c \
pklib/explode.c \
pklib/crc32.c

HEADERS= \
	SCommon.h \
	Storm.h \
	StormLib.h \
	StormPort.h \
	huffman/huff.h \
	huffman/huffman.h \
	pklib/pklib.h \
	wave/wave.h

OBJS:=$(CPP_SOURCES:.cpp=.o) $(C_SOURCES:.c=.o)

include Makefile.config

LIB = libStorm.so

CFLAGS = $(CFLAGS_ADD)
I_FLAGS = $(I_FLAGS_ADD)
LDFLAGS = $(LDFLAGS_ADD) -lz -lbz2

all: $(LIB)

$(LIB): $(OBJS)
	$(CXX) -shared -o $(LIB) $(OBJS) $(LDFLAGS)

clean:
	rm -f $(OBJS)

dist:
	@mkdir $(DISTDIR)
	@mkdir $(DISTDIR)/wave
	@mkdir $(DISTDIR)/huffman
	@mkdir $(DISTDIR)/pklib
	@cp Makefile Makefile.config $(DISTDIR)
	@for i in $(CPP_SOURCES) $(C_SOURCES) $(HEADERS); do cp $$i $(DISTDIR)/$$i; done

#install: $(LIB)
	#install $(I_FLAGS) $(LIB) /usr/local/lib
	#mkdir -p /usr/local/include/StormLib
	#cp Storm.h /usr/local/include/StormLib
	#cp StormPort.h /usr/local/include/StormLib
	#ldconfig

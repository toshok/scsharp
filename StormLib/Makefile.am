
SUBDIRS=bzlib

lib_LTLIBRARIES=libStorm.la

libStorm_la_SOURCES= \
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
	SCommon.h \
	Storm.h \
	StormLib.h \
	StormPort.h \
	wave/wave.cpp \
	wave/wave.h \
	huffman/huffman.cpp \
	huffman/huffman.h \
	huffman/huff.h \
	pklib/implode.c \
	pklib/explode.c \
	pklib/crc32.c \
	pklib/pklib.h

libStorm_la_LIBADD=bzlib/libbz2.la
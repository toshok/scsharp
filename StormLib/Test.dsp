# Microsoft Developer Studio Project File - Name="Test" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Console Application" 0x0103

CFG=Test - Win32 Debug Ansi Static
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "Test.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "Test.mak" CFG="Test - Win32 Debug Ansi Static"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "Test - Win32 Release Ansi Static" (based on "Win32 (x86) Console Application")
!MESSAGE "Test - Win32 Debug Ansi Static" (based on "Win32 (x86) Console Application")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "Test - Win32 Release Ansi Static"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Test___Win32_Release_Ansi_Static"
# PROP BASE Intermediate_Dir "Test___Win32_Release_Ansi_Static"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "ReleaseAS"
# PROP Intermediate_Dir "ReleaseAS"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W4 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_CONSOLE" /D "_MBCS" /YX /FD /c
# ADD CPP /nologo /W4 /GX /O2 /D "NDEBUG" /D "_CONSOLE" /D "WIN32" /D "_MBCS" /YX /FD /c
# ADD BASE RSC /l 0x405 /d "NDEBUG"
# ADD RSC /l 0x405 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /machine:I386

!ELSEIF  "$(CFG)" == "Test - Win32 Debug Ansi Static"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Test___Win32_Debug_Ansi_Static"
# PROP BASE Intermediate_Dir "Test___Win32_Debug_Ansi_Static"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "DebugAS"
# PROP Intermediate_Dir "DebugAS"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W4 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_CONSOLE" /D "_MBCS" /YX /FD /GZ /c
# ADD CPP /nologo /W4 /Gm /GX /ZI /Od /D "_DEBUG" /D "_CONSOLE" /D "WIN32" /D "_MBCS" /FR /YX /FD /GZ /c
# ADD BASE RSC /l 0x405 /d "_DEBUG"
# ADD RSC /l 0x405 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:console /profile /debug /machine:I386

!ENDIF 

# Begin Target

# Name "Test - Win32 Release Ansi Static"
# Name "Test - Win32 Debug Ansi Static"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Group "pklib"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\pklib\crc32.c
# End Source File
# Begin Source File

SOURCE=.\pklib\explode.c
# End Source File
# Begin Source File

SOURCE=.\pklib\implode.c
# End Source File
# Begin Source File

SOURCE=.\pklib\pklib.h
# End Source File
# End Group
# Begin Group "zlib"

# PROP Default_Filter ""
# Begin Group "Zlib Headers"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\zlib\zconf.h
# End Source File
# Begin Source File

SOURCE=.\zlib\zlib.h
# End Source File
# End Group
# Begin Group "Zlib Sources"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\zlib\adler32.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\deflate.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\infblock.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\infcodes.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\inffast.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\inflate.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\inftrees.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\infutil.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\trees.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# Begin Source File

SOURCE=.\zlib\zmemory.c
# ADD BASE CPP /W3
# ADD CPP /W3
# End Source File
# End Group
# End Group
# Begin Group "huffman"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\huffman\huffman.cpp
# End Source File
# Begin Source File

SOURCE=.\huffman\huffman.h
# End Source File
# End Group
# Begin Group "wave"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\wave\wave.cpp
# End Source File
# Begin Source File

SOURCE=.\wave\wave.h
# End Source File
# End Group
# Begin Source File

SOURCE=.\SCommon.cpp
# End Source File
# Begin Source File

SOURCE=.\SCompression.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileCompactArchive.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileCreateArchiveEx.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileExtractFile.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileFindFile.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileOpenArchive.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileOpenFileEx.cpp
# End Source File
# Begin Source File

SOURCE=.\SFileReadFile.cpp
# End Source File
# Begin Source File

SOURCE=.\SListFile.cpp
# End Source File
# Begin Source File

SOURCE=.\Test.cpp
# End Source File
# End Group
# Begin Group "Library Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Storm.lib
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SCommon.h
# End Source File
# Begin Source File

SOURCE=.\StormLib.h
# End Source File
# Begin Source File

SOURCE=.\StormPort.h
# End Source File
# End Group
# End Target
# End Project

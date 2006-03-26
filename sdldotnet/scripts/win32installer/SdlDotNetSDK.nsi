!verbose 3

!define PRODUCT_NAME "SDL.NET SDK"
!define PRODUCT_TYPE "sdk"
!define PRODUCT_VERSION "4.0.3"
!define PRODUCT_BUILD "1"
!define PRODUCT_PUBLISHER "SDL.NET"
!define PRODUCT_PACKAGE "sdldotnet"
!define PRODUCT_WEB_SITE "http://cs-sdl.sourceforge.net"
!define PRODUCT_TUTORIALS_WEB_SITE "http://cs-sdl.sourceforge.net/index.php/Category:Tutorials"
!define PRODUCT_EXAMPLES_WEB_SITE "http://cs-sdl.sourceforge.net/index.php/Category:Examples"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\SdlDotNetSDK"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\SdlDotNetSDK"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_PATH "../../bin/${PRODUCT_PACKAGE}-${PRODUCT_VERSION}-${PRODUCT_BUILD}"

;!define MUI_WELCOMEFINISHPAGE_BITMAP "SdlDotNetLogo.bmp"
;!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH
;!define MUI_UNWELCOMEFINISHPAGE_BITMAP "SdlDotNetLogo.bmp"
;!define MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH

BrandingText "© 2003-2006 David Hudson, http://cs-sdl.sourceforge.net/"
SetCompressor lzma
CRCCheck on

; File Association defines
;!include "fileassoc.nsh"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

;--------------------------------
;Variables

Var STARTMENU_FOLDER
;--------------------------------
;Installer Pages

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "..\..\COPYING"
; Components Page
!insertmacro MUI_PAGE_COMPONENTS
; Directory page
!insertmacro MUI_PAGE_DIRECTORY

;Start Menu Folder Page Configuration
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\SdlDotNet" 
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  
!insertmacro MUI_PAGE_STARTMENU Application $STARTMENU_FOLDER

; Instfiles page
!define MUI_FINISHPAGE_NOAUTOCLOSE
!insertmacro MUI_PAGE_INSTFILES

; Finish page
!insertmacro MUI_PAGE_FINISH

;------------------------------------
; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!define MUI_UNFINISHPAGE_NOAUTOCLOSE 
!insertmacro MUI_UNPAGE_FINISH
;------------------------------------

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "..\..\bin\${PRODUCT_PACKAGE}-${PRODUCT_VERSION}-${PRODUCT_BUILD}-${PRODUCT_TYPE}-setup.exe"
InstallDir "$PROGRAMFILES\SdlDotNet\sdk"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

; .NET Framework check
; http://msdn.microsoft.com/netframework/default.aspx?pull=/library/en-us/dnnetdep/html/redistdeploy1_1.asp
; Section "Detecting that the .NET Framework 1.1 is installed"
Function .onInit
	ReadRegDWORD $R0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" Install
	StrCmp $R0 "" 0 CheckPreviousVersion
	MessageBox MB_OK "Microsoft .NET Framework 1.1 was not found on this system.$\r$\n$\r$\nUnable to continue this installation."
	Abort

  CheckPreviousVersion:
	ReadRegStr $R0 ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName"
	StrCmp $R0 "" CheckOSVersion 0
	MessageBox MB_OK "An old version of SdlDotNet is installed on this computer, please uninstall first.$\r$\n$\r$\nUnable to continue this installation."
	Abort
	
  CheckOSVersion:
        Call IsSupportedWindowsVersion
        Pop $R0
        StrCmp $R0 "False" NoAbort 0
	MessageBox MB_OK "The operating system you are using is not supported by SdlDotNet (95/98/ME/NT3.x/NT4.x)."
        Abort

  NoAbort:
FunctionEnd

Section "Source" SecSrc
  SetOverwrite ifnewer
  
  SetOutPath "$INSTDIR\tools"
  File /r ${PRODUCT_PATH}\tools\*.*

  SetOutPath "$INSTDIR\tests"
  File /r /x obj /x CVS ${PRODUCT_PATH}\tests\*.*

  SetOutPath "$INSTDIR\src"
  File /r /x obj /x bin /x CVS ${PRODUCT_PATH}\src\*.*
  
  SetOutPath "$INSTDIR\extras"
  File /r /x obj /x bin /x CVS ${PRODUCT_PATH}\extras\*.*
  
  SetOutPath "$INSTDIR\examples"
  File /r /x obj /x bin /x CVS ${PRODUCT_PATH}\examples\*.*

  SetOutPath "$INSTDIR\scripts"
  File /r /x CVS ${PRODUCT_PATH}\scripts\*.*

  SetOutPath "$INSTDIR"
  File ${PRODUCT_PATH}\*.*

  ;Store installation folder
  WriteRegStr HKCU "Software\SdlDotNet" "" $INSTDIR
  
SectionEnd

Section "Libraries" SecRuntime
  SetOverwrite ifnewer
  SetOutPath "$INSTDIR\bin"
  File /r /x CVS ${PRODUCT_PATH}\bin\*.*

  SetOutPath "$INSTDIR\lib"
  File /r /x CVS ${PRODUCT_PATH}\lib\*.*
  
  ;Store installation folder
  WriteRegStr HKCU "Software\SdlDotNet" "" $INSTDIR
  
  ;Read a value from an InstallOptions INI file
  SetOutPath "$SYSDIR"
  File /r /x CVS ${PRODUCT_PATH}\lib\win32deps\*.*
  
SectionEnd

Section "Examples" SecExamples
  SetOverwrite ifnewer

  CreateDirectory "$SMPROGRAMS\SdlDotNet"
  CreateDirectory "$SMPROGRAMS\SdlDotNet\Examples"
  SetOutPath "$INSTDIR\bin\examples"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\AudioExample.lnk" "$INSTDIR\bin\examples\AudioExample.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\BombRun.lnk" "$INSTDIR\bin\examples\BombRun.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\BounceSprites.lnk" "$INSTDIR\bin\examples\BounceSprites.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\CDPlayer.lnk" "$INSTDIR\bin\examples\CDPlayer.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\Gears.lnk" "$INSTDIR\bin\examples\Gears.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\MoviePlayer.lnk" "$INSTDIR\bin\examples\MoviePlayer.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\NeHe.lnk" "$INSTDIR\bin\examples\NeHe.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\ParticleExample.lnk" "$INSTDIR\bin\examples\ParticlesExample.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\PhysFsTest.lnk" "$INSTDIR\bin\examples\PhysFsTest.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\Rectangles.lnk" "$INSTDIR\bin\examples\Rectangles.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\RedBook.lnk" "$INSTDIR\bin\examples\RedBook.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\SimpleGame.lnk" "$INSTDIR\bin\examples\SimpleGame.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\SnowDemo.lnk" "$INSTDIR\bin\examples\SnowDemo.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\SpriteGuiDemos.lnk" "$INSTDIR\bin\examples\SpriteGuiDemos.exe"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Examples\Triad.lnk" "$INSTDIR\bin\examples\Triad.exe"
  CreateDirectory "$SMPROGRAMS\SdlDotNet\Documentation"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Documentation\SdlDotNet Help.lnk" "$INSTDIR\doc\chm\SdlDotNet.chm"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Documentation\SdlDotNet.Particles Help.lnk" "$INSTDIR\doc\chm\SdlDotNet.Particles.chm"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Documentation\SdlDotNet HTML Help.lnk" "$INSTDIR\doc\html\SdlDotNet\index.html"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Documentation\SdlDotNet.Particles HTML Help.lnk" "$INSTDIR\doc\html\SdlDotNet.Particles\index.html"
  
  CreateDirectory "$SMPROGRAMS\SdlDotNet\Documentation\Tutorials and Examples"
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}_Tutorials.url" "InternetShortcut" "URL" "${PRODUCT_TUTORIALS_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Documentation\Tutorials and Examples\Tutorials.lnk" "$INSTDIR\${PRODUCT_NAME}_Tutorials.url"
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}_Examples.url" "InternetShortcut" "URL" "${PRODUCT_EXAMPLES_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Documentation\Tutorials and Examples\Examples.lnk" "$INSTDIR\${PRODUCT_NAME}_Examples.url"

  ;Store installation folder
  WriteRegStr HKCU "Software\SdlDotNet" "" $INSTDIR
  
SectionEnd

Section "Documentation" SecDocs
  SetOverwrite ifnewer
  SetOutPath "$INSTDIR\doc"
  File /r ${PRODUCT_PATH}\doc\*.*

  ;Store installation folder
  WriteRegStr HKCU "Software\SdlDotNet" "" $INSTDIR
SectionEnd

;Language strings
LangString TEXT_IO_TITLE ${LANG_ENGLISH} "Installation Options"
LangString TEXT_IO_SUBTITLE ${LANG_ENGLISH} "SdlDotNet Installation Options."
LangString DESC_SecExamples ${LANG_ENGLISH} "Installs examples using various features of SdlDotNet."
LangString DESC_SecSrc ${LANG_ENGLISH} "Installs the source code."
LangString DESC_SecDocs ${LANG_ENGLISH} "Installs documentation"
LangString DESC_SecRuntime ${LANG_ENGLISH} "Installs the libaries into the SdlDotNet directory. It does not install them into the GAC."

;Assign language strings to sections
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${SecExamples} $(DESC_SecExamples)
!insertmacro MUI_DESCRIPTION_TEXT ${SecSrc} $(DESC_SecSrc)
!insertmacro MUI_DESCRIPTION_TEXT ${SecDocs} $(DESC_SecDocs)
!insertmacro MUI_DESCRIPTION_TEXT ${SecRuntime} $(DESC_SecRuntime)
!insertmacro MUI_FUNCTION_DESCRIPTION_END


Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\SdlDotNet\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

Section Uninstall
  Delete "$SMPROGRAMS\SdlDotNet\*.*"

  ; set OutPath to somewhere else because the current working directory cannot be deleted!
  SetOutPath "$DESKTOP"
  
  RMDir /r "$SMPROGRAMS\SdlDotNet"
 
  
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"

  RMDir /r "$INSTDIR"
SectionEnd

; GetWindowsVersion, taken from NSIS help, modified for our purposes
Function IsSupportedWindowsVersion

   Push $R0
   Push $R1

   ReadRegStr $R0 HKLM \
   "SOFTWARE\Microsoft\Windows NT\CurrentVersion" CurrentVersion

   IfErrors 0 lbl_winnt

   ; we are not NT
   ReadRegStr $R0 HKLM \
   "SOFTWARE\Microsoft\Windows\CurrentVersion" VersionNumber

   StrCpy $R1 $R0 1
   StrCmp $R1 '4' 0 lbl_error

   StrCpy $R1 $R0 3

   StrCmp $R1 '4.0' lbl_win32_95
   StrCmp $R1 '4.9' lbl_win32_ME lbl_win32_98

   lbl_win32_95:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_win32_98:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_win32_ME:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_winnt:

   StrCpy $R1 $R0 1

   StrCmp $R1 '3' lbl_winnt_x
   StrCmp $R1 '4' lbl_winnt_x

   StrCpy $R1 $R0 3

   StrCmp $R1 '5.0' lbl_winnt_2000
   StrCmp $R1 '5.1' lbl_winnt_XP
   StrCmp $R1 '5.2' lbl_winnt_2003 lbl_error

   lbl_winnt_x:
     StrCpy $R0 'False'
   Goto lbl_done

   lbl_winnt_2000:
     Strcpy $R0 'True'
   Goto lbl_done

   lbl_winnt_XP:
     Strcpy $R0 'True'
   Goto lbl_done

   lbl_winnt_2003:
     Strcpy $R0 'True'
   Goto lbl_done

   lbl_error:
     Strcpy $R0 'False'
   lbl_done:

   Pop $R1
   Exch $R0

FunctionEnd

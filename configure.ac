AC_INIT(HACKING)

# Gross hack to enable 'make dist' on automake 1.9+tar 1.14.
# The extra brackets are to foil regex-based scans.
m4_ifdef([_A][M_PROG_TAR],[_A][M_SET_OPTION([tar-ustar])])

AM_INIT_AUTOMAKE(scsharp,0.0000007)
AM_MAINTAINER_MODE

AC_PROG_LIBTOOL
AC_PROG_LN_S
AC_PROG_CXX
AC_PROG_CC

MONO_REQUIRED_VERSION=1.1.14

PKG_CHECK_MODULES(mono, mono >= $MONO_REQUIRED_VERSION, ,
                  AC_MSG_ERROR([You must have mono version $MONO_REQUIRED_VERSION or higher to compile and run SCSharp]))

AC_OUTPUT([
Makefile
SCSharp/Makefile
StormLib/Makefile
StormLib/bzlib/Makefile
docs/Makefile
src/Makefile
Tests/Makefile])

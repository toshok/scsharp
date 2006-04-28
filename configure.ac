AC_INIT(HACKING)

# Gross hack to enable 'make dist' on automake 1.9+tar 1.14.
# The extra brackets are to foil regex-based scans.
m4_ifdef([_A][M_PROG_TAR],[_A][M_SET_OPTION([tar-ustar])])

AM_INIT_AUTOMAKE(scsharp,0.0000005)
AM_MAINTAINER_MODE

AC_PROG_LIBTOOL
AC_PROG_LN_S
AC_PROG_CXX
AC_PROG_CC

AC_OUTPUT([
Makefile
SCSharp/Makefile
StormLib/Makefile
StormLib/bzlib/Makefile
src/Makefile])
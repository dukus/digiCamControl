The AVIFile handler included with VirtualDub 1.3b is capable
of running in Proxy mode -- that is, you can install it as the
*default* handler for AVI files, and it will work with signposts
that are renamed to the AVI extension.  This means that
applications which previously failed with the AVIFile handler
because of the .VDR extension can now be used, provided that
the .VDR files are renamed to the .AVI extension.

This support is very experimental, so try at your own risk.

To try the AVIFile proxy support, first use auxsetup.exe to
install AVIFile frameserver support, then use the REGEDIT tool
to install the proxyon.reg file.  Use the proxyoff.reg file
to disable it.

The proxy support will be invisible to most programs, but some
programs do not like the proxy handler and will no longer work
if it is installed; you will have to disable the proxy for
these programs.  These applications were tested:

Ligos LSX-MPEG Encoder 3.0:	works
Panasonic MPEG Encoder 2.30:	works most of the time
XingMPEG Encoder 2.20:		fails (must deinstall proxy)

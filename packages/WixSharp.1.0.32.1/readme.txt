Building MSI:
After building the project the corresponding .msi file can be found in the root project folder.

Tips and Hints:
If you are implementing managed CA you may want to set "Target Framework" to "v3.5" as the lower CLR version will help avoid potential conflicts during the installation (e.g. target system has .NET v3.5 only).

Note: 
WixSharp depend on the WiX toolset. However because of the excessive size of the WiX toolset it is not included in the WixSharp NuGet package. Thus unless you already installed it you will need to download and install it manually either from http://wixtoolset.org/ or from https://wixsharp.codeplex.com/ (as part of the WixSharp suite). 

WixSharp suite contains WIX (v3.9.1208.0+) as well as the set of samples for all major deployment scenarios. 
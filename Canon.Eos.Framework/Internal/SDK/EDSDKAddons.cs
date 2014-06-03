using System;
using System.Runtime.InteropServices;

namespace Canon.Eos.Framework.Internal.SDK
{
    public partial class Edsdk
    {
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetPropertyData( IntPtr inRef, uint inPropertyId,
             int inParam, int inPropertySize, byte[] inPropertyData);

        [DllImport("EDSDK.dll", EntryPoint="EdsCreateEvfImageRef", CallingConvention=CallingConvention.Cdecl)]        
		public extern static uint EdsCreateEvfImageRefCdecl(IntPtr inStreamRef, out IntPtr outEvfImageRef);

        [DllImport("EDSDK.dll", EntryPoint="EdsDownloadEvfImage", CallingConvention=CallingConvention.Cdecl)]
        public extern static uint EdsDownloadEvfImageCdecl(IntPtr inCameraRef, IntPtr outEvfImageRef);   
    }
}

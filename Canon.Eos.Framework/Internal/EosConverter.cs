using System;
using System.Runtime.InteropServices;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Interfaces;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework.Internal
{
    public class EosConverter : IEosAssertable
    {
        public byte[] ConvertImageStreamToBytes(IntPtr imageStream)
        {
            IntPtr imagePtr;
            Util.Assert(Edsdk.EdsGetPointer(imageStream, out imagePtr), "Failed to get image pointer.");
            
            uint imageLen;
            Util.Assert(Edsdk.EdsGetLength(imageStream, out imageLen), "Failed to get image pointer length.");

            var bytes = new byte[imageLen];
            Marshal.Copy(imagePtr, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}

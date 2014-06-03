using System;
using System.IO;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Interfaces;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework.Internal
{
    internal class EosImageTransporter : IEosAssertable
    {
        private static Edsdk.EdsDirectoryItemInfo GetDirectoryItemInfo(IntPtr directoryItem)
        {
            Edsdk.EdsDirectoryItemInfo directoryItemInfo;
            Util.Assert(Edsdk.EdsGetDirectoryItemInfo(directoryItem, out directoryItemInfo), "Failed to get directory item info.");
            return directoryItemInfo;
        }

        private static IntPtr CreateFileStream(string imageFilePath)
        {
            IntPtr stream;
            Util.Assert(Edsdk.EdsCreateFileStream(imageFilePath, Edsdk.EdsFileCreateDisposition.CreateAlways, 
                Edsdk.EdsAccess.ReadWrite, out stream), "Failed to create file stream");
            return stream;    
        }

        private static IntPtr CreateMemoryStream(uint size)
        {
            IntPtr stream;
            Util.Assert(Edsdk.EdsCreateMemoryStream(size, out stream), "Failed to create memory stream");
            return stream;
        }

        private static void DestroyStream(ref IntPtr stream)
        {
            if(stream != IntPtr.Zero)
            {
                Util.Assert(Edsdk.EdsRelease(stream), "Failed to release stream");
                stream = IntPtr.Zero;
            }
        }

        private static void Download(IntPtr directoryItem, uint size, IntPtr stream)
        {
            if (stream == IntPtr.Zero)
                return;
            try
            {
                Util.Assert(Edsdk.EdsDownload(directoryItem, size, stream), "Failed to download to stream");
                Util.Assert(Edsdk.EdsDownloadComplete(directoryItem), "Failed to complete download");
            }
            catch (EosException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EosException(-1, "Unexpected exception while downloading.", ex);
            }            
        }

        private static void Transport(IntPtr directoryItem, uint size, IntPtr stream, bool destroyStream)
        {
            try
            {
                Download(directoryItem, size, stream);
            }
            catch (EosException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EosException(-1, "Unexpected exception while transporting.", ex);
            }
            finally
            {
                if (destroyStream) DestroyStream(ref stream);
            }
        }

        public EosImageEventArgs TransportAsFile(IntPtr directoryItem, string imageBasePath)
        {
            var directoryItemInfo = GetDirectoryItemInfo(directoryItem);
            var imageFilePath = Path.Combine(imageBasePath ?? Environment.CurrentDirectory, directoryItemInfo.szFileName);
            var stream = CreateFileStream(imageFilePath);
            Transport(directoryItem, directoryItemInfo.Size, stream, true);            

            return new EosFileImageEventArgs(imageFilePath);
        }

        public EosImageEventArgs TransportInMemory(IntPtr directoryItem)
        {
            var directoryItemInfo = GetDirectoryItemInfo(directoryItem);
            var stream = CreateMemoryStream(directoryItemInfo.Size);
            try
            {
                Transport(directoryItem, directoryItemInfo.Size, stream, false);           
                var converter = new EosConverter();
                return new EosMemoryImageEventArgs(converter.ConvertImageStreamToBytes(stream)){FileName = directoryItemInfo.szFileName};
            }
            finally
            {
                DestroyStream(ref stream);
            }
        }                
    }
}

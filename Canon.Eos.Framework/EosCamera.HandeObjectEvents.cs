using System;
using System.Threading.Tasks;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        public readonly EosImageTransporter _transporter = new EosImageTransporter();

        private void OnPictureTaken(EosMemoryImageEventArgs eventArgs)
        {
            var directoryItemInfo = EosImageTransporter.GetDirectoryItemInfo(eventArgs.Pointer);
            eventArgs.FileName = directoryItemInfo.szFileName;
            if (this.PictureTaken != null)
                Task.Factory.StartNew(() => this.PictureTaken(this, eventArgs));
        }

        private void OnVolumeInfoChanged(EosVolumeInfoEventArgs eventArgs)
        {
            if (this.VolumeInfoChanged != null)
                this.VolumeInfoChanged(this, eventArgs);
        }

        private void OnObjectEventVolumeInfoChanged(IntPtr sender)
        {
            Edsdk.EdsVolumeInfo volumeInfo;
            Util.Assert(Edsdk.EdsGetVolumeInfo(sender, out volumeInfo), "Failed to get volume info.");

            this.OnVolumeInfoChanged(new EosVolumeInfoEventArgs(new EosVolumeInfo
            {
                Access = volumeInfo.Access,
                FreeSpaceInBytes = volumeInfo.FreeSpaceInBytes,
                MaxCapacityInBytes = volumeInfo.MaxCapacity,
                StorageType = volumeInfo.StorageType,
                VolumeLabel = volumeInfo.szVolumeLabel
            }));
        }
        
        private void OnObjectEventVolumeUpdateItems(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventFolderUpdateItems(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemCreated(IntPtr sender, IntPtr context)
        {
            //LiveViewqueue.Enqueue(() =>
            //{
            //    this.OnPictureTaken(new EosMemoryImageEventArgs(null){Pointer = sender});
            //});
            this.OnPictureTaken(new EosMemoryImageEventArgs(null) { Pointer = sender });
        }
        
        private void OnObjectEventDirItemRemoved(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemInfoChanged(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemContentChanged(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemRequestTransfer(IntPtr sender)
        {
            //LiveViewqueue.Enqueue(() =>
            //{
            //    this.OnPictureTaken(new EosMemoryImageEventArgs(null) { Pointer = sender });
            //    this.OnPictureTaken(_transporter.TransportInMemory(sender));
            //    Edsdk.EdsRelease(sender);
            //});
            this.OnPictureTaken(new EosMemoryImageEventArgs(null) { Pointer = sender });
        }
        
        private void OnObjectEventDirItemRequestTransferDt(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventDirItemCancelTransferDt(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventVolumeAdded(IntPtr sender, IntPtr context)
        {
        }
        
        private void OnObjectEventVolumeRemoved(IntPtr sender, IntPtr context)
        {
        }

        private uint HandleObjectEvent(uint objectEvent, IntPtr sender, IntPtr context)
        {
            try
            {
                EosFramework.LogInstance.Debug("HandleObjectEvent fired: " + objectEvent);
                Console.WriteLine("Canon event {0}", objectEvent);
                switch (objectEvent)
                {
                    case Edsdk.ObjectEvent_VolumeInfoChanged:
                        //this.OnObjectEventVolumeInfoChanged(sender);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_VolumeUpdateItems:
                        this.OnObjectEventVolumeUpdateItems(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_FolderUpdateItems:
                        this.OnObjectEventFolderUpdateItems(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_DirItemCreated:
                        this.OnObjectEventDirItemCreated(sender, context);
                        break;
                    case Edsdk.ObjectEvent_DirItemRemoved:
                        this.OnObjectEventDirItemRemoved(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_DirItemInfoChanged:
                        this.OnObjectEventDirItemInfoChanged(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_DirItemContentChanged:
                        this.OnObjectEventDirItemContentChanged(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_DirItemRequestTransfer:
                        this.OnObjectEventDirItemRequestTransfer(sender);
                        break;
                    case Edsdk.ObjectEvent_DirItemRequestTransferDT:
                        this.OnObjectEventDirItemRequestTransferDt(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_DirItemCancelTransferDT:
                        this.OnObjectEventDirItemCancelTransferDt(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_VolumeAdded:
                        this.OnObjectEventVolumeAdded(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    case Edsdk.ObjectEvent_VolumeRemoved:
                        this.OnObjectEventVolumeRemoved(sender, context);
                        Edsdk.EdsRelease(sender);
                        break;
                    default:
                        Edsdk.EdsRelease(sender);
                        break;
                }
            }
            catch (Exception ex)
            {
                EosFramework.LogInstance.Error("Handing HandleObjectEvent: {0}", ex);
            }
            finally
            {
                
            }

            return Edsdk.EDS_ERR_OK;
        }
    }
}

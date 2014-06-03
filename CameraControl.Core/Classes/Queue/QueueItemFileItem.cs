using System;
using System.IO;
using CameraControl.Core.Interfaces;

namespace CameraControl.Core.Classes.Queue
{
    public class QueueItemFileItem : IQueueItem
    {
        public FileItem FileItem { get; set; }

        #region Implementation of IQueueItem

        public bool Execute(QueueManager manager)
        {
            try
            {
                if (File.Exists(FileItem.SmallThumb))
                {
                    FileItem.Thumbnail = BitmapLoader.Instance.LoadSmallImage(FileItem);
                }
                else
                {
                    if (FileItem.ItemType == FileItemType.File)
                        FileItem.GetExtendedThumb();
                }
            }
            catch (Exception)
            {
                //Log.Error(e);
            }

            return true;
        }

        #endregion
    }
}

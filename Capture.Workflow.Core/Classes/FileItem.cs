using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Capture.Workflow.Core.Classes
{
    public class FileItem:BaseItem
    {
        private BitmapSource _thumb;
        public string ThumbFile { get; set; }
        public string FileName { get; set; }
        public string TempFile { get; set; }

        public BitmapSource Thumb
        {
            get { return _thumb; }
            set
            {
                _thumb = value;
                OnPropertyChanged();
            }
        }

        public VariableCollection Variables { get; set; }

        public FileItem()
        {
            Variables = new VariableCollection();
        }

        /// <summary>
        /// Remove the temporary files and clear all values of the instance 
        /// </summary>
        public void Clear()
        {
            Utils.DeleteFile(TempFile);
        }
    }
}

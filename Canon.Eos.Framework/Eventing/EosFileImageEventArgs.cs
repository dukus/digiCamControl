using System.IO;

namespace Canon.Eos.Framework.Eventing
{
    public class EosFileImageEventArgs : EosImageEventArgs
    {
        internal EosFileImageEventArgs(string imageFilePath)
        {
            this.ImageFilePath = imageFilePath;
        }

        public string ImageFilePath { get; private set; }

        public override Stream GetStream()
        {
            return new FileStream(this.ImageFilePath, FileMode.Open,
                FileAccess.Read, FileShare.Read);
        }        
    }
}

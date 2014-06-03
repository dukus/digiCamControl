using System.IO;

namespace Canon.Eos.Framework.Eventing
{
    public class EosMemoryImageEventArgs : EosImageEventArgs
    {
        internal EosMemoryImageEventArgs(byte[] imageData)
        {
            this.ImageData = imageData;
        }

        public byte[] ImageData { get; private set; }

        public string FileName { get; set; }

        public override Stream GetStream()
        {
            return new MemoryStream(this.ImageData);
        }
    }    
}

using System;
using System.Drawing;
using System.IO;

namespace Canon.Eos.Framework.Eventing
{
    public abstract class EosImageEventArgs : EventArgs
    {
        public virtual Image GetImage()
        {
            using (var stream = this.GetStream())
                return Image.FromStream(stream);
        }

        public virtual Bitmap GetBitmap()
        {
            using (var stream = this.GetStream())
                return new Bitmap(stream);
        }

        public abstract Stream GetStream();
    }
}

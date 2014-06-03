using System.Drawing;

namespace Canon.Eos.Framework.Eventing
{
    public class EosLiveImageEventArgs : EosMemoryImageEventArgs
    {
        internal EosLiveImageEventArgs(byte[] imageData)
            : base(imageData) { }

        public Point ImagePosition { get; internal set; }

        public long[] Histogram { get; internal set; }

        public long Zoom { get; internal set; }

        public Rectangle ZommBounds { get; set; }
        
        public Size ImageSize { get; set; }
    }
}

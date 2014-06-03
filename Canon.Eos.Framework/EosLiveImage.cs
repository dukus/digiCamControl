using System;
using System.Drawing;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public class EosLiveImage : EosObject
    {
        internal static EosLiveImage CreateFromStream(IntPtr stream)
        {
            IntPtr imagePtr;
            Util.Assert(Edsdk.EdsCreateEvfImageRef(stream, out imagePtr), "Failed to create evf image.");
            return new EosLiveImage(imagePtr);    
        }

        internal EosLiveImage(IntPtr imagePtr)
            : base(imagePtr) { }

        [EosProperty(Edsdk.PropID_Evf_ImagePosition)]
        public Point ImagePosition
        {
            get { return this.GetPropertyPointData(Edsdk.PropID_Evf_ImagePosition); }
        }

        [EosProperty(Edsdk.PropID_Evf_Zoom)]
        public long[] Histogram
        {
            get { return this.GetPropertyIntegerArrayData(Edsdk.PropID_Evf_Histogram); }
        }

        [EosProperty(Edsdk.PropID_Evf_Zoom)]
        public long Zoom
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_Zoom); }
        }

        [EosProperty(Edsdk.PropID_Evf_ZoomRect)]
        public Rectangle ZoomBounds
        {
            get { return this.GetPropertyRectangleData(Edsdk.PropID_Evf_ZoomRect); }
        }

        [EosProperty(Edsdk.PropID_Evf_ZoomPosition)]
        public Point ZoomPosition
        {
            get { return this.GetPropertyPointData(Edsdk.PropID_Evf_ZoomPosition); }
        }

        [EosProperty(Edsdk.PropID_Evf_CoordinateSystem)]
        public Size Size
        {
            get { return this.GetPropertySizeData(Edsdk.PropID_Evf_CoordinateSystem); }
        }
    }
}

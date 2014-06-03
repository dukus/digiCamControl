using System;

namespace Canon.Eos.Framework.Eventing
{    
    public class EosVolumeInfoEventArgs : EventArgs
    {
        internal EosVolumeInfoEventArgs(EosVolumeInfo volumeInfo)
        {
            this.VolumeInfo = volumeInfo;
        }

        public EosVolumeInfo VolumeInfo { get; private set; }
    }
}

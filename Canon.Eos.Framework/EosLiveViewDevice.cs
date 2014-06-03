using System;

namespace Canon.Eos.Framework
{
    [Flags]
    public enum EosLiveViewDevice : int
    {
        None = 0,
        Camera = 1,
        Host = 2
    }
}

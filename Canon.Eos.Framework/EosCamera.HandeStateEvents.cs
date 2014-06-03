using System;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {
        private void OnStateEventShutdown(EventArgs eventArgs)
        {
            if (this.Shutdown != null)
                this.Shutdown.BeginInvoke(this, eventArgs, null, null);
        }

        private uint HandleStateEvent(uint stateEvent, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("HandleStateEvent fired: " + stateEvent);
            switch (stateEvent)
            {
                case Edsdk.StateEvent_Shutdown:
                    this.OnStateEventShutdown(EventArgs.Empty);
                    break;
            }
            return Edsdk.EDS_ERR_OK;
        }        
    }
}

using System;
using System.Threading;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal;
using Canon.Eos.Framework.Internal.SDK;
using Canon.Eos.Framework.Threading;

namespace Canon.Eos.Framework
{
    partial class EosCamera
    {        
        private bool _liveMode;
        private bool _cancelLiveViewRequested;
        private bool _pauseLiveViewRequested;
        private bool _liveViewRunning;

        private void OnLiveViewStarted(EventArgs eventArgs)
        {
            if (this.LiveViewStarted != null)
                this.LiveViewStarted(this, eventArgs);
        }

        private void OnLiveViewStopped(EventArgs eventArgs)
        {
            if (this.LiveViewStopped != null)
                this.LiveViewStopped(this, eventArgs);
        }

        private void OnLiveViewPaused(EventArgs eventArgs)
        {
            if (this.LiveViewPaused != null)
                this.LiveViewPaused(this, eventArgs);
        }

        private void OnLiveViewUpdate(EosLiveImageEventArgs eventArgs)
        {
            if (this.LiveViewUpdate != null)
                this.LiveViewUpdate(this, eventArgs);            
        }

        public bool DownloadEvf()
        {
            lock (_locker)
            {

                // Do not download if pauseUpdate requested
                if (_pauseLiveViewRequested)
                    return true;

                if ((this.LiveViewDevice & EosLiveViewDevice.Host) == EosLiveViewDevice.None || _cancelLiveViewRequested)
                    return false;

                //lock (_locker)
                //{
                _liveViewRunning = true;
                DownloadEvfInternal();
                if (LiveViewqueue.Count > 0)
                {
                    try
                    {
                        LiveViewqueue.Dequeue().Invoke();
                    }
                    catch (Exception)
                    {
                    }
                }
                _liveViewRunning = false;
                //}
                return true;
            }
        }

        public bool DownloadEvfInternal()
        {
            var memoryStream = IntPtr.Zero;
            try
            {
                Edsdk.EdsCreateMemoryStream(0, out memoryStream);
                using (var image = EosLiveImage.CreateFromStream(memoryStream))
                {
                    Edsdk.EdsDownloadEvfImage(this.Handle, image.Handle);

                    var converter = new EosConverter();
                    this.OnLiveViewUpdate(
                        new EosLiveImageEventArgs(converter.ConvertImageStreamToBytes(memoryStream))
                        {
                            Zoom = image.Zoom,
                            ZommBounds = image.ZoomBounds,
                            ImagePosition = image.ImagePosition,
                            ImageSize = image.Size,

                            //Histogram = image.Histogram,

                        });

                }
            }
            catch (EosException eosEx)
            {
                //if (eosEx.EosErrorCode != EosErrorCode.DeviceBusy && eosEx.EosErrorCode != EosErrorCode.ObjectNotReady)
                //    throw;
            }
            finally
            {
                if (memoryStream != IntPtr.Zero)
                    Edsdk.EdsRelease(memoryStream);
            }
            return true;
        }


        private void StartDownloadEvfInBackGround()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.Work(() =>
            {

                while (this.DownloadEvf())
                {  
                    // Take Picture requested?
                    if (this._pauseLiveViewRequested)
                    {
                        this.OnLiveViewPaused(EventArgs.Empty);
                        while (this._pauseLiveViewRequested)
                        {
                            //Wait until picture processing is finished
                            Thread.Sleep(EosCamera.WaitTimeoutForNextLiveDownload);
                        }
                    }
                    Thread.Sleep(EosCamera.WaitTimeoutForNextLiveDownload);
                }
                
                this.LiveViewDevice = EosLiveViewDevice.None;
            });
        }

        private void OnPropertyEventPropertyEvfOutputDeviceChanged(uint param, IntPtr context)
        {            
            if (!_liveMode && (this.LiveViewDevice & EosLiveViewDevice.Host) != EosLiveViewDevice.None)
            {
                _liveMode = true;
                this.OnLiveViewStarted(EventArgs.Empty);
                //this.StartDownloadEvfInBackGround();
            }
            else if (_liveMode && (this.LiveViewDevice & EosLiveViewDevice.Host) == EosLiveViewDevice.None)
            {
                _liveMode = false;                
                this.OnLiveViewStopped(EventArgs.Empty);
            }
        }

        private void OnPropertyEventPropertyChanged(uint propertyId, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("OnPropertyEventPropertyChanged: " + propertyId);
            switch (propertyId)
            {
                case Edsdk.PropID_Evf_OutputDevice:
                    this.OnPropertyEventPropertyEvfOutputDeviceChanged(param, context);
                    break;
            }
            if (PropertyChanged != null)
                PropertyChanged(this, new EosPropertyEventArgs() {PropertyId = propertyId});
        }

        private void OnPropertyEventPropertyDescChanged(uint propertyId, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("OnPropertyEventPropertyDescChanged: " + propertyId);            
        }

        private uint HandlePropertyEvent(uint propertyEvent, uint propertyId, uint param, IntPtr context)
        {
            EosFramework.LogInstance.Debug("HandlePropertyEvent fired: " + propertyEvent + ", id: " + propertyId);
            switch (propertyEvent)
            {
                case Edsdk.PropertyEvent_PropertyChanged:
                    this.OnPropertyEventPropertyChanged(propertyId, param, context);
                    break;

                case Edsdk.PropertyEvent_PropertyDescChanged:
                    this.OnPropertyEventPropertyDescChanged(propertyId, param, context);
                    break;
            }
            return Edsdk.EDS_ERR_OK;
        }        
    }
}

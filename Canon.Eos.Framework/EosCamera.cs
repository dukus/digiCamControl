using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Canon.Eos.Framework.Eventing;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Internal.SDK;

namespace Canon.Eos.Framework
{
    public sealed partial class EosCamera : EosObject
    {

        private object _lock = new object();
        public Queue<Action> LiveViewqueue = new Queue<Action>();

        const int WaitTimeoutForNextLiveDownload = 125;
        const int MaximumCopyrightLengthInBytes = 64;
        const int MaximumArtistLengthInBytes = 64;
        const int MaximumOwnerNameLengthInBytes = 32;

        private Edsdk.EdsDeviceInfo _deviceInfo;
        private Edsdk.EdsObjectEventHandler _edsObjectEventHandler;
        private Edsdk.EdsPropertyEventHandler _edsPropertyEventHandler;
        private Edsdk.EdsStateEventHandler _edsStateEventHandler;

        public event EventHandler LiveViewStarted;
        public event EventHandler LiveViewStopped;
        public event EventHandler<EosLiveImageEventArgs> LiveViewUpdate;
        public event EventHandler LiveViewPaused;
        public event EventHandler<EosImageEventArgs> PictureTaken;
        public event EventHandler Shutdown;
        public event EventHandler WillShutdown;
        public event EventHandler<EosVolumeInfoEventArgs> VolumeInfoChanged;
        public event EventHandler<EosPropertyEventArgs> PropertyChanged;
        public event EventHandler<EosExceptionEventArgs> Error;
        

        internal EosCamera(IntPtr camera)
            : base(camera)
        {
            Util.Assert(Edsdk.EdsGetDeviceInfo(this.Handle, out _deviceInfo), 
                "Failed to get device info.");                  
            //this.SetEventHandlers();
            //this.EnsureOpenSession();
        }

        /// <summary>
        /// Gets or sets the artist.
        /// </summary>
        /// <value>
        /// The artist.
        /// </value>
        public new string Artist
        {
            get { return base.Artist; }
            set { this.SetPropertyStringData(Edsdk.PropID_Artist, value, 
                EosCamera.MaximumArtistLengthInBytes); }
        }

        /// <summary>
        /// Gets or sets the battery level.
        /// </summary>
        /// <value>
        /// The battery level.
        /// </value>
        [EosProperty(Edsdk.PropID_BatteryQuality)]
        public long BatteryLevel
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_BatteryLevel); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_BatteryLevel, value); }
        }

        /// <summary>
        /// Gets or sets the battery quality.
        /// </summary>
        /// <value>
        /// The battery quality.
        /// </value>
        public EosBatteryQuality BatteryQuality
        {
            get { return (EosBatteryQuality)this.GetPropertyIntegerData(Edsdk.PropID_BatteryQuality); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_BatteryQuality, (long)value); }
        }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>
        /// The copyright.
        /// </value>
        public new string Copyright
        {
            get { return base.Copyright; }
            set { this.SetPropertyStringData(Edsdk.PropID_Copyright, value, 
                EosCamera.MaximumCopyrightLengthInBytes); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether depth of field preview is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if depth of field preview is enabled; otherwise, <c>false</c>.
        /// </value>
        [EosProperty(Edsdk.PropID_Evf_DepthOfFieldPreview)]        
        public bool DepthOfFieldPreview
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_DepthOfFieldPreview) != 0; }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_DepthOfFieldPreview, value ? 1 : 0); }
        }

        /// <summary>
        /// Gets the device description.
        /// </summary>
        public string DeviceDescription
        {
            get { return _deviceInfo.szDeviceDescription; }
        }

        /// <summary>
        /// Gets or sets the image quality.
        /// </summary>
        /// <value>
        /// The image quality.
        /// </value>
        [EosProperty(Edsdk.PropID_ImageQuality)]        
        public EosImageQuality ImageQuality
        {
            get { return EosImageQuality.Create(this.GetPropertyIntegerData(Edsdk.PropID_ImageQuality)); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_ImageQuality, value.ToBitMask()); }
        }


        public bool IsErrorTolerantMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in live view mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in live view mode; otherwise, <c>false</c>.
        /// </value>
        [EosProperty(Edsdk.PropID_Evf_Mode)]        
        public bool IsInLiveViewMode
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_Mode) != 0; }                    
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_Mode, value ? 1 : 0); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in host live view mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in host live view mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsInHostLiveViewMode
        {
            get { return this.IsInLiveViewMode && this.LiveViewDevice.HasFlag(EosLiveViewDevice.Host); }
        }

        /// <summary>
        /// Gets a value indicating whether session to the camera is open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the session is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsSessionOpen { get; private set; }

        /// <summary>
        /// Gets or sets the live view auto focus.
        /// </summary>
        /// <value>
        /// The live view auto focus.
        /// </value>
        [EosProperty(Edsdk.PropID_Evf_AFMode)]
        public EosLiveViewAutoFocus LiveViewAutoFocus
        {
            get { return (EosLiveViewAutoFocus)this.GetPropertyIntegerData(Edsdk.PropID_Evf_AFMode); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_AFMode, (long)value); }
        }

        /// <summary>
        /// Gets or sets the live view color temperature.
        /// </summary>
        /// <value>
        /// The live view color temperature.
        /// </value>
        [EosProperty(Edsdk.PropID_Evf_ColorTemperature)]
        public long LiveViewColorTemperature
        {
            get { return this.GetPropertyIntegerData(Edsdk.PropID_Evf_ColorTemperature); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_ColorTemperature, value); }
        }

        /// <summary>
        /// Gets or sets the live view device.
        /// </summary>
        /// <value>
        /// The live view device.
        /// </value>
        [EosProperty(Edsdk.PropID_Evf_OutputDevice)]
        public EosLiveViewDevice LiveViewDevice
        {
            get { return (EosLiveViewDevice)this.GetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_OutputDevice, (long)value); }
        }

        /// <summary>
        /// Gets or sets the live view white balance.
        /// </summary>
        /// <value>
        /// The live view white balance.
        /// </value>
        [EosProperty(Edsdk.PropID_Evf_WhiteBalance)]
        public EosWhiteBalance LiveViewWhiteBalance
        {
            get { return (EosWhiteBalance)this.GetPropertyIntegerData(Edsdk.PropID_Evf_WhiteBalance); }
            set { this.SetPropertyIntegerData(Edsdk.PropID_Evf_WhiteBalance, (long)value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is legacy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is legacy; otherwise, <c>false</c>.
        /// </value>
        public bool IsLegacy
        {
            get { return _deviceInfo.DeviceSubType == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked { get; private set; }

        /// <summary>
        /// Gets or sets the name of the owner.
        /// </summary>
        /// <value>
        /// The name of the owner.
        /// </value>
        public new string OwnerName
        {
            get { return base.OwnerName; }
            set { this.SetPropertyStringData(Edsdk.PropID_OwnerName, value, 
                EosCamera.MaximumOwnerNameLengthInBytes); }
        }

        /// <summary>
        /// Gets the name of the port.
        /// </summary>
        /// <value>
        /// The name of the port.
        /// </value>
        public string PortName
        {
            get { return _deviceInfo.szPortName; }
        }

        [Flags]
        private enum SaveLocation { Camera = 1, Host = 2 };
 
        private void ChangePicturesSaveLocation(SaveLocation saveLocation)
        {
            this.CheckDisposed();

            this.EnsureOpenSession();

            Util.Assert(Edsdk.EdsSetPropertyData(this.Handle, Edsdk.PropID_SaveTo, 0, Marshal.SizeOf(typeof(int)), 
                (int)saveLocation), "Failed to set SaveTo location.");
            
            if(!this.IsLegacy)
            {
                this.LockAndExceute(() =>
                {
                    var capacity = new Edsdk.EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
                    Util.Assert(Edsdk.EdsSetCapacity(this.Handle, capacity), "Failed to set capacity.");
                });                
            }            
        }
                
        protected internal override void DisposeUnmanaged()
        {            
            if (this.IsSessionOpen)
                Edsdk.EdsCloseSession(this.Handle);
            base.DisposeUnmanaged();
        }

        public void EnsureOpenSession()
        {
            this.CheckDisposed();
            if (!this.IsSessionOpen)
            {
                Util.Assert(Edsdk.EdsOpenSession(this.Handle), "Failed to open session.");
                this.IsSessionOpen = true;
            }
        }

        protected override TResult ExecuteGetter<TResult>(Func<TResult> function)
        {
            try
            {
                if (this.IsLegacy && !this.IsLocked)
                    return this.LockAndExceute(function);
                return base.ExecuteGetter(function);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
                return default(TResult);
            }
        }

        protected override void ExecuteSetter(Action action)
        {
            lock (_locker)
            {

                try
                {
                    if (this.IsLegacy && !this.IsLocked)
                    {
                        this.LockAndExceute(action);
                        return;
                    }

                    base.ExecuteSetter(action);
                }
                catch (Exception ex)
                {
                    this.OnError(ex);
                }
            }
        }

        public void Lock()
        {
            lock (_locker)
            {
                this.CheckDisposed();

                if (!this.IsLocked)
                {
                    Util.Assert(Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UILock,0),
                                "Failed to lock camera.");
                    this.IsLocked = true;
                }
            }
        }

        public void DeleteItem(IntPtr itemInfo)
        {
            LockAndExceute(() => { Edsdk.EdsDeleteDirectoryItem(itemInfo); });
        }

        private void LockAndExceute(Action action)
        {
            this.Lock();
            try { action(); }
            finally { this.Unlock(); }
        }

        private TResult LockAndExceute<TResult>(Func<TResult> function)
        {
            this.Lock();
            try { return function(); }
            finally { this.Unlock(); }
        }

        private void OnError(Exception exception)
        {
            if (!this.IsErrorTolerantMode)
                throw exception;
            this.OnError(new EosExceptionEventArgs { Exception = exception });
        }

        private void OnError(EosExceptionEventArgs args)
        {
            if (this.Error != null)
                this.Error(this, args);
        }

        public void SetProperty(uint propertyId, long val)
        {
            lock (_locker)
            {
                SendCommand(Edsdk.CameraCommand_DoEvfAf, 0);
                bool retry = false;
                int retrynum = 0;
                //DeviceReady();
                do
                {
                    if (retrynum > 10)
                    {
                        return;
                    }
                    try
                    {
                        this.SetPropertyIntegerData(propertyId, val);
                    }
                    catch (EosPropertyException)
                    {
                        Thread.Sleep(50);
                        retry = true;
                        retrynum++;
                    }
                } while (retry);
            }
        }

        public long GetProperty(uint propertyId)
        {
            lock (_locker)
            {
                return (long) this.GetPropertyIntegerData(propertyId);
            }
        }

        public DateTime GetDate()
        {
            lock (_locker)
            {
                //return (long)this.GetPropertyIntegerData(propertyId);
                var v = this.GetPropertyStruct<Edsdk.EdsTime>(Edsdk.PropID_DateTime, Edsdk.EdsDataType.Time);
                return new DateTime(v.Year, v.Month, v.Day, v.Hour, v.Minute, v.Second);
            }
        }

        public void SetDate(DateTime time)
        {

            lock (_locker)
            {
                //return (long)this.GetPropertyIntegerData(propertyId);
//                var v = this.GetPropertyStruct<Edsdk.EdsTime>(Edsdk.PropID_DateTime, Edsdk.EdsDataType.Time);
                Edsdk.EdsTime edsTime = new Edsdk.EdsTime()
                {
                    Day = time.Day, Hour = time.Hour, Year = time.Year, Minute = time.Minute, Month = time.Month,
                    Second = time.Second
                };
                this.SetPropertyStruct<Edsdk.EdsTime>(Edsdk.PropID_DateTime, edsTime);
            }
        }



        /// <summary>
        /// Saves the pictures to camera.
        /// </summary>
        public void SavePicturesToCamera()
        {
            this.CheckDisposed();
            this.ChangePicturesSaveLocation(SaveLocation.Camera);
        }

        /// <summary>
        /// Saves the pictures to host.
        /// </summary>
        /// <param name="pathFolder">The path folder.</param>
        public void SavePicturesToHost(string pathFolder)
        {
            this.SavePicturesToHost(pathFolder, SaveLocation.Host);
        }

        /// <summary>
        /// Saves the pictures to host and camera.
        /// </summary>
        /// <param name="pathFolder">The path folder.</param>
        public void SavePicturesToHostAndCamera(string pathFolder)
        {
            this.SavePicturesToHost(pathFolder, SaveLocation.Camera);
        }

        private void SavePicturesToHost(string pathFolder, SaveLocation saveLocation)
        {
            if (string.IsNullOrWhiteSpace(pathFolder))
                throw new ArgumentException("Cannot be null or white space.", "pathFolder");

            this.CheckDisposed();

            this.ChangePicturesSaveLocation(saveLocation | SaveLocation.Host);
        }

        public uint SendCommand(uint command, int parameter = 0)
        {
            lock (_locker)
            {
                this.EnsureOpenSession();
                return Edsdk.EdsSendCommand(this.Handle, command, parameter);
            }
        }

        public void SetEventHandlers()
        {   
            _edsStateEventHandler = this.HandleStateEvent;
            Util.Assert(Edsdk.EdsSetCameraStateEventHandler(this.Handle, Edsdk.StateEvent_All, 
                _edsStateEventHandler, IntPtr.Zero), "Failed to set state handler.");                     

            _edsObjectEventHandler = this.HandleObjectEvent;            
            Util.Assert(Edsdk.EdsSetObjectEventHandler(this.Handle, Edsdk.ObjectEvent_All, 
                _edsObjectEventHandler, IntPtr.Zero), "Failed to set object handler.");

            _edsPropertyEventHandler = this.HandlePropertyEvent;
            Util.Assert(Edsdk.EdsSetPropertyEventHandler(this.Handle, Edsdk.PropertyEvent_All, 
                _edsPropertyEventHandler, IntPtr.Zero), "Failed to set object handler.");            
        }

        /// <summary>
        /// Starts the live view.
        /// </summary>
        /// <returns></returns>
        public EosLiveViewAutoFocus StartLiveView()
        {
            if (!this.IsInLiveViewMode)
                this.IsInLiveViewMode = true;
            this._cancelLiveViewRequested = false;
            this._pauseLiveViewRequested = false;
            var device = this.LiveViewDevice;
            device = device | EosLiveViewDevice.Host;
            this.LiveViewDevice = device;
            return this.LiveViewAutoFocus;
        }

        public EosLiveViewAutoFocus StartLiveViewCamera()
        {
            if (!this.IsInLiveViewMode)
                this.IsInLiveViewMode = true;
            this._cancelLiveViewRequested = false;
            this._pauseLiveViewRequested = false;
            var device = this.LiveViewDevice;
            device = device| EosLiveViewDevice.Camera | EosLiveViewDevice.Host;
            this.LiveViewDevice = device;
            this.LiveViewAutoFocus = EosLiveViewAutoFocus.LiveMode;
            return this.LiveViewAutoFocus;
        }

        /// <summary>
        /// Starts the live view with special auto focus.
        /// </summary>
        /// <param name="autoFocus">The auto focus.</param>
        /// <returns></returns>
        public EosLiveViewAutoFocus StartLiveView(EosLiveViewAutoFocus autoFocus)
        {
            this.StartLiveView();
            this.LiveViewAutoFocus = autoFocus;
            return this.LiveViewAutoFocus;
        }

        /// <summary>
        /// Stops the live view.
        /// </summary>
        public void StopLiveView()
        {
            this.LiveViewDevice = EosLiveViewDevice.None;
        }

        public void StartRecord()
        {
            LiveViewqueue.Enqueue(() =>
            {
                StopLiveView();
                SavePicturesToCamera();
                this.SendCommand(Edsdk.CameraCommand_MovieSelectSwON);
                StartLiveView();
                this.SendCommand(Edsdk.CameraCommand_DoEvfAf, 0);
                SetPropertyIntegerData(Edsdk.PropID_Record, (long)4);
            });
        }

        public void StopRecord()
        {
            //LiveViewqueue.Enqueue(() =>
            //{
                this.SendCommand(Edsdk.CameraCommand_DoEvfAf, 0);
                SetPropertyIntegerData(Edsdk.PropID_Record, (long)0);
                this.SendCommand(Edsdk.CameraCommand_MovieSelectSwOFF);
            //});
        }

        public bool IsOldCanon()
        {
            if (string.IsNullOrEmpty(ProductName))
                return false;
            string[] models =
            {
                " 1000D", " 40D", " 450D", " 50D", " 400D", " 500D", "Rebel XSi", "Rebel XTi",
                "Rebel XS", "Rebel T1i", "1Ds Mark III"
            };
            return models.Any(model => DeviceDescription.ToLower().Contains(model.ToLower()));
        }

        /// <summary>
        /// Takes the picture.
        /// </summary>

        public void TakePicture()
        {
            lock (_locker)
            {
                if (IsOldCanon())
                {
                    Util.Assert(this.SendCommand(Edsdk.CameraCommand_TakePicture), "Failed to capture picture with CameraCommand_TakePicture.");
                    return;
                }

                if (this.IsLegacy && !this.IsLocked)
                {
                    this.LockAndExceute(this.TakePicture);
                    return;
                }
                Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Completely),
                    "Failed to press fully.");
                Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF),
                    "Failed to release.");
            }
        }

        public void TakePictureNoAf()
        {
            //_photoCounter = ImageQuality.SecondaryCompressLevel != EosCompressLevel.Unknown ? 0 : 1;
            if (IsOldCanon())
            {
                Util.Assert(this.SendCommand(Edsdk.CameraCommand_TakePicture), "Failed to capture picture with CameraCommand_TakePicture.");
                return;
            }

            if (this.IsLegacy && !this.IsLocked)
            {
                this.LockAndExceute(this.TakePictureNoAf);
                return;
            }

            Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int) Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Completely_NonAF),
                "Failed to take picture no AF.");
            Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF),
                "Failed to release.");
        }

        public void ResetShutterButton()
        {
            if (this.IsLegacy && !this.IsLocked)
            {
                this.LockAndExceute(this.ResetShutterButton);
                return;
            }

            Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, (int)Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF),
                "Failed to take picture no AF.");
        }

        public void AutoFocus()
        {
            lock (_locker)
            {
                if (this.IsLegacy && !this.IsLocked)
                {
                    this.LockAndExceute(this.AutoFocus);
                    return;
                }

                Util.Assert(
                    this.SendCommand(Edsdk.CameraCommand_PressShutterButton,
                                     (int) Edsdk.EdsShutterButton.CameraCommand_ShutterButton_OFF),
                    "Failed to take picture no AF.");
                Util.Assert(
                    this.SendCommand(Edsdk.CameraCommand_PressShutterButton,
                                     (int) Edsdk.EdsShutterButton.CameraCommand_ShutterButton_Halfway),
                    "Failed to take picture no AF.");
            }
        }


        public void BulbStart()
        {
            //this.Lock();
            //Thread.Sleep(120);
            Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, 65539),
                "Failed to start bulb mode");
        }

        public void BulbEnd()
        {
            try
            {
                Util.Assert(this.SendCommand(Edsdk.CameraCommand_PressShutterButton, 0),
                    "Failed to start bulb mode");
            }
            finally
            {
                if (this.IsLocked)
                    this.Unlock();
            }
        }

        public void FocusInLiveView(uint focus)
        {
            if (this.IsLegacy && !this.IsLocked)
            {
                this.LockAndExceute(() => this.FocusInLiveView(focus));
                return;
            }

            Util.Assert(this.SendCommand(Edsdk.CameraCommand_DriveLensEvf, (int) focus),
                "Failed to focus");
        }

        public void FocusModeLiveView(uint focus)
        {
            if (this.IsLegacy && !this.IsLocked)
            {
                this.LockAndExceute(() => this.FocusModeLiveView(focus));
                return;
            }

            Util.Assert(this.SendCommand(Edsdk.CameraCommand_DoEvfAf, (int)focus),
                "Failed to focus");
        }

        public void TakePictureInLiveview()
        {
            //_photoCounter = ImageQuality.SecondaryCompressLevel != EosCompressLevel.Unknown ? 0 : 1;
            this._pauseLiveViewRequested = true;
        }

        public void ResumeLiveview()
        {
            //has to be called if Taking photo fails
            this._pauseLiveViewRequested = false;

        }

        public void PauseLiveview()
        {
            //has to be called if Taking photo fails
            this._pauseLiveViewRequested = true;
            Thread.Sleep(150);
            //while (_liveViewRunning)
            //{

            //}
        }

        public override string ToString()
        {
            return this.DeviceDescription ?? string.Empty;
        }
        
        public void Unlock()
        {
            lock (_locker)
            {
                if (this.IsLocked)
                {
                    Edsdk.EdsSendStatusCommand(this.Handle, Edsdk.CameraState_UIUnLock,0);
                    this.IsLocked = false;
                }
            }
        }                        
    }
}

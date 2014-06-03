using System;
using Canon.Eos.Framework.Helper;
using Canon.Eos.Framework.Interfaces;
using Canon.Eos.Framework.Internal.SDK;


namespace Canon.Eos.Framework
{
    public sealed class EosFramework : EosDisposable
    {        
        private static readonly object __referenceLock = new object();
        private static readonly object __eventLock = new object();
        private static int __referenceCount;
        private static Edsdk.EdsCameraAddedHandler __edsCameraAddedHandler;
        private static event EventHandler GlobalCameraAdded;

        static EosFramework()
        {
            EosFramework.LogInstance = new ConsoleLog();
        }

        /// <summary>
        /// Gets or sets the log instance.
        /// </summary>
        /// <value>
        /// The log instance.
        /// </value>
        public static IEosLog LogInstance { get; set; }

        /// <summary>
        /// Occurs when a camera is added to the host,
        /// </summary>
        public event EventHandler CameraAdded
        {
            add 
            {
                lock (__eventLock)
                {
                    EosFramework.GlobalCameraAdded += value;
                }
            }
            remove
            {
                lock (__eventLock)
                {
                    EosFramework.GlobalCameraAdded -= value;
                }
            }
        }

        public EosFramework()
        {
            lock (__referenceLock)
            {
                if (__referenceCount == 0)
                {
                    try
                    {
                        Util.Assert(Edsdk.EdsInitializeSDK(), "Failed to initialize the SDK.");
                        __edsCameraAddedHandler = EosFramework.HandleCameraAddedEvent;
                        Edsdk.EdsSetCameraAddedHandler(__edsCameraAddedHandler, IntPtr.Zero);
                    }
                    catch (EosException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Util.Assert(0xFFFFFFFF, "Failed to initialize the SDK.", ex);
                    }
                }
                ++__referenceCount;
            }
        }

        private static uint HandleCameraAddedEvent(IntPtr context)
        {
            lock (__eventLock)
            {
                if (EosFramework.GlobalCameraAdded != null)
                {
                    // TODO: find something better than null to pass as sender!
                    EosFramework.GlobalCameraAdded(null, EventArgs.Empty);
                }
            }
            return Edsdk.EDS_ERR_OK;
        }

        /// <summary>
        /// Gets a collection of cameras currenly connected to the host.
        /// </summary>
        /// <returns></returns>
        public EosCameraCollection GetCameraCollection()
        {
            this.CheckDisposed();
            return new EosCameraCollection();
        }

        /// <summary>
        /// Gets the first camera available.
        /// </summary>
        /// <returns></returns>
        public EosCamera GetCamera()
        {
            using (var cameras = this.GetCameraCollection())
                return cameras.Count > 0 ? cameras[0] : null;
        }

        protected internal override void DisposeUnmanaged()
        {
            lock (__referenceLock)
            {
                if (__referenceCount > 0)
                {
                    if(--__referenceCount == 0)
                        Edsdk.EdsTerminateSDK();
                }
            }
            base.DisposeUnmanaged();
        }
    }
}

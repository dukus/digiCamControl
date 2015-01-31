// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BroadcasterPlugin.cs" company="Starboard">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <author> William Eddins </author>
// <summary>
//   Encapsulates logic behind sending plugin updates to the XSplit rendering system. Instances should be
//   created through the CreateInstance static method, which ensures XSplit is installed when attempting
//   to create the COM object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using VHMediaCOMLib;

namespace CameraControl.XSplit
{
    /// <summary>
    /// Encapsulates logic behind sending plugin updates to the XSplit rendering system. Instances should be
    ///   created through the CreateInstance static method, which ensures XSplit is installed when attempting
    ///   to create the COM object.
    /// </summary>
    public class BroadcasterPlugin
    {
        #region Constants and Fields

        /// <summary>
        ///   Instance of the XSplit COM object.
        /// </summary>
        private readonly VHCOMRenderEngineExtSrc2 xsplit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcasterPlugin"/> class. 
        ///   Prevents the class from being manually created.
        /// </summary>
        /// <param name="xsplit">
        /// The xsplit instance to attach.
        /// </param>
        protected BroadcasterPlugin(VHCOMRenderEngineExtSrc2 xsplit)
        {
            this.xsplit = xsplit;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets a value indicating whether the XSplit connection is ready to receive images.
        /// </summary>
        public bool ConnectionIsReady
        {
            get
            {
                return (this.xsplit.ConnectionStatus & 3) == 3;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to create an instance of the BroadcasterPlugin class. If XSplit is not installed, null is returned.
        /// </summary>
        /// <param name="connectionUID">
        /// Unique ID to apply to this application, should match the accompanying .xbs file.
        /// </param>
        /// <returns>
        /// Returns an instance of BroadcasterPlugin if XSplit is installed on the system, else null is returned.
        /// </returns>
        public static BroadcasterPlugin CreateInstance(string connectionUID)
        {
            BroadcasterPlugin plugin = null;

            try
            {
                var extsrc = new VHCOMRenderEngineExtSrc2 { ConnectionUID = connectionUID };
                plugin = new BroadcasterPlugin(extsrc);
            }
            catch (COMException)
            {
                // Do nothing, the plugin failed to load so null will be returned.
            }

            return plugin;
        }

        /// <summary>
        /// Renders an object and sends the image to the XSplit plugin.
        ///   This method must be called on the same thread as the owner of the Visual, in most cases the UI (main) thread.
        /// </summary>
        /// <param name="obj">
        /// Visual object to render and send to XSplit.
        /// </param>
        /// <param name="width">
        /// Desired output width, in pixels.
        /// </param>
        /// <param name="height">
        /// Desired output height, in pixels.
        /// </param>
        /// <returns>
        /// Returns whether the call was successful and the object was rendered. If obj is null, false will always be returned.
        /// </returns>
        public bool RenderVisual(Visual obj, int width, int height)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.ConnectionIsReady)
            {
                var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);

                var elementBrush = new VisualBrush(obj);
                var visual = new DrawingVisual();
                var dc = visual.RenderOpen();

                dc.DrawRectangle(elementBrush, null, new Rect(0, 0, width, height));
                dc.Close();

                bmp.Render(visual);

                // The remaining work (format conversion, sending to xsplit) can be done on a seperate thread)
                Task.Factory.StartNew(
                    () =>
                        {
                            var encoder = new BmpBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bmp));

                            using (var stream = new MemoryStream())
                            {
                                encoder.Save(stream);

                                stream.Position = 0;

                                byte[] bytes = stream.ToArray();

                                // Length of output data we're going to send.
                                int length = width * height * 4;

                                // Allocate memory for bitmap transfer to COM
                                IntPtr dataptr = Marshal.AllocCoTaskMem(length);
                                Marshal.Copy(bytes, bytes.Length - length, dataptr, length);
                                this.xsplit.SendFrame((uint)width, (uint)height, dataptr.ToInt32());

                                // Send to broadcaster
                                Marshal.FreeCoTaskMem(dataptr);
                            }
                        });

                return true;
            }

            return false;
        }

        public bool DownloadVisual(string url, int width, int height)
        {

            if (this.ConnectionIsReady)
            {

                // The remaining work (format conversion, sending to xsplit) can be done on a seperate thread)
                Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {

                            if (! ServiceProvider.DeviceManager.LiveViewImage.ContainsKey(
                                ServiceProvider.DeviceManager.SelectedCameraDevice))
                                return;
                            if (
                                ServiceProvider.DeviceManager.LiveViewImage[
                                    ServiceProvider.DeviceManager.SelectedCameraDevice] == null)
                                return;

                            var ms = new MemoryStream(ServiceProvider.DeviceManager.LiveViewImage[ServiceProvider.DeviceManager.SelectedCameraDevice]);

                            BitmapImage src = new BitmapImage();

                            src.BeginInit();
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.StreamSource = ms;
                            src.EndInit();
                            WriteableBitmap wb = new WriteableBitmap(new FormatConvertedBitmap(src, PixelFormats.Bgra32, null, 0));
                            width = wb.PixelWidth;
                            height = wb.PixelHeight;
                            var encoder = new BmpBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(wb));

                            using (var stream = new MemoryStream())
                            {
                                encoder.Save(stream);

                                stream.Position = 0;

                                byte[] bytes = stream.ToArray();

                                // Length of output data we're going to send.
                                int length = width * height * 4;

                                // Allocate memory for bitmap transfer to COM
                                IntPtr dataptr = Marshal.AllocCoTaskMem(length);
                                Marshal.Copy(bytes, bytes.Length - length, dataptr, length);
                                this.xsplit.SendFrame((uint)width, (uint)height, dataptr.ToInt32());

                                // Send to broadcaster
                                Marshal.FreeCoTaskMem(dataptr);
                            }
                        }
                        catch (Exception)
                        {


                        }
                    });

                return true;
            }

            return false;
        }
        #endregion
    }
}
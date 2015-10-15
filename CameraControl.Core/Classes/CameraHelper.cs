#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    public static class CameraHelper
    {
        /// <summary>
        /// Captures the specified camera device.
        /// </summary>
        /// <param name="o">ICameraDevice</param>
        public static void Capture(object o)
        {
            if (o != null)
            {
                var camera = o as ICameraDevice;
                if (camera != null)
                {
                    ServiceProvider.DeviceManager.LastCapturedImage[camera] = "";
                    CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(camera);
                    for (int i = 0; i < property.Delay; i++)
                    {
                        StaticHelper.Instance.SystemMessage = "Countig down " + (property.Delay - i);
                        Thread.Sleep(1000);
                    }
                    if (property.UseExternalShutter && property.SelectedConfig != null)
                    {
                        ServiceProvider.ExternalDeviceManager.AssertFocus(property.SelectedConfig);
                        Thread.Sleep(1000);
                        ServiceProvider.ExternalDeviceManager.OpenShutter(property.SelectedConfig);
                        Thread.Sleep(1000);
                        ServiceProvider.ExternalDeviceManager.CloseShutter(property.SelectedConfig);
                        return;
                    }
                    camera.CapturePhoto();
                    ServiceProvider.Analytics.CameraCapture(camera);
                }
            }
        }

        public static void Capture()
        {
            try
            {
                CaptureWithError();
            }
            catch (Exception e)
            {
                Log.Debug("Error capture", e);
                StaticHelper.Instance.SystemMessage = e.Message;
            }
        }

        public static void CaptureWithError(ICameraDevice device = null)
        {
            Capture(device ?? ServiceProvider.DeviceManager.SelectedCameraDevice);
        }

        /// <summary>
        /// Captures with all connected cameras.
        /// </summary>
        /// <param name="delay">The delay between camera captures in milli sec</param>
        public static void CaptureAll(int delay)
        {
            foreach (
                ICameraDevice connectedDevice in
                    ServiceProvider.DeviceManager.ConnectedDevices.Where(
                        connectedDevice => connectedDevice.IsConnected && connectedDevice.IsChecked))
            {
                Thread.Sleep(delay);
                ICameraDevice device = connectedDevice;
                Thread threadcamera = new Thread(new ThreadStart(delegate
                                                                     {
                                                                         try
                                                                         {
                                                                             Capture(device);
                                                                         }
                                                                         catch (Exception exception)
                                                                         {
                                                                             Log.Error(exception);
                                                                             StaticHelper.Instance.
                                                                                 SystemMessage =
                                                                                 exception.Message;
                                                                         }
                                                                     }));
                threadcamera.Start();
            }
        }

        public static void CaptureNoAf()
        {
            try
            {
                CaptureNoAf(ServiceProvider.DeviceManager.SelectedCameraDevice);
            }
            catch (Exception e)
            {
                Log.Debug("Error capture", e);
                StaticHelper.Instance.SystemMessage = e.Message;
            }
        }

        public static void CaptureNoAf(object o)
        {
            if (o != null)
            {
                var camera = o as ICameraDevice;
                if (camera != null)
                {
                    CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(camera);
                    if (property.UseExternalShutter && property.SelectedConfig != null)
                    {
                        ServiceProvider.ExternalDeviceManager.OpenShutter(property.SelectedConfig);
                        Thread.Sleep(200);
                        ServiceProvider.ExternalDeviceManager.CloseShutter(property.SelectedConfig);
                        return;
                    }
                    camera.CapturePhotoNoAf();
                }
            }
        }

        /// <summary>
        /// Waits for a camera to be ready.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        public static void WaitForCamera(this ICameraDevice device, int timeout)
        {
            DateTime startTime = DateTime.Now;
            while (device.IsBusy)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > timeout)
                    break;
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Loads the atached properties to a camera.
        /// </summary>
        /// <param name="cameraDevice">The camera device.</param>
        /// <returns>The atached CameraProperty</returns>
        public static CameraProperty LoadProperties(this ICameraDevice cameraDevice)
        {
            CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(cameraDevice);
            cameraDevice.DisplayName = property.DeviceName;
            cameraDevice.AttachedPhotoSession = ServiceProvider.Settings.GetSession(property.PhotoSessionName);
            //if (cameraDevice!=null && cameraDevice.GetCapability(CapabilityEnum.CaptureInRam))
            //    cameraDevice.CaptureInSdRam = property.CaptureInSdRam;
            return property;
        }
    }
}
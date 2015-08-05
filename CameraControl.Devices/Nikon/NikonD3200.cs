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

using System.Runtime.InteropServices;
using System.Threading;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD3200 : NikonBase
    {
        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            bool res = base.Init(deviceDescriptor);
            Capabilities.Clear();
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.RecordMovie);
            Capabilities.Add(CapabilityEnum.CaptureInRam);
            CaptureInSdRam = false;
            //PropertyChanged -= NikonBase_PropertyChanged;
            return res;
        }


        public override void CapturePhoto()
        {
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                DeviceReady();
                ErrorCodes.GetException(CaptureInSdRam
                                            ? ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram, 0xFFFFFFFF)
                                            : ExecuteWithNoData(CONST_CMD_InitiateCapture));
            }
            catch (COMException comException)
            {
                IsBusy = false;
                ErrorCodes.GetException(comException);
            }
            catch
            {
                IsBusy = false;
                throw;
            }
            finally
            {
                Monitor.Exit(Locker);
            }
        }

        protected override void GetAdditionalLiveViewData(LiveViewData viewData, byte[] result)
        {
            viewData.LiveViewImageWidth = ToInt16(result, 8);
            viewData.LiveViewImageHeight = ToInt16(result, 10);

            viewData.ImageWidth = ToInt16(result, 12);
            viewData.ImageHeight = ToInt16(result, 14);

            viewData.FocusFrameXSize = 800;
            viewData.FocusFrameYSize = 800;

            viewData.FocusX = ToInt16(result, 20);
            viewData.FocusY = ToInt16(result, 22);

            viewData.Focused = result[40] != 1;
        }

        public override void StartRecordMovie()
        {
            SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_ApplicationMode);
            base.StartRecordMovie();
        }

        public override void StopRecordMovie()
        {
            base.StopRecordMovie();
            SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)0 }, CONST_PROP_ApplicationMode);
        }
    }
}
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

using System.IO;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices.Nikon
{
    public class NikonD3X : NikonBase
    {
        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            bool ret = base.Init(deviceDescriptor);
            Capabilities.Add(CapabilityEnum.LiveView);
            return ret;
        }

        public override LiveViewData GetLiveViewImage()
        {
            LiveViewData viewData = new LiveViewData();
            viewData.HaveFocusData = true;

            const int headerSize = 64;

            var result = StillImageDevice.ExecuteReadData(CONST_CMD_GetLiveViewImage);
            if (result.ErrorCode == ErrorCodes.MTP_Not_LiveView)
            {
                _timer.Start();
                viewData.IsLiveViewRunning = false;
                viewData.ImageData = null;
                return viewData;
            }
            if (result.Data == null || result.Data.Length <= headerSize)
                return null;
            int cbBytesRead = result.Data.Length;
            GetAdditionalLiveViewData(viewData, result.Data);

            MemoryStream copy = new MemoryStream((int) cbBytesRead - headerSize);
            copy.Write(result.Data, headerSize, (int)cbBytesRead - headerSize);
            copy.Close();
            viewData.ImageData = copy.GetBuffer();

            return viewData;
        }

        protected override void InitFocusMode()
        {
            base.InitFocusMode();
            FocusMode.IsEnabled = false;
        }
    }
}
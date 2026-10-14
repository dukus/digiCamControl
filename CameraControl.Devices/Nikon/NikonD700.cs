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

using CameraControl.Devices.Classes;
using PortableDeviceLib;
using System.Collections.Generic;
using System.IO;

#endregion

namespace CameraControl.Devices.Nikon
{
	public class NikonD700 : NikonBase
	{
		//public const int CONST_PROP_RecordingMedia = 0xD10B;
		public const int CONST_PROP_LiveViewMode = 0xD1A0;

		public override bool Init (DeviceDescriptor deviceDescriptor)
		{
			bool res = base.Init(deviceDescriptor);
			Capabilities.Clear();
			Capabilities.Add(CapabilityEnum.LiveView);
			Capabilities.Add(CapabilityEnum.RecordMovie);
			Capabilities.Add(CapabilityEnum.CaptureInRam);
			CaptureInSdRam = true;
			return res;
		}

		protected override bool SupportsAEBracketing
			=> true;

		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "1/2 EV");
				yield return (2, "2/3 EV");
				yield return (3, "1 EV");
			}
		}

		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (0, 2, "-1..0");
				yield return (1, 2, "0..+1");
				yield return (2, 3, "-2..0");
				yield return (3, 3, "0..+2");
				yield return (4, 3, "-1..+1");
				yield return (5, 5, "-2..+2");
				yield return (6, 7, "-3..+3");
				yield return (7, 9, "-4..+4");
			}
		}

		public override void StartLiveView ()
		{
			// set record media to SDRAM
			SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_RecordingMedia);
			DeviceReady();
			// set to Tripod shooting mode
			SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_LiveViewMode);
			DeviceReady();
			base.StartLiveView();
		}

		public override void StopLiveView ()
		{
			base.StopLiveView();
			DeviceReady();
			SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)0 }, CONST_PROP_RecordingMedia);
			DeviceReady();
		}

		public override void CapturePhotoNoAf ()
		{
			lock (Locker)
			{
				try
				{
					IsBusy = true;
					MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus);
					ErrorCodes.GetException(response.ErrorCode);
					// test if live view is on 
					if (response.Data != null && response.Data.Length > 0 && response.Data[0] > 0)
					{
						if (CaptureInSdRam)
						{
							DeviceReady();
							ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram, 0xFFFFFFFF));
							return;
						}
						StopLiveView();
					}
					DeviceReady();
					ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCapture));
				}
				catch
				{
					IsBusy = false;
					throw;
				}
			}
		}

		public override LiveViewData GetLiveViewImage ()
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

			MemoryStream copy = new MemoryStream((int)cbBytesRead - headerSize);
			copy.Write(result.Data, headerSize, (int)cbBytesRead - headerSize);
			copy.Close();
			viewData.ImageData = copy.GetBuffer();

			return viewData;
		}

		protected override void InitFocusMode ()
		{
			base.InitFocusMode();
			FocusMode.IsEnabled = false;
		}
	}
}
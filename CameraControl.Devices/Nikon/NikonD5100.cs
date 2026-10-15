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
using System.Collections.Generic;


#endregion

namespace CameraControl.Devices.Nikon
{
	public class NikonD5100 : NikonBase
	{
		public override bool Init (DeviceDescriptor deviceDescriptor)
		{
			bool res = base.Init(deviceDescriptor);
			Capabilities.Clear();
			Capabilities.Add(CapabilityEnum.LiveView);
			Capabilities.Add(CapabilityEnum.RecordMovie);
			Capabilities.Add(CapabilityEnum.CaptureInRam);
			Capabilities.Add(CapabilityEnum.CaptureNoAf);
			//Capabilities.Add(CapabilityEnum.Bulb);
			return res;
		}

		protected override bool SupportsAEBracketing
			=> true;

		protected override IEnumerable<(long, string)> SupportedExposureEVSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "1/2 EV");
			}
		}

		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "1/2 EV");
				yield return (2, "2/3 EV");
				yield return (3, "1 EV");
				yield return (4, "1+1/3 EV");
				yield return (5, "1+1/2 EV");
				yield return (6, "1+2/3 EV");
				yield return (7, "2 EV");
			}
		}

		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (2, 3, "-1..+1");
			}
		}

		//public override void StartLiveView()
		//{
		//  //SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_RecordingMedia, -1);
		//  //DeviceReady();
		//  base.StartLiveView();
		//}

		//public override void StopLiveView()
		//{
		//  base.StopLiveView();
		//  DeviceReady();
		//  //SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)0 }, CONST_PROP_RecordingMedia, -1);
		//  DeviceReady();
		//}
	}
}
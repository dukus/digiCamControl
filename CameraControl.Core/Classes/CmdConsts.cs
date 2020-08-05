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

#endregion

namespace CameraControl.Core.Classes
{
    public class CmdConsts
    {
        public const string LiveView_Zoom_All = "LiveView_Zoom_All";
        public const string LiveView_Zoom_25 = "LiveView_Zoom_25";
        public const string LiveView_Zoom_33 = "LiveView_Zoom_33";
        public const string LiveView_Zoom_50 = "LiveView_Zoom_50";
        public const string LiveView_Zoom_66 = "LiveView_Zoom_66";
        public const string LiveView_Zoom_100 = "LiveView_Zoom_100";
        public const string LiveView_Focus_M = "LiveView_Focus_M";
        public const string LiveView_Focus_P = "LiveView_Focus_P";
        public const string LiveView_Focus_MM = "LiveView_Focus_MM";
        public const string LiveView_Focus_PP = "LiveView_Focus_PP";
        public const string LiveView_Focus_MMM = "LiveView_Focus_MMM";
        public const string LiveView_Focus_PPP = "LiveView_Focus_PPP";
        public const string LiveView_Focus_Move_Right = "LiveView_Focus_Move_Right";
        public const string LiveView_Focus_Move_Left = "LiveView_Focus_Move_Left";
        public const string LiveView_Focus_Move_Up = "LiveView_Focus_Move_Up";
        public const string LiveView_Focus_Move_Down = "LiveView_Focus_Move_Down";
        public const string LiveView_Focus = "LiveView_Focus";
        public const string LiveView_Preview = "LiveView_Preview";
        public const string LiveView_NoProcess = "LiveView_NoProcess";
        public const string LiveView_Capture = "LiveView_Capture";
        public const string LiveView_CaptureSnapShot = "LiveView_CaptureSnapShot";
        public const string LiveView_ManualFocus = "LiveView_ManualFocus";
        public const string All_Close = "All_Close";
        public const string All_Minimize = "All_Minimize";
        public const string Capture = "Capture";
        public const string CaptureNoAf = "CaptureNoAf";
        public const string CaptureAll = "CaptureAll";
        public const string StartBulb = "StartBulb";
        public const string EndBulb = "EndBulb";
        public const string ResetDevice = "ResetDevice";
        public const string NextAperture = "NextAperture";
        public const string PrevAperture = "PrevAperture";
        public const string NextIso = "NextIso";
        public const string PrevIso = "PrevIso";
        public const string NextShutter = "NextShutter";
        public const string PrevShutter = "PrevShutter";
        public const string NextWhiteBalance = "NextWhiteBalance";
        public const string PrevWhiteBalance = "PrevWhiteBalance";
        public const string NextExposureCompensation = "NextExposureCompensation";
        public const string PrevExposureCompensation = "PrevExposureCompensation";
        public const string NextCamera = "NextCamera";
        public const string PrevCamera = "PrevCamera";
        public const string NextSeries = "NextSeries";
        public const string PrevSeries = "PrevSeries";
        public const string SortCameras = "SortCameras";
        public const string StartZoomIn = "StartZoomIn";
        public const string StopZoomIn = "StopZoomIn";
        public const string StartZoomOut = "StartZoomOut";
        public const string StopZoomOut = "StopZoomOut";
    }
}
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
using System.Xml.Serialization;

#endregion

namespace CameraControl.Core.Classes
{
    public class Branding
    {
        public string ApplicationTitle { get; set; }

        public string StartupScreenImage { get; set; }

        public string LogoImage { get; set; }

        public string DefaultThumbImage { get; set; }

        public string DefaultMissingThumbImage { get; set; }

        public bool ShowAboutWindow { get; set; }

        public bool OnlineReference { get; set; }

        public bool ShowStartupScreen { get; set; }

        public bool CheckForUpdate { get; set; }
        
        public bool ShowRefreshButtonMainWindow { get; set; }

        public bool ShowDownloadButtonMainWindow { get; set; }
       
        public bool ShowBracketingButtonMainWindow { get; set; }

        public bool ShowTimelapseButtonMainWindow { get; set; }

        public bool ShowFullScreenButtonMainWindow { get; set; }

        public bool ShowLiveViewButtonMainWindow { get; set; }

        public bool ShowBrowseButtonMainWindow { get; set; }

        public bool ShowTagsButtonMainWindow { get; set; }

        public bool ShowAstronomyButtonMainWindow { get; set; }

        public bool ShowMulyiCameraButtonMainWindow { get; set; }
        
        public bool ShowWifiCameraButtonMainWindow { get; set; }

        public bool ShowSettingMenuMainWindow { get; set; }

        public bool ShowMenuMenuMainWindow { get; set; }

        public bool ShowCameraSelectorMainWindow { get; set; }

        public bool ShowRightPanelMainWindow { get; set; }

        public bool ShowCameraPropertiesMainWindow { get; set; }

        public Branding()
        {
            ShowAboutWindow = true;
            OnlineReference = true;
            ShowStartupScreen = true;
            CheckForUpdate = true;
            ShowRefreshButtonMainWindow = true;
            ShowDownloadButtonMainWindow = true;
            ShowBracketingButtonMainWindow = true;
            ShowTimelapseButtonMainWindow = true;
            ShowFullScreenButtonMainWindow = true;
            ShowLiveViewButtonMainWindow = true;
            ShowBrowseButtonMainWindow = true;
            ShowTagsButtonMainWindow = true;
            ShowAstronomyButtonMainWindow = true;
            ShowMulyiCameraButtonMainWindow = true;
            ShowWifiCameraButtonMainWindow = true;
            ShowSettingMenuMainWindow = true;
            ShowMenuMenuMainWindow = true;
            ShowCameraSelectorMainWindow = true;
            ShowRightPanelMainWindow = true;
            ShowCameraPropertiesMainWindow = true;
        }
    }
}
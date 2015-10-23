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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Devices;

#endregion

namespace CameraControl.Core.Classes
{
    public class Branding
    {
        private string _startupScreenImage;
        private string _logoImage;
        private string _defaultThumbImage;
        public string ApplicationDataFolder { get; set; }
        
        public string ApplicationTitle { get; set; }

        public string StartupScreenImage
        {
            get
            {
                return _startupScreenImage!=null && !_startupScreenImage.Contains("\\") ? Path.Combine(Settings.ApplicationFolder, _startupScreenImage) : _startupScreenImage;
            }
            set { _startupScreenImage = value; }
        }

        public string LogoImage
        {
            get { return _logoImage!=null && !_logoImage.Contains("\\") ? Path.Combine(Settings.ApplicationFolder, _logoImage) : _logoImage;}
            set { _logoImage = value; }
        }

        public string DefaultThumbImage
        {
            get
            {
                return _defaultThumbImage != null && !_defaultThumbImage.Contains("\\")
                    ? Path.Combine(Settings.ApplicationFolder, _defaultThumbImage)
                    : _defaultThumbImage;
            }
            set { _defaultThumbImage = value; }
        }

        public string DefaultMissingThumbImage { get; set; }

        public bool ResetSettingsOnLoad { get; set; }

        public bool ShowAboutWindow { get; set; }

        public bool OnlineReference { get; set; }

        public bool ShowStartupScreen { get; set; }
        
        public bool ShowStartupScreenAnimation { get; set; }
        
        public bool ShowWelcomeScreen { get; set; }

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
        
        public bool ShowPrintButtonMainWindow { get; set; }

        public bool ShowSettingMenuMainWindow { get; set; }

        public bool ShowMenuMenuMainWindow { get; set; }

        public bool ShowCameraSelectorMainWindow { get; set; }

        public bool ShowRightPanelMainWindow { get; set; }

        public bool ShowCameraPropertiesMainWindow { get; set; }

        public bool ShowFocusControlLiveView { get; set; }
        
        public bool ShowFocusStackingLiveView { get; set; }

        public bool ShowControlLiveView { get; set; }

        public bool ShowOverlayLiveView { get; set; }

        public bool ShowDisplayLiveView { get; set; }

        public bool ShowLevelLiveView { get; set; }
        
        public bool ShowAutofocusLiveView { get; set; }

        public bool ShowLuminosityLiveView { get; set; }

        public bool ShowMotionDetectionLiveView { get; set; }

        public bool ShowAnimation { get; set; }

        public bool ShowHistogram { get; set; }
        
        public bool ShowSelectionPanel { get; set; }

        public bool ShowLayoutPanel { get; set; }

        public bool ShowPreviewPanel { get; set; }

        public bool ShowSessionPanel { get; set; }
        
        public bool ShowAutoExportPanel { get; set; }
        
        public bool ShowPresetPanel { get; set; }

        public bool ShowImagePropertiesPanel { get; set; }

        public bool ShowMetadataPanel { get; set; }

        public bool ShowExportPanel { get; set; }

        public bool ShowToolsPanel { get; set; }

        public bool ShowMainMenu { get; set; }
        
        public bool ShowBattery { get; set; }
        
        public string DefaultTheme { get; set; }

        public bool ShowCameraAdvancedProperties { get; set; }

        public bool UseThemeSelector
        {
            get { return string.IsNullOrEmpty(DefaultTheme); }
        }

        public string DefaultSettings
        {
            get { return Path.Combine(Settings.ApplicationFolder, "default_settings.xml"); }
        }

        public Branding()
        {
            ApplicationDataFolder = String.Empty;
            ResetSettingsOnLoad = false;
            ShowAboutWindow = true;
            OnlineReference = true;
            ShowStartupScreen = true;
            ShowStartupScreenAnimation = true;
            ShowWelcomeScreen = true;
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
            ShowPrintButtonMainWindow = true;
            ShowSettingMenuMainWindow = true;
            ShowMenuMenuMainWindow = true;
            ShowCameraSelectorMainWindow = true;
            ShowRightPanelMainWindow = true;
            ShowCameraPropertiesMainWindow = true;
            ShowCameraAdvancedProperties = true;
            ShowFocusControlLiveView = true;
            ShowFocusStackingLiveView = true;
            ShowControlLiveView = true;
            ShowOverlayLiveView = true;
            ShowDisplayLiveView = true;
            ShowLevelLiveView = true;
            ShowAutofocusLiveView = true;
            ShowLuminosityLiveView = true;
            ShowMotionDetectionLiveView = true;
            ShowHistogram = true;
            ShowSelectionPanel = true;
            ShowLayoutPanel = true;
            ShowPreviewPanel = true;
            ShowSessionPanel = true;
            ShowAutoExportPanel = true;
            ShowPresetPanel = true;
            ShowImagePropertiesPanel = true;
            ShowMetadataPanel = true;
            ShowExportPanel = true;
            ShowToolsPanel = true;
            ShowAnimation = true;
            ShowMainMenu = true;
            ShowBattery = true;
        }


        public static Branding LoadBranding()
        {
            Branding branding = new Branding();
            string filename = Path.Combine(Settings.ApplicationFolder, "Branding.xml");
            try
            {
                if (File.Exists(filename))
                {
                    XmlSerializer mySerializer =
                        new XmlSerializer(typeof (Branding));
                    FileStream myFileStream = new FileStream(filename, FileMode.Open,FileAccess.Read,FileShare.Read);
                    branding = (Branding) mySerializer.Deserialize(myFileStream);
                    myFileStream.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return branding;
        }
    }
}
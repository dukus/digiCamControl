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
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    public class CameraPreset : BaseFieldClass
    {
        public string Name { get; set; }

        public AsyncObservableCollection<ValuePair> Values { get; set; }

        public CameraPreset()
        {
            Values = new AsyncObservableCollection<ValuePair>();
        }

        public string FileName
        {
            get { return Path.Combine(Settings.PresetFolder, Name + ".xml"); }
        }

        public void Get(ICameraDevice camera)
        {
            Add(GetFrom(camera.Mode, "Mode"));
            Add(GetFrom(camera.CompressionSetting, "CompressionSetting"));
            Add(GetFrom(camera.ExposureCompensation, "ExposureCompensation"));
            Add(GetFrom(camera.ExposureMeteringMode, "ExposureMeteringMode"));
            Add(GetFrom(camera.FNumber, "FNumber"));
            Add(GetFrom(camera.IsoNumber, "IsoNumber"));
            Add(GetFrom(camera.ShutterSpeed, "ShutterSpeed"));
            Add(GetFrom(camera.WhiteBalance, "WhiteBalance"));
            Add(GetFrom(camera.FocusMode, "FocusMode"));
            Add(GetFrom(camera.LiveViewImageZoomRatio, "LiveViewImageZoomRatio"));
            Add(new ValuePair {Name = "CaptureInSdRam", Value = camera.CaptureInSdRam.ToString()});
            var property = camera.LoadProperties();
            Add(new ValuePair { Name = "NoDownload", Value = property.NoDownload.ToString() });
            if (camera.AdvancedProperties != null)
            {
                foreach (PropertyValue<long> propertyValue in camera.AdvancedProperties)
                {
                    Add(GetFrom(propertyValue, propertyValue.Name));
                }
            }
        }

        public void Verify(ICameraDevice camera)
        {
            try
            {
                camera.Mode.HaveError = camera.Mode.Value != GetValue("Mode");
                camera.CompressionSetting.HaveError = camera.CompressionSetting.Value != GetValue("CompressionSetting");
                camera.ExposureCompensation.HaveError = camera.ExposureCompensation.Value != GetValue("ExposureCompensation");
                camera.ExposureMeteringMode.HaveError = camera.ExposureMeteringMode.Value != GetValue("ExposureMeteringMode");
                camera.FNumber.HaveError = camera.FNumber.Value != GetValue("FNumber");
                camera.IsoNumber.HaveError = camera.IsoNumber.Value != GetValue("IsoNumber");
                camera.ShutterSpeed.HaveError = camera.ShutterSpeed.Value != GetValue("ShutterSpeed");
                camera.WhiteBalance.HaveError = camera.WhiteBalance.Value != GetValue("WhiteBalance");
                camera.FocusMode.HaveError = camera.FocusMode.Value != GetValue("FocusMode");
                if (camera.AdvancedProperties != null)
                {
                    foreach (PropertyValue<long> propertyValue in camera.AdvancedProperties)
                    {
                        propertyValue.HaveError = propertyValue.Value != GetValue(propertyValue.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to verify the preset " + Name, ex);
            }
        }

        public void Set(ICameraDevice camera)
        {
            SetIntern(camera);
            Thread.SpinWait(250);
            SetIntern(camera);
        }

        public void SetIntern(ICameraDevice camera)
        {
            Log.Debug("Loading preset for "+camera.DisplayName);
            camera.IsBusy = true;
            SetTo(camera.Mode, "Mode");
            SetTo(camera.CompressionSetting, "CompressionSetting");
            SetTo(camera.ExposureCompensation, "ExposureCompensation");
            SetTo(camera.ExposureMeteringMode, "ExposureMeteringMode");
            SetTo(camera.FNumber, "FNumber");
            SetTo(camera.IsoNumber, "IsoNumber");
            SetTo(camera.ShutterSpeed, "ShutterSpeed");
            SetTo(camera.WhiteBalance, "WhiteBalance");
            SetTo(camera.FocusMode, "FocusMode");
            SetTo(camera.LiveViewImageZoomRatio, "LiveViewImageZoomRatio");
            var property = camera.LoadProperties();
            if (!string.IsNullOrEmpty(GetValue("CaptureInSdRam")))
            {
                bool val;
                if (bool.TryParse(GetValue("CaptureInSdRam"), out val))
                {
                    camera.CaptureInSdRam = val;
                    property.CaptureInSdRam = val;
                }
            }
            if (!string.IsNullOrEmpty(GetValue("NoDownload")))
            {
                bool val;
                if (bool.TryParse(GetValue("NoDownload"), out val))
                {
                    property.NoDownload = val;
                }
            }
            if (camera.AdvancedProperties != null)
            {
                foreach (PropertyValue<long> propertyValue in camera.AdvancedProperties)
                {
                    SetTo(propertyValue, propertyValue.Name);
                }
            }
            
            camera.IsBusy = false;
            Verify(camera);
        }


        public void SetTo(PropertyValue<int> value, string name)
        {
            if (value == null)
                return;
            foreach (ValuePair valuePair in Values)
            {
                if (valuePair.Name == name && value.IsEnabled && !string.IsNullOrEmpty(valuePair.Value) )
                {
                    value.SetValue(valuePair.Value);
                    return;
                }
            }
            Thread.Sleep(25);
        }

        public void SetTo(PropertyValue<uint> value, string name)
        {
            if (value == null)
                return;
            foreach (ValuePair valuePair in Values)
            {
                if (valuePair.Name == name && value.IsEnabled && value.Value != valuePair.Value)
                {
                    value.SetValue(valuePair.Value);
                    return;
                }
            }
            //Thread.Sleep(100);
       }


        public void SetTo(PropertyValue<long> value, string name)
        {
            if (value == null)
            {
                Log.Debug("Value is null ");
                return;
            }
            foreach (ValuePair valuePair in Values)
            {
                // set the value only if the value is different from current value 
                if (valuePair.Name == name && value.IsEnabled && value.Value != valuePair.Value)
                {
                    if (value.Values.Count == 0)
                    {
                        Log.Debug("No value list " + value.Name);
                    }
                    value.SetValue(valuePair.Value);
                    return;
                }
            }
        }


        private ValuePair GetFrom(PropertyValue<uint> value, string name)
        {
            if (value == null)
                return null;
            return new ValuePair {Name = name, IsDisabled = value.IsEnabled, Value = value.Value};
        }

        private ValuePair GetFrom(PropertyValue<int> value, string name)
        {
            if (value == null)
                return null;
            return new ValuePair {Name = name, IsDisabled = value.IsEnabled, Value = value.Value};
        }

        private ValuePair GetFrom(PropertyValue<long> value, string name)
        {
            if (value == null)
                return null;
            return new ValuePair {Name = name, IsDisabled = value.IsEnabled, Value = value.Value};
        }

        public void Add(ValuePair pairParam)
        {
            if (pairParam == null)
                return;
            ValuePair pair = new ValuePair()
                                 {
                                     Name = pairParam.Name,
                                     IsDisabled = pairParam.IsDisabled,
                                     Value = pairParam.Value == null ? null : pairParam.Value.Replace('\0', ' ')
                                 };
            foreach (ValuePair valuePair in Values)
            {
                if (pair.Name == valuePair.Name)
                {
                    valuePair.Value = pair.Value;
                    valuePair.IsDisabled = pair.IsDisabled;
                    return;
                }
            }
            Values.Add(pair);
        }

        public string GetValue(string name)
        {
            return (from valuePair in Values where valuePair.Name == name select valuePair.Value).FirstOrDefault();
        }

        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (CameraPreset));
            // Create a FileStream to write with.

            Stream writer = new FileStream(filename, FileMode.Create);
            // Serialize the object, and close the TextWriter
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static CameraPreset Load(string filename)
        {
            CameraPreset cameraPreset = new CameraPreset();
            if (File.Exists(filename))
            {
                XmlSerializer mySerializer =
                    new XmlSerializer(typeof (CameraPreset));
                FileStream myFileStream = new FileStream(filename, FileMode.Open);
                cameraPreset = (CameraPreset) mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
            }
            return cameraPreset;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
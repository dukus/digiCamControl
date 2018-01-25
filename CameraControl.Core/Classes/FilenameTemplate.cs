using System;
using System.IO;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Core.Classes
{
    public class FilenameTemplate : IFilenameTemplate
    {
        public bool IsRuntime => false;

        public string Pharse(string template, PhotoSession session, ICameraDevice device, string fileName, string tempfileName)
        {
            CameraProperty property = device.LoadProperties();
            switch (template)
            {
                case "[Counter 3 digit]":
                case "[Counter 4 digit]":
                case "[Counter 5 digit]":
                case "[Counter 6 digit]":
                case "[Counter 7 digit]":
                case "[Counter 8 digit]":
                case "[Counter 9 digit]":
                    return session.Counter.ToString(new string('0', Convert.ToInt16(template.Substring(9, 1))));
                case "[Series 4 digit]":
                    return session.Series.ToString(new string('0', 4));
                case "[Camera Counter 3 digit]":
                case "[Camera Counter 4 digit]":
                case "[Camera Counter 5 digit]":
                case "[Camera Counter 6 digit]":
                case "[Camera Counter 7 digit]":
                case "[Camera Counter 8 digit]":
                case "[Camera Counter 9 digit]":
                    return property.Counter.ToString(new string('0', Convert.ToInt16(template.Substring(16, 1))));
                case "[Session Name]":
                    return session.Name;
                case "[Capture Name]":
                    return session.CaptureName;
                case "[Exposure Compensation]":
                    if (device!=null && device.ExposureCompensation != null)
                        return device.ExposureCompensation.Value != "0" ? device.ExposureCompensation.Value : "";
                    return "";
                case "[FNumber]":
                    if (device != null && device.FNumber != null)
                        return device.FNumber.Value ?? "";
                    return "";
                case "[Date yyyy-MM-dd]":
                    return DateTime.Now.ToString("yyyy-MM-dd");
                case "[Date yyyy]":
                    return DateTime.Now.ToString("yyyy");
                case "[Date yyyy-MM]":
                    return DateTime.Now.ToString("yyyy-MM");
                case "[Date yyyyMMdd]":
                    return DateTime.Now.ToString("yyyyMMdd");
                case "[Date MMM]":
                    return DateTime.Now.ToString("MMM");
                case "[Date MM]":
                    return DateTime.Now.ToString("MM");
                case "[Date dd]":
                    return DateTime.Now.ToString("dd");
                case "[Date HH]":
                    return DateTime.Now.ToString("HH");
                case "[Date mm]":
                    return DateTime.Now.ToString("mm");
                case "[Date ss]":
                    return DateTime.Now.ToString("ss");
                case "[Date yyyy-MM-dd-hh-mm-ss]":
                    return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                case "[Time hh-mm-ss]":
                    return DateTime.Now.ToString("HH-mm-ss");
                case "[Time hhmmss]":
                    return DateTime.Now.ToString("HHmmss");
                case "[Time hh-mm]":
                    return DateTime.Now.ToString("HH-mm");
                case "[Time hh]":
                    return DateTime.Now.ToString("HH");
                case "[Barcode]":
                    return session.Barcode;
                case "[File format]":
                    return GetType(fileName);
                case "[Original Filename]":
                    return Path.GetFileNameWithoutExtension(fileName);
                case "[Camera Order]":
                    return property.SortOrder.ToString("D3");
                case "[Camera Name]":
                    return property.DeviceName.Replace(":", "_").Replace("?", "_").Replace("*", "_");
                case "[Selected Tag1]":
                    return session.SelectedTag1 != null ? session.SelectedTag1.Value.Trim() : "";
                case "[Selected Tag2]":
                    return session.SelectedTag2 != null ? session.SelectedTag2.Value.Trim() : "";
                case "[Selected Tag3]":
                    return session.SelectedTag3 != null ? session.SelectedTag3.Value.Trim() : "";
                case "[Selected Tag4]":
                    return session.SelectedTag4 != null ? session.SelectedTag4.Value.Trim() : "";
                case "[Unix Time]":
                    var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTime.Now.Kind);
                    var unixTimestamp = System.Convert.ToInt64((DateTime.Now - date).TotalSeconds);
                    return unixTimestamp.ToString();
                case "[DB Row 1]":
                    return (session.ExternalData != null && session.ExternalData.Row1 != null)
                        ? session.ExternalData.Row1
                        : "";
                case "[DB Row 2]":
                    return (session.ExternalData != null && session.ExternalData.Row2 != null)
                        ? session.ExternalData.Row2
                        : "";
                case "[DB Row 3]":
                    return (session.ExternalData != null && session.ExternalData.Row3 != null)
                        ? session.ExternalData.Row3
                        : "";
                case "[DB Row 4]":
                    return (session.ExternalData != null && session.ExternalData.Row4 != null)
                        ? session.ExternalData.Row4
                        : "";
                case "[DB Row 5]":
                    return (session.ExternalData != null && session.ExternalData.Row5 != null)
                        ? session.ExternalData.Row5
                        : "";
                case "[DB Row 6]":
                    return (session.ExternalData != null && session.ExternalData.Row6 != null)
                        ? session.ExternalData.Row6
                        : "";
                case "[DB Row 7]":
                    return (session.ExternalData != null && session.ExternalData.Row7 != null)
                        ? session.ExternalData.Row7
                        : "";
                case "[DB Row 8]":
                    return (session.ExternalData != null && session.ExternalData.Row8 != null)
                        ? session.ExternalData.Row8
                        : "";
                case "[DB Row 9]":
                    return (session.ExternalData != null && session.ExternalData.Row9 != null)
                        ? session.ExternalData.Row9
                        : "";
            }
            return "";
        }

        private string GetType(string file)
        {
            var ext = Path.GetExtension(file);
            if (ext.StartsWith("."))
                ext = ext.Substring(1);
            switch (ext.ToLower())
            {
                case "jpg":
                    return "Jpg";
                case "nef":
                    return "Raw";
                case "cr2":
                    return "Raw";
                case "tif":
                    return "Tif";
            }
            return ext;
        }

    }
}

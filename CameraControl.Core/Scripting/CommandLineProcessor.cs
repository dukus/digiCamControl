using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Scripting
{
    public class CommandLineProcessor
    {
        public ICameraDevice TargetDevice { get; set; }

        public object Pharse(string[] args)
        {
            var cmd = args[0].ToLower().Trim();
            switch (cmd)
            {
                case "capturenoaf":
                case "capture":
                    if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
                    {
                        // fix if space is in file name 
                        //http://digicamcontrol.com/phpbb/viewtopic.php?f=4&t=1929&p=5486#p5486
                        var file = string.Join(" ", args.Skip(1)).Trim();
                        if (file.Contains(":\\") || file.StartsWith(@"\\"))
                        {
                            ServiceProvider.Settings.DefaultSession.Folder = Path.GetDirectoryName(file);
                            ServiceProvider.Settings.DefaultSession.FileNameTemplate = Path.GetFileNameWithoutExtension(file);
                        }
                        else
                        {
                            ServiceProvider.Settings.DefaultSession.FileNameTemplate = file;
                        }

                    }
                    if (cmd == "capturenoaf")
                        CameraHelper.CaptureNoAf();
                    else
                        CameraHelper.CaptureWithError(TargetDevice);
                    ServiceProvider.DeviceManager.SelectedCameraDevice.WaitForCamera(30000);
                    return null;
                    
                //case "startbulb":
                //    CameraHelper.Capture(ServiceProvider.DeviceManager.SelectedCameraDevice);
                //    return null;
                //case "stopbulb":
                case "set":
                    Set(args.Skip(1).ToArray());
                    Thread.Sleep(200);
                    return null;
                case "do":
                    return DoCmd(args.Skip(1).ToArray());
                case "get":
                    return Get(args.Skip(1).ToArray());
                case "list":
                    return List(args.Skip(1).ToArray());
                default:
                    throw new Exception("Unknow parameter " + cmd);
            }
        }

        public void SetCamera(string camera)
        {
            if (string.IsNullOrEmpty(camera))
                return;
            foreach (var cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
            {
                if ((PhotoUtils.IsNumeric(camera) && cameraDevice.SerialNumber == camera.Trim()) || cameraDevice.DeviceName.Replace(" ", "_") == camera.Replace(" ", "_"))
                {
                    TargetDevice = cameraDevice;
                    break;
                }
            }
        }

        private string DoCmd(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();
            switch (arg)
            {
                case "startrecord":
                    return CameraHelper.StartRecordVideo(device);
                    break;
                case "stoprecord":
                    return CameraHelper.StopRecordVideo(device);
                    break;
                default:
                    ServiceProvider.WindowsManager.ExecuteCommand(args[0]);
                    // cammand with _ are special commands 
                    if (!args[0].Contains("_") && !ServiceProvider.Settings.Actions.Select((x) => x.Name).Contains(args[0]))
                        throw new Exception(string.Format("Invalid command {0}", args[0]));
                    break;
            }
            return "";
        }

        private object List(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();

            switch (arg)
            {
                case "shutterspeed":
                    return device.ShutterSpeed.Values;
                case "iso":
                    return device.IsoNumber.Values;
                case "exposurecompensation":
                    return device.ExposureCompensation.Values;
                case "aperture":
                    return device.FNumber.Values;
                case "focusmode":
                    return device.FocusMode.Values;
                case "whitebalance":
                    return device.WhiteBalance.Values;
                case "mode":
                    return device.Mode.Values;
                case "compressionsetting":
                    return device.CompressionSetting.Values;
                case "sessions":
                    return ServiceProvider.Settings.PhotoSessions.Select(x => x.Name).ToArray();
                case "cmds":
                    var reslist = ServiceProvider.WindowsManager.WindowCommands.Select(x => x.Name).ToList();
                    reslist.Add("StartRecord");
                    reslist.Add("StopRecord");
                    return reslist;
                case "cameras":
                    return
                        ServiceProvider.DeviceManager.ConnectedDevices.Where(x => x.IsConnected)
                            .Select(x => x.SerialNumber)
                            .ToArray();
                case "session":
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                        return (from prop in props where prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool) select "session." + prop.Name.ToLower() + "=" + prop.GetValue(ServiceProvider.Settings.DefaultSession, null)).ToList();
                    }
                case "property":
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(CameraProperty).GetProperties());
                        return (from prop in props where prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool) select "property." + prop.Name.ToLower() + "=" + prop.GetValue(device.LoadProperties(), null)).ToList();
                    }
                case "liveview":
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(LiveviewSettings).GetProperties());
                        return (from prop in props where prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool) select "liveview." + prop.Name.ToLower() + "=" + prop.GetValue(device.LoadProperties().LiveviewSettings, null)).ToList();
                    }
                case "camera":
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
                        List<string> res = new List<string>();
                        foreach (PropertyInfo info in props)
                        {
                            if (info.PropertyType.Name.StartsWith("PropertyValue"))
                            {
                                dynamic valp = info.GetValue(device, null);
                                object val = valp.Value;
                                if (val != null)
                                {
                                    res.Add("camera." + info.Name.ToLower() + "=" + val);
                                }
                            }
                        }
                        foreach (PropertyValue<long> property in device.AdvancedProperties)
                        {
                            if (!string.IsNullOrEmpty(property.Name) && property.Value != null)
                            {
                                res.Add("camera." + property.Name.ToLower().Replace(" ", "_") + "=" + property.Value);
                            }
                        }
                        res.Add("camera." + "exposurestatus" + "=" + device.ExposureStatus);
                        return res;
                    }
                default:
                    if (arg.StartsWith("camera."))
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
                        foreach (PropertyInfo info in props)
                        {
                            if (info.PropertyType.Name.StartsWith("PropertyValue") &&
                                (arg.Split('.')[1].ToLower().Replace(" ", "_") == info.Name.ToLower()))
                            {
                                dynamic valp = info.GetValue(device, null);
                                if (valp != null)
                                    return valp.Values;
                                else
                                    return new[] { "" };
                            }
                        }
                        foreach (PropertyValue<long> property in device.AdvancedProperties)
                        {
                            if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].ToLower() == property.Name.ToLower().Replace(" ", "_")))
                            {
                                return property.Values;
                            }
                        }
                    }
                    throw new Exception("Unknow parameter");
            }
        }

        private object Get(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();

            switch (arg)
            {
                case "transfer":
                    {
                        CameraProperty property = ServiceProvider.DeviceManager.SelectedCameraDevice.LoadProperties();
                        if (ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureInRam))
                        {
                            if (ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam)
                                return "Save to PC only";
                            if (!ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam && property.NoDownload)
                                return "Save to camera only";
                            return "Save to PC and camera";
                        }

                        return (property.NoDownload) ? "Save to camera only" : "Save to PC and camera";
                    }
                case "shutterspeed":
                    return device.ShutterSpeed.Value;
                case "iso":
                    return device.IsoNumber.Value;
                case "exposurecompensation":
                    return device.ExposureCompensation.Value;
                case "aperture":
                    return device.FNumber.Value;
                case "focusmode":
                    return device.FocusMode.Value;
                case "whitebalance":
                    return device.WhiteBalance.Value;
                case "mode":
                    return device.Mode.Value;
                case "compressionsetting":
                    return device.CompressionSetting.Value;
                case "lastcaptured":
                    if (!ServiceProvider.DeviceManager.LastCapturedImage.ContainsKey(device) || string.IsNullOrEmpty(ServiceProvider.DeviceManager.LastCapturedImage[device]))
                        return "?";
                    if (ServiceProvider.DeviceManager.LastCapturedImage[device] == "-")
                        return "-";
                    return Path.GetFileName(ServiceProvider.DeviceManager.LastCapturedImage[device]);
                case "session":
                    return ServiceProvider.Settings.DefaultSession.Name;
                case "camera.exposurestatus":
                    return device.ExposureStatus;
                case "camera.recordcondition":
                    return device.GetProhibitionCondition(OperationEnum.RecordMovie);
                case "camera":
                    return device.SerialNumber;
                default:
                    if (arg.StartsWith("session."))
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                prop.PropertyType == typeof(bool))
                            {
                                if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
                                {
                                    return prop.GetValue(ServiceProvider.Settings.DefaultSession, null);
                                }
                            }
                        }
                    }
                    if (arg.StartsWith("property."))
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(CameraProperty).GetProperties());
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                prop.PropertyType == typeof(bool))
                            {
                                if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
                                {
                                    return prop.GetValue(device.LoadProperties(), null);
                                }
                            }
                        }
                    }
                    if (arg.StartsWith("liveview."))
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(LiveviewSettings).GetProperties());
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                prop.PropertyType == typeof(bool))
                            {
                                if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
                                {
                                    return prop.GetValue(device.LoadProperties().LiveviewSettings, null);
                                }
                            }
                        }
                    }
                    if (arg.StartsWith("camera."))
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
                        foreach (PropertyInfo info in props)
                        {
                            if (info.PropertyType.Name.StartsWith("PropertyValue") &&
                                (arg.Split('.')[1].ToLower().Replace("_", " ") == info.Name.ToLower())
                                )
                            {
                                dynamic valp = info.GetValue(device, null);
                                object val = valp.Value;
                                if (val != null)
                                {
                                    return val;
                                }
                            }
                        }
                        foreach (PropertyValue<long> property in device.AdvancedProperties)
                        {
                            if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].ToLower().Replace("_", " ") == property.Name.ToLower()))
                            {
                                return property.Value;
                            }
                        }
                    }
                    throw new Exception("Unknow parameter");
            }
        }


        private void Set(string[] args)
        {
            var device = GetDevice();
            args = args.ToArray().Aggregate("", (current, s) => current + s + " ").Split('|');

            string arg;
            string param;
            for (int k = 0; k < args.Length; k++)
            {
                bool notFound = true;
                arg = args[k].Split(' ')[0];
                param = args[k].Skip(arg.Length).ToArray().Aggregate("", (current, s) => current + s).Trim();
                arg = arg.Trim().ToLower();
                switch (arg)
                {
                    case "transfer":
                        {
                            CameraProperty property = ServiceProvider.DeviceManager.SelectedCameraDevice.LoadProperties();
                            var val = param.Replace("_", " ");
                            switch (val)
                            {
                                case "Save to PC only":
                                    if (ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureInRam))
                                        ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam = true;
                                    break;
                                case "Save to camera only":
                                    property.NoDownload = true;
                                    ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam = false;
                                    break;
                                case "Save to PC and camera":
                                    property.NoDownload = false;
                                    ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam = false;
                                    break;
                            }
                        }
                        break;
                    case "shutterspeed":
                        {
                            var val = param;
                            if (val.Equals("bulb"))
                            {
                                val = "Bulb";
                            }
                            // if the value not found check 
                            if (!device.ShutterSpeed.Values.Contains(val))
                                if (!val.Contains("/") && !val.EndsWith("s") && !val.Equals("bulb"))
                                {
                                    val += "s";
                                }

                            if (!device.ShutterSpeed.Values.Contains(val))
                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                            device.ShutterSpeed.SetValue(val);
                        }
                        break;
                    case "iso":
                        if (!device.IsoNumber.Values.Contains(param))
                            throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                        device.IsoNumber.SetValue(param);
                        break;
                    case "exposurecompensation":
                        if (!device.ExposureCompensation.Values.Contains(param))
                            throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                        device.ExposureCompensation.SetValue(param);
                        break;
                    case "aperture":
                        {
                            var val = param;
                            if (!val.Contains("."))
                                val = val + ".0";
                            if (!device.FNumber.Values.Contains(val))
                                throw new Exception(string.Format("Wrong value {0} for property aperture", val));
                            device.FNumber.SetValue(param);
                        }
                        break;
                    case "focusmode":
                        if (!device.FocusMode.Values.Contains(param))
                            throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                        device.FocusMode.SetValue(param);
                        break;
                    case "whitebalance":
                        if (device?.WhiteBalance != null)
                        {
                            if (!device.WhiteBalance.Values.Contains(param) == true)
                                throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                            device.WhiteBalance.SetValue(param);
                        }
                        break;
                    case "mode":
                        if (!device?.Mode?.Values?.Contains(param)==true)
                            throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                        device.Mode.SetValue(param);
                        break;
                    case "compressionsetting":
                        if (!device.CompressionSetting.Values.Contains(param))
                            throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                        device.CompressionSetting.SetValue(param);
                        break;
                    case "camera":
                        {
                            foreach (var cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                            {
                                if ((PhotoUtils.IsNumeric(param) && cameraDevice.SerialNumber == param) || cameraDevice.DeviceName.Replace(" ", "_") == param.Replace(" ", "_"))
                                {
                                    ServiceProvider.DeviceManager.SelectedCameraDevice = cameraDevice;
                                    break;
                                }
                            }
                        }
                        break;
                    case "session":
                        device.CompressionSetting.SetValue(param);
                        foreach (var session in ServiceProvider.Settings.PhotoSessions)
                        {
                            if (session.Name.ToLower() == param.ToLower())
                            {
                                ServiceProvider.Settings.DefaultSession = session;
                                notFound = false;
                                break;// return;
                            }
                        }
                        if (notFound)
                            throw new Exception("Unknow session name");
                        else
                            break;
                    default:
                        if (arg.StartsWith("session."))
                        {
                            var val = param;
                            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                            foreach (PropertyInfo prop in props)
                            {
                                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                    prop.PropertyType == typeof(bool))
                                {
                                    if (arg.Split('.')[1] == prop.Name.ToLower())
                                    {
                                        if (prop.PropertyType == typeof(string))
                                        {
                                            prop.SetValue(ServiceProvider.Settings.DefaultSession, val, null);
                                            notFound = false;
                                        }
                                        else if (prop.PropertyType == typeof(bool))
                                        {
                                            val = val.ToLower().Trim();
                                            if (val != "true" && val != "false" && val != "0" && val != "1")
                                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                                            prop.SetValue(ServiceProvider.Settings.DefaultSession, (val == "true" || val == "1"), null);
                                            notFound = false;
                                        }
                                        else if (prop.PropertyType == typeof(int))
                                        {
                                            int i = 0;
                                            if (int.TryParse(val, out i))
                                            {
                                                prop.SetValue(ServiceProvider.Settings.DefaultSession, i, null);
                                                notFound = false;
                                            }
                                            else
                                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                                        }
                                        break;
                                    }
                                }
                            }
                            //return;
                        }
                        else if (arg.StartsWith("property."))
                        {
                            var val = param;
                            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(CameraProperty).GetProperties());
                            foreach (PropertyInfo prop in props)
                            {
                                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                    prop.PropertyType == typeof(bool))
                                {
                                    if (arg.Split('.')[1] == prop.Name.ToLower())
                                    {
                                        if (prop.PropertyType == typeof(string))
                                        {
                                            notFound = false;
                                            prop.SetValue(device.LoadProperties(), val, null);
                                        }
                                        else if (prop.PropertyType == typeof(bool))
                                        {
                                            val = val.ToLower().Trim();
                                            if (val != "true" && val != "false" && val != "0" && val != "1")
                                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                                            notFound = false;
                                            prop.SetValue(ServiceProvider.Settings.DefaultSession, (val == "true" || val == "1"), null);
                                        }
                                        else if (prop.PropertyType == typeof(int))
                                        {
                                            int i = 0;
                                            if (int.TryParse(val, out i))
                                            {
                                                notFound = false;
                                                prop.SetValue(device.LoadProperties(), i, null);
                                            }
                                            else
                                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                                        }
                                        break;
                                    }
                                }
                            }
                            //return;
                        }
                        else if (arg.StartsWith("liveview."))
                        {
                            var val = param;
                            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(LiveviewSettings).GetProperties());
                            foreach (PropertyInfo prop in props)
                            {
                                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                    prop.PropertyType == typeof(bool))
                                {
                                    if (arg.Split('.')[1] == prop.Name.ToLower())
                                    {
                                        if (prop.PropertyType == typeof(string))
                                        {
                                            notFound = false;
                                            prop.SetValue(device.LoadProperties().LiveviewSettings, val, null);
                                        }
                                        else if (prop.PropertyType == typeof(bool))
                                        {
                                            val = val.ToLower().Trim();
                                            if (val != "true" && val != "false" && val != "0" && val != "1")
                                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                                            notFound = false;
                                            prop.SetValue(ServiceProvider.Settings.DefaultSession, (val == "true" || val == "1"), null);
                                        }
                                        else if (prop.PropertyType == typeof(int))
                                        {
                                            int i = 0;
                                            if (int.TryParse(val, out i))
                                            {
                                                notFound = false;
                                                prop.SetValue(device.LoadProperties().LiveviewSettings, i, null);
                                            }
                                            else
                                                throw new Exception(string.Format("Wrong value {0} for property {1}", val, arg));
                                        }
                                        break;
                                    }
                                }
                            }
                            //return;
                        }
                        else if (arg.StartsWith("camera."))
                        {
                            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
                            foreach (PropertyInfo info in props)
                            {
                                if (info.PropertyType.Name.StartsWith("PropertyValue") &&
                                    (arg.Split('.')[1].Replace("_", " ") == info.Name.ToLower())
                                    )
                                {
                                    dynamic valp = info.GetValue(device, null);
                                    if (!valp.Values.Contains(param.Replace("_", " ")))
                                        throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                                    valp.Value = param.Replace("_", " ");
                                    notFound = false;
                                    break;
                                }
                            }
                            if (notFound)
                                foreach (PropertyValue<long> property in device.AdvancedProperties)
                                {
                                    if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].Replace("_", " ") == property.Name.ToLower()))
                                    {
                                        if (!property.Values.Contains(param.Replace("_", " ")))
                                            throw new Exception(string.Format("Wrong value {0} for property {1}", param, arg));
                                        property.Value = param.Replace("_", " ");
                                        notFound = false;
                                        break;
                                    }
                                }
                            //return;
                        }
                        if (notFound)
                            throw new Exception("Unknow parameter");
                        break;
                }
            }
        }

        private ICameraDevice GetDevice()
        {
            if (ServiceProvider.DeviceManager == null)
                return null;
            if (TargetDevice != null)
                return TargetDevice;
            return ServiceProvider.DeviceManager.SelectedCameraDevice;
        }
    }
}

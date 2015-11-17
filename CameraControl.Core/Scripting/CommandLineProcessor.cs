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
                case "capture":
                    if (args.Length > 1)
                    {
                        var file = args[1];
                        if (file.Contains(":\\") || file.StartsWith(@"\\"))
                        {
                            ServiceProvider.Settings.DefaultSession.Folder = Path.GetDirectoryName(file);
                        }
                        ServiceProvider.Settings.DefaultSession.FileNameTemplate = Path.GetFileNameWithoutExtension(file);
                    }
                    CameraHelper.CaptureWithError(TargetDevice);
                    ServiceProvider.DeviceManager.SelectedCameraDevice.WaitForCamera(3000);
                    return null;
                case "capturenoaf":
                    //Task.Factory.StartNew(CameraHelper.Capture);
                    //Thread.Sleep(200);
                    CameraHelper.CaptureNoAf();
                    ServiceProvider.DeviceManager.SelectedCameraDevice.WaitForCamera(3000);
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
                    DoCmd(args.Skip(1).ToArray());

                    return null;
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

        private void DoCmd(string[] args)
        {
            ServiceProvider.WindowsManager.ExecuteCommand(args[0]);
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
                    return ServiceProvider.WindowsManager.WindowCommands.Select(x => x.Name).ToArray();
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
                                return valp.Values;
                            }
                        }
                        foreach (PropertyValue<long> property in device.AdvancedProperties)
                        {
                            if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].ToLower() == property.Name.ToLower().Replace(" ","_")))
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
                        return "-";
                    return Path.GetFileName(ServiceProvider.DeviceManager.LastCapturedImage[device]);
                case "session":
                    return ServiceProvider.Settings.DefaultSession.Name;
                case "camera.exposurestatus":
                    return device.ExposureStatus;
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
                    if (arg.StartsWith("camera."))
                    {
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(ICameraDevice).GetProperties());
                        foreach (PropertyInfo info in props)
                        {
                            if (info.PropertyType.Name.StartsWith("PropertyValue") &&
                                (arg.Split('.')[1].ToLower().Replace(" ", "_") == info.Name.ToLower())
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
                            if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].ToLower() == property.Name.ToLower()))
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
            var arg = args[0].ToLower().Trim();
            var param = args.Skip(1).ToArray().Aggregate("", (current, s) => current + s + " ");
            args[1] = param.Trim();
            switch (arg)
            {
                case "shutterspeed":
                    {
                        var val = args[1].Trim();
                        if (!val.Contains("/") && !val.EndsWith("s") && !val.Equals("bulb"))
                        {
                            val += "s";
                        }
                        if (val.Equals("bulb"))
                        {
                            val = "Bulb";
                        }
                        device.ShutterSpeed.SetValue(val);
                    }
                    break;
                case "iso":
                    device.IsoNumber.SetValue(args[1]);
                    break;
                case "exposurecompensation":
                    device.ExposureCompensation.SetValue(args[1]);
                    break;
                case "aperture":
                    {
                        var val = args[1].Trim();
                        val = val.Replace("f", "ƒ");
                        if (!val.Contains("/"))
                        {
                            val = "ƒ/" + val;
                        }
                        if (!val.Contains("."))
                            val = val + ".0";
                        if (!device.FNumber.Values.Contains(val))
                            throw new Exception(string.Format("Wrong value {0} for property aperture", val));
                        device.FNumber.SetValue(args[1]);
                    }
                    break;
                case "focusmode":
                    device.FocusMode.SetValue(args[1]);
                    break;
                case "whitebalance":
                    device.WhiteBalance.SetValue(args[1]);
                    break;
                case "mode":
                    device.Mode.SetValue(args[1]);
                    break;
                case "compressionsetting":
                    device.CompressionSetting.SetValue(args[1]);
                    break;
                case "camera":
                    {
                        foreach (var cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                        {
                            if ((PhotoUtils.IsNumeric(args[1]) && cameraDevice.SerialNumber == args[1]) || cameraDevice.DeviceName.Replace(" ", "_") == args[1].Replace(" ", "_"))
                            {
                                ServiceProvider.DeviceManager.SelectedCameraDevice = cameraDevice;
                                break;
                            }
                        }
                    }
                    break;
                case "session":
                    device.CompressionSetting.SetValue(args[1]);
                    foreach (var session in ServiceProvider.Settings.PhotoSessions)
                    {
                        if (session.Name.ToLower() == args[1].ToLower().Trim())
                        {
                            ServiceProvider.Settings.DefaultSession = session;
                            return;
                        }
                    }
                    throw new Exception("Unknow session name");
                default:
                    if (arg.StartsWith("session."))
                    {
                        var val = args[1].Trim();
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                prop.PropertyType == typeof(bool))
                            {
                                if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
                                {
                                    if (prop.PropertyType == typeof(string))
                                    {
                                        prop.SetValue(ServiceProvider.Settings.DefaultSession, val, null);
                                    }
                                    if (prop.PropertyType == typeof(bool))
                                    {
                                        prop.SetValue(ServiceProvider.Settings.DefaultSession, val == "true", null);
                                    }
                                    if (prop.PropertyType == typeof(int))
                                    {
                                        int i = 0;
                                        if (int.TryParse(val, out i))
                                            prop.SetValue(ServiceProvider.Settings.DefaultSession, i, null);
                                    }
                                }
                            }
                        }
                        return;
                    }
                    if (arg.StartsWith("property."))
                    {
                        var val = args[1].Trim();
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(CameraProperty).GetProperties());
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
                                prop.PropertyType == typeof(bool))
                            {
                                if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
                                {
                                    if (prop.PropertyType == typeof(string))
                                    {
                                        prop.SetValue(device.LoadProperties(), val, null);
                                    }
                                    if (prop.PropertyType == typeof(bool))
                                    {
                                        prop.SetValue(device.LoadProperties(), val == "true", null);
                                    }
                                    if (prop.PropertyType == typeof(int))
                                    {
                                        int i = 0;
                                        if (int.TryParse(val, out i))
                                            prop.SetValue(device.LoadProperties(), i, null);
                                    }
                                }
                            }
                        }
                        return;
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
                                valp.Value = args[1].Replace("_"," ");
                            }
                        }
                        foreach (PropertyValue<long> property in device.AdvancedProperties)
                        {
                            if (!string.IsNullOrEmpty(property.Name) && property.Value != null && (arg.Split('.')[1].ToLower().Replace("_", " ") == property.Name.ToLower()))
                            {
                                property.Value = args[1].Replace("_", " ");
                            }
                        }
                        return;
                    }
                    throw new Exception("Unknow parameter");
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

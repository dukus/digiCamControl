using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Core.Scripting
{
    public class CommandLineProcessor
    {
        public object Pharse(string[] args)
        {
            var cmd = args[0].ToLower().Trim();
            switch (cmd)
            {
                case "capture":
                    //Task.Factory.StartNew(CameraHelper.Capture);
                    //Thread.Sleep(200);
                    CameraHelper.Capture();
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
                    return (from prop in props where prop.PropertyType == typeof (string) || prop.PropertyType == typeof (int) || prop.PropertyType == typeof (bool) select "session." + prop.Name.ToLower() + "=" + prop.GetValue(ServiceProvider.Settings.DefaultSession, null)).ToList();
                }
                default:
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
                case "session":
                    return ServiceProvider.Settings.DefaultSession.Name;
                case "camera":
                    return ServiceProvider.DeviceManager.SelectedCameraDevice.SerialNumber;
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
                    throw new Exception("Unknow parameter");
            }
        }


        private void Set(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();
            var param = args.Skip(1).ToArray().Aggregate("", (current, s) => current + s+ " ");
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
                        if (cameraDevice.SerialNumber == args[1])
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
                        if (session.Name.ToLower()==args[1].ToLower().Trim())
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
                        IList<PropertyInfo> props = new List<PropertyInfo>(typeof (PhotoSession).GetProperties());
                        foreach (PropertyInfo prop in props)
                        {
                            if (prop.PropertyType == typeof (string) || prop.PropertyType == typeof (int) ||
                                prop.PropertyType == typeof (bool))
                            {
                                if (arg.Split('.')[1].ToLower() == prop.Name.ToLower())
                                {
                                    if (prop.PropertyType == typeof (string))
                                    {
                                        prop.SetValue(ServiceProvider.Settings.DefaultSession, val, null);
                                    }
                                    if (prop.PropertyType == typeof (bool))
                                    {
                                        prop.SetValue(ServiceProvider.Settings.DefaultSession, val == "true", null);
                                    }
                                    if (prop.PropertyType == typeof (int))
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
                    throw new Exception("Unknow parameter");
            }
        }

        private ICameraDevice GetDevice()
        {
            if (ServiceProvider.DeviceManager == null)
                return null;
            return ServiceProvider.DeviceManager.SelectedCameraDevice;
        }
    }
}

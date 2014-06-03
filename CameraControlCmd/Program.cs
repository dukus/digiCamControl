using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControlCmd.Classes;

namespace CameraControlCmd
{
    
    class Program
    {
        //public static bool IsBusy { get; set; }

        private static string _outFilename = null;

        private static InputArguments _arguments;
        
        //[STAThread]
        static int  Main(string[] args)
        {
            Console.WriteLine("digiCamControl command line utility");
            Console.WriteLine();

            _arguments = new InputArguments(args, "/");
            if (!args.Any() || _arguments.Contains("help"))
            {
                ShowHelp();
                Console.ReadLine();
                return 0;
            }
            InitApplication();
            Thread.Sleep(1000);
            while (CamerasAreBusy())
            {
                Thread.Sleep(1);
            }
            if (args != null && args.Count() == 1 && File.Exists(args[0]))
            {
                RunScript(args[0]);
                return 0;
            }
            if (ServiceProvider.DeviceManager.ConnectedDevices.Count == 0)
            {
                Console.WriteLine("No connected device was found ! Exiting");
                return 0;
            }
            int exitCodes= ExecuteArgs();
            Thread.Sleep(250);
            Thread thread = new Thread(WaitForCameras);
            thread.Start();
            
            Dispatcher.Run();

            return exitCodes;
        }

        static void WaitForCameras()
        {
            while (CamerasAreBusy())
            {
                Thread.Sleep(1);
            }
            if (_arguments.Contains("wait"))
            {
                int time = 0;
                int.TryParse(_arguments["wait"], out time);
                if (time > 0)
                {
                    Dispatcher.CurrentDispatcher.Invoke(
                        new Action(() => Console.Write("Waiting {0} milliseconds", time)));
                   
                    Thread.Sleep(time);
                }
                else
                {
                    Dispatcher.CurrentDispatcher.Invoke(new Action(() => Console.Write("Press any key ...")));

                    Console.ReadLine();
                }
            }
            System.Environment.Exit(0);
        }

        static void RunScript(string filename)
        {
            ScriptObject scriptObject = null;
            try
            {
                scriptObject = ServiceProvider.ScriptManager.Load(filename);
                scriptObject.CameraDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Loading error :" + exception.Message);
                return;
            }
            if (ServiceProvider.ScriptManager.Verify(scriptObject))
            {
                ServiceProvider.ScriptManager.Execute(scriptObject);
                while (ServiceProvider.ScriptManager.IsBusy)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                Console.WriteLine("Error in script. Running aborted ! ");
            }
        }

        static int ExecuteArgs()
        {
            try
            {

                if (_arguments.Contains("export"))
                {
                    if (string.IsNullOrEmpty(_arguments["export"]))
                    {
                        Console.WriteLine("No export file is specified");
                    }
                    else
                    {
                        using (StreamWriter writer = File.CreateText(_arguments["export"]))
                        {
                            Console.WriteLine("Exporting properties to: " + _arguments["export"]);
                            CameraPreset preset = new CameraPreset();
                            preset.Get(ServiceProvider.DeviceManager.SelectedCameraDevice);
                            foreach (ValuePair valuePair in preset.Values)
                            {
                                writer.WriteLine("\"{0}\",\"{1}\"", valuePair.Name, valuePair.Value);
                            }
                            writer.Close();
                        }
                    }
                }
                if (_arguments.Contains("session"))
                {
                    PhotoSession session = ServiceProvider.Settings.GetSession(_arguments["session"]);
                    if (session != null)
                    {
                        Console.WriteLine("Using session {0}", _arguments["session"]);
                        ServiceProvider.Settings.DefaultSession = session;
                    }
                    else
                    {
                        Console.WriteLine("Session not found {0}! Using default session", _arguments["session"]);
                    }
                }
                if (_arguments.Contains("preset"))
                {
                    CameraPreset preset = ServiceProvider.Settings.GetPreset(_arguments["preset"]);
                    if (preset != null)
                    {
                        Console.WriteLine("Using preset {0}", _arguments["preset"]);
                        foreach (ICameraDevice cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                        {
                            preset.Set(cameraDevice);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Preset not found {0}!", _arguments["preset"]);
                    }
                }
                if (_arguments.Contains("folder"))
                {
                    if (string.IsNullOrEmpty(_arguments["folder"]))
                    {
                        Console.WriteLine("No folder !!!");
                    }
                    else
                    {
                        ServiceProvider.Settings.DefaultSession.Folder = _arguments["folder"];
                    }
                }
                if (_arguments.Contains("filenametemplate"))
                {
                    if (string.IsNullOrEmpty(_arguments["filenametemplate"]))
                    {
                        Console.WriteLine("Wrong filename !!!");
                    }
                    else
                    {
                        ServiceProvider.Settings.DefaultSession.FileNameTemplate = _arguments["filenametemplate"];
                    }
                }
                if (_arguments.Contains("counter"))
                {
                    int i = 0;
                    string val = _arguments["counter"];
                    if (string.IsNullOrEmpty(_arguments["counter"]) || !int.TryParse(val, out i))
                    {
                        Console.WriteLine("Wrong counter !!!");
                    }
                    else
                    {
                        ServiceProvider.Settings.DefaultSession.Counter = i;
                    }
                }
                if (_arguments.Contains("filename"))
                {
                    _outFilename = _arguments["filename"];
                    //if(string.IsNullOrEmpty(_outFilename))
                    //{
                    //    SaveFileDialog dlg = new SaveFileDialog();
                    //    dlg.Filter = "Jpg file (*.jpg)|*.jpg|All files|*.*";
                    //    if(dlg.ShowDialog()==DialogResult.OK)
                    //    {
                    //        _outFilename = dlg.FileName;
                    //    }
                    //}
                }
                if (_arguments.Contains("iso"))
                {
                    if (string.IsNullOrEmpty(_arguments["iso"]))
                    {
                        Console.WriteLine("No iso number !!!");
                    }
                    else
                    {
                        Thread.Sleep(200);
                        ServiceProvider.DeviceManager.SelectedCameraDevice.IsoNumber.SetValue(_arguments["iso"]);
                    }
                }
                if (_arguments.Contains("aperture"))
                {
                    if (string.IsNullOrEmpty(_arguments["aperture"]))
                    {
                        Console.WriteLine("No aperture number !!!");
                    }
                    else
                    {
                        Thread.Sleep(200);
                        ServiceProvider.DeviceManager.SelectedCameraDevice.FNumber.SetValue("ƒ/" + _arguments["aperture"]);
                    }
                }
                if (_arguments.Contains("shutter"))
                {
                    if (string.IsNullOrEmpty(_arguments["shutter"]))
                    {
                        Console.WriteLine("No shutter number !!!");
                    }
                    else
                    {
                        Thread.Sleep(200);
                        ServiceProvider.DeviceManager.SelectedCameraDevice.ShutterSpeed.SetValue(_arguments["shutter"]);
                    }
                }
                if (_arguments.Contains("ec"))
                {
                    if (string.IsNullOrEmpty(_arguments["ec"]))
                    {
                        Console.WriteLine("No ec number !!!");
                    }
                    else
                    {
                        Thread.Sleep(200);
                        ServiceProvider.DeviceManager.SelectedCameraDevice.ExposureCompensation.SetValue( _arguments["ec"]);
                    }
                }
                
                if (_arguments.Contains("comment"))
                {
                    Thread.Sleep(200);
                    ServiceProvider.DeviceManager.SelectedCameraDevice.SetCameraField(CameraFieldType.Comment, _arguments["comment"]);
                    Console.WriteLine("Comment was set");
                }
                if (_arguments.Contains("artist"))
                {
                    Thread.Sleep(200);
                    ServiceProvider.DeviceManager.SelectedCameraDevice.SetCameraField(CameraFieldType.Artist, _arguments["artist"]);
                    Console.WriteLine("Artist was set");
                }
                if (_arguments.Contains("copyright"))
                {
                    Thread.Sleep(200);
                    ServiceProvider.DeviceManager.SelectedCameraDevice.SetCameraField(CameraFieldType.Copyright, _arguments["copyright"]);
                    Console.WriteLine("Copyright was set");
                }

                if (_arguments.Contains("capture"))
                {
                    new Thread(Capture).Start();
                    Thread.Sleep(200);
                    return 0;
                }

                if (_arguments.Contains("capturenoaf"))
                {
                    try
                    {
                        ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhotoNoAf();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Error occurred while capturing photo " + exception);
                        ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy = false;
                        return 1;
                    }

                    return 0;
                }
                if (_arguments.Contains("captureall"))
                {
                    foreach (ICameraDevice cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                    {
                        ICameraDevice device = cameraDevice;
                        new Thread(device.CapturePhoto).Start();
                    }
                }
                if (_arguments.Contains("captureallnoaf"))
                {
                    foreach (ICameraDevice cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
                    {
                        ICameraDevice device = cameraDevice;
                        new Thread(device.CapturePhotoNoAf).Start();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                Console.WriteLine(exception.Message);
                return 1;
            }
            return 0;
        }

        private static void Capture()
        {
            try
            {
                ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy = true;
                ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam = true;
                // prevent use this mode if the camera not support it 
                 if (ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureInRam))
                    ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureInSdRam = false;
                ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();
            }
            catch (Exception exception)
            {
                ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy = false;
                Console.WriteLine("Error occurred while capturing photo " + exception);
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Arguments :");
            Console.WriteLine(" /help                      - this screen");
            Console.WriteLine(" /export filename           - export current connected camera properties");
            Console.WriteLine(" /capture                   - capture photo");
            Console.WriteLine(" /capturenoaf               - capture photo without autofocus");
            Console.WriteLine(" /captureall                - capture photo with all connected devices");
            Console.WriteLine(" /captureallnoaf            - capture photo without autofocus with all connected devices");
            Console.WriteLine(" /session session_name      - use session [session_name]");
            Console.WriteLine(" /preset preset_name        - use preset [session_name]");
            Console.WriteLine(" /folder path               - set the photo save folder");
            Console.WriteLine(" /filenameTemplate template - set the photo save file name template");
            Console.WriteLine(" /filename fileName         - set the photo save file name");
            Console.WriteLine(" /counter number            - set the photo initial counter");
            Console.WriteLine(" /wait [mseconds]           - after done wait for a keypress/ milliseconds ");
            Console.WriteLine("----------------------------------------------------------------------------------------");
            Console.WriteLine("For single camera usage :");
            Console.WriteLine("----------------------------------------------------------------------------------------");
            Console.WriteLine(" /iso isonumber             - set the iso number ex. 100 200 400");
            Console.WriteLine(" /aperture aperture         - set the aperture number ex. 9,5 8,0");
            Console.WriteLine(" /shutter shutter speed     - set the shutter speed ex. \"1/50\" \"1/250\" 1s 3s");
            Console.WriteLine(" /ec compensation           - set the exposure comp. -1,5 +2");
            Console.WriteLine("----------------------------------------------------------------------------------------");
            Console.WriteLine("For Nikon camera only :");
            Console.WriteLine("----------------------------------------------------------------------------------------");
            Console.WriteLine(" /comment comment           - set in camera comment string");
            Console.WriteLine(" /copyright copyright       - set in camera copyright string");
            Console.WriteLine(" /artist artist             - set in camera artist string");

        }

        private static void InitApplication()
        {
            ServiceProvider.Configure();
            Log.Debug("Command line utility started");
            ServiceProvider.Settings = new Settings();
            ServiceProvider.Settings = ServiceProvider.Settings.Load();
            ServiceProvider.Settings.LoadSessionData();
            ServiceProvider.WindowsManager = new WindowsManager();
            //WIAManager manager = new WIAManager();
            StaticHelper.Instance.PropertyChanged += Instance_PropertyChanged;
            ServiceProvider.DeviceManager.CameraConnected += DeviceManagerCameraConnected;
            ServiceProvider.DeviceManager.ConnectToCamera();
            ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            if (ServiceProvider.DeviceManager.SelectedCameraDevice.AttachedPhotoSession != null)
                ServiceProvider.Settings.DefaultSession = (PhotoSession)
                  ServiceProvider.DeviceManager.SelectedCameraDevice.AttachedPhotoSession;
            foreach (ICameraDevice cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
            {
                cameraDevice.CaptureCompleted += SelectedCameraDevice_CaptureCompleted;
            }
            ServiceProvider.ScriptManager.OutPutMessageReceived += ScriptManager_OutPutMessageReceived;
            //ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureCompleted += SelectedCameraDevice_CaptureCompleted;
        }

        static void ScriptManager_OutPutMessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void SelectedCameraDevice_CaptureCompleted(object sender, EventArgs e)
        {
            //ICameraDevice device = sender as ICameraDevice;
            //device.IsBusy = false; 
        }

        static void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SystemMessage")
            {
                Console.WriteLine(StaticHelper.Instance.SystemMessage);
            }
        }

        static void DeviceManagerCameraConnected(ICameraDevice cameraDevice)
        {
            CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(cameraDevice);
            cameraDevice.DisplayName = property.DeviceName;
            cameraDevice.AttachedPhotoSession = ServiceProvider.Settings.GetSession(property.PhotoSessionName);
        }


        static void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            Thread thread = new Thread(PhotoCaptured);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(eventArgs);
            //thread.Join();
        }


        static void PhotoCaptured(object o)
        {
            PhotoCapturedEventArgs eventArgs = o as PhotoCapturedEventArgs;
            if (eventArgs == null)
                return;

            try
            {
                Console.WriteLine("Photo transfer begin.");

                CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(eventArgs.CameraDevice);
                if ((property.NoDownload && !eventArgs.CameraDevice.CaptureInSdRam))
                {
                    eventArgs.CameraDevice.IsBusy = false;
                    return;
                }
                string fileName = "";
                if (string.IsNullOrEmpty(_outFilename))
                {
                    if (!ServiceProvider.Settings.DefaultSession.UseOriginalFilename ||
                        eventArgs.CameraDevice.CaptureInSdRam)
                    {
                        fileName =
                            ServiceProvider.Settings.DefaultSession.GetNextFileName(
                                Path.GetExtension(eventArgs.FileName),
                                eventArgs.CameraDevice);
                    }
                    else
                    {
                        fileName = Path.Combine(ServiceProvider.Settings.DefaultSession.Folder, eventArgs.FileName);
                        if (File.Exists(fileName))
                            fileName =
                                StaticHelper.GetUniqueFilename(
                                    Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) +
                                    "_", 0,
                                    Path.GetExtension(fileName));
                    }
                }
                else
                {
                    if(File.Exists(_outFilename))
                        File.Delete(_outFilename);
                    fileName = _outFilename;
                }
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                Console.WriteLine("Transfer started :" + fileName);
                eventArgs.CameraDevice.TransferFile(eventArgs.Handle, fileName);
                Console.WriteLine("Transfer done :" + fileName);
                ServiceProvider.Settings.DefaultSession.AddFile(fileName);
                ServiceProvider.Settings.Save(ServiceProvider.Settings.DefaultSession);
                StaticHelper.Instance.SystemMessage = "Photo transfer done.";
                if (ServiceProvider.Settings.PlaySound)
                {
                    PhotoUtils.PlayCaptureSound();
                }
                eventArgs.CameraDevice.IsBusy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Transfer error !\nMessage :" + ex.Message);
                eventArgs.CameraDevice.IsBusy = false;
                Log.Error("Transfer error !", ex);
            }
        }

        private static bool CamerasAreBusy()
        {
            return ServiceProvider.DeviceManager.ConnectedDevices.Aggregate(false, (current, connectedDevice) => connectedDevice.IsBusy || current);
        }

    }
}

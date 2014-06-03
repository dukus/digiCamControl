using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Nikon;

namespace CameraControl.Core.Scripting
{
    public class ScriptObject : BaseFieldClass
    {
        public bool ExitLoop { get; set; }

        public ValuePairEnumerator Variabiles { get; set; }

        private AsyncObservableCollection<IScriptCommand> _commands;
        public AsyncObservableCollection<IScriptCommand> Commands
        {
            get { return _commands; }
            set
            {
                _commands = value;
                NotifyPropertyChanged("Commands");
            }
        }

        private bool _useExternal;
        public bool UseExternal
        {
            get { return _useExternal; }
            set
            {
                _useExternal = value;
                NotifyPropertyChanged("UseExternal");
            }
        }

        private CustomConfig _selectedConfig;
        public CustomConfig SelectedConfig
        {
            get { return _selectedConfig; }
            set
            {
                _selectedConfig = value;
                NotifyPropertyChanged("SelectedConfig");
            }
        }

        private ICameraDevice _cameraDevice;
        public ICameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set
            {
                _cameraDevice = value;
                NotifyPropertyChanged("CameraDevice");
            }
        }

        public ScriptObject()
        {
            Commands = new AsyncObservableCollection<IScriptCommand>();
            Variabiles = new ValuePairEnumerator();
            UseExternal = false;
        }

        public void StartCapture()
        {
            Thread thread = new Thread(StartCaptureThread);
            thread.Start();   
        }

        public void StopCapture()
        {
            Thread thread = new Thread(StopCaptureThread);
            thread.Start();
        }

        private void StopCaptureThread()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    if (UseExternal)
                    {
                        if (SelectedConfig != null)
                        {
                            ServiceProvider.ExternalDeviceManager.CloseShutter(SelectedConfig);
                            NikonBase nikonBase = CameraDevice as NikonBase;
                            if (nikonBase != null)
                                nikonBase.StartEventTimer();
                        }
                        else
                        {
                            StaticHelper.Instance.SystemMessage = TranslationStrings.LabelNoExternalDeviceSelected;
                        }
                    }
                    else
                    {
                        if (CameraDevice.GetCapability(CapabilityEnum.Bulb))
                        {
                            CameraDevice.EndBulbMode();
                        }
                        else
                        {
                            StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                        }
                    }
                    StaticHelper.Instance.SystemMessage = "Capture done";
                    Log.Debug("Bulb capture done");
                }
                catch (DeviceException deviceException)
                {
                    if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY)
                        retry = true;
                    else
                    {
                        StaticHelper.Instance.SystemMessage = deviceException.Message;
                        Log.Error("Bulb done", deviceException);
                    }

                }
                catch (Exception exception)
                {
                    StaticHelper.Instance.SystemMessage = exception.Message;
                    Log.Error("Bulb done", exception);
                }
            } while (retry);
        }

        void StartCaptureThread()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    Log.Debug("Bulb capture started");
                    if (UseExternal)
                    {
                        if (SelectedConfig != null)
                        {
                            NikonBase nikonBase = CameraDevice as NikonBase;
                            if (nikonBase != null)
                                nikonBase.StopEventTimer();
                            ServiceProvider.ExternalDeviceManager.OpenShutter(SelectedConfig);
                        }
                        else
                        {
                            StaticHelper.Instance.SystemMessage = TranslationStrings.LabelNoExternalDeviceSelected;
                        }
                    }
                    else
                    {
                        if (CameraDevice.GetCapability(CapabilityEnum.Bulb))
                        {
                            CameraDevice.LockCamera();
                            CameraDevice.StartBulbMode();
                        }
                        else
                        {
                            StaticHelper.Instance.SystemMessage = TranslationStrings.MsgBulbModeNotSupported;
                        }
                    }
                }
                catch (DeviceException deviceException)
                {
                    if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY)
                        retry = true;
                    else
                    {
                        StaticHelper.Instance.SystemMessage = deviceException.Message;
                        Log.Error("Bulb start", deviceException);
                    }
                }
                catch (Exception exception)
                {
                    StaticHelper.Instance.SystemMessage = exception.Message;
                    Log.Error("Bulb start", exception);
                }
            } while (retry);
        }

        public string ParseString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            StringBuilder output = new StringBuilder(input);
            int offset = 0;

            Regex variablePattern = new Regex(@"\${([^:{}]+)(?::([^}\(]+))?(?:\(([^\)]+)\))?}");
            MatchCollection matches = variablePattern.Matches(input);
            foreach (Match currMatch in matches)
            {
                string varName = "";
                string modifier = string.Empty;
                string value = string.Empty;
                string options = string.Empty;

                // get rid of the escaped variable string
                output.Remove(currMatch.Index + offset, currMatch.Length);

                // grab details for this parse
                varName = currMatch.Groups[1].Value;

                if(varName.StartsWith("session."))
                {
                    IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                    foreach (PropertyInfo prop in props)
                    {
                        //object propValue = prop.GetValue(myObject, null);
                        if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(bool))
                        {
                            if(varName.Split('.')[1].ToLower()==prop.Name.ToLower())
                            {
                                value = prop.GetValue(ServiceProvider.Settings.DefaultSession, null).ToString();
                            }
                        }
                        // Do something with propValue
                    }
                } if(varName.StartsWith("camera."))
                {
                    if(ServiceProvider.DeviceManager.SelectedCameraDevice!=null)
                    {
                        CameraPreset preset = new CameraPreset();
                        preset.Get(ServiceProvider.DeviceManager.SelectedCameraDevice);
                        foreach (ValuePair pair in preset.Values)
                        {
                            if (varName.Split('.')[1].ToLower() == pair.Name.Replace(" ","").ToLower())
                                value = pair.Value;
                        }
                    }
                }
                else
                {
                    value = Variabiles[varName];    
                }
                

                if (currMatch.Groups.Count >= 3)
                    modifier = currMatch.Groups[2].Value.ToLower();
                if (currMatch.Groups.Count >= 4)
                    options = currMatch.Groups[3].Value;

                // if there is no variable for what was passed in we are done
                if (string.IsNullOrEmpty(value))
                {
                    offset -= currMatch.Length;
                    continue;
                }

                //// handle any modifiers
                //if (!modifier.IsNullOrWhiteSpace())
                //{
                //    IValueModifier handler = Load(modifier);
                //    if (handler != null)
                //    {
                //        value = handler.Parse(this.Context, value, options);
                //    }
                //}

                output.Insert(currMatch.Index + offset, value);
                offset = offset - currMatch.Length + value.Length;
            }

            // if we did some replacements search again to check for embedded variables
            if (matches.Count > 0)
                return ParseString(output.ToString());
            else return output.ToString();
        }

        public void ExecuteCommands(AsyncObservableCollection<IScriptCommand> commands)
        {
            ExitLoop = false;
            foreach (IScriptCommand command in commands)
            {
                try
                {
                    command.Execute(this);
                    if (ServiceProvider.ScriptManager.ShouldStop)
                        break;
                    if (ExitLoop)
                        break;
                }
                catch (Exception exception)
                {
                    ServiceProvider.ScriptManager.OutPut("Error executing script command " + command.DisplayName +
                                                         " Exception:" + exception.Message);
                    Log.Debug("Error executing commands",exception);
                }
            }
        }
    }
}

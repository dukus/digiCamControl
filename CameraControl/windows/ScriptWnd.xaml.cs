using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.Win32;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for ScriptWnd.xaml
    /// </summary>
    public partial class ScriptWnd : IWindow, IToolPlugin
    {
        public string ScriptFileName { get; set; }

        public ScriptWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            NewScript();
        }

        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "<")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (IScriptCommand command in ServiceProvider.ScriptManager.AvaiableCommands)
                {
                    data.Add(new MyCompletionData(command.DefaultValue, command.Description, command.Name.ToLower()));
                }
                completionWindow.Show();
                completionWindow.Closed += delegate
                                               {
                                                   completionWindow = null;
                                               };
            }
            if (e.Text == ".")
            {
                string word = textEditor.GetWordBeforeDot();
                if (word == "{session" || word == "session")
                {
                    IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    // provide AvalonEdit with the data:
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (PropertyInfo prop in props)
                    {
                        //object propValue = prop.GetValue(myObject, null);
                        if(prop.PropertyType==typeof(string) || prop.PropertyType==typeof(int) || prop.PropertyType==typeof(bool))
                        {
                            data.Add(new MyCompletionData(prop.Name.ToLower(), "", prop.Name.ToLower()));
                        }
                        // Do something with propValue
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate
                    {
                        completionWindow = null;
                    };
                }
                if (word == "{camera" && ServiceProvider.DeviceManager.SelectedCameraDevice != null)
                {
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                    CameraPreset preset = new CameraPreset();
                    preset.Get(ServiceProvider.DeviceManager.SelectedCameraDevice);
                    foreach (ValuePair value in preset.Values)
                    {
                        data.Add(new MyCompletionData(value.Name.Replace(" ", "").ToLower(), "Current value :" + value.Value, value.Name.Replace(" ", "").ToLower()));
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate
                                                   {
                                                       completionWindow = null;
                                                   };
                }
            }
            if (e.Text == " ")
            {
                string line = textEditor.GetLine();
                
                if (line.StartsWith("setcamera"))
                {
                    if (!line.Contains("property") && !line.Contains("value"))
                    {
                        completionWindow = new CompletionWindow(textEditor.TextArea);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new MyCompletionData("property", "", "property"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                                                       {
                                                           completionWindow = null;
                                                       };
                    }
                    if (line.Contains("property") && !line.Contains("value"))
                    {
                        completionWindow = new CompletionWindow(textEditor.TextArea);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new MyCompletionData("value", "", "value"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }
                }
            }


            if (e.Text == "=" && ServiceProvider.DeviceManager.SelectedCameraDevice != null)
            {
                string line = textEditor.GetLine();
                string word = textEditor.GetWordBeforeDot();
                if (line.StartsWith("setcamera"))
                {
                    if (word == "property")
                    {
                        completionWindow = new CompletionWindow(textEditor.TextArea);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new MyCompletionData("\"" + "aperture" + "\"", "", "aperture"));
                        data.Add(new MyCompletionData("\"" + "iso" + "\"", "", "iso"));
                        data.Add(new MyCompletionData("\"" + "shutter" + "\"", "", "shutter"));
                        data.Add(new MyCompletionData("\"" + "ec" + "\"", "Exposure Compensation", "ec"));
                        data.Add(new MyCompletionData("\"" + "wb" + "\"", "White Balance", "wb"));
                        data.Add(new MyCompletionData("\"" + "cs" + "\"", "Compression Setting", "cs"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }
                    if (word == "value")
                    {

                        if (line.Contains("property=\"aperture\"") &&
                            ServiceProvider.DeviceManager.SelectedCameraDevice.FNumber != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (string value in ServiceProvider.DeviceManager.SelectedCameraDevice.FNumber.Values)
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate
                                                           {
                                                               completionWindow = null;
                                                           };
                        }
                        if (line.Contains("property=\"iso\"") &&
                            ServiceProvider.DeviceManager.SelectedCameraDevice.IsoNumber != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (string value in ServiceProvider.DeviceManager.SelectedCameraDevice.IsoNumber.Values
                                )
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate
                                                           {
                                                               completionWindow = null;
                                                           };
                        }
                        if (line.Contains("property=\"shutter\"") &&
                            ServiceProvider.DeviceManager.SelectedCameraDevice.ShutterSpeed != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (
                                string value in ServiceProvider.DeviceManager.SelectedCameraDevice.ShutterSpeed.Values)
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate
                                                           {
                                                               completionWindow = null;
                                                           };
                        }
                        if (line.Contains("property=\"ec\"") &&
                            ServiceProvider.DeviceManager.SelectedCameraDevice.ExposureCompensation != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (
                                string value in
                                    ServiceProvider.DeviceManager.SelectedCameraDevice.ExposureCompensation.Values)
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate
                                                           {
                                                               completionWindow = null;
                                                           };
                        }
                        if (line.Contains("property=\"wb\"") &&
                             ServiceProvider.DeviceManager.SelectedCameraDevice.WhiteBalance != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (
                                string value in
                                    ServiceProvider.DeviceManager.SelectedCameraDevice.WhiteBalance.Values)
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate
                                                           {
                                                               completionWindow = null;
                                                           };
                        }
                        if (line.Contains("property=\"cs\"") &&
                                ServiceProvider.DeviceManager.SelectedCameraDevice.CompressionSetting != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (
                                string value in
                                    ServiceProvider.DeviceManager.SelectedCameraDevice.CompressionSetting.Values)
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate
                            {
                                completionWindow = null;
                            };
                        }
                    }
                }
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }
        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.ScriptWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        Show();
                        Activate();
                        Topmost = true;
                        ServiceProvider.ScriptManager.OutPutMessageReceived += ScriptManager_OutPutMessageReceived;
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.ScriptWnd_Hide:
                    ServiceProvider.ScriptManager.OutPutMessageReceived -= ScriptManager_OutPutMessageReceived;
                    Hide();
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        ServiceProvider.ScriptManager.OutPutMessageReceived -= ScriptManager_OutPutMessageReceived;
                        Hide();
                        Close();
                    }));
                    break;
            }
        }

        void ScriptManager_OutPutMessageReceived(object sender, MessageEventArgs e)
        {
            AddOutput(e.Message);
        }

        #endregion

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_Hide);
            }
        }

        #region Implementation of IToolPlugin

        public bool Execute()
        {
            ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_Show);
            return true;
        }

        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Script file(*.dccscript)|*.dccscript|All files|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    ScriptFileName = dlg.FileName;
                    LoadScriptFile();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error loading script file" + exception.Message);
                    Log.Error("Error loading script file", exception);
                }
            }
        }

        public void LoadScriptFile()
        {
            textEditor.Load(ScriptFileName);
        }

        public void SaveScriptFile()
        {
            textEditor.Save(ScriptFileName);
        }

        private void mnu_save_as_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Script file(*.dccscript)|*.dccscript|All files|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    ScriptFileName = dlg.FileName;
                    SaveScriptFile();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error saving script file" + exception.Message);
                    Log.Error("Error saving script file", exception);
                }
            }
        }

        private void mnu_save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ScriptFileName) || !File.Exists(ScriptFileName))
                mnu_save_as_Click(null, null);
            else
                SaveScriptFile();
        }

        private void mnu_verify_Click(object sender, RoutedEventArgs e)
        {
            mnu_save_Click(null,null);
            ScriptObject scriptObject = null;
            try
            {
                lst_output.Items.Clear();
                scriptObject = ServiceProvider.ScriptManager.Load(ScriptFileName);
            }
            catch (Exception exception)
            {
                AddOutput("Loading error :" + exception.Message);
            }
            AddOutput(ServiceProvider.ScriptManager.Verify(scriptObject) ? "Verification done " : "Verification fail ");
        }

        private void NewScript()
        {
            textEditor.Text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                  "<dccscript> \n" +
                  "   <commands>\n" +
                  "     \n" +
                  "   </commands>\n" +
                  "</dccscript>";
            ScriptFileName = null;
        }

        public void AddOutput(string msg)
        {
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 lst_output.Items.Add(msg);
                                                 lst_output.ScrollIntoView(lst_output.Items[lst_output.Items.Count - 1]);
                                             }));
        }

        private void mnu_run_Click(object sender, RoutedEventArgs e)
        {
            mnu_save_Click(null, null);
            ScriptObject scriptObject = null;
            try
            {
                lst_output.Items.Clear();
                scriptObject = ServiceProvider.ScriptManager.Load(ScriptFileName);
                scriptObject.CameraDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
            }
            catch (Exception exception)
            {
                AddOutput("Loading error :" + exception.Message);
                return;
            }
            if (ServiceProvider.ScriptManager.Verify(scriptObject))
            {
                ServiceProvider.ScriptManager.Execute(scriptObject);
            }
            else
            {
                AddOutput("Error in script. Running aborted ! ");
            }
        }

        private void mnu_stop_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.ScriptManager.Stop();
        }

    }
}

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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CameraControl.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Scripting;
using CameraControl.Core.TclScripting;
using CameraControl.Core.Wpf;
using CameraControl.Devices;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.Win32;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for ScriptWnd.xaml
    /// </summary>
    public partial class ScriptWnd : IWindow, IToolPlugin
    {
        private readonly TclScripManager _manager = new TclScripManager();

        public string Id
        {
            get { return "{04F1DD8E-3E4E-497D-80A9-125ABC76DA7E}"; }
        }

        public string ScriptFileName { get; set; }

        public ScriptWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            NewScript();
        }

        private CompletionWindow completionWindow;

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "<")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                ServiceProvider.Settings.ApplyTheme(completionWindow);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (IScriptCommand command in ServiceProvider.ScriptManager.AvaiableCommands)
                {
                    data.Add(new MyCompletionData(command.DefaultValue, command.Description, command.Name.ToLower()));
                }
                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
            }
            if (e.Text == ".")
            {
                string word = textEditor.GetWordBeforeDot();
                if (word == "{session" || word == "session")
                {
                    IList<PropertyInfo> props = new List<PropertyInfo>(typeof (PhotoSession).GetProperties());
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                    // provide AvalonEdit with the data:
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (PropertyInfo prop in props)
                    {
                        //object propValue = prop.GetValue(myObject, null);
                        if (prop.PropertyType == typeof (string) || prop.PropertyType == typeof (int) ||
                            prop.PropertyType == typeof (bool))
                        {
                            data.Add(new MyCompletionData(prop.Name.ToLower(), "", prop.Name.ToLower()));
                        }
                        // Do something with propValue
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate { completionWindow = null; };
                }
                if (word == "{camera" && ServiceProvider.DeviceManager.SelectedCameraDevice != null)
                {
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                    CameraPreset preset = new CameraPreset();
                    preset.Get(ServiceProvider.DeviceManager.SelectedCameraDevice);
                    foreach (ValuePair value in preset.Values)
                    {
                        data.Add(new MyCompletionData(value.Name.Replace(" ", "").ToLower(),
                            "Current value :" + value.Value,
                            value.Name.Replace(" ", "").ToLower()));
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate { completionWindow = null; };
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
                        completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new MyCompletionData("property", "", "property"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate { completionWindow = null; };
                    }
                    if (line.Contains("property") && !line.Contains("value"))
                    {
                        completionWindow = new CompletionWindow(textEditor.TextArea);
                        completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new MyCompletionData("value", "", "value"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate { completionWindow = null; };
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
                        completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new MyCompletionData("\"" + "aperture" + "\"", "", "aperture"));
                        data.Add(new MyCompletionData("\"" + "iso" + "\"", "", "iso"));
                        data.Add(new MyCompletionData("\"" + "shutter" + "\"", "", "shutter"));
                        data.Add(new MyCompletionData("\"" + "ec" + "\"", "Exposure Compensation", "ec"));
                        data.Add(new MyCompletionData("\"" + "wb" + "\"", "White Balance", "wb"));
                        data.Add(new MyCompletionData("\"" + "cs" + "\"", "Compression Setting", "cs"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate { completionWindow = null; };
                    }
                    if (word == "value")
                    {
                        if (line.Contains("property=\"aperture\"") &&
                            ServiceProvider.DeviceManager.SelectedCameraDevice.FNumber != null)
                        {
                            completionWindow = new CompletionWindow(textEditor.TextArea);
                            completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
                            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                            foreach (string value in ServiceProvider.DeviceManager.SelectedCameraDevice.FNumber.Values)
                            {
                                data.Add(new MyCompletionData("\"" + value + "\"", value, value));
                            }
                            completionWindow.Show();
                            completionWindow.Closed += delegate { completionWindow = null; };
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
                            completionWindow.Closed += delegate { completionWindow = null; };
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
                            completionWindow.Closed += delegate { completionWindow = null; };
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
                            completionWindow.Closed += delegate { completionWindow = null; };
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
                            completionWindow.Closed += delegate { completionWindow = null; };
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
                            completionWindow.Closed += delegate { completionWindow = null; };
                        }
                    }
                }
            }
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
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
                        Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                        Show();
                        Activate();
                        ServiceProvider.ScriptManager.OutPutMessageReceived +=
                            ScriptManager_OutPutMessageReceived;
                        _manager.Output += manager_Output;
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.ScriptWnd_Hide:
                    ServiceProvider.ScriptManager.OutPutMessageReceived -= ScriptManager_OutPutMessageReceived;
                    _manager.Output -= manager_Output;
                    Hide();
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        ServiceProvider.ScriptManager.OutPutMessageReceived -=
                            ScriptManager_OutPutMessageReceived;
                        Hide();
                        Close();
                    }));
                    break;
            }
        }

        private void ScriptManager_OutPutMessageReceived(object sender, MessageEventArgs e)
        {
            AddOutput(e.Message);
        }

        #endregion

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
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
            dlg.Filter = "Tcl Script file(*.tcl)|*.tcl|Script file(*.dccscript)|*.dccscript|All files|*.*";
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
            if (Path.GetExtension(ScriptFileName) == ".tcl")
            {
                textEditorTcl.Load(ScriptFileName);
                TabControl.SelectedItem = TclTabItem;
            }
            else
            {
                textEditor.Load(ScriptFileName);
                TabControl.SelectedItem = XmTabItem;
            }
        }

        public void SaveScriptFile()
        {
            if (IsXmlActive())
                textEditor.Save(ScriptFileName);
            else
                textEditorTcl.Save(ScriptFileName);
        }

        private void mnu_save_as_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = IsXmlActive()
                ? "Script file(*.dccscript)|*.dccscript|All files|*.*"
                : "Tcl Script file(*.tcl)|*.tcl|All files|*.*";
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
            if (!IsXmlActive())
                return;

            mnu_save_Click(null, null);
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
            if (IsXmlActive())
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
            else
            {
                try
                {
                    lst_outputTcl.Items.Clear();
                    _manager.Execute(textEditorTcl.Text);
                }
                catch (Exception exception)
                {
                    AddOutput("Error in script. Running aborted ! " + exception.Message);
                }
            }
        }

        private bool IsXmlActive()
        {
            return TabControl.SelectedItem == XmTabItem;
        }

        private void manager_Output(string message, bool newline)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                lst_outputTcl.Items.Add(message);
                lst_outputTcl.SelectedItem = message;
                lst_outputTcl.ScrollIntoView(message);
            }));
        }

        private void mnu_stop_Click(object sender, RoutedEventArgs e)
        {
            if (IsXmlActive())
                ServiceProvider.ScriptManager.Stop();
            else
                _manager.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var processor = new CommandLineProcessor();
                var resp = processor.Pharse(TextBoxCmd.Text.Split(' '));
                var list = resp as IEnumerable<string>;
                if (list != null)
                {
                    TextBlockError.Text = "";
                    foreach (var o in list)
                    {
                        TextBlockError.Text += o + "\n";
                    }
                }
                else
                {
                    if (resp != null)
                        TextBlockError.Text = resp.ToString();
                }
                lst_cmd.Items.Add(TextBoxCmd.Text);
            }
            catch (Exception ex)
            {
                TextBlockError.Text = ex.Message;
            }
        }

        private void lst_cmd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_cmd.SelectedItem != null)
                TextBoxCmd.Text = lst_cmd.SelectedItem.ToString();
        }

        private void TextBoxCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(null, null);

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PhotoUtils.Run("http://digicamcontrol.com/wiki/index.php/Single_Command_System");
        }
    }
}
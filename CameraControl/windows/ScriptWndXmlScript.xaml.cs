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

using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Scripting;
using CameraControl.Core.Wpf;
using CameraControl.Devices;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ScriptWndXmlScript.xaml
	/// </summary>
	public partial class ScriptWndXmlScript : UserControl, ScriptWnd.IScriptControl, INotifyPropertyChanged
	{
		public ScriptWndXmlScript ()
		{
			DataContext = this;
			InitializeComponent();
			textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
			textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
			NewScript();
			if (ServiceProvider.ScriptManager != null)
			{
				IsRunning = ServiceProvider.ScriptManager.IsBusy;
				ServiceProvider.ScriptManager.PropertyChanged += ScriptManager_PropertyChanged;
				ServiceProvider.ScriptManager.OutPutMessageReceived += ScriptManager_OutPutMessageReceived;
			}
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion

		private bool _isRunning;
		public bool IsRunning
		{
			get => _isRunning;
			set
			{
				if (_isRunning != value)
				{
					_isRunning = value;
					NotifyPropertyChanged(nameof(IsRunning));
					NotifyPropertyChanged(nameof(IsNotRunning));
					NotifyPropertyChanged(nameof(FullScriptTitle));
				}
			}
		}

		public bool IsNotRunning
			=> !IsRunning;

		private string _scriptFileName;
		private bool _scriptSaved;
		private bool ScriptSaved
		{
			get => _scriptSaved;
			set
			{
				if (_scriptSaved != value)
				{
					_scriptSaved = value;
					NotifyPropertyChanged(nameof(FullScriptTitle));
				}
			}
		}

		public string FullScriptTitle
		{
			get
			{
				var result = ScriptTitle;
				if (IsRunning)
				{
					result += " (running)";
				}
				if (!ScriptSaved)
				{
					result += "*";
				}
				if (_scriptFileName != null)
				{
					result += $" [{Path.GetFileName(_scriptFileName)}]";
				}
				return result;
			}
		}

		public string ScriptTitle
			=> "Xml script";

		bool ScriptWnd.IScriptControl.SupportsFile => true;
		bool ScriptWnd.IScriptControl.SupportsVerify => true;

		void ScriptWnd.IScriptControl.FileNew ()
		{
			NewScript();
		}

		void ScriptWnd.IScriptControl.FileOpen ()
		{
			OpenFileDialog dlg = new OpenFileDialog()
			{
				Filter = "Script file (*.dccscript)|*.dccscript|All files|*.*"
			};
			if (dlg.ShowDialog() == true)
			{
				try
				{
					LoadScriptFile(dlg.FileName);
				}
				catch (Exception exception)
				{
					MessageBox.Show("Error loading script file" + exception.Message);
					Log.Error("Error loading script file", exception);
				}
			}
		}

		void ScriptWnd.IScriptControl.FileSaveAs ()
		{
			SaveFileDialog dlg = new SaveFileDialog
			{
				Filter = "Script file (*.dccscript)|*.dccscript|All files|*.*",
				FileName = _scriptFileName
			};
			if (dlg.ShowDialog() == true)
			{
				try
				{
					SaveScriptFile(dlg.FileName);
				}
				catch (Exception exception)
				{
					MessageBox.Show("Error saving script file" + exception.Message);
					Log.Error("Error saving script file", exception);
				}
			}
			else
			{
				ScriptSaved = false;
			}
		}

		void ScriptWnd.IScriptControl.FileSave ()
		{
			if (string.IsNullOrEmpty(_scriptFileName) || !File.Exists(_scriptFileName))
			{
				(this as ScriptWnd.IScriptControl).FileSaveAs();
			}
			else
				SaveScriptFile(_scriptFileName);
		}

		void ScriptWnd.IScriptControl.Verify ()
		{
			(this as ScriptWnd.IScriptControl).FileSave();
			lst_output.Items.Clear();
			if (!ScriptSaved)
			{
				AddOutput("Script not saved. Verification aborted! ");
				return;
			}
			ScriptObject scriptObject = null;
			try
			{
				scriptObject = ServiceProvider.ScriptManager.Load(_scriptFileName);
			}
			catch (Exception exception)
			{
				AddOutput("Loading error :" + exception.Message);
			}
			AddOutput(ServiceProvider.ScriptManager.Verify(scriptObject) ? "Verification done " : "Verification failed ");
		}

		void ScriptWnd.IScriptControl.Run ()
		{
			(this as ScriptWnd.IScriptControl).FileSave();
			lst_output.Items.Clear();
			if (!ScriptSaved)
			{
				AddOutput("Script not saved. Running aborted! ");
				return;
			}

			ScriptObject scriptObject = null;
			try
			{
				scriptObject = ServiceProvider.ScriptManager.Load(_scriptFileName);
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
				AddOutput("Error in script. Running aborted! ");
			}
		}

		void ScriptWnd.IScriptControl.Stop ()
		{
			ServiceProvider.ScriptManager.Stop();
		}

		void ScriptWnd.IScriptControl.Dispose ()
		{
			if (ServiceProvider.ScriptManager != null)
			{
				ServiceProvider.ScriptManager.PropertyChanged -= ScriptManager_PropertyChanged;
				ServiceProvider.ScriptManager.OutPutMessageReceived -= ScriptManager_OutPutMessageReceived;
			}
		}

		private void ScriptManager_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ServiceProvider.ScriptManager.IsBusy))
			{
				IsRunning = ServiceProvider.ScriptManager.IsBusy;
			}
		}

		private void ScriptManager_OutPutMessageReceived (object sender, MessageEventArgs e)
		{
			AddOutput(e.Message);
		}

		private CompletionWindow completionWindow;

		private void textEditor_TextArea_TextEntered (object sender, TextCompositionEventArgs e)
		{
			if (e.Text == "<")
			{
				// open code completion after the user has pressed dot:
				completionWindow = new CompletionWindow(textEditor.TextArea);
				completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
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
					IList<PropertyInfo> props = new List<PropertyInfo>(typeof(PhotoSession).GetProperties());
					completionWindow = new CompletionWindow(textEditor.TextArea);
					completionWindow.CompletionList.ListBox.Foreground = new SolidColorBrush(Colors.Black);
					// provide AvalonEdit with the data:
					IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
					foreach (PropertyInfo prop in props)
					{
						//object propValue = prop.GetValue(myObject, null);
						if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) ||
							prop.PropertyType == typeof(bool))
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

		private void textEditor_TextArea_TextEntering (object sender, TextCompositionEventArgs e)
		{
			ScriptSaved = false;
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

		private void NewScript ()
		{
			textEditor.Text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
							  "<dccscript> \n" +
							  "   <commands>\n" +
							  "     \n" +
							  "   </commands>\n" +
							  "</dccscript>";
			_scriptFileName = null;
			ScriptSaved = false;
		}

		private void LoadScriptFile (string scriptFileName)
		{
			ScriptSaved = false;
			_scriptFileName = scriptFileName;
			textEditor.Load(_scriptFileName);
			ScriptSaved = true;
			lst_output.Items.Clear();
		}

		private void SaveScriptFile (string scriptFileName)
		{
			if (scriptFileName != null)
			{
				_scriptFileName = scriptFileName;
			}
			ScriptSaved = false;
			if (_scriptFileName != null)
			{
				textEditor.Save(_scriptFileName);
				ScriptSaved = true;
			}
		}

		public void AddOutput (string msg)
		{
			Dispatcher.BeginInvoke(new Action(delegate
			{
				lst_output.Items.Add(msg);
				lst_output.ScrollIntoView(lst_output.Items[lst_output.Items.Count - 1]);
			}));
		}
	}
}

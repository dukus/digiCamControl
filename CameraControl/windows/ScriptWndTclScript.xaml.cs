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

using CameraControl.Core.TclScripting;
using CameraControl.Devices;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ScriptWndTclScript.xaml
	/// </summary>
	public partial class ScriptWndTclScript : UserControl, ScriptWnd.IScriptControl, INotifyPropertyChanged
	{
		public ScriptWndTclScript ()
			: this($"Tcl script")
		{
		}

		public ScriptWndTclScript (int index)
			: this($"Tcl script #{index}")
		{
		}

		private ScriptWndTclScript (string scriptTitle)
		{
			DataContext = this;
			_scriptTitle = scriptTitle;
			InitializeComponent();
			textEditor.TextArea.TextInput += (s, e) => ScriptSaved = _savedScript == textEditor.Text;
			textEditor.TextArea.KeyUp += (s, e) => ScriptSaved = _savedScript == textEditor.Text;
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
				}
			}
		}

		public bool IsNotRunning
			=> !IsRunning;

		private readonly string _scriptTitle;

		string ScriptWnd.IScriptControl.ScriptTitle
		{
			get
			{
				var result = _scriptTitle;
				if (ScriptFileName != null)
				{
					result += $" [{Path.GetFileName(ScriptFileName)}]";
				}
				return result;
			}
		}

		public string FullScriptTitle
		{
			get
			{
				var result = _scriptTitle;
				if (IsRunning)
				{
					result += " (running)";
				}
				if (!ScriptSaved)
				{
					result += "*";
				}
				if (ScriptFileName != null)
				{
					result += $" [{Path.GetFileName(ScriptFileName)}]";
				}
				return result;
			}
		}

		private string _scriptFileName;
		private string ScriptFileName
		{
			get => _scriptFileName;
			set
			{
				if (_scriptFileName != value)
				{
					_scriptFileName = value;
					NotifyPropertyChanged(nameof(ScriptWnd.IScriptControl.ScriptTitle));
					NotifyPropertyChanged(nameof(FullScriptTitle));
				}
			}
		}

		private bool _scriptSaved = true;
		private string _savedScript = "";
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

		bool ScriptWnd.IScriptControl.SupportsFile => true;
		bool ScriptWnd.IScriptControl.SupportsVerify => false;

		void ScriptWnd.IScriptControl.FileNew ()
		{
			NewScript();
		}

		void ScriptWnd.IScriptControl.FileOpen ()
		{
			OpenFileDialog dlg = new OpenFileDialog()
			{
				Filter = "Tcl script file(*.tcl)|*.tcl|All files|*.*"
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
				Filter = "Tcl script file (*.tcl)|*.tcl|All files|*.*",
				FileName = ScriptFileName
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
		}

		void ScriptWnd.IScriptControl.FileSave ()
		{
			if (string.IsNullOrEmpty(ScriptFileName) || !File.Exists(ScriptFileName))
			{
				(this as ScriptWnd.IScriptControl).FileSaveAs();
			}
			else
				SaveScriptFile(ScriptFileName);
		}

		void ScriptWnd.IScriptControl.Verify ()
		{
		}

		void ScriptWnd.IScriptControl.Run ()
		{
			lst_output.Items.Clear();

			try
			{
				EnsureManager();
				_manager.Execute(textEditor.Text);
			}
			catch (Exception exception)
			{
				AddOutput("Error in script. Running aborted! " + exception.Message);
				DisposeOfManager();
			}
		}

		void ScriptWnd.IScriptControl.Stop ()
		{
			if (_manager != null)
			{
				try
				{
					_manager.Stop();
				}
				catch (Exception exception)
				{
					AddOutput("Error while stopping: " + exception.Message);
					DisposeOfManager();
				}
			}
		}

		void ScriptWnd.IScriptControl.Dispose ()
		{
			DisposeOfManager();
		}

		private void manager_Output (string message, bool newline)
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				Dispatcher.Invoke(new Action(delegate
				{
					lst_output.Items.Add(message);
					lst_output.SelectedItem = message;
					lst_output.ScrollIntoView(message);
				}));
			}
		}

		private void manager_Error (string message, bool newline)
		{
			manager_Output("Error: " + message, newline);
		}

		private void manager_IsBusyChanged (object sender, EventArgs e)
		{
			if (_manager is null || !_manager.IsBusy)
			{
				IsRunning = false;
				DisposeOfManager();
			}
			else
			{
				IsRunning = true;
			}
		}

		private void NewScript ()
		{
			textEditor.Text = "";
			ScriptFileName = null;
			_savedScript = "";
			ScriptSaved = false;
		}

		private void LoadScriptFile (string scriptFileName)
		{
			ScriptFileName = scriptFileName;
			textEditor.Load(ScriptFileName);
			_savedScript = textEditor.Text;
			ScriptSaved = true;
		}

		private void SaveScriptFile (string scriptFileName)
		{
			if (scriptFileName != null)
			{
				ScriptFileName = scriptFileName;
			}
			if (ScriptFileName != null)
			{
				textEditor.Save(ScriptFileName);
				_savedScript = textEditor.Text;
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

		private TclScriptManager _manager;

		private void EnsureManager ()
		{
			if (_manager == null)
			{
				_manager = new TclScriptManager();
				_manager.Output += manager_Output;
				_manager.Error += manager_Error;
				_manager.IsBusyChanged += manager_IsBusyChanged;
			}
		}

		private void DisposeOfManager ()
		{
			if (_manager != null)
			{
				// The manager/interpreter cannot always recover if something goes wrong
				// with the execution of the TCL script.
				_manager.IsBusyChanged -= manager_IsBusyChanged;
				_manager.Stop();
				_manager.Output -= manager_Output;
				_manager.Error -= manager_Error;
				_manager = null;
				manager_IsBusyChanged(this, EventArgs.Empty);
			}
		}

	}
}

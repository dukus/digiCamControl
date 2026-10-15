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
using CameraControl.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ScriptWnd.xaml
	/// </summary>
	public partial class ScriptWnd : IWindow, IToolPlugin, INotifyPropertyChanged
	{

		public string Id
		{
			get { return "{04F1DD8E-3E4E-497D-80A9-125ABC76DA7E}"; }
		}

		public void Init ()
		{

		}

		public ScriptWnd ()
		{
			DataContext = this;
			InitializeComponent();
			_ = new Script(this, mnu_singlecommand, tab_singlecommand, ctrl_singlecommand);
			_ = new Script(this, mnu_xmlscript, tab_xmlscript, ctrl_xmlscript);
		}

		#region Implementation of IWindow

		public void ExecuteCommand (string cmd, object param)
		{
			switch (cmd)
			{
				case WindowsCmdConsts.ScriptWnd_Show:
					Dispatcher.Invoke(new Action(delegate
					{
						Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
						Show();
						Activate();
						Focus();
					}));
					break;
				case WindowsCmdConsts.ScriptWnd_Hide:
					Hide();
					break;
				case CmdConsts.All_Close:
					Dispatcher.Invoke(new Action(delegate
					{
						Hide();
						foreach (var script in _scripts)
						{
							script.Control?.Dispose();
						}
						_scripts.Clear();
						Close();
					}));
					break;
			}
		}
		#endregion

		private void MetroWindow_Closing (object sender, CancelEventArgs e)
		{
			if (IsVisible)
			{
				e.Cancel = true;
				ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_Hide);
			}
		}

		#region Implementation of IToolPlugin

		public bool Execute ()
		{
			ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ScriptWnd_Show);
			return true;
		}

		#endregion

		#region Implementation of INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion

		private int _numRunning;
		public string WindowTitle
		{
			get
			{
				var result = "Script execution";
				if (_numRunning > 0)
				{
					result += $" ({_numRunning} running)";
				}
				return result;
			}
		}

		private List<Script> _scripts = new List<Script>();
		private Script _selectedScript;

		public bool ScriptCanClose
			=> (_selectedScript?.SupportsMultiple ?? false) && ScriptIsNotRunning;
		public bool ScriptSupportsFile
			=> _selectedScript?.SupportsFile ?? false;
		public bool ScriptCanVerify
			=> (_selectedScript?.SupportsVerify ?? false) && ScriptIsNotRunning;
		public bool ScriptIsNotRunning
			=> _selectedScript?.IsNotRunning ?? false;
		public bool ScriptIsRunning
			=> _selectedScript?.IsRunning ?? false;
		public bool AnyScriptIsRunning
			=> _numRunning > 0;

		private void mnu_select_script (Script script)
		{
			if (script == null)
			{
				script = (tabs.SelectedItem as TabItem)?.Tag as Script;
				if (script == null)
				{
					lock (_scripts)
					{
						script = (from s in _scripts
								  where s.IsRunning
								  select s).FirstOrDefault()
								  ??
								  (_scripts.Count > 0 ? _scripts[0] : null);
					}
				}
			}
			if (_selectedScript != script)
			{
				if (_selectedScript != null)
				{
					_selectedScript.Menu.IsChecked = false;
				}
				_selectedScript = script;
				if (_selectedScript != null)
				{
					_selectedScript.Menu.IsChecked = true;
					_selectedScript.Tab.IsSelected = true;
				}

				NotifyPropertyChanged(nameof(ScriptCanClose));
				NotifyPropertyChanged(nameof(ScriptSupportsFile));
				NotifyPropertyChanged(nameof(ScriptCanVerify));
				NotifyPropertyChanged(nameof(ScriptIsNotRunning));
				NotifyPropertyChanged(nameof(ScriptIsRunning));
			}
		}

		private void script_StateChanged (Script sender)
		{
			if (_selectedScript == sender)
			{
				NotifyPropertyChanged(nameof(ScriptCanClose));
				NotifyPropertyChanged(nameof(ScriptCanVerify));
				NotifyPropertyChanged(nameof(ScriptIsNotRunning));
				NotifyPropertyChanged(nameof(ScriptIsRunning));
			}
			var running = 0;
			lock (_scripts)
			{
				foreach (var script in _scripts)
				{
					if (script.IsRunning)
					{
						running++;
					}
				}
			}
			if (running != _numRunning)
			{
				_numRunning = running;
				NotifyPropertyChanged(nameof(WindowTitle));
				NotifyPropertyChanged(nameof(AnyScriptIsRunning));
			}
		}

		private void mnu_new_tclscript_Click (object sender, RoutedEventArgs e)
		{
			_lastTclScriptIndex++;
			var menu = new MenuItem();
			var control = new ScriptWndTclScript(_lastTclScriptIndex);
			var tab = new TabItem()
			{
				Content = control
			};
			var script = new Script(this, menu, tab, control);

			mnu_scripts.Items.Add(menu);
			tabs.Items.Add(tab);

			mnu_select_script(script);
		}
		private int _lastTclScriptIndex;

		private void mnu_close_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptCanClose)
			{
				return;
			}
			_selectedScript.Close();
		}

		private void mnu_new_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptSupportsFile || !ScriptIsNotRunning)
			{
				return;
			}
			_selectedScript.Control.FileNew();
		}

		private void mnu_open_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptSupportsFile || !ScriptIsNotRunning)
			{
				return;
			}
			_selectedScript.Control.FileOpen();
		}

		private void mnu_save_as_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptSupportsFile)
			{
				return;
			}
			_selectedScript.Control.FileSaveAs();
		}

		private void mnu_save_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptSupportsFile)
			{
				return;
			}
			_selectedScript.Control.FileSave();
		}

		private void mnu_verify_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptCanVerify)
			{
				return;
			}
			_selectedScript.Control.Verify();
		}

		private void mnu_run_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptIsNotRunning)
			{
				return;
			}
			_selectedScript.Control.Run();
		}

		private void mnu_stop_Click (object sender, RoutedEventArgs e)
		{
			if (!ScriptIsRunning)
			{
				return;
			}
			_selectedScript.Control.Stop();
		}

		private void mnu_stop_all_Click (object sender, RoutedEventArgs e)
		{
			if (!AnyScriptIsRunning)
			{
				return;
			}
			foreach (var script in _scripts)
			{
				script.Control.Stop();
			}
		}

		public interface IScriptControl : INotifyPropertyChanged
		{
			string ScriptTitle
			{
				get;
			}

			bool SupportsFile
			{
				get;
			}

			bool SupportsVerify
			{
				get;
			}

			bool IsRunning
			{
				get;
			}

			void FileNew ();

			void FileOpen ();

			void FileSave ();

			void FileSaveAs ();

			void Verify ();

			void Run ();

			void Stop ();

			void Dispose ();
		}

		private class Script
		{
			internal Script (ScriptWnd window, MenuItem menu, TabItem tab, UserControl content)
			{
				_window = window;
				Menu = menu;
				_defaultMenuTitle = Menu.Header as string;
				Tab = tab;
				Tab.Visibility = Visibility.Collapsed;
				Tab.Tag = this;
				Control = content as IScriptControl;
				if (Control != null)
				{
					IsRunning = Control.IsRunning;
					IsNotRunning = !IsRunning;
				}
				SupportsMultiple = menu != window.mnu_xmlscript && menu != window.mnu_singlecommand;
				if (content is INotifyPropertyChanged npc)
				{
					npc.PropertyChanged += Content_OnPropertyChanged;
				}
				Menu.Click += (s, e) =>
				{
					_window.mnu_select_script(this);
				};
				Menu.Header = MenuTitle;
				Menu.IsCheckable = true;
				lock (_window._scripts)
				{
					_window._scripts.Add(this);
				}
			}

			private readonly ScriptWnd _window;
			internal MenuItem Menu
			{
				get;
			}
			internal TabItem Tab
			{
				get;
			}

			internal IScriptControl Control
			{
				get;
			}

			internal bool SupportsFile
				=> Control?.SupportsFile ?? false;

			internal bool SupportsVerify
				=> Control?.SupportsVerify ?? false;

			internal bool SupportsRunStop
				=> Control != null;

			internal bool IsNotRunning
			{
				get;
				private set;
			}

			internal bool IsRunning
			{
				get;
				private set;
			}

			internal bool SupportsMultiple
			{
				get;
			}

			private readonly string _defaultMenuTitle;
			internal string MenuTitle
			{
				get
				{
					var result = Control?.ScriptTitle ?? _defaultMenuTitle;
					if (IsRunning)
					{
						result += " (running)";
					}
					return result;
				}
			}

			internal void Close ()
			{
				if (SupportsMultiple && _window._selectedScript == this)
				{
					lock (_window._scripts)
					{
						_window._scripts.Remove(this);
						Control?.Dispose();
					}
					_window.mnu_scripts.Items.Remove(Menu);
					_window.tabs.Items.Remove(Tab);
					_window.mnu_select_script(null);
				}
			}

			private void Content_OnPropertyChanged (object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "IsRunning")
				{
					IsRunning = Control.IsRunning;
					IsNotRunning = !IsRunning;
					_window.script_StateChanged(this);
					_window.Dispatcher.Invoke(new Action(delegate
					{
						Menu.Header = MenuTitle;
					}));
				}
				else if (e.PropertyName == "ScriptTitle")
				{
					_window.Dispatcher.Invoke(new Action(delegate
					{
						Menu.Header = MenuTitle;
					}));
				}
			}
		}


	}
}
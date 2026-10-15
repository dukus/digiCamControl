using CameraControl.Core.Classes;
using CameraControl.Core.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CameraControl.windows
{
	/// <summary>
	/// Interaction logic for ScriptWndSingleCommand.xaml
	/// </summary>
	public partial class ScriptWndSingleCommand : UserControl, ScriptWnd.IScriptControl, INotifyPropertyChanged
	{
		public ScriptWndSingleCommand ()
		{
			DataContext = this;
			InitializeComponent();
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void NotifyPropertyChanged (string info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
		#endregion

		#region ScriptWnd.IScriptControl implementation
		private bool _isRunning;
		private bool _isCancellationRequested;

		public bool IsRunning
		{
			get => _isRunning;
			private set
			{
				if (_isRunning != value)
				{
					_isRunning = value;
					_isCancellationRequested = false;
					NotifyPropertyChanged(nameof(IsRunning));
					NotifyPropertyChanged(nameof(IsNotRunning));
				}
			}
		}

		public bool IsNotRunning
			=> !IsRunning;

		void ScriptWnd.IScriptControl.Run ()
		{
			if (TextBoxCmd.Text.Length == 0)
			{
				ExecuteCommand("Help");
			}
			else
			{
				ExecuteCommand(TextBoxCmd.Text);
			}
		}

		public void Stop ()
		{
			_isCancellationRequested = true;
		}

		string ScriptWnd.IScriptControl.ScriptTitle
			=> null;

		bool ScriptWnd.IScriptControl.SupportsFile => false;
		bool ScriptWnd.IScriptControl.SupportsVerify => false;

		void ScriptWnd.IScriptControl.FileNew ()
		{
			throw new NotSupportedException();
		}

		void ScriptWnd.IScriptControl.FileOpen ()
		{
			throw new NotSupportedException();
		}

		void ScriptWnd.IScriptControl.FileSave ()
		{
			throw new NotSupportedException();
		}

		void ScriptWnd.IScriptControl.FileSaveAs ()
		{
			throw new NotSupportedException();
		}

		void ScriptWnd.IScriptControl.Verify ()
		{
			throw new NotSupportedException();
		}

		void ScriptWnd.IScriptControl.Dispose ()
		{
		}
		#endregion

		private void Execute_Click (object sender, RoutedEventArgs e)
		{
			ExecuteCommand(TextBoxCmd.Text);
		}

		private void lst_cmd_MouseDoubleClick (object sender, MouseButtonEventArgs e)
		{
			if (lst_cmd.SelectedItem != null && !IsRunning)
				TextBoxCmd.Text = lst_cmd.SelectedItem.ToString();
		}

		private void TextBoxCmd_KeyDown (object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				Execute_Click(null, null);

		}

		private void Help_Click (object sender, RoutedEventArgs e)
		{
			PhotoUtils.Run(CommandLineProcessor.SingleCommandDocumentationUrl);
			if (TextBoxCmd.Text.Length == 0)
			{
				TextBoxCmd.Text = "Help";
				ExecuteCommand(TextBoxCmd.Text);
			}
		}

		private void ExecuteCommand (string commandText)
		{
			var addToHistory = commandText.Trim().ToLower() != "help";
			if (addToHistory)
			{
				_ = Task.Run(() => ExecuteCommandThread(commandText, addToHistory));
			}
			else
			{
				ExecuteCommandThread(commandText, addToHistory);
			}
		}
		private void ExecuteCommandThread (string commandText, bool addToHistory)
		{
			if (IsRunning)
			{
				return;
			}
			IsRunning = true;
			try
			{
				var commands = new List<string>();
				var startCommand = 0;
				void _AddCommand (int separatorPosition)
				{
					if (startCommand < separatorPosition
						&& separatorPosition <= commandText.Length)
					{
						var cmd = commandText.Substring(startCommand, separatorPosition - startCommand).Trim();
						if (!string.IsNullOrEmpty(cmd))
						{
							commands.Add(cmd);
						}
					}
					startCommand = separatorPosition + 1;
				}
				var inString = false;
				for (var i = 0; i < commandText.Length; i++)
				{
					if (commandText[i] == '"')
					{
						inString = !inString;
					}
					else if (!inString && commandText[i] == ';')
					{
						_AddCommand(i);
						startCommand = i + 1;
					}
				}
				_AddCommand(commandText.Length);
				var indent = commands.Count > 1 ? "    " : "";
				var output = "";
				try
				{
					using (var processor = new CommandLineProcessor(isCancellationRequested: () => _isCancellationRequested))
					{
						foreach (var command in commands)
						{
							if (_isCancellationRequested)
							{
								break;
							}
							if (commands.Count > 1)
							{
								output += command + ":\n";
							}
							Dispatcher.Invoke(() => TextBlockOutput.Text = output);

							var resp = processor.Pharse(command.Split(' '));
							var list = resp as IEnumerable<string>;

							if (list != null)
							{
								foreach (var o in list)
								{
									output += indent + o + "\n";
								}
							}
							else
							{
								if (resp != null)
								{
									output += indent + resp.ToString() + "\n";
								}
							}
							Dispatcher.Invoke(() => TextBlockOutput.Text = output);
						}
					}
					if (addToHistory)
					{
						Dispatcher.Invoke(() => lst_cmd.Items.Insert(0, string.Join(" ; ", commands)));
					}
				}
				catch (Exception ex)
				{
					output += indent + ex.Message;
				}
				Dispatcher.Invoke(() => TextBlockOutput.Text = output);
			}
			finally
			{
				IsRunning = false;
			}
		}
	}
}

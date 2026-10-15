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
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace CameraControl.Core.Scripting
{
	public class ScriptTriggerProvider : IScriptExternalObjectProvider
	{
		public string DisplayName => "UI button";

		public string Description => "A button to be displayed in the button window. A script is required to detect whether the button has been pressed and act accordingly.";

		public IReadOnlyList<ScriptExternalObjectCreateParameter> Parameters
		{
			get;
		} = _Parameters;
		private static readonly ScriptExternalObjectCreateParameter[] _Parameters = new ScriptExternalObjectCreateParameter[]
		{
			new ScriptExternalObjectCreateParameter ("Description", ScriptExternalObjectCreateParameter.ValueType.String)
			{
				Description = "Description to be displayed next to the button (optional).",
			},
			new ScriptExternalObjectCreateParameter ("IsEnabled", ScriptExternalObjectCreateParameter.ValueType.Boolean)
			{
				Description = "Indicates whether the button is enabled when it is created.",
				DefaultValue = true
			}
		};

		public IScriptExternalObject CreateInstance (string name, Dictionary<ScriptExternalObjectCreateParameter, object> parameters)
			=> new Button(this, name, parameters);

		private sealed class Button : IScriptExternalObject, IUIButtonProvider, IUIButton, INotifyPropertyChanged
		{
			internal Button (ScriptTriggerProvider provider, string name, Dictionary<ScriptExternalObjectCreateParameter, object> parameters)
			{
				Provider = provider;
				Name = name;
				ProviderParameters = parameters;
				Description = (string)_Parameters[0].GetValue(parameters);
				IsEnabled = (bool)_Parameters[1].GetValue(parameters);
			}

			#region IScriptExternalObject
			public IScriptExternalObjectProvider Provider { get; }

			public IReadOnlyDictionary<ScriptExternalObjectCreateParameter, object> ProviderParameters { get; }

			public string Name
			{
				get;
			}

			public IEnumerable<ScriptExternalObjectProperty> Properties
				=> _Properties;
			private static readonly ScriptExternalObjectProperty[] _Properties = new ScriptExternalObjectProperty[]
			{
				new ScriptExternalObjectProperty ("IsClicked", ScriptExternalObjectProperty.ValueType.Event, true, false)
				{
					Description = "Event that is raised when the button is clicked."
				},
				new ScriptExternalObjectProperty ("IsEnabled", ScriptExternalObjectProperty.ValueType.Boolean, true, true)
				{
					Description = "Indicates whether the button is enabled and can be clicked."
				}
			};

			public bool IsActivated
				=> true;

			void IExternalServiceScriptObject.Activate ()
			{
			}

			public bool IsEventRaised (string propertyName)
			{
				if (propertyName == _Properties[0].Name)
				{
					lock (_lock)
					{
						var result = _isClicked;
						_isClicked = false;
						NotifyPropertyChanged(nameof(IsEnabled));
						return result;
					}
				}
				throw new NotImplementedException();
			}

			public object Read (string propertyName)
			{
				if (propertyName == _Properties[1].Name)
				{
					return IsEnabled;
				}
				throw new NotImplementedException();
			}

			void IExternalServiceScriptObject.Trigger (string propertyName)
			{
				throw new NotImplementedException();
			}

			public void Write (string propertyName, object value)
			{
				if (propertyName == _Properties[1].Name)
				{
					IsEnabled = (bool)value;
				}
				throw new NotImplementedException();
			}
			#endregion

			#region IUIButtonProvider
			void IUIButtonProvider.ShowWindow ()
			{
				ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ExternalServicesWnd_ShowExternalObject, this);
			}

			IEnumerable<IUIButton> IUIButtonProvider.Buttons
			{
				get
				{
					yield return this;
				}
			}

			event EventHandler IUIButtonProvider.ButtonsChanged
			{
				add { }
				remove { }
			}
			#endregion

			#region IUIButton
			IUIButtonProvider IUIButton.Provider => this;

			public string Title
				=> Name;

			public string Description
			{
				get;
			}

			private bool _isEnabled = true;
			public bool IsEnabled
			{
				get => _isEnabled && !_isClicked;
				set
				{
					if (_isEnabled != value)
					{
						_isEnabled = value;
						NotifyPropertyChanged(nameof(IsEnabled));
					}
				}
			}

			private object _lock = new object();
			private bool _isClicked;
			public void Click ()
			{
				lock (_lock)
				{
					if (_isClicked)
					{
						return;
					}
					_isClicked = true;
				}
				NotifyPropertyChanged(nameof(IsEnabled));
			}
			#endregion

			#region Implementation of INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;
			private void NotifyPropertyChanged (string info)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
			}
			#endregion
		}
	}
}

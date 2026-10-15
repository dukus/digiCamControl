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
using System.Linq;

#endregion

namespace CameraControl.Core.Scripting
{
	/// <summary>
	/// Helper that exposes a <see cref="IScriptExternalObject"/> as an <see cref="IExternalServiceProvider"/>.
	/// </summary>
	public sealed class ScriptExternalObjectUIButtonProvider : IUIButtonProvider
	{
		private readonly IScriptExternalObject _externalObject;

		public ScriptExternalObjectUIButtonProvider (IScriptExternalObject externalObject)
		{
			_externalObject = externalObject;
			VerifyButtons();
		}

		public string Name => _externalObject.Name;

		public IEnumerable<IUIButton> Buttons
		{
			get;
			private set;
		} = Array.Empty<UIButton>();

		public IScriptExternalObject ScriptObject => _externalObject;

		public void ShowWindow ()
		{
			ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.ExternalServicesWnd_ShowExternalObject, _externalObject);
		}

		public event EventHandler ButtonsChanged;

		public void VerifyButtons ()
		{
			if (_externalObject.IsActivated)
			{
				var buttons = (from p in _externalObject.Properties
							   where p.Type == ScriptExternalObjectProperty.ValueType.Trigger
							   select new UIButton(this, _externalObject, p)).ToList();
				if (buttons.Count > 0)
				{
					Buttons = buttons;
				}
				ButtonsChanged?.Invoke(this, EventArgs.Empty);
			}
			else if (Buttons.Any())
			{
				Buttons = Array.Empty<UIButton>();
				ButtonsChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private sealed class UIButton : IUIButton
		{
			private readonly IScriptExternalObject _externalObject;
			private readonly ScriptExternalObjectProperty _property;

			internal UIButton (IUIButtonProvider provider, IScriptExternalObject externalObject, ScriptExternalObjectProperty property)
			{
				Provider = provider;
				_externalObject = externalObject;
				_property = property;
			}

			public IUIButtonProvider Provider
			{
				get;
			}

			public string Title => _property.Name;

			public string Description => _property.Description;

			public bool IsEnabled => true;

			public void Click ()
				=> _externalObject.Trigger(_property.Name);
		}
	}
}

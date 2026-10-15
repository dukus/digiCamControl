using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CameraControl.Plugins.ToolPlugins
{
	public sealed class ArduinoScriptObject : IExternalServiceScriptObject
	{
		private readonly ArduinoPlugin _plugin;
		private readonly ArduinoViewModel _viewModel;
		public ArduinoScriptObject (ArduinoPlugin plugin, ArduinoViewModel viewModel)
		{
			_plugin = plugin;
			_viewModel = viewModel;
		}

		public string Name => _plugin.Name;

		public IEnumerable<ScriptExternalObjectProperty> Properties
			=> from b in _viewModel.UIButtons
			   select new ScriptExternalObjectProperty(b.Title, ScriptExternalObjectProperty.ValueType.Trigger, true, false);

		public bool IsActivated => _plugin.IsActivated;

		public void Activate () => _viewModel.Active = true;

		public void Trigger (string propertyName)
		{
			(from b in _viewModel.UIButtons
			 where b.Title == propertyName
			 select b).FirstOrDefault()?.Click();
		}

		public bool IsEventRaised (string propertyName)
		{
			throw new NotSupportedException();
		}

		public object Read (string propertyName)
		{
			throw new NotSupportedException();
		}

		public void Write (string propertyName, object value)
		{
			throw new NotSupportedException();
		}
	}
}

namespace ScriptExternalObjectRestService.Components.ScriptExternalObjectDemo
{
	public sealed class ScriptExternalObject
	{
		#region Object as declared to digiCamControl
		public bool BooleanValue
		{
			get; set;
		} = true;

		public long LongValue
		{
			get; set;
		} = 42;

		public string StringValue
		{
			get; set;
		} = "Some string";

		public enum EnumValueType
		{
			One, Two, Three
		}
		public EnumValueType EnumValue
		{
			get; set;
		}

		public long ReadOnlyValue
		{
			get => _ReadOnlyValue;
		}

		public string WriteOnlyValue
		{
			set => _WriteOnlyValue = value;
		}

		public void PressButton ()
		{
			TriggerCount++;
		}

		public bool EventRaised
		{
			get
			{
				if (_EventRaised)
				{
					_EventRaised = false;
					return true;
				}
				else
				{
					return false;
				}
			}

		}
		#endregion

		#region Description of the object
		internal static ScriptExternalObjectProperties ObjectDescription
		{
			get;
		} = [
			new()
			{
				Name = nameof (BooleanValue),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.Boolean,
				Description = "Example of a boolean read/write property.",
				CanRead = true,
				CanWrite = true,
			},
			new()
			{
				Name = nameof (LongValue),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.Long,
				Description = "Example of a read/write property with a value of type 'long'.",
				CanRead = true,
				CanWrite = true,
			},
			new()
			{
				Name = nameof (StringValue),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.String,
				Description = "Example of a read/write property with a string value.",
				CanRead = true,
				CanWrite = true,
			},
			new()
			{
				Name = nameof (EnumValue),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.String,
				EnumerationValues = Enum.GetNames<EnumValueType> ().ToList(),
				Description = "Example of a read/write property with values from an enumeration.",
				CanRead = true,
				CanWrite = true,
			},
			new()
			{
				Name = nameof (ReadOnlyValue),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.Long,
				Description = "Example of a read-only property with a value of type 'long'.",
				CanRead = true,
				CanWrite = false,
			},
			new()
			{
				Name = nameof (WriteOnlyValue),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.String,
				Description = "Example of a read-only property with a string value.",
				CanRead = false,
				CanWrite = true,
			},
			new()
			{
				Name = nameof (PressButton),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.Trigger,
				Description = "Example of a trigger."
			},
			new()
			{
				Name = nameof (EventRaised),
				Type = ScriptExternalObjectProperties.ScriptExternalObjectProperty.ValueType.Event,
				Description = "Example of an event."
			}
		];
		#endregion

		#region Internal implementation
		internal static ScriptExternalObject Instance
		{
			get;
		} = new();

		internal void SetReadOnlyValue (long value)
		{
			_ReadOnlyValue = value;
		}
		private long _ReadOnlyValue = 123;

		internal string GetWriteOnlyValue ()
		{
			return _WriteOnlyValue;
		}
		private string _WriteOnlyValue = "(Not yet assigned)";

		internal int TriggerCount
		{
			get;
			private set;
		}

		internal void RaiseEvent ()
		{
			_EventRaised = true;
		}

		internal bool HasEventBeenRaised
			=> _EventRaised;
		private bool _EventRaised;
		#endregion
	}
}

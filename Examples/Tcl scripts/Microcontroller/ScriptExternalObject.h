#ifndef SCRIPT_EXTERNAL_OBJECT
#define SCRIPT_EXTERNAL_OBJECT

#include <Arduino.h>

#define EVENT_PIN 1


class ScriptExternalObject
{
public:
	enum EnumValueType
	{
		One, Two, Three
	};

private:
	static ScriptExternalObject* _instance;
	bool _booleanValue = true;
	long _longValue = 42;
	String _stringValue = String ("Some string");
	EnumValueType _enumValue = EnumValueType::One;
	String _readOnlyValue = String ("Not assigned; assign via WriteOnlyValue.");
	long _writeOnlyValue;
	bool _pressButtonTrigger = false;
	bool _eventRaised = false;

public:
	ScriptExternalObject ()
	{
		_instance = this;
		pinMode (LED_BUILTIN, OUTPUT);
		pinMode (EVENT_PIN, INPUT_PULLDOWN);
		attachInterrupt (EVENT_PIN, pinEvent, HIGH);
  #ifdef DEMO_WRITE_DEBUG_MESSAGES
		Serial.println ("PressButton will turn the builtin LED on/off.");
		Serial.println (String ("EventRaised is triggered by a high value of pin ") + String (EVENT_PIN));
  #endif
	}

	bool getBooleanValue () {
		return _booleanValue;
	}
	void setBooleanValue (bool value) {
		_booleanValue = value;
	}

	long getLongValue ()
	{
		return _longValue;
	}
	void setLongValue (int value)
	{
		_longValue = value;
	}

	String &getStringValue ()
	{
		return _stringValue;
	}
	void setStringValue (String value)
	{
		_stringValue = value;
	}

	EnumValueType getEnumValue ()
	{
		return _enumValue;
	}
	void setEnumValue (EnumValueType value)
	{
		_enumValue = value;
	}

	String &getReadOnlyValue ()
	{
		return _readOnlyValue;
	}

	void setWriteOnlyValue (long value)
	{
		_writeOnlyValue = value;
		_readOnlyValue = String ("WriteOnlyValue is set to ") + String (value);
	}

	void PressButton ()
	{
		_pressButtonTrigger = !_pressButtonTrigger;
		digitalWrite (LED_BUILTIN, _pressButtonTrigger ? HIGH : LOW);
#ifdef DEMO_WRITE_DEBUG_MESSAGES
	  Serial.print ("PressButton: builtin LED should be ");
		Serial.println (_pressButtonTrigger ? "on" : "off");
#endif
	}

	bool getEventRaised ()
	{
		bool result = _eventRaised;
		_eventRaised = false;
		return result;
	}

private:
	static void pinEvent ()
	{
		_instance->_eventRaised = true;
	}

public:
	static constexpr char *OBJECT_DESCRIPTION = "\
[\
	{\
		\"Name\": \"BooleanValue\",\
			\"Type\" : \"Boolean\",\
			\"Description\" : \"Example of a boolean read / write property.\",\
			\"CanRead\" : true,\
			\"CanWrite\" : true\
	},\
  {\
	\"Name\": \"LongValue\",\
	\"Type\" : \"Long\",\
	\"Description\" : \"Example of a read / write property with a value of type 'long'.\",\
	\"CanRead\" : true,\
	\"CanWrite\" : true\
  },\
  {\
	\"Name\": \"StringValue\",\
	\"Type\" : \"String\",\
	\"Description\" : \"Example of a read / write property with a string value.\",\
	\"CanRead\" : true,\
	\"CanWrite\" : true\
  },\
  {\
	\"Name\": \"EnumValue\",\
	\"Type\" : \"String\",\
	\"Description\" : \"Example of a read / write property with values from an enumeration.\",\
	\"EnumerationValues\" : [\
	  \"One\",\
	  \"Two\",\
	  \"Three\"\
	],\
	\"CanRead\" : true,\
	\"CanWrite\" : true\
  },\
  {\
	\"Name\": \"ReadOnlyValue\",\
	\"Type\" : \"String\",\
	\"Description\" : \"Example of a read - only property with a string value.\",\
	\"CanRead\" : true,\
	\"CanWrite\" : false\
  },\
  {\
	\"Name\": \"WriteOnlyValue\",\
	\"Type\" : \"long\",\
	\"Description\" : \"Example of a read - only property with a value of type 'long'.\",\
	\"CanRead\" : false,\
	\"CanWrite\" : true\
  },\
  {\
	\"Name\": \"PressButton\",\
	\"Type\" : \"Trigger\",\
	\"Description\" : \"Example of a trigger.\"\
  },\
  {\
	\"Name\": \"EventRaised\",\
	\"Type\" : \"Event\",\
	\"Description\" : \"Example of an event.\"\
  }\
]";
};

ScriptExternalObject* ScriptExternalObject::_instance = nullptr;
#endif


#ifndef SCRIPT_EXTERNAL_OBJECT_SERIAL_SERVICE
#define SCRIPT_EXTERNAL_OBJECT_SERIAL_SERVICE

#include <Arduino.h>
#include "ScriptExternalObject.h"

#define READ_BUFFER_SIZE 10240

#define REQUEST_BUFFER_SIZE 10240
#define START_REQUEST_CHAR '\a'
#define PROPERTY_VALUE_SEPARATOR '='
#define END_REQUEST_CHAR '\f'

#define START_RESPONSE_CHAR '\a'
#define CODE_RESULT_SEPARATOR ':'
#define CODE_OK 0
#define CODE_UNKNOWN_PROPERTY 1
#define CODE_INVALID_VALUE 2
#define END_RESPONSE_CHAR '\f'

class ScriptExternalObjectSerialService
{
private:
	ScriptExternalObject *_instance;
	char _buffer[READ_BUFFER_SIZE];
	char _request[REQUEST_BUFFER_SIZE];
	int _requestLength = -1;
	char *_requestValue = nullptr;

public:
	ScriptExternalObjectSerialService (ScriptExternalObject* instance) {
		_instance = instance;
	}

	void DetectRequest () {
		while (true)
		{
			int available = Serial.available ();
			if (available == 0) {
				return;
			}

			int read = Serial.readBytes (_buffer, available > READ_BUFFER_SIZE ? READ_BUFFER_SIZE : available);
			for (int i = 0; i < read; i++) {
				if (_requestLength < 0) {
					if (_buffer[i] == START_REQUEST_CHAR) {
						_requestLength = 0;
					}
				}
				else if (_buffer[i] == END_REQUEST_CHAR) {
					if (_requestLength > 0) {
						_request[_requestLength] = '\0';
						HandleRequest ();
					}
					_requestLength = -1;
					_requestValue = nullptr;
				}
				else if (_requestLength < REQUEST_BUFFER_SIZE - 1) {
					if (_buffer[i] == PROPERTY_VALUE_SEPARATOR) {
						if (_requestLength > 0) {
							_request[_requestLength++] = '\0';
						}
						_requestValue = &_request[_requestLength];
					}
					else {
						_request[_requestLength++] = _buffer[i];
					}
				}
			}
		}
	}

private:
	void HandleRequest () {
		if (_requestValue == _request && strcmp (_requestValue, "Index") == 0) {
#ifdef DEMO_WRITE_DEBUG_MESSAGES
      Serial.println ("Object description requested");
#endif
			SendObjectDescription ();
		}
		else if (_requestValue == nullptr) {
#ifdef DEMO_WRITE_DEBUG_MESSAGES
      Serial.print ("Property value or event state or activation of trigger requested (");
      Serial.print (_request);
      Serial.println (")");
#endif
			if (strcmp (_request, "BooleanValue") == 0) {
				SendValue (_instance->getBooleanValue () ? "true" : "false");
			}
			else if (strcmp (_request, "LongValue") == 0) {
				SendValue (String (_instance->getLongValue ()).c_str());
			}
			else if (strcmp (_request, "StringValue") == 0) {
				SendValue (_instance->getStringValue ().c_str());
			}
			else if (strcmp (_request, "EnumValue") == 0) {
				ScriptExternalObject::EnumValueType value = _instance->getEnumValue ();
				switch (value) {
				case ScriptExternalObject::EnumValueType::One: SendValue ("One"); break;
				case ScriptExternalObject::EnumValueType::Two: SendValue ("Two"); break;
				case ScriptExternalObject::EnumValueType::Three: SendValue ("Three"); break;
				default: SendValue ("???"); break;
				}
			}
			else if (strcmp (_request, "ReadOnlyValue") == 0) {
				SendValue (_instance->getReadOnlyValue ().c_str());
			}
    	else if (strcmp (_request, "PressButton") == 0) {
				_instance->PressButton ();
				SendCode (CODE_OK);
			}
			else if (strcmp (_request, "EventRaised") == 0) {
				SendValue (_instance->getEventRaised () ? "true" : "false");
			}
			else {
				SendCode (CODE_UNKNOWN_PROPERTY);
			}
		}
		else {
#ifdef DEMO_WRITE_DEBUG_MESSAGES
      Serial.print ("Assignment of property value requested (");
      Serial.print (_request);
      Serial.println (")");
#endif
			if (strcmp (_request, "BooleanValue") == 0) {
				_instance->setBooleanValue (strcmp (_requestValue, "true") == 0);
				SendCode (CODE_OK);
			}
			else if (strcmp (_request, "LongValue") == 0) {
				_instance->setLongValue (String (_requestValue).toInt ());
				SendCode (CODE_OK);
			}
			else if (strcmp (_request, "StringValue") == 0) {
        _instance->setStringValue (String (_requestValue));
        SendCode (CODE_OK);
			}
			else if (strcmp (_request, "EnumValue") == 0) {
        if (strcmp (_requestValue, "One") == 0) {
          _instance->setEnumValue (ScriptExternalObject::EnumValueType::One);
          SendCode (CODE_OK);
        }
        else if (strcmp (_requestValue, "Two") == 0) {
          _instance->setEnumValue (ScriptExternalObject::EnumValueType::Two);
          SendCode (CODE_OK);
        }
        else if (strcmp (_requestValue, "Three") == 0) {
          _instance->setEnumValue (ScriptExternalObject::EnumValueType::Three);
          SendCode (CODE_OK);
        }
        else {
          SendCode (CODE_INVALID_VALUE);
        }
			}
			else if (strcmp (_request, "WriteOnlyValue") == 0) {
				_instance->setWriteOnlyValue (String (_requestValue).toInt ());
				SendCode (CODE_OK);
			}
			else {
				SendCode (CODE_UNKNOWN_PROPERTY);
			}
		}
#ifdef DEMO_WRITE_DEBUG_MESSAGES
      Serial.println ("Response has been sent");
#endif
	}

	void SendObjectDescription () {
		Serial.print (START_RESPONSE_CHAR);
		Serial.print (CODE_OK);
		Serial.print (CODE_RESULT_SEPARATOR);
		Serial.print (ScriptExternalObject::OBJECT_DESCRIPTION);
		Serial.println (END_RESPONSE_CHAR);
	}

	void SendValue (const char *value) {
		Serial.print (START_RESPONSE_CHAR);
		Serial.print (CODE_OK);
		Serial.print (CODE_RESULT_SEPARATOR);
		Serial.print (value);
		Serial.println (END_RESPONSE_CHAR);
	}

	void SendCode (int code) {
		Serial.print (START_RESPONSE_CHAR);
		Serial.print (code);
		Serial.println (END_RESPONSE_CHAR);
	}
};

#endif
// Include Serial.println debug messages to prove that the digiCamControl plugin can handle and display them.
#define DEMO_WRITE_DEBUG_MESSAGES

#include "ScriptExternalObject.h"
#include "ScriptExternalObjectSerialService.h"

ScriptExternalObjectSerialService *service;
void setup () {

	Serial.begin (115200);

	ScriptExternalObject* instance = new ScriptExternalObject ();
	service = new ScriptExternalObjectSerialService (instance);
}

void loop () {
	service->DetectRequest ();
}

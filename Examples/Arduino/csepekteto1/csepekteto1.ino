#include <EEPROM.h>

/*
  Serial Event example

 When new serial data arrives, this sketch adds it to a String.
 When a newline is received, the loop prints the string and
 clears it.

 A good test for this is to try it with a GPS receiver
 that sends out NMEA 0183 sentences.

 Created 9 May 2011
 by Tom Igoe

 This example code is in the public domain.

 http://www.arduino.cc/en/Tutorial/SerialEvent

 */
const int valvePin =  5;
const int cameraPin =  3;
const int focusPin =  2;
const int flashPin =  4;
const int ledPin = 13;
const int buttonPin = 6;

// timers-----------------
int camera_timer = 350;
int drop1_time = 45;
int drop_wait_time = 100;
int drop2_time = 45;
int drop2_wait_time = 100;
int drop3_time = 45;
int flash_time = 370;
int mode = 0;
// -----------------------
bool shouldBlink = false;
// -----------------------



String inputString = "";         // a string to hold incoming data
String command = "";
String value = "" ;
boolean stringComplete = false;  // whether the string is complete
boolean start = false;


void setup() {
  pinMode(valvePin, OUTPUT);
  pinMode(cameraPin, OUTPUT);
  pinMode(focusPin, OUTPUT);
  pinMode(flashPin, OUTPUT);
  pinMode(buttonPin, INPUT);
  // initialize serial:
  Serial.begin(9600);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
  resetState();
  LoadData();
  LEDPIN_Init();
  PrintText("Welcome");
  //-----------------
}

void PrintText(char* text)
{
  PrintText(text,"");
}

void PrintText(char* text,char* text1)
{
  LED_Init();
  if (mode = 1) {
    LED_P6x8Str(1, 0, "Water drop mode");
  }
  LED_P6x8Str(1, 1, text);
    LED_P6x8Str(1, 2, text1);
  //LED_P6x8Str(1, 6, "{{{{{{{{{{{{{{{{{{{");
}


void resetState() {
  digitalWrite(valvePin, LOW);
  digitalWrite(cameraPin, LOW);
  digitalWrite(focusPin, LOW);
  digitalWrite(flashPin, LOW);
  digitalWrite(ledPin, LOW);
  digitalWrite(buttonPin, HIGH);
}

void loop() {
  if (shouldBlink)
  {
    blinkLed();
    shouldBlink = false;
  }
  if (digitalRead(buttonPin) == LOW)
  {
    Serial.println("??");
    start = true;
    blinkLed();
    delay(200);
  }
  // print the string when a newline arrives:
  if (start) {
    resetState();
    doWaterDrop();
    start = false;
  }
  // detect sound
  if (mode == 3)
  {
    //   DetectSound();
  }
}

void doWaterDrop()
{
  PrintText("Drop","in progress");
  digitalWrite(cameraPin, HIGH);
  LED_P6x8Str(1, 6, "{{{");
  delay(camera_timer);
  stPin(valvePin, drop1_time);
  LED_P6x8Str(1, 6, "{{{{{{");
  delay(drop_wait_time);
  stPin(valvePin, drop2_time);
  LED_P6x8Str(1, 6, "{{{{{{{{{");
  delay(drop2_wait_time);

  /*
    Trigger the 3 drop only if size >0
   */
  if (drop3_time > 0) {
    stPin(valvePin, drop3_time);
  }
  LED_P6x8Str(1, 6, "{{{{{{{{{{{{");
  delay(flash_time - drop1_time - drop_wait_time - drop2_time - drop2_wait_time - drop3_time );
  stPin(flashPin, 100);
  delay(50);
  digitalWrite(cameraPin, LOW);
  PrintText("Done");
}

void blinkLed()
{
  // for (int i=0; i <= 1; i++){
  stPin(ledPin, 25);
  //  }
}


void stPin(int pin, int delayM)
{
  digitalWrite(pin, HIGH);
  delay(delayM);
  digitalWrite(pin, LOW);
}

void serialEvent() {
  while ( Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    inputString += inChar;

    if (inChar == ' ')
    {
      command = "";
      inputString = "";
      start = true;
      mode = 0;
      shouldBlink = true;
    }

    if (inChar == '|')
    {
      digitalWrite(valvePin, HIGH);
    }

    if (inChar == '\\')
    {
      digitalWrite(valvePin, LOW);
    }

    if (inChar == '<')
    {
      stPin(valvePin, drop1_time);
    }

    if (inChar == '?')
    {
      command = "";
      inputString = "";
      sendData();
    }


    // check for varieable separator
    if (inChar == '=')
    {
      command = inputString;
      inputString = "";
    }

    // if the incoming character is a newline, set a flag
    // so the main loop can do something about it:
    if (inChar == '\n') {
      stringComplete = true;
      phraseParams();
    }
  }
}

void sendData()
{
  Serial.print("camera_timer=");
  Serial.println(camera_timer);

  Serial.print("drop1_time=");
  Serial.println(drop1_time);

  Serial.print("drop_wait_time=");
  Serial.println(drop_wait_time);

  Serial.print("drop2_wait_time=");
  Serial.println(drop2_wait_time);

  Serial.print("drop2_time=");
  Serial.println(drop2_time);

  Serial.print("drop3_time=");
  Serial.println(drop3_time);

  Serial.print("flash_time=");
  Serial.println(flash_time);

  Serial.print("mode=");
  Serial.println(mode);
}

void phraseParams()
{
  if (command == "c=")
  {
    camera_timer = inputString.toInt();
    //    shouldBlink = true;
  }
  if (command == "d1=")
  {
    drop1_time = inputString.toInt();
    //    shouldBlink = true;
  }
  if (command == "dw=")
  {
    drop_wait_time = inputString.toInt();
    //    shouldBlink = true;
  }
  if (command == "dw2=")
  {
    drop2_wait_time = inputString.toInt();
    //    shouldBlink = true;
  }
  if (command == "d2=")
  {
    drop2_time = inputString.toInt();
    //    shouldBlink = true;
  }
  if (command == "d3=")
  {
    drop3_time = inputString.toInt();
    //    shouldBlink = true;
  }
  if (command == "f=")
  {
    flash_time = inputString.toInt();
    shouldBlink = true;
    SaveData();
  }
  if (command == "m=")
  {
    mode = inputString.toInt();
  }
  command = "";
  inputString = "";
}

void DoFlash()
{
  digitalWrite(flashPin, HIGH);
  delay(100);
  digitalWrite(flashPin, LOW);
}
void DoValve()
{
  digitalWrite(valvePin, HIGH);
  delay(100);
  digitalWrite(valvePin, LOW);
}
void DoFocus()
{
  digitalWrite(focusPin, HIGH);
  delay(500);
  digitalWrite(focusPin, LOW);
}


void DoCapture()
{
  digitalWrite(focusPin, HIGH);
  digitalWrite(cameraPin, HIGH);
  delay(500);
  digitalWrite(focusPin, LOW);
  digitalWrite(cameraPin, LOW);
}

void SaveData()
{
  int eeAddress = 0;
  int marker = 1258;
  EEPROM.put( eeAddress, marker );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, camera_timer );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, drop1_time );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, drop_wait_time );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, drop2_time );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, drop2_wait_time );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, drop3_time );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, flash_time );
  eeAddress += sizeof(int);
  EEPROM.put( eeAddress, mode );
  eeAddress += sizeof(int);
  PrintText("Data saved");
}

void LoadData()
{
  int eeAddress = 0;
  int marker = 0;
  EEPROM.get( eeAddress, marker );
  if (marker != 1258)
    return;
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, camera_timer );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, drop1_time );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, drop_wait_time );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, drop2_time );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, drop2_wait_time );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, drop3_time );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, flash_time );
  eeAddress += sizeof(int);
  EEPROM.get( eeAddress, mode );
  eeAddress += sizeof(int);
}



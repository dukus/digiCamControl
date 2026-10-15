

String inputString = "";         // a string to hold incoming data
String command = "";
String value = "" ; 
boolean stringComplete = false;  // whether the string is complete
boolean start = false;
// pins -----------------
const int valvePin =  P2_3;
const int cameraPin =  P2_4;
const int flashPin =  P2_5;
const int soundPin =  A7;
const int ledPin =  RED_LED;
const int buttonPin = PUSH2;
// -----------------------
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
long time=0;
int maxdetect=0;
// -----------------------
void setup() {
  // initialize serial:
  Serial.begin(9600);
  // reserve 200 bytes for the inputString:
  inputString.reserve(300);
  pinMode(valvePin, OUTPUT);
  pinMode(cameraPin, OUTPUT);
  pinMode(ledPin, OUTPUT);  
  pinMode(flashPin, OUTPUT);   
  pinMode(buttonPin, INPUT_PULLUP); 
  resetState();
}

void loop() {

  if(shouldBlink)
  {
    blinkLed();
    shouldBlink = false;
  }
  if (digitalRead(buttonPin) == LOW)
  {
    start = true;
    blinkLed();
    delay(200);
  }
  // print the string when a newline arrives:
  if (start) {
    resetState();
    start = false;
    doWaterDrop();
  }
  // detect sound 
  if(mode==3)
  {
 //   DetectSound();
  }
}

void DetectSound()
{
  if(time=0){
    time = millis();
    maxdetect = 0;
  }
  int i = analogRead(A7);
  if(i>maxdetect)
      maxdetect = i;
  if(millis() -time >1000)
  {
     time = millis();
     Serial.print("sound="); 
     Serial.println(maxdetect);
     maxdetect = 0 ; 
  }
}


/*
  SerialEvent occurs whenever a new data comes in the
 hardware serial RX.  This routine is run between each
 time loop() runs, so using delay inside loop can delay
 response.  Multiple bytes of data may be available.
 */
void serialEvent() {
  while ( Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read(); 
    inputString += inChar;

    if(inChar == ' ')
    {
      command = "";
      inputString = "";
      start = true;
      mode = 0;
      shouldBlink = true;
    }

    if(inChar == '|')
    {
      digitalWrite(valvePin, HIGH);
    }    

    if(inChar == '\\')
    {
      digitalWrite(valvePin, LOW);
    }    

    if(inChar == '<')
    {
      stPin(valvePin, drop1_time);
    }    

    if(inChar == '?')
    {
      command = "";
      inputString = "";
      sendData();
    }


    // check for varieable separator
    if(inChar == '=')
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

void resetState(){
  digitalWrite(valvePin, LOW);
  digitalWrite(cameraPin, LOW);
  digitalWrite(flashPin, LOW);
  digitalWrite(ledPin, LOW);
}

void phraseParams()
{
  if(command=="c=")
  {
    camera_timer = inputString.toInt();
    //    shouldBlink = true;
  } 
  if(command=="d1=")
  {
    drop1_time = inputString.toInt();
    //    shouldBlink = true;
  } 
  if(command=="dw=")
  {
    drop_wait_time = inputString.toInt();
    //    shouldBlink = true;
  } 
  if(command=="dw2=")
  {
    drop2_wait_time = inputString.toInt();
    //    shouldBlink = true;
  }       
  if(command=="d2=")
  {
    drop2_time = inputString.toInt();
    //    shouldBlink = true;
  } 
  if(command=="d3=")
  {
    drop3_time = inputString.toInt();
    //    shouldBlink = true;
  }      
  if(command=="f=")
  {
    flash_time = inputString.toInt();
    shouldBlink = true;
  } 
  if(command=="m=")
  {
    mode = inputString.toInt();
  }
  command = "";
  inputString = "";  
}


void blinkLed()
{
  // for (int i=0; i <= 1; i++){
  stPin(ledPin, 25);
  //  }
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

void doWaterDrop()
{
  digitalWrite(cameraPin, HIGH);

  delay(camera_timer);
  stPin(valvePin, drop1_time);

  delay(drop_wait_time);        
  stPin(valvePin, drop2_time);

  delay(drop2_wait_time);        

  /*
    Trigger the 3 drop only if size >0
   */
  if(drop3_time>0){
    stPin(valvePin, drop3_time);
  }

  delay(flash_time-drop1_time-drop_wait_time-drop2_time-drop2_wait_time-drop3_time );    
  stPin(flashPin, 100);
  delay(50);
  digitalWrite(cameraPin, LOW);  
}

void stPin(int pin, int delayM)
{
  digitalWrite(pin, HIGH);
  delay(delayM);    
  digitalWrite(pin, LOW);     
}












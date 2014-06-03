
const int focusPin =  P1_6;
const int capturePin =  P1_7;
const int irPin =  P2_5;
  
// the setup routine runs once when you press reset:
void setup() {    
  pinMode(focusPin, OUTPUT); 
  pinMode(capturePin, OUTPUT);   
  Reset();
  // initialize the digital pin as an output.
  pinMode(irPin, OUTPUT);  
  Serial.begin(9600);  
}

// the loop routine runs over and over again forever:
void loop() {
 if (Serial.available() > 0) {
    char inChar = (char)Serial.read();   
    if(inChar== '1')
      AssertFocus();
    if(inChar== '2'){
      AssertFocus();
      ShutterOpen();
    }
    if(inChar== '3'){
      DeassertFocus();
      ShutterClose();
    }
    if(inChar== '4')
      DeassertFocus();
    if(inChar== '5'){
      AssertFocus();
      delay(2000);        
      ShutterOpen();      
      delay(200);  
      DeassertFocus();
      ShutterClose();
    }
    if(inChar== '6'){
      AssertFocus();
      delay(3000);  
      DeassertFocus();
    }
    if(inChar== '7'){
      Reset();      
      NikonShutterNow(irPin);
      delay(500);  
    }
    if(inChar== 'v'){
      Serial.println("V 1.01"); 
    }
    
    if(inChar== '0')
      Reset();
  }
}

void AssertFocus()
{
  digitalWrite(focusPin, HIGH);
}

void DeassertFocus()
{
  digitalWrite(focusPin, LOW);
}

void ShutterOpen()
{
  digitalWrite(capturePin, HIGH);
}

void ShutterClose()
{
  digitalWrite(capturePin, LOW);
}

void Reset()
{
  digitalWrite(focusPin, LOW);
  digitalWrite(capturePin, LOW);
  digitalWrite(irPin, LOW);
}

void wait(unsigned int time)
{
  unsigned long start = micros();

  while(micros()-start<=time)
  {
  }
}

void high(unsigned int time, int freq, int pinLED)
{
  int pause = (1000/freq/2)-4;
  unsigned long start = micros();

  while(micros()-start<=time)
  {
    digitalWrite(pinLED,HIGH);
    delayMicroseconds(pause);
    digitalWrite(pinLED,LOW);
    delayMicroseconds(pause);
  }
}

void NikonShutterNow(int pin)
{
  int freq = 40;
  //Serial.print("NikonShutterNow(pin=");
  //Serial.print(pin);
  //Serial.print(")\n");
  high(2000,freq,pin);
  wait(27830);
  high(390,freq,pin);
  wait(1580);
  high(410,freq,pin);
  wait(3580);
  high(400,freq,pin);
} 


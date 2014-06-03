/*
Code from http://www.cameraaxe.com 
 */
const int buttonPin = P1_3; 
const int ledPin =  GREEN_LED;  
int buttonState = 0;

void setup() {                
  // initialize the digital pin as an output.
  // Pin 14 has an LED connected on most Arduino boards:
  pinMode(P1_7, OUTPUT);     
  pinMode(ledPin, OUTPUT);      
  // initialize the pushbutton pin as an input:
  pinMode(buttonPin, INPUT_PULLUP);  
}

void loop() {
  buttonState = digitalRead(buttonPin);
  if (buttonState == LOW) {     
    // turn LED on:    
    digitalWrite(ledPin, HIGH);     
    NikonShutterNow(P1_7);   
    digitalWrite(ledPin, LOW); 
    delay(500);   
  }
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

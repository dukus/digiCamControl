
const int focusPin =  P1_6;
const int capturePin =  P1_7;

void setup() {
  pinMode(focusPin, OUTPUT); 
  pinMode(capturePin, OUTPUT); 
  Reset();
  // initialize serial:
  Serial.begin(9600);
}

void loop() {
  if (Serial.available() > 0) {
    char inChar = (char)Serial.read();   
    if(inChar== '1')
      AssertFocus();
    if(inChar== '2')
      ShutterOpen();
    if(inChar== '3')
      ShutterClose();
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
}







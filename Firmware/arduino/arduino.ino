int analogData = 0;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
}

void loop() {
  // put your main code here, to run repeatedly:
  analogData = analogRead(A0);

  uint8_t lsb = analogData & 0xFF;
  uint8_t msb = (analogData >> 8) & 0xFF;
  
  Serial.write(0xAB);
  Serial.write(0xCD);
  
  Serial.write(lsb);
  Serial.write(msb);
  
  Serial.write(0);// put the byte you want
  Serial.write(0);// put the byte you want
  
  Serial.write(0);// put the byte you want
  Serial.write(0);// put the byte you want
  
  Serial.write(0xAF);
  Serial.write(0xCF);

  delay(10);
}

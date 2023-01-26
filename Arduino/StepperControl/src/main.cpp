#include <Arduino.h>
#include "TMCStepper.h"
#include "AccelStepper.h"
using namespace TMC2209_n;

#define RX_PIN 7
#define TX_PIN 6
#define R_SENSE 0.11f

#define STEP 5

AccelStepper stepper(AccelStepper::FULL4WIRE, 2,3,4,5);

void setup()
{
  stepper.setMaxSpeed(200);
  stepper.setAcceleration(100);
  stepper.moveTo(100000);
  // put your setup code here, to run once:
}

bool shaft = false;

void loop()
{
  if(stepper.distanceToGo() == 0)
  stepper.moveTo(-stepper.currentPosition());
  stepper.run();
}
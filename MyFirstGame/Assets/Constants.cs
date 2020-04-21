using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants 
{
    public const float UMBRELLA_DIAMETER = 1.5f; 
    public const float POLE_LENGTH = 2.0f; 
    public const float MAX_SPEED = 30; 
    public const float stdDevThetaDot = 1; // degrees per second
    public const float stdDevPhiDot = 1; //degrees per second
    
    public const float sensorDevThetaDot = 2; 
    public const float sensorDevPhiDot = 2;
    public const float sensorDevPhi = 10; 
    public const float sensorDevTheta = 10; 
    //We can then calculate stdDev for theta and phi as dt*stdDevThetaDot
}

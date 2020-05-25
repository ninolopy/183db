using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants 
{
    public const float UMBRELLA_DIAMETER = 1.5f;
    public const float UMBRELLA_MASS = 3.0f; //3kg 
    public const float POLE_LENGTH = 2.0f; 
    public const float POLE_MASS = 4.0f; // 4kg  
    public const float MAX_SPEED = 30; 
    public const float stdDevThetaDot = 2; // degrees per second
    public const float stdDevPhiDot = 2; //degrees per second
    
    public const float sensorDevThetaDot = 4; 
    public const float sensorDevPhiDot = 4;
    public const float sensorDevPhi = 10; 
    public const float sensorDevTheta = 10; 



    // DC Motor simulation constants

    // Publish results
    public const bool publish_velocity = true; 
    public const bool publish_encoder =  false; 
    public const bool publish_current =  true; 
    public const bool publish_motor_joint_state= false; 
    public const double update_rate = 100.0; 

    // motor model
    public const double motor_nominal_voltage= 24.0; // Volts
    public const double moment_of_inertia= 0.001; // kgm^2 THIS IS THE INERTIA OF THE MOTOR BIT ALONE
    public const double armature_damping_ratio= 0.0001; // Nm/(rad/s)
    public const double electromotive_force_constant= 0.08;  // Nm/A
    public const double electric_resistance= 1.0; // Ohm
    public const double electric_inductance= 0.001; // Henry
    
    // transmission
    public const double gear_ratio= 400.0; 
    public const double joint_damping= 0.005; 
    public const double joint_friction=  0.01; 
    
    // shaft encoder
    public const double encoder_ppr= 4096; 

    //We can then calculate stdDev for theta and phi as dt*stdDevThetaDot
}

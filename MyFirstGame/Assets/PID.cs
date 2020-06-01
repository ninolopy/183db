using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

public class PID : MonoBehaviour
{
    public State currState;
    private float sum;
    private float perviousError;
    private double output;

    private float kp;
    private float ki;
    private float kd;

    private float maxError;
    public float goal;
    private float maxPhi;

    private bool turnedOn;

    private Clock GlobalClock;
    private System.DateTime lastTime;
    private Sun sun; 

    private float cheatRatio;


    void Start() {
        currState = GameObject.Find("State").GetComponent(typeof(State)) as State;
        sum = 0;
        output = 0;

        kp = 10.0f;
        ki = 0.025f;
        kd = 7.0f;

        maxError = 2f;
        

        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        sun = GameObject.Find("Sun").GetComponent(typeof(Sun)) as Sun; 
        lastTime = GlobalClock.GetTime();
        cheatRatio = 20;
        turnedOn = false;
        maxPhi = 48;
        goal = -40;
    }

    void Update() {
        if (!turnedOn) {
            goal = -40;
        }

        System.TimeSpan timeDiff = GlobalClock.GetTime() - lastTime;
        float seconds = (float)timeDiff.TotalSeconds;
        lastTime = GlobalClock.GetTime();
        
        if (goal > maxPhi) {
            goal = maxPhi;
        }
        if(sun.getPhi()>goal & Mathf.Abs(goal- currState.get_phi()) < maxError){
            goal += Mathf.Min(maxError,Mathf.Abs(goal-sun.getPhi())); 
        } 
        else if(sun.getPhi()<goal & Mathf.Abs(goal- currState.get_phi()) < maxError){
            goal -= Mathf.Min(maxError,Mathf.Abs(goal-sun.getPhi())); 
        } 

        // if (Mathf.Abs(goal - currState.get_phi()) < maxError & goal < 35.0f ) { // Goal is less than actual goal
        //     goal += Mathf.Abs(goal - currState.get_phi());
        // } 
        
        // Debug.Log("Current Phi (from Kalman Filter) = " + currState.get_phi() + " Goal Phi = " +  goal);
        float state = currState.get_phi();

        float error = goal - currState.get_phi();
        float derivative = error - perviousError;
        sum += error;
        output = (kp * error + kd * (derivative/seconds) + ki * sum) * cheatRatio;
        if (output > Constants.motor_nominal_voltage*cheatRatio) {
            output = Constants.motor_nominal_voltage*cheatRatio;
        }
        else if (output < -1.0f * Constants.motor_nominal_voltage) {
            output = -1 * Constants.motor_nominal_voltage;
        }
        WriteString(output.ToString());

        perviousError = error;
    }

    public void turnOn() {
        turnedOn = true;
        goal += sun.getPhi();
    }

    public double getOutput() {
        return output;
    }

    static void WriteString(string s)
    {
        string path = "Assets/logs/PID.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(s);
        writer.Close();
    }

}
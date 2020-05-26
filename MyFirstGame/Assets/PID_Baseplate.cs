using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

public class PID_Baseplate : MonoBehaviour
{
 public State currState;
    private float sum;
    private float perviousError;
    private double output;

    private float kp;
    private float ki;
    private float kd;

    private float maxError;
    private float goal;

    private Clock GlobalClock;
    private System.DateTime lastTime;
    private Sun sun; 



    void Start() {
        currState = GameObject.Find("State").GetComponent(typeof(State)) as State;
        sum = 0;
        output = 0;

        kp = 10.0f;
        ki = 0.025f;
        kd = 7.0f;
        perviousError = 0; 

        maxError = 2f;
        goal = -maxError;

        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        sun = GameObject.Find("Sun").GetComponent(typeof(Sun)) as Sun; 
        lastTime = GlobalClock.GetTime();
        //cheatRatio = 20.0f;
    }

    void Update() {
        System.TimeSpan timeDiff = GlobalClock.GetTime() - lastTime;
        float seconds = (float)timeDiff.TotalSeconds;
        lastTime = GlobalClock.GetTime();
        
        if(sun.getTheta()>goal & Mathf.Abs(goal- currState.get_theta()) < maxError){
            goal += Mathf.Min(maxError,Mathf.Abs(goal-sun.getTheta())); 
        } 
        else if(sun.getTheta()<goal & Mathf.Abs(goal- currState.get_theta()) < maxError){
            goal -= Mathf.Min(maxError,Mathf.Abs(goal-sun.getTheta())); 
        } 

        // if (Mathf.Abs(goal - currState.get_phi()) < maxError & goal < 35.0f ) { // Goal is less than actual goal
        //     goal += Mathf.Abs(goal - currState.get_phi());
        // } 
        
        Debug.Log("Current Theta (from Kalman Filter) = " + currState.get_theta() + " Goal Theta = " +  goal);
        float state = currState.get_theta();

        float error = goal - currState.get_theta();
        float derivative = error - perviousError;
        sum += error;
        //Debug.Log(cheatRatio); 
        output = (kp * error + kd * (derivative/seconds) + ki * sum) * 20; // Replaced cheatRatio with 20
        if (output > Constants.motor_nominal_voltage*20) {
            output = Constants.motor_nominal_voltage*20;
        }
        else if (output < -1.0f * Constants.motor_nominal_voltage*20) {
            output = -1 * Constants.motor_nominal_voltage*20;
        }
        WriteString(output.ToString());

        perviousError = error;
    }

    public double getOutput() {
        return output;
    }

    static void WriteString(string s)
    {
        string path = "Assets/logs/PID2.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(s);
        writer.Close();
    }
}

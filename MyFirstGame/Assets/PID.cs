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
    private float goal;

    private Clock GlobalClock;
    private System.DateTime lastTime;

    private float cheatRatio;


    void Start() {
        currState = GameObject.Find("State").GetComponent(typeof(State)) as State;
        sum = 0;
        output = 0;

        kp = 10.0f;
        ki = 0.025f;
        kd = 7.0f;

        maxError = 2f;
        goal = maxError;

        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        lastTime = GlobalClock.GetTime();
        cheatRatio = 20;
    }

    void Update() {
        System.TimeSpan timeDiff = GlobalClock.GetTime() - lastTime;
        float seconds = (float)timeDiff.TotalSeconds;
        lastTime = GlobalClock.GetTime();

        if (Mathf.Abs(goal - currState.get_phi()) < maxError & goal < 35.0f) {
            goal += Mathf.Abs(goal - currState.get_phi());
        } 
        Debug.Log("Current Phi (from Kalman Filter) = " + currState.get_phi() + " Goal Phi = " +  goal);
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
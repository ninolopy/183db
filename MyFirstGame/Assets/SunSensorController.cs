using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSensorController : MonoBehaviour
{
    public SunSensor SunSensor1;
    public SunSensor SunSensor2;
    public SunSensor SunSensor3;
    public SunSensor SunSensor4;
    public PID armPID;
    public float average;
    public int secondsInSun; //in seconds
    public int threshold;
    // Start is called before the first frame update
    void Start()
    {
        SunSensor1 = GameObject.Find("SunSensor1").GetComponent(typeof(SunSensor)) as SunSensor;
        SunSensor2 = GameObject.Find("SunSensor2").GetComponent(typeof(SunSensor)) as SunSensor;
        SunSensor3 = GameObject.Find("SunSensor3").GetComponent(typeof(SunSensor)) as SunSensor;
        SunSensor4 = GameObject.Find("SunSensor4").GetComponent(typeof(SunSensor)) as SunSensor;
        armPID = GameObject.Find("PID").GetComponent(typeof(PID)) as PID;
        average = 0;
        secondsInSun = 0;
    }

    // Update is called once per frame
    void Update()
    {
        average += SunSensor1.solarIntensity();
        average += SunSensor2.solarIntensity();
        average += SunSensor3.solarIntensity();
        average += SunSensor4.solarIntensity();
        average /= 4f;
        // Debug.Log("SunSensor1 = " + SunSensor1.solarIntensity());
        // Debug.Log("SunSensor2 = " + SunSensor1.solarIntensity());
        // Debug.Log("SunSensor3 = " + SunSensor1.solarIntensity());
        // Debug.Log("SunSensor4 = " + SunSensor1.solarIntensity());
        if (average > 10) {
            //it is sunny
            Debug.Log("Sunny");
            secondsInSun += Constants.timeStepInSeconds;
            if (secondsInSun > threshold) {
                armPID.turnOn();
            }
            return;
        }
        else {
            //it is not sunny
            //Debug.Log("Not Sunny");
            return;
        }
    }
}

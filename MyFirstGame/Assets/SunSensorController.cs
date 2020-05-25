using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSensorController : MonoBehaviour
{
    public SunSensor SunSensor1;
    public SunSensor SunSensor2;
    public SunSensor SunSensor3;
    public SunSensor SunSensor4;
    // Start is called before the first frame update
    void Start()
    {
        SunSensor1 = GameObject.Find("SunSensor1").GetComponent(typeof(SunSensor)) as SunSensor;
        SunSensor2 = GameObject.Find("SunSensor2").GetComponent(typeof(SunSensor)) as SunSensor;
        SunSensor3 = GameObject.Find("SunSensor3").GetComponent(typeof(SunSensor)) as SunSensor;
        SunSensor4 = GameObject.Find("SunSensor4").GetComponent(typeof(SunSensor)) as SunSensor;

    }

    // Update is called once per frame
    void Update()
    {
        float average = 0;
        average += SunSensor1.solarIntensity();
        average += SunSensor2.solarIntensity();
        average += SunSensor3.solarIntensity();
        average += SunSensor4.solarIntensity();
        average /= 4f;
        // Debug.Log("SunSensor1 = " + SunSensor1.solarIntensity());
        // Debug.Log("SunSensor2 = " + SunSensor1.solarIntensity());
        // Debug.Log("SunSensor3 = " + SunSensor1.solarIntensity());
        // Debug.Log("SunSensor4 = " + SunSensor1.solarIntensity());
        if (average > 0) {
            //it is sunny
            //Debug.Log("Sunny");

            return;
        }
        else {
            //it is not sunny
            //Debug.Log("Not Sunny");
            return;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class SunSensor : MonoBehaviour
{
    // Start is called before the first frame update
    public Sun sun; 
    public Clock GlobalClock;
    public Vector3 sunPosition; 
    public Vector3 sensorPosition; 
    public float LastReading;

    static float SunIntensity = 1000f;


    void Start()
    {
        LastReading = 0;
        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        sun = GameObject.Find("Sun").GetComponent(typeof(Sun)) as Sun;
        sunPosition = sun.transform.position;
        sensorPosition =  transform.position; 
    }

    // Update is called once per frame
    void Update()
    {   
        is_sunny();
        //Debug.Log("is_sunny returns:" + is_sunny());
        // Calc angle between sun and plants
    }

    public float solarIntensity() {
        return LastReading;
    }

    public bool is_sunny(){
        LastReading = 0;
        sunPosition = sun.transform.position; 
        Vector3 rayDirection = sensorPosition - sunPosition; 
        RaycastHit hitinfo; 

        System.DateTime Time = GlobalClock.GetTime();
        if(Physics.Raycast(sunPosition,rayDirection,out hitinfo, 100000)){
            if(hitinfo.collider.tag == "Ground"){
                Debug.Log(hitinfo.collider.tag);
                return false;
            }
            else if(hitinfo.collider.tag == "SunSensor"){
                float SolarIntensity = SunIntensity * 1.0f / (hitinfo.distance * hitinfo.distance);
                WriteString(name + " " + SolarIntensity);
                LastReading = SolarIntensity;
                return true; 
            }
            else {
                Debug.Log(hitinfo.collider.tag);
            }
        } 
        //Debug.Log("NO COLLISIONS!!");
        WriteString("" + Time.Hour + ":" + Time.Minute + ":" + Time.Second + ":: Not Sunny");
        return false; 
    }
    static void WriteString(string s)
    {
        string path = "Assets/logs/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(s);
        writer.Close();
    }
}

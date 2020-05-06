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

    void Start()
    {
        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        sun = GameObject.Find("Sun").GetComponent(typeof(Sun)) as Sun;
        sunPosition = sun.transform.position;
        sensorPosition =  transform.position; 
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.Log("is_sunny returns:" + is_sunny());
        // Calc angle between sun and plants
    }

    public bool is_sunny(){
        sunPosition = sun.transform.position; 
        Vector3 rayDirection = sensorPosition - sunPosition; 
        RaycastHit hitinfo; 

        System.DateTime Time = GlobalClock.GetTime();
        if(Physics.Raycast(sunPosition,rayDirection,out hitinfo, 100000)){
            if(hitinfo.collider.tag == "Ground"){
                Debug.Log(hitinfo.collider.tag);

                WriteString("" + Time.Hour + ":" + Time.Minute + ":" + Time.Second + ":: Not Sunny");
                return false;
            }
            else if(hitinfo.collider.tag == "SunSensor"){
                WriteString("" + Time.Hour + ":" + Time.Minute + ":" + Time.Second + ":: Sunny");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTime : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject theDisplay;
    public Clock GlobalClock;
    public int hour; 
    public int minutes;
    public int seconds;
    
    void Start()
    {
        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        UpdateClock();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
    }
    void UpdateClock() {
        System.DateTime Time = GlobalClock.GetTime();
        theDisplay.GetComponent<Text>().text = "" + Time.Hour + ":" + Time.Minute + ":" + Time.Second; 
    }
}

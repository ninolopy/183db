using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    // Start is called before the first frame update
    int hour;
    int minute;
    int second;
    int year;
    int month;
    int day;


    void Start()
    {
        hour = 0;
        minute = 0;
        month = 4;
        day = 20;
        year = 2020;
        second = 0;
    }

    // Update is called once per frame
    void Update()
    {
        second += 20;
        if (second >= 60) {
            second = 0;
            minute += 1;
        }

        if (minute >= 60) {
            hour += 1;
            minute = 0;
        }

        if (hour >= 24) {
            hour = 0;
        }
    }

    public System.DateTime GetTime() {
        return new System.DateTime(year, month, day, hour, minute, second);//, hour, minute, second);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlate : MonoBehaviour
{
    public State state; 
    float thetaPrev; 
    Vector3 currentEulerAngles; 
    Sun TheSun;

    // Start is called before the first frame update
    void Start()
    {
        thetaPrev = 0;
        currentEulerAngles = new Vector3(0,0,0);
        TheSun = GameObject.Find("Sun").GetComponent(typeof(Sun)) as Sun;

    }

    // Update is called once per frame
    void Update()
    {
        float theta = state.get_theta(); 
        float diff = theta - thetaPrev; 
        thetaPrev = theta; 

        currentEulerAngles += new Vector3(0,diff,0); 
        transform.eulerAngles = currentEulerAngles;
        
    }
}

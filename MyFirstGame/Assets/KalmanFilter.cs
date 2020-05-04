using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalmanFilter : MonoBehaviour
{
    // Start is called before the first frame update
    public float[] x;
    public float[] u; 
    public float[] q; 
    
    public float max_angle; 
    public float min_angle; 
    public System.Random rand; 

    void Start()
    {
        x = new float[4]; 
        x[0] = 0;
        x[1] = 0;
        x[2] = 0;
        x[3] = 0;

        u = new float[2]; 
        u[0] = 0; 
        u[1] = 0; 

        //We will represent q only with the values of q[1][1],q[2][2],q[3][3],q[4][4]. These are the 
        q = new float[4];
        q[0] = (float)System.Math.Pow(Constants.stdDevThetaDot * Time.deltaTime,2); 
        q[1] = (float)System.Math.Pow(Constants.stdDevPhiDot * Time.deltaTime,2);
        q[2] = (float)System.Math.Pow(Constants.stdDevThetaDot,2); 
        q[3] = (float)System.Math.Pow(Constants.stdDevPhiDot,2);   

        rand = new System.Random();
        
        float diameter = Constants.UMBRELLA_DIAMETER;
        float pole_length = Constants.POLE_LENGTH; 

        //Debug.Log("Diameter =" + diameter + " Pole_length=" + pole_length); 

        min_angle = (float)((System.Math.Atan(((diameter/2)/pole_length)))*(180/3.1415))-90; //Degrees
        max_angle = (float)(-min_angle);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

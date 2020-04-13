using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class State : MonoBehaviour
{
    public float[] x;
    public float[] u; 
    public float max_angle; 
    public float min_angle; 

    // Start is called before the first frame update
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
 

        //Determine max and min phi angles for given umbrella diameter and pole length. 
        float diameter = Constants.UMBRELLA_DIAMETER;
        float pole_length = Constants.POLE_LENGTH; 

        //Debug.Log("Diameter =" + diameter + " Pole_length=" + pole_length); 

        min_angle = (float)((System.Math.Atan(((diameter/2)/pole_length)))*(180/3.1415))-90; //Degrees
        max_angle = (float)(-min_angle);
        //Debug.Log("Min_Angle =" + min_angle + " Max_Angle=" + max_angle); 
    }

    // Update is called once per frame

    //[theta,phi,thetadot,phidot]

    void Update()
    {
        //CONTROLS MOVEMENT OF THE BASEPLATE
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)){
            u[0] = 0; 
        }
        else if (Input.GetKey(KeyCode.RightArrow)){
             // Turn Clockwise 
             u[0] = Constants.MAX_SPEED; 
        } 
        else if (Input.GetKey(KeyCode.LeftArrow)){
             // Turn CounterClockwise
             u[0] = -Constants.MAX_SPEED; 
        }
        else{
            u[0] = 0; 
        }


        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)){
            u[1] = 0; 
        }
        else if (Input.GetKey(KeyCode.UpArrow)){
             // Umbrella up
             if(x[1]>=max_angle){
                 u[1] = 0; 
             }
             else{
                u[1] = Constants.MAX_SPEED;
             } 
        } 
        else if (Input.GetKey(KeyCode.DownArrow)){
             // Umbrella down
             if(x[1]<=min_angle){
                 u[1] = 0; 
             }
             else{
                u[1] = -Constants.MAX_SPEED;
             } 
        }
        else{
            u[1] = 0; 
        }


        float[] stateUpdate = new float[4];
        stateUpdate[0] = x[0] + x[2]*Time.deltaTime + Time.deltaTime*u[0];
        stateUpdate[1] = x[1] + x[3]*Time.deltaTime +  Time.deltaTime*u[1];
        stateUpdate[2] = x[2];
        stateUpdate[3] = x[3];
        if(stateUpdate[0]>=360){
            stateUpdate[0] = stateUpdate[0]-360; 
        }
        else if(stateUpdate[0]<0){
            stateUpdate[0] = stateUpdate[0]+360; 
        }
        x = stateUpdate;
        
        Debug.Log("Theta = " + x[0] + " Phi = " + x[1]) ;
    }

    public float get_theta(){
        return x[0];
    }

    public float get_phi(){
        return x[1]; 
    }

    public float get_theta_dot(){
        return x[2]; 
    }

    public float get_phi_dot(){
        return x[3]; 
    }

}

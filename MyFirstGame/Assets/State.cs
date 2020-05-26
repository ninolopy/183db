using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.IO;


public class State : MonoBehaviour
{
    // True state
    public Vector<float> x;
    // Input vector
    public Vector<float> u; 

    //state estimate
    public Vector<float> xHat; 
    public Vector<float> uHat; 
    //Noise covariance matrix
    public Matrix<float> q; 
    //State update matrix
    public Matrix<float> f; 
    public Matrix<float> i ; 
    //Control update matrix
    Matrix<float> b; 
    Matrix<float> h; 
    // Uncertainty estimate 
    Matrix<float> p;

    // Observation vector
    Vector<float> z; 

    Matrix<float> r; 

    public float max_angle; 
    public float min_angle; 
    public System.Random rand; 
    public Motor_Baseplate baseMotor; 
    public Motor umbrellaMotor; 
    // Start is called before the first frame update

    private Clock GlobalClock;
    private System.DateTime lastTime;

    void Start()
    {
        x = Vector<float>.Build.Dense(4); 
        xHat = Vector<float>.Build.Dense(4); 
        uHat = Vector<float>.Build.Dense(2); 
        u = Vector<float>.Build.Dense(2); 

        //Q matrix is dynamics uncertainty model
        q = Matrix<float>.Build.Dense(4,4); 
        q[0,0] = (float)System.Math.Pow(Constants.stdDevThetaDot * Time.deltaTime,2); 
        q[1,1] = (float)System.Math.Pow(Constants.stdDevPhiDot * Time.deltaTime,2);
        q[2,2] = (float)System.Math.Pow(Constants.stdDevThetaDot,2); 
        q[3,3] = (float)System.Math.Pow(Constants.stdDevPhiDot,2);   

        // F Matrix is the dynamics update matrix
        f = Matrix<float>.Build.Dense(4,4); 
        f[0,0] = 1; 
        f[1,1] = 1; 

        i = Matrix<float>.Build.Dense(4,4); 
        i[0,0] = 1;
        i[1,1] = 1; 
        i[2,2] = 1;
        i[3,3] = 1;
        // B Matrix is the control update matrix
        b = Matrix<float>.Build.Dense(4,2); 
        b[0,0] = Time.deltaTime; 
        b[1,1] = Time.deltaTime; 
        b[2,0] = 1; 
        b[3,1] = 1; 

        //z matrix is observations matrix
        z = Vector<float>.Build.Dense(4); 

        // H matrix maps the state to the observations
        h = Matrix<float>.Build.Dense(4,4); 
        h[0,0] = 1; 
        h[1,1] = 1;
        h[2,2] = 1;
        h[3,3] = 1;  

        // P Matrix is the uncertainty estimate
        p = Matrix<float>.Build.Dense(4,4); 

        // R matrix is the uncertainty of sensors matrix
        r = Matrix<float>.Build.Dense(4,4); 
        r[0,0] =(float) System.Math.Pow(Constants.sensorDevTheta,2); 
        r[1,1] =(float) System.Math.Pow(Constants.sensorDevPhi,2); 
        r[2,2] =(float) System.Math.Pow(Constants.sensorDevThetaDot,2); 
        r[3,3] = (float) System.Math.Pow(Constants.sensorDevPhiDot,2);

        rand = new System.Random();

        //Determine max and min phi angles for given umbrella diameter and pole length. 
        float diameter = Constants.UMBRELLA_DIAMETER;
        float pole_length = Constants.POLE_LENGTH; 

        //Debug.Log("Diameter =" + diameter + " Pole_length=" + pole_length); 

        min_angle = (float)((System.Math.Atan(((diameter/2)/pole_length)))*(180/3.1415))-90; //Degrees
        max_angle = (float)(-min_angle);
        //Debug.Log("Min_Angle =" + min_angle + " Max_Angle=" + max_angle); 

        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        lastTime = GlobalClock.GetTime();
    }

    // Update is called once per frame

    //[theta,phi,thetadot,phidot]

    void Update()
    {
        //THIS WAS REPLACED!!
        //getUserInput(); 
        getUserInputMotorModel(); 


        System.TimeSpan timeDiff = GlobalClock.GetTime() - lastTime;
        float seconds = (float)timeDiff.TotalSeconds;
        lastTime = GlobalClock.GetTime();

        trueStateUpdate(Time.deltaTime);


        // STATE UPDATE
        Vector<float> dynamicsUpdate = Vector<float>.Build.Dense(4);
        dynamicsUpdate[0] = xHat[0] + Time.deltaTime*uHat[0]; 
        dynamicsUpdate[1] = xHat[1] + Time.deltaTime*uHat[1];
        dynamicsUpdate[2] = uHat[0];
        dynamicsUpdate[3] = uHat[1];

        // if(dynamicsUpdate[0]>=360){
        //     dynamicsUpdate[0] = dynamicsUpdate[0]-360; 
        // }
        // else if(dynamicsUpdate[0]<0){
        //     dynamicsUpdate[0] = dynamicsUpdate[0]+360; 
        // }
        xHat = dynamicsUpdate;

        // Uncertainty update
        p = f*p*f.Transpose() + q;

        // Get Sensor values
        z[0] = x[0] + generate_gaussian(0, Constants.sensorDevTheta);
        z[1] = x[1] + generate_gaussian(0, Constants.sensorDevPhi); 
        z[2] = x[2] + generate_gaussian(0, Constants.sensorDevThetaDot); 
        z[3] = x[3] + generate_gaussian(0, Constants.sensorDevPhiDot); 

        //Implement the rest of the kalman filter
        Vector<float> y = z-h*xHat; 
        Matrix<float> sInv = r+h*p*h.Transpose();
        sInv = sInv.Inverse();
        Matrix<float> k = p*h.Transpose()*sInv; 
        xHat = xHat + k*y;
        //Debug.Log("p[0][0]= " + p[0,0] + " p[1][1]= " + p[1,1] + " p[2][2]= " + p[2,2] + " p[3][3]= " + p[3,3]);
 
        p = (i-k*h)*p;
        
        

        WriteString( x[0] + " " + x[1] + " " + xHat[0]  + " "  + xHat[1] );
        //Debug.Log("x[0]= " + x[0] + " x[1]= " + x[1] );
        //Debug.Log("p[0][0]= " + p[0,0] + " p[1][1]= " + p[1,1] + " p[2][2]= " + p[2,2] + " p[3][3]= " + p[3,3]);

    }

    public float generate_gaussian(float mean, float stdDev){
        double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0-rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (float) randNormal; 
    }

    public void getUserInputMotorModel(){
        // if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)){
        //     u[0] = 0; 
        //     uHat[0] = 0; 
        // }
        // else if (Input.GetKey(KeyCode.RightArrow)){
        //      // Turn Clockwise 
        //      u[0] = Constants.MAX_SPEED; 
        //      uHat[0] = Constants.MAX_SPEED; 
        // } 
        // else if (Input.GetKey(KeyCode.LeftArrow)){
        //      // Turn CounterClockwise
        //      u[0] = -Constants.MAX_SPEED; 
        //      uHat[0] = -Constants.MAX_SPEED;             
        // }
        // else{
        //     u[0] = 0; 
        //     uHat[0] = 0; 
        // }

        u[0] = (float)baseMotor.get_ang_velocity(); 
        u[1] = (float)umbrellaMotor.get_ang_velocity(); 
    }



    public void getUserInput(){
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)){
            u[0] = 0; 
            uHat[0] = 0; 
        }
        else if (Input.GetKey(KeyCode.RightArrow)){
             // Turn Clockwise 
             u[0] = Constants.MAX_SPEED; 
             uHat[0] = Constants.MAX_SPEED; 
        } 
        else if (Input.GetKey(KeyCode.LeftArrow)){
             // Turn CounterClockwise
             u[0] = -Constants.MAX_SPEED; 
             uHat[0] = -Constants.MAX_SPEED;             
        }
        else{
            u[0] = 0; 
            uHat[0] = 0; 
        }


        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)){
            u[1] = 0; 
            uHat[1] = 0; 
        }
        else if (Input.GetKey(KeyCode.UpArrow)){
             // Umbrella up
             if(x[1]>=max_angle){
                 u[1] = 0; 
             }
             else{
                u[1] = Constants.MAX_SPEED;
             } 

             if(xHat[1]>=max_angle){
                 uHat[1] = 0 ;
             }
             else{
                 uHat[1] = Constants.MAX_SPEED; 
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

            if(xHat[1]<=min_angle){
                 uHat[1] = 0;
            }
             else{
                 uHat[1] = -Constants.MAX_SPEED; 
            }
        }
        else{
            u[1] = 0; 
            uHat[1] = 0;
        }
    }




    public void trueStateUpdate(float seconds){
        float thetaDotError = generate_gaussian(0,Constants.stdDevThetaDot);
        float phiDotError = generate_gaussian(0,Constants.stdDevPhiDot); 
        float thetaError = thetaDotError*seconds; 
        float phiError = phiDotError*seconds;
        
        if(x[1]<=min_angle && u[1] == 0){
            if(phiDotError<0){
                phiDotError = 0; 
                phiError = 0; 
            }
        }

        if(x[1]>=max_angle && u[1] == 0){
            if(phiDotError>0){
                phiDotError = 0; 
                phiError = 0; 
            }
        }

        Vector<float> stateUpdate = Vector<float>.Build.Dense(4);
        stateUpdate[0] = x[0] + seconds*u[0] + thetaError;
        stateUpdate[1] = x[1] + seconds*u[1] + phiError;
        stateUpdate[2] = u[0] + thetaDotError;
        stateUpdate[3] = u[1] + phiDotError;

        // if(stateUpdate[0]>=360){
        //     stateUpdate[0] = stateUpdate[0]-360; 
        // }
        // else if(stateUpdate[0]<0){
        //     stateUpdate[0] = stateUpdate[0]+360; 
        // }
        x = stateUpdate;
    }

    public float get_theta(){
        return x[0];
    }

    public float get_phi(){
        //Debug.Log("Phi = "+x[1]); 
        return x[1]; 
    }

    public float get_theta_dot(){
        return x[2]; 
    }

    public float get_phi_dot(){
        return x[3]; 
    }


    static void WriteString(string s){
        string path = "Assets/logs/kalman.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(s);
        writer.Close();
    }
}

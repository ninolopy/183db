using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class Motor_Baseplate : MonoBehaviour
{
    public double input_; 
    public double encoder_counter_; 
    public double internal_current_; 
    public double internal_omega_; 
    public double output_omega; 
    public double torque;  
    public State currState; 
    public float max_angle; 
    public float min_angle; 
    PID_Baseplate PIDController;


    // Start is called before the first frame update
    void Start()
    {
        // input_ = 0;
        input_ = -1;
        encoder_counter_ = 0;
        internal_current_ = 0;
        internal_omega_ = 0;
        output_omega = 0; 
        torque = 0; 

        float diameter = Constants.UMBRELLA_DIAMETER;
        float pole_length = Constants.POLE_LENGTH; 

        min_angle = (float)((System.Math.Atan(((diameter/2)/pole_length)))*(180/3.1415))-90; //Degrees
        max_angle = (float)(-min_angle);

        PIDController = GameObject.Find("PID_Baseplate").GetComponent(typeof(PID_Baseplate)) as PID_Baseplate;

    }

    // Update is called once per frame
    void Update()
    {
    
    //getUserInput() ; 
    getPIDInput();
    
    // if (input_ > 1.0) {
    //     input_ = 1.0;
    // } else if (input_ < -1.0) {
    //     input_ = -1.0;
    // }

    // input_ = 1.0;

    
    motorModelUpdate(true,output_omega,0); // NO Toorque

    }

    public double get_ang_velocity(){
        //Debug.Log("output shaft omega= " + output_omega); 
        return output_omega; 
    }



    public double get_inertia(bool is_baseplate){
        if(is_baseplate){
            return Constants.moment_of_inertia + ((1/3.0)*Constants.POLE_MASS*System.Math.Pow(Constants.POLE_LENGTH*System.Math.Sin(System.Math.Abs(currState.get_phi())*3.14/180.0),2) + Constants.UMBRELLA_MASS*System.Math.Pow(Constants.POLE_LENGTH*System.Math.Sin(System.Math.Abs(currState.get_phi())*3.14/180.0),2))/System.Math.Pow(Constants.gear_ratio,2);
        }
        return Constants.moment_of_inertia + ((1/3.0)*Constants.POLE_MASS*System.Math.Pow(Constants.POLE_LENGTH,2) + Constants.UMBRELLA_MASS*System.Math.Pow(Constants.POLE_LENGTH,2))/System.Math.Pow(Constants.gear_ratio,2); 
    }

    public void getPIDInput() {

    }

    // CONTROL MOTOR WITH UP AND DOWN KEY
    // public void getUserInput(){
    //     if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)){
    //         input_ = 0;  
    //     }
    //     else if (Input.GetKey(KeyCode.UpArrow)){
    //          // Umbrella up
    //         if(currState.get_phi()>=max_angle){
    //              input_ = 0; 
    //              output_omega = 0; 
    //         }
    //         else{
    //             input_ = 1;
    //         } 
    //     }
    //     else if (Input.GetKey(KeyCode.DownArrow)){
    //          // Umbrella down
    //         if(currState.get_phi()<=min_angle){
    //             input_ = 0; 
    //             output_omega = 0; 
    //         }
    //         else {
    //             input_ = -1;
    //         }
    //     }
    //     else{
    //         input_ = 0; 
    //     }
    // }

    public double calculateExternalTorque(bool is_baseplate){
        //using gravitational constant g= 9.81 m/sec^2
        if(currState.get_phi()<=min_angle){
            return 0; 
        }
        else if(currState.get_phi()>=max_angle){
            return 0; 
        }
        else if(is_baseplate){
            return 0; 
        }
        double external_torque = (Constants.POLE_LENGTH/2)*Constants.POLE_MASS*9.81*System.Math.Sin((currState.get_phi())*3.14/180.0) + Constants.POLE_LENGTH * Constants.UMBRELLA_MASS * 9.81*System.Math.Sin((currState.get_phi())*3.14/180.0); 
        //Debug.Log("External torque= " + external_torque); 
        return external_torque;
    }

    // public double get_omega(bool is_baseplate){
        
    // }
    public void motorModelUpdate(bool is_baseplate, double output_shaft_omega, double actual_load_torque){
        //ADD THE EXTERNAL TORQUE
        
        double T = -actual_load_torque / Constants.gear_ratio; // external loading torque converted to internal side 
        //double T = 0; 
        //double V = input_ * Constants.motor_nominal_voltage; // input voltage (command input for motor velocity)
        double V = PIDController.getOutput();
        internal_omega_ = output_shaft_omega * Constants.gear_ratio; // external shaft angular veloc. converted to internal side
        const double d = Constants.armature_damping_ratio;
        const double L = Constants.electric_inductance;
        const double R = Constants.electric_resistance;
        const double Km = Constants.electromotive_force_constant;
        double J = get_inertia(is_baseplate);
        //double J = Constants.moment_of_inertia;
        //Debug.Log("Inertia= " + J); 
        double dt = Time.deltaTime;
        double i0 = internal_current_;
        double o0 = internal_omega_;
        double d2 = System.Math.Pow(d,2);
        double L2 = System.Math.Pow(L,2);
        double J2 = System.Math.Pow(J,2);
        double R2 = System.Math.Pow(R,2);
        double Km2 = System.Math.Pow(Km,2);
        double Km3 = Km2 * Km;
        double Om = System.Math.Sqrt(d2*L2 + J2*R2 - 2*J*L*(2*Km2 + d*R));
        double eOp1 = System.Math.Exp((Om*dt)/(J*L)) + 1.0;
        double eOm1 = eOp1 - 2.0; // = exp((Om*t)/(J*L)) - 1.0;
        double eA = System.Math.Exp(((d*L + Om + J*R)*dt)/(2.0*J*L));
        double emA = 1.0/eA; // = exp(-((d*L + Om + J*R)*t)/(2.0*J*L));
        double i_t = (emA*(i0*(Km2 + d*R)*(d*L*(d*eOp1*L + eOm1*Om) + eOp1*J2*R2 - J*(4*eOp1*Km2*L + 2*d*eOp1*L*R + eOm1*Om*R)) - d*L*(d*(-2*eA + eOp1)*L + eOm1*Om)*(Km*T + d*V) - (-2*eA + eOp1)*J2*R2*(Km*T + d*V) + J*(Km3*(-2*eOm1*o0*Om + 4*(-2*eA + eOp1)*L*T) - Km*R*(2*d*eOm1*o0*Om - 2*d*(-2*eA + eOp1)*L*T + eOm1*Om*T) + 2*Km2*(2*d*(-2*eA + eOp1)*L + eOm1*Om)*V + d*(2*d*(-2*eA + eOp1)*L + eOm1*Om)*R*V)))/ (2*(Km2 + d*R)*(d2*L2 + J2*R2 - 2*J*L*(2*Km2 + d*R)));
        double o_t = (emA*(-4*eOp1*J*System.Math.Pow(Km,4)*L*o0 + J*Km2*R* (-6*d*eOp1*L*o0 + eOm1*o0*Om - 4*(-2*eA + eOp1)*L*T) + J*R2*(-2*d2*eOp1*L*o0 + d*eOm1*o0*Om - 2*d*(-2*eA + eOp1)*L*T + eOm1*Om*T) + 4*(-2*eA + eOp1)*J*Km3*L*V - J*Km*(-2*d*(-2*eA + eOp1)*L + eOm1*Om)*R*V + J2*R2*(eOp1*Km2*o0 + d*eOp1*o0*R + (-2*eA + eOp1)*R*T - (-2*eA + eOp1)*Km*V) + L*(System.Math.Pow(d,3)*eOp1*L*o0*R + 2*eOm1*Km2*Om*(i0*Km - T) + d2*(eOp1*Km2*L*o0 - eOm1*o0*Om*R + (-2*eA + eOp1)*L*R*T -  (-2*eA + eOp1)*Km*L*V) - d*eOm1*Om*(Km2*o0 + R*T + Km*(-2*i0*R + V)) )))/(2*(Km2 + d*R)* (d2*L2 + J2*R2 - 2*J*L*(2*Km2 + d*R)));
        // Update internal variables
        internal_current_ = i_t;
        internal_omega_   = o_t;
        // GLOBAL VARIABLE SET
        output_omega=internal_omega_ / Constants.gear_ratio;
        torque = Km*i_t*Constants.gear_ratio;
    }
}

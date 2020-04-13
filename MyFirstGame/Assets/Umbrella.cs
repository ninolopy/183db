using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : MonoBehaviour
{
    public GameObject customPivot;
    public GameObject basePlate; 
    public State state;  
    float thetaPrev;
    float phiPrev; 
    Vector3 currentEulerAngle; 
    Vector3 pivot; 
    // Start is called before the first frame update

    void Start()
    {
        thetaPrev = 0; 
        phiPrev = 0; 
        currentEulerAngle = new Vector3(0,0,0); 
        pivot = customPivot.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        float theta = state.get_theta(); 
        float thetaDiff = theta - thetaPrev; 
        thetaPrev = theta; 

        float phi = state.get_phi(); 
        float phiDiff = phi-phiPrev; 
        phiPrev = phi; 
        
        transform.RotateAround(pivot,Vector3.up,thetaDiff); 
        transform.RotateAround(pivot,basePlate.transform.right, phiDiff);
    
    }

}

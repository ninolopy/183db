using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sun; 
    public Vector3 sunPosition; 
    public Vector3 plantPosition; 

    void Start()
    {
        sunPosition = sun.transform.position;
        plantPosition =  transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("is_sunny returns:" + is_sunny());
        // Calc angle between sun and plants
    }

    public bool is_sunny(){
        sunPosition = sun.transform.position; 
        Vector3 rayDirection = plantPosition - sunPosition; 

        RaycastHit hitinfo; 

        if(Physics.Raycast(sunPosition,rayDirection,out hitinfo)){
            if(hitinfo.collider.tag == "Ground"){
                return false;
            }
            else if(hitinfo.collider.tag == "Plant"){
                return true; 
            }
        } 
        else{
            Debug.Log("NO COLLISIONS!!");
            return false; 
        }
        return false;
    }
}

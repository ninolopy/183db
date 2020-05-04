using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaTop : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        Umbrella umbrella = GameObject.Find("Umbrella").GetComponent(typeof(Umbrella)) as Umbrella;
        transform.SetParent(umbrella.transform);

    }

    // Update is called once per frame
    void Update()
    {
    
    }

}

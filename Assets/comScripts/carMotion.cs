
//////Deprecated included in the extCarMotionController Script
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carMotion : MonoBehaviour {
    UDPReceive CAN;
    public Vector3 direction; // here we need to build an interface that lets us allign the real and virtual world 
    double speed;
	// Use this for initialization
	void Start () {
        CAN= GameObject.FindObjectOfType<UDPReceive>();
      
    }
   
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = Vector3.Lerp(transform.position, transform.position + (transform.forward * ((float)CAN.speed)), Time.fixedDeltaTime);/// LAST NIGHT CHANGE  : Time.fixedDeltaTime
    }
}

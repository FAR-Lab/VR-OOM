using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gpsBeacon : MonoBehaviour {
    public float lon;
    public float lat;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 offset()
    {
        
        return transform.position - UDPReceive.LatLonToVec(lat, lon);

    }

    
}

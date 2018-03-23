using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalVehicleMotion : MonoBehaviour {
    Vector3 startPosition;
    Quaternion startOrientation;

    Quaternion childRotation;
    bool calibrate = false;
	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        startOrientation = transform.rotation;

    }
	
	// Update is called once per frame
	void Update () {
        if (calibrate)
        {
            calibrate = false;
            transform.position= startPosition;
            transform.rotation= childRotation;
            
        }
		
	}
    public void ReCallibrate( Quaternion _childRotation)
    {
        childRotation = _childRotation;
        if (!calibrate) { calibrate = true; }


    }
}

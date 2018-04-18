using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSteering : MonoBehaviour {
    Quaternion startRotation;

    //float targetANgle;
	// Use this for initialization
	void Start () {
        startRotation = transform.localRotation;
        //  targetANgle = 0f;

        LogitechGSDK.LogiSteeringInitialize(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
           
            transform.localRotation = startRotation * Quaternion.AngleAxis(Mathf.LerpUnclamped(0, 450, rec.lX / 32767f), Vector3.up);
            
            //  transform.Rotate(transform.forward, 2);
            // transform.RotateAroundLocal(new Vector3())
            //logData(transform, (float)rec.lX, "SteeringWheelInput");

            // Debug.Log("x-axis" + (float)rec.lX);
        }

    }
}

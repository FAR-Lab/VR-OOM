using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogitechManager : MonoBehaviour {
    bool Success = false;
	// Use this for initialization
	void Start () {

        LogitechGSDK.LogiSteeringInitialize(false);
       
    }
	
	// Update is called once per frame
	void Update () {


        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0) && !Success)
        {
            Success=LogitechGSDK.LogiPlayCarAirborne(0);
        }



    }
}

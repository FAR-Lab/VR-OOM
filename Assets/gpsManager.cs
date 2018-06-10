using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gpsManager : MonoBehaviour {
    gpsBeacon[] beacons;

        public Vector3 offset = Vector3.zero;
    public bool reportDistance = true;
	// Use this for initialization
	void Start () {
        beacons = transform.GetComponentsInChildren<gpsBeacon>();
        if (reportDistance && beacons.Length >= 2)
        {
            Debug.Log((beacons[0].transform.position - beacons[1].transform.position).magnitude);
                }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
   public Vector3 updateOffset()
    {
        Vector3 output = Vector3.zero;
        for (int i = 0; i < beacons.Length; i++) {
            output +=  beacons[i].offset();
        }
        output /= beacons.Length;
        offset = output;
        return output;
    }
}

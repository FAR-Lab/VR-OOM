using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyPos : MonoBehaviour {
    public Transform targetLocation;
    public bool copyRotation=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void EnableRotation()
    {
        copyRotation = true;


    }
    public void DisableRotation()
    {
        copyRotation = false;


    }
    private void FixedUpdate()
    {
        transform.position = targetLocation.position;
        if(copyRotation){ transform.rotation = targetLocation.rotation; }
    }

}

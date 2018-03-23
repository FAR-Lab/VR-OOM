using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.localRotation = transform.localRotation * Quaternion.Euler(5.3f*Time.deltaTime, 3.2f*Time.deltaTime, 7.1f*Time.deltaTime);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorColliderScript : MonoBehaviour {
	private bool triggered=false;
	private Collider theCollider;
	private IEnumerator openDoorDelay;
	private interactingObjectDoor theDoor;

	public float delayTime;
	// Use this for initialization
	void Start () {
		theCollider = transform.GetComponent<Collider>();
		theDoor = GameObject.FindObjectOfType<interactingObjectDoor>() as interactingObjectDoor;
		triggered=false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider theOther)
	{
		//Debug.Log("we just had something enter the trigger collider box");
		if (theOther.transform.GetComponent<extCarMotionController>()!=null) { // this is ugly but we just need to check if the thing that entered the Collider is actually the car and not something else.
			if(!triggered){
				openDoorDelay = openTheDoor(delayTime);
				StartCoroutine(openDoorDelay);
				triggered = true;
			}

		}

	}
	IEnumerator openTheDoor(float delay){

		yield return new WaitForSeconds(delay);

		theDoor.openDoor();

	}
}

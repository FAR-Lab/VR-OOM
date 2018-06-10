using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisionController : MonoBehaviour {




	private bool triggered=false;
	private Collider theCollider;
	private IEnumerator startCar;

	public float delayTime;
	public AIInput CrashCar;
    public bool triggerPlease;
	logger mainLogger;
	// Use this for initialization
	void Start () {
		mainLogger = GameObject.FindObjectOfType<logger>() as logger;
		theCollider = transform.GetComponent<Collider>();
		triggered=false;
	}
	
	// Update is called once per frame
	void Update () {
        if (triggerPlease && !triggered)
        {
            triggerPlease = false;
            triggered = true;
            startCar = startCarMoving(delayTime);
            StartCoroutine(startCar);
        }

		if (triggered) {
			mainLogger.logData(CrashCar.transform, CrashCar.transform.position.x, "xPos");
			mainLogger.logData(CrashCar.transform, CrashCar.transform.position.y, "yPos");
			mainLogger.logData(CrashCar.transform, CrashCar.transform.position.z, "zPos");

			mainLogger.logData(CrashCar.transform, CrashCar.transform.rotation.x, "Qx");
			mainLogger.logData(CrashCar.transform, CrashCar.transform.rotation.y, "Qy");
			mainLogger.logData(CrashCar.transform, CrashCar.transform.rotation.z, "Qz");
			mainLogger.logData(CrashCar.transform, CrashCar.transform.rotation.w, "Qw");
		}
	}



	void OnTriggerEnter(Collider theOther)
	{
		//Debug.Log("we just had something enter the trigger collider box");
		if (theOther.transform.GetComponent<extCarMotionController>()!=null) { // this is ugly but we just need to check if the thing that entered the Collider is actually the car and not something else.
			if(!triggered){
				startCar = startCarMoving(delayTime);
				StartCoroutine(startCar);
				triggered = true;
			}

		}

	}

	IEnumerator startCarMoving(float delay){

		yield return new WaitForSeconds(delay);
		CrashCar.full_stop = false;
		mainLogger.logData(transform,0, "fullstop");



	}
}

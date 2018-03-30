using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Saving and loading of car position and rotation is not fully implemented
/// 
/// </summary>
public class ConditionManager : MonoBehaviour {
	public  Vector3 carStartPosition;
	public Quaternion carStartOrientation;

	 Transform theCar;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SaveCarPose(){// should only be called in the editor
		theCar = GameObject.FindObjectOfType<extCarMotionController>().transform;// this is not good code could be something else attached to this script even though its unlikely
		carStartPosition = theCar.position;
		carStartOrientation = theCar.rotation;
	}
	public void LoadCarPose(){
		theCar = GameObject.FindObjectOfType<extCarMotionController>().transform; // see above, same issue
		theCar.position = carStartPosition;
		theCar.rotation = carStartOrientation;
	}
	public void ApplyPose(ref Vector3 pos_,ref Quaternion quat_){
		pos_ = carStartPosition;
		quat_ = carStartOrientation;
	}
}

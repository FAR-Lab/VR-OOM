using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RVP;
public class interactingObjectDoor : MonoBehaviour {

    public float targetDoorAngle;
	 bool open;

 
	 float lerper;
    Quaternion startRotation;
	Vector3 pos;
    // Use this for initialization
    void Start() {
		startRotation = transform.localRotation;
		pos = transform.localPosition;
		open = false;
	
		lerper = 2f;
    }
   /* private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 300, 100, 30), "OpenDoor"))
        {
            openDoor();
        }
    }*/
    // Update is called once per frame
    void Update() {
		if(lerper<1)
        {
            lerper += Time.deltaTime*2;
			if(transform.GetComponent<DetachablePart>().joints[0]!=null){
				RVP.PartJoint temp= transform.GetComponent<DetachablePart>().joints[0];
				transform.RotateAround(transform.TransformPoint(temp.hingeAnchor), temp.hingeAxis, targetDoorAngle*Time.deltaTime*2);
			}
			if (lerper >= 1)
            {
				if (targetDoorAngle > 0) {
					open = true;
				} else {
					open = false;
				}
               
            }
        }

    }
    public void openDoor() { 
		if(lerper>=1){
			if (!open) {
				targetDoorAngle = Mathf.Abs(targetDoorAngle);
				lerper = 0;
			}           
        }

    }
	public void closeDoor(){
		if(lerper>=1){
			if (open) {
				targetDoorAngle = -Mathf.Abs(targetDoorAngle);
				lerper = 0;
			}           
		}
	}
	public void resetDoor(){
		open = false;
		lerper = 2f;
		transform.localRotation = startRotation;
		transform.localPosition=pos;
	}

}

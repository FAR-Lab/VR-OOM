using UnityEngine;
using System.Collections;

public class waypoint : MonoBehaviour {
		public float targetSpeed;
	public Vector3	position;

	// Use this for initialization
	void Awake () {
		position=transform.position;
	}
	public waypoint getNextWaypoint(){
		
		waypoint nextOne=null;
		if (transform.parent.GetComponent<waypointStreetManager> () != null) {
			nextOne= transform.parent.GetComponent<waypointStreetManager> ().getNextWaypoint (this);
		} else if (transform.parent.GetComponent<lane> () != null) {
			nextOne = transform.parent.GetComponent<lane> ().getNextWaypoint (this);
		}

		return nextOne;


	}
	public waypoint getPreviousWaypoint(){
		
		waypoint previousOne=null;
		if (transform.parent.GetComponent<waypointStreetManager> () != null) {
			previousOne = transform.parent.GetComponent<waypointStreetManager> ().getPreviousWaypoint (this);
		} else if (transform.parent.GetComponent<lane> () != null) {// These version is 
			previousOne = transform.parent.GetComponent<lane> ().getPreviousWaypoint (this);
		}
		return previousOne;


	}
	public waypoint changeLaneTo(int laneNumbe){
		if (transform.parent.GetComponent<lane> () != null) {
			return transform.parent.GetComponent<lane> ().changeLane (this, laneNumbe);
		}
		else return null;

	}
    public float getWaypointSpeed()
    {


        return targetSpeed;

    }
	void Update () {
	
	}
}

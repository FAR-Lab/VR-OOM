using UnityEngine;
using System.Collections;
using RVP;

public class waypointMovementManager : MonoBehaviour
{
	public waypoint startWaypoint;



	 
	waypoint previousWaypoint;
	waypoint nextWaypoint;
	bool full_stop;
	public bool WrongSideOfTheRoad;
	public bool changeLanes =false;
	// Use this for initialization
	void Start()
	{	
		if(startWaypoint!=null){
			nextWaypoint = startWaypoint;}
		else{
			initialize();
		}
		WrongSideOfTheRoad=false;
		full_stop = false;
	}

	public void initialize()
	{
//		Debug.Log ("uninitialized street waypoint target. Automatic init attempted");
		RaycastHit hit;
		Debug.DrawRay (transform.position , -transform.up);
		if (Physics.Raycast (transform.position, -transform.up, out hit, 50)) {
			if (hit.transform.GetComponent<waypointStreetManager> () != null) {
				Vector3 A;
				Vector3 B;
				hit.transform.GetComponent<waypointStreetManager> ().getDirectionVectors (out A, out B);

				Vector3 forward = new Vector3 (transform.forward.x, 0, transform.forward.z).normalized;
				A = new Vector3 (A.x, 0, A.z).normalized;
				B = new Vector3 (B.x, 0, B.z).normalized;
				float angleA = Vector3.Angle (forward, A);
				float angleB = Vector3.Angle (forward, B);
				//Debug.Log ("fwd:" + forward + " A:" + A + "   B:" + B);
				//Debug.Log ("Obj:" + transform.name + " angleA:" + angleA + "   angleB:" + angleB);

				if (angleA < angleB) {
					nextWaypoint = hit.transform.GetComponent<waypointStreetManager> ().getClossedWaypoint (0, transform.position);

				} else if (angleB < angleA) {

					nextWaypoint = hit.transform.GetComponent<waypointStreetManager> ().getClossedWaypoint (1, transform.position);
				} else {
					nextWaypoint = hit.transform.GetComponent<waypointStreetManager> ().getClossedWaypoint (0, transform.position);
				}


				moveToNextWaypoint ();
				full_stop = false;
				transform.GetComponent<AIInput> ().startTheCar ();
				transform.GetComponent<AIInput> ().Move (nextWaypoint.position);
			} else {
				Debug.Log ("I am not standing on a waypoint Street Manager. Automatic Init Failed. Where should I move?");
			}

		} else{
			Debug.Log ("I am not standing on anything. Automatic Init Failed. Where should I move?");
			//Debug.Log ("What we hit was called:" +hit.transform.name);
		}
		/// This function needs to A-calculate direction vectors for each lane(by looking at the first and last waypoint object in the list(maybe this should run in the waypoint class))
		/// B-compare them to the vehicles forward direction or velocity
		/// C- Based on that pick a road side 
		/// and D- retrieve the neareast way point 
		/// E- update so that not th nearest way point is the next but the nexzt one in that direction						



	}

	public void goGhost(){
		//move onthe wrong side of the road forward...
		//Debug.Log("Befor: "+ nextWaypoint.position.ToString());
		WrongSideOfTheRoad = !WrongSideOfTheRoad;
		waypoint temp=nextWaypoint;

		nextWaypoint = nextWaypoint.transform.parent.GetComponent<waypointStreetManager>().ReturnMirrorWaypoint(nextWaypoint,WrongSideOfTheRoad);
		//Debug.Log("After: "+ nextWaypoint.position.ToString());


		moveToNextWaypoint();
		transform.GetComponent<AIInput>().Move(nextWaypoint.position);
		previousWaypoint = temp;
		//A trigger to retrieve the mirrored image
		//set the direction to revers
		//B get the next way point


	}


	// Update is called once per frame
	void Update()
	{
		
		if (changeLanes) {
			goGhost();
			changeLanes = false;

		}


		if (previousWaypoint!=null && nextWaypoint!=null) {
			//we Can set here a host parameter to tell the car that its going in the wrong direction
			Debug.DrawLine (transform.position, previousWaypoint.position, Color.red);
			Debug.DrawLine (transform.position, nextWaypoint.position, Color.blue);

		}
		if ((previousWaypoint == null && nextWaypoint == null) || (previousWaypoint != null && nextWaypoint == null)) {
			// here we need to call reinitialize
			transform.GetComponent<AIInput>().stopTheCar();
			initialize();
			full_stop = true;
		} else if (previousWaypoint == null && nextWaypoint != null) {
			full_stop = false;
			if ((nextWaypoint.position - transform.position).magnitude < 3) {
				moveToNextWaypoint();

			}
			if (nextWaypoint != null) {
				transform.GetComponent<AIInput>().Move(nextWaypoint.position);
			}
		} else if (previousWaypoint != null && nextWaypoint != null) {
			float previousDistance	= (transform.position - previousWaypoint.position).magnitude;
			float nextDistance = (transform.position - nextWaypoint.position).magnitude;
			if (2.0f * nextDistance < previousDistance) {//if we moved 75% towards the next waypoint we jump to the next one
				if (moveToNextWaypoint()) {
					transform.GetComponent<AIInput>().Move(nextWaypoint.position);
				} else {
					transform.GetComponent<AIInput>().stopTheCar();
					full_stop = true;
				}
			}
		}
	}
	public void OverrideNextWaypoint(waypoint input){
		nextWaypoint = input;
		if (nextWaypoint != null) {
			transform.GetComponent<AIInput> ().Move (nextWaypoint.position);

		}
	}
	bool moveToNextWaypoint()
	{
		previousWaypoint = nextWaypoint;
		if(WrongSideOfTheRoad){
			nextWaypoint = previousWaypoint.getPreviousWaypoint();
		}
		else{
			nextWaypoint = previousWaypoint.getNextWaypoint();
		}

		if (nextWaypoint == null) {
			return false;
		} else {
			return true;
		}
	}


}

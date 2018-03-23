using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class waypointStreetManager : MonoBehaviour
{
	public List<waypoint> DirectionA = new List<waypoint>();
//	public List<waypoint> DirectionA2 = new List<waypoint>();
//	public List<waypoint> DirectionA3 = new List<waypoint>();

	public List<waypoint> DirectionB = new List<waypoint>();
	//public List<waypoint> DirectionB2 = new List<waypoint>();
	//public List<waypoint> DirectionB3 = new List<waypoint>();
	public waypointStreetManager NextDirectionA;
	public waypointStreetManager NextDirectionB;


	// Use this for initialization
	void Start(){

	}
	public void getDirectionVectors(out Vector3 A,out Vector3 B){

		A=(DirectionA[DirectionA.Count - 1].position-DirectionA[0].position);
	//	Debug.Log(DirectionA[0].position+"und dann noch"+DirectionA[DirectionA.Count - 1].position);
	
		B=(DirectionB[DirectionB.Count - 1].position-DirectionB[0].position);

	}


	public waypoint getLastRelevant(waypoint lastWaypoint)/// this has to be changed so we look at the closest point not the one ina specific road... 
	// This is because we do not know the rotation of a certain piece of road ... moving from A to B to A just because its rotated needs to be possible...
	// This is easier fixed in code and not in design ... we only have to points possible any whay ... this is whay end points are a good idea!  
	{
		float DistanceToA = (DirectionA[DirectionA.Count - 1].position - lastWaypoint.position).magnitude;
		float DistanceToB = (DirectionB[DirectionB.Count - 1].position - lastWaypoint.position).magnitude;
		if (DistanceToA < DistanceToB) {
			return DirectionA[DirectionA.Count - 1];
		} else if (DistanceToA > DistanceToB) {
			return DirectionB[DirectionB.Count - 1];
		} else {
			Debug.Log("You need to check your sceneary the is an ambigues road connection");
			return DirectionA[DirectionA.Count - 1];
		}
	}

	public waypoint getFirstRelevant(waypoint lastWaypoint)/// this has to be changed so we look at the closest point not the one ina specific road... 
	// This is because we do not know the rotation of a certain piece of road ... moving from A to B to A just because its rotated needs to be possible...
	// This is easier fixed in code and not in design ... we only have to points possible any whay ... this is whay end points are a good idea!  
	{
		float DistanceToA = (DirectionA[0].position - lastWaypoint.position).magnitude;
		float DistanceToB = (DirectionB[0].position - lastWaypoint.position).magnitude;
		if (DistanceToA < DistanceToB) {
			return DirectionA[0];
		} else if (DistanceToA > DistanceToB) {
			return DirectionB[0];
		} else {
			Debug.Log("You need to check your sceneary the is an ambigues road connection");
			return DirectionA[0];
		}
	}

	public waypoint getPreviousWaypoint(waypoint currentWaypoint)
	{

		if (DirectionA.Contains(currentWaypoint)) {
			if (DirectionA.IndexOf(currentWaypoint) >= 1) {
				return DirectionA[DirectionA.IndexOf(currentWaypoint) - 1];
			} else {
				return choosePreviousPath(currentWaypoint);
			}
		} else if (DirectionB.Contains(currentWaypoint)) {
			if (DirectionB.IndexOf(currentWaypoint) >= 1) {
				return DirectionB[DirectionB.IndexOf(currentWaypoint) - 1];
			} else {
				return choosePreviousPath(currentWaypoint);

			}

		} else {
			Debug.Log("This should not be happening! The waypoint does not belong to this street");
			return null;
		}

	}

	public waypoint getNextWaypoint(waypoint currentWaypoint)
	{
		if (DirectionA.Contains(currentWaypoint)) {
			if (DirectionA.IndexOf(currentWaypoint) < DirectionA.Count - 1) {
				return DirectionA[DirectionA.IndexOf(currentWaypoint) + 1];
			} else {
				return chooseNextPath(currentWaypoint);
			}
		} else if (DirectionB.Contains(currentWaypoint)) {
			if (DirectionB.IndexOf(currentWaypoint) < DirectionB.Count - 1) {
				return DirectionB[DirectionB.IndexOf(currentWaypoint) + 1];
			} else {
				return chooseNextPath(currentWaypoint);

			}
				
		} else {
			Debug.Log("This should not be happening! The waypoint does not belong to this street");
			return null;
		}
	}
	public waypoint getClossedWaypoint(int layer,Vector3 pos){
		if(layer==0){
			//Debug.Log("Target Layer is A input is: " +layer);
			float distance = (DirectionA[0].position - pos).magnitude;
			int counter=0;
			int i=0;
			foreach (waypoint wp in DirectionA) {
				float NewDistance = (wp.position - pos).magnitude;
				if(distance>NewDistance){
					distance=NewDistance;
					counter=i;
				}
				i++;
			}
			return DirectionA[counter];

		}
		else if(layer==1){//if (layer>=1)
			//Debug.Log("Target Layer is B input is: " +layer);
			float distance = (DirectionB[0].position - pos).magnitude;
			int counter=0;
			int i=0;
			foreach (waypoint wp in DirectionB) {
				float NewDistance = (wp.position - pos).magnitude;
				if(distance>NewDistance){
					distance=NewDistance;
					counter=i;
				}
				i++;
			}
			return DirectionB[counter];

		}
		else return null;


	}
	public waypoint ReturnMirrorWaypoint(waypoint originalWaypoint,bool ghostDriver)
	{

		if (DirectionA.Contains(originalWaypoint)) {
			float distance = (DirectionB[0].position - originalWaypoint.position).magnitude;
			int counter=0;
			int i=0;
			foreach (waypoint wp in DirectionB) {
				float NewDistance = (wp.position - originalWaypoint.position).magnitude;
				if(distance>NewDistance){
					distance=NewDistance;
					counter=i;
				}
				i++;
			}
			return DirectionB[counter];
		} else if (DirectionB.Contains(originalWaypoint)) {
			float distance = (DirectionA[0].position - originalWaypoint.position).magnitude;
			int counter=0;
			int i=0;
			foreach (waypoint wp in DirectionA) {
				float NewDistance = (wp.position - originalWaypoint.position).magnitude;
				if(distance>NewDistance){
					distance=NewDistance;
					counter=i;
				}
				i++;
			}
			return DirectionA[counter];
			
		} else {
			Debug.Log("This should not be happening! The waypoint does not belong to this street");
			return null;
		}
		Debug.Log("This should not happen. something is wrong with the foreach loop");//I know this is very dirty!// fixing it that it compiles
		return null;
	}

	waypoint choosePreviousPath(waypoint currentWaypoint)
	{
		if (NextDirectionA != null && NextDirectionB != null) {
			waypoint returnCandidateA = NextDirectionA.getLastRelevant(currentWaypoint);
			waypoint returnCandidateB = NextDirectionB.getLastRelevant(currentWaypoint);
			float optionA = (returnCandidateA.position - currentWaypoint.position).magnitude;
			float optionB = (returnCandidateB.position - currentWaypoint.position).magnitude;

			if (optionA < optionB) {
				if (optionA < 1.5f) {// We try to jump over the first (double) waypoint 
					returnCandidateA = NextDirectionA.getPreviousWaypoint(returnCandidateA);
				}
				return returnCandidateA;
			} else if (optionA > optionB) {
				if (optionB < 1.5f) {
					returnCandidateB = NextDirectionB.getPreviousWaypoint(returnCandidateB);
				}
				return returnCandidateB;
			} else {
				Debug.Log("Again we have an equal distance between two candidate next points. This should not behappening /n please check your road network and the waypoints");
				return null;
			}
		} else if (NextDirectionA != null && NextDirectionB == null) {
			waypoint returnCandidate = NextDirectionA.getFirstRelevant(currentWaypoint);
			if ((returnCandidate.position - currentWaypoint.position).magnitude < 1.5f) {
				returnCandidate = NextDirectionA.getNextWaypoint(returnCandidate);
			} else if ((returnCandidate.position - currentWaypoint.position).magnitude > 25f) {
				returnCandidate = null;
			}
			return returnCandidate;
		} else if (NextDirectionA == null && NextDirectionB != null) {
			waypoint returnCandidate = NextDirectionB.getFirstRelevant(currentWaypoint);
			if ((returnCandidate.position - currentWaypoint.position).magnitude < 1.5f) {
				returnCandidate = NextDirectionB.getNextWaypoint(returnCandidate);
			} else if ((returnCandidate.position - currentWaypoint.position).magnitude > 25f) {
				returnCandidate = null;
			}
			return returnCandidate;
		} else {
			return null;
		}


	}

	waypoint chooseNextPath(waypoint currentWaypoint)
	{
		if (NextDirectionA != null && NextDirectionB != null) {
			waypoint returnCandidateA = NextDirectionA.getFirstRelevant(currentWaypoint);
			waypoint returnCandidateB = NextDirectionB.getFirstRelevant(currentWaypoint);
			float optionA = (returnCandidateA.position - currentWaypoint.position).magnitude;
			float optionB = (returnCandidateB.position - currentWaypoint.position).magnitude;

			if (optionA < optionB) {
				if (optionA < 1.5f) {// We try to jump over the first (double) waypoint 
					returnCandidateA = NextDirectionA.getNextWaypoint(returnCandidateA);
				}
				return returnCandidateA;
			} else if (optionA > optionB) {
				if (optionB < 1.5f) {
					returnCandidateB = NextDirectionB.getNextWaypoint(returnCandidateB);
				}
				return returnCandidateB;
			} else {
				Debug.Log("Again we have an equal distance between two candidate next points. This should not behappening /n please check your road network and the waypoints");
				return null;
			}
		} else if (NextDirectionA != null && NextDirectionB == null) {
			waypoint returnCandidate = NextDirectionA.getFirstRelevant(currentWaypoint);
			if ((returnCandidate.position - currentWaypoint.position).magnitude < 1.5f) {
				returnCandidate = NextDirectionA.getNextWaypoint(returnCandidate);
			} else if ((returnCandidate.position - currentWaypoint.position).magnitude > 25f) {
				returnCandidate = null;
			}
			return returnCandidate;
		} else if (NextDirectionA == null && NextDirectionB != null) {
			waypoint returnCandidate = NextDirectionB.getFirstRelevant(currentWaypoint);
			if ((returnCandidate.position - currentWaypoint.position).magnitude < 1.5f) {
				returnCandidate = NextDirectionB.getNextWaypoint(returnCandidate);
			} else if ((returnCandidate.position - currentWaypoint.position).magnitude > 25f) {
				returnCandidate = null;
			}
			return returnCandidate;
		} else {
			return null;
		}
	}

	// Update is called once per frame
	void Update()
	{
	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Compas : MonoBehaviour {

    public Vector3 NorthDirection;
    public Transform Player;
    public Quaternion Destination;

    public RectTransform Northlayer;
    public RectTransform Destinationlayer;
    public Transform destinplace;

	
	// Update is called once per frame
	void Update () {
        ChangeNorthDirection();
        ChangeDestination();
    }
    public void ChangeNorthDirection()
    {
        NorthDirection.z = Player.eulerAngles.y;
        Northlayer.localEulerAngles = NorthDirection;
    }
    public void ChangeDestination()
    {
        Vector3 dir = transform.position - destinplace.position;
        Destination = Quaternion.LookRotation(dir);
        Destination.z = -Destination.y;
        Destination.x = 0;
        Destination.y = 0;

        Destinationlayer.localRotation = Destination * Quaternion.Euler(NorthDirection);

    }
}

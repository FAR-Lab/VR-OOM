
using UnityEngine;
using System.Collections;


public class extArRotation : MonoBehaviour
{
	public Quaternion startOrientation;
	UDPReceive myReceiver;
    
    
	void Start(){
		
		myReceiver= GameObject.FindObjectOfType<UDPReceive>();
       
    }
    public bool setCallibrate()
    {
        if (myReceiver.markerUsable)/// if we find a usable marker we use it if not we reset and look for it the next second
        {
            startOrientation = myReceiver.marker;
            return true;
        }
        else { return false; }
    }
	void Update()
	{
		transform.localRotation = Quaternion.Inverse (startOrientation) * myReceiver.marker;
 
    }
   


}


using UnityEngine;
using System.Collections;


public class extCarMotionController : MonoBehaviour
{
	Quaternion startOrientation;
    public  Quaternion originalOrientation;
	public bool callibrate = true;
	UDPReceive myReceiver;
    Vector3 startPosition;
    float targetY;


    //Static Condition Variables;
    private bool isCalibrated;
    public  bool RealCarMotion = true;

    public InternalVehicleMotion CopyCar;
    Vector3 OffsetVector = new Vector3(0, 0, 0);
    /// <summary>
    ///  this implementation only works for flat surfaces 
    ///  it fixes the cars position to that Zvalue 
    ///  
    /// rotationally it allows us to do alot but we stay on the x-z plae
    /// </summary>
    // Invoked when a line of data is received from the serial device.
	void Start(){
        isCalibrated = false;
        myReceiver = GameObject.FindObjectOfType<UDPReceive>();
        targetY =  transform.position.y;
        startPosition = transform.position;
       // startOrientation = myReceiver.rotation;
        originalOrientation = transform.rotation;
        if (RealCarMotion) { startOrientation = myReceiver.rotation; }
        else
        {
            CopyCar = GameObject.FindObjectOfType<InternalVehicleMotion>() as InternalVehicleMotion;
            //startOrientation = CopyCar.transform.rotation;

        }

        StartCoroutine(DelayedClaibration(1f));
    }
	public void overwriteConditionStartPose(){
		GameObject.FindObjectOfType<ConditionManager>().ApplyPose(ref startPosition, ref originalOrientation);

	}
	void Update()
	{
        if (RealCarMotion)
        {
            if (callibrate)
            {
                //CopyCar.ReCallibrate();
                callibrate = false;
                isCalibrated = true;
                transform.rotation = originalOrientation;
                startOrientation = myReceiver.rotation;
                transform.position = startPosition;
            }
            transform.rotation = originalOrientation * Quaternion.Inverse(startOrientation) * myReceiver.rotation;
        }else {
            if (callibrate)
            {
                CopyCar.ReCallibrate(originalOrientation);
                callibrate = false;
                isCalibrated = true;
                transform.rotation = originalOrientation;
                startOrientation = CopyCar.transform.rotation;
                transform.position = startPosition;

                OffsetVector = transform.position - CopyCar.transform.position;
            }
            transform.rotation = originalOrientation * Quaternion.Inverse(startOrientation) * CopyCar.transform.rotation;


        }
 

        
	}
	public void CalibrateCar(){
		if (!callibrate) {
			callibrate = true;
		}
	}

    IEnumerator DelayedClaibration(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        CalibrateCar();
        if (RealCarMotion) GameObject.FindObjectOfType<copyPos>().DisableRotation();
        else GameObject.FindObjectOfType<copyPos>().EnableRotation();
        //Your Function You Want to Call
    }

    void FixedUpdate()
    {
        //transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0,(float)myReceiver.speed);
        if (isCalibrated)
        {
            if (RealCarMotion)
            {
                Vector3 newPos;
                if (myReceiver.forward)
                {
                    newPos = Vector3.Lerp(transform.position, transform.position + (transform.forward * ((float)myReceiver.speed)), Time.fixedDeltaTime);/// LAST NIGHT CHANGE  : Time.fixedDeltaTime
                }
                else
                {
                    newPos = Vector3.Lerp(transform.position, transform.position + (transform.forward * ((float)-myReceiver.speed)), Time.fixedDeltaTime);/// LAST NIGHT CHANGE  : Time.fixedDeltaTime

                }

                newPos.y = targetY;
                transform.position = newPos;
            }
            else
            {
                transform.position = CopyCar.transform.position + OffsetVector;
            }
        }
    }


}

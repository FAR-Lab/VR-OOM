using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using UnityEngine.XR;

public class seatCallibration : MonoBehaviour
{
    public HandModelBase HandModelL;
    public HandModelBase HandModelR;

    public Transform steeringWheelCenter;
    public Transform passangerCenter;


    public bool handTrackin = true;
    public bool arTracking = true;
    public bool forPassenger = true;
    public bool continuesHandTracking = true;

    float yRotationCorrectio = 0;
    float accumelatedYError=0;
    bool runCorrection;
    float prevCorrectRotation;
    Quaternion prevRot;
    float rotationTimer = 0;

   public float callibrationTimer = 2;
    float refreshTimer = 0;
    int callibrationState = 0;
   public bool callibrating;

    UDPReceive myReceiver;
    private Quaternion initalARRotation;
    public Transform headPose;
    public Transform markerPose;

    // Use this for initialization
    Vector3 handCenterPosition;
    private Vector3 OriginalPosition;
    public Vector3 offset;
    public Quaternion rotOld;
    public Quaternion driftCorrection;
    void Start()
    {
        myReceiver = GameObject.FindObjectOfType<UDPReceive>();

        if (myReceiver.markerUsable)
        {
            initalARRotation = headPose.rotation * Quaternion.Inverse(markerPose.rotation);
        }
        OriginalPosition = transform.position;




    }
	public void reCallibrate()
    {
		if(!callibrating){
        callibrationTimer = 0;
        callibrating = true;
        callibrationState = 0;
        runCorrection = false;
		}

    }

    void OnGUI()
    {
        GUIStyle gs = new GUIStyle();
        gs.fontSize = 30;
        GUI.Label(new Rect(610, 10, 600, 300), (accumelatedYError).ToString("F4") + "y Error&Diffy" + (yRotationCorrectio).ToString("F4"),gs);

    }

    // Update is called once per frame
    void Update()
    {
       // if (Input.GetKeyUp(KeyCode.Return))
        //{
       //     reCallibrate();
       //}
        if (callibrating)
        {
            bool setUp = false;
            if (callibrationTimer > 0)
            {
                callibrationTimer -= Time.deltaTime;
            }
            else
            {
                callibrationState++;
                setUp = true;
                Debug.Log(callibrationState);
            }

            if (forPassenger)
            {
                handCenterPosition = passangerCenter.position;
            }
            else
            {
                handCenterPosition = steeringWheelCenter.position;

            }

            if (callibrationState == 1)
            {
                if (setUp)
                {
                    callibrationTimer = 0;
                }
                transform.position= OriginalPosition;
                InputTracking.Recenter();
                //Quaternion rotation = Quaternion.(.eulerAngles, headPose.parent.transform.forward); ;
                Quaternion rotation = Quaternion.FromToRotation(headPose.forward, transform.parent.forward);

                headPose.parent.transform.Rotate(new Vector3(0, rotation.eulerAngles.y, 0));
                //headPose.parent.transform.rotation = transform.parent.rotation;
                //transform.rotation = transform.parent.rotation;

            }
            else if (callibrationState == 2)
            {
                if (handTrackin)
                {
                    if (setUp)
                    {
                        callibrationTimer = 10;
                        //    Debug.Break();
                    }
                    if (HandModelL.IsTracked && HandModelR.IsTracked)
                    {
                        Vector3 A = HandModelL.GetLeapHand().PalmPosition.ToVector3();
                        Vector3 B = HandModelR.GetLeapHand().PalmPosition.ToVector3();
                        Vector3 AtoB = B - A;
                        Debug.DrawLine(A, B);
                        if (steeringWheelCenter != null)
                        {
                            Vector3 transformDifference = (A + (AtoB * 0.5f)) - handCenterPosition;
                            Debug.DrawLine(transform.position, handCenterPosition);
                            offset = transformDifference;
                            transform.position -= transformDifference;

                            //  Quaternion rot = Quaternion.FromToRotation(AtoB, -steeringWheelCenter.right);
                            // rotOld = rot;
                            //Debug.Log(rot);
                            // transform.RotateAround(handCenterPosition, Vector3.up, rot.eulerAngles.y);

                        }

                    }

                    else
                    {
                        callibrationTimer += Time.deltaTime;
                    }
                }


            }
            else if (callibrationState == 3)
            {
                if (arTracking)
                {
                    if (markerPose.GetComponent<extArRotation>().setCallibrate())
                    {/// if we find a usable marker we use it if not we reset and look for it the next second

                        initalARRotation = headPose.rotation * Quaternion.Inverse(markerPose.rotation);
                        callibrationTimer = 0;
                    }
                    else
                    {
                        callibrationTimer = 1;
                    }
                }


            }
            else
            {
                callibrationTimer = 0;
                callibrating = false;
                callibrationState = 0;
            }
        }
        else
        {
            //  driftCorrection = initalARRotation * Quaternion.Inverse(myReceiver.marker * Quaternion.Inverse(headPose.rotation));\
            ///Adjusing the rotation over time isdifficult with markers as there is a long dela. It would be better to find adifferent solution.
            ///One approache we could explore is too look at the actual headset motion and have based on that two modes.
            ///either we are measuring the drifft that happend
            ///or
            ///we are corrrecting for the drifft all based on the speed of the motion from the headset. 
            if (arTracking)
            {
                if (myReceiver.markerUsable)
                {
                    driftCorrection = (initalARRotation * markerPose.rotation) * Quaternion.Inverse(headPose.rotation);
                    float temp = driftCorrection.eulerAngles.y;
                    if (temp > 180)
                    {
                        temp -= 360;
                    }

                    if (accumelatedYError > 30 && !runCorrection)
                    {
                        runCorrection = true;
                        prevCorrectRotation = headPose.parent.rotation.eulerAngles.y;
                        prevRot = headPose.parent.rotation;
                        rotationTimer = 0;
                    }
                    else
                    {
                        yRotationCorrectio = temp * 0.8f + 0.2f * yRotationCorrectio;
                        accumelatedYError = Mathf.Abs(temp) * 0.1f + 0.9f * accumelatedYError;

                    }

                    if (runCorrection)
                    {
                        rotationTimer += Time.deltaTime * 0.1f;
                        
                        
                        // Vector3 blub = new Vector3(headPose.parent.rotation.eulerAngles.x, Mathf.Lerp(prevCorrectRotation, yRotationCorrectio, rotationTimer), headPose.parent.rotation.eulerAngles.z);
                        //headPose.parent.Rotate(0, yRotationCorrectio, 0);
                        headPose.parent.rotation = Quaternion.Euler(headPose.parent.rotation.eulerAngles.x, Mathf.LerpAngle(headPose.parent.rotation.eulerAngles.y, headPose.parent.rotation.eulerAngles.y + yRotationCorrectio, rotationTimer), headPose.parent.rotation.eulerAngles.z);
                        //Quaternion.Lerp(prevRot, prevRot * Quaternion.Euler(0,yRotationCorrectio,0), rotationTimer);
                        //= Quaternion.Lerp(prevCorrectRotation, prevCorrectRotation * Quaternion.Euler(0, yRotationCorrectio, 0), rotationTimer);
                        //headPose.parent.Rotate(0f, yRotationCorrectio * .05f, 0f);
                    }
                    if (rotationTimer > 1)
                    {
                        runCorrection = false;

                    }
                }
                //driftCorrection = initalARRotation * (myReceiver.marker * Quaternion.Inverse(headPose.rotation*Quaternion.Inverse(transform.parent.rotation)));
                //driftCorrection.eulerAngles.Set(driftCorrection.eulerAngles.x, driftCorrection.eulerAngles.y-180f,driftCorrection.eulerAngles.z);
                //headPose.parent.Rotate(0f, Quaternion.Lerp(headPose.parent.rotation, driftCorrection, 0.10f).eulerAngles.y, 0f);
            }
            if (continuesHandTracking)
            {
                if (HandModelL.IsTracked && HandModelR.IsTracked)
                {
                    Vector3 A = HandModelL.GetLeapHand().PalmPosition.ToVector3();
                    Vector3 B = HandModelR.GetLeapHand().PalmPosition.ToVector3();
                    Vector3 AtoB = B - A;
                    Debug.DrawLine(A, B,Color.green);
                    if (steeringWheelCenter != null)
                    {
                        Vector3 transformDifference = (A + (AtoB * 0.5f)) - handCenterPosition;
                       
                        Vector3 carForward = transform.parent.forward;
                        carForward.y = 0;
                        Vector3 handForward = ((A + (AtoB * 0.5f)) - transform.position);
                        handForward.y = 0;

                        Debug.DrawLine(transform.position, transform.position+carForward*2,Color.red);
                        Debug.DrawLine(transform.position, transform.position + handForward*2, Color.green);
                        float Rot = Quaternion.FromToRotation(carForward, handForward).eulerAngles.y;
                        
                        if (Rot > 180)
                        {
                            Rot -= 360;
                        }
                        yRotationCorrectio = Rot;
                        headPose.parent.Rotate(new Vector3(0, -Rot/ 10f, 0));
                       // Debug.Log(Rot.eulerAngles.y);
                        //Debug.Break();
                    }
                    

                }

            }
        }
    }
}

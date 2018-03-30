using UnityEngine;
using System.Collections;

public class interaction : MonoBehaviour
{
    int state = 0;
	public Transform playerHead;
	public Collider stopPoint;
	public AIInput car;
	public Collider carCollider;
	public Collider carCollider2;

	public Collider centerCollider;

	float stateTwoCount=-1;
	float stateThreeCount=-1;
    float stateFourCounter = -1;

	bool eyecontactLastFrame=false;
	float eyecontactSum=-1;
	Vector3	 previosuPlayerPos;

	bool enteredCenter = false;
    // Use this for initialization
    void Start()
    {
		stateTwoCount = -1;
		stateThreeCount=-1;
         stateFourCounter = -1;

        stopFollower();
    }

    // Update is called once per frame
    void Update()
	{


		if (stateTwoCount > 0f) {
			stateTwoCount += Time.deltaTime;
		}
		if (stateThreeCount > 0f) {
			stateThreeCount += Time.deltaTime;
		}
        if (stateFourCounter > 0f)
        {
            stateFourCounter += Time.deltaTime;
        }

       // Debug.Log(stateTwoCount)
		if (state == 0) {/// driving/// start driving
           
            // we need to check if the driver car entered the collider

            if (stopPoint.bounds.Contains(car.transform.position)) { 
				stateTwoCount = 0.01f;
				eyecontactSum = 0;
				//car.targetSpeed = 0;
                car.desiredSpeed = 0;
				state = 1;
                transform.GetComponent<emergencyBreak>().setActive(false);
                Debug.Log("my target speed is:" + car.desiredSpeed);
                Debug.Log("Send the stop call waiting for eye site");

			}

			//if so we
			// first stop the car // Best speed ==0;
			// save current time to determin the 1-2 sec. delay in the next step.

		} else if (state == 1) { /// waiting

			//waiting for eyecontact
			//  delayed 1-2 sec.// start signaling crosswalk


            
			RaycastHit hit;
            int layer = 1 << 8;
            if (Physics.Raycast(playerHead.position, playerHead.forward, out hit, 150, layer)) {
                Debug.DrawLine(playerHead.position, hit.point);
             //   Debug.Log("we are raycasting lets see where we hit" + hit.transform.name);
				if (hit.collider == carCollider || hit.collider == carCollider2) {
					// we have eye contact

					if (eyecontactLastFrame) {
						eyecontactSum += Time.deltaTime;
                       // Debug.Log("Gathering eye time"+ eyecontactSum);

                    } else {
						eyecontactLastFrame = true;
					}
				}

			} else {

				eyecontactLastFrame = false;
			}

			if (stateTwoCount > 5) {
				// we had not enough eye contact we consider no interaction... and go on;
				state = 3;
				stateTwoCount = -1;
                Debug.Log("no contact proceding");

            } else if (eyecontactSum > 1) {
                Debug.Log("We got contact");
                state = 2;
				previosuPlayerPos = playerHead.position;
				eyecontactSum = 0;
                stateThreeCount = 0.01f;

                startDisplay();

			}
            if (centerCollider.bounds.Contains(playerHead.transform.position))
            {
                startFollower();
                startDisplay();
                state = 4;
                 //   enteredCenter = true;
            }
			//4 second bounds .. if after 4 seconds we still have less then 1 sec eye contact ray cast then we go to state=3; if at any point in time during these 4 secnds 
			// we have 1> seconds of eye contact we jump to state=2;

			//here we need to check if the person looks at the car i.e. execute a ray cast from the player object if that hits the cars collider
			//store starting position of the person vector3 pastPlayerPos;
			//again store the time of the transition

		} else if (state == 2) {/// eyecontact established
			//
			//waiting for movment for max 2 sec

			if (stateThreeCount > 6f) {

                Debug.Log("Waited for movment evaluating attempts");
                float distance = Mathf.Abs(playerHead.position.x - previosuPlayerPos.x);
				if (distance > 0.5) {
                    Debug.Log("Movement ... we wait");
                    state = 4;
					startFollower();
					enteredCenter = false;

				} else {
                    Debug.Log("No Movement ... so lets go");
                    state = 3;
					stateThreeCount = -1f;
				}


			}

			// once the timmer runs out we need to take a decision.
			// if the x-pos is less then .5 meters distance then the previousplayer.x then we say that the person did not started to move. go to state=3
			// start the car with a low speed
			// option: display a warning sign as in like I am about to go. 

			// however if its more or equal to .5 meters the person started to move...  go to state 4



		} else if (state == 3) { //no movment //no eye contact // interaction done
            Debug.Log("setting everything to go again");
            car.startTheCar();
            car.desiredSpeed = 6;

			stopDisplay();
			stopFollower();
			state = 5;
            transform.GetComponent<emergencyBreak>().setActive(true);

            //the car slowley drives towards the end of the street

            // once it leafes the avoidance collider it should speed up 10 -  15  
        } else if (state == 4) {// movment 



			if (centerCollider.bounds.Contains(playerHead.position) && stateFourCounter<=0) {
				if (!enteredCenter) {
					enteredCenter = true;

				} 
			} else {
				if (enteredCenter) {
					//state = 3;
                    stateFourCounter = 0.01f;
                    enteredCenter = false;
				}
			}
           if(stateFourCounter > 3)
            {

                if (centerCollider.bounds.Contains(playerHead.position))
                {
                    enteredCenter = true;
                    stateFourCounter = -1;
                }
                else
                {
                    stateFourCounter = -1;
                    state = 3;
                }

            }

            //keep displaying interaction pattern
            //turn on ids to indicate that they are aknowleged
            //wait untill they leave the center collider

            //1st chek if they have ever entered the center collider (use boolean)
            //2nd wait for the player to leave the center colider
            //3rd go to state 3 and start driving slowley

        } else if (state == 5) { // start driving slowly 
			// indicate driving start 
			//drive slowley
			if(! centerCollider.bounds.Contains(car.transform.position)){
				car.desiredSpeed = 6;
				state = 6;
			}



		} else if (state == 6) {
			//get up to speed and start up

			state = 0;
			stateTwoCount = -1;
			stateThreeCount=-1;
			 eyecontactLastFrame=false;
			 eyecontactSum=-1;
			previosuPlayerPos=Vector3.zero;
			enteredCenter = false;
		}

	}
	void startDisplay(){
        car.transform.Find("crosswalkLight").transform.gameObject.SetActive(true);
        Debug.Log("turning on the light");
	}
	void stopDisplay(){
        car.transform.Find("crosswalkLight").transform.gameObject.SetActive(false);
        Debug.Log("turning OFF the light");
    }
	void startFollower(){

        playerHead.transform.Find("lightBeamProjector").transform.GetComponent<Projector>().enabled = true;
        

    }
	void stopFollower(){
        playerHead.transform.Find("lightBeamProjector").transform.GetComponent<Projector>().enabled = false;
    }
}

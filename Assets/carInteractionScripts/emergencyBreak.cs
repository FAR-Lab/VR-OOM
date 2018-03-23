using UnityEngine;
using System.Collections;
using RVP;

public class emergencyBreak : MonoBehaviour
{
    public Collider pedestrianDetector;
    public Collider carCaller;
    public Transform player;
    public Transform car;

	bool active=true;
    // Use this for initialization
    void Start()
    {

    }
	public void setActive(bool incomming){
		active=incomming;
	}

    // Update is called once per frame
    void Update()
    {

		if(active){
        if (Input.GetKeyDown("s")) car.GetComponent<AIInput>().stopTheCar();

        if (Input.GetKeyDown("x")) car.GetComponent<AIInput>().startTheCar();

        if (carCaller.bounds.Contains(car.position))
        {
            if (pedestrianDetector.bounds.Contains(player.position))
            {
               // Debug.Log("called Stop");
                car.GetComponent<AIInput>().stopTheCar();
            }
            else
            {
               // Debug.Log("called Start");
                car.GetComponent<AIInput>().startTheCar();
            }

        }

    }
	}
}

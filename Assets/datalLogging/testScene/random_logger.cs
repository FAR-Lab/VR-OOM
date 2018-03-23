using UnityEngine;
using System.Collections;

public class random_logger : MonoBehaviour {
	double timer;
	public float myValue=0.1f;

	// Use this for initialization
	void Start () {
		//GameObject.Find("GameManager").GetComponent<logger>().add_Variable_Logger(0.5f,this,"myValue");

	}
	
	// Update is called once per frame
	void Update () {
		myValue++;

		if(timer<0){
		

		
			timer=2.0;
		
		}
		else{
			timer-=Time.deltaTime;
		}
		
	}
}

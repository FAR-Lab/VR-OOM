using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class auto_logger:MonoBehaviour {
	/*List<GameObject> listener_Objects = new List<GameObject>(); 
	// Use this for initialization
	void Start () {
		//gameManager=GameObject.Find("GameManager").GetComponent<logger>();

	}
	void add_Variable_Logger(float wait_Time,GameObject origin, string valueName){
		listener_Objects.Add(origin);
		int idd=listener_Objects.LastIndexOf(origin);

		StartCoroutine(log_variable(wait_Time,valueName,idd));
	}


	public IEnumerator log_variable(float wait_Time, string valueName, int IDD){
		logger gameManager=GameObject.Find("GameManager").GetComponent<logger>();
		log_message temp_Message;

		temp_Message.origin=listener_Objects[IDD].name.ToString();
		temp_Message.valueName=valueName;
	

		while(gameManager!=null){
			temp_Message.time=Time.time.ToString();

			yield return new WaitForSeconds(wait_Time);
		}




	}
	// Update is called once per frame
	void Update () {
	
	}
	*/
}

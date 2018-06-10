using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum sceneState { SCENESTATE_EMPTY, SCENESTATE_NORMAL, SCENESTATE_CON1, SCENESTATE_CON2 };
public class experimentController : MonoBehaviour {
    
    // Use this for initialization
    
    public string Normal;
    public string Condition1;
    public string Condition2;
    public sceneState currentState;
	string ControllerInfo;

	bool overWriteSpeed;
	float speedValue;
	UDPReceive updController;
	IEnumerator coroutine;
    void Start () {
        currentState = sceneState.SCENESTATE_EMPTY;
		ControllerInfo = "";
		updController=GameObject.FindObjectOfType<UDPReceive>();

    }
     void Update()
    {
        //Input.GetJoystickNames();
       // Debug.Log(Input.GetAxis("Horizontal"));
        //Debug.Log(Input.GetJoystickNames()[0].ToString());
        
    }


    void resetAtSceneStart(){
		GameObject.FindObjectOfType<interactingObjectDoor>().resetDoor();
		/// 

	}
    // Update is called once per frame
    void OnGUI()
    {
        bool loadNextScene = false;
        sceneState nextState = sceneState.SCENESTATE_EMPTY;
        int yPos = 100;
		int height = 30;
		int width = 150;
        
		if (GUI.Button(new Rect(10, yPos+=height, width, height), "ResetCar"))
		{
			GameObject.FindObjectOfType<extCarMotionController>().CalibrateCar();
			ControllerInfo += "Resetting CarPosition";
		}

		if (GUI.Button(new Rect(10, yPos+=height, width, height), "Callibrate Seat"))
		{
			GameObject.FindObjectOfType<seatCallibration>().reCallibrate();
			ControllerInfo += "Callibrating Seat";
		}
        if (GUI.Button(new Rect(10, yPos += height, width, height), "findGPSPosition"))
        {
            GameObject.FindObjectOfType<extCarMotionController>().estimateCarPositionWithGPS=true;
            ControllerInfo += "find GPS Position";
        }



        if (GUI.Button(new Rect(10, yPos+=height, width, height), "Change to: "+Normal))
        {
        loadNextScene = true;
        nextState = sceneState.SCENESTATE_NORMAL;
        }
  
		if (GUI.Button(new Rect(10, yPos+= height, width, height), "Change to: " + Condition1))
        {
            loadNextScene = true;
            nextState = sceneState.SCENESTATE_CON1;
        }
        
		if (GUI.Button(new Rect(10, yPos+=height, width, height), "Change to: " + Condition2))
        {
            loadNextScene = true;
            nextState = sceneState.SCENESTATE_CON2;
        }
		if (updController.forward) {
			if (GUI.Button(new Rect(10, yPos += height, width, height), "Go Backwards")) {
				updController.forward = false;
			}
		} else {
			if (GUI.Button(new Rect(10, yPos += height, width, height), "Go Forwards")) {
				updController.forward = true;
			}

		}
		if (GUI.Button(new Rect(10, yPos += height, width, height), "Control Speed")) {
			overWriteSpeed = !overWriteSpeed;
			if (overWriteSpeed) {
				speedValue = (float)updController.speed;
			}
		}
		if (overWriteSpeed) {
			GUIStyle style = new GUIStyle();
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 16;

			GUI.Label(new Rect(10, yPos += height, width, height), (speedValue*3.6f).ToString("F2")+"km/h",style);
			speedValue = GUI.HorizontalSlider(new Rect(10, yPos += height, width, height), speedValue, 0.0F, 22.0F);
			updController.speed=(double)speedValue;
		}

		if (GUI.Button(new Rect(10, yPos+=height, width, height), "Open Door"))
		{
			GameObject.FindObjectOfType<interactingObjectDoor>().openDoor();
		}
		if (GUI.Button(new Rect(10, yPos+=height, width, height), "Close Door"))
		{
			GameObject.FindObjectOfType<interactingObjectDoor>().closeDoor();
		}
		if (GUI.Button(new Rect(10, yPos+=height, width, height), "Reset Door"))
		{
			GameObject.FindObjectOfType<interactingObjectDoor>().resetDoor();
		}

        if (loadNextScene) {
                // first we unload

            switch (currentState)
            {   case sceneState.SCENESTATE_EMPTY:
                    break;
                case sceneState.SCENESTATE_NORMAL:
					SceneManager.UnloadSceneAsync(Normal);
                    break;
                case sceneState.SCENESTATE_CON1:
				SceneManager.UnloadSceneAsync(Condition1);
           			 break;
                case sceneState.SCENESTATE_CON2:
				SceneManager.UnloadSceneAsync(Condition2);
           		 break;
            }

		
			log_message sceneUpdateMessage =  new log_message();
			sceneUpdateMessage.origin = transform.name;
			sceneUpdateMessage.time = Time.time;
			sceneUpdateMessage.valueName = "loadScene";
			AsyncOperation currentOpperation;
            switch (nextState) // then we load the next one
            {
				case sceneState.SCENESTATE_EMPTY:
					sceneUpdateMessage.value = 0;
					sceneFinishedLoading();
                    break;
                case sceneState.SCENESTATE_NORMAL:
					sceneUpdateMessage.value = 1;
					currentOpperation= SceneManager.LoadSceneAsync(Normal,LoadSceneMode.Additive);
					coroutine = reportFinished(currentOpperation);
					StartCoroutine(coroutine);
                    break;
                case sceneState.SCENESTATE_CON1:
					sceneUpdateMessage.value = 2;
					currentOpperation=SceneManager.LoadSceneAsync(Condition1, LoadSceneMode.Additive);
					coroutine = reportFinished(currentOpperation);
					StartCoroutine(coroutine);
                    break;
                case sceneState.SCENESTATE_CON2:
					sceneUpdateMessage.value = 3;
					currentOpperation=SceneManager.LoadSceneAsync(Condition2, LoadSceneMode.Additive);
					coroutine = reportFinished(currentOpperation);
					StartCoroutine(coroutine);
                    break;
            }
			transform.GetComponent<logger>().LogMessage(sceneUpdateMessage);
        // last step is remebering our new state
        currentState=nextState;



        }

    }
	void sceneFinishedLoading(){
		Debug.Log("we Finished loading");
	}
	IEnumerator reportFinished(AsyncOperation loadingOpperation){

		while (! loadingOpperation.isDone) {

			Debug.Log(loadingOpperation.progress);
			yield return new WaitForEndOfFrame();


		}
			sceneFinishedLoading();
	}
}

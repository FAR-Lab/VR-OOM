using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;


public class logger : MonoBehaviour{

    //unique GameObjects
    UDPReceive canObject;
    Camera mainCam;

    //public Variables
	public bool doLog = true;
	public int type=0; // logging method (0=text file;1=MQTT;2=UDP)
	public string ipAddress="localhost";
	public int port = 8134;
	private MqttClient client;



	private System.Guid myGUID; // for multiple clients we generate a GUID 
	int publishCount = 0;
	List<log_message> listener_Objects = new List<log_message>(); // Main queue of messages to be send
	double short_Timer	= 0;  //periodic automatic timming
	double medium_Timer	= 0;
	double long_Timer	= 0;
	bool IsLogging=false;
	public GameObject player;

    double timer = 0;
    Queue log_messages = new Queue();
    bool keepRunning = true;
    StreamWriter sw;

    public UdpClient logSender;
    IPEndPoint  remoteEndPoint;

    
    void Start ()
	{
        //LogitechGSDK.LogiSteeringInitialize(false);
        myGUID = System.Guid.NewGuid();
        canObject = GameObject.FindObjectOfType<UDPReceive>(); // should be only one in the scene 
        mainCam = GameObject.FindObjectOfType<Camera>();
        if (doLog && type == 0) {
			sw = new StreamWriter ( DateTime.Now.ToString ().Replace ('/', '_').Replace(' ','_').Replace(':','_') + ".csv");
            sw.WriteLine ("Time,origin,valuename,value,queuedMessageCount");
			string time = (Time.time / 60) + ":" + (Time.time % 60);
			sw.WriteLine (time + "," + transform.name + ",sceneName," + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name);
			sw.AutoFlush = true;
			IsLogging = true;
			StartCoroutine (writeToFile ());

		} else if (doLog && type == 1) {
			IsLogging = true;
			logData (this.transform, 0f, "sceneName: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name);
			IPHostEntry host;
			host = Dns.GetHostEntry (ipAddress);
			client = new MqttClient (host.AddressList [0], port, false, null); 
			client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
			string clientId = Guid.NewGuid ().ToString (); 
			client.Connect (clientId); 
			StartCoroutine (writeToMQTT ());
		} else if (doLog && type == 2) {
			Application.ExternalCall ("WebSocketTest");
			IsLogging = true;
			logData (this.transform, 0f, "sceneName: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name);
			StartCoroutine (writeToWebsocket ());
		}
        else if(doLog && type == 3)
        {
            IsLogging = true;
            logData(this.transform, 0f, "sceneName: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            logSender=new UdpClient();
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            StartCoroutine(writeToUDP());

        }


	}
void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
{ 
	Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)); //The logger is not (yet) design to handle any external data input
} 

	public void LogMessage (log_message incomming_log_message)// for additional log messages
	{
		log_messages.Enqueue (incomming_log_message);
	}

	
	// Update is called once per frame
	void Update ()
	{
		short_Timer -= Time.deltaTime;
		medium_Timer -= Time.deltaTime;
        long_Timer -= Time.deltaTime;
        if (short_Timer < 0) { short_Timer = 0.1;
            logData(player.transform, player.transform.position.x, "xPos");
            logData(player.transform, player.transform.position.y, "yPos");
            logData(player.transform, player.transform.position.z, "zPos");
            logData(player.transform, player.transform.rotation.x, "Qx");
            logData(player.transform, player.transform.rotation.y, "Qy");
            logData(player.transform, player.transform.rotation.z, "Qz");
            logData(player.transform, player.transform.rotation.w, "Qw");
            logData(transform, (1.0f / Time.deltaTime), "frameRate");
            /*if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);
                logData(transform, (float)rec.lX, "SteeringWheelInput");

               // Debug.Log("x-axis" + (float)rec.lX);
            }*/

            if (player.GetComponent<seatCallibration>() != null)
            {
                logData(player.transform, player.GetComponent<seatCallibration>().offset.x, "xOffset");
                logData(player.transform, player.GetComponent<seatCallibration>().offset.y, "yOffset");
                logData(player.transform, player.GetComponent<seatCallibration>().offset.z, "zOffset");
                logData(player.transform, player.GetComponent<seatCallibration>().rotOld.x, "QOffsetx");
                logData(player.transform, player.GetComponent<seatCallibration>().rotOld.y, "QOffsety");
                logData(player.transform, player.GetComponent<seatCallibration>().rotOld.z, "QOffsetz");
                logData(player.transform, player.GetComponent<seatCallibration>().rotOld.w, "QOffsetw");
            }
            if (mainCam != null)
            {
                logData(mainCam.transform, mainCam.transform.localPosition.x, "xPos");
                logData(mainCam.transform, mainCam.transform.localPosition.y, "yPos");
                logData(mainCam.transform, mainCam.transform.localPosition.z, "zPos");
                logData(mainCam.transform, mainCam.transform.localRotation.x, "Qx");
                logData(mainCam.transform, mainCam.transform.localRotation.y, "Qy");
                logData(mainCam.transform, mainCam.transform.localRotation.z, "Qz");
                logData(mainCam.transform, mainCam.transform.localRotation.w, "Qw");


            }
            //  logData(this.transform, Time.timeScale, "timeScale");
            if (canObject != null){ 
            logData(canObject.transform, (float)canObject.speed, "CANSpeed");

                logData(canObject.transform, canObject.rotation.x, "car-Qx");
                logData(canObject.transform, canObject.rotation.y, "car-Qy");
                logData(canObject.transform, canObject.rotation.z, "car-Qz");
                logData(canObject.transform, canObject.rotation.w, "car-Qw");
                logData(canObject.transform, (float)canObject.IMUQuality, "IMUQuality");



                logData(canObject.transform, canObject.marker.x, "marker-Qx");
                logData(canObject.transform, canObject.marker.y, "marker-Qy");
                logData(canObject.transform, canObject.marker.z, "marker-Qz");
                logData(canObject.transform, canObject.marker.w, "marker-Qw");
                if (canObject.markerUsable)
                {
                    logData(canObject.transform, 1.0f, "markerUsable");
                }
                else
                {
                    logData(canObject.transform,    0f, "markerUsable");
                }
            }
		}
		if (medium_Timer < 0) {medium_Timer = 0.75;
		}
		if (long_Timer < 0) {long_Timer = 2.53;
			//logData (transform,(1.0f / Time.deltaTime), "frameRate");
		}
		
	}
	public void logData (Transform origin, float value, string valueName)
	{
		log_message temp_Message = new log_message ();
		temp_Message.origin = origin.name;
		temp_Message.valueName = valueName;
		temp_Message.time = Time.time;
		temp_Message.value = value;
		LogMessage (temp_Message);
	}
     /// <summary>
     /// /old code ... no idea what was happening here about to be removed ....
     /// </summary>
	/*public void add_Variable_Logger (float wait_Time,MonoBehaviour origin, string valueName)
	{
		log_message temp_Message = new log_message();

		temp_Message.origin =origin.name;
		temp_Message.valueName = valueName;
		temp_Message.time = Time.time.ToString ();
		temp_Message.originMONO=origin;
		listener_Objects.Add(temp_Message);
		int idd = listener_Objects.LastIndexOf(temp_Message);

		//StartCoroutine (log_variable (wait_Time, idd));
	}
*/
	/*void log_Point (int IDD)
	{
		Debug.Log(listener_Objects.Count);
		log_message temp_Message = listener_Objects[IDD];
		Debug.Log(temp_Message.valueName);
		Debug.Log(temp_Message.originMONO.GetInstanceID());

		//Debug.Log(typeof(MonoBehaviour).GetField(temp_Message.valueName).GetValue(temp_Message.originMONO));
		//temp_Message.time = Time.time.ToString ();
		//temp_Message.value=(float)typeof(MonoBehaviour).GetProperty(temp_Message.valueName).GetValue(temp_Message.originMONO,null);
			

	//	Debug.Log (temp_Message.value);
	

	}*/

	public void halt(){// no data is logged any kind of calls are discarded 
		IsLogging=false;
	}
	public void RestartRunning(string DataLogMessage){
		IsLogging=true;

	}
	IEnumerator writeToFile ()
	{while (keepRunning) {
			Boolean firstNewEntry = true;
			while (log_messages.Count > 0) {
				if (firstNewEntry) {
					firstNewEntry = false;
					//sw.WriteLine ("––––newEntry period––––");
				}
				log_message temp_message = (log_message)log_messages.Dequeue ();
				string time =  ((int)temp_message.time / 60) + ":" + (temp_message.time % 60);/// this needs to be debuged I think might caus eproblems 
				sw.WriteLine (time + ',' + temp_message.origin + ',' + temp_message.valueName + ',' + temp_message.value.ToString ()+','+log_messages.Count);
			}
			if (!firstNewEntry) {
				sw.Flush ();}
			yield return new WaitForSeconds (0.5f);
		}
	}
	IEnumerator writeToMQTT ()
	{
		Debug.Log ("trolly/" + myGUID.ToString ());
		while (keepRunning) {
			Boolean firstNewEntry = true;
			while (log_messages.Count > 0) {
				if (firstNewEntry) {
					firstNewEntry = false;
					
				}
                publishCount++;
                simplifiedLogMessage temp_message = simplify((log_message)log_messages.Dequeue());
                //string time =  ((int)temp_message.time / 60) + "." + (temp_message.time % 60);
                client.Publish("trolly/" + myGUID.ToString(), System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(temp_message).ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

				

			}
			yield return new WaitForSeconds (0.5f);
			
		}
	}

    IEnumerator writeToUDP()
    {
        Debug.Log("trolly/" + myGUID.ToString());
        while (keepRunning)
        {
            Boolean firstNewEntry = true;
            while (log_messages.Count > 0)
            {
                if (firstNewEntry)
                {
                    firstNewEntry = false;

                }

                try
                {
                    simplifiedLogMessage temp_message = simplify((log_message)log_messages.Dequeue());
                    string test = "trolly/" + myGUID.ToString() +"_"+ JsonUtility.ToJson(temp_message).ToString();
                    byte[] data = Encoding.UTF8.GetBytes(test);
                    logSender.Send(data, data.Length, remoteEndPoint);
                    publishCount++;

                }
                catch (Exception err)
                {
                    print(err.ToString());
                }

            }
            yield return new WaitForSeconds(0.5f);

        }
    }


    IEnumerator writeToWebsocket ()
	{
		Debug.Log ("trolly/" + myGUID.ToString ());
		while (keepRunning) {
			Boolean firstNewEntry = true;
			while (log_messages.Count > 0) {
				if (firstNewEntry) {
					firstNewEntry = false;
				}
				publishCount++;
				simplifiedLogMessage temp_message = simplify((log_message)log_messages.Dequeue());
				Application.ExternalCall ("sendStuff", "trolly/" + myGUID.ToString () + "_" + JsonUtility.ToJson (temp_message).ToString ());
			}
			yield return new WaitForSeconds (0.5f);
		}
	}
	void OnApplicationQuit ()
	{Debug.Log ("Final logger publish Count: "+publishCount);
		Debug.Log ("End Time: " + Time.time);
		keepRunning = false; // stopping the writing processes
		if (doLog && type==0) {
			sw.WriteLine (Time.time + ", ––––FILE_END––––");
			sw.Close ();
		}
	}
	simplifiedLogMessage  simplify(log_message incomming){

		simplifiedLogMessage temp = new simplifiedLogMessage();
		temp.time=incomming.time;
		temp.valueName=incomming.origin+"-"+incomming.valueName;
		temp.value=incomming.value;
		return temp;
	}

	void onGUI(){
		GUI.Label (new Rect (50, 50, 200, 15), "published Messages: "+publishCount);
	}

}


/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 192.168.56.1 : 8051
   
    // send
    // nc -u 192.168.56.1 8051
 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    
	//public varables recieved through the CAN BUS
	public double speed;
	public bool forward=true;
    public bool useOBD2;
	public Quaternion rotation =  Quaternion.identity;
    public bool markerUsable;
    public Quaternion marker;
    public Vector3 markerRotationTEMP;
    Quaternion prevMarker;
    float angle;

    public volatile bool running = true;
	// receiving Thread
	Thread speedReceiveThread;
	Thread rotationReceiveThread;
    Thread markerReceiveThread;
   public int IMUQuality=0;
    private bool markerReceived;
    float markerTimer;

	public int speedPort = 5000;
	// define > init
	public int rotationPort = 5001;
    public int markerPort = 5002;

    public UdpClient speedClient;
	public UdpClient rotationClient;
    public UdpClient markerClient;

    public void Start ()
	{
		running = true;

		speedClient = new UdpClient (speedPort);
		speedReceiveThread = new Thread (this.ReceiveData);
		speedReceiveThread.IsBackground = true;
		speedReceiveThread.Start (speedClient);

		rotationClient = new UdpClient (rotationPort);
		rotationReceiveThread = new Thread (this.ReceiveData);
		rotationReceiveThread.IsBackground = true;
		rotationReceiveThread.Start (rotationClient);

        markerClient = new UdpClient(markerPort);
        markerReceiveThread = new Thread(this.ReceiveData);
        markerReceiveThread.IsBackground = true;
        markerReceiveThread.Start(markerClient);
		rotation =  Quaternion.identity;
    }

	// OnGUI
	void OnGUI ()
	{
		GUIStyle gs = new GUIStyle ();
		gs.fontSize = 30;
		GUI.Label (new Rect (10, 10, 600, 300), speed.ToString ("F2") + "m/s \n" + (speed * 3.6).ToString ("F2") + "km/h\nIMUQuality: "+IMUQuality+"tracking angle:"+angle.ToString(), gs);

		// init
	}
    void Update()
    {
        if (markerTimer > 0.060)
        {
            markerUsable = false;
        }
        else
        {
            markerUsable = true;
        }
        
        if (markerReceived)
        {
            markerReceived = false;
            markerTimer = 0;


        }
        else
        {
            markerTimer += Time.deltaTime;
        }


    }


	private void ReceiveData (object portin)
	{
		
		UdpClient client = (UdpClient)portin;

		while (running) {
			//Debug.Log (client.Client.Available);
			try {

				IPEndPoint anyIP = new IPEndPoint (IPAddress.Any, 0);

			
				byte[] data = client.Receive (ref anyIP);
			


				// Bytes mit der UTF8-Kodierung in das Textformat kodieren.
				string text = Encoding.UTF8.GetString (data);
                //Debug.Log(text);
                if (client == speedClient) {
                    //Debug.Log(text);
                    if (!useOBD2) {
                        string msgID = text.Substring(0, 4);
                        if (msgID.Equals("00B6")) {
                            string str_speed = text.Substring(4, 5);
                            speed = ((double)Int32.Parse(str_speed)) / 36.0d;
                            //Debug.Log(speed);
                        }
                    }
                    else
                    {
                        speed = ((double)Int32.Parse(text)) / 1.609d / 3.6d;  // We changed this here 
                        //speed = (speed * 0.2d) + (0.8d * ((double)Int32.Parse(text)) / 1.609d / 3.6d);  // We changed this here 

                    }

                } else if (client == rotationClient) {
                    char[] spl = { ',' };
                    string[] blub = text.Split(spl, 5);
                   
                    if (blub.Length>4) { 
                    IMUQuality = int.Parse(blub[4]); }
                    if (IMUQuality >= 0) {
                        // rotation =Quaternion.Lerp(rotation, new Quaternion(float.Parse(blub[1]), float.Parse(blub[2]), -float.Parse(blub[0]), -float.Parse(blub[3])),0.5f); /// use this for the BNO055
                        rotation = Quaternion.Lerp(rotation, new Quaternion(float.Parse(blub[3]), float.Parse(blub[2]), float.Parse(blub[0]), float.Parse(blub[1])), 0.5f);

                    }
                }
                else if (client == markerClient)
                {
                    char[] spl = { ',' };
                    string[] blub = text.Split(spl, 4);
                    Quaternion temp = new Quaternion(-float.Parse(blub[0]), float.Parse(blub[1]), float.Parse(blub[2]), float.Parse(blub[3]));
                    angle = temp.eulerAngles.y;
                    prevMarker = temp;
                    markerRotationTEMP = temp.eulerAngles;
                    if (temp.eulerAngles.y > 196)
                    {
                        marker = Quaternion.Lerp(temp, marker, 0.5f);
                        markerReceived = true;
                    }
                 
                }
            } catch (Exception err) {
				print (err.ToString ());
			}
		}
		Debug.Log ("Going to stop receving data");
		client.Close ();

	}



	void OnDestroy ()
	{
		Debug.Log ("Destroying the UDP receiver");

		running = false;
		speedClient.Close ();
		rotationClient.Close ();
        markerClient.Close();
		int i = 0;
		while (speedReceiveThread.IsAlive || rotationReceiveThread.IsAlive || markerReceiveThread.IsAlive) {
			speedReceiveThread.Join ();
			rotationReceiveThread.Join ();
            markerReceiveThread.Join();
            Debug.Log ("Waiting to close both threads");
			i++;

			if (i > 5000) {
				Debug.Log ("this is not working");
				break;
			}

		}

		speedReceiveThread.Abort ();
		rotationReceiveThread.Abort ();
        markerReceiveThread.Abort();
    }

}
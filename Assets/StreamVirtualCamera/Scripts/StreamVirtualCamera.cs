using System.Collections.Generic;
using UnityEngine;
using System.Threading;
//using System.Threading.Tasks;
using System.Net.Sockets;

[RequireComponent(typeof(Camera))]
public class StreamVirtualCamera : MonoBehaviour {

	public int quality = 30;
	public string endpointIP = "127.0.0.1";
	public int endpointPort = 11235;
	public int width = 640;
	public int height = 480;
	public float sendFPS = 10.0f;
	private float _lastFrame = 0.0f;
	private RenderTexture _rt;
	
    
	//private Task _sendTask;
	private Thread _sendThread;
	private readonly System.Object _sendQueueLock = new System.Object();
	private Queue<byte[]> _sendQueue;
	private UdpClient _udpClient;
	private bool _running;
	private Texture2D tex;

    private bool readyToEncode = false;

	public void OnApplicationQuit() {
		_running = false;
		GetComponent<Camera>().targetTexture = null;
		RenderTexture.active = null;
		DestroyImmediate(_rt);
		if (_udpClient != null) {
			try {
				_udpClient.Close();
			} catch (System.Exception e) {
				Debug.Log("Faild to close gracefully");
			}
		}
		_sendThread.Join(500);
	}
	
	void Start () {
        
		_rt = new RenderTexture(width, height, 24);
		tex = new Texture2D(width, height, TextureFormat.RGBAHalf, false);
		GetComponent<Camera>().targetTexture = _rt;
		GetComponent<Camera>().Render();
		RenderTexture.active = _rt;
		
		_sendQueue = new Queue<byte[]>();
		//_sendTask = Task.Run(() => DataSender());
		_sendThread = new Thread(DataSender);
		_sendThread.Start();
	}
	
	// Update is called once per frame
	void OnPostRender () {
		if (_lastFrame + (1.0f / (sendFPS/2)) <= Time.time) {
            _lastFrame = Time.time;
            if (!readyToEncode)
            {
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                readyToEncode = true;
            }
            else
            {
                readyToEncode = false;
                byte[] encodedTexture = tex.EncodeToJPG(quality);
                lock (_sendQueueLock)
                {
                    _sendQueue.Enqueue(encodedTexture);
                }
            }
		}
	}
	
	//private async Task DataSender() {
	private void DataSender() {
		_running = true;
		_udpClient = new UdpClient(endpointIP, endpointPort);
		Debug.Log("Streaming to: " + endpointIP+":"+ endpointPort);
		while (_running) {
			byte[] sendBytes = null;
			lock (_sendQueueLock) {
				if (_sendQueue.Count > 0) {
					sendBytes = _sendQueue.Dequeue();
				}
			}
			
			if (sendBytes != null) {
				//await _udpClient.SendAsync(sendBytes, sendBytes.Length);
				_udpClient.Send(sendBytes, sendBytes.Length);
			}
		}
		_udpClient.Close();
	}
}

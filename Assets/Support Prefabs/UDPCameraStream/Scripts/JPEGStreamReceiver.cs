using System.Collections.Generic;
using UnityEngine;
using System.Threading;
//using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

[RequireComponent(typeof(MeshRenderer))]
public class JPEGStreamReceiver : MonoBehaviour {

    public int width = 640;
    public int height = 480;
    
    public int listenPort = 11235;
    private MeshRenderer renderer;
    
    //private Task _listenTask;
    private Thread _listenThread;
    private readonly System.Object _listenQueueLock = new System.Object();
    private Queue<byte[]> _listenQueue;
    private UdpClient _udpClient;
    private bool _running;

    public void Start() {
        renderer = GetComponent<MeshRenderer>();
        renderer.material.mainTexture = Texture2D.blackTexture;
        _listenQueue = new Queue<byte[]>();
        //_listenTask = Task.Run(() => DataListener());
        _listenThread = new Thread(DataListener);
        _listenThread.Start();
    }

    public void OnApplicationQuit() {
        _running = false;
        if (_udpClient != null) {
            try {
                _udpClient.Close();
            } catch (System.Exception e) {
                Debug.Log("Faild to close gracefully");
            }
        }
        _listenThread.Join(500);
    }

    public void Update() {
        byte[] data = null;
        lock (_listenQueueLock) {
            if (_listenQueue.Count > 0) {
                data = _listenQueue.Dequeue();
            }
        }

        if (data != null) {
            Texture2D newFrame = new Texture2D(height, width, TextureFormat.RGBAHalf, false);
            newFrame.LoadImage(data);
            if (renderer != null) {
                renderer.material.mainTexture = newFrame;
            }
        }
    }

    //private async Task DataListener() {
    private void DataListener() {
        _running = true;
        IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, listenPort);
        _udpClient = new UdpClient(localEndpoint);
        Debug.Log("Client listening on " + listenPort);
        while (_running) {
            //UdpReceiveResult receivedResults = await _udpClient.ReceiveAsync();
            byte[] buffer = _udpClient.Receive(ref localEndpoint);
            lock (_listenQueueLock) {
                //_listenQueue.Enqueue(receivedResults.Buffer);
                _listenQueue.Enqueue(buffer);
                
            }
        }
        _udpClient.Close();
    }

}

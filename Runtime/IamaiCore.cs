using UnityEngine;
using NetMQ; // https://github.com/zeromq/netmq
using NetMQ.Sockets;
using System.Threading;

public class IamaiCore : MonoBehaviour
{
    private PushSocket pushSocket;
    private PullSocket pullSocket;
    private Thread pullThread;
    private bool running = false;

    void Start()
    {
        AsyncIO.ForceDotNet.Force();
        pushSocket = new PushSocket();
        pullSocket = new PullSocket();

        pushSocket.Connect("tcp://localhost:5557");
        pullSocket.Connect("tcp://localhost:5558");

        running = true;
        pullThread = new Thread(PullMessages);
        pullThread.Start();
    }

    void PullMessages()
    {
        while (running)
        {
            string message = pullSocket.ReceiveFrameString();
            Debug.Log($"Received: {message}");
        }
    }

    public void SendMessage(string message)
    {
        pushSocket.SendFrame(message);
    }

    void OnDestroy()
    {
        running = false;
        pullThread?.Join();
        pushSocket?.Dispose();
        pullSocket?.Dispose();
        NetMQConfig.Cleanup();
    }
}

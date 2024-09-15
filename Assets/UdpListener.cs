using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpListener : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread udpListenerThread;
    private bool isListening = false;
    private int port = 5555;

    void Start()
    {
        // Start the UDP listener in a separate thread
        udpListenerThread = new Thread(new ThreadStart(ListenForMessages));
        udpListenerThread.IsBackground = true;
        udpListenerThread.Start();
    }

    void ListenForMessages()
    {
        try
        {
            udpClient = new UdpClient(port);
            isListening = true;
            Debug.Log($"UDP server is listening on port {port}");

            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

            while (isListening)
            {
                // Wait for incoming UDP message
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                // Convert byte data to string and log the message
                string receivedMessage = Encoding.ASCII.GetString(receivedBytes);
                Debug.Log($"Received message: {receivedMessage} from {remoteEndPoint.Address}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        isListening = false;
        udpClient?.Close();
        udpListenerThread?.Abort();
    }
}

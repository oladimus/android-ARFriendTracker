using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class UDPLocationSender : MonoBehaviour
{
    public string targetIP; // Set this to your friend's device IP
    public string mylaptopIP = "192.168.101.119";
    public int port = 9050;

    UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
        InvokeRepeating(nameof(SendLocation), 1f, 1f); // Send every second
    }

    void SendLocation()
    {
        var lm = LocationManager.Instance;
        Debug.Log("SendLocation() called");
        if (lm == null)
        {
            Debug.LogWarning("LocationManager.Instance is null");
            return;
        }
        string message = $"{SystemInfo.deviceUniqueIdentifier}:{lm.latitude}:{lm.longitude}:{lm.heading}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        Debug.Log($"Sending to {mylaptopIP}:{port} {message}");
        udpClient.Send(data, data.Length, mylaptopIP, port);
        udpClient.Send(data, data.Length, targetIP, port);

#if UNITY_EDITOR
        string id = "FAKE_EDITOR_ID";
        float lat = 61.49626f;
        float lon = 23.77689f;
        float heading = 0;

        string msg = $"{id}:{lat}:{lon}:{heading}";
        byte[] dat = Encoding.UTF8.GetBytes(msg);
        udpClient.Send(dat, dat.Length, mylaptopIP, port);
        udpClient.Send(dat, dat.Length, targetIP, port);

#endif
    }
}

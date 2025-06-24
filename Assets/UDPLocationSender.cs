using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class UDPLocationSender : MonoBehaviour
{
    public string targetIP = "192.168.101.69"; // Set this to your friend's device IP
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
        string latStr = lm.latitude.ToString().Replace('.', ',');
        string lonStr = lm.longitude.ToString().Replace('.', ',');
        string headingStr = lm.heading.ToString().Replace('.', ',');

        string message = $"{SystemInfo.deviceUniqueIdentifier}:{latStr}:{lonStr}:{headingStr}";

        byte[] data = Encoding.UTF8.GetBytes(message);

        Debug.Log($"Sending to {mylaptopIP}:{port} {message}");
        Debug.Log($"Sending to {targetIP}:{port} {message}");

        udpClient.Send(data, data.Length, mylaptopIP, port);
        udpClient.Send(data, data.Length, targetIP, port);
    }
}

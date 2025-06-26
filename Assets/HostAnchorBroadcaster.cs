using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class HostAnchorBroadcaster : MonoBehaviour
{
    public HostAnchorManager anchorManager; // Assign in inspector
    public string targetIP = "192.168.101.69"; // Client's IP
    public int port = 9050;

    UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
        anchorManager.OnAnchorPlaced += SendAnchorData;
    }

    void SendAnchorData(Vector3 pos, Quaternion rot)
    {
        string data = string.Format(CultureInfo.InvariantCulture,
            "{0},{1},{2}:{3},{4},{5},{6}",
            pos.x, pos.y, pos.z,
            rot.x, rot.y, rot.z, rot.w
        );

        byte[] bytes = Encoding.UTF8.GetBytes(data);
        udpClient.Send(bytes, bytes.Length, targetIP, port);
        Debug.Log($"Sent anchor pose to {targetIP}: {data}");
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}

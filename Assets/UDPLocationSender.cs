using System.Globalization;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UDPLocationSender : MonoBehaviour
{
    public ARAnchor localAnchor;        // Assign after it's created
    public string targetIP = "192.168.101.69";
    public int port = 9050;

    private UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
    }

    void Update()
    {
        if (localAnchor == null)
        {
            Debug.Log("Anchor is null");
            return;
        }

        Transform cam = Camera.main.transform;

        Vector3 relativePos = localAnchor.transform.InverseTransformPoint(cam.position);
        Quaternion relativeRot = Quaternion.Inverse(localAnchor.transform.rotation) * cam.rotation;

        string data = string.Format(CultureInfo.InvariantCulture,
            "{0},{1},{2}:{3},{4},{5},{6}",
            relativePos.x, relativePos.y, relativePos.z,
            relativeRot.x, relativeRot.y, relativeRot.z, relativeRot.w
        );

        byte[] bytes = Encoding.UTF8.GetBytes(data);
        udpClient.Send(bytes, bytes.Length, targetIP, port);
        udpClient.Send(bytes, bytes.Length, "192.168.101.119", port); // laptop
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}

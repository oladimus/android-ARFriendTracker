using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UDPLocationReceiver : MonoBehaviour
{
    public int port = 9050;
    private UdpClient udpClient;
    private Thread receiveThread;

    public ARAnchor receivedAnchor;          // Set this when you instantiate the anchor
    public GameObject friendAvatarPrefab;    // Assign in inspector

    private GameObject friendAvatar;

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                ApplyRemotePose(message);
            }
            catch { }
        }
    }

    void ApplyRemotePose(string message)
    {
        string[] split = message.Split(':');
        if (split.Length != 2) return;

        string[] posParts = split[0].Split(',');
        string[] rotParts = split[1].Split(',');
        if (posParts.Length != 3 || rotParts.Length != 4) return;

        Vector3 relativePos = new Vector3(
            float.Parse(posParts[0], CultureInfo.InvariantCulture),
            float.Parse(posParts[1], CultureInfo.InvariantCulture),
            float.Parse(posParts[2], CultureInfo.InvariantCulture)
        );

        Quaternion relativeRot = new Quaternion(
            float.Parse(rotParts[0], CultureInfo.InvariantCulture),
            float.Parse(rotParts[1], CultureInfo.InvariantCulture),
            float.Parse(rotParts[2], CultureInfo.InvariantCulture),
            float.Parse(rotParts[3], CultureInfo.InvariantCulture)
        );

        Vector3 worldPos = receivedAnchor.transform.TransformPoint(relativePos);
        Quaternion worldRot = receivedAnchor.transform.rotation * relativeRot;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (friendAvatar == null)
            {
                friendAvatar = Instantiate(friendAvatarPrefab, worldPos, worldRot);
            }
            else
            {
                friendAvatar.transform.SetPositionAndRotation(worldPos, worldRot);
            }
        });
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}

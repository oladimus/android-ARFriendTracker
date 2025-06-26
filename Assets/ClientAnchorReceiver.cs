using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClientAnchorReceiver : MonoBehaviour
{
    public int port = 9050;
    private UdpClient udpClient;
    private Thread receiveThread;

    public ARAnchorManager anchorManager;
    public GameObject anchorPrefab;

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log(message);

                string[] split = message.Split(':');
                if (split.Length != 2) continue;

                string[] posParts = split[0].Split(',');
                string[] rotParts = split[1].Split(',');

                if (posParts.Length != 3 || rotParts.Length != 4) continue;

                Vector3 position = new Vector3(
                    float.Parse(posParts[0]),
                    float.Parse(posParts[1]),
                    float.Parse(posParts[2])
                );

                Quaternion rotation = new Quaternion(
                    float.Parse(rotParts[0]),
                    float.Parse(rotParts[1]),
                    float.Parse(rotParts[2]),
                    float.Parse(rotParts[3])
                );

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (anchorPrefab != null)
                    {
                        GameObject go = Instantiate(anchorPrefab, position, rotation);
                        ARAnchor receivedAnchor = go.GetComponent<ARAnchor>();
                        if (receivedAnchor == null)
                        {
                            receivedAnchor = go.AddComponent<ARAnchor>();
                        }
                        Debug.Log($"Anchor instantiated at {position}");

                        GetComponent<UDPLocationReceiver>().receivedAnchor = receivedAnchor;
                    }
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"UDP receive error: {e.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}

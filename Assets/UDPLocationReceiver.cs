using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Collections.Generic;

public class UDPLocationReceiver : MonoBehaviour
{
    public int port = 9050;
    UdpClient udpClient;

    [Header("Prefabs for known friends")]
    public GameObject alicePrefab;
    public GameObject bobPrefab;
    public GameObject fakePrefab;

    Dictionary<string, GameObject> friends = new();

    void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.Client.Blocking = false; // Make Receive non-blocking
    }

    void Update()
    {
        while (udpClient.Available > 0)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
            byte[] data = udpClient.Receive(ref remoteEP);
            string message = Encoding.UTF8.GetString(data);

            string[] parts = message.Split(':');
            if (parts.Length != 4)
                continue;

            string id = parts[0];
            float lat = float.Parse(parts[1]);
            float lon = float.Parse(parts[2]);
            float heading = float.Parse(parts[3]);

            Vector3 pos = GPSUtils.GPSToUnityPosition(lat, lon);
            Debug.Log("ASDASD");
            if (!friends.ContainsKey(id))
            {
                Debug.Log("BBBB");
                GameObject prefabToUse = null;

                if (id == "faac94c364ac4d71135a1e5cbf797d8e") // Replace with real ID
                    prefabToUse = alicePrefab;
                else if (id == "9a8a2f9c6e7eced8cb424ebc689031c7")
                    prefabToUse = bobPrefab;
                else if (id == "FAKE_EDITOR_ID")
                    prefabToUse = fakePrefab;
                else
                    prefabToUse = bobPrefab;
                    //continue; // Unknown user

                GameObject go = Instantiate(prefabToUse);
                Debug.Log($"Instantiated prefab for ID: {id} at position {pos}");
                friends[id] = go;
            } else
            {
                Debug.Log($"Updated position for ID: {id} to {pos}");
            }

                friends[id].transform.position = pos;
        }
    }
}

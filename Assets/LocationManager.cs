using UnityEngine;
using UnityEngine.Android;
using System.Collections;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    public float latitude;
    public float longitude;
    public float heading;

    private bool locationEnabled;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    IEnumerator Start()
    {
        // Request permission if not already granted
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1f); // Allow time for prompt
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location services are disabled by user");
            yield break;
        }

        Input.location.Start(1f, 0.1f); // Desired accuracy in meters, update distance in meters
        Input.compass.enabled = true;

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            yield break;
        }

        locationEnabled = true;
    }

    void Update()
    {
        if (!locationEnabled || Input.location.status != LocationServiceStatus.Running)
            return;

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        heading = Input.compass.trueHeading;

        // Debug.Log($"Updated Location: {latitude}, {longitude}, heading: {heading}");
    }
}

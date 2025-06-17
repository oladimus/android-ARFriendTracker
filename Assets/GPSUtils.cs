using UnityEngine;

public static class GPSUtils
{
    static float originLat;
    static float originLon;
    static bool originSet = false;

    public static Vector3 GPSToUnityPosition(float lat, float lon)
    {
        if (!originSet)
        {
            originLat = LocationManager.Instance.latitude;
            originLon = LocationManager.Instance.longitude;
            originSet = true;
        }

        float metersPerDeg = 111320f;

        float dx = (lon - originLon) * metersPerDeg;
        float dz = (lat - originLat) * metersPerDeg;

        return new Vector3(dx, 0, dz);
    }
}

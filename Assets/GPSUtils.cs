using UnityEngine;
public static class GPSUtils
{
    static float originLat = 61.49627f;  // example fixed origin
    static float originLon = 23.77688f;

    public static Vector3 GPSToUnityPosition(float lat, float lon)
    {
        float metersPerDeg = 111320f;

        float dx = (lon - originLon) * metersPerDeg;
        float dz = (lat - originLat) * metersPerDeg;

        return new Vector3(dx, 0, dz);
    }

    public static float HaversineDistance(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000f; // Radius of Earth in meters
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c; // Distance in meters
    }
}

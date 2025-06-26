using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HostAnchorManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARAnchorManager anchorManager;

    public Action<Vector3, Quaternion> OnAnchorPlaced; // Event to notify network sender

    private ARAnchor currentAnchor;

    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Debug.Log("Hello");
            Vector2 touchPos = Input.GetTouch(0).position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(touchPos, hits, TrackableType.Planes))
            {
                Pose pose = hits[0].pose;

                if (currentAnchor != null)
                {
                    Destroy(currentAnchor.gameObject);
                }

                GameObject anchorGO = new GameObject("LocalSharedAnchor");
                anchorGO.transform.position = pose.position;
                anchorGO.transform.rotation = pose.rotation;

                currentAnchor = anchorGO.AddComponent<ARAnchor>();
                Debug.Log($"Anchor placed at {pose.position}");

                GetComponent<UDPLocationSender>().localAnchor = currentAnchor;
                Debug.Log("Local host anchor sent to UDPLocationSender");

                // Send the transform to clients
                OnAnchorPlaced?.Invoke(pose.position, pose.rotation);
            }
        }
    }
}

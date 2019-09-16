using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class TouchManager : MonoBehaviour
{
    public GameObject planeToRaycastAgainstPrefab;
    public GameObject objectToRaycastPrefab;
    public Camera arCamera;

    private List<ARRaycastHit> hitsAR = new List<ARRaycastHit>();
    private RaycastHit hits;
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private bool isPlanePlaced = false;
    private bool isCubePlaced = false;
    private GameObject placedObject;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    bool TryGetTouchPosition(out Touch touch)
    {
        if (Input.touchCount > 0)
        {
            try
            {
                touch = Input.GetTouch(0);
                return true;

            }
            catch (System.Exception)
            {
                Debug.LogError("touch not available");
                throw;
            }
        }

        touch = default;

        return false;
    }

    private void TogglePlaneDetection()
    {
        arPlaneManager.enabled = !arPlaneManager.enabled;

        // Go though each plane
        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(arPlaneManager.enabled);
        }
    }

    void Update()
    {
        int layerMask = 1 << 8;

        if (!isPlanePlaced)
        {
            if (!TryGetTouchPosition(out Touch touch))
                return;

            if (arRaycastManager.Raycast(touch.position, hitsAR, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("Placed the plane");
                var hitPose = hitsAR[0].pose;
                Instantiate(planeToRaycastAgainstPrefab, hitPose.position, hitPose.rotation);
                isPlanePlaced = true;
                //arPlaneManager.enabled = false;
                TogglePlaneDetection();
            }
        }

        else if (isPlanePlaced)
        {
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(i).position);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo, layerMask))
                    {
                        placedObject = Instantiate(objectToRaycastPrefab, hitInfo.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}

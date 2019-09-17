using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class TouchManager : MonoBehaviour
{
    public GameObject planeToRaycastAgainstPrefab;
    public GameObject objectToRaycastPrefab;
    public Camera arCamera;
    public GameObject lineRendererPrefab;
    public GameObject wallPrefab;

    private List<ARRaycastHit> hitsAR = new List<ARRaycastHit>();
    private RaycastHit hits;
    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private bool isPlanePlaced = false;
    private List<GameObject> listOfPlacedObjects;
    private List<GameObject> listOfLinerenderers;
    //private LineRenderer lineRenderer;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        listOfPlacedObjects = new List<GameObject>();
        listOfLinerenderers = new List<GameObject>();
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

        // Disable spawning of stuff by pressing a button
        if (EventSystem.current.IsPointerOverGameObject(0))
        {
            return;
        }

        else if (!isPlanePlaced)
        {
            if (!TryGetTouchPosition(out Touch touch))
                return;

            if (arRaycastManager.Raycast(touch.position, hitsAR, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("Placed the plane");
                var hitPose = hitsAR[0].pose;
                Instantiate(planeToRaycastAgainstPrefab, hitPose.position, hitPose.rotation);
                isPlanePlaced = true;
                TogglePlaneDetection();
            }
        }

        else if (isPlanePlaced)
        {
            if(Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo, layerMask))
                    {
                        var gameObject = Instantiate(objectToRaycastPrefab, hitInfo.point, Quaternion.identity);
                        listOfPlacedObjects.Add(gameObject);
                        //DrawLinesBetweenObjects();
                        DrawWallBetweenObjects();
                    }
                }
            }
        }
    }

    public Vector3 FindPerpendicularAngle(Vector3 pos1, Vector3 pos2)
    {
        var temp = Quaternion.LookRotation(pos2 - pos1).eulerAngles;
        return new Vector3(temp.x, temp.y + 90f, temp.z);
    }

    private void DrawWallBetweenObjects()
    {
        int lengthOfList = listOfPlacedObjects.Count;
        if (lengthOfList == 2)
        {
            var A = listOfPlacedObjects[0].transform;
            var B = listOfPlacedObjects[1].transform;

            var vector_AB = B.position - A.position;
            var length_AB = vector_AB.magnitude;

            Debug.Log("Length vector: " + length_AB);

            Quaternion rotation = Quaternion.LookRotation(vector_AB);
            var wall = Instantiate(wallPrefab, A.position, rotation);
            
            // I need to rotate the plane 90 deg clockwise
            }
    }

    private void DrawLinesBetweenObjects()
    {
        int lengthOfList = listOfPlacedObjects.Count;
        if (lengthOfList > 1)
        {
            for (int i = 0; i < lengthOfList-1; i++)
            {
                try
                {
                    var lineRendererGameObject = Instantiate(lineRendererPrefab);
                    var lineRenderer = lineRendererGameObject.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, listOfPlacedObjects[i].transform.position);
                    lineRenderer.SetPosition(1, listOfPlacedObjects[i+1].transform.position);
                    listOfLinerenderers.Add(lineRendererGameObject);
                }
                catch (Exception)
                {
                    Debug.LogError("Exceptions baby!");
                    throw;
                }
            }
        }
    }

    public void ClearListOfObjects()
    {
        // destroy linerenderers
        for (int i = 0; i < listOfLinerenderers.Count; i++)
        {
            Destroy(listOfLinerenderers[i].gameObject);
        }   listOfLinerenderers.Clear();

        // destroy the placed objects if any
        for (int i = 0; i < listOfPlacedObjects.Count; i++)
        {
            Destroy(listOfPlacedObjects[i].gameObject);
        }   listOfPlacedObjects.Clear();
    }
}

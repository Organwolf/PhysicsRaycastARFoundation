using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersect : MonoBehaviour
{

    // Variables
    public GameObject clickPointPrefab;
    public GameObject quad;
    public GameObject debugWallPrefab;
    private GameObject clickPoint;
    private List<GameObject> clickedPoints = new List<GameObject>();
    //public GameObject debugWall;
    //public GameObject[] fences;
    //Vector3[] fenceNormals;

    Plane m_Plane;

    // Start is called before the first frame update
    // TransformaPoint - transforms a local point to worldspace
    // TransformVector - transforms a vector from local space to worldspace
    void Start()
    {
        Vector3[] vertices = quad.GetComponent<MeshFilter>().mesh.vertices;
        m_Plane = new Plane(quad.transform.TransformPoint(vertices[0]) + new Vector3(0, 0.3f, 0),
                            quad.transform.TransformPoint(vertices[1]) + new Vector3(0, 0.3f, 0),
                            quad.transform.TransformPoint(vertices[2]) + new Vector3(0, 0.3f, 0));
        //fenceNormals = new Vector3[fences.Length];
        //for (int i = 0; i < fences.Length; i++)
        //{
        //    Vector3 normal = fences[i].GetComponent<MeshFilter>().mesh.normals[0];
        //    fenceNormals[i] = fences[i].transform.TransformVector(normal);
        //}

    }

    // Update is called once per frame
    void Update()
    {
        //Detect when there is a mouse click
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Click detected");
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;

            if (m_Plane.Raycast(ray, out enter))
            {
                Debug.Log("Ray hit");
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);

                clickPoint = Instantiate(clickPointPrefab);
                clickedPoints.Add(clickPoint);
                clickPoint.transform.position = hitPoint;

                if (clickedPoints.Count == 2)
                {
                    // try to place the wall between the points
                }
                //bool inside = true;

                //for (int i = 0; i < fences.Length; i++)
                //{
                //    Vector3 hitPointToFence = fences[i].transform.position - hitPoint;
                //    inside = inside && Vector3.Dot(fenceNormals[i], hitPointToFence) <= 0;
                //}

                //if(inside)
                    //Move your cube GameObject to the point where you clicked
            }
        }
    }
}

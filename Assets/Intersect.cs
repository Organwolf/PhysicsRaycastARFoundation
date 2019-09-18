using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersect : MonoBehaviour
{

    // Variables
    public GameObject clickPointPrefab;
    public GameObject quad;
    private GameObject clickPoint;
    private List<GameObject> clickedPoints = new List<GameObject>();
    //public GameObject debugWall;
    //public GameObject[] fences;
    //Vector3[] fenceNormals;

    Plane m_Plane;
    Plane m_Wall;

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

    private bool HasSavedPoint;
    private Vector3 savedPoint;

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

                clickPoint = Instantiate(clickPointPrefab, hitPoint, Quaternion.identity);

                if(HasSavedPoint)
                {
                    createQuadFromPoints(savedPoint, hitPoint);
                    HasSavedPoint = false;
                }
                else
                {
                    savedPoint = hitPoint;
                    HasSavedPoint = true;
                }
            }
        }
    }

    private float height = 1;

    private void createQuadFromPoints(Vector3 firstPoint, Vector3 secondPoint)
    {
        GameObject newMeshObject = new GameObject("wall");
        MeshFilter newMeshFilter = newMeshObject.AddComponent<MeshFilter>();

        Mesh newMesh = new Mesh();

        Vector3 heightVector = new Vector3(0, height, 0);

        newMesh.vertices = new Vector3[]
        {
            firstPoint,
            secondPoint,
            firstPoint + heightVector,
            secondPoint + heightVector
        };

        newMesh.triangles = new int[]
        {
            0,2,1,1,2,3,
            3,2,1,1,2,0
        };

        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        newMesh.RecalculateBounds();

        newMeshFilter.mesh = newMesh;

        newMeshObject.AddComponent<MeshRenderer>();

        // skapa en quad från de två punkterna

        // ge quaden ett visst material

        // lägg in quaden i en lista - så att jag kan radera dem via en knapp
    }
}

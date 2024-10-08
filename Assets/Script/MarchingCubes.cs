using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MarchingCubes : MonoBehaviour
{
    public Material weldMaterial;
    private Mesh weldMesh;
    private Vector3 lastPosition;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private MeshFilter meshFilter;

    void Start()
    {
        weldMesh = new Mesh();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = weldMesh;
        gameObject.AddComponent<MeshRenderer>().material = weldMaterial;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Replace with VR controller input if needed
        {
            Vector3 currentPosition = transform.position;
            if (Vector3.Distance(lastPosition, currentPosition) > 0.05f)
            {
                AddWeldPoint(currentPosition);
                lastPosition = currentPosition;
            }
        }
    }

    void AddWeldPoint(Vector3 position)
    {
        // Add new vertices and triangles for weld bead mesh here
        vertices.Add(position + new Vector3(0.01f, 0, 0)); // Example offset for thickness
        vertices.Add(position - new Vector3(0.01f, 0, 0));

        int start = vertices.Count - 2;
        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start - 2);  // Create quad for trail

        weldMesh.vertices = vertices.ToArray();
        weldMesh.triangles = triangles.ToArray();
        weldMesh.RecalculateNormals();  // Update shading
    }
}

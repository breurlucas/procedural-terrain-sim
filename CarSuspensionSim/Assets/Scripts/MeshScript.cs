using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshScript : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] indices;

    public int xSize = 20;
    public int zSize = 20;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //StartCoroutine(CreateShape());
        CreateShape();
        UpdateMesh();
    }

     // Update is called once per frame
    void Update(){}

    //IEnumerator CreateShape() {
    void CreateShape() {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                // Set y height
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                 vertices[i] = new Vector3(x, y, z);   
                 i++;
            }
        }

        indices = new int[xSize * zSize * 6]; // Total amount of triangle vertices

        int xShift = 0; // Shift one vertex to the right
        int index = 0; // Keeps track of the indices

        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                indices[index] = xShift + 0;
                indices[index + 1] = xShift + xSize + 1;
                indices[index + 2] = xShift + 1;
                indices[index + 3] = xShift + 1;
                indices[index + 4] = xShift + xSize + 1;
                indices[index + 5] = xShift + xSize + 2;

                xShift++;
                index += 6;
                //yield return new WaitForSeconds(.1f);
            }
            xShift++;
        }

    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    // private void OnDrawGizmos() {
    //     if (vertices == null)
    //         return;
    //     for (int i = 0; i < vertices.Length; i++) {
    //         Gizmos.DrawSphere(vertices[i], .1f);
    //     }
    // }
}

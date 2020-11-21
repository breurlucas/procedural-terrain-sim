using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshScript : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] indices;
    Color[] colors;

    public int xSize;
    public int zSize;
    public float noiseFreq1, noiseFreq2, noiseFreq3;
    public float noiseAmp1, noiseAmp2, noiseAmp3;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;


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
                // float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                 float y = CalculateNoise(x, z);
                 vertices[i] = new Vector3(x, y, z);   

                 // Set up height map
                 if (y > maxTerrainHeight)
                    maxTerrainHeight = y;

                if (y < minTerrainHeight)
                    minTerrainHeight = y;

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

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                // Convert height to a value between 0 and 1
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                 colors[i] = gradient.Evaluate(height);   
                 i++;
            }
        }

    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    private float CalculateNoise(float x, float z)
    {
        float noise = Mathf.PerlinNoise(x * noiseFreq1, z * noiseFreq1) * noiseAmp1;
        noise += Mathf.PerlinNoise(x * noiseFreq2, z * noiseFreq2) * noiseAmp2;
        noise += Mathf.PerlinNoise(x * noiseFreq3, z * noiseFreq3) * noiseAmp3;
        return noise;
    }

    // private void OnDrawGizmos() {
    //     if (vertices == null)
    //         return;
    //     for (int i = 0; i < vertices.Length; i++) {
    //         Gizmos.DrawSphere(vertices[i], .1f);
    //     }
    // }
}

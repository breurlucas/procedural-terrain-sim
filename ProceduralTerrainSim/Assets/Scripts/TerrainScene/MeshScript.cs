using System.Collections;
using UnityEngine;

// This script considers 3D coordinates x, y and z
public class MeshScript {
    public static IEnumerator GenerateMesh(Mesh mesh, float[,] noiseMap, int terrainHeight) {
        
        int xSize = noiseMap.GetLength(0);
        int zSize = noiseMap.GetLength(1);

        int[] indices;
        Vector3[] vertices = new Vector3[xSize * zSize];
        // There is a max of 65535 vertices allowed in Unity before auto mesh splitting
        //Debug.Log(vertices.GetLength(0));

        for (int i = 0, z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                // Set y height
                float y = noiseMap[x,z] * terrainHeight;
                vertices[i] = new Vector3(x, y, z);   
                i++;
            }
        }

        indices = new int[(xSize - 1) * (zSize - 1) * 6]; // Total amount of triangle vertices

        int xShift = 0; // Shift one vertex to the right
        int index = 0; // Keeps track of the indices

        for (int z = 0; z < (zSize - 1); z++) {
            for (int x = 0; x < (xSize - 1); x++) {
                indices[index] = xShift + 0;
                indices[index + 1] = xShift + (xSize - 1) + 1;
                indices[index + 2] = xShift + 1;
                indices[index + 3] = xShift + 1;
                indices[index + 4] = xShift + (xSize - 1) + 1;
                indices[index + 5] = xShift + (xSize - 1) + 2;

                xShift++;
                index += 6;
            }
            xShift++;
        }

        // mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();

        yield return null;
    }

}

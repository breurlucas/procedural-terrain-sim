using System.Collections;
using UnityEngine;

// This script considers 3D coordinates x, y and z
public class MeshScript {
    public static Mesh GenerateMesh(int xSize, int zSize, float[,] noiseMap) {

        Mesh mesh = new Mesh();
        int[] indices;
        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                // Set y height
                // float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                float y = noiseMap[x,z] * 40;
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
            }
            xShift++;
        }

        mesh.vertices = vertices;
        mesh.triangles = indices;

        return mesh;
    }
}

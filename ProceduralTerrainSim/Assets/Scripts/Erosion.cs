using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  RESOURCES
 *  'Implementation of a method for hydraulic erosion' Bachelor's Thesis by Hans Theobald Beyer
 *  https://www.firespark.de/resources/downloads/implementation%20of%20a%20methode%20for%20hydraulic%20erosion.pdf
 *
 *  Sebastian Lague's Hydraulic Erosion implementation 
 *  https://github.com/SebLague/Hydraulic-Erosion
 *
 *  Water erosion on heightmap terrain blog post by E-DOG
 *  http://ranmantaru.com/blog/2011/10/08/water-erosion-on-heightmap-terrain/
 *
 */

public class Erosion : MonoBehaviour {
    public Vector3[] dropletPath;
    System.Random random;

    // Droplet params
    int iterations = 10;
    float radius = 2.5f;
    float pInertia = 0.2f; // Value between 0 and 1

    public void Simulate(float[,] map, int terrainHeight) {
        random = new System.Random();

        float[] pos = new float[2];
        float[] dir = new float[2];
        float[] gradient = new float[2];

        // Randomize starting coordinates of one droplet
        pos[0] = random.Next(0, map.GetLength(0) - 1); // x
        pos[1] = random.Next(0, map.GetLength(1) - 1); // z
        float y = map[(int)pos[0],(int)pos[1]];

        // Gizmo path
        dropletPath = new Vector3[iterations + 1];
        dropletPath[0] = new Vector3(pos[0], y * terrainHeight + radius, pos[1]);

        dir[0] = 0; // Set x direction equal to 0
        dir[1] = 0; // Set z direction equal to 0

        for(int i = 0; i < iterations; i++) {
            
            // Define the bottom left corner of the one-by-one cell the droplet fell within
            int cellX00 = (int)pos[0];
            int cellZ00 = (int)pos[1];

            // Droplet offsets 'u' and 'v' from the bottom left corner of the cell
            float u = pos[0] - cellX00; // Offset in the x axis
            float v = pos[1] - cellZ00; // Offset in the z axis

            // Retrieve the heights of the four corners of the cell
            float cellH00 = map[cellX00, cellZ00];
            float cellH10 = map[cellX00 + 1, cellZ00];
            float cellH01 = map[cellX00, cellZ00 + 1] ;
            float cellH11 = map[cellX00 + 1, cellZ00 + 1];

            /* Calculate the gradients using the Hans Beyer method considering the position 
            of the droplet (in the sources) */
            gradient[0] = (cellH10 - cellH00) * (1 - v) + (cellH11 - cellH01) * v;
            gradient[1] = (cellH01 - cellH00) * (1 - u) + (cellH11 - cellH10) * u;
            
            // With inertia
            // dir[0] = dir[0] * pInertia - gradient[0] * (1 - pInertia);
            // dir[1] = dir[1] * pInertia - gradient[1] * (1 - pInertia);

            // Without inertia
            dir[0] = dir[0] - gradient[0];
            dir[1] = dir[1] - gradient[1];

            /* Normalize the 'dir' vector: [v / magnitude[v]]
            magnitude[v] = sqrt(x^2 + y^2)
            */
            float magnitude = Mathf.Sqrt(dir[0] * dir[0] + dir[1] * dir[1]);
            dir[0] /= magnitude;
            dir[1] /= magnitude;

            pos[0] += dir[0];
            pos[1] += dir[1];

            // Break loop if the droplet falls outside the map's boundaries
            if(pos[0] >= map.GetLength(0) - 1 || pos[0] < 0 || 
                pos[1] >= map.GetLength(1) - 1 || pos[1] < 0)
                break;

            // Find the new droplet height through bilinear interpolation (Unit square simplification)
            float yNew = cellH00 * (1 - u) * (1 - v) + cellH10 * u * (1 - v) 
                    + cellH01 * (1 - u) * v + cellH11 * u * v;

            float yDelta = yNew - y;

            dropletPath[i].x = pos[0];
            dropletPath[i].y = yNew * terrainHeight + radius;
            dropletPath[i].z = pos[1];
        }
    }
        
    // Draw droplet's path for debugging purposes
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        for (int i = 0; i < dropletPath.GetLength(0); i++) {
            Gizmos.DrawSphere(dropletPath[i], radius);
        }
    }
}

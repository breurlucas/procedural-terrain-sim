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
    int maxSteps = 30;
    int maxIterations = 1;
    float radius = 1f;

    struct HG {
        public float height;
        public float[] gradient;
    }

    public IEnumerator Simulate(float[,] map, int terrainHeight) {
        random = new System.Random();
        float[] pos = new float[2];
        float[] dir = new float[2];
        float pInertia = .05f; // Value between 0 and 1
        float pCapacity = 4f;
        float pDeposition = .3f;
        float pErosion = .3f;
        float pEvaporation = .2f;
        float pGravity = 4f;
        int pErosionRadius = 3;
        float minSlope = 0;

        for (int iteration = 0; iteration < maxIterations; iteration++) {
            // Randomize starting coordinates of one droplet
            pos[0] = random.Next(0, map.GetLength(0) - 1); // x
            pos[1] = random.Next(0, map.GetLength(1) - 1); // z

            dir[0] = 0; // Set x direction equal to 0
            dir[1] = 0; // Set z direction equal to 0
            float speed = 0;
            float sediment = 0;
            float water = 1;

            for (int step = 0; step < maxSteps; step++) {
                /*
                 *  CURRENT POSITION
                 */

                // Define the bottom left corner of the one-by-one cell the droplet fell within
                int cellX00 = (int)pos[0];
                int cellZ00 = (int)pos[1];

                // Droplet offsets 'u' and 'v' from the bottom left corner of the cell
                float u = pos[0] - cellX00; // Offset in the x axis
                float v = pos[1] - cellZ00; // Offset in the z axis

                // Calculate the droplet's current height and x and z gradients
                HG hg = CalculateHGParams(map, pos);
                float heightCurrent = hg.height;

                // With inertia
                dir[0] = dir[0] * pInertia - hg.gradient[0] * (1 - pInertia);
                dir[1] = dir[1] * pInertia - hg.gradient[1] * (1 - pInertia);

                // Without inertia
                // dir[0] = dir[0] - gradient[0];
                // dir[1] = dir[1] - gradient[1];

                /* Normalize the 'dir' vector: [v / magnitude[v]]
                magnitude[v] = sqrt(x^2 + y^2)
                */
                float dirMagnitude = Mathf.Sqrt(dir[0] * dir[0] + dir[1] * dir[1]);
                if (dirMagnitude != 0) {
                    dir[0] /= dirMagnitude;
                    dir[1] /= dirMagnitude;
                }

                float[] posPrevious = new float[2];
                posPrevious[0] = pos[0];
                posPrevious[1] = pos[1];

                pos[0] += dir[0];
                pos[1] += dir[1];

                // Break loop if the droplet falls outside the map's boundaries
                if(pos[0] >= map.GetLength(0) - 1 || pos[0] < 0 || 
                pos[1] >= map.GetLength(1) - 1 || pos[1] < 0) {
                    break;
                }

                /*
                 *  NEW POSITION
                 */

                // Find the new droplet height through bilinear interpolation (Unit square simplification)
                hg = CalculateHGParams(map, pos);      
                float heightNew = hg.height;
                float heightDelta = heightNew - heightCurrent;

                // Calculate the droplet's max sediment carrying capacity
                float capacity = Mathf.Max(-heightDelta, minSlope) * speed * water * pCapacity;

                // If yDelta is positive, sediment is deposited at the last position 
                // Otherwise, the last position is eroded
                // If the droplet is carrying more sediment than it's capacity, deposit a percentage (pDeposition)
                if (heightDelta > 0 || sediment > capacity) {
                    // The deposit volume is adjusted percentually by the variable pDeposition 
                    float volDeposit = (sediment - capacity) * pDeposition;

                    // When travelling uphill, try to fill up the pit instead
                    if (heightDelta > 0)
                        volDeposit = Mathf.Min(heightDelta, sediment);

                    sediment -= volDeposit;

                    // Deposits through bilinear interpolation (Unit square simplification)
                    map[cellX00, cellZ00] += volDeposit * (1 -u) * (1 - v);
                    map[cellX00 + 1, cellZ00] += volDeposit * u * (1 - v);
                    map[cellX00, cellZ00 + 1] += volDeposit * (1 - u) * v;
                    map[cellX00 + 1, cellZ00 + 1] += volDeposit * u * v;
                }
                else {
                    // Don't erode more than the height difference, otherwise the drop would dig holes
                    // Erode
                    float volErosion = Mathf.Min((capacity - sediment) * pErosion, -heightDelta);
                    sediment += volErosion;
                    
                    // Calculate the number of vertices to be eroded based off the pErosionRadius
                    // The number of gridpoints inside of a circle is equal to 4 times it's radius squared
                    int nrOfVertices = pErosionRadius * pErosionRadius * 4;

                    // Weights for determining erosion levels based off distance to droplet
                    float[] weights = new float[nrOfVertices];
                    Vector2[] vertices = new Vector2[nrOfVertices];

                    int[] vertex = new int[2];
                    int sideOfErosionArea = 1 + (int)Mathf.Pow(2, pErosionRadius - 1);
                    int xOffset = cellX00 - pErosionRadius + 1;
                    int zOffset = cellZ00 - pErosionRadius + 1;
                    float weightsMagnitude = 0;

                    for (int i =0, x = 0; x < sideOfErosionArea; x++) {
                        for(int z = 0; z < sideOfErosionArea; z++) {
                            vertices[i].x = xOffset + x;
                            vertices[i].y = zOffset + z;
                            float distance = Mathf.Sqrt(Mathf.Pow((posPrevious[0] - vertices[i].x), 2) + Mathf.Pow((posPrevious[1] - vertices[i].y), 2)); 
                            weights[i] = Mathf.Max(0, pErosionRadius - distance); 
                            weightsMagnitude += weights[i];
                            i++;
                        }
                    }

                    for (int i = 0; i < nrOfVertices; i++) {
                        weights[i] /= weightsMagnitude;
                        if (vertices[i].x >= 0 && vertices[i].x < map.GetLength(0) - 1 &&
                            vertices[i].y >= 0 && vertices[i].y < map.GetLength(0) - 1 ) 

                            map[(int)vertices[i].x, (int)vertices[i].y] -= weights[i] * volErosion;
                    }
                }

                // Update speed
                speed = Mathf.Sqrt(speed * speed + heightDelta * pGravity);

                // Update water volume
                water = water * (1 - pEvaporation);


                // Gizmo
                dropletPath = new Vector3[maxSteps + 1];
                dropletPath[0] = new Vector3(pos[0], heightNew * terrainHeight + radius, pos[1]);
                dropletPath[step].x = pos[0];
                dropletPath[step].y = heightNew * terrainHeight + radius;
                dropletPath[step].z = pos[1];
            }
                // yield return new WaitForSeconds(.001f);
        }
        
        yield return null;
    }

    private HG CalculateHGParams(float[,] map, float[] pos) {
        // Initialize a new struct
        HG hg = new HG();
        hg.gradient = new float[2];

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
        hg.gradient[0] = (cellH10 - cellH00) * (1 - v) + (cellH11 - cellH01) * v;
        hg.gradient[1] = (cellH01 - cellH00) * (1 - u) + (cellH11 - cellH10) * u;

        // Find droplet's height through bilinear interpolation (Unit square simplification)
        hg.height = cellH00 * (1 - u) * (1 - v) + cellH10 * u * (1 - v) 
                        + cellH01 * (1 - u) * v + cellH11 * u * v;
        return hg;
    }
        
    // Draw droplet's path for debugging purposes
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        for (int i = 0; i < dropletPath.GetLength(0); i++) {
            Gizmos.DrawSphere(dropletPath[i], radius);
        }
    }
}

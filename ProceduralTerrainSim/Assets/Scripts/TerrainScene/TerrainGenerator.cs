using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float zoom;
    public int octaves;
    [Range(0,1)] // Make it into a slider in the inspector
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;

    public void GenerateMap() {

        // Match the number of points in the mesh
        int noiseMapWidth = mapWidth + 1;
        int noiseMapHeight = mapHeight + 1;
        float[,] noiseMap = Noise.GenerateNoiseMap(noiseMapWidth, noiseMapHeight, seed, zoom, 
        octaves, persistance, lacunarity, offset);

        Mesh mesh = new Mesh();
        //mesh.Clear();
        mesh = MeshScript.GenerateMesh(mapWidth, mapHeight, noiseMap);
        mesh.RecalculateNormals();

        Terrain terrain = FindObjectOfType<Terrain>();
        terrain.Generate(mesh);
    }

    // Called automatically when script variables are changed in the inspector
    void OnValidate() {
        if(mapWidth < 1)
            mapWidth = 1;
        if(mapHeight < 1)
            mapHeight = 1;
        if(lacunarity < 1)
            lacunarity = 1;
        if(octaves < 0)
            octaves = 0;
        if(zoom < 1)
            zoom = 1;
    }
}
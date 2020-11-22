using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{   
    [Range(1,255)] // Prevent auto mesh splitting due to number of vertices
    public int mapWidth;
    [Range(1,255)] // Prevent auto mesh splitting due to number of vertices
    public int mapDepth;
    public float zoom;
    public int octaves;
    [Range(0,1)] // Make it into a slider in the inspector
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public int terrainHeight;
    public bool autoUpdate;

    public void GenerateMap() {

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, seed, zoom, 
        octaves, persistance, lacunarity, offset);

        Mesh mesh = new Mesh();
        mesh = MeshScript.GenerateMesh(noiseMap, terrainHeight);

        ProceduralTerrain terrain = FindObjectOfType<ProceduralTerrain>();
        terrain.ApplyMesh(mesh);
    }

    // Called automatically when script variables are changed in the inspector
    void OnValidate() {
        if(lacunarity < 1)
            lacunarity = 1;
        if(octaves < 0)
            octaves = 0;
        if(zoom < 1)
            zoom = 1;
        if(terrainHeight < 0)
            terrainHeight = 0;
    }
}
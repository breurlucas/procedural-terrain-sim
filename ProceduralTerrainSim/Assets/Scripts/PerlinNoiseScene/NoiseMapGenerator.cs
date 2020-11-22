using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGenerator : MonoBehaviour
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
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, zoom, 
        octaves, persistance, lacunarity, offset);

        NoiseDisplay display = FindObjectOfType<NoiseDisplay>();
        display.DrawNoiseMap(noiseMap);
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
    }

}

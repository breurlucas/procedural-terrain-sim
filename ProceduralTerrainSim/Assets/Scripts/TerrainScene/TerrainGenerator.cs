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
    public bool enableRotation;
    public int rotationSpeed = 100;

    // Private variables
    private Mesh mesh;
    private float[,] noiseMap;
    private ProceduralTerrain terrain;
    private Vector3 center;
    private GameObject cam;

    void Start() {
        // Step 1: Generate noise map
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, seed, zoom, 
        octaves, persistance, lacunarity, offset);

        // Step 2: Generate mesh
        mesh = new Mesh();
        MeshScript.GenerateMesh(mesh, noiseMap, terrainHeight);

        // Step 3: Add mesh to the terrain game object
        terrain = FindObjectOfType<ProceduralTerrain>();
        terrain.ApplyMesh(mesh);

        // Step 4: Simulate erosion
        Erosion erosion = FindObjectOfType<Erosion>();
        StartCoroutine(erosion.Simulate(noiseMap, terrainHeight));
        
        center = new Vector3(noiseMap.GetLength(0) / 2f, 0, noiseMap.GetLength(0) / 2f);
        cam = GameObject.Find("Main Camera");
    }

    // Update the mesh on each frame
    void Update() {
        StartCoroutine(MeshScript.GenerateMesh(mesh, noiseMap, terrainHeight));

        // Rotate the camera around the terrain
        if (enableRotation)
            cam.transform.RotateAround(center, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void GenerateMap() {

        // Step 1: Generate noise map
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapDepth, seed, zoom, 
        octaves, persistance, lacunarity, offset);

        // Step 2: Generate mesh
        mesh = new Mesh();
        StartCoroutine(MeshScript.GenerateMesh(mesh, noiseMap, terrainHeight));

        // Step 3: Add mesh to the terrain game object
        terrain = FindObjectOfType<ProceduralTerrain>();
        terrain.ApplyMesh(mesh);

        // Step 4: Simulate erosion
        // Erosion erosion = FindObjectOfType<Erosion>();
        // StartCoroutine(erosion.Simulate(noiseMap, terrainHeight));
        // StartCoroutine(MeshScript.GenerateMesh(mesh, noiseMap, terrainHeight));
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
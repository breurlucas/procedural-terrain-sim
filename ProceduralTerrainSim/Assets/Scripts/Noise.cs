using System.Collections;
using UnityEngine;

/*
 * by Sebastian Lague
 * https://github.com/SebLague
 *
 */

// This script considers 2D coordinates x and y
public class Noise {

    public static float[,] GenerateNoiseMap(int width, int height, int seed, float zoom, int octaves,
                                            float persistance, float lacunarity, Vector2 offset) {
        
        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(seed);

        // Offset each octave by a random x and y coordinate
        Vector2[] octave0ffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-1000, 1000) + offset.x;
            float offsetY = prng.Next(-1000, 1000) + offset.y;
            octave0ffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (zoom <= 0) {
            zoom = 0.0001f;
        }

        float maxNoiseValue = float.MinValue; // Min value of a float
        float minNoiseValue = float.MaxValue; // Max value of a float

        // Center zoom functionality
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseValue = 0;

                for(int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth) / zoom * frequency + octave0ffsets[i].x;
                    float sampleY = (y - halfHeight) / zoom * frequency + octave0ffsets[i].y;
                    
                    // Allowing for negatives values
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    // float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseValue += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseValue > maxNoiseValue)
                    maxNoiseValue = noiseValue;
                else if (noiseValue < minNoiseValue)
                    minNoiseValue = noiseValue;

                noiseMap[x,y] = noiseValue;
            }
        }
                
        // Normalize the negative values
        for (int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}

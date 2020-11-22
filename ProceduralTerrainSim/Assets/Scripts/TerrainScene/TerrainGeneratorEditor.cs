using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        TerrainGenerator mapGen = (TerrainGenerator)target;

        // Updates the terrain when a variable is updated in the inspector
        if(DrawDefaultInspector()) {
            if(mapGen.autoUpdate) {
                mapGen.GenerateMap();
            }
        }
        
        // Creates a button in the inspector for generating the terrain in the scene tab
        if(GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }
    }
}

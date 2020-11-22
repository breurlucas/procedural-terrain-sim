using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseMapGenerator))]
public class NoiseMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        NoiseMapGenerator mapGen = (NoiseMapGenerator)target;

        // Updates the noise map when a variable is updated in the inspector
        if(DrawDefaultInspector()) {
            if(mapGen.autoUpdate) {
                mapGen.GenerateMap();
            }
        }
        
        // Creates a button in the inspector for generating the map in the scene tab
        if(GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }
    }
}

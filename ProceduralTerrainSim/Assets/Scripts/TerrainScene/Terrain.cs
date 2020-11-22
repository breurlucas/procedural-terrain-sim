using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Terrain : MonoBehaviour
{
    public void Generate(Mesh mesh) {
        GetComponent<MeshFilter>().mesh = mesh;
    }
   
}

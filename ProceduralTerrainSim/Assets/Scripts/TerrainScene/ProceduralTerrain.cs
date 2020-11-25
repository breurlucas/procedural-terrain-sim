using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProceduralTerrain : MonoBehaviour
{
    public void ApplyMesh(Mesh mesh) {
        GetComponent<MeshFilter>().mesh = mesh;
    }
}

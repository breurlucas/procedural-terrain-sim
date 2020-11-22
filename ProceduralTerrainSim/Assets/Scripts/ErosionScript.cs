using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionScript : MonoBehaviour
{
    TerrainGenerator mapGen;

    // Start is called before the first frame update
    void Start()
    {        
        mapGen.GenerateMap();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

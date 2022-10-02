using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1;
    public float oceanDepthMultiplier;
    public float mountainBlend;
    public float power;

    public TerrainNoiseSettings[] noiseLayers;
    [SerializeField]
    public  ComputeShader planetHeightCompute;

   
}

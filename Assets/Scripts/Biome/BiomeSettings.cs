using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
[System.Serializable]
public class BiomeSettings : ScriptableObject
{
    public BiomeNoiseSettings[] biomeNoiseSettings;
    public ComputeShader planetTextureCompute;
    public ComputeShader biomeCalculationCompute;
}



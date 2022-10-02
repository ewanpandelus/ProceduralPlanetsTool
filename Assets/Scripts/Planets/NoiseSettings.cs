using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public float amplitude = 1;
    public float frequency = 1;
    public float persistance = 1;
    public float lacunarity = 1;
    public int octaves = 1;
    public bool active;
    public Vector3 offset;

}
[System.Serializable]
public class TerrainNoiseSettings : NoiseSettings
{
    public float verticalShift;
    public float minValue;
}

[System.Serializable]
public class BiomeNoiseSettings : NoiseSettings
{
    
}
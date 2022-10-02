using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class TerracedNoiseFilter : INoiseFilter
//{
//    [SerializeField] NoiseSettings settings;
//    Noise noise;
//    private float radius;
//    public TerracedNoiseFilter(NoiseSettings settings, float radius)
//    {
//        noise = new Noise();
//        this.settings = settings;
//        this.radius = radius;

//    }

    //public float Evaluate(Vector3 point)
    //{
     
    //    float amplitude = 1;
    //    float frequency = settings.frequency;
    //    float noiseHeight = 0;
    //    for (int octave = 0; octave < settings.octaves; octave++)
    //    {
    //        float perlinValue = EvaluateTerracedNoise(point, frequency);// 1 - Mathf.Abs(noise.Evaluate((point) * frequency + settings.offset));
    //        perlinValue *= perlinValue;
    //        noiseHeight += perlinValue * amplitude;
    //        amplitude *= settings.persistance;
    //        frequency *= settings.lacunarity;
    //    }

    //    noiseHeight *= settings.amplitude;
    //    noiseHeight = Mathf.Max(0, noiseHeight - settings.minValue);
    //    if (settings.invert)
    //    {
    //        noiseHeight *= -1;
    //    }
    //    return noiseHeight;
    //}
    //public float EvaluateTerracedNoise(Vector3 point, float frequency)
    //{
    //    int exponent = settings.exponent;
    //    float e0 = 1 * noise.Evaluate(new Vector3(1 * point.x * frequency, point.y * frequency, point.z * frequency));
    //    float e1 = (float)(0.5 * noise.Evaluate(new Vector3(2 * point.x * frequency, 2 * point.y * frequency, 2 * point.z * frequency)) * e0);
    //    float e2 = (float)(0.25 * noise.Evaluate(new Vector3(4 * point.x * frequency, 4 * point.y * frequency, 4 * point.z * frequency) * (e0 + e1)));
    //    float e = (e0 + e1 + e2) / (1 + 0.5f + 0.25f);
    //    return Mathf.Round(e * exponent) / exponent;
    //}



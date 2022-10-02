//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////public class RidgeNoiseFilter : INoiseFilter
////{
////    [SerializeField] NoiseSettings settings;
////    Noise noise;
////    private float radius;
////    public RidgeNoiseFilter(NoiseSettings settings, float radius)
////    {
////        noise = new Noise();
////        this.settings = settings;
////        this.radius = radius;

////    }

////    public float Evaluate(Vector3 point)
////    {
////        float amplitude = 1;
////        float frequency = settings.frequency;
////        float noiseHeight = 0;
////        for (int octave = 0; octave < settings.octaves; octave++)
////        {
////            float perlinValue = 1- Mathf.Abs(noise.Evaluate((point) * frequency));
////            perlinValue *= perlinValue; 
////            noiseHeight += perlinValue * amplitude;
////            amplitude *= settings.persistance;
////            frequency *= settings.lacunarity;
////        }

////        noiseHeight *= settings.amplitude;
////        noiseHeight = Mathf.Max(0, noiseHeight - settings.minValue);
////        if (settings.invert)
////        {
////            noiseHeight *= -1;
////        }
////        return noiseHeight;
////    }

  
//}

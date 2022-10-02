
using System.Collections.Generic;
using UnityEngine;

public static class BiomeClassifier
{
  
    static float temperatureMax;

    struct BiomeType
    {

        public float minVal;
        public float maxVal;

    };

    private static BiomeType[] m_biomes = new BiomeType[3];
    static BiomeType m_desert;
    static BiomeType m_forest;
    static BiomeType m_snow;
   
    public static void Initialise()
    {
        temperatureMax  = 100;



  
        m_desert.minVal = 0;
        m_desert.maxVal = 10;
        m_forest.minVal = 40;
        m_forest.maxVal = 50;
        m_snow.minVal = 90;
        m_snow.maxVal = 100;
  
        
        m_biomes[0] = m_desert;
        m_biomes[1] = m_forest;
        m_biomes[2] = m_snow;


    }

    public static Vector3 ClassifyBiomes(float noiseVal)
    {
        int index = 0;
        float[] biomeClassification = new float[3];
        noiseVal *= temperatureMax;
       
        foreach(var biome in m_biomes)
        {
           if (noiseVal >= biome.minVal && noiseVal <= biome.maxVal)
           {
                biomeClassification[index] = 1;
                return new Vector3(biomeClassification[0], biomeClassification[1], biomeClassification[2]);
            }
           index++;
        }
        float[] classification = CalculateDistanceToBiomes(noiseVal);
        return new Vector3(classification[0], classification[1], classification[2]);
    }

    private static float[] ClassifyBasedOnDistance(float[] distances)
    {
        float totalDistanceWithoutMax = 0;
        float max = -1;
      
        foreach(var distance in distances)
        {
            if (distance > max)
            {
                max = distance;
            }
        }
        for(int i = 0; i< distances.Length; i++)
        {
            if (distances[i] == max)
            {
                distances[i] = 0;
            }
            else
            {
                //distances[i] = ExponentialFraction(distances[i]);
                totalDistanceWithoutMax += distances[i];
            }
        }

        return CalculateFractionOfClosestBiomes(distances, totalDistanceWithoutMax); 
    }
    private static float[] CalculateDistanceToBiomes(float noise)
    {
        float[] distances = new float[m_biomes.Length];
        int i = 0;
        foreach (BiomeType biome in m_biomes)
        {
            distances[i] = Mathf.Min(Mathf.Pow((noise - biome.minVal), 2), Mathf.Pow(noise - biome.maxVal, 2));
            i++;

        }
        return ClassifyBasedOnDistance(distances);
    }




    static float[] CalculateFractionOfClosestBiomes(float[] distances, float total)
    {
       // frac1 = ExponentialFraction(frac1);         //Creates faster changes between biomes
      //  frac2 = ExponentialFraction(frac2);
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] != 0)
            {
                distances[i] = 1 - (distances[i] / total); //inverts distances so closer elements are classified higher
            }
        }
        return distances;
    }

    static float ExponentialFraction(float expFrac)
    {
        return Mathf.Pow((expFrac * 10), 1.2f);
    }

}



















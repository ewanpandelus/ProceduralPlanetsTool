
using UnityEngine;
public class ClimateMap
{
  
    Color[] climateMap;
    public ClimateMap(int resolution)
    {
        climateMap = new Color[resolution * resolution];
    }
   
    public Color[] GetClimateMap()
    {
        return climateMap;
    }
    public void SetClimateMap(Color[] climateMap)
    {
        this.climateMap = climateMap;
    }

}























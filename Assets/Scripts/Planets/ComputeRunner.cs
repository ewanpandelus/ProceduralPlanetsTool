using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeRunner
{
    ComputeShader planetHeightCompute;
    ComputeShader planetTextureCompute;
    ComputeShader calculateBiomesCompute;
    float[] noiseValues;


    int resolution;
    public ComputeRunner(ComputeShader calculateBiomesCompute, ComputeShader planetHeightCompute, ComputeShader planetTextureCompute, int resolution)
    {
        this.calculateBiomesCompute = calculateBiomesCompute;
        this.planetHeightCompute = planetHeightCompute;
        this.planetTextureCompute = planetTextureCompute;
        this.resolution = resolution;
    }
    public Vector3[] RunPlanetHeightCompute(Vector3[] vertices, ref float closestPoint, ref float farthestPoint)
    {
        int vectorSize = sizeof(float) * 3;
        float[] maxMinHeight = new float[2];
        maxMinHeight[0] = int.MaxValue;
        maxMinHeight[1] = int.MinValue;

        //Setup Buffers
        ComputeBuffer maxMinBuffer = new ComputeBuffer(maxMinHeight.Length, vectorSize/3);
        ComputeBuffer vertexBuffer = new ComputeBuffer(vertices.Length, vectorSize);
        maxMinBuffer.SetData(maxMinHeight);
        vertexBuffer.SetData(vertices);
        planetHeightCompute.SetBuffer(0, "minMaxHeights", maxMinBuffer);
        planetHeightCompute.SetBuffer(0, "vertices", vertexBuffer);
      
        //Run Compute
        planetHeightCompute.Dispatch(0, vertices.Length / resolution, 1, 1);

        //Retrieve Data
        maxMinBuffer.GetData(maxMinHeight);
        vertexBuffer.GetData(vertices);

        //Release Buffers 
        vertexBuffer.Dispose();
        maxMinBuffer.Dispose();

        closestPoint = maxMinHeight[0];
        farthestPoint = maxMinHeight[1];
        return vertices;
        
    }
    
    public void SetupPlanetHeightCompute(ShapeSettings settings)
    {

        planetHeightCompute.SetInt("numVertices", resolution * resolution);
        planetHeightCompute.SetFloat("oceanFloorMultiplier", settings.oceanDepthMultiplier);

        planetHeightCompute.SetFloat("mountainBlend", settings.mountainBlend);
        planetHeightCompute.SetFloat("power", settings.power);



        int noiseParamIndex = 1;
        foreach (var noiseLayer in settings.noiseLayers)
        {
            Vector4[] noiseParamVectors;

            noiseParamVectors = noiseLayer.active ? SetupTerrainSettings(noiseLayer) : SetupEmptyVectors();
            planetHeightCompute.SetVectorArray("noiseParams" + noiseParamIndex.ToString(), noiseParamVectors);
            noiseParamIndex++;
        }
    }
    private Vector4[] SetupEmptyVectors()
    {
        Vector4[] vector4s = new Vector4[3];
        vector4s[0] = Vector4.zero;
        vector4s[1] = Vector4.zero;
        vector4s[2] = Vector4.zero;
        return vector4s;
    }
    private Vector4[] SetupTerrainSettings(TerrainNoiseSettings noiseLayer)
    {
        Vector4[] vector4s = new Vector4[3];
        Vector4 noiseValues1 = new Vector4();
        Vector4 noiseValues2 = new Vector4();
        Vector4 noiseValues3 = new Vector4();

        SetupGeneralSettings(noiseLayer, ref noiseValues1, ref noiseValues2);

        noiseValues2.y = noiseLayer.verticalShift;
        noiseValues2.z = noiseLayer.offset.x;
        noiseValues2.w = noiseLayer.offset.y;
        noiseValues3.x = noiseLayer.offset.z;
       


        vector4s[0] = noiseValues1;
        vector4s[1] = noiseValues2;
        vector4s[2] = noiseValues3;
        return vector4s;
    }
    public Vector4[] SetupBiomeSettings(BiomeNoiseSettings noiseLayer)
    {
        Vector4[] vector4s = new Vector4[2];
        Vector4 noiseValues1 = new Vector4();
        Vector4 noiseValues2 = new Vector4();
        SetupGeneralSettings(noiseLayer, ref noiseValues1, ref noiseValues2);
        vector4s[0] = noiseValues1;
        vector4s[1] = noiseValues2;
        return vector4s;
    }
    private void SetupGeneralSettings(NoiseSettings noiseLayer, ref Vector4 noiseValues1, ref Vector4 noiseValues2)
    {
        noiseValues1.x = noiseLayer.octaves;
        noiseValues1.y = noiseLayer.persistance;
        noiseValues1.z = noiseLayer.lacunarity;
        noiseValues1.w = noiseLayer.frequency;
        noiseValues2.x = noiseLayer.amplitude;
    
    }

    public Vector2 RunPlanetTextureCompute(Vector3[] vertices)
    {
        int floatSize = sizeof(float);
        int vectorSize = sizeof(float) * 3;
        Vector2 minMaxNoise = new Vector2();


        noiseValues = new float[resolution * resolution];

        float[] minMaxNoiseValues = new float[2];
        minMaxNoiseValues[0] = int.MaxValue;
        minMaxNoiseValues[1] = int.MinValue;

        //Setup Buffers
        ComputeBuffer noiseValueBuffer = new ComputeBuffer(noiseValues.Length, floatSize);
        ComputeBuffer minMaxBuffer = new ComputeBuffer(minMaxNoiseValues.Length, floatSize);
        ComputeBuffer vertexBuffer = new ComputeBuffer(vertices.Length, vectorSize);


        noiseValueBuffer.SetData(noiseValues);
        minMaxBuffer.SetData(minMaxNoiseValues);
        vertexBuffer.SetData(vertices);

        planetTextureCompute.SetBuffer(0, "noiseValues", noiseValueBuffer);
        planetTextureCompute.SetBuffer(0, "minMaxValues", minMaxBuffer);
        planetTextureCompute.SetBuffer(0, "vertices", vertexBuffer);


        //Run Compute
        planetTextureCompute.Dispatch(0, vertices.Length / resolution, 1, 1);

        //Retrieve Data
        noiseValueBuffer.GetData(noiseValues);
        minMaxBuffer.GetData(minMaxNoiseValues);

        //Release Buffers 
        noiseValueBuffer.Dispose();
        minMaxBuffer.Dispose();
        vertexBuffer.Dispose();

        minMaxNoise.x = minMaxNoiseValues[0];
        minMaxNoise.y = minMaxNoiseValues[1];

        return minMaxNoise;
    }
    public void SetupPlanetTextureCompute(BiomeSettings settings)
    {

  
        planetTextureCompute.SetInt("numVertices", resolution * resolution);



        int noiseParamIndex = 1;
        foreach (var noiseLayer in settings.biomeNoiseSettings)
        {
            Vector4[] noiseParamVectors;

            noiseParamVectors = noiseLayer.active ? SetupBiomeSettings(noiseLayer) : SetupEmptyVectors();
            planetTextureCompute.SetVectorArray("noiseParams" + noiseParamIndex.ToString(), noiseParamVectors);
            noiseParamIndex++;

        }
        if (settings.biomeNoiseSettings.Length < 3)
        {
            return;
        }
        Vector4[] warpNoise = SetupBiomeSettings(settings.biomeNoiseSettings[2]);
        planetTextureCompute.SetVectorArray("warpNoise", warpNoise);
    }
    public Color[] SetupBiomesCompute(Color[] climateMapArray, Vector2 minMaxValues)
    {

        float[] minMaxNoiseVals = new float[2];
        minMaxNoiseVals[0] = minMaxValues.x;
        minMaxNoiseVals[1] = minMaxValues.y;


        calculateBiomesCompute.SetInt("numVertices", resolution * resolution);

        ComputeBuffer noiseValueBuffer = new ComputeBuffer(noiseValues.Length, sizeof(float));
        ComputeBuffer biomeMapBuffer = new ComputeBuffer(climateMapArray.Length, sizeof(float)*4);
        ComputeBuffer minMaxBuffer = new ComputeBuffer(minMaxNoiseVals.Length, sizeof(float));

        biomeMapBuffer.SetData(climateMapArray);
        noiseValueBuffer.SetData(noiseValues);
        minMaxBuffer.SetData(minMaxNoiseVals);

        calculateBiomesCompute.SetBuffer(0, "biomeMap", biomeMapBuffer);
        calculateBiomesCompute.SetBuffer(0, "noiseValues", noiseValueBuffer);
        calculateBiomesCompute.SetBuffer(0, "maxMinValues", minMaxBuffer);

        calculateBiomesCompute.Dispatch(0, climateMapArray.Length / resolution, 1, 1);


        biomeMapBuffer.GetData(climateMapArray);

        biomeMapBuffer.Dispose();
        noiseValueBuffer.Dispose();
        minMaxBuffer.Dispose();

        return climateMapArray;

    }
}



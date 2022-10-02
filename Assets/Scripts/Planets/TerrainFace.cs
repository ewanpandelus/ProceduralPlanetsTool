using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace : SphereFace
{
    private float closestPoint = int.MaxValue;
    private float farthestPoint = int.MinValue;
    public TerrainFace(ShapeSettings shapeSettings, BiomeSettings biomeSettings, Mesh mesh, int resolution, Vector3 localUp) 
        :base(shapeSettings, biomeSettings, mesh, resolution , localUp)
    {

        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
        climateMap = new ClimateMap(resolution);
        meshColors = new Color[resolution * resolution];
        BiomeClassifier.Initialise();


        this.shapeSettings = shapeSettings;
        this.biomeSettings = biomeSettings;

        computeRunner = new ComputeRunner(biomeSettings.biomeCalculationCompute,
            shapeSettings.planetHeightCompute, biomeSettings.planetTextureCompute, resolution);
    }


    public Vector2 ConstructMesh()
    {

        farthestPoint = int.MinValue;
        vertices = new Vector3[resolution * resolution];
        triangles = GenerateTriangles();
        GenerateMeshHeightWithComputeShader();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        return new Vector2(closestPoint, farthestPoint);
   }

 
   private void GenerateMeshHeightWithComputeShader()
   {
        computeRunner.SetupPlanetHeightCompute(shapeSettings);
        mesh.vertices = computeRunner.RunPlanetHeightCompute(vertices, ref closestPoint, ref farthestPoint);
   }
    public Vector2 CreateTexture(BiomeSettings biomeSettings)
    {
        computeRunner.SetupPlanetTextureCompute(biomeSettings);
        return computeRunner.RunPlanetTextureCompute(vertices);

    }
    public void CreateBiomes(Vector2 minMaxNoise)
    {
        mesh.colors = computeRunner.SetupBiomesCompute(climateMap.GetClimateMap(), minMaxNoise);
    }


}


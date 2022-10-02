using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereFace
{
    protected Mesh mesh;


    protected int resolution;
    protected Vector3 localUp;
    protected Vector3 axisA;
    protected Vector3 axisB;

    protected Color[] meshColors;


    protected Vector3[] vertices;
    protected int[] triangles;

    protected ComputeRunner computeRunner;
    protected BiomeSettings biomeSettings;
    protected ShapeSettings shapeSettings;
    protected ClimateMap climateMap;

    public SphereFace(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
    }

    public SphereFace(ShapeSettings shapeSettings, BiomeSettings biomeSettings, Mesh mesh, int resolution, Vector3 localUp)
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


    protected int[] GenerateTriangles()
    {
        triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int index = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[index] = pointOnUnitSphere;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = index;
                    triangles[triIndex + 1] = index + resolution + 1;
                    triangles[triIndex + 2] = index + resolution;

                    triangles[triIndex + 3] = index;
                    triangles[triIndex + 4] = index + 1;
                    triangles[triIndex + 5] = index + resolution + 1;
                    triIndex += 6;

                }
            }
        }
        return triangles;
    }

  


}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    [SerializeField] private GameObject waterBody;
    [SerializeField] private bool regenerateWaterMesh;
    [SerializeField] protected GraphicsManager graphicsManager;
    [SerializeField]
    protected Material planetMat;
    [SerializeField]
    [HideInInspector]
    protected MeshFilter[] meshFilters;
    protected SphereFace[] faces;
    [Range(2, 512)]
    public int res = 10;


    public ShapeSettings shapeSettings;
    public BiomeSettings biomeSettings;
    public bool shapeSettingsFoldout;
    public bool biomeSettingsFoldout;
    public bool autoUpdate = false;
    private float closestPoint;
    private float farthestPoint;

    private float minNoise;
    private float maxNoise;


    private void Start()
    {
        planetMat.SetVector("_Position", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
    }
    protected virtual void Initialise()
    {

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        faces = new TerrainFace[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject(gameObject.transform.name);
                meshObj.transform.parent = gameObject.transform;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            faces[i] = new TerrainFace(shapeSettings, biomeSettings,  meshFilters[i].sharedMesh, res, directions[i]);
        }
       
    }
    public virtual void GeneratePlanet(bool generate)
    {
        if (generate || autoUpdate)
        {
          
            Initialise();
            GenerateMesh();
            UpdateShaderVariables();
            if (regenerateWaterMesh)
            {
                RegenerateWaterMesh();
            }
        }
    }
    protected virtual void GenerateMesh()
    {
        closestPoint = int.MaxValue;
        farthestPoint = int.MinValue;
        foreach (TerrainFace face in faces)
        {
            Vector2 distanceValues = face.ConstructMesh();
            if (distanceValues.x < closestPoint)
            {
                closestPoint = distanceValues.x;
            }
            if (distanceValues.y > farthestPoint)
            {
                farthestPoint = distanceValues.y;
            }

        }
     
    }
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialise();
            GenerateMesh();
        }
    }
    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialise();
            GenerateMesh();
            GenerateColours();
        }
    }
    public void GenerateColours()
    {
        CreateTerrainTextures();
        CreateTerrainBiomes();
    }
    private void CreateTerrainTextures()
    {
        minNoise = int.MaxValue;
        minNoise = int.MinValue;
        foreach (TerrainFace face in faces)
        {
            Vector2 noiseValues = face.CreateTexture(biomeSettings);
            if (noiseValues.x < minNoise)
            {
                minNoise = noiseValues.x;
            }
            if (noiseValues.y > minNoise)
            {
                minNoise = noiseValues.y;
            }

        }
    }
    private void CreateTerrainBiomes()
    {
        foreach (TerrainFace face in faces)
        {
           face.CreateBiomes(new Vector2(minNoise, maxNoise));
        }
    }
    protected virtual void UpdateShaderVariables() 
    { 
        planetMat.SetVector("_Position", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
        planetMat.SetVector("_LightDir", graphicsManager.GetDirectionToSun());
        planetMat.SetFloat("_MinDistance", closestPoint);
        planetMat.SetFloat("_MaxDistance", farthestPoint);
    }

    private void RegenerateWaterMesh()
    {
        WaterBody waterBodyObj = waterBody.GetComponent<WaterBody>();
        waterBodyObj.SetMinHeight(closestPoint);
        waterBodyObj.SetMaxHeight(farthestPoint);
        waterBodyObj.GeneratePlanet(true);
    }
}

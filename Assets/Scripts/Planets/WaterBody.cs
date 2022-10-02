using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class WaterBody : Planet
{
    [SerializeField] private float _percentageOceanHeight;
    [SerializeField] private ComputeShader _waterHeightCompute;
    [SerializeField] private Material _waterMat;
    private float minHeight = 0;
    private float maxHeight = 0;
    
    public override void GeneratePlanet(bool generate)
    {
        if (generate)
        {

            Initialise();
            GenerateMesh();
        }
    }
    protected override void Initialise()
    {

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        faces = new WaterFace[6];
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
            faces[i] = new WaterFace(meshFilters[i].sharedMesh, res, directions[i], _waterHeightCompute, _percentageOceanHeight);
        }

    }
    protected override void GenerateMesh()
    {
        foreach (WaterFace face in faces)
        {
            face.ConstuctMesh(minHeight, maxHeight);
        }
        UpdateShaderVariables();
    }
    protected override void UpdateShaderVariables()
    {
        _waterMat.SetVector("_Position", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
        _waterMat.SetVector("_LightDir", graphicsManager.GetDirectionToSun());
    }

    public void SetMinHeight(float minHeight) => this.minHeight = minHeight;
    public void SetMaxHeight(float maxHeight) => this.maxHeight = maxHeight;

}

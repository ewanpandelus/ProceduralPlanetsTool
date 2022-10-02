

using UnityEngine;

class WaterFace : SphereFace
{
    private ComputeShader distanceFromCentreCompute;
    private float percentageHeight;
    public WaterFace( Mesh mesh, int resolution, Vector3 localUp, ComputeShader computeShader, float percentageHeight)
    : base( mesh, resolution, localUp)
    {

        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        this.axisB = Vector3.Cross(localUp, axisA);
        this.distanceFromCentreCompute = computeShader;
        this.percentageHeight = percentageHeight;
  

    }
    public void ConstuctMesh(float minHeight, float maxHeight)
    {
        vertices = new Vector3[resolution * resolution];
        triangles = GenerateTriangles();
        UseComputeForHeight(minHeight, maxHeight);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    public void UseComputeForHeight(float minHeight, float maxHeight)
    {
     
        int vectorSize = sizeof(float) * 3;

        distanceFromCentreCompute.SetInt("numVertices", resolution * resolution);
        distanceFromCentreCompute.SetFloat("oceanHeightPercentage", percentageHeight);
        distanceFromCentreCompute.SetFloat("minHeight", minHeight);
        distanceFromCentreCompute.SetFloat("maxHeight", maxHeight);


        //Setup Buffers
        ComputeBuffer vertexBuffer = new ComputeBuffer(vertices.Length, vectorSize);


        vertexBuffer.SetData(vertices);


        distanceFromCentreCompute.SetBuffer(0, "vertices", vertexBuffer);


        //Run Compute
        distanceFromCentreCompute.Dispatch(0, vertices.Length / resolution, 1, 1);

        vertexBuffer.GetData(vertices);
        vertexBuffer.Dispose();

        mesh.vertices = vertices;
    }
}

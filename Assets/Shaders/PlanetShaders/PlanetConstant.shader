

Shader "Unlit/PlanetConstant"
{
    Properties
    {



          [HDR] _Biome1Colour("Biome 1 Colour", Color) = (0,1,0,1)
          [HDR] _Biome2Colour("Biome 2 Colour", Color) = (0,1,0,1)
          [HDR] _Biome3Colour("Biome 3 Colour", Color) = (0,1,0,1)
          [HDR] _MountainColour("Mountain Colour", Color) = (0,1,0,1)
          [HDR] _OppNoiseColour("OppNoiseColour", Color) = (0,1,0,1)

          [HDR] _AmbientLight("Ambient Light", Color) = (0,1,0,1)
          [HDR] _DiffuseLight("Diffuse Light", Color) = (0,1,0,1)
           _BlendWeight("Blend Weight", Float) = 1
           _TexScaleNormal("TexScale", Float) = 1
           _TexScaleNormal2("TexScale", Float) = 1
           _NoiseMapScale("NoiseMapScale", Float) = 1
           _Position("Planet Position", Vector) = (0,0,0,0)
           _LightDir("Light Direction", Vector) = (0,0,0)
            normalMap("Texture", 2D) = "white" {}
            normalMap2("Texture", 2D) = "white" {}
            noiseMap("Texture", 2D) = "white" {}


     

    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "DisableBatching" = "true"}
        LOD 100
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"   // Adds planet to Z buffer 

         Pass
         {

             ZWrite On
             ZTest On

             CGPROGRAM
             #pragma vertex vert



        #pragma fragment frag 
   //     #pragma geometry geom



        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"
        #include "../Utilities/Triplanar.cginc"
        #include "../Utilities/Math.cginc"


        float4 _WaterColour;
        float4 _SandColour;
        float4 _SnowColour;
        float4 _Biome1Colour;
        float4 _Biome2Colour;
        float4 _Biome3Colour;
        float4 _OppNoiseColour;
        float4 _MountainColour;

        float _MinDistance;
        float _MaxDistance;
        float _BlendWeight;
        float _TexScaleNormal;
        float _TexScaleNormal2;

        float _NoiseMapScale;





        float4 _AmbientLight;
        float4 _DiffuseLight;


        float4 _Position;
        float3 _LightDir;

        float4 _MainTex_ST;
        sampler2D normalMap;
        sampler2D normalMap2;
        sampler2D noiseMap;



        float DistanceFromCentre(float3 pos1, float3 pos2) {
            float distanceFromX = (pos1.x - pos2.x) * (pos1.x - pos2.x);
            float distanceFromY = (pos1.y - pos2.y) * (pos1.y - pos2.y);
            float distanceFromZ = (pos1.z - pos2.z) * (pos1.z - pos2.z);

            return (distanceFromX + distanceFromY + distanceFromZ);
        }
       
  
        struct MeshData
        {
            float4 vertex : POSITION;
            float3 normals: NORMAL;
            float2 uv : TEXCOORD0;
            float4 colour : COLOR;
            float4 tangent: TANGENT;

        };

        struct Interpolators
        {
            float3 normal:TEXCOORD0;
            float2 uv : TEXCOORD1;
            float4 vertex : SV_POSITION;
            float3 worldPos :TEXCOORD2;
            float distance : TEXCOORD3;
            float4 colour : COLOR;
            float3 viewVector: TEXCOORD4;




        };



       float4 OverwriteColour(float4 colour)
       {
           float average = (colour.x + colour.y + colour.z) / 3.0;
           return float4(average, average, average, 1);
       }
        Interpolators vert(MeshData v)
        {

            Interpolators o;

            o.colour = v.colour;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.distance = length(o.worldPos - _Position.xyz);
            o.uv = v.uv;
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;


            // Set output properties
            o.normal = UnityObjectToWorldNormal(normalize(v.normals));
            o.worldPos = worldPos;
            o.vertex = UnityObjectToClipPos(v.vertex);

            float4 screenPos = ComputeScreenPos(o.vertex);

            float3 viewVector = mul(unity_CameraInvProjection, float4((screenPos.xy / screenPos.w) * 2 - 1, 0, -1));
            o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));



            return o;
        }


        float4 frag(Interpolators i) : SV_Target
        {


        
            _LightDir = normalize(_LightDir);
   
        
            float4 noiseMapTriplanar = triplanar(i.worldPos, i.normal, _NoiseMapScale, noiseMap);
            float3 lightingNormal = triplanarNormal(i.worldPos, i.normal, _TexScaleNormal, 0, normalMap);
            float3 lightingNormal2 = triplanarNormal(i.worldPos, i.normal, _TexScaleNormal2, 0, normalMap2);
              


            float lightShading = saturate(dot(lightingNormal, _LightDir));
            float lightShading2 = saturate(dot(lightingNormal2, _LightDir));
              

            _Biome3Colour = lerp(_Biome3Colour, _OppNoiseColour, noiseMapTriplanar.r);

            float t = InverseLerp(_MinDistance, _MaxDistance, i.distance);
            float4 allBiomesColour;
              
            if (i.colour.r < 0.5) {
                allBiomesColour = lerp(_Biome1Colour, _Biome2Colour, i.colour*2);
            }
            else if(i.colour.r>=0.5) {
                allBiomesColour = lerp(_Biome2Colour, _Biome3Colour, (i.colour - 0.5)*2);
            }
            if (i.colour.r < 0.5) {
                allBiomesColour *= lerp(0.8, 1.3, abs(sin(_Time.y)));
            }
    
            float4   colour;
            colour = _AmbientLight;
            colour += (t >= 0.01) * (_DiffuseLight * lerp(lightShading, lightShading2, noiseMapTriplanar)); //adding ambient
            colour += (t < 0.01) * (_DiffuseLight * lightShading2);
          
            
            return colour * allBiomesColour;




         }
          ENDCG
      }
    }
}

//[maxvertexcount(3)]
//void geom(triangle Interpolators  input[3], inout TriangleStream<g2f> triStream)
//{
//    g2f o;
//    float3 normal = normalize(cross(input[1].vertex - input[0].vertex, input[2].vertex - input[0].vertex));
//    for (int i = 0; i < 3; i++)
//    {

//        float3 viewVector = mul(unity_CameraInvProjection, float4(input[i].uv * 2 - 1, 0, -1));
//        o.viewDir = mul(unity_CameraToWorld, float4(viewVector, 0));
//        float offset = sin(_Time.y / 2) * 0.1f;

//        o.vertex = UnityObjectToClipPos(input[i].vertex);
//        if (offset > 0) {
//            //  o.vertex.xyz += normal * offset;
//          }


//          o.colour = input[i].colour;

//          o.worldPos = input[i].worldPos;
//          o.distance = input[i].distance;
//          o.normal = input[i].normal;

//          o.uv = input[i].uv;


//          triStream.Append(o);
//      }
//      triStream.RestartStrip();

//  }
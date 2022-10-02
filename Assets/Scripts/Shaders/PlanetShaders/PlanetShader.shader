

Shader "Unlit/PlanetShader"
{

    Properties
    {

        
          [HDR] _MountainColour("Mountain Colour", Color) = (0,1,0,1)
          [HDR] _SnowColour("Snow Colour", Color) = (0,1,0,1)


          [HDR] _Biome1Colour("Biome 1 Colour", Color) = (0,1,0,1)
          [HDR] _Biome2Colour("Biome 2 Colour", Color) = (0,1,0,1)
          [HDR] _Biome3Colour("Biome 3 Colour", Color) = (0,1,0,1)

          [HDR] _AmbientLight("Ambient Light", Color) = (0,1,0,1)
          [HDR] _DiffuseLight("Diffuse Light", Color) = (0,1,0,1)

           _MinDistance("Min Distance", Float) = 1
           _MaxDistance("Max Distance", Float) = 1
           _BlendWeight("Blend Weight", Float) = 1
           _TexScaleNormal("TexScale", Float) = 1
           _TexScaleNormal2("TexScale", Float) = 1
       

           _NoiseMapScale("Noise Scale", Float) = 1





            


            
           _Position("Planet Position", Vector)  =(0,0,0,0)
           _LightDir("Light Direction", Vector) = (0,0,0)



            normalMap("Texture", 2D) = "white" {}
            normalMap2("Texture", 2D) = "white" {}
            normalMap3("Texture", 2D) = "white" {}

            noiseMap("Texture", 2D) = "white" {}




    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "AlphaTest" "DisableBatching" = "true"}
        LOD 100
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        Pass
        {

            ZWrite On
            ZTest On
              
            CGPROGRAM
            #pragma vertex vert
            //#pragma multi_compile_shadowcaster

            #pragma fragment frag 
       //     #pragma geometry geom
        //#pragma surface surf Standard fullforwardshadows vertex:vert

        #define TAU 6.283185
        #define OCTAVES 5
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"
        #include "../Utilities/Triplanar.cginc"


        float4 _MountainColour;
        float4 _SnowColour;
        float4 _Biome1Colour;
        float4 _Biome2Colour;
        float4 _Biome3Colour;


        float _MinDistance;
        float _MaxDistance;
        float _BlendWeight;
        float _TexScaleNormal;
        float _TexScaleNormal2;
       
        float _NoiseMapScale;

        float _ColourWash;
        float _SpecularExponent;
        float _Smoothness;
        float _Gloss;



      
        float4 _AmbientLight;
        float4 _DiffuseLight;


        float4 _Position;
        float3 _LightDir;

        float4 _MainTex_ST;
        sampler2D _CameraDepthTexture;
        sampler2D normalMap;
        sampler2D normalMap2;
        sampler2D normalMap3;
        sampler2D noiseMap;


   
        float DistanceFromCentre(float3 pos1, float3 pos2) {
            float distanceFromX = (pos1.x - pos2.x) * (pos1.x - pos2.x);
            float distanceFromY = (pos1.y - pos2.y) * (pos1.y - pos2.y);
            float distanceFromZ = (pos1.z - pos2.z) * (pos1.z - pos2.z);

            return (distanceFromX + distanceFromY + distanceFromZ);
        }
        float InverseLerp(float u, float v, float value)
        {
            return saturate((value - u) / (v - u));
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

    
       float4 OverwriteColour(float4 colour) {
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
            //o.vertex = UnityObjectToClipPos(v.vertex + float4(1, 1, 1, 0) * sin(_Time.x * 50) * 0.05);
            float4 screenPos = ComputeScreenPos(o.vertex);

            float3 viewVector = mul(unity_CameraInvProjection, float4((screenPos.xy / screenPos.w) * 2 - 1, 0, -1));
            o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));



            return o;
        }
   
      

        
        float4 GradientColour(float t, float4 allBiomesColour) {
         
            if (t < 0.5) {
                return allBiomesColour;
            }
            else if (t > 0.5 && t < 0.7) {
                return lerp(allBiomesColour, _MountainColour, (t - 0.5) / 0.2);
            }
            else if (t >= 0.7 && t < 0.8) {
                return lerp(_MountainColour, _SnowColour, (t - 0.7) / 0.1);
            }
            else {
                return _SnowColour;
            }
        }
       
   
        float calculateSpecular(float3 normal, float3 viewDir, float smoothness) {

            float specularAngle = acos(dot(normalize(_LightDir - viewDir), normal));
            float specularExponent = specularAngle / smoothness;
            float specularHighlight = exp(-specularExponent * specularExponent);
            return specularHighlight;
        }
        float4 frag(Interpolators i) : SV_Target
        {

         

            _LightDir = normalize(_LightDir);
            float3 viewDir = normalize(i.viewVector);

            // -------- Specularity --------
       


            float3 reflection;
            float4 specular;
            float specularPower = 8;




            float4 noiseMapTriplanar = triplanar(i.worldPos, i.normal, _NoiseMapScale, noiseMap);

            float3 lightingNormal = triplanarNormal(i.worldPos, i.normal, _TexScaleNormal, 0, normalMap);
            float3 lightingNormal2 = triplanarNormal(i.worldPos, i.normal, _TexScaleNormal2, 0, normalMap2);
            float3 lightingNormal3 = triplanarNormal(i.worldPos, i.normal, _TexScaleNormal2, 0, normalMap3);


            float lightShading = saturate(dot(lightingNormal, _LightDir));
            float lightShading2 = saturate(dot(lightingNormal2, _LightDir));
            float lightShading3 = saturate(dot(lightingNormal3, _LightDir));
            float normalLighting = saturate(dot(i.normal, _LightDir));
            float4 allBiomesColour = lerp(_Biome1Colour, _Biome2Colour, i.colour.r);


  
            float t = InverseLerp(_MinDistance, _MaxDistance, i.distance);

        
     
    

   
          
            float4   colour;
            colour = _AmbientLight;
            colour +=(t >= 0.01) * (_DiffuseLight * normalLighting); //adding ambient
            colour += (t < 0.01) * (_DiffuseLight * normalLighting);
            colour *= GradientColour(t, allBiomesColour);
           


       
            return colour;
           
        
  
       
       }
       ENDCG
   }
    }
}

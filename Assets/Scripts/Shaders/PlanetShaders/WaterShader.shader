

Shader "Unlit/Water"
{
    Properties
    {

          [HDR] _DeepColour("Deep Colour", Color) = (0,1,0,1)
          [HDR] _ShallowColour("Shallow Colour", Color) = (0,1,0,1)
     

          [HDR] _AmbientLight("Ambient Light", Color) = (0,1,0,1)
          [HDR] _DiffuseLight("Diffuse Light", Color) = (0,1,0,1)

           _LowerTransparencyLimit("Lower Transparency Limit", Float) = 1
           _UpperTransparencyLimit("Upper Transparency Limit", Float) = 1
           _UpperDepthLimit("Upper Depth Limit", Float) = 1



            


           _BlendWeight("Blend Weight", Float) = 1
           _TexScale("TexScale", Float) = 1
           _TexScaleNormal("TexScale", Float) = 1


           _DepthMultiplier("Depth Multiplier", Float) = 1




          _CameraPosition("Camera Position", Vector) = (0,0,-1)

           _Position("Planet Position", Vector) = (0,0,0,0)

           _LightDir("Light Direction", Vector) = (0,0,0)

            normalMap("Texture", 2D) = "white" {}
           _MainTex("Texture", 2D) = "white" {}

    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "AlphaTest" }

        LOD 100

        Pass
        {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -0.2,-0.2

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag





            #include "UnityCG.cginc"
            #include "../Utilities/Triplanar.cginc"

            float4 _DeepColour;
            float4 _ShallowColour;
  
            float4 _CameraPosition;
            float _LowerTransparencyLimit;
            float _UpperTransparencyLimit;
            float _UpperDepthLimit;

            float _BlendWeight;
            float _TexScale;
            float _DepthMultiplier;

            float _TexScaleNormal;


            float4 _AmbientLight;
            float4 _DiffuseLight;


            float4 _Position;
            float3 _LightDir;

            float4 _MainTex_ST;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D normalMap;


            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normals: NORMAL;
                float2 uv : TEXCOORD0;

            };

            struct Interpolators
            {
                float3 normal:TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 screenSpace : TEXCOORD2;
                float3 viewVector : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
         
            };
 
     
   
            Interpolators vert(MeshData v)
            {

                Interpolators o;
                float3 outwardVector = v.vertex.xyz - _Position;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
               // o.vertex += float4(outwardVector, 0) * (sin(_Time.y))/600;
                o.normal = UnityObjectToWorldNormal(v.normals);
     
         
            
                o.screenSpace = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
                return o;
            }




            float4 frag(Interpolators i) : SV_Target
            {

                float3 reflection;
                float4 specular;
                float specularPower = 32;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;


          


                float3 lightingNormal = triplanarNormal(i.worldPos, i.normal, _TexScaleNormal, 0, normalMap);

                float lightShading = saturate(dot(i.normal, _LightDir ));


                specular = pow(saturate(dot(reflection, i.viewVector)), specularPower);
         
                float nonLinearDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenSpace);
                float depth = LinearEyeDepth(nonLinearDepth);
                float dstToWater = i.screenSpace.w;
                float waterViewDepth = depth - dstToWater;
                float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;


                float4 waterColour = lerp(_ShallowColour, _DeepColour, 1- exp(-waterViewDepth* _DepthMultiplier));
             
                if (waterViewDepth > 0 && waterViewDepth < _UpperDepthLimit) {
                    waterColour.w = lerp(_LowerTransparencyLimit, _UpperTransparencyLimit,  waterViewDepth / _UpperDepthLimit);
                }
                if (waterViewDepth <= 0.0001) {
                    return 0;
                }
                float a = waterColour.a;
                waterColour *= (_DiffuseLight * lightShading); 
                waterColour.a = a;  //retains alpha after diffuse calculation - solves problems with camera movement 
                return waterColour;

            }
                
         
             ENDCG
         }
    }
}

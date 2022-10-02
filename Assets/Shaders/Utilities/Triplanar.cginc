float4 triplanar(float3 vertPos, float3 normal, float scale, sampler2D tex) {

    // Calculate triplanar coordinates
    float2 uvX = vertPos.zy * scale;
    float2 uvY = vertPos.xz * scale;
    float2 uvZ = vertPos.xy * scale;

    float4 colX = tex2D(tex, uvX);
    float4 colY = tex2D(tex, uvY);
    float4 colZ = tex2D(tex, uvZ);
    // Square normal to make all values positive + increase blend sharpness
    float3 blendWeight = normal * normal;
    // Divide blend weight by the sum of its components. This will make x + y + z = 1
    blendWeight /= dot(blendWeight, 1);
    return colX * blendWeight.x + colY * blendWeight.y + colZ * blendWeight.z;
}
float3 blend_rnm(float3 n1, float3 n2)
{
    n1.z += 1;
    n2.xy = -n2.xy;

    return n1 * dot(n1, n2) / n1.z - n2;
}
float3 triplanarNormal(float3 vertPos, float3 normal, float3 scale, float2 offset, sampler2D normalMap) {
    float3 absNormal = abs(normal);

    // Calculate triplanar blend
    float3 blendWeight = saturate(pow(normal, 4));
    // Divide blend weight by the sum of its components. This will make x + y + z = 1
    blendWeight /= dot(blendWeight, 1);

    // Calculate triplanar coordinates
    float2 uvX = vertPos.zy * scale + offset;
    float2 uvY = vertPos.xz * scale + offset;
    float2 uvZ = vertPos.xy * scale + offset;

    // Sample tangent space normal maps
    // UnpackNormal puts values in range [-1, 1] (and accounts for DXT5nm compression)
    float3 tangentNormalX = UnpackNormal(tex2D(normalMap, uvX));
    float3 tangentNormalY = UnpackNormal(tex2D(normalMap, uvY));
    float3 tangentNormalZ = UnpackNormal(tex2D(normalMap, uvZ));

    // Swizzle normals to match tangent space and apply reoriented normal mapping blend
    tangentNormalX = blend_rnm(half3(normal.zy, absNormal.x), tangentNormalX);
    tangentNormalY = blend_rnm(half3(normal.xz, absNormal.y), tangentNormalY);
    tangentNormalZ = blend_rnm(half3(normal.xy, absNormal.z), tangentNormalZ);

    // Apply input normal sign to tangent space Z
    float3 axisSign = sign(normal);
    tangentNormalX.z *= axisSign.x;
    tangentNormalY.z *= axisSign.y;
    tangentNormalZ.z *= axisSign.z;

    // Swizzle tangent normals to match input normal and blend together
    float3 outputNormal = normalize(
        tangentNormalX.zyx * blendWeight.x +
        tangentNormalY.xzy * blendWeight.y +
        tangentNormalZ.xyz * blendWeight.z
    );

    return outputNormal;
}
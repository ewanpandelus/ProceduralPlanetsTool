#include "/SimplexNoise.cginc"


float ridge(float h, float power, float offset) {

    h = abs(h);     // create creases
    h = offset - h; // invert so creases are at top
    h = pow(h, 2);
    return h;
}
float simplexNoise(float3 pos, float4 params[3]) {
    // Extract parameters for readability

    int numLayers = int(params[0].x);
    float persistence = params[0].y;
    float lacunarity = params[0].z;
    float scale = params[0].w;
    float multiplier = params[1].x;
    float3 offset = float3(params[1].z, params[1].w, params[2].x);

    // Sum up noise layers
    float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    for (int i = 0; i < numLayers; i++) {
        noiseSum +=  snoise((pos + offset) * frequency) * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
    return noiseSum * multiplier;
}

float ridgedMF(float3 p, float power, float4 params[3]) {
    int numLayers = int(params[0].x);
    float persistence = params[0].y;
    float lacunarity = params[0].z;
    float freq = params[0].w;
    float amp = params[1].x;
    float verticalShift = params[1].y;
    float3 offset3D = float3(params[1].z, params[1].w, params[2].x);

    float offset = 0.9;

    float sum = 0.0;

    float prev = 1.0;
    for (int i = 0; i < numLayers; i++) {
        float n = ridge(snoise((p+offset3D) * freq), power, offset);
        sum += n * amp;
        sum += n * amp * prev;  // scale by previous octave
        prev = n;
        freq *= lacunarity;
        amp *= persistence;
    }
    return sum;
}


float terracedNoise(float3 pos ,  float exponent, float4 params[3]) {
    float e0 = 1 * simplexNoise(pos, params);
    float e1 = 0.5 * (2*simplexNoise(pos, params));
    float e2 = 0.25 * (4*simplexNoise(pos, params));
    float e = (e0 + e1 + e2) / (1 + 0.5 + 0.25);
    return round(e * exponent) / exponent;
    

}









sampler uImage0 : register(s0); // The texture that you are currently drawing.
sampler uImage1 : register(s1); // A secondary texture that you can use for various purposes. This is usually a noise map.
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect; // The position and size of the currently drawn frame.
float2 uWorldPosition;
float uDirection;
float3 uLightSource; // Used for reflective dyes.
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 SunstoneDye(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy - (uTime * 4.5f)) / (uImageSize1 * 0.5f);
    float4 noise = tex2D(uImage1, noiseCoords);
    
    float luminosity = (color.r + color.g + color.b) / 3;
    float wave = cos(coords.x + uTime * 1.65f) * 0.25f + 0.75f;
    
    color.rgb = luminosity + (noise.rgb);
    color.rgb *= (((wave) * uColor) + ((1 - wave) * uSecondaryColor)) * 1.25f;
    return color * sampleColor * color.a;
}

technique Technique1
{
    pass SunstoneDyePass
    {
        PixelShader = compile ps_2_0 SunstoneDye();
    }
}
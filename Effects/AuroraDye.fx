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

float4 AuroraDye(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    /*float4 color = tex2D(uImage0, coords);
    
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy) / uImageSize1;
    float4 noise = tex2D(uImage1, noiseCoords);
    
    float luminosity = (color.r + color.g + color.b) / 3;
    float wave = sin(coords.x + uTime * 1.65f) * 0.5f + 0.5f;
    
    color.rgb *= luminosity + noise.rgb;
    color.rgb *= (((coords.x - wave) * uColor) + (1 - (coords.x - wave) * uSecondaryColor)) * 1.1f;
    return color * sampleColor * color.a;*/
    
    float4 color = tex2D(uImage0, coords);
    
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy + (uTime * 2.5f)) / (uImageSize1 * 0.4f);
    float4 noise = tex2D(uImage1, noiseCoords);
    
    float luminosity = (color.r + color.g + color.b) / 2;
    float wave = sin(coords.x + uTime * 1.65f) * 0.5f + 0.5f;
    
    color.rgb = luminosity + (noise.rgb/4);
    color.rgb *= (((wave) * uColor) + ((1 - wave) * uSecondaryColor)) * 1.15f;
    return color * sampleColor * color.a;
    
    //float wave = tan(coords.x + uTime * 1.65f) * 0.5f + 0.5f;
}

technique Technique1
{
    pass AuroraDyePass
    {
        PixelShader = compile ps_2_0 AuroraDye();
    }
    pass AuroraNoisePass
    {

    }
}
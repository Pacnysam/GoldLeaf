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
    
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy + (-uTime * 20.0f)) / (uImageSize1 * 0.75f);
    float2 noiseCoords2 = (coords * uImageSize0 - uSourceRect.xy + (uTime * 16.0f)) / (uImageSize1 * 0.625f);
    float4 noise = tex2D(uImage1, noiseCoords);
    float4 noise2 = tex2D(uImage1, noiseCoords2);
    
    float luminosity = (color.r + color.g + color.b) / 2;
    float wave = sin(coords.x + uTime * 2.25f) * 0.5f + 0.5f;
    
    //float wave = tan(coords.x + uTime * 1.65f) * 0.5f + 0.5f;
    
    float noiseLuminosity = (noise.r + noise.g + noise.b) / 3;
    float noiseLuminosity2 = (noise2.r + noise2.g + noise2.b) / 3;
    
    color.rgb = (luminosity) + (noise.rgb / 7) + (noise2.rgb / 7) + (clamp((sin(uTime * 2.5f) * 0.125f - 0.0675f), 0, 1) * 1.65f);
    color.rgb *= (((wave * 0.75f) * uColor) + ((1 - wave * 0.75f) * uSecondaryColor)) * 1.15f;
    
    color.rb += noiseLuminosity * 0.25f;
    color.gb += noiseLuminosity2 * 0.25f;
    color.gb -= noiseLuminosity * 0.1725f;
    color.rb -= noiseLuminosity2 * 0.1725f;
    
    //these are cool but should be their own dye
    //color.gb *= noiseLuminosity;
    //color.rb *= noiseLuminosity2;
    
    return color * sampleColor * color.a;
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
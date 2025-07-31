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
    //float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    //coords.x += sin((frameY * 36.0f) + (uTime * 3)) * 0.0175f;
    
    float4 color = tex2D(uImage0, coords);
    
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy - (uTime * 4.85f)) / (uImageSize1 * 0.175f);
    float2 noiseCoords2 = (coords * uImageSize0 - uSourceRect.xy - (-uTime * 3.25f)) / (uImageSize1 * 0.135f);
    float4 noise = tex2D(uImage1, noiseCoords);
    float4 noise2 = tex2D(uImage1, noiseCoords2);
    
    float luminosity = (color.r + color.g + color.b) / 3;
    float wave = cos(coords.x + uTime * 1.95f) * 0.65f + 0.65f;
    
    float noiseLuminosity = (noise.r + noise.g + noise.b) / 3;
    float noiseLuminosity2 = (noise2.r + noise2.g + noise2.b) / 3;
    
    color.rgb = (luminosity * 1.485f) + (noise.rgb * 0.135f) + (noise2.rgb * 0.135f);
    color.rgb *= (((wave) * uColor) + ((1 - wave) * uSecondaryColor)) * 1.3f;

    color.r += noiseLuminosity * 0.1f;
    color.rg += noiseLuminosity * 0.275f;
    color.g += noiseLuminosity2 * 0.215f;
    color.g -= noiseLuminosity * 0.165f;
    color.rg -= noiseLuminosity2 * 0.165f;
    
    /*color.g += noiseLuminosity * 0.275f;
    color.rg += noiseLuminosity2 * 0.27f;
    color.gb -= noiseLuminosity * 0.175f;
    color.rb -= noiseLuminosity2 * 0.165f;*/
    
    //color.g *= 0.9f;
    //color.b *= 0.645f;
    
    return color * sampleColor * color.a;
    //return (((color * 3.45f) * sampleColor) * luminosity) * color.a;
}

technique Technique1
{
    pass SunstoneDyePass
    {
        PixelShader = compile ps_2_0 SunstoneDye();
    }
}
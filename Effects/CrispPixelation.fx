sampler uImage0 : register(s0);
sampler uImage1 : register(s1); 
sampler uImage2 : register(s2); 
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 originalColor = tex2D(uImage0, coords);
    
    float2 offset = (0.0f, 0.0f);
    
    if (trunc((uScreenResolution.x * coords.x) % (2 * uZoom.x)) >= (1 * uZoom.x))
    {
        offset += float2(-1.0f, 0.0f);
    }
    if (trunc((uScreenResolution.y * coords.y) % (2 * uZoom.x)) >= (1 * uZoom.x))
    {
        offset += float2(0.0f, -1.0f);
    }
    
    float4 color = tex2D(uImage0, coords.xy + (offset.xy / uScreenResolution.xy));
    
    return lerp(originalColor, color, uOpacity);
}

technique Technique1
{
    pass CrispPixelationPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
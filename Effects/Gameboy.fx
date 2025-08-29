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

float4 GameboyShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 originalColor = tex2D(uImage0, coords);
    
    float2 offset = (0.0f, 0.0f);
    float pixelSize = 2.0f;
    
    if (trunc((uScreenResolution.x * coords.x) % pixelSize) >= (pixelSize / 2.0f))
    {
        offset += float2(-(pixelSize / 2.0f), 0.0f);
    }
    if (trunc((uScreenResolution.y * coords.y) % pixelSize) >= (pixelSize / 2.0f))
    {
        offset += float2(0.0f, -(pixelSize / 2.0f));
    }
    
    float4 color = tex2D(uImage0, coords.xy + (offset.xy / uScreenResolution.xy));
    
    float brightness = (color.r + color.g + color.b) / 3.0;
    
    if (brightness < 0.2275)
        color.rgb = float3(8, 24, 32) / 255.0;
    else if (brightness < 0.475)
        color.rgb = float3(52, 104, 86) / 255.0;
    else if (brightness < 0.735)
        color.rgb = float3(136, 192, 112) / 255.0;
    else
        color.rgb = float3(224, 248, 208) / 255.0;
    
    return lerp(originalColor, color, uOpacity);
}

technique Technique1
{
    pass GameboyPass
    {
        PixelShader = compile ps_2_0 GameboyShaderFunction();
    }
}
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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    uv = round(uv / (2 / uScreenResolution) + 0.5) * (2 / uScreenResolution);
    uv = round(uv / (2 / uScreenResolution) - 0.5) * (2 / uScreenResolution);
    float4 color = tex2D(uImage0, uv);

    float brightness = (color.x + color.y + color.z) / 3.0;
    
    if (brightness < 0.2275)
        color.rgb = float3(8, 24, 32) / 255.0;
    else if (brightness < 0.475)
        color.rgb = float3(52, 104, 86) / 255.0;
    else if (brightness < 0.735)
        color.rgb = float3(136, 192, 112) / 255.0;
    else
        color.rgb = float3(224, 248, 208) / 255.0;
    
    return color;
}

technique Technique1
{
    pass GameboyPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
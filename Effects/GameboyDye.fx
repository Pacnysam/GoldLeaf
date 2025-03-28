sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 ArmorMyShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float brightness = (color.r + color.g + color.b) / 3.0;
    
    if (color.a == 0)
        return color;
    else if (brightness < 0.235)
        color.rgb = float3(8, 24, 32) / 255.0;
    else if (brightness < 0.5)
        color.rgb = float3(52, 104, 86) / 255.0;
    else if (brightness < 0.75)
        color.rgb = float3(136, 192, 112) / 255.0;
    else
        color.rgb = float3(224, 248, 208) / 255.0;
    
    return color;
}

technique Technique1
{
    pass GameboyDyePass
    {
        PixelShader = compile ps_2_0 ArmorMyShader();
    }
}

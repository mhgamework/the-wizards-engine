texture colorMap; 
texture normalMap;
texture depthMap;
sampler colorSampler = sampler_state
{
    Texture = (colorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler depthSampler = sampler_state
{
    Texture = (depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
sampler normalSampler = sampler_state
{
    Texture = (normalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
 


struct VI_Position
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VO_Position
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
VO_Position VS_Passthrough(VI_Position input)
{
    VO_Position output;
    output.Position = float4(input.Position,1);
	output.TexCoord = input.TexCoord;
    return output;
}
struct PO_GBuffer
{
    float4 Color : COLOR0;
};
PO_GBuffer PS_Diffuse(VO_Position input)
{
    PO_GBuffer output;
	output.Color =float4( tex2D(colorSampler,input.TexCoord).rgb,0);
    return output;
}
PO_GBuffer PS_Normal(VO_Position input)
{
    PO_GBuffer output;
	output.Color = float4(tex2D(normalSampler,input.TexCoord).rgb,0);
    return output;
}
PO_GBuffer PS_Depth(VO_Position input)
{
    PO_GBuffer output;
	output.Color = float4(tex2D(depthSampler,input.TexCoord).r,tex2D(depthSampler,input.TexCoord).r,tex2D(depthSampler,input.TexCoord).r,tex2D(depthSampler,input.TexCoord).r);
    return output;
}
technique Diffuse
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS_Passthrough();
        PixelShader = compile ps_2_0 PS_Diffuse();
    }
}
technique Normal
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS_Passthrough();
        PixelShader = compile ps_2_0 PS_Normal();
    }
}
technique Depth
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS_Passthrough();
        PixelShader = compile ps_2_0 PS_Depth();
    }
}
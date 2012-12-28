#include <TestHelper.fx>

Texture2D colorMap;
Texture2D lightMap;
Texture2D ambientOcclusionMap;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
Texture2D hdrMap;
Texture2D finalMap;
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    output.TexCoord = input.TexCoord;
    return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
    float3 diffuseColor = colorMap.Sample(samLinear, input.TexCoord).rgb;
	
    float4 light = lightMap.Sample(samLinear,input.TexCoord);
	float4 ambientOcclusion = ambientOcclusionMap.Sample(samLinear,input.TexCoord).x;
    float3 diffuseLight = light.rgb *2;
    float specularLight = light.a;
	float3 ambient=float3(1,1,1)*0.5f;

	//ambientOcclusion = 0;

    return float4((diffuseColor * (diffuseLight+ambient*(1-ambientOcclusion)) + specularLight.xxx)+0.001,1);
}


technique10 Technique0
{
    pass Pass0
    {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunction() ) );
    }
}
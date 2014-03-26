
#include <TestHelper.fx>
#include <GBuffer.fx>


Texture2D inputMap;
float4x4 InvertProjection;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerState samPoint
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};
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

float4 FogPS ( VertexShaderOutput input ) : SV_TARGET0
	//uniform bool bEncodeLogLuv )    : COLOR0 
{
	// Sample the original HDR image 
	float3 vSample = inputMap.Sample(samLinear, input.TexCoord).rgb; 

	GBuffer_Raw g = SampleGBuffer(samLinear, input.TexCoord);

    //read depth
    float depthVal = g.Depth;
	//compute screen-space position
	float4 position;
	position.x = input.TexCoord.x * 2.0f - 1.0f;
	position.y = -(input.TexCoord.y * 2.0f - 1.0f);
	position.z = g.Depth;
	position.w = 1.0f;

    //transform to world space
    position = mul(position, InvertProjection);
    position /= position.w;
	float linearDepth = -position.z;
	
	float minDepth = 50;
	float maxDepth = 400;
	float3 fogColor = float3(180.0f/255.0f,224.0f/255.0f,227.0f/255.0f);

	float factor = saturate( (linearDepth - minDepth) / maxDepth);

	return t(lerp(vSample,fogColor,factor));
} 


technique10 Technique0
{
    pass Pass0
    {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, FogPS() ) );
    }
}
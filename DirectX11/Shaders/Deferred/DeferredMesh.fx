
#include <TestHelper.fx>
#include <GBuffer.fx>

matrix World;
single cbuffer sharedData
{
	matrix View;
	matrix Projection;
};

float specularIntensity = 8.0f;
float specularPower = 0.2f;
Texture2D txDiffuse;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
}
;
struct VertexShaderInput
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD;
}
;
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
}
;
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
	
	//output.Position = input.Position;
	//output.TexCoord = float2(0,0);
	//output.Normal = float3(1,0,0);
	//return output;
    matrix worldViewProj = mul(World,mul(View,Projection));
    //float4 worldPosition = mul(input.Position, World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);
	output.Position = mul(input.Position, worldViewProj);
	
	float farClip = 400;
	//output.Position.z = output.Position.z * output.Position.w / farClip;// Test: use linear depth
    output.TexCoord = input.TexCoord;
    //pass the texture coordinates further
    output.Normal = normalize(mul(input.Normal,(float3x3)World)); 
    //get normal into world space
    return output;
}


GBuffer PixelShaderFunction(VertexShaderOutput input)
{
    return CreateGBuffer(
		txDiffuse.Sample(samLinear, input.TexCoord),
		normalize(input.Normal),
		specularIntensity,
		specularPower);
}



technique10 Technique1
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunction() ) );
    }

}
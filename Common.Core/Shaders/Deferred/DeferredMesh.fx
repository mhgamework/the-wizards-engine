
#include <TestHelper.fx>
#include <GBuffer.fx>

cbuffer perObject
{
	matrix World;
}
single cbuffer sharedData
{
	matrix View;
	matrix Projection;
};



cbuffer perMaterial
{
	float specularIntensity = 0.1f;
	float specularPower = 0.2f;
	
};
float3 diffuseColor = float3(1,1,0); 
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
	
    matrix worldViewProj = mul(World,mul(View,Projection));
	output.Position = mul(input.Position, worldViewProj);
	
	float farClip = 400;
	//output.Position.z = output.Position.z * output.Position.w / farClip;// Test: use linear depth
    output.TexCoord = input.TexCoord;
    output.Normal = normalize(mul(input.Normal,(float3x3)World)); 
    return output;
}


GBuffer PixelShaderFunctionColored(VertexShaderOutput input)
{
    return CreateGBuffer(
		diffuseColor,
		normalize(input.Normal),
		specularIntensity,
		specularPower);
}
GBuffer PixelShaderFunctionTextured(VertexShaderOutput input)
{
    return CreateGBuffer(
		txDiffuse.Sample(samLinear, input.TexCoord),
		normalize(input.Normal),
		specularIntensity,
		specularPower);
}

float4 DepthOnlyVS(VertexShaderInput input) : SV_POSITION
{
    VertexShaderOutput output = (VertexShaderOutput)0;
	
    matrix worldViewProj = mul(World,mul(View,Projection)); // Should preshader


	return mul(input.Position, worldViewProj);
}


technique10 Textured
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunctionTextured() ) );
    }

}
technique10 Colored
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunctionColored() ) );
    }

}

technique10 DepthOnly
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, DepthOnlyVS() ) );
		SetPixelShader( NULL );
    }

}

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
	float4 diffuseColor = float4(0.5f,0.5f,0.5f,1);
	float specularIntensity = 0.1f;
	float specularPower = 0.2f;
};
Texture2D txDiffuse		: register(t0);
Texture2D txNormal		: register(t1);
Texture2D txSpecular	: register(t2);
SamplerState Sampler	: register(s0);

struct VertexShaderInput
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD;

//#ifdef NORMAL_MAPPING
	float4 Tangent : TANGENT;
//#endif
	
};
struct VertexShaderOutput
{
    float4 PositionCS : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 NormalWS : TEXCOORD1;

#ifdef NORMAL_MAPPING
	float3 PositionWS	: POSITIONWS;
	float3 TangentWS	: TANGENTWS;
	float3 BitangentWS	: BITANGENTSWS
#endif
	
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
	
	// World space normal
    float3 normalWS = normalize(mul(input.Normal,(float3x3)World)); 
	output.NormalWS = normalWS;

	float4 positionWS = mul (input.Position, World);

#ifdef NORMAL_MAPPING
	float3 tangentWS = normalize( mul ( input.Tangent.xyz, (float3x3)WorldMatrix ) );
	float3 bitangentWS = normalize( cross( normalWS, tangentWS ) ) * input.Tangent.w;

	output.PositionWS = positionWS.xyz;
	output.TangentWS = tangentWS;
	output.BitangentWS = bitangentWS;
#endif


	// Screen space Position and Texcoord
    matrix viewProj = mul(View,Projection); // TODO: check preshader (also check the world multiplication above, now 2 multiplications are done)
	output.PositionCS = mul(positionWS, viewProj);
    output.TexCoord = input.TexCoord;

    return output;
}


GBuffer PixelShaderFunction(VertexShaderOutput input)
{
	float4 diffuseAlbedo;
	float specularAlbedo;
	float3 normalWS;

#ifdef DIFFUSE_MAPPING
	diffuseAlbedo = txDiffuse.Sample(samLinear, input.TexCoord);
#else
	diffuseAlbedo = diffuseColor;
#endif

	// clip when alpha is below 95 %
	clip( diffuseAlbedo - 0.95f );

#ifdef SPECULAR_MAPPING
	specularAlbedo = txSpecular.Sample(samLinear, input.TexCoord).r;
#else
	specularAlbedo = specularIntensity;
#endif

#ifdef NORMAL_MAPPING
	// Normalize the tangent frame after interpolation
	float3x3 tangentFrameWS = float3x3( normalize( input.TangentWS ), normalize( input.BitangentWS ), normalize( input.NormalWS ) );

	// Sample the tangent-space normal map and decompress
	float3 normalTS = txNormal.Sample(samLinear, input.TexCoord).rgb;
	normalTS = normalize( normalTS * 2.0f - 1.0f );

	normalWS = mul( normalTS, tangentFrameWS );
#else
	normalWS = normalize(input.NormalWS);
#endif

    return CreateGBuffer(diffuseAlbedo,	normalWS,specularAlbedo,specularPower);
}

float4 DepthOnlyVS(VertexShaderInput input) : SV_POSITION
{
    VertexShaderOutput output = (VertexShaderOutput)0;
	
    matrix worldViewProj = mul(World,mul(View,Projection)); // Should preshader


	return mul(input.Position, worldViewProj);
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

technique10 DepthOnly
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, DepthOnlyVS() ) );
		SetPixelShader( NULL );
    }

}
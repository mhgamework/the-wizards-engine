float4 Color = float4(1,1,0,1);
#include <IncludeTest.fx>
struct VS_IN
{
	float3 pos : POSITION;
	float3 uv : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float3 uv : TEXCOORD;
};

Texture2D txDiffuse;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};


PS_IN FullScreenQuadVS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = float4(input.pos,1);
	output.uv = input.uv;
	
	return output;
}

float4 TestQuadTexturedPS( PS_IN input ) : SV_Target
{
	return Color+Color2;

}


technique10 TestAutoReload
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, FullScreenQuadVS() ) );
		SetPixelShader( CompileShader( ps_4_0, TestQuadTexturedPS() ) );

	}
}

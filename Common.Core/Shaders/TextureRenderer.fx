float2 OffsetSS;
float2 Scale;
float4 Color;
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
	


	output.pos = float4((input.pos.x + 1) * Scale.x + OffsetSS.x - 1,(input.pos.y - 1) * Scale.y - OffsetSS.y + 1,input.pos.z,1);
	output.uv = input.uv;
	
	return output;
}

float4 TexturedPS( PS_IN input ) : SV_Target
{
	float4 col = txDiffuse.Sample( samLinear, input.uv.xy );
	return  col;
}
float4 ColoredPS( PS_IN input ) : SV_Target
{
	
	return  Color;
}


technique10 TextureRenderer
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, FullScreenQuadVS() ) );
		SetPixelShader( CompileShader( ps_4_0, TexturedPS() ) );

	}
}
technique10 ColorRenderer
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, FullScreenQuadVS() ) );
		SetPixelShader( CompileShader( ps_4_0, ColoredPS() ) );

	}
}

#include <Common.fx>
#include <CSM/CSMCommon.fx>

float4x4	g_matWorld;
float4x4	g_matWorldIT;
float4x4	g_matViewProj;
float4x4	g_matProj;

float		g_fFarClip;

//float2		g_vOcclusionTextureSize;

float4		g_vFrustumCornersVS [4]; // Was float3

bool		g_bShowSplitColors = true;


Texture2D DepthTexture;


struct PS_IN
{
	float4 vPositionCS				: SV_POSITION;
	float2 vTexCoord				: TEXCOORD0;
	float3 vFrustumCornerVS			: TEXCOORD1;
};

// Vertex shader for rendering the full-screen quad used for calculating
// the shadow occlusion factor.
PS_IN ShadowTermVS (	in float3 in_vPositionOS				: POSITION,
					in float3 in_vTexCoordAndCornerIndex	: TEXCOORD0)
{
	PS_IN ret;

	// Offset the position by half a pixel to correctly align texels to pixels
	ret.vPositionCS.x = in_vPositionOS.x;
	ret.vPositionCS.y = in_vPositionOS.y;
	ret.vPositionCS.z = in_vPositionOS.z;
	ret.vPositionCS.w = 1.0f;
	
	// Pass along the texture coordiante and the position of the frustum corner
	ret.vTexCoord = in_vTexCoordAndCornerIndex.xy;
	ret.vFrustumCornerVS = g_vFrustumCornersVS[in_vTexCoordAndCornerIndex.z].xyz;

	return ret;
}	
// Pixel shader for computing the shadow occlusion factor
float4 ShadowTermPS( PS_IN input, uniform int iFilterSize	)	: SV_TARGET0
{
	// Reconstruct view-space position from the depth buffer
	float fPixelDepth = DepthTexture.Sample(samPoint, input.vTexCoord).r;
	//input.vFrustumCornerVS.z = input.vFrustumCornerVS.z;
	//float4 vPositionVS = float4(fPixelDepth * input.vFrustumCornerVS, 1.0f);	
	


	float4 position;
	position.x = input.vTexCoord.x * 2.0f - 1.0f;
	position.y = -(input.vTexCoord.y * 2.0f - 1.0f);
	position.z = fPixelDepth;
	position.w = 1.0f;
	float4 vPositionVS = mul( position , g_matProj);
	vPositionVS /= vPositionVS.w; 
	


	int iSplit;

	float shadowTerm = CalculateCSMShadowTerm(vPositionVS, iSplit, iFilterSize);

	float3 vColor = GetSplitColor(iSplit);
	// If we're not showing the split colors, set it back to 1 to remove the coloring
	if (!g_bShowSplitColors)
		vColor = 1;	
			
	return float4(shadowTerm * vColor, 1);
}



BlendState NoBlend
{
	BlendEnable[0] = false;
};
RasterizerState NoCull
{
	CullMode = NONE;
};
DepthStencilState NoDepth
{
	DepthEnable = false;
};

technique10 CreateShadowTerm2x2PCF
{
    pass p0
    {
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		SetRasterizerState( NoCull );
		SetDepthStencilState( NoDepth , 0);

		SetGeometryShader( NULL );
        SetVertexShader( CompileShader( vs_4_0, ShadowTermVS() ) );
        SetPixelShader( CompileShader( ps_4_0, ShadowTermPS(2) ) );	
    }
}

technique10 CreateShadowTerm3x3PCF
{
    pass p0
    {
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		SetRasterizerState( NoCull );
		SetDepthStencilState( NoDepth , 0);

		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, ShadowTermVS() ) );
        SetPixelShader( CompileShader( ps_4_0, ShadowTermPS(3) ) );	
    }
}

technique10 CreateShadowTerm5x5PCF
{
    pass p0
    {
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		SetRasterizerState( NoCull );
		SetDepthStencilState( NoDepth , 0);

		SetGeometryShader( NULL );
        SetVertexShader( CompileShader( vs_4_0, ShadowTermVS() ) );
        SetPixelShader( CompileShader( ps_4_0, ShadowTermPS(5) ) );	
    }
}

technique10 CreateShadowTerm7x7PCF
{
    pass p0
    {
		SetBlendState( NoBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		SetRasterizerState( NoCull );
		SetDepthStencilState( NoDepth , 0);

		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, ShadowTermVS() ) );
        SetPixelShader( CompileShader( ps_4_0, ShadowTermPS(7) ) );	
    }
}



//========================================================================
//
//	DeferredShadowMaps
//
//		by MJP  (mpettineo@gmail.com)
//		12/14/08      
//
//========================================================================
//
//	File:		Depth.fx
//
//	Desc:		Outputs normalized view-space depth
//
//========================================================================

float4x4 g_matWorld : WORLD;
float4x4 g_matWorldIT;
float4x4 g_matView : VIEW;
float4x4 g_matProj : PROJECTION;

float g_fFarClip = 100.0f;


void DepthVS(	in float4 in_vPositionOS	: POSITION,
				out float4 out_vPositionCS	: POSITION,
				out float  out_fDepthVS		: TEXCOORD0	)
{	
	// Figure out the position of the vertex in view space and clip space
	float4x4 matWorldView = mul(g_matWorld, g_matView);
	float4 vPositionVS = mul(in_vPositionOS, matWorldView);
    out_vPositionCS = mul(vPositionVS, g_matProj);
	out_fDepthVS = vPositionVS.z;
}


float4 DepthPS(in float in_fDepthVS : TEXCOORD0) : COLOR0
{
	// Negate and divide by distance to far clip (so that depth is in range [0,1])
    
	float fDepth =-in_fDepthVS/g_fFarClip;
	
    return float4(fDepth, 1.0f, 1.0f, 1.0f);
}

technique LinearDepth
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 DepthVS();
        PixelShader = compile ps_2_0 DepthPS();
        
        ZEnable = true;
        ZWriteEnable = true;
        //CullMode = CCW;
        FillMode = Solid;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}



//========================================================================
//
//	DeferredShadowMaps
//
//		by MJP  (mpettineo@gmail.com)
//		12/14/08      
//
//========================================================================
//
//	File:		ShadowMap.fx
//
//	Desc:		Contains shaders used for generating and applying deferred
//				shadow maps.  
//
//========================================================================
float4x4	g_matWorld;
float4x4	g_matWorldIT;
float4x4	g_matViewProj;

float4x4	g_matInvView;
float		g_fFarClip;

static const int NUM_SPLITS = 4;
float4x4	g_matLightViewProj [NUM_SPLITS];
float2		g_vClipPlanes[NUM_SPLITS];
float2		g_vShadowMapSize;
float2		g_vOcclusionTextureSize;

float3		g_vFrustumCornersVS [4];

bool		g_bShowSplitColors = true;

static const float BIAS = 0.006f;

texture DepthTexture;
sampler2D DepthTextureSampler = sampler_state
{
    Texture = <DepthTexture>;
    MinFilter = point;
    MagFilter = point;
    MipFilter = none;
};

texture ShadowMap;
sampler2D ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
    MinFilter = point; 
    MagFilter = point; 
    MipFilter = none; 
};



// Vertex shader for outputting light-space depth to the shadow map
void GenerateShadowMapVS(	in float4 in_vPositionOS	: POSITION,
							out float4 out_vPositionCS	: POSITION,
							out float2 out_vDepthCS		: TEXCOORD0	)
{
	// Figure out the position of the vertex in view space and clip space
	float4x4 matWorldViewProj = mul(g_matWorld, g_matViewProj);
    out_vPositionCS = mul(in_vPositionOS, matWorldViewProj);
	out_vDepthCS = out_vPositionCS.zw;
}

// Pixel shader for outputting light-space depth to the shadow map
float4 GenerateShadowMapPS(in float2 in_vDepthCS : TEXCOORD0) : COLOR0
{

	// Negate and divide by distance to far clip (so that depth is in range [0,1])
	float fDepth = in_vDepthCS.x / in_vDepthCS.y;			
		
    return float4(fDepth, 1, 1, 1); 
}



// Vertex shader for rendering the full-screen quad used for calculating
// the shadow occlusion factor.
void ShadowTermVS (	in float3 in_vPositionOS				: POSITION,
					in float3 in_vTexCoordAndCornerIndex	: TEXCOORD0,		
					out float4 out_vPositionCS				: POSITION,
					out float2 out_vTexCoord				: TEXCOORD0,
					out float3 out_vFrustumCornerVS			: TEXCOORD1	)
{
	// Offset the position by half a pixel to correctly align texels to pixels
	out_vPositionCS.x = in_vPositionOS.x - (1.0f / g_vOcclusionTextureSize.x);
	out_vPositionCS.y = in_vPositionOS.y + (1.0f / g_vOcclusionTextureSize.y);
	out_vPositionCS.z = in_vPositionOS.z;
	out_vPositionCS.w = 1.0f;
	
	// Pass along the texture coordiante and the position of the frustum corner
	out_vTexCoord = in_vTexCoordAndCornerIndex.xy;
	out_vFrustumCornerVS = g_vFrustumCornersVS[in_vTexCoordAndCornerIndex.z];
}	

// Calculates the shadow occlusion using bilinear PCF
float CalcShadowTermPCF(float fLightDepth, float2 vShadowTexCoord)
{
	float fShadowTerm = 0.0f;

	// transform to texel space
	float2 vShadowMapCoord = g_vShadowMapSize * vShadowTexCoord;
    
	// Determine the lerp amounts           
	float2 vLerps = frac(vShadowMapCoord);

	// Read in the 4 samples, doing a depth check for each
	float fSamples[4];	
	fSamples[0] = (tex2D(ShadowMapSampler, vShadowTexCoord).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[1] = (tex2D(ShadowMapSampler, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 0)).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[2] = (tex2D(ShadowMapSampler, vShadowTexCoord + float2(0, 1.0/g_vShadowMapSize.y)).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[3] = (tex2D(ShadowMapSampler, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 1.0/g_vShadowMapSize.y)).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
    
	// lerp between the shadow values to calculate our light amount
	fShadowTerm = lerp(lerp(fSamples[0], fSamples[1], vLerps.x), lerp( fSamples[2], fSamples[3], vLerps.x), vLerps.y);							  
								
	return fShadowTerm;								 
}

// Calculates the shadow term using PCF soft-shadowing
float CalcShadowTermSoftPCF(float fLightDepth, float2 vShadowTexCoord, int iSqrtSamples)
{
	float fShadowTerm = 0.0f;  
		
	float fRadius = (iSqrtSamples - 1.0f) / 2;
	float fWeightAccum = 0.0f;
	
	for (float y = -fRadius; y <= fRadius; y++)
	{
		for (float x = -fRadius; x <= fRadius; x++)
		{
			float2 vOffset = 0;
			vOffset = float2(x, y);				
			vOffset /= g_vShadowMapSize;
			float2 vSamplePoint = vShadowTexCoord + vOffset;			
			float fDepth = tex2D(ShadowMapSampler, vSamplePoint).x;
			float fSample = (fLightDepth <= fDepth + BIAS);
			
			// Edge tap smoothing
			float xWeight = 1;
			float yWeight = 1;
			
			if (x == -fRadius)
				xWeight = 1 - frac(vShadowTexCoord.x * g_vShadowMapSize.x);
			else if (x == fRadius)
				xWeight = frac(vShadowTexCoord.x * g_vShadowMapSize.x);
				
			if (y == -fRadius)
				yWeight = 1 - frac(vShadowTexCoord.y * g_vShadowMapSize.y);
			else if (y == fRadius)
				yWeight = frac(vShadowTexCoord.y * g_vShadowMapSize.y);
				
			fShadowTerm += fSample * xWeight * yWeight;
			fWeightAccum = xWeight * yWeight;
		}											
	}		
	
	fShadowTerm /= (iSqrtSamples * iSqrtSamples);
	fShadowTerm *= 1.55f;	
	
	return fShadowTerm;
}

// Pixel shader for computing the shadow occlusion factor
float4 ShadowTermPS(	in float2 in_vTexCoord			: TEXCOORD0,
						in float3 in_vFrustumCornerVS	: TEXCOORD1,
						uniform int iFilterSize	)	: COLOR0
{
	// Reconstruct view-space position from the depth buffer
	float fPixelDepth = tex2D(DepthTextureSampler, in_vTexCoord).r;
    //return float4(fPixelDepth,0,0,1);
	float4 vPositionVS = float4(fPixelDepth * in_vFrustumCornerVS, 1.0f);	
	//return vPositionVS;
	// Figure out which split this pixel belongs to, based on view-space depth.
	float4x4 matLightViewProj = g_matLightViewProj[0];
	float fOffset = 0;
	
	float3 vSplitColors [4];
	vSplitColors[0] = float3(1, 0, 0);
	vSplitColors[1] = float3(0, 1, 0);
	vSplitColors[2] = float3(0, 0, 1);
	vSplitColors[3] = float3(1, 1, 0);
	float3 vColor = vSplitColors[0];
	int iCurrentSplit = 0;
		
	// Unrolling the loop allows for a performance boost on the 360
#ifdef XBOX	
	[unroll(NUM_SPLITS - 1)]
#endif	
	for (int i = 1; i < NUM_SPLITS; i++)
	{
#ifdef XBOX		
		[flatten]
#endif		
		if (vPositionVS.z <= g_vClipPlanes[i].x && vPositionVS.z > g_vClipPlanes[i].y)
		{
			matLightViewProj = g_matLightViewProj[i];
			fOffset = i / (float)NUM_SPLITS;
			vColor = vSplitColors[i];
			iCurrentSplit = i;
		}
	}		
	
	// If we're not showing the split colors, set it back to 1 to remove the coloring
	if (!g_bShowSplitColors)
		vColor = 1;		
	
	// Determine the depth of the pixel with respect to the light
	float4x4 matViewToLightViewProj = mul(g_matInvView, matLightViewProj);
	float4 vPositionLightCS = mul(vPositionVS, matViewToLightViewProj);
	//return vPositionLightCS;
	float fLightDepth = vPositionLightCS.z / vPositionLightCS.w;	
	
	// Transform from light space to shadow map texture space.
    float2 vShadowTexCoord = 0.5 * vPositionLightCS.xy / vPositionLightCS.w + float2(0.5f, 0.5f);
    vShadowTexCoord.x = vShadowTexCoord.x / NUM_SPLITS + fOffset;
    vShadowTexCoord.y = 1.0f - vShadowTexCoord.y;
        
    // Offset the coordinate by half a texel so we sample it correctly
    vShadowTexCoord += (0.5f / g_vShadowMapSize);
    //return float4(vShadowTexCoord,0,1);
	//return float4((tex2D(ShadowMapSampler, vShadowTexCoord).x),0,0,1);
	// Get the shadow occlusion factor and output it
	float fShadowTerm = 0;
	if (iFilterSize == 2)
		fShadowTerm = CalcShadowTermPCF(fLightDepth, vShadowTexCoord);
	else
		fShadowTerm = CalcShadowTermSoftPCF(fLightDepth, vShadowTexCoord, iFilterSize);

	return float4(fShadowTerm * vColor, 1);
}


technique GenerateShadowMap
{
	pass p0
	{
		ZWriteEnable = true;
		ZEnable = true;		
		AlphaBlendEnable = false;
		FillMode = Solid;
		//CullMode = CCW;
		
		VertexShader = compile vs_2_0 GenerateShadowMapVS();
        PixelShader = compile ps_2_0 GenerateShadowMapPS();
	}
}

technique CreateShadowTerm2x2PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(2);	
    }
}

technique CreateShadowTerm3x3PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(3);	
    }
}

technique CreateShadowTerm5x5PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(5);	
    }
}

technique CreateShadowTerm7x7PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(7);	
    }
}



#include <Common.fx>

float4x4	g_matWorld;
float4x4	g_matWorldIT;
float4x4	g_matViewProj;
float4x4	g_matProj;

float4x4	g_matInvView;
float		g_fFarClip;

static const int NUM_SPLITS = 4;
float4x4	g_matLightViewProj [NUM_SPLITS];
float4		g_vClipPlanes[NUM_SPLITS]; // Was float2
float2		g_vShadowMapSize;
//float2		g_vOcclusionTextureSize;

float4		g_vFrustumCornersVS [4]; // Was float3

bool		g_bShowSplitColors = true;

static const float BIAS = 0.006f;

Texture2D DepthTexture;
Texture2D ShadowMap;

SamplerState samPoint
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

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
	fSamples[0] = (ShadowMap.Sample(samPoint, vShadowTexCoord).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[1] = (ShadowMap.Sample(samPoint, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 0)).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[2] = (ShadowMap.Sample(samPoint, vShadowTexCoord + float2(0, 1.0/g_vShadowMapSize.y)).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[3] = (ShadowMap.Sample(samPoint, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 1.0/g_vShadowMapSize.y)).x + BIAS < fLightDepth) ? 0.0f: 1.0f;  
    
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
			float fDepth = ShadowMap.Sample(samPoint, vSamplePoint).x;
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
float4 ShadowTermPS( PS_IN input, uniform int iFilterSize	)	: SV_TARGET0
{
	// Reconstruct view-space position from the depth buffer
	float fPixelDepth = DepthTexture.Sample(samPoint, input.vTexCoord).r;
	fPixelDepth = ConvertToLinearDepth(fPixelDepth, g_matProj);
	//return t(frac(-fPixelDepth*10000));

	float4 vPositionVS = float4(fPixelDepth * input.vFrustumCornerVS, 1.0f);	
	return t(frac(vPositionVS));
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
		
	for (int i = 1; i < NUM_SPLITS; i++)
	{
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



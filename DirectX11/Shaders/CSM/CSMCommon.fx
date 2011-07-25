static const int NUM_SPLITS = 4;
float4x4	g_matLightViewProj [NUM_SPLITS];
float4		g_vClipPlanes[NUM_SPLITS]; // Was float2
float4x4	g_matInvView;

float2		g_vShadowMapSize;
Texture2D ShadowMap;

SamplerState samPoint
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};
static const float BIAS = 0.006f;


// Calculates the shadow occlusion using bilinear PCF
float CalcShadowTermPCF(float fLightDepth, float2 vShadowTexCoord)
{


	float bias = -0.005 * fLightDepth; // Not sure this does anything
	//bias = -BIAS;

	float fShadowTerm = 0.0f;

	// transform to texel space
	float2 vShadowMapCoord = g_vShadowMapSize * vShadowTexCoord;
    
	// Determine the lerp amounts           
	float2 vLerps = frac(vShadowMapCoord);

	// Read in the 4 samples, doing a depth check for each
	float fSamples[4];	
	fSamples[0] = (ShadowMap.Sample(samPoint, vShadowTexCoord).x < fLightDepth  + bias) ? 0.0f: 1.0f;  
	fSamples[1] = (ShadowMap.Sample(samPoint, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 0)).x  < fLightDepth + bias) ? 0.0f: 1.0f;  
	fSamples[2] = (ShadowMap.Sample(samPoint, vShadowTexCoord + float2(0, 1.0/g_vShadowMapSize.y)).x < fLightDepth + bias) ? 0.0f: 1.0f;  
	fSamples[3] = (ShadowMap.Sample(samPoint, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 1.0/g_vShadowMapSize.y)).x  < fLightDepth + bias) ? 0.0f: 1.0f;  
    
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

float CalculateCSMShadowTerm(float4 vPositionVS, out int iCurrentSplit, uniform int iFilterSize)
{
	// Figure out which split this pixel belongs to, based on view-space depth.
	float4x4 matLightViewProj = g_matLightViewProj[0];
	float fOffset = 0;
	
	iCurrentSplit = 0;

	for (int i = 1; i < NUM_SPLITS; i++)
	{
		if (vPositionVS.z <= g_vClipPlanes[i].x && vPositionVS.z > g_vClipPlanes[i].y)
		{
			matLightViewProj = g_matLightViewProj[i];
			fOffset = i / (float)NUM_SPLITS;
			iCurrentSplit = i;
		}
	}		
	
	
	// Determine the depth of the pixel with respect to the light
	float4x4 matViewToLightViewProj = mul(g_matInvView, matLightViewProj);
	float4 vPositionLightCS = mul(vPositionVS, matViewToLightViewProj);
	//return vPositionLightCS;
	float fLightDepth = vPositionLightCS.z / vPositionLightCS.w;	
	
	// Transform from light space to shadow map texture space.
    float2 vShadowTexCoord = 0.5 * vPositionLightCS.xy / vPositionLightCS.w + float2(0.5f, 0.5f);
    vShadowTexCoord.x = vShadowTexCoord.x / NUM_SPLITS + fOffset;
    vShadowTexCoord.y = 1.0f - vShadowTexCoord.y;
        
	// Get the shadow occlusion factor and output it
	float fShadowTerm = 0;
	if (iFilterSize == 2)
		fShadowTerm = CalcShadowTermPCF(fLightDepth, vShadowTexCoord);
	else
		fShadowTerm = CalcShadowTermSoftPCF(fLightDepth, vShadowTexCoord, iFilterSize);

	return fShadowTerm;
}

float3 GetSplitColor( int iSplit )
{
	float3 vSplitColors [4];
	vSplitColors[0] = float3(1, 0, 0);
	vSplitColors[1] = float3(0, 1, 0);
	vSplitColors[2] = float3(0, 0, 1);
	vSplitColors[3] = float3(1, 1, 0);
	return vSplitColors[iSplit];
}
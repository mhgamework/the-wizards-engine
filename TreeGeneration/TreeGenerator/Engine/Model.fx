//=========================================================================
//
//	DeferredShadowMaps
//
//		by MJP  (mpettineo@gmail.com)
//		12/14/08      
//
//=========================================================================
//
//	File:		Model.fx
//
//	Desc:		Outputs the lighting contribution for a single directional
//				light source. Also samples shadow occlusion from a texture.
//
//=========================================================================

float4x4 g_matWorld;
float4x4 g_matViewProj;
float4x4 g_matWorldIT;

float2 g_vRTDimensions;

float3 g_vDiffuseAlbedo = {0.5f, 0.5f, 0.5f};
float3 g_vSpecularAlbedo = {1.0f, 1.0f, 1.0f};
float g_fSpecularPower = 32.0f;

float3 g_vLightDirectionWS;
float3 g_vLightColor;

float3 g_vAmbientColor;

float3 g_vCameraPositionWS;

texture ShadowOcclusion;
sampler2D ShadowOcclusionSampler = sampler_state
{
    Texture = <ShadowOcclusion>;
    MinFilter = Point; 
    MagFilter = Point; 
    MipFilter = Point;
};

void ModelVS(	in float4 in_vPositionOS	: POSITION,
				in float3 in_vNormalOS		: NORMAL,				
				out float4 out_vPositionCS	: POSITION,
				out float3 out_vNormalWS	: TEXCOORD0,
				out float3 out_vPositionWS	: TEXCOORD1,
				out float4 out_vPositionCS2	: TEXCOORD2	)										
{	
	// Figure out the position of the vertex in world space, and clip space
	out_vPositionWS = mul(in_vPositionOS, g_matWorld).xyz;	
    out_vPositionCS = mul(float4(out_vPositionWS, 1), g_matViewProj);    
	
	// Rotate the normal so it's in world space
	out_vNormalWS = mul(in_vNormalOS, g_matWorldIT);
	
	// Also store the clip-space position in a TEXCOORD, since we can't
	// read from the POSITION register in the pixel shader.
	out_vPositionCS2 = out_vPositionCS;
}

// Calculates the contribution for a single light source using phong lighting
float3 CalcLighting (	float3 vDiffuseAlbedo, 
						float3 vSpecularAlbedo, 
						float fSpecularPower, 
						float3 vLightColor, 
						float3 vNormal, 
						float3 vLightDir, 
						float3 vViewDir	)
{
//return float3(0.3,0.5,0);
	float3 R = normalize(reflect(-vLightDir, vNormal));
    
    // Calculate the raw lighting terms
    float fDiffuseReflectance = saturate(dot(vNormal, vLightDir));
    float fSpecularReflectance = saturate(dot(R, vViewDir));

	// Modulate the lighting terms based on the material colors, and the attenuation factor
    float3 vSpecular = vSpecularAlbedo * vLightColor * pow(fSpecularReflectance, fSpecularPower);
    float3 vDiffuse = vDiffuseAlbedo * vLightColor * fDiffuseReflectance;	

	// Lighting contribution is the sum of the diffuse and specular terms
	return vDiffuse + vSpecular;
}

// Gets the screen-space texel coord from clip-space position
float2 CalcSSTexCoord (float4 vPositionCS)
{
	float2 vSSTexCoord = vPositionCS.xy / vPositionCS.w;
	vSSTexCoord = vSSTexCoord * 0.5f + 0.5f;    
    vSSTexCoord.y = 1.0f - vSSTexCoord.y; 
    vSSTexCoord += 0.5f / g_vRTDimensions;    
    return vSSTexCoord;
}   

float4 ModelPS(	in float3 in_vNormalWS		: TEXCOORD0,
				in float3 in_vPositionWS	: TEXCOORD1,
				in float4 in_vPositionCS	: TEXCOORD2) : COLOR0
{    
    // Sample the shadow term based on screen position
    float2 vScreenCoord = CalcSSTexCoord(in_vPositionCS);    
    float3 vShadowTerm = tex2D(ShadowOcclusionSampler, vScreenCoord).rgb;  
    //return float4(vShadowTerm,1);

	// Normalize after interpolation
    float3 vNormalWS = normalize(in_vNormalWS);
    float3 vLightDirWS = normalize(-g_vLightDirectionWS);
    float3 vViewDirWS = normalize(g_vCameraPositionWS - in_vPositionWS);
    
    // Calculate the lighting term for the directional light
    float3 vLightContribition = 0;
    vLightContribition = vShadowTerm * CalcLighting(	g_vDiffuseAlbedo, 
														g_vSpecularAlbedo, 
														g_fSpecularPower,
														g_vLightColor, 
														vNormalWS, 
														vLightDirWS, 
														vViewDirWS);
					
	// Add in ambient term	
	vLightContribition.xyz += g_vDiffuseAlbedo * g_vAmbientColor;
													
	return float4(vLightContribition, 1.0f);
}

technique Model
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 ModelVS();
        PixelShader = compile ps_2_0 ModelPS();
        
        ZEnable = true;
        ZWriteEnable = true;
        //CullMode = CCW;
        FillMode = Solid;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
    }
}



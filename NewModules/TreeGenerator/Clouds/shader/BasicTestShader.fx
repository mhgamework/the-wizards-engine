/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;


shared float4x4 viewInverse : ViewInverse;



texture diffuseTexture : Diffuse
<
	string UIName = "Diffuse Texture";
	
>;
sampler DiffuseTextureSampler = sampler_state
{
	Texture = <diffuseTexture>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};
sampler PixelTexSampler = sampler_state
{
	Texture = <diffuseTexture>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};


//----------------------------------------------------




// testing of the point sprite shader file doesn't load strangely enough

//------- Technique: PointSprites --------
//by me
struct VertexInput
{
	float3 pos : POSITION;
	float Size:PSIZE;
};

struct SpritesVertexOut
{
    float4 Position: POSITION0;
    float Size : PSIZE;
	//float2 TexCoord:TEXCOORD0;
};
struct SpritesPixelIn
{
    
	float2 TexCoord : TEXCOORD0;
	
};
float fPartX,fPartY,
fVolBoundingMinY=-5,fVolBoundingMinX=-5,
fVolBoundingMaxX=5,fVolBoundingMaxY=5,
fLightOffsetY=1,fLightOffsetX=1,
fLightOffsetXShading=1,fLightOffsetYShading=1,
fPixelWidth,fPixelHeight,
fAttenuationRatio,fSmoothedVolumeValue
;

SpritesVertexOut PointSpritesVS (VertexInput In)//float4 Position : POSITION, float4 Color : COLOR0, float Size : PSIZE)
{
    SpritesVertexOut Output;
     fPartX=In.pos.x;
	 fPartY=In.pos.y;
    //float4x4 preViewProjection = mul (viewInverse, viewProjection);
	//float4x4 preWorldViewProjection = mul (world, preViewProjection); 
    //Output.Position = mul(Position, preWorldViewProjection);
	Output.Position = mul(float4(In.pos,1), viewProjection);
    Output.Size = 350/(Output.Position.z) * In.Size;  //(1/pow(Output.Position.z/Output.Position.w,2)+1) * 1000;
    //Output.Size=100;
    
    
    return Output; 
}

float4 PointSpritesPS(SpritesPixelIn PSIn) : Color0
{ 

	float2 texCoord = PSIn.TexCoord.xy;
	float4 diffuse = tex2D(DiffuseTextureSampler, texCoord);
	diffuse.rgb = diffuse.rgb * 0.9;
    //return float4(diffuse.a,0,0,1);
    return diffuse;
}


/*void ShadowTermVS (	in float3 in_vPositionOS				: POSITION,
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
}*/	




float4 VolumeShadingPS( SpritesPixelIn In) : COLOR0
{
float2 vVolumePos;
float2 vShadingPos;
vVolumePos.x = In.TexCoord.x * fPartX + fVolBoundingMinX + fLightOffsetX;
vVolumePos.y = In.TexCoord.y * fPartY + fVolBoundingMinY + fLightOffsetY;
vShadingPos.x = In.TexCoord.x * fPartX + fVolBoundingMinX+fLightOffsetXShading;
vShadingPos.y = In.TexCoord.y * fPartY + fVolBoundingMinY+fLightOffsetYShading;
float fVolumeValue;
float fVolumeNeighbour1;
float fVolumeNeighbour2;
float fVolumeNeighbour3;
float fVolumeNeighbour4;
if((vVolumePos.x >= fVolBoundingMinX) && (vVolumePos.x <= fVolBoundingMaxX) &&
(vVolumePos.y >= fVolBoundingMinY) && (vVolumePos.y <= fVolBoundingMaxY)) {
fVolumeValue = tex2D(PixelTexSampler, vVolumePos).g;
}
else {
//out of boudings - so there is no cloud particle on this position
fVolumeValue = 0.0;
}
//left neighbour
if(((vVolumePos.x - fPixelWidth) >= fVolBoundingMinX) && ((vVolumePos.x -
fPixelWidth) <= fVolBoundingMaxX) && (vVolumePos.y >= fVolBoundingMinY) &&
(vVolumePos.y <= fVolBoundingMaxY)) {
fVolumeNeighbour1 = tex2D(PixelTexSampler, float2(vVolumePos.x - fPixelWidth,
vVolumePos.y)).g;
}
else {
//out of boundings - so there is no cloud particle on this position
fVolumeNeighbour1 = 0.0;
}
//right neighbor
if(((vVolumePos.x + fPixelWidth) >= fVolBoundingMinX) && ((vVolumePos.x -
fPixelWidth)<= fVolBoundingMaxX) && (vVolumePos.y >= fVolBoundingMinY) &&
(vVolumePos.y <= fVolBoundingMaxY)) {
fVolumeNeighbour2 = tex2D(PixelTexSampler, float2(vVolumePos.x + fPixelWidth,
vVolumePos.y)).g;
}
else {
//out of boundings - so there is no cloud particle on this position
fVolumeNeighbour2 = 0.0;
}
//top neighbor
if((vVolumePos.x >= fVolBoundingMinX) && (vVolumePos.x <= fVolBoundingMaxX) &&
((vVolumePos.y - fPixelHeight) >= fVolBoundingMinY) && ((vVolumePos.y -
fPixelHeight) <= fVolBoundingMaxY)) {
fVolumeNeighbour3 = tex2D(PixelTexSampler, float2(vVolumePos.x, vVolumePos.y -
fPixelHeight)).g;
}
else {
//out of boundings - so there is no cloud particle on this position
fVolumeNeighbour3 = 0.0;
}
//bottom neighbor
if((vVolumePos.x >= fVolBoundingMinX) && (vVolumePos.x <= fVolBoundingMaxX) &&
((vVolumePos.y + fPixelHeight) >= fVolBoundingMinY) && ((vVolumePos.y +
fPixelHeight) <= fVolBoundingMaxY)) {
fVolumeNeighbour4 = tex2D(PixelTexSampler, float2(vVolumePos.x, vVolumePos.y +
fPixelHeight)).g;
}
else {
//out of boundings - so there is no cloud particle on this position
fVolumeNeighbour4 = 0.0;
}
float fSmoothedVolumeValue = (fVolumeValue + fVolumeNeighbour1 +
fVolumeNeighbour2 + fVolumeNeighbour3 + fVolumeNeighbour4) / 5.0;
//compute attenuation for the current position on the volume slice
float fAttenuation = 1.0 - (1.0 - fAttenuationRatio) * fSmoothedVolumeValue;
//get result of previous shading step
float fPreviousValue;
if((vShadingPos.x >= (fVolBoundingMinX + fPixelWidth)) && (vShadingPos.x <=
(fVolBoundingMaxX - fPixelWidth)) && (vShadingPos.y >= (fVolBoundingMinY +
fPixelHeight)) && (vShadingPos.y <= (fVolBoundingMaxY - fPixelHeight))) {
fPreviousValue = tex2D(PixelTexSampler, vShadingPos).r;
}
else {
fPreviousValue = 1.0;
}
//decrease brightness when a particle absorbed some amount of light
float fFinalValue = fPreviousValue * fAttenuation;
return float4(fFinalValue, fFinalValue, fFinalValue, 0.0);
}


technique PointSprites
{
	pass P0
	{   
		PointSpriteEnable = true;
		VertexShader = compile vs_2_0 PointSpritesVS();
		PixelShader  = compile ps_2_0 PointSpritesPS();
	}
}


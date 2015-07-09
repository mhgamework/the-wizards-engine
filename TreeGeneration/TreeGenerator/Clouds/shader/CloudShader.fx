

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;

/*shared float4x4 viewInverse : ViewInverse;*/



texture diffuseTexture : Diffuse
<
	string UIName = "Diffuse Texture";
	//string ResourceName = "marble.dds";
>;
sampler DiffuseTextureSampler = sampler_state
{
	Texture = <diffuseTexture>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};



//----------------------------------------------------



struct VertexInput
{
	float3 pos : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL;
};

struct VertexOutput
{
	float4 pos : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : TEXCOORD1;
};
struct VertexTemp
{
	float3 pos:POSITION;
	float3 normal:NORMAL;
};


struct VS_OUTPUT
	{
		float4 Position:POSITION;
		float2 TexCoord:TEXCOORD0;
		float Size;
		float4 Color:COLOR;
		float4 ParticleDensity;
	
	};

	
	float fVolumeOffsetX;
	float fVolumeOffsetZ;
	float fPixelHeight;
	float SamplerVolume;
	float fPixelWidth;
	
	VS_OUTPUT BillboardAreaVS(VS_OUTPUT IN)
{
VS_OUTPUT OUT;
float fParticle = tex2Dlod(SamplerVolume, float4(fVolumeOffsetX +
IN.Position.x * 0.25 * fPixelWidth, fVolumeOffsetZ +
IN.Position.z * 0.25 * fPixelHeight, 0.0, 1.0)).r;
if(fParticle > 0.3) {
float4 vCameraSpacePos = mul(float4(IN.Position.xyz + vPosition.xyz, 1.0),
mtView);
float fDistance = distance(float3(0.0, 0.0, 1.0), vCameraSpacePos.xyz);
OUT.Position = mul(float4(IN.Position.xyz + vPosition.xyz, 1.0),
mtWorldViewProj);
OUT.TexCoord = IN.TexCoord;
OUT.Size = ((9500 * fParticle) / fDistance) * fParticle;
OUT.Color = vSunCol * tex2Dlod(SamplerShading, float4(fVolumeOffsetX +
IN.Position.x * fDistanceScale * fPixelWidth,
fVolumeOffsetZ + IN.Position.z *
fDistanceScale * fPixelHeight, 0.0, 1.0)).r;
OUT.ParticleDensity = float4(fParticle, 0.0, 0.0, 0.0);
}
else {OUT.Position = float4(0.0, 0.0, -1.0, 0.0);
OUT.TexCoord = IN.TexCoord;
OUT.Size = 1.0;
OUT.Color = float4(0.0, 0.0, 0.0, 0.0);
OUT.ParticleDensity = float4(0.0, 0.0, 0.0, 0.0);
}
return OUT;
}


	

float4 SimulationStepPS( VS_OUTPUT In) : COLOR0
{
float4 vResult;
//get current states at actual map position
float4 vLastStep = tex2D(SamplerLastStep, In.TexCoord.xy);
//compute new humidity value with hum(t+1) := hum(t) and not act(t)
//note, that the humidity value is written to the r-component
if((vLastStep.r >= 0.5) && (vLastStep.b < 0.5)) {
vResult.r = 1.0;
}
else {
vResult.r = 0.0;
}
//compute new cloud value with cld(t+1) = cld(t) or act(t)
//note that the cld value is written to the g-component
if((vLastStep.g >= 0.5) || (vLastStep.b >= 0.5)) {
vResult.g = 1.0;
}
else {
vResult.g = 0.0;
}
//compute new activation value with act(t+1) = not act(t) and hum(t) and f_act
//where f_act is definied in the original publication by Dobashi et al (A
//Simple, Efficient Method for Realistic Animation of Clouds).
//first, compute the actual segment number
float2 vActSegment;
float fActSegmentIndex;
float fTempSegmentIndex;
float2 vTemp;
float2 vTemp2;
vTemp.x = In.TexCoord.x * 2047.0;
vTemp.y = In.TexCoord.y * 1023.0;
vTemp2.x = In.TexCoord.x / fSegmentWidth;
vTemp2.y = In.TexCoord.y / fSegmentHeight;
vActSegment.x = round(vTemp2.x);
vActSegment.y = round(vTemp2.y);
if((vActSegment.x - vTemp2.x) > 0.0) vActSegment.x = vActSegment.x - 1.0;
if((vActSegment.y - vTemp2.y) > 0.0) vActSegment.y = vActSegment.y - 1.0;
fActSegmentIndex = round(vActSegment.x + (vActSegment.y * 8.0));
//compute local uv-offset
float2 vLocalUV;
vLocalUV.x = In.TexCoord.x - (vActSegment.x / 2048.0 * 256.0);
vLocalUV.y = In.TexCoord.y - (vActSegment.y / 1024.0 * 256.0);
float fAct;
float fAct1 = tex2D(SamplerLastStep, float2(In.TexCoord.x +
fOffsetX1, In.TexCoord.y)).b;
float fAct2 = tex2D(SamplerLastStep, float2(In.TexCoord.x, In.TexCoord.y +
fOffsetZ1)).b;
float fAct3 = tex2D(SamplerLastStep, float2(In.TexCoord.x - fOffsetX1,
In.TexCoord.y)).b;
float fAct4 = tex2D(SamplerLastStep, float2(In.TexCoord.x, In.TexCoord.y -
fOffsetZ1)).b;
float fAct5 = tex2D(SamplerLastStep, float2(In.TexCoord.x - fOffsetX2,
In.TexCoord.y)).b;
float fAct6 = tex2D(SamplerLastStep, float2(In.TexCoord.x + fOffsetX2,
In.TexCoord.y)).b;
float fAct7 = tex2D(SamplerLastStep, float2(In.TexCoord.x, In.TexCoord.y -
fOffsetZ2)).b;
float fAct8 = tex2D(SamplerLastStep, float2(In.TexCoord.x, In.TexCoord.y +
fOffsetZ2)).b;
float fAct9 = CheckOtherSegment(fActSegmentIndex - 1.0, vLocalUV);
float fAct10 = CheckOtherSegment(fActSegmentIndex - 2.0, vLocalUV);
float fAct11 = CheckOtherSegment(fActSegmentIndex + 1.0, vLocalUV);
if((fAct1 >= 0.5) || (fAct2 >= 0.5) || (fAct3 >= 0.5) || (fAct4 >= 0.5) ||
(fAct5 >= 0.5) || (fAct6 >= 0.5) || (fAct7 >= 0.5) || (fAct8 >= 0.5) ||
(fAct9 >= 0.5) || (fAct10 >= 0.5) || (fAct11 >= 0.5)) {fAct = 1.0;
}
else {
fAct = 0.0;
}
if((vLastStep.b < 0.5) && (vLastStep.r >= 0.5) && (fAct >= 0.5)) {
vResult.b = 1.0;
}
else {
vResult.b = 0.0;
}
float2 vNoise;
float fNoiseValue;
//get density scale value for slice
float fSliceDensity = tex1D(SamplerSliceDensity, fActSegmentIndex / 31.0).r;
//fetch values from regionmask textures
float fRegionMaskHumAct = tex2D(SamplerRegionVolume,
float2(In.TexCoord.x, In.TexCoord.y)).r;
float fRegionMaskExt = 1.0 - fRegionMaskHumAct;
//compute cloud extinction
vNoise = float2(In.TexCoord.x / 2.0 + fNoiseOffsetExtX, In.TexCoord.y / 2.0 +
fNoiseOffsetExtY);
fNoiseValue = tex2D(SamplerNoise, vNoise).r;
if(fNoiseValue <= (fRegionMaskExt * 0.1)) {
vResult.g = 0.0;
}
//compute humidity regeneration
vNoise = float2(In.TexCoord.x / 2.0 + fNoiseOffsetHumX, In.TexCoord.y / 2.0 +
fNoiseOffsetHumY);
fNoiseValue = tex2D(SamplerNoise, vNoise).r;
if(fNoiseValue < (fRegionMaskHumAct * 0.1)) {
vResult.r = 1.0;
}
//compute activation values
vNoise = float2(In.TexCoord.x / 2.0 + fNoiseOffsetActX, In.TexCoord.y / 2.0 +
fNoiseOffsetActY);
fNoiseValue = tex2D(SamplerNoise, vNoise).r;
if(fNoiseValue < (fRegionMaskHumAct * 0.001)) {
vResult.b = 1.0;
}
//remove cloud particles outside of the defined ellipsoids
if(fRegionMaskHumAct <= 0.001) {
vResult.g = 0.0;
}
//alpha channel is not used, so set it simply to zero. maybe it could be used
//to store cloud regions in future versions.
vResult.a = 0.0;
return vResult;
}


float4 VolumeShadingPS( VS_OUTPUT In) : COLOR0
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
fVolumeValue = tex2D(SamplerVolume, vVolumePos).g;
}
else {
//out of boudings - so there is no cloud particle on this position
fVolumeValue = 0.0;
}
//left neighbour
if(((vVolumePos.x - fPixelWidth) >= fVolBoundingMinX) && ((vVolumePos.x -
fPixelWidth) <= fVolBoundingMaxX) && (vVolumePos.y >= fVolBoundingMinY) &&
(vVolumePos.y <= fVolBoundingMaxY)) {
fVolumeNeighbour1 = tex2D(SamplerVolume, float2(vVolumePos.x - fPixelWidth,
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
fVolumeNeighbour2 = tex2D(SamplerVolume, float2(vVolumePos.x + fPixelWidth,
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
fVolumeNeighbour3 = tex2D(SamplerVolume, float2(vVolumePos.x, vVolumePos.y -
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
fVolumeNeighbour4 = tex2D(SamplerVolume, float2(vVolumePos.x, vVolumePos.y +
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
fPreviousValue = tex2D(SamplerShading, vShadingPos).r;
}
else {
fPreviousValue = 1.0;
}
//decrease brightness when a particle absorbed some amount of light
float fFinalValue = fPreviousValue * fAttenuation;
return float4(fFinalValue, fFinalValue, fFinalValue, 0.0);
}


technique AnimatedCloud
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_AnimatedCloud();
		PixelShader = compile ps_3_0 PS_AnimatedCloud();
	} 
} 






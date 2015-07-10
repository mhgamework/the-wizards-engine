
#include <TestHelper.fx>

Texture2D finalMap;
Texture2D lumAvgMap;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerState samPoint
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    output.TexCoord = input.TexCoord;
    return output;
}
//float g_fMiddleGrey = 0.6f; Original
float g_fMiddleGrey = 0.6f; 

float g_fMaxLuminance = 16.0f; 

static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f); 
float3 ToneMap(float3 vColor) 
{

	// Get the calculated average luminance 
	//NOTE: between -infinity and 16 ?
	//TODO: float fLumAvg = 300;//tex2D(PointSampler1, float2(0.5f, 0.5f)).r;    
	
	float fLumAvg = lumAvgMap.Sample(samPoint,float2(0.5f,0.5f)).r;
	// Calculate the luminance of the current pixel 
	float fLumPixel = dot(vColor, LUM_CONVERT);     
	// Apply the modified operator (Eq. 4) 
	float fLumScaled = (fLumPixel * g_fMiddleGrey) / fLumAvg;
	float fLumCompressed = (fLumScaled * (1 + (fLumScaled / (g_fMaxLuminance * g_fMaxLuminance)))) / (1 + fLumScaled); 
	//return t(fLumCompressed);
	return fLumCompressed * vColor; 
}
float4 ToneMapPS ( VertexShaderOutput input ) : SV_TARGET0
	//uniform bool bEncodeLogLuv )    : COLOR0 
{
	// Sample the original HDR image 
	float4 vSample = finalMap.Sample(samLinear, input.TexCoord); 
	float3 vHDRColor; 
	/*if (bEncodeLogLuv) 
	vHDRColor = LogLuvDecode(vSample); 
	else */
	vHDRColor = vSample.rgb; 

	// Do the tone-mapping 
	float3 vToneMapped = ToneMap(vHDRColor); 
	return t(vToneMapped);

	// Add in the bloom component 
	float3 vColor = vToneMapped; //+ tex2D(LinearSampler2, in_vTexCoord).rgb * g_fBloomMultiplier; 

	return float4(vColor, 1.0f); 
} 


technique10 Technique0
{
    pass Pass0
    {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, ToneMapPS() ) );
    }
}
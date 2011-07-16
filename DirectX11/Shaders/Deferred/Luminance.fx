
#include <TestHelper.fx>

Texture2D hdrImage;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
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

static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f); 

float4 LuminancePS (	VertexShaderOutput input)  : SV_TARGET0
						//uniform bool bEncodeLogLuv )	: COLOR0
{						
    float4 vSample = hdrImage.Sample(samLinear, input.TexCoord);
    float3 vColor;
    /*if (bEncodeLogLuv)
		vColor = LogLuvDecode(vSample);
	else*/
		vColor = vSample.rgb ;
   
    // calculate the luminance using a weighted average
    float fLuminance = dot(vColor, LUM_CONVERT) ;
                
    float fLogLuminace = log(1e-5 + fLuminance); 
        return t(fLogLuminace); //TODO:remove this
    // Output the luminance to the render target
    return float4(fLogLuminace, 1.0f, 0.0f, 0.0f);
}

Texture2D lastLum;
Texture2D currentLum;
float g_fDt;

float4 CalcAdaptedLumPS (VertexShaderOutput input)  : SV_TARGET0
{
	
	// CurrentLum is logarithmic scale
	// LastLum is linear scale
	// Output (the next lastlum) is thus linear scale
	// NOTE: these scales i divised from the original code for this algorithm
	// However, logically i dont see why this should be like this
	// Both give nonlinear fading between adapted luminance levels

	float fLastLum = lastLum.Sample(samLinear,0).r;
    float fCurrentLogLum = currentLum.Sample(samLinear,0).r;
    
	float fCurrentLum = 1+ 1e-5+ exp(fCurrentLogLum); // I have no clue why there is +1 here, but it was in the original code
	//float fCurrentLum = fCurrentLogLum; 

    // Adapt the luminance using Pattanaik's technique
    const float fTau = 0.5f;
    float fAdaptedLum = fLastLum + (fCurrentLum - fLastLum) * (1 - exp(-g_fDt * fTau));
//	return t(0.1f);
	//return t(fCurrentLum);
    //return t(fLastLum+ (fCurrentLum - fLastLum) );
	//if (fAdaptedLum < 0) fAdaptedLum = 0;
    return float4(fAdaptedLum, 1.0f, 1.0f, 1.0f);
}


technique10 CalculateLuminance
{
    pass Pass0
    {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, LuminancePS() ) );
    }
}
technique10 CalculateAverage
{
    pass Pass0
    {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, CalcAdaptedLumPS() ) );
    }
}
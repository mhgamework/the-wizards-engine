texture colorMap;
texture lightMap;
texture ambientOcclusionMap;

sampler colorSampler = sampler_state
{
    Texture = (colorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler lightSampler = sampler_state
{
    Texture = (lightMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler ambientOcclusionSampler = sampler_state
{
    Texture = (ambientOcclusionMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
texture hdrMap;
sampler hdrSampler = sampler_state
{
    Texture = (hdrMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
texture finalMap;
sampler finalSampler = sampler_state
{
    Texture = (finalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
float2 halfPixel;
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    output.TexCoord = input.TexCoord - halfPixel;
    return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{


    float3 diffuseColor = tex2D(colorSampler,input.TexCoord).rgb;
	
    float4 light = tex2D(lightSampler,input.TexCoord);
	float4 ambientOcclusion = tex2D(ambientOcclusionSampler,input.TexCoord).r;
    float3 diffuseLight = light.rgb;
    float specularLight = light.a;
	float3 ambient=float3(1,1,1)*0.3f;


    return float4((diffuseColor * (diffuseLight+ambient*(1-ambientOcclusion)) + specularLight),1);
}


technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}



float g_fMiddleGrey = 0.6f; 
float g_fMaxLuminance = 16.0f; 

static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f); 

float3 ToneMap(float3 vColor) 
{

	// Get the calculated average luminance 
	float fLumAvg = 300;//tex2D(PointSampler1, float2(0.5f, 0.5f)).r;     

	// Calculate the luminance of the current pixel 
	float fLumPixel = dot(vColor, LUM_CONVERT);     

	// Apply the modified operator (Eq. 4) 
	float fLumScaled = (fLumPixel * g_fMiddleGrey) / fLumAvg;     
	float fLumCompressed = (fLumScaled * (1 + (fLumScaled / (g_fMaxLuminance * g_fMaxLuminance)))) / (1 + fLumScaled); 
	return fLumCompressed * vColor; 
} 


float4 ToneMapPS (    in float2 in_vTexCoord        : TEXCOORD0, 
	uniform bool bEncodeLogLuv )    : COLOR0 
{
	// Sample the original HDR image 
	float4 vSample = tex2D(finalSampler, in_vTexCoord); 
	float3 vHDRColor; 
	/*if (bEncodeLogLuv) 
	vHDRColor = LogLuvDecode(vSample); 
	else */
	vHDRColor = vSample.rgb; 

	// Do the tone-mapping 
	float3 vToneMapped = ToneMap(vHDRColor); 

	// Add in the bloom component 
	float3 vColor = vToneMapped; //+ tex2D(LinearSampler2, in_vTexCoord).rgb * g_fBloomMultiplier; 

	return float4(vColor, 1.0f); 
} 



technique TechniqueToneMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 ToneMapPS(false);
    }
}




texture pointTex0;
sampler PointSampler0 = sampler_state
{
    Texture = (pointTex0);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = LINEAR;
};

texture linearTex0;
sampler LinearSampler0 = sampler_state
{
    Texture = (linearTex0);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};



static const float g_vOffsets[4] = {-1.5f, -0.5f, 0.5f, 1.5f};


// Downscales to 1/16 size, using 16 samples
float4 DownscalePS (	in float2 in_vTexCoord			: TEXCOORD0,
						uniform bool bEncodeLogLuv,
						uniform bool bDecodeLuminance	)	: COLOR0
{
	float4 vColor = 0;
	for (int x = 0; x < 4; x++)
	{
		for (int y = 0; y < 4; y++)
		{
			float2 vOffset;
			vOffset = float2(g_vOffsets[x], g_vOffsets[y]) *halfPixel*2;/// g_vSourceDimensions;
			float4 vSample = tex2D(PointSampler0, in_vTexCoord + vOffset);
			/*if (bEncodeLogLuv)
				vSample = float4(LogLuvDecode(vSample), 1.0f);*/
			vColor += vSample;
		}
	}

	vColor /= 16.0f;
	
	/*if (bEncodeLogLuv)
		vColor = LogLuvEncode(vColor.rgb);*/
		
	/*if (bDecodeLuminance)
		vColor = float4(1-exp(vColor.r), 1.0f, 1.0f, 1.0f);*/
	
	return vColor;
}

// Upscales or downscales using hardware bilinear filtering
float4 HWScalePS (	in float2 in_vTexCoord			: TEXCOORD0	)	: COLOR0
{

	return tex2D(LinearSampler0, in_vTexCoord);
}


technique Downscale4
{
    pass p0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 DownscalePS(false, false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique Downscale4Luminance
{
    pass p0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 DownscalePS(false, true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique Downscale4Encode
{
    pass p0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 DownscalePS(true, false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique ScaleHW
{
    pass p0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 HWScalePS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}

float g_fDT;
float g_fBloomMultiplier;

float4 LuminancePS (	in float2 in_vTexCoord			: TEXCOORD0,
						uniform bool bEncodeLogLuv )	: COLOR0
{						
    float4 vSample = tex2D(LinearSampler0, in_vTexCoord);
    float3 vColor;
    /*if (bEncodeLogLuv)
		vColor = LogLuvDecode(vSample);
	else*/
		vColor = vSample.rgb;
   
    // calculate the luminance using a weighted average
    float fLuminance = dot(vColor, LUM_CONVERT);
                
    float fLogLuminace = log(1e-5 + fLuminance); 
        
    // Output the luminance to the render target
    return float4(fLogLuminace, 1.0f, 0.0f, 0.0f);
}

float4 CalcAdaptedLumPS (in float2 in_vTexCoord		: TEXCOORD0)	: COLOR0
{
	/*float fLastLum = tex2D(PointSampler1, float2(0.5f, 0.5f)).r;
    float fCurrentLum = tex2D(PointSampler0, float2(0.5f, 0.5f)).r;
    
    // Adapt the luminance using Pattanaik's technique
    const float fTau = 0.5f;
    float fAdaptedLum = fLastLum + (fCurrentLum - fLastLum) * (1 - exp(-g_fDT * fTau));
    
    return float4(fAdaptedLum, 1.0f, 1.0f, 1.0f);*/
	return float4(0,0,0,0);
}


technique Luminance
{
    pass p0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 LuminancePS(false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique LuminanceEncode
{
    pass p0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 LuminancePS(true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique CalcAdaptedLuminance
{
    pass p0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 CalcAdaptedLumPS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        AlphaTestEnable = false;
        StencilEnable = false;
    }
}


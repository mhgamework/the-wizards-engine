
#include <GBuffer.fx>
#include <TestHelper.fx>

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 LightViewProjection;
//color of the light 
float3 Color; 
//position of the camera, for specular light
float3 cameraPosition; 
//this is used to compute the world-position
float4x4 InvertViewProjection; 
//this is the position of the light
float3 lightPosition;
//how far does this light reach
float lightRadius;
//control the brightness of the light
float lightIntensity = 1.0f;
// diffuse color, and specularIntensity in the alpha channel
float3 spotDirection;
float spotLightAngleCosine;
float spotDecayExponent;
float2 g_vShadowMapSize;
static const float BIAS = 0.0005f; //0.006f
Texture2D shadowMap;
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
    float4 ScreenPosition : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	output.ScreenPosition = float4(input.Position,1);
    output.TexCoord = input.TexCoord;

	return output;
    //TODO: preshader worldviewproj
    //float4 worldPosition = mul(input.Position, World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);
	//output.Position = float4(input.Position,1); // For quad rendering
    //output.ScreenPosition = output.Position;
    //return output;
}

float CalcShadowTermPCF(float fLightDepth, float2 vShadowTexCoord,float scaledBias)
{
	float fShadowTerm = 0.0f;

	// transform to texel space
	float2 vShadowMapCoord = g_vShadowMapSize * vShadowTexCoord;
    
	// Determine the lerp amounts           
	float2 vLerps = frac(vShadowMapCoord);
	
	//float scaledBias = BIAS*(1-fLightDepth);
	//float scaledBias = BIAS;

	// Read in the 4 samples, doing a depth check for each
	float fSamples[4];	
	fSamples[0] =  (shadowMap.Sample(samPoint, vShadowTexCoord).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
	
	fSamples[1] = (shadowMap.Sample(samPoint, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 0)).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[2] = (shadowMap.Sample(samPoint, vShadowTexCoord + float2(0, 1.0/g_vShadowMapSize.y)).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[3] = (shadowMap.Sample(samPoint, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 1.0/g_vShadowMapSize.y)).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
    
	//return fSamples[0];

	// lerp between the shadow values to calculate our light amount
	fShadowTerm = lerp(lerp(fSamples[0], fSamples[1], vLerps.x), lerp( fSamples[2], fSamples[3], vLerps.x), vLerps.y);							  
								
	return fShadowTerm;								 
}

float2 halfPixel;
float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
	GBuffer_Raw g = SampleGBuffer(samLinear, input.TexCoord);


    float2 texCoord =input.TexCoord;
	
    //allign texels to pixels
    texCoord -=halfPixel;
	
    //tranform normal back into [-1,1] range
    float3 normal = g.Normal;
    //get specular power
    float specularPower = g.SpecularPower * 255;
    //get specular intensity from the colorMap
    float specularIntensity = g.SpecularIntensity;
    //read depth
    float depthVal = g.Depth;
    
		//compute screen-space position
	float4 position;
	position.x = input.TexCoord.x * 2.0f - 1.0f;
	position.y = -(input.TexCoord.y * 2.0f - 1.0f);
	position.z = g.Depth;
	position.w = 1.0f;
	//transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;
    //surface-to-light vector
	float ret=0;
	if (length(position)<2.5f)
		ret = 1;
    float3 lightVector = lightPosition - position;




	



	float3 diffuseLight = float3(0,0,0);
	float specularLight = 0;
	 

	float attenuation = saturate(1.0f - length(lightVector)/lightRadius); 
	//normalize light vector
	lightVector = normalize(lightVector);
	//SpotDotLight = cosine of the angle between spotdirection and lightvector
	float SdL = dot(spotDirection,-lightVector);

	if (SdL > spotLightAngleCosine) 
	{
		float spotIntensity = pow(SdL,spotDecayExponent);
		//rest of computations from the point light go here

	    //compute diffuse light
		float NdL = max(0,dot(normal,lightVector));
		diffuseLight = NdL * Color.rgb;
		//reflection vector
		float3 reflectionVector = normalize(reflect(-lightVector, normal));
		//camera-to-surface vector
		float3 directionToCamera = normalize(cameraPosition - position);
		//compute specular light
		specularLight = specularIntensity 
			* pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);
		//take into account attenuation and lightIntensity.

		//multiply the attenuation by spotIntensity before applying it to the light
		attenuation *= spotIntensity;

	}
	
	//TODO: WARNING: shadow disabled

	float shadowTerm = 1;

	//Shadow Map
#ifndef DISABLE_SHADOWS
	float4 shadowMapPosition = mul(position,LightViewProjection);
	float fLightDepth = shadowMapPosition.z / shadowMapPosition.w;
	float2 vShadowTexCoord = 0.5 * shadowMapPosition.xy / shadowMapPosition.w + float2(0.5f, 0.5f);
	//return t(vShadowTexCoord);
    //vShadowTexCoord.x = vShadowTexCoord.x / NUM_SPLITS + fOffset;
    vShadowTexCoord.y = 1.0f - vShadowTexCoord.y;
        
    // Offset the coordinate by half a texel so we sample it correctly
    //vShadowTexCoord += (0.5f / g_vShadowMapSize);
	//float2 shadowTexCoord= float2(shadowMapPosition.x*0.5f+0.5f,shadowMapPosition.y*0.5f+0.5f);
	
	

	float shadowMapDepth = shadowMap.Sample(samLinear, vShadowTexCoord).r;


	shadowTerm = (shadowMapDepth< fLightDepth-0.001f) ? 0.0f: 1.0f;

	float bias = depthVal*0.001;

	shadowTerm = CalcShadowTermPCF(fLightDepth,vShadowTexCoord,bias);
	//shadowTerm = CalcShadowTermPCF(fLightDepth,vShadowTexCoord,bias+BIAS/(1-depthVal));
#endif

		
	// Big booboo: when dot(reflectionVector, directionToCamera) <0, specularLight seems to become negative infinity?
	if (specularLight< 0) specularLight = 0; 
	return attenuation * lightIntensity * float4(diffuseLight.rgb,specularLight)* shadowTerm;

    
}


technique10 Technique0
{
    pass Pass0
    {
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunction() ) );
    }
}
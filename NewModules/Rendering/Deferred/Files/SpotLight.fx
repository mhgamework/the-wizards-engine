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
texture colorMap; 
// normals, and specularPower in the alpha channel
texture normalMap;
//depth
texture depthMap;
texture shadowMap;
sampler colorSampler = sampler_state
{
    Texture = (colorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler depthSampler = sampler_state
{
    Texture = (depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
sampler normalSampler = sampler_state
{
    Texture = (normalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
sampler shadowMapSampler = sampler_state
{
    Texture = (shadowMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
struct VertexShaderInput
{
    float3 Position : POSITION0;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 ScreenPosition : TEXCOORD0;
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    //processing geometry coordinates
    float4 worldPosition = mul(float4(input.Position,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.ScreenPosition = output.Position;
    return output;
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
	fSamples[0] = (tex2D(shadowMapSampler, vShadowTexCoord).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[1] = (tex2D(shadowMapSampler, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 0)).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[2] = (tex2D(shadowMapSampler, vShadowTexCoord + float2(0, 1.0/g_vShadowMapSize.y)).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
	fSamples[3] = (tex2D(shadowMapSampler, vShadowTexCoord + float2(1.0/g_vShadowMapSize.x, 1.0/g_vShadowMapSize.y)).x + scaledBias < fLightDepth) ? 0.0f: 1.0f;  
    
	// lerp between the shadow values to calculate our light amount
	fShadowTerm = lerp(lerp(fSamples[0], fSamples[1], vLerps.x), lerp( fSamples[2], fSamples[3], vLerps.x), vLerps.y);							  
								
	return fShadowTerm;								 
}

float2 halfPixel;
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

    //obtain screen position
    input.ScreenPosition.xy /= input.ScreenPosition.w;
    //obtain textureCoordinates corresponding to the current pixel
    //the screen coordinates are in [-1,1]*[1,-1]
    //the texture coordinates need to be in [0,1]*[0,1]
    float2 texCoord = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1);
	
    //allign texels to pixels
    texCoord -=halfPixel;
    //get normal data from the normalMap
    float4 normalData = tex2D(normalSampler,texCoord);
	
    //tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    //get specular power
    float specularPower = normalData.a * 255;
    //get specular intensity from the colorMap
    float specularIntensity = tex2D(colorSampler, texCoord).a;
    //read depth
    float depthVal = tex2D(depthSampler,texCoord).r;
    //compute screen-space position
    float4 position;
    position.xy = input.ScreenPosition.xy;
    position.z = depthVal;
    position.w = 1.0f;
    //transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;
    //surface-to-light vector
	float ret=0;
	if (length(position)<2.5f)
		ret = 1;
    float3 lightVector = lightPosition - position;




	//Shadow Map

	float4 shadowMapPosition = mul(position,LightViewProjection);
	float fLightDepth = shadowMapPosition.z / shadowMapPosition.w;
	float2 vShadowTexCoord = 0.5 * shadowMapPosition.xy / shadowMapPosition.w + float2(0.5f, 0.5f);
    //vShadowTexCoord.x = vShadowTexCoord.x / NUM_SPLITS + fOffset;
    vShadowTexCoord.y = 1.0f - vShadowTexCoord.y;
        
    // Offset the coordinate by half a texel so we sample it correctly
    vShadowTexCoord += (0.5f / g_vShadowMapSize);
	//float2 shadowTexCoord= float2(shadowMapPosition.x*0.5f+0.5f,shadowMapPosition.y*0.5f+0.5f);
	
	//float4 ret2 = float4(vShadowTexCoord,0,0);
	
	//return ret2;

	/*if (abs(shadowMapPosition.x)>1 || abs(shadowMapPosition.y)>1) ret2 = float4(0,0,0,0);

	return ret2;*/
	

	float shadowMapDepth = tex2D(shadowMapSampler, vShadowTexCoord).r;

	//return float4(0,0,(shadowMapDepth< fLightDepth-0.001f) ? 0.0f: 1.0f,0);
	//return float4(fLightDepth,shadowMapDepth,(shadowMapDepth< fLightDepth) ? 1.0f: 0.0f,0);

	//return float4(shadowMapDepth,0,0,0);
	float shadowTerm = (shadowMapDepth< fLightDepth-0.001f) ? 0.0f: 1.0f;
	//return float4(shadowTerm,0,0,0);
	shadowTerm = CalcShadowTermPCF(fLightDepth,vShadowTexCoord,BIAS);
	//shadowTerm = CalcShadowTermPCF(fLightDepth,vShadowTexCoord,bias+BIAS/(1-depthVal));



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
	
	return attenuation * lightIntensity * float4(diffuseLight.rgb,specularLight)* shadowTerm;

    
}


technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
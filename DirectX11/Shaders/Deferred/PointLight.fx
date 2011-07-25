

#include <GBuffer.fx>
#include <Common.fx>

float4x4 World;
float4x4 View;
float4x4 Projection;
//color of the light 
float3 Color; 
//position of the camera, for specular light
float3 cameraPosition; 
//this is used to compute the world-position
float4x4 InvertViewProjection; 
float4x4 ShadowMapProjection;
//this is the position of the light
float3 lightPosition;
//how far does this light reach
float lightRadius;
//control the brightness of the light
float lightIntensity = 1.0f;
// diffuse color, and specularIntensity in the alpha channel
TextureCube shadowMap;



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

    ////processing geometry coordinates
    //float4 worldPosition = mul(float4(input.Position,1), World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);
    //output.ScreenPosition = output.Position;
    //return output;
}




float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
	GBuffer_Raw g = SampleGBuffer(samLinear, input.TexCoord);

    float2 texCoord = input.TexCoord;
	
    //tranform normal back into [-1,1] range
    float3 normal = g.Normal;
    //get specular power
    float specularPower = g.SpecularPower * 255;
    //get specular intensity from the colorMap
    float specularIntensity =g.SpecularIntensity;
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
    float3 lightVector = lightPosition - position;
    //compute attenuation based on distance - linear attenuation
    float attenuation = saturate(1.0f - length(lightVector)/lightRadius); 
	
	//normalize light vector
    lightVector = normalize(lightVector); 
    //compute diffuse light
    float NdL = max(0,dot(normal,lightVector));
    

    float3 diffuseLight = NdL * Color.rgb;
    //reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(cameraPosition - position.xyz);
    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);
	//return t(saturate(dot(reflectionVector, directionToCamera)));
	
	float shadowTerm = 1;

#ifndef DISABLE_SHADOWS
	float3 toLight = position.xyz - lightPosition;
	float fLightDepth = length(toLight);

	toLight = normalize(toLight);

	// This my friend, is a MHGW trick.
	// The largest component of the toLight vector determines which face of the cube is used.
	//   This means that this component is the projection of the toLight vector onto the axis that points to the correct
	//   cube face. By multiplying fLightDepth * longest we project onto this axis, and get the view space depth.
	float longest = max(max(abs(toLight.x),abs(toLight.y)),abs(toLight.z)); 
	fLightDepth = fLightDepth * longest;


	float shadowMapDepth = shadowMap.Sample(samPoint,toLight.xyz).x;
	//shadowMapDepth = ConvertToLinearDepth(shadowMapDepth, zNear,zFar);
	shadowMapDepth = ConvertToLinearDepth(shadowMapDepth, ShadowMapProjection);

	shadowTerm = (shadowMapDepth< fLightDepth*0.99f) ? 0.0f: 1.0f;

#endif

	// Big booboo: when dot(reflectionVector, directionToCamera) <0, specularLight seems to become negative infinity?
	if (specularLight< 0) specularLight = 0; 

    //take into account attenuation and lightIntensity.
    return attenuation * lightIntensity * float4(diffuseLight.rgb,specularLight) * shadowTerm;
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
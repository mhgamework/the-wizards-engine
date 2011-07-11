
#include <GBuffer.fx>

float4x4 World;
float4x4 View;
float4x4 Projection;
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
float2 halfPixel;
float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
	GBuffer_Raw g = SampleGBuffer(samLinear, input.TexCoord);

    float2 texCoord = input.TexCoord;
	
    //allign texels to pixels
    texCoord -=halfPixel;
    //get normal data from the normalMap
	
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
    float3 directionToCamera = normalize(cameraPosition - position);
    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);

	

    //take into account attenuation and lightIntensity.
    return attenuation * lightIntensity * float4(diffuseLight.rgb,specularLight);
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
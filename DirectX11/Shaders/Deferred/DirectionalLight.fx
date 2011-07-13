
#include <GBuffer.fx>

//direction of the light
float3 lightDirection;
//color of the light 
float3 Color; 
//position of the camera, for specular light
float3 cameraPosition; 
//this is used to compute the world-position
matrix InvertViewProjection; 

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
float2 halfPixel;
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    //align texture coordinates
	//DX11 CHANGE: aligning not necessary anymore??
    //output.TexCoord = input.TexCoord - halfPixel;
    output.TexCoord = input.TexCoord;
	return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
	GBuffer_Raw g = SampleGBuffer(samLinear, input.TexCoord);
	
	//compute screen-space position
	float4 position;
	position.x = input.TexCoord.x * 2.0f - 1.0f;
	position.y = -(input.TexCoord.y * 2.0f - 1.0f);
	position.z = g.Depth;
	position.w = 1.0f;
	
	//transform to world space
	position = mul(position, InvertViewProjection);
	position /= position.w;
	
	//surface-to-light vector: TODO: normalize shouldn't be necessary, or preshadererd?
    float3 lightVector = -normalize(lightDirection);
	
    //compute diffuse light
    float NdL = max(0,dot(g.Normal,lightVector));
	
    float3 diffuseLight = NdL * Color.rgb;
    //reflexion vector
    float3 reflectionVector = normalize(reflect(lightVector, g.Normal));
    //camera-to-surface vector
    float3 directionToCamera = -normalize(cameraPosition - position.xyz);

	//get specular power, and get it into [0,255] range]
	float power = g.SpecularPower * 255;
    
	//return float4(dot(reflectionVector, directionToCamera),0,0,1);
	//compute specular light
    float specularLight = g.SpecularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), power);

		
	// Big booboo: when dot(reflectionVector, directionToCamera) <0, specularLight seems to become negative infinity?
	if (specularLight< 0) specularLight = 0; 
    //output the two lights
    return float4(diffuseLight.rgb, specularLight) ;
	//return float4(specularLight,0,0,0);
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
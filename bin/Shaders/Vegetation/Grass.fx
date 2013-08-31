
#include <GBuffer.fx>

float frequency=1.5f;
float amplitude=0.5f;
float Time;
float waveSpeed=5.0f;
float3 startPointWave=float3(2,2,2);
float3 WindDirection=float3(1,0,0);


matrix World;
matrix View;
matrix Projection;
float specularIntensity = 0.2f;
float specularPower = 0.2f;
Texture2D txDiffuse;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
}
;
struct VertexShaderInput
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD;
}
;
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
}
;

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float3 pos=float3(input.Position.x,input.Position.y,input.Position.z);
		
	//TODO: check if removing branching is faster
	if(input.TexCoord.y==0)
	{
		float3 translation=input.Position;
		float3 translation2=float3(0,0,0);//In.pos;
	
	
		float distX=startPointWave.x-input.Position.x;
		float distZ=startPointWave.z-input.Position.z;
		
		// this makes the grass move according to the winds direction
		float3 movement=amplitude*WindDirection*(sin(frequency*(Time-(1/waveSpeed)))*0.1);


		pos+=movement;
		
        
		//this makes the grass move like there is a point from were all the wind commes from
		float dist=sqrt(distX*distX+distZ*distZ);
	   translation2.x=amplitude*sin(frequency*(Time-(dist/waveSpeed)));
	   translation2.z=amplitude*sin(frequency*(Time-(dist/waveSpeed)));
	   
	   pos += translation2*0.1;
	

    
	}
	float4 position=float4(pos,1);
    matrix worldViewProj = mul(World,mul(View,Projection));
    //float4 worldPosition = mul(input.Position, World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);
	output.Position = mul(position, worldViewProj);
    output.TexCoord = input.TexCoord;
    //pass the texture coordinates further
    output.Normal = mul(input.Normal,(float3x3)World); //Note: w might be lost, but normal has no length anyway
    //get normal into world space
    return output;
} 

GBuffer PixelShaderFunction(VertexShaderOutput input)
{
	float4 diffuse = txDiffuse.Sample(samLinear, input.TexCoord);
	clip (diffuse.a - 0.1f);

    return CreateGBuffer(
		diffuse,
		normalize(input.Normal),
		specularIntensity,
		specularPower);
}

 
technique10 AnimatedGrass
{
	pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunction() ) );
    }
} 











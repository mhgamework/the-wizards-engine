float4 calculateNone(float3 oldVelocity, float3 oldPosition)
{
	return float4(0,0,0,0);
}
float4 calculateBall(float3 oldVelocity, float3 oldPosition)
{
	return float4(0,0,0,1);
}
float4 calculateFlame(float3 oldVelocity, float3 oldPosition)
{
	return float4(0,-5,0,1);
}
float4 CalculateSpark(float3 oldVelocity, float3 oldPosition)
{
	return float4(0,0,0,1);
}



Texture2D txDiffuse;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
Texture2D oldPositionTex;
Texture2D oldVelocityTex ;

// Vertex Output Declaration
struct VSOut
{
    float4 Position	: SV_POSITION;
    float2 TexCoord	: TEXCOORD0;
}
;
float width;
float height;
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
}
;
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
}
;

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    output.TexCoord = input.TexCoord.xy;
    //- halfPixel;
    return output;
}

float elapsed;
struct PixelShaderOutput
{
    half4 NewPosition : SV_TARGET0;
    half4 NewVelocity : SV_TARGET1;
}
;
float size;
float3 center;
// Pixel Shader
PixelShaderOutput ps_main(VSOut In)
{
    float4 oldPosition =oldPositionTex.Sample(samLinear, In.TexCoord.xy);
    float4 oldVelocity =oldVelocityTex.Sample(samLinear, In.TexCoord.xy);
    float3 velocity;
    velocity=oldVelocity.xyz+CALCULATE_ACCELERATION(oldVelocity,oldPosition)*elapsed;//float4(normalize((center.xyz-oldPosition.xyz))*dot(oldVelocity,oldVelocity)*elapsed,1);
	 
	 

	 float3 r = center.xyz-oldPosition.xyz;

	 //velocity=oldVelocity.xyz+float4(1/length(r)*elapsed*(normalize(r))*1000,1);
	 float clampVar = 40;
    //velocity=clamp(velocity,float3(-clampVar,-clampVar,-clampVar),float3(clampVar,clampVar,clampVar));
    //velocity = float3(0,100,0);
	//velocity=oldVelocity.xyz+float4(normalize((center.xyz-oldPosition.xyz))*dot(oldVelocity,oldVelocity)*elapsed/6,1);
	PixelShaderOutput output;
    output.NewPosition = float4(oldPosition.xyz + velocity * elapsed,1);
    //output.NewVelocity=float4(velocity,1);
	output.NewVelocity=float4(velocity,1);
    //output.NewPosition = float4(In.TexCoord*5,0,1);
	//output.NewPosition = float4(halfTexel,0,0,1);
	//output.NewPosition = float4(center,1);
	return output;
}




// Technique
technique10 particleSimulation
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, ps_main() ) );

	}
}

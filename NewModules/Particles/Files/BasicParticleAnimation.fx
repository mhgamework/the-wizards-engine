

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;
shared float4x4 view : View;
shared float4x4 projection : Projection;

shared float4x4 viewInverse : ViewInverse;

texture oldPosition : Diffuse
sampler OldPositionSampler = sampler_state
{
	Texture = <oldPosition>;
	MinFilter=Linear;
	MagFilter=Linear;
	MipFilter=Linear;
};
texture oldVelocity : Diffuse
sampler OldVelocitySampler = sampler_state
{
	Texture = <oldVelocity>;
	MinFilter=Linear;
	MagFilter=Linear;
	MipFilter=Linear;
};

// Vertex Output Declaration
struct VSOut
{
	float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
	
};


 float width;
 float height;

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
    output.TexCoord = input.TexCoord; //- halfPixel;
    return output;
}
float elapsed;

struct PixelShaderOutput
{
    half4 NewPosition : COLOR0;
    half4 NewVelocity : COLOR1;
    
};
// Pixel Shader
PixelShaderOutput ps_main(VSOut In) : COLOR0
{
	
	float4 oldPosition =tex2D(OldPositionSampler, In.TexCoord);
	float4 oldVelocity =tex2D(OldVelocitySampler, In.TexCoord);

	float4 velocity=oldVelocity+float4(0,-5*elapsed,0,1);//simple gravity calculation
	PixelShaderOutput output;
	output.NewPosition = oldPosition + float4(velocity * elapsed,1)
	output.NewVelocity=velocity;
	return output;
}

// Technique
technique Billboard
{
    pass p0 
    {	
		// Shaders
		VertexShader = compile vs_2_0 vs_main();
		PixelShader  = compile ps_2_0 ps_main();
		
    }
}
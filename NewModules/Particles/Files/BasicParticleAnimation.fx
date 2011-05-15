
texture oldPosition;
sampler OldPositionSampler = sampler_state
{
	Texture = <oldPosition>;
	MinFilter=Linear;
	MagFilter=Linear;
	MipFilter=Linear;
};
texture oldVelocity ;
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
float size;
float3 center;
// Pixel Shader
PixelShaderOutput ps_main(VSOut In)
{
	float halfTexel = 1.0/size*0.5;
	float4 oldPosition =tex2D(OldPositionSampler, In.TexCoord+halfTexel);
	float4 oldVelocity =tex2D(OldVelocitySampler, In.TexCoord+halfTexel);

	float3 velocity=oldVelocity.xyz+float4(0,0,0,1);//float4(normalize((center.xyz-oldPosition.xyz))*dot(oldVelocity,oldVelocity)*elapsed,1);
	velocity=clamp(velocity,float3(-10,-10,-10),float3(10,10,10));
	//velocity = float3(0,0,0);

	PixelShaderOutput output;
	output.NewPosition = float4(oldPosition.xyz + velocity * elapsed,1);
	//output.NewVelocity=float4(velocity,1);
	output.NewVelocity=float4(velocity,1);
	//output.NewPosition = float4(In.TexCoord,0,1);
	return output;
}

// Technique
technique particleSimulation
{
    pass p0 
    {	
		// Shaders
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader  = compile ps_3_0 ps_main();
		
    }
}
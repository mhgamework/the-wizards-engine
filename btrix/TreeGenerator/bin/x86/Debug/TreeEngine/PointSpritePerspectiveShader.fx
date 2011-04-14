/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;


shared float4x4 viewInverse : ViewInverse;

texture diffuseTexture : Diffuse
<
	string UIName = "Diffuse Texture";
	
>;
sampler DiffuseTextureSampler = sampler_state
{
	Texture = <diffuseTexture>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};
sampler PixelTexSampler = sampler_state
{
	Texture = <diffuseTexture>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};


//----------------------------------------------------




// testing of the point sprite shader file doesn't load strangely enough

//------- Technique: PointSprites --------
//by me
struct VertexInput
{
	float3 pos : POSITION;
	float2 Size:TEXCOORD0;
};

struct SpritesVertexOut
{
    float4 Position: POSITION0;
    float Size : PSIZE;
	//float2 TexCoord:TEXCOORD0;
	float4 OutPosition: TEXCOORD1;
};
struct SpritesPixelIn
{
    float4 Position: TEXCOORD1;
	float2 TexCoord : TEXCOORD0;
	
};


SpritesVertexOut PointSpritesVS (VertexInput In)//float4 Position : POSITION, float4 Color : COLOR0, float Size : PSIZE)
{
    SpritesVertexOut Output;
  
	Output.Position = mul(float4(In.pos,1),mul(world,viewProjection));
	Output.OutPosition = Output.Position;
    Output.Size =350/(Output.Position.w) * In.Size.x;  //(1/pow(Output.Position.z/Output.Position.w,2)+1) * 1000
    
    
    return Output; 
}

float4 PointSpritesPS(SpritesPixelIn PSIn) : Color0
{ 
//return float4(-PSIn.Position.z*100,0,0,1);
	float2 texCoord = PSIn.TexCoord.xy;
	float4 diffuse = tex2D(DiffuseTextureSampler, texCoord);
	diffuse.rgb = diffuse.rgb * 0.9;
    //return float4(diffuse.a,0,0,1);
    return diffuse;
}


technique PointSprites
{
	pass P0
	{   
		PointSpriteEnable = true;
		VertexShader = compile vs_2_0 PointSpritesVS();
		PixelShader  = compile ps_2_0 PointSpritesPS();
	}
}


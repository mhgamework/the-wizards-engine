/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;


shared float4x4 viewInverse : ViewInverse;



texture diffuseTexture : Diffuse
<
	string UIName = "Diffuse Texture";
	//string ResourceName = "marble.dds";
>;
sampler DiffuseTextureSampler = sampler_state
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
	float Size:PSIZE;
};

struct SpritesVertexOut
{
    float4 Position: POSITION0;
    float Size : PSIZE;
	//float2 TexCoord:TEXCOORD0;
};
struct SpritesPixelIn
{
    
	float2 TexCoord : TEXCOORD0;
	
};
struct PixelToFrame
{
    float4 Color : COLOR0;
};

SpritesVertexOut PointSpritesVS (VertexInput In)//float4 Position : POSITION, float4 Color : COLOR0, float Size : PSIZE)
{
    SpritesVertexOut Output;
     
    //float4x4 preViewProjection = mul (viewInverse, viewProjection);
	//float4x4 preWorldViewProjection = mul (world, preViewProjection); 
    //Output.Position = mul(Position, preWorldViewProjection);
	Output.Position = mul(float4(In.pos,1), viewProjection);
    Output.Size = 350/(Output.Position.z) * In.Size;  //(1/pow(Output.Position.z/Output.Position.w,2)+1) * 1000;
    //Output.Size=100;
    
    
    return Output; 
}

PixelToFrame PointSpritesPS(SpritesPixelIn PSIn)
{ 
	
    PixelToFrame Output = (PixelToFrame)0;    
   // Output.Color = float4(1,0,0,1);
 //return Output;
   
		float2 texCoord = PSIn.TexCoord.xy;
    

    Output.Color = tex2D(DiffuseTextureSampler, texCoord);
    
    return Output;
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


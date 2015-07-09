

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;
shared float4x4 view : View;
shared float4x4 projection : Projection;

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

// Vertex Output Declaration
struct VSOut
{
	float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
	
};

struct VertexInput
{
	float3 Position : POSITION;
	float2 size : TEXCOORD0;
	float2  TexCoord:TEXCOORD1;
	float2 TexCoord0:TEXCOORD2;
	//float3 Normal : NORMAL;
};
float width;
float height;

VSOut vs_main(VertexInput In)
{
	VSOut output;
	float4 pos=mul(float4(In.Position,1),world);
   
   float4 translationUp=viewInverse[1]*In.TexCoord.y*In.size.y;
   float4 translationRight=viewInverse[0]*In.TexCoord.x*In.size.x;
   //translationUp=float3(0,1,0)*In.TexCoord.y*In.size.y;
   //translationRight=float3(1,0,0)*In.TexCoord.x*In.size.x;
   //pos=pos/pos.w;
   pos=pos+translationUp+ translationRight;

  
   output.Position=mul(pos,viewProjection);
	output.TexCoord=In.TexCoord0;
    return output;
}

// Pixel Shader
float4 ps_main(VSOut In) : COLOR0
{
	//return float4(1,0,0,1);
	float4 Out;
	Out =tex2D(DiffuseTextureSampler, In.TexCoord);
	return Out;
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
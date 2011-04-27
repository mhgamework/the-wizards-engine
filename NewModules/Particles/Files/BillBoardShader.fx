

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
texture displacementTexture;
sampler displacementSampler = sampler_state
{
	Texture = <displacementTexture>;
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

struct VertexInput
{
	float2 uv : TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
	
};
 float width;
 float height;
 float size;
VSOut vs_main(VertexInput In)
{
	VSOut output;
	float halfTexel = 1.0/size*0.5;
	float2 mapUV = In.uv.xy / size + halfTexel;
	float4 position= tex2Dlod(displacementSampler, float4(mapUV,0,0));
	//position= float4(0,0,0,1);
	float4 pos=mul(position,world);
 
   float4 translationUp=viewInverse[1]*In.TexCoord.y*height;
   float4 translationRight=viewInverse[0]*In.TexCoord.x*width;
   //float4 translationUp = float4(In.TexCoord,0,1);
   //translationUp=float3(0,1,0)*In.TexCoord.y*In.size.y;
   //translationRight=float3(1,0,0)*In.TexCoord.x*In.size.x;
   //pos=pos/pos.w;
   pos=pos+translationUp+ translationRight;

   
  
	output.Position=mul(pos,viewProjection);
	output.TexCoord=float2(0.5,0.5)+In.TexCoord;
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
		VertexShader = compile vs_3_0 vs_main();
		PixelShader  = compile ps_3_0 ps_main();
		
    }
}
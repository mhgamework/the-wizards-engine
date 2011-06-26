

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;
shared float4x4 view : View;
shared float4x4 projection : Projection;

shared float4x4 viewInverse : ViewInverse;
float4 startColor=float4(1,0.4f,0.4f,1);
float4 endColor=float4(1,0.2f,0.2f,1);
float oneOverTotalLifeTime=1/1000.0f;
float width;
 float height;
 float heightEnd;
 float widthEnd;
 float size;
 float2 uvStart=float2(0,0);
 float2 uvSize=float2(1,1);
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
texture timeTexture;
sampler timeSampler = sampler_state
{
	Texture = <timeTexture>;
	MinFilter=Linear;
	MagFilter=Linear;
	MipFilter=Linear;
};
// Vertex Output Declaration
struct VSOut
{
	float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
	float lifeTime	: TEXCOORD1;
	
};

struct VertexInput
{
	float2 uv : TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
	
};

 float currentTime;
 
VSOut vs_main(VertexInput In)
{
	VSOut output;
	float halfTexel = 1.0/size*0.5;
	float2 mapUV = In.uv.xy / size + halfTexel;
	float4 position= tex2Dlod(displacementSampler, float4(mapUV,0,0));
	//position= float4(0,0,0,1);
	float4 pos=mul(position,world);
	//pos = float4(In.uv,0,1);
	float createTime= tex2Dlod(timeSampler, float4(mapUV,0,0)).x;
	float lifetime= currentTime-createTime;

   float4 translationUp=viewInverse[1]*In.TexCoord.y*lerp(height,heightEnd,lifetime*oneOverTotalLifeTime);
   float4 translationRight=viewInverse[0]*In.TexCoord.x*lerp(width,widthEnd,lifetime*oneOverTotalLifeTime);
   //float4 translationUp = float4(In.TexCoord,0,1);
   //translationUp=float3(0,1,0)*In.TexCoord.y*In.size.y;
   //translationRight=float3(1,0,0)*In.TexCoord.x*In.size.x;
   //pos=pos/pos.w;
   pos=pos+translationUp+ translationRight;

   
  
	output.Position=mul(pos,viewProjection);
	output.TexCoord.x=(0.5+In.TexCoord.x)*uvSize.x+uvStart.x;
	output.TexCoord.y=(0.5+In.TexCoord.y)*uvSize.y+uvStart.y;
	output.lifeTime=lifetime;
    return output;
}
float3 startPosition=float3(0,0,0);
VSOut vs_DirectionalBillboard(VertexInput In)
{
	VSOut output;
	float halfTexel = 1.0/size*0.5;
	float2 mapUV = In.uv.xy / size + halfTexel;
	float4 position= tex2Dlod(displacementSampler, float4(mapUV,0,0));
	//position= float4(0,0,0,1);
	float4 pos=mul(position,world);
	//pos = float4(In.uv,0,1);
	float createTime= tex2Dlod(timeSampler, float4(mapUV,0,0)).x;
	float lifetime= currentTime-createTime;

	
   float4 Up=viewInverse[1];
   float4 Right=viewInverse[0];
   //WARNING: startPosition should in WorldSpace
   float3 dir=normalize(startPosition-pos.xyz);  
   float3 cameraDirection = viewInverse[2];
   //cameraDirection = float3(0,0,1); 
   float3 translationUp=normalize(cross(dir,cameraDirection));
	
   //float4 translationUp = float4(In.TexCoord,0,1);
   //translationUp=float3(0,1,0)*In.TexCoord.y*In.size.y;
   //translationRight=float3(1,0,0)*In.TexCoord.x*In.size.x;
   //pos=pos/pos.w;
   //pos=pos+ translationUp*In.TexCoord.y*lerp(height,heightEnd,lifetime*oneOverTotalLifeTime)+ dir*(In.TexCoord.x*lerp(width,widthEnd,lifetime*oneOverTotalLifeTime)+0.5);
   pos=pos+ float4(translationUp*In.TexCoord.y*lerp(height,heightEnd,lifetime*oneOverTotalLifeTime),1)
		+ float4(dir*(In.TexCoord.x-0.5)*lerp(width,widthEnd,lifetime*oneOverTotalLifeTime),1);
   
  
	output.Position=mul(pos,viewProjection);
	output.TexCoord.x=(0.5+In.TexCoord.x)*uvSize.x+uvStart.x;
	output.TexCoord.y=(0.5+In.TexCoord.y)*uvSize.y+uvStart.y;//(float2(0.5,0.5)+In.TexCoord)*uvEnd+ uvStart;//float2(0.5,0.5)+In.TexCoord;
	output.lifeTime=lifetime;
    return output;
}



// Pixel Shader
float4 ps_main(VSOut In) : COLOR0
{
	
	

	//return float4(lifetime*oneOverTotalLifeTime,0,0,1);
	float4 Out;
	
	Out =tex2D(DiffuseTextureSampler, In.TexCoord);
	
	float3 color=lerp(startColor,endColor,In.lifeTime*oneOverTotalLifeTime);
	return float4(color,Out.r);//float4(Out.a,0,0,1);
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
technique DirectionalBillboard
{
	pass p1
	{
	VertexShader = compile vs_3_0 vs_DirectionalBillboard();
	PixelShader  = compile ps_3_0 ps_main();
		
    }
}
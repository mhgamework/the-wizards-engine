#include <TestHelper.fx>

/*** Camera Info ***/

float4x4 world : World;
float4x4 viewProjection : ViewProjection;
float4x4 view : View;
float4x4 projection : Projection;

float4x4 viewInverse : ViewInverse;
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
Texture2D txDiffuse;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerState samPoint
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};
Texture2D displacementTexture;

Texture2D timeTexture;

// Vertex Output Declaration
struct VSOut
{
	float4 Position	: SV_POSITION;
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
	

	
	float2 mapUV = In.uv.xy / size;
	float4 position= displacementTexture.Load( float3(In.uv.xy,0)); //otherwise use sample
	//position= float4(0,0,0,1);
	float4 pos=mul(position,world);
	float2 map2 = mapUV;

	float4 test = timeTexture.Load( float3( In.uv.xy,0));

	float createTime = test.x;
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

	//output.TexCoord.x = test.x;
    return output;
}
float3 startPosition=float3(0,0,0);
VSOut vs_DirectionalBillboard(VertexInput In)
{
	VSOut output;
	
	
	float4 position= displacementTexture.Load(float3(In.uv.xy,0));
	//position= float4(0,0,0,1);
	float4 pos=mul(position,world);
	//pos = float4(In.uv,0,1);
	float createTime= timeTexture.Load(float3(In.uv.xy,0));
	float lifetime= currentTime-createTime;

	
   float4 Up=viewInverse[1];
   float4 Right=viewInverse[0];
   //WARNING: startPosition should in WorldSpace
   float3 dir=normalize(startPosition-pos.xyz);  
   float3 cameraDirection = viewInverse[2].xyz;
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
float4 ps_main(VSOut In):SV_TARGET0
{
	
	
		clip(-In.lifeTime*oneOverTotalLifeTime+1);
	//return float4(0.1f,0,0,0.1f);
	//return float4(In.lifeTime*oneOverTotalLifeTime-1,0,0,1);
	float4 Out;
	
	Out =txDiffuse.Sample( samLinear, In.TexCoord);
	
	float3 color=lerp(startColor.rgb,endColor.rgb,In.lifeTime*oneOverTotalLifeTime);
	return float4(color*Out.r,Out.r);//float4(Out.a,0,0,1);
}

// Technique

technique10 Billboard
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, vs_main() ) );
		SetPixelShader( CompileShader( ps_4_0, ps_main() ) );

	}
}
technique10 DirectionalBillboard
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, vs_DirectionalBillboard() ) );
		SetPixelShader( CompileShader( ps_4_0, ps_main() ) );

	}
}
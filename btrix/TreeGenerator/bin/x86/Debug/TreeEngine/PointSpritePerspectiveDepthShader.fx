/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;
shared float4x4 viewInverse : ViewInverse;
float4x4 g_matView : VIEW;
float4x4 g_matProj : PROJECTION;
float g_fFarClip = 100.0f;


struct VertexInput
{
	float3 pos : POSITION;
	float2 Size:TEXCOORD0;
};

struct SpritesVertexOut
{
    float4 Position: POSITION0;
    float Size : PSIZE;
	float out_fDepthVS:TEXCOORD0;
};


SpritesVertexOut PointSpritesVS (VertexInput In)
{
    SpritesVertexOut Output;
  
	float4 Position = mul(float4(In.pos,1),mul(world,viewProjection));
    Output.Size =350/(Position.w) * In.Size.x;  //(1/pow(Output.Position.z/Output.Position.w,2)+1) * 1000
    
    float4x4 matWorldView = mul(world, g_matView);
	float4 vPositionVS = mul(float4(In.pos,1), matWorldView);
	Output.Position = mul(vPositionVS, g_matProj);
	Output.out_fDepthVS = vPositionVS.z;

    return Output; 
}

float4 PointSpritesPS(in float in_fDepthVS : TEXCOORD0) : Color0
{ 
// Negate and divide by distance to far clip (so that depth is in range [0,1])
    //return float4(0,0,0,1);
	//float fDepth = -in_fDepthVS/g_fFarClip;
	float fDepth =in_fDepthVS/100;

    return float4(fDepth, fDepth, fDepth, 1.0f);
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


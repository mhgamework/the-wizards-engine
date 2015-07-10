// Project: XNARacerXNA, File: LineRendering.fx
// Path: C:\code\XNARacerXNA\Shaders, Author: Abi
// Code lines: 52, Size of file: 1,18 KB
// Creation date: 31.08.2006 05:36
// Last modified: 31.08.2006 06:44
// Generated with Commenter by abi.exDream.com
// Note: To test this use FX Composer from NVIDIA!

string description = "Line rendering helper shader for XNA";

// Default variables, supported by the engine
matrix worldViewProjection : WorldViewProjection;

struct VertexInput
{
	float3 pos   : POSITION;
	float4 color : COLOR;
};

struct VertexOutput 
{
   float4 pos   : SV_POSITION;
   float4 color : COLOR;
};

VertexOutput LineRenderingVS(VertexInput In)
{
	VertexOutput Out;
	
	// Transform position
	Out.pos = mul(float4(In.pos, 1), worldViewProjection);
	Out.color = In.color;

	// And pass everything to the pixel shader
	return Out;
} // LineRenderingVS(VertexInput In)

float4 LineRenderingPS(VertexOutput In) : SV_Target
{
	return In.color;
} // LineRenderingPS(VertexOutput In)

VertexOutput LineRendering2DVS(VertexInput In)
{
	VertexOutput Out;
	
	// Transform position (just pass over)
	Out.pos = float4(In.pos, 1);
	Out.color = In.color;

	// And pass everything to the pixel shader
	return Out;
} // LineRendering2DVS(VertexInput In)

float4 LineRendering2DPS(VertexOutput In) : SV_Target
{
	return In.color;
} // LineRendering2DPS(VertexOutput In)

//*does not work
// Techniques
technique10 LineRendering3D
{
	pass PassFor3D
	{
		VertexShader = compile vs_4_0 LineRenderingVS();
		PixelShader = compile ps_4_0 LineRenderingPS();
	} // PassFor3D
} // technique LineRendering

// Not accessible :(
technique LineRendering2D
{
	pass PassFor2D
	{
		VertexShader = compile vs_1_1 LineRendering2DVS();
		PixelShader = compile ps_1_1 LineRendering2DPS();
	} // PassFor2D
} // LineRendering
//*/

/*
// Techniques
technique LineRendering
{
	pass PassFor3D
	{
		VertexShader = compile vs_1_1 LineRenderingVS();
		PixelShader = compile ps_1_1 LineRenderingPS();
	} // PassFor3D
	
	pass PassFor2D
	{
		VertexShader = compile vs_1_1 LineRendering2DVS();
		PixelShader = compile ps_1_1 LineRendering2DPS();
	} // PassFor2D
} // LineRendering
*/
// Billboard Shader (Tutorial Part 1)

// Include RCs helpers
#include "Include\BBDXInclude.fxh"
ImportTexture0

// Constants
float4x4 World : World;
float4x4 View : View;
float4x4 Projection : Projection;

// Vertex Output Declaration
struct VSOut
{
	float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
};

// Vertex shader
VSOut vs_main(VSStandard In)
{
	// Setup our output variable
	VSOut Out;

	// Copy our input texture coordinate to the output
	Out.TexCoord = In.TexCoord;

	// Transform a virtual position into View Space
	Out.Position = mul(float4(0, 0, 0, 1), mul(World, View));

	// Offset the vertex position based upon its vertex position and its scale
	Out.Position.xy += In.Position.xy * float2(World[0][0], World[1][1]);

	// Transform vertex into screen space
	Out.Position = mul(Out.Position, Projection);

	// Return output
	return Out;
}

// Pixel Shader
float4 ps_main(VSOut In) : COLOR0
{
	// Setup out output color
	float4 Out;

	// Read our texture into the output
	Out = tex2D(Tex0, In.TexCoord);

	// Done, return the color
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
		
		// Render states
		alphablendenable = false;
		alphatestenable = true;
		alpharef = 0xf;
		alphafunc = greater;
    }
}
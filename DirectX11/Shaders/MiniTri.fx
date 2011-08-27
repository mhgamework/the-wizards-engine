// -----------------------------------------------------------------------------
// Original code from SlimDX project.
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
matrix world: WORLD;
matrix viewProjection: VIEWPROJECTION;

matrix GetWorld() { return world; }
matrix GetViewProjection() { return viewProjection; }
float4 Transform(float4 pos) 
{
	matrix m = mul(world,viewProjection); 
	return mul(pos,m);
}
float4 Transform(float3 pos) 
{
	return Transform(float4(pos,1));
}

Texture2D txDiffuse;
SamplerState samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float2 uv : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 uv : TEXCOORD;
};


PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = input.pos;
	output.col = input.col;
	output.uv = input.uv;
	
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	//return float4(1,1,0,1);
	//return float4(input.uv,0,1);
	return txDiffuse.Sample( samLinear, input.pos.xy/600);

	return input.col;
}

PS_IN VSTransform( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = Transform(input.pos);
	output.col = input.col;
	output.uv = output.pos.xy;
	
	return output;
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );

	}
}
technique10 RenderTransform
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VSTransform() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );

	}
}
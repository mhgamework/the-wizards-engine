texture lightMap;
sampler lightSampler = sampler_state
{
    Texture = (lightMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
texture diffuseMap;
sampler diffuseSampler = sampler_state
{
    Texture = (diffuseMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
//////////////////////////////////////////////////////////////////////
// Vertex shader
//////////////////////////////////////////////////////////////////////

struct VS_OUT
{
  float4 Pos:  POSITION;
  float2 Tex:  TEXCOORD0;
};
float2 halfPixel;
VS_OUT vs_main( float3 inPos: POSITION, float2 inTex: TEXCOORD0 )
{
  VS_OUT OUT;

  // Output the transformed vertex
  //OUT.Pos = mul( matMVP, float4( inPos, 1 ) );
  OUT.Pos = float4( inPos, 1 ) ;

  // Output the texture coordinates
  OUT.Tex = inTex -  halfPixel;

  return OUT;
}

//////////////////////////////////////////////////////////////////////
// Pixel shader
//////////////////////////////////////////////////////////////////////

float4 SuppressLDR( float4 c )
{
   if( c.r > 1.0f || c.g > 1.0f || c.b > 1.0f )
      return c;
   else
      return float4( 0.0f, 0.0f, 0.0f, 0.0f );
}

float4 ps_main( float2 inTex: TEXCOORD0 ) : COLOR0
{
//return float4(1,0,0,0);
  //float4 color = tex2D( DownSampler, inTex ) * Kd;
  float4 diffuse = tex2D(diffuseSampler,inTex);
  
  float4 light = tex2D( lightSampler, inTex )  ;
  //WARNING: this is incorrect and breaks the lightning equation in CombineFinal
  light = light * diffuse;

  return SuppressLDR( light );
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 vs_main();
        PixelShader = compile ps_2_0 ps_main();
    }
}
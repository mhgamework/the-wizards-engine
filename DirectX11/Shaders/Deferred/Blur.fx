texture blurMap;
sampler BlurSampler = sampler_state
{
    Texture = (blurMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};


float2 halfPixel;

// Pixel offsets ( 1 / 512, 1 / 384 )
//float2 PixelOffset   = float2( 0.001953195, 0.00260416666 );
float2 BlurOffset;    //= float2( 0.001953195, 0 );
float PixelWeight[8] = { 0.2537, 0.2185, 0.0821, 0.0461, 0.0262, 0.0162, 0.0102, 0.0052 };



struct VS_OUT
{
  float4 Pos:  POSITION;
  float2 Tex:  TEXCOORD0;
};

VS_OUT vs_main( float3 inPos: POSITION, float2 inTex: TEXCOORD0 )
{
  VS_OUT OUT;

  // Output the transformed vertex
  OUT.Pos =  float4( inPos, 1 );//mul( matMVP, float4( inPos, 1 ) );

  // Output the texture coordinates
  //OUT.Tex = inTex + ( PixelOffset * 0.5 );
  OUT.Tex = inTex + ( halfPixel );

  return OUT;
}


float4 ps_blur( float2 inTex: TEXCOORD0 ) : COLOR0
{
  float4 color = tex2D( BlurSampler, inTex );

  
  // Sample pixels on either side
  for( int i = 0; i < 8; ++i )
  {
    color += tex2D( BlurSampler, inTex + ( BlurOffset * i ) )
             * PixelWeight[i];
    color += tex2D( BlurSampler, inTex - ( BlurOffset * i ) )
             * PixelWeight[i];
  }

  return color;
}

technique GaussionBlur
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 vs_main();
        PixelShader = compile ps_2_0 ps_blur();
    }
}
texture rnm;
texture normalMap;
texture depthMap;
sampler rnmSampler = sampler_state
{
    Texture = (rnm);
    AddressU = WRAP;
    AddressV = WRAP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler normalMapSampler = sampler_state
{
    Texture = (normalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler depthMapSampler = sampler_state
{
    Texture = (depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
static const float totStrength = 1.38;
static const float strength = 0.07;//0.07;
static const float offset = 50.0;//18
static const float falloff = 0.000002;
//static const float rad = 0.006;
static const float rad = 0.01;
#define SAMPLES 10 // 10 is good
static const float invSamples = -1.38/10.0; 

float2 halfPixel; 

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    //align texture coordinates
    output.TexCoord = input.TexCoord - halfPixel;
    return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

	float2 uv = input.TexCoord;
	
	// these are the random vectors inside a unit sphere
	float3 pSphere[10] = {float3(-0.010735935, 0.01647018, 0.0062425877),float3(-0.06533369, 0.3647007, -0.13746321),float3(-0.6539235, -0.016726388, -0.53000957),float3(0.40958285, 0.0052428036, -0.5591124),float3(-0.1465366, 0.09899267, 0.15571679),float3(-0.44122112, -0.5458797, 0.04912532),float3(0.03755566, -0.10961345, -0.33040273),float3(0.019100213, 0.29652783, 0.066237666),float3(0.8765323, 0.011236004, 0.28265962),float3(0.29264435, -0.40794238, 0.15964167)};
 
   // grab a normal for reflecting the sample rays later on
   float3 fres = normalize((tex2D(rnmSampler,uv*offset).xyz*2.0) - float3(1,1,1));
   
 
   float4 currentPixelSample = tex2D(normalMapSampler,uv);
 
   float currentPixelDepth = tex2D(depthMapSampler,uv).r;
 
   // current fragment coords in screen space
   float3 ep = float3(uv.xy,currentPixelDepth);
  // get the normal of current fragment
   float3 norm = 2.0f * currentPixelSample.xyz - 1.0f;
   
   float bl = 0.0;
   // adjust for the depth ( not shure if this is good..)
   float radD = rad/currentPixelDepth;
 
   //float3 ray, se, occNorm;
   float occluderDepth, depthDifference;
   float4 occluderFragment;
   float3 ray;
   int i = 0;
   for(int i=0; i<SAMPLES;++i)
   {
      // get a vector (randomized inside of a sphere with radius 1.0) from a texture and reflect it
      ray = radD*reflect(pSphere[i],fres);
	  
 
      // get the depth of the occluder fragment
      occluderFragment = tex2D(depthMapSampler,ep.xy + sign(dot(ray,norm) )*ray.xy);
	  float4 occluderFragmentNormal = tex2D(normalMapSampler,ep.xy + sign(dot(ray,norm) )*ray.xy);
	  float3 fragmentNormal = 2.0f * occluderFragmentNormal.xyz - 1.0f;
	  //return float4(ep.xy + sign(dot(ray,norm) )*ray.xy,0,0);
    // if depthDifference is negative = occluder is behind current fragment
      depthDifference = currentPixelDepth-occluderFragment.r;
 
      // calculate the difference between the normals as a weight
 // the falloff equation, starts at falloff and is kind of 1/x^2 falling
      bl += step(falloff,depthDifference)*(1.0-dot(fragmentNormal.xyz,norm))*(1.0-smoothstep(falloff,strength,depthDifference));
   }
 
   // output the result
   return float4( 1-(1.0+bl*invSamples),0,0,0);
   //return float4( (1.0+bl*invSamples),0,0,0);
 
}
technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
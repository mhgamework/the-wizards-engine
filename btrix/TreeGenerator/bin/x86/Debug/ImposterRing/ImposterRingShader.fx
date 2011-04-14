float4x4 World; 
float4x4 View; 
float4x4 ViewInverse;
float4x4 Projection; 
float LerpValue;
 //textures
 texture diffuseTexture1 : Diffuse
<
	string UIName = "Diffuse Texture1";
	//string ResourceName = "marble.dds";
>;
sampler DiffuseTextureSampler1 = sampler_state
{
	Texture = <diffuseTexture1>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};
 texture diffuseTexture2 : Diffuse
<
	string UIName = "Diffuse Texture2";
	//string ResourceName = "marble.dds";
>;
sampler DiffuseTextureSampler2 = sampler_state
{
	Texture = <diffuseTexture2>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};


struct VertexShaderInput 
{ 
    float4 Position  : POSITION0; 
    float2 UV:TEXCOORD0;
    
}; 
 
struct VertexShaderOutput 
{ 
    float4 Position : POSITION0;
    float2 UV:TEXCOORD0;
  
}; 
 
VertexShaderOutput VS_Morph(VertexShaderInput input) 
{ 
    VertexShaderOutput output; 
 
	float4x4 worldViewProj =  mul(World , mul(View,Projection));
 
    float4 pos = input.Position;
    float4 worldPosition = mul(pos, World); 
    float4 viewPosition  = mul(worldPosition, View);
    
  
    
	float3 worldPos = mul(pos, World);

    
    output.UV=input.UV;
    output.Position = mul(pos, worldViewProj); 
 
    return output; 
} 



float4 PS_Morph(VertexShaderOutput In) : COLOR
{
	
	
		float4 textureColor = lerp(tex2D(DiffuseTextureSampler1, In.UV),tex2D(DiffuseTextureSampler2, In.UV),LerpValue);
		return textureColor;
	
    								
	
					

	
} 


technique ImposterRing
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_Morph();
		PixelShader = compile ps_3_0 PS_Morph();
	} 
} 
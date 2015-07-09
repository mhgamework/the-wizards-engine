float4x4 World; 
float4x4 View; 
float4x4 ViewInverse;
float4x4 Projection; 
float LerpValue; 
float3 lightDir;
 
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


struct VertexShaderInput 
{ 
    float4 Position  : POSITION0; 
    float3 Normal: NORMAL0;
    float2 UV:TEXCOORD0;
    float4 NextPos   : POSITION1; 
	float3 NextNormal:NORMAL1;
	float2 NextUV:TEXCOORD1;
}; 
 
struct VertexShaderOutput 
{ 
    float4 Position : POSITION0;
    float3 Normal: NORMAL0;
    float2 UV:TEXCOORD0;
    float3 eyeVec: TEXCOORD1;
}; 
 
VertexShaderOutput VS_Morph(VertexShaderInput input) 
{ 
    VertexShaderOutput output; 
 
	float4x4 worldViewProj =  mul(World , mul(View,Projection));
 
    float4 pos = lerp( input.Position, input.NextPos, LerpValue ); 
    float4 worldPosition = mul(pos, World); 
    float4 viewPosition  = mul(worldPosition, View);
    float3 normal=  lerp( input.Normal, input.NextNormal, LerpValue );
    float2 UV=lerp( input.UV, input.NextUV, LerpValue );
    
    
    
    
	

	float3 eyePos =ViewInverse[3];
	
	float3 worldPos = mul(pos, World);

	float3 eyeVector = normalize(eyePos-worldPos);
    
    
       
    
	
   
    
    
    
    
    
    
    
    
    output.eyeVec = eyeVector;
    output.Normal = mul(normal, World);
    output.UV=UV;
    output.Position      = mul(pos, worldViewProj); 
 
    return output; 
} 

// Calculates the contribution for a single light source using phong lighting
float3 CalcLightingPhong (	float3 vDiffuseAlbedo, 
		float3 vSpecularAlbedo, 
		float fSpecularPower, 
		float3 vLightColor, 
		float3 vNormal, 
		float3 vLightDir, 
		float3 vViewDir	)
{
	float3 R = normalize(reflect(-vLightDir, vNormal));
    
    // Calculate the raw lighting terms
    float fDiffuseReflectance = saturate(dot(vNormal, vLightDir));
    float fSpecularReflectance = saturate(dot(R, vViewDir));

	// Modulate the lighting terms based on the material colors, and the attenuation factor
    float3 vSpecular = vSpecularAlbedo * vLightColor * pow(fSpecularReflectance, fSpecularPower);
    float3 vDiffuse = vDiffuseAlbedo * vLightColor * fDiffuseReflectance;	

	// Lighting contribution is the sum of the diffuse and specular terms
	return vDiffuse + vSpecular;
}

float4 PS_Morph(VertexShaderOutput In) : COLOR
{
	
	// Normalize after interpolation
	float3 vNormalWS = normalize(In.Normal);
    float4 textureColor = tex2D(DiffuseTextureSampler, In.UV);
    //return float4(textureColor.a,0,0,1);
    //float4 textureColor = float4(0,1,0,1);
    float4 specularColor = float4(1,0,0,1);
    float shininess = 20;
    float3 lightColor = float3(1,1,1);
        
    float3 vLightDirWS = normalize(lightDir);
    
    float3 vViewDirWS = normalize(In.eyeVec);
    
    // Calculate the lighting term for the directional light
    float3 phong=CalcLightingPhong(	textureColor.rgb, 
														specularColor.rgb, 
														shininess,
														lightColor, 
														vNormalWS, 
														vLightDirWS, 
														vViewDirWS);
														
	return float4(textureColor*0.4+phong*0.6,textureColor.a);
					

	
} 


technique AnimatedGrass
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_Morph();
		PixelShader = compile ps_3_0 PS_Morph();
	} 
} 
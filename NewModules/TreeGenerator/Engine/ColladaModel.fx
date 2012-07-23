// Tutorial 06: Simple shader
//float4x4 worldViewProj : WorldViewProjection;
float4x4 viewProjection : ViewProjection;
float4x4 world : World;
float4x4 viewInverse : ViewInverse;

float3 lightDir : Direction
<
	string Object = "DirectionalLight";
	string Space = "World";
> = { 1, 0, 0 };

float4 ambientColor : Ambient
<
	string UIName = "Ambient Color";
	string Space = "material";
> = { 0.2f, 0.2f, 0.2f, 1.0f };
float4 diffuseColor : Diffuse 
<
	string UIName = "Diffuse Color";
	string Space = "material";
> = { 0.5f, 0.5f, 0.5f, 1.0f };
float4 specularColor : Specular
<
	string UIName = "Specular Color";
	string Space = "material";
> = { 1.0, 1.0, 1.0f, 1.0f };
float shininess : SpecularPower
<
	string UIName = "Specular Power";
	string UIWidget = "slider";
> = 24.0f;

texture diffuseTexture : Diffuse
<
	string UIName = "Diffuse Texture";
	//string ResourceName = "marble.dds";
>;
sampler DiffuseTextureSampler = sampler_state
{
	Texture = <diffuseTexture>;
	MinFilter=Anisotropic;
	MagFilter=Anisotropic;
	MipFilter=Linear;
};
float diffuseTextureRepeatU = 1;
float diffuseTextureRepeatV = 1;

texture normalTexture : NORMAL
<
	string UIName = "Normal Texture";
	//string ResourceName = "marble.dds";
>;
sampler NormalTextureSampler = sampler_state
{
	Texture = <normalTexture>;
	MinFilter=Anisotropic;
	MagFilter=Anisotropic;
	MipFilter=Linear;
};
float normalTextureRepeatU = 1;
float normalTextureRepeatV = 1;


struct VertexInput
{
	float3 pos : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL;
};

struct VertexOutput
{
	float4 pos : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 halfVec : TEXCOORD2;
};

// Vertex shader
VertexOutput VS_SpecularPerPixel(VertexInput In)
{
	VertexOutput Out = (VertexOutput)0;
	float4 pos = float4(In.pos, 1); 
	Out.pos = mul(pos, mul(world , viewProjection));
	Out.texCoord = In.texCoord * float2(diffuseTextureRepeatU ,diffuseTextureRepeatV );
	Out.normal = mul(In.normal, world);
	// Eye pos
	float3 eyePos = viewInverse[3];
	// World pos
	float3 worldPos = mul(pos, world);
	// Eye vector
	float3 eyeVector = normalize(eyePos-worldPos);
	// Half vector
	Out.halfVec = normalize(eyeVector+lightDir);
	
	return Out;
} // VS_SpecularPerPixel(In)

// Pixel shader
float4 PS_SpecularPerPixel(VertexOutput In) : COLOR
{
	float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
	float3 normal = normalize(In.normal);
	float brightness = dot(normal, lightDir);
    brightness = clamp(brightness,0.5,1); 

//return float4(brightness,0,0,1);
	float specular = pow(dot(normal, In.halfVec), shininess);
	return  float4(textureColor.rgb * brightness +
		//(ambientColor +
		//brightness * diffuseColor) +
		specular * specularColor.rgb,textureColor.a);
} // PS_SpecularPerPixel(In)

/*float4 PS_RenderNormals(VertexOutput In) : COLOR
{
	float3 normal = normalize(In.normal);


	return float4((In.normal+1)*0.5,1);
	
} */


float4 PS_SpecularPerPixelColored(VertexOutput In) : COLOR
{

	float3 normal = normalize(In.normal);
	float brightness = dot(normal, lightDir);
	float specular = pow(dot(normal, In.halfVec), shininess);
  
	float4 Out = float4(0,0,0,0);
	Out += ambientColor * 0.2;
	Out += (brightness * diffuseColor);
	Out += (specular * specularColor);
	return Out;
    //return float4(0,0,0,0);
    
    
	//return ambientColor +
	//	        brightness * diffuseColor +
	//        	specular * specularColor;
} // PS_SpecularPerPixel(In)

/*float4 PS_SpecularPerPixelNormalMapping(VertexOutput In) : COLOR
{
	float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
	//float3 normal = normalize(In.normal);

	//FOUT:: normalTextureRepeatU  wordt nog niet gebruikt!!!!

	float3 normal = tex2D(NormalTextureSampler , in.texCoord).rgb;

	float brightness = dot(normal, lightDir);
	float specular = pow(dot(normal, In.halfVec), shininess);
	return textureColor *
		(ambientColor +
		brightness * diffuseColor) +
		specular * specularColor;
} // PS_SpecularPerPixel(In)*/


















//----------------------------------------------------

float4 TransformPosition(float3 pos)
{
    return mul(mul(float4(pos.xyz,1), world), viewProjection);
}

float3 GetWorldPos(float3 pos)
{
    return mul(float4(pos, 1), world).xyz;
}

float3 GetCameraPos()
{
    return viewInverse[3].xyz;
}

float3 CalcNormalVector(float3 nor)
{
    return normalize(mul(nor, (float3x3)world));
}

float3 GetLightDir()
{
    return lightDir;
}

// Common functions
float3x3 ComputeTangentMatrix(float3 tangent, float3 normal)
{
	// Compute the 3x3 tranform from tangent space to object space
	float3x3 worldToTangentSpace;
	float3 binormal = cross(normal, tangent);
	worldToTangentSpace[0] = mul((float3)binormal, (float3x3)world);
	worldToTangentSpace[1] = mul((float3)tangent, (float3x3)world);
	worldToTangentSpace[2] = mul((float3)normal, (float3x3)world);
	return worldToTangentSpace;
} // ComputeTangentMatrix(..)


















// Vertex input structure (used for ALL techniques here!)
struct VertexInputNormalMapping
{
	float3 pos      : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 normal   : NORMAL;
	float3 tangent	: TANGENT;
};

//----------------------------------------------------



// -----------------------------------------------------

// Vertex output structure
struct VertexOutput_DiffuseSpecular20
{
	float4 pos      : POSITION;
	float2 texCoord	: TEXCOORD0;
	float3 lightVec : TEXCOORD1;
	float3 viewVec  : TEXCOORD2;
};

// Vertex shader for ps_2_0 (need more constants than ps_1_1)
VertexOutput_DiffuseSpecular20 VS_DiffuseSpecular20(VertexInputNormalMapping In)
{
	VertexOutput_DiffuseSpecular20 Out = (VertexOutput_DiffuseSpecular20)0;      
	
    Out.pos = TransformPosition(In.pos);
    
    //Er zouden aparte texcoords moeten zijn voor diffuse en normal
    Out.texCoord = In.texCoord * float2(diffuseTextureRepeatU ,diffuseTextureRepeatV );;
    
    // Compute the 3x3 tranform from tangent space to object space
    float3x3 worldToTangentSpace = ComputeTangentMatrix(In.tangent, In.normal);
	
    
    float3 worldEyePos = GetCameraPos();
    float3 worldVertPos = GetWorldPos(In.pos);
    
    //Transform light vector and pass it as a color (clamped from 0 to 1)
    // For ps_2_0 we don't need to clamp from 0 to 1
    Out.lightVec = normalize(mul(worldToTangentSpace, GetLightDir()));
    Out.viewVec = mul(worldToTangentSpace, worldEyePos - worldVertPos);
   
	// Rest of the calculation is done in pixel shader
	return Out;
} // VS_DiffuseSpecular20

// Pixel shader
float4 PS_DiffuseSpecular20(VertexOutput_DiffuseSpecular20 In) : COLOR
{
	// Grab texture data
	float4 diffuseTexture = tex2D(DiffuseTextureSampler, In.texCoord);
  
	float3 normalVector = (2.0 * tex2D(NormalTextureSampler,
            In.texCoord).rgb) - 1.0;

            
  // Normalize normal to fix blocky errors
	normalVector = normalize(normalVector);
//return diffuseTexture;
//return float4(normalVector/2+0.5f, 1);

//return float4(normalVector,1);
	// Additionally normalize the vectors
	float3 lightVector = In.lightVec;
    //return float4(lightVector,1);
	float3 viewVector = normalize(In.viewVec);
//return float4(lightVector, 1.0);
//return float4(viewVector, 1.0);
//return float4(normalVector,1);
	// Compute the angle to the light
	float bump = saturate(dot(normalVector, lightVector));
    //return float4(normalVector,1);
	// Specular factor
	float3 reflect = normalize(2 * bump * normalVector - lightVector);
    
    
  float spec = pow(saturate(dot(reflect, viewVector)), shininess);
//return float4(spec, 0, 0, 1);
//return float4(bump, 0, 0, 1);

	//float4 ambDiffColor = ambientColor + bump * diffuseColor;
    float4 ambDiffColor = bump *1.5 * float4(0.9,0.9,0.9,1);//* diffuseColor;
//return ambDiffColor * diffuseTexture;

  return diffuseTexture * ambDiffColor 
    + bump * spec * specularColor *0.2f;//* diffuseTexture.a;
} // PS_DiffuseSpecular20






































technique SpecularPerPixelNormalMapping
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_DiffuseSpecular20();
		PixelShader = compile ps_2_0 PS_DiffuseSpecular20();
	} // pass P0
} // SpecularPerPixel

technique SpecularPerPixelColored
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_SpecularPerPixel();
		PixelShader = compile ps_2_0 PS_SpecularPerPixelColored();
	} // pass P0
} // SpecularPerPixel

technique SpecularPerPixel
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_SpecularPerPixel();
		PixelShader = compile ps_2_0 PS_SpecularPerPixel();
	} // pass P0
} // SpecularPerPixel
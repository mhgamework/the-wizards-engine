/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;
shared float4x4 viewInverse : ViewInverse;

/*** Various ***/
shared float2 BackbufferSize;
shared float4 ambientColor : Ambient
<
	string UIName = "Ambient Color";
	string Space = "material";
> = { 0.2f, 0.2f, 0.2f, 1.0f };

/*** Shadow Occlusion map ***/
shared texture ShadowOcclusionTexture;
sampler2D ShadowOcclusionSampler = sampler_state
{
    Texture = <ShadowOcclusionTexture>;
    MinFilter = Point; 
    MagFilter = Point; 
    MipFilter = Point;
};

// Gets the screen-space texel coord from clip-space position
float2 CalcSSTexCoord (float4 vPositionCS, float2 BackbufferSize)
{
	float2 vSSTexCoord = vPositionCS.xy / vPositionCS.w;
	vSSTexCoord = vSSTexCoord * 0.5f + 0.5f;    
    vSSTexCoord.y = 1.0f - vSSTexCoord.y; 
    vSSTexCoord += 0.5f / BackbufferSize;    
    return vSSTexCoord;
} 

float3 CalcShadowTerm(float4 vPositionCS, float2 BackbufferSize)
{
    // Sample the shadow term based on screen position
    float2 vScreenCoord = CalcSSTexCoord(vPositionCS, BackbufferSize);
    return tex2D(ShadowOcclusionSampler, vScreenCoord).rgb; 
}




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





































float4 PIXELSHADER(
	  float4 position : TEXCOORD0,
	  float2 texcoord : TEXCOORD1,
	  float3 normal : TEXCOORD2,
	  float3 tangent : TEXCOORD3,
	  float3 binormal : TEXCOORD4) : COLOR
{
	  return float4(1,1,1,1);
}

void VERTEXSHADER(
		  float3 position : POSITION,
		  float2 texcoord : TEXCOORD0,
		  float3 normal : NORMAL,
		  float3 tangent : TANGENT,
		  float3 binormal : BINORMAL,
		  out float3 sys_position : POSITION,
		  out float3 out_position : TEXCOORD0,
		  out float2 out_texcoord : TEXCOORD1,
		  out float3 out_normal : TEXCOORD2,
		  out float3 out_tangent : TEXCOORD3,
		  out float3 out_binormal : TEXCOORD4)
{
		  sys_position = position;
		  out_position = position;
		  out_texcoord = texcoord;
		  out_normal = normal;
		  out_tangent = tangent;
		  out_binormal = binormal;
}
			

technique MainNOP
{
	  Pass p0
	  {
	  //VertexShader = compile vs_2_0 VERTEXSHADER();
	  CullMode = None;
	  AlphaBlendEnable = false;
	  ZEnable = true;
	  ZWriteEnable = true;
	  ZFunc = LessEqual;
	  //PixelShader = compile ps_2_0 PIXELSHADER();
	  }
}
	





shared float3 lightDir : Direction
<
	string Object = "DirectionalLight";
	string Space = "World";
> = { 1, 0, 0 };
shared float3 lightColor = float3(1,1,1);



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
	MagFilter=Linear;
	MipFilter=Linear;
	AddressU=Wrap;
	AddressV=Wrap;
};
float2 diffuseTextureRepeat = float2(1,1);
//float2 diffuseTextureRepeatU
//float diffuseTextureRepeatV = 1;

texture normalTexture : NORMAL
<
	string UIName = "Normal Texture";
	//string ResourceName = "marble.dds";
>;
sampler NormalTextureSampler = sampler_state
{
	Texture = <normalTexture>;
	MinFilter=Anisotropic;
	MagFilter=Linear;
	MipFilter=Linear;
};
float2 normalTextureRepeat = float2(1,1);
//float normalTextureRepeatU = 1;
//float normalTextureRepeatV = 1;








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
	float3 eyeVec : TEXCOORD2;
  float4 screenCoord : TEXCOORD3;
};





// Vertex shader
VertexOutput VS_SpecularPerPixel(VertexInput In)
{
	VertexOutput Out = (VertexOutput)0;
	float4 pos = float4(In.pos, 1); 
    
	Out.texCoord = In.texCoord * diffuseTextureRepeat;
	Out.normal = normalize(mul(In.normal, (float3x3)world));
	// Eye pos
	float3 eyePos = viewInverse[3];
	// World pos
	float3 worldPos = mul(pos, world);
	// Eye vector
	float3 eyeVector = normalize(eyePos-worldPos);
    
    Out.eyeVec = eyeVector;
    //Out.eyeVec = normalize(worldPos);
	// Half vector
	//Out.halfVec = normalize(eyeVector+lightDir);
    
    pos = mul(pos, mul(world , viewProjection));
	Out.pos = pos;
    
	Out.screenCoord = pos;
    
	return Out;
} // VS_SpecularPerPixel(In)

// Pixel shader
float4 PS_SpecularPerPixel(VertexOutput In) : COLOR
{

   // Sample the shadow term based on screen position
  float3 vShadowTerm = CalcShadowTerm(In.screenCoord,BackbufferSize).rgb;
  // This was added: this is a shader WITHOUT SHADOWS
  vShadowTerm = float3(1,1,1);

	// Normalize after interpolation
	float3 vNormalWS = normalize(In.normal);
    float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
        
    float3 vLightDirWS = normalize(-lightDir);
    
    float3 vViewDirWS = normalize(In.eyeVec);//normalize(g_vCameraPositionWS - in_vPositionWS);
    
    // Calculate the lighting term for the directional light
    float3 vLightContribition = 0;
    vLightContribition = vShadowTerm * CalcLightingPhong(	textureColor.rgb, 
														specularColor.rgb, 
														shininess,
														lightColor, 
														vNormalWS, 
														vLightDirWS, 
														vViewDirWS);
					
	// Add in ambient term	
	vLightContribition.xyz += textureColor.rgb * ambientColor.rgb;
													
	return float4(vLightContribition, textureColor.a);
    
    
    



	//float brightness = dot(normal, lightDir);
    //brightness = clamp(brightness,0.2,1); 

	//float specular = pow(dot(normal, In.halfVec), shininess);
	//return  shadowTerm * (float4(textureColor.rgb * brightness +
	//	specular * specularColor.rgb,textureColor.a));
} // PS_SpecularPerPixel(In)

// Pixel shader
float4 PS_TexturedShadowed(VertexOutput In) : COLOR
{

   // Sample the shadow term based on screen position
  float3 vShadowTerm = CalcShadowTerm(In.screenCoord,BackbufferSize).rgb;

	// Normalize after interpolation
	float3 vNormalWS = normalize(In.normal);
    float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
        
    float3 vLightDirWS = normalize(-lightDir);
    
    float3 vViewDirWS = normalize(In.eyeVec);//normalize(g_vCameraPositionWS - in_vPositionWS);
    
    // Calculate the lighting term for the directional light
    float3 vLightContribition = 0;
    vLightContribition = vShadowTerm * CalcLightingPhong(	textureColor.rgb, 
														specularColor.rgb, 
														shininess,
														lightColor, 
														vNormalWS, 
														vLightDirWS, 
														vViewDirWS);
					
	// Add in ambient term	
	vLightContribition.xyz += textureColor.rgb * ambientColor.rgb;
													
	return float4(vLightContribition, textureColor.a);
    
    

} 

/*float4 PS_RenderNormals(VertexOutput In) : COLOR
{
	float3 normal = normalize(In.normal);


	return float4((In.normal+1)*0.5,1);
	
} */


float4 PS_SpecularPerPixelColored(VertexOutput In) : COLOR
{
	//return float4(1,0,0,1);

   // Sample the shadow term based on screen position
  //float3 vShadowTerm = CalcShadowTerm(In.screenCoord,BackbufferSize).rgb;
float3 vShadowTerm = float3(1,1,1);

	// Normalize after interpolation
	float3 vNormalWS = normalize(In.normal);
       
    float3 vLightDirWS = normalize(-lightDir);
    float3 vViewDirWS = normalize(In.eyeVec);//normalize(g_vCameraPositionWS - in_vPositionWS);
    
    // Calculate the lighting term for the directional light
    float3 vLightContribition = 0;
    vLightContribition = vShadowTerm * CalcLightingPhong(	diffuseColor.rgb, 
														specularColor.rgb, 
														shininess,
														lightColor, 
														vNormalWS, 
														vLightDirWS, 
														vViewDirWS);
					
	// Add in ambient term	
	vLightContribition.xyz += diffuseColor.rgb * ambientColor.rgb;
													
    //return float4(specularColor.rgb,1);
    
	return float4(vLightContribition, 1.0f);
    
    

//return;
//return In.screenCoord;
    //return tex2D(ShadowOcclusionSampler, In.screenCoord); 
  float4 shadowTerm = float4(CalcShadowTerm(In.screenCoord,BackbufferSize).rgb,1);
  
	float3 normal = normalize(In.normal);
	float brightness = dot(normal, lightDir);
	//float specular = pow(dot(normal, In.halfVec), shininess);
    float specular = 0;
  
	float4 Out = float4(0,0,0,0);
	Out += ambientColor * 0.2;
	Out += shadowTerm * ((brightness * diffuseColor) + (specular * specularColor));
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





















float3 GetLightDir()
{
    return lightDir;
}















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
  float4 screenCoord : TEXCOORD3;
};

// Vertex shader for ps_2_0 (need more constants than ps_1_1)
VertexOutput_DiffuseSpecular20 VS_DiffuseSpecular20(VertexInputNormalMapping In)
{
	VertexOutput_DiffuseSpecular20 Out = (VertexOutput_DiffuseSpecular20)0;      
	
    float4 screenPosition = TransformPosition(In.pos);
    
    Out.pos = screenPosition;
    
    //Er zouden aparte texcoords moeten zijn voor diffuse en normal
    Out.texCoord = In.texCoord * diffuseTextureRepeat;
    
    // Compute the 3x3 tranform from tangent space to object space
    float3x3 worldToTangentSpace = ComputeTangentMatrix(In.tangent, In.normal);
	
    
    float3 worldEyePos = GetCameraPos();
    float3 worldVertPos = GetWorldPos(In.pos);
    
    //Transform light vector and pass it as a color (clamped from 0 to 1)
    // For ps_2_0 we don't need to clamp from 0 to 1
    Out.lightVec = normalize(mul(worldToTangentSpace, GetLightDir()));
    Out.viewVec = mul(worldToTangentSpace, worldEyePos - worldVertPos);
    Out.screenCoord = screenPosition;
   
	// Rest of the calculation is done in pixel shader
	return Out;
} // VS_DiffuseSpecular20



// Calculates the contribution for a single light source using normal mapping
float3 CalcLightingNormalMapping (	float3 vDiffuseAlbedo, 
						float3 vSpecularAlbedo, 
						float fSpecularPower, 
						float3 vLightColor, 
						float3 vNormal, 
						float3 vLightDir, 
						float3 vViewDir	)
{
//return vNormal;
	float3 R = normalize(reflect(-vLightDir, vNormal));
    
    // Calculate the raw lighting terms
    float fDiffuseReflectance = saturate(dot(vNormal, vLightDir));
    float fSpecularReflectance = saturate(dot(R, vViewDir));

	// Modulate the lighting terms based on the material colors, and the attenuation factor
    float3 vSpecular = vSpecularAlbedo * vLightColor * pow(fSpecularReflectance, fSpecularPower);
    float3 vDiffuse = vDiffuseAlbedo * vLightColor * fDiffuseReflectance;	

    //return vSpecularAlbedo;
	// Lighting contribution is the sum of the diffuse and specular terms
	return vDiffuse + vSpecular;
}



// Pixel shader
float4 PS_DiffuseSpecular20(VertexOutput_DiffuseSpecular20 In) : COLOR
{
  
    
    	// Grab texture data
	float4 diffuseTexture = tex2D(DiffuseTextureSampler, In.texCoord);
    	float3 normalVector = (2.0 * tex2D(NormalTextureSampler,
            In.texCoord).rgb) - 1.0;

            
  // Normalize normal to fix blocky errors
	normalVector = normalize(normalVector);
    //return float4(normalVector,1);
    
	// Additionally normalize the vectors
	float3 lightVector = In.lightVec;
	float3 viewVector = normalize(In.viewVec);

    
    
    
     // Sample the shadow term based on screen position
  float3 vShadowTerm = CalcShadowTerm(In.screenCoord,BackbufferSize).rgb;


	// Normalize after interpolation
	float3 vNormalWS = normalVector;//normalize(In.normal);
    float4 textureColor = diffuseTexture;
        
    float3 vLightDirWS = -lightVector;
    
    float3 vViewDirWS = viewVector;//normalize(g_vCameraPositionWS - in_vPositionWS);
    
    // Calculate the lighting term for the directional light
    float3 vLightContribition = 0;
    vLightContribition = CalcLightingPhong(	textureColor.rgb, 
														specularColor.rgb, 
														shininess,
														lightColor, 
														vNormalWS, 
														vLightDirWS, 
														vViewDirWS);
//return float4(vLightContribition,1);
					 vLightContribition *= vShadowTerm ;
	// Add in ambient term	
	vLightContribition.xyz += textureColor.rgb * ambientColor.rgb;
													
	return float4(vLightContribition, diffuseTexture.a);
    
    
   
} // PS_DiffuseSpecular20





// Pixel shader
float4 PS_DiffuseSpecular20Oud(VertexOutput_DiffuseSpecular20 In) : COLOR
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
	float3 lightVector = -In.lightVec;
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
  //spec = 0;
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


technique SpecularPerPixelTextured
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_SpecularPerPixel();
		PixelShader = compile ps_2_0 PS_SpecularPerPixel();
	} // pass P0
} // SpecularPerPixel


technique TexturedShadowed
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_SpecularPerPixel();
		PixelShader = compile ps_2_0 PS_TexturedShadowed();
	} // pass P0
} // SpecularPerPixel

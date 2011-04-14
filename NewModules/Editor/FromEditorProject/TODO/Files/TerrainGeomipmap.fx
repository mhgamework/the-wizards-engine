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

//float4x4 proj;                          // projection matrix  
float maxHeight = 128;           //maximum height of the terrain
texture displacementMap;       // this texture will point to the heightmap
sampler displacementSampler = sampler_state      //this sampler will be used to read (sample) the heightmap
{
        Texture = <displacementMap>;
        MipFilter = Point;
        MinFilter = Point;
        MagFilter = Point;
        AddressU = Clamp;
        AddressV = Clamp;
};

texture gridTileTexture;       // this texture will point to the heightmap
sampler gridTileSampler = sampler_state      //this sampler will be used to read (sample) the heightmap
{
        Texture = <gridTileTexture>;
        MipFilter = Linear;
        MinFilter = Anisotropic;
        MagFilter = Linear;
        AddressU = WRAP;
        AddressV = WRAP;
};



texture WeightMap1  <
    string ResourceName = "";
    string UIName =  "Weightmap1";
    string ResourceType = "2D";
>;

sampler2D WeightMap1Sampler = sampler_state {
    Texture = <WeightMap1>;
    MinFilter = Linear;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Mirror;
    AddressV = Mirror;
};

texture Texture1  <
    string ResourceName = "";
    string UIName =  "Texture1";
    string ResourceType = "2D";
>;

sampler2D Texture1Sampler = sampler_state {
    Texture = <Texture1>;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Texture2  <
    string ResourceName = "";
    string UIName =  "Texture2";
    string ResourceType = "2D";
>;

sampler2D Texture2Sampler = sampler_state {
    Texture = <Texture2>;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Texture3  <
    string ResourceName = "";
    string UIName =  "Texture3";
    string ResourceType = "2D";
>;

sampler2D Texture3Sampler = sampler_state {
    Texture = <Texture3>;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Texture4  <
    string ResourceName = "";
    string UIName =  "Texture4";
    string ResourceType = "2D";
>;

sampler2D Texture4Sampler = sampler_state {
    Texture = <Texture4>;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};


struct VS_INPUT_HEIGHTMAP {
    float4 position : POSITION;
    float4 uv : TEXCOORD0;
};
struct VS_OUTPUT_HEIGHTMAP
{
    float4 position  : POSITION;
    float4 uv : TEXCOORD0;
    float4 worldPos : TEXCOORD1;
    float2 texCoordDetail : TEXCOORD2;
	
};

VS_OUTPUT_HEIGHTMAP TransformHeightmap(VS_INPUT_HEIGHTMAP In)
{
    VS_OUTPUT_HEIGHTMAP Out = (VS_OUTPUT_HEIGHTMAP)0;                                  //initialize the output structure
    //float4x4 viewProj = mul(view, proj);                                        //compute View * Projection matrix
    float4x4 worldViewProj= mul(world, viewProjection);                      //finally, compute the World * View * Projection matrix
    // this instruction reads from the heightmap, the value at the corresponding texture coordinate
    // Note: we selected level 0 for the mipmap parameter of tex2Dlod, since we want to read data exactly as it appears in the heightmap
    In.position.y = tex2Dlod(displacementSampler, float4(In.uv.xy , 0 , 0 ));
    //Pass the world position the the Pixel Shader
    Out.worldPos = mul(In.position, world);
    //Compute the final projected position by multiplying with the world, view and projection matrices                                                      
    Out.position = mul( In.position , worldViewProj);
    Out.uv = In.uv;
    Out.texCoordDetail = In.uv * float2(128/2,128/2);
    return Out;
}

struct VS_INPUT_NORMAL
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 Normal   : NORMAL0;
};

struct VS_OUTPUT_NORMAL
{
    float4 Position  : POSITION0;
    float2 Texcoord  : TEXCOORD0;
    float3 Normal    : TEXCOORD1;
    float2 TexcoordDetail : TEXCOORD2;
    //float  Fog       : FOG;
};

VS_OUTPUT_NORMAL TransformNormal( VS_INPUT_NORMAL Input )
{
    VS_OUTPUT_NORMAL Output;
    float4x4 worldViewProj= mul(world, viewProjection);  
    Output.Position = mul( Input.Position, worldViewProj );
    Output.Texcoord = Input.Texcoord;
    Output.TexcoordDetail = Input.Texcoord * 32;
    Output.Normal   = normalize( Input.Normal );
    //Output.Fog      = 1.0f - ( length( Input.Position - CameraPosition ) / 4000.0f );

    return Output;
}



float4 PixelShaderHeightColored(in float4 worldPos : TEXCOORD1) : COLOR
{       
    return worldPos.y / maxHeight;
}

float4 PixelShader_GridTile(in float4 worldPos : TEXCOORD1) : COLOR
{       
    float4 color = tex2D(gridTileSampler,worldPos.xz);
    color *= color.a;
    return color;
}

struct PixelShader_INPUT_TexturedEditor
{
    float2 Texcoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float2 TexcoordDetail : TEXCOORD2;
};

float4 PixelShader_EditorTextured( PixelShader_INPUT_TexturedEditor Input ) : COLOR0
{
	//return float4(Input.Normal,1);
    float brightness =clamp(dot(Input.Normal, -lightDir),0.2,1);
    float4 weights = tex2D(WeightMap1Sampler, Input.Texcoord);
//return weights;
    float4 color1 = tex2D(Texture1Sampler, Input.TexcoordDetail) ;

    float4 color2 = tex2D(Texture2Sampler, Input.TexcoordDetail) ;
    float4 color3 = tex2D(Texture3Sampler, Input.TexcoordDetail) ;
    float4 color4 = tex2D(Texture4Sampler, Input.TexcoordDetail) ;
    
  
    
    color1 = color1 * weights.r;
    color2 = color2 * weights.g;
    color3 = color3 * weights.b;
    color4 = color4 * weights.a;

    color4 = color1+color2+color3+color4;
    return color4 * brightness;

}


VS_OUTPUT_NORMAL TransformPreprocessed( VS_INPUT_NORMAL Input )
{
    VS_OUTPUT_NORMAL Output;
    float4x4 worldViewProj= mul(world, viewProjection);  
    Output.Position = mul( Input.Position, worldViewProj );
    Output.Texcoord = Input.Texcoord;
    Output.TexcoordDetail = Input.Position.xz * (1/3.0f);
    Output.Normal   = normalize( Input.Normal );
    //Output.Fog      = 1.0f - ( length( Input.Position - CameraPosition ) / 4000.0f );

    return Output;
}

float4 PixelShader_PreprocessedTextured( PixelShader_INPUT_TexturedEditor Input ) : COLOR0
{
//return float4(1,0,0,1);
    float brightness =1;
    float4 weights = tex2D(WeightMap1Sampler, Input.Texcoord);
//return float4(weights);
//return float4(Input.Texcoord,0,1);
    float4 color1 = tex2D(Texture1Sampler, Input.TexcoordDetail) ;
//return color1;
    float4 color2 = tex2D(Texture2Sampler, Input.TexcoordDetail) ;
    float4 color3 = tex2D(Texture3Sampler, Input.TexcoordDetail) ;
    float4 color4 = tex2D(Texture4Sampler, Input.TexcoordDetail) ;
    
  
    
    color1 = color1 * weights.r;
    color2 = color2 * weights.g;
    color3 = color3 * weights.b;
    color4 = color4 * weights.a;

    color4 = color1+color2+color3+color4;
    return color4;

}
float4 PixelShader_PreprocessedWeightmap( PixelShader_INPUT_TexturedEditor Input ) : COLOR0
{

    float4 weights = tex2D(WeightMap1Sampler, Input.Texcoord);
	return float4(weights);

}
float4 PixelShader_PreprocessedWeightmapTexcoords( PixelShader_INPUT_TexturedEditor Input ) : COLOR0
{
return float4(Input.Texcoord,0,1);

}


struct VS_OUTPUT_SHADOWED
{
    float4 Position  : POSITION0;

    float2 Texcoord  : TEXCOORD0;
    float3 Normal    : TEXCOORD1;
    float2 TexcoordDetail : TEXCOORD2;
	float4 PositionCS  : TEXCOORD3;
	float4 PositionW  : TEXCOORD4;
};

VS_OUTPUT_SHADOWED TransformPreprocessedShadowed( VS_INPUT_NORMAL Input )
{
    VS_OUTPUT_SHADOWED Output;
    float4x4 worldViewProj= mul(world, viewProjection);  
	float4 posCS = mul( Input.Position, worldViewProj );
    Output.Position = posCS;
	Output.PositionCS = posCS;
	Output.PositionW = mul(Input.Position,world);
    Output.Texcoord = Input.Texcoord;
    Output.TexcoordDetail = Input.Position.xz * (1/3.0f);
    Output.Normal   = normalize( Input.Normal );
    //Output.Fog      = 1.0f - ( length( Input.Position - CameraPosition ) / 4000.0f );

    return Output;
}
float3 water_scattering(float3 InColor, float depth, float length)
{
	float extinction = exp(-0.02f*max(depth,0));
    float alpha = (1 - extinction*exp(-max(length,0)*0.2f));
    
    return lerp(InColor, 0.7f*float3(0.157, 0.431, 0.706), alpha);   
}
float4 PixelShader_PreprocessedTexturedShadowed( VS_OUTPUT_SHADOWED Input ) : COLOR0
{
	//return Input.PositionCS;
	
	float3 fShadowTerm = CalcShadowTerm(Input.PositionCS,BackbufferSize).rgb;
	//return float4(fShadowTerm,1);
//return float4(1,0,0,1);
    float brightness =1;
    float4 weights = tex2D(WeightMap1Sampler, Input.Texcoord);
//return float4(weights);
//return float4(Input.Texcoord,0,1);
    float4 color1 = tex2D(Texture1Sampler, Input.TexcoordDetail) ;
//return color1;
    float4 color2 = tex2D(Texture2Sampler, Input.TexcoordDetail) ;
    float4 color3 = tex2D(Texture3Sampler, Input.TexcoordDetail) ;
    float4 color4 = tex2D(Texture4Sampler, Input.TexcoordDetail) ;
    
  
    
    color1 = color1 * weights.r;
    color2 = color2 * weights.g;
    color3 = color3 * weights.b;
    color4 = color4 * weights.a;

    color4 = color1+color2+color3+color4;
	
	
			float3 vSpecularAlbedo = float3(1,1,1);
		float fSpecularPower = 500;
		float3 vLightColor = lightColor;
		float3 vNormal = normalize(Input.Normal);
		float3 vLightDir = -lightDir;
		float3 vViewDir	= float3(1,0,0);
	
	float4 result = float4(fShadowTerm * CalcLightingPhong (color4, 
		 vSpecularAlbedo, 
		 fSpecularPower, 
		 vLightColor, 
		 vNormal, 
		 vLightDir, 
		 vViewDir	),1);
		 
	// Add temp ambient
	result += ambientColor * color4;
	result.a = max(-Input.PositionW.y,0)*0.15f;
		//return float4(Input.PositionW.y,0,0,1);
		result.rgb = water_scattering(result.rgb,-Input.PositionW.y,0);
	return result;
    //return color4;

}



technique DrawHeightColored{ 
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformHeightmap(); 
        pixelShader  = compile ps_3_0 PixelShaderHeightColored(); 
    }
}
// Uses heightmap, should be DrawGridHeightmap
technique DrawGrid{ 
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformHeightmap(); 
        pixelShader  = compile ps_3_0 PixelShader_GridTile(); 
    }
}

technique DrawHeightMapTextured{ 
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformHeightmap(); 
        pixelShader  = compile ps_3_0 PixelShader_EditorTextured(); 
    }
}

technique DrawTexturedPreprocessed
{
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformPreprocessed(); 
        pixelShader  = compile ps_3_0 PixelShader_PreprocessedTextured(); 
    }
}

technique DrawTexturedPreprocessedShadowed
{
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformPreprocessedShadowed(); 
        pixelShader  = compile ps_3_0 PixelShader_PreprocessedTexturedShadowed(); 
    }
}

technique DrawWeightmapPreprocessed
{
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformPreprocessed(); 
        pixelShader  = compile ps_3_0 PixelShader_PreprocessedWeightmap(); 
    }
}

technique DrawWeightmapTexcoordsPreprocessed
{
    pass P0   
    {       
        vertexShader = compile vs_3_0 TransformPreprocessed(); 
        pixelShader  = compile ps_3_0 PixelShader_PreprocessedWeightmapTexcoords(); 
    }
}















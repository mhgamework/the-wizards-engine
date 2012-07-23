// Project: ColladaSkinningInXna, File: SkinnedNormalMapping.fx
// Creation date: 24.02.2007 09:56
// Last modified: 24.02.2007 17:31
// Author: Benjamin Nitschke (abi@exdream.com) (c) 2007
// Skinning shader for the test project to load skinned collada files
// Based on RocketCommanderXNA shaders.

// Default variables, supported by the engine (may not be used here)
// If you don't need any global variable, just comment it out, this way
// the game engine does not have to set it!
shared float4x4 viewProj         : ViewProjection;
float4x4 world            : World;
shared float3 cameraPos          : CameraPosition;

// Use 3x4 matrices to save some variables, this way we can have 80 instead
// of 60 bone matrices if we would use 4x4 matrices. Shader model 2.0
// can only guarantee 256 constants and 80 3x4 will fill up 240 of them.
// Note: Storing these as a big float4 makes both setting and accessing
// the data a little bit easier. Indices are always pre-multiplied by 3!
#define SKINNED_MATRICES_SIZE_VS20 80
shared float4 skinnedMatricesVS20[SKINNED_MATRICES_SIZE_VS20*3];

float SelectedBoneIndex = 0;

// Still using directional lighting here (like in RC), easier for testing.
float3 lightDir : Direction
<
	string Object = "DirectionalLight";
	string Space = "World";
> = { 1, 0, 0 };

// Color values for our material, all colors are pre-multiplied with the light color!
float4 ambientColor : Ambient
<
	string UIName = "Ambient Color";
	string Space = "material";
> = {0.1f, 0.1f, 0.1f, 1.0f};

float4 diffuseColor : Diffuse
<
	string UIName = "Diffuse Color";
	string Space = "material";
> = {1.0f, 1.0f, 1.0f, 1.0f};

float4 specularColor : Specular
<
	string UIName = "Specular Color";
	string Space = "material";
> = {1.0f, 1.0f, 1.0f, 1.0f};

float shininess : SpecularPower
<
	string UIName = "Specular Power";
	string UIWidget = "slider";
	float UIMin = 1.0;
	float UIMax = 128.0;
	float UIStep = 1.0;
> = 12.0;

// Material textures and samplers
texture diffuseTexture : Diffuse
<
	string UIName = "Diffuse Texture";
	//string ResourceName = "Goblin.dds";
>;
sampler diffuseTextureSampler = sampler_state
{
	Texture = <diffuseTexture>;
	AddressU  = Wrap;//Clamp;
	AddressV  = Wrap;//Clamp;
	AddressW  = Wrap;//Clamp;
	MinFilter=linear;
	MagFilter=linear;
	MipFilter=linear;
};
texture normalTexture : Diffuse
<
	string UIName = "Normal Texture";
	//string ResourceName = "asteroid4Normal.dds";
>;
sampler normalTextureSampler = sampler_state
{
	Texture = <normalTexture>;
	AddressU  = Wrap;//Clamp;
	AddressV  = Wrap;//Clamp;
	AddressW  = Wrap;//Clamp;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

//----------------------------------------------------

// Vertex input structure (used for ALL techniques here!)
struct VertexInput
{
	float3 pos      : POSITION;
	float3 blendWeights : BLENDWEIGHT;
	float3 blendIndices : BLENDINDICES;
	float2 texCoord : TEXCOORD0;
	float3 normal   : NORMAL;
	float3 tangent	: TANGENT;
};

//----------------------------------------------------

// Common functions
float3x3 ComputeTangentMatrix(float3 tangent, float3 normal, float4x4 world)
{
	// Compute the 3x3 tranform from tangent space to object space
	float3x3 worldToTangentSpace;
	float3 binormal = cross(normal, tangent);
	// Please not the reversed order we use the world matrix here.
	// The reason for this is the rebuild skin matrix, which is transposed!
	worldToTangentSpace[0] = normalize(mul((float3x3)world, (float3)tangent));
	worldToTangentSpace[1] = normalize(mul((float3x3)world, (float3)binormal));
	worldToTangentSpace[2] = normalize(mul((float3x3)world, (float3)normal));
	return worldToTangentSpace;
} // ComputeTangentMatrix(..)

// Note: This returns a transposed matrix, use it in reversed order.
// First tests used a 3x3 matrix +3 w values for the transpose values, but
// reconstructing this in the shader costs 20+ extra instructions and after
// some testing I found out this is finally the best way to use 4x3 matrices
// for skinning :)
float4x4 RebuildSkinMatrix(float index)
{
	return float4x4(
		skinnedMatricesVS20[index+0],
		skinnedMatricesVS20[index+1],
		skinnedMatricesVS20[index+2],
		float4(0, 0, 0, 1));
} // RebuildSkinMatrix(.)

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
VertexOutput_DiffuseSpecular20 VS_DiffuseSpecular20(VertexInput In)
{
	VertexOutput_DiffuseSpecular20 Out = (VertexOutput_DiffuseSpecular20)0;      

	// First transform position with bones that affect this vertex
	// Use the 3 indices and blend weights we have precalculated.
	float4x4 skinMatrix =
  	RebuildSkinMatrix(In.blendIndices.x) * In.blendWeights.x +
		RebuildSkinMatrix(In.blendIndices.y) * In.blendWeights.y +
		RebuildSkinMatrix(In.blendIndices.z) * In.blendWeights.z;
	
  // Calculate local world matrix with help of the skinning matrix
	float4x4 localWorld = mul(skinMatrix,transpose(world));//mul(world, skinMatrix);
    

	//float4x4 localWorld = mul(skinMatrix,world);
	
    	// Now calculate final screen position with world and viewProj matrices.
	float4 worldPos = mul(localWorld, float4(In.pos, 1));
	Out.pos = mul(worldPos, viewProj);
    //Out.pos = Out.pos / Out.pos.w;
	Out.texCoord = In.texCoord;
    
  	// Compute the 3x3 tranform from tangent space to object space
	float3x3 worldToTangentSpace =
		ComputeTangentMatrix(In.tangent, In.normal, localWorld);

	// Transform light and view vectors and pass them to the pixel shader
	Out.lightVec = normalize(mul(worldToTangentSpace, lightDir));
	Out.viewVec = mul(worldToTangentSpace, cameraPos - worldPos.xyz);


	//Out.lightVec = Out.pos.xyz;
	// Rest of the calculation is done in pixel shader
	return Out;
} // VS_DiffuseSpecular20

// Pixel shader
float4 PS_DiffuseSpecular20(VertexOutput_DiffuseSpecular20 In) : COLOR
{
	// Grab texture data
	float4 diffuseTexture = tex2D(diffuseTextureSampler, In.texCoord);
	float3 normalVector = (2.0 * tex2D(normalTextureSampler, In.texCoord).agb) - 1.0;
	// Normalize normal to fix blocky errors
	normalVector = normalize(normalVector);

	// Additionally normalize the vectors
	float3 lightVector = In.lightVec;
	float3 viewVector = normalize(In.viewVec);

	// Compute the angle to the light
	float bump = saturate(dot(normalVector, lightVector));
	// Specular factor
	float3 reflect = normalize(2 * bump * normalVector - lightVector);
	float spec = pow(saturate(dot(reflect, viewVector)), shininess);

	float4 ambDiffColor = ambientColor + bump * diffuseColor;
diffuseTexture.rgb = (diffuseTexture.rgb-0.5f)*1.25f+0.5f;
	return diffuseTexture * (ambientColor +
		bump * (diffuseColor + spec * specularColor));
} // PS_DiffuseSpecular20


// Vertex output structure
struct VertexOutput_DiffuseSpecularColored20
{
	float4 pos      : POSITION;
	float2 texCoord	: TEXCOORD0;
	float3 halfVec : TEXCOORD1;
	//float3 viewVec  : TEXCOORD2;
  float3 normal   : TEXCOORD4;
};

// Vertex shader for ps_2_0 (need more constants than ps_1_1)
VertexOutput_DiffuseSpecularColored20 VS_DiffuseSpecularColored20(VertexInput In)
{
	VertexOutput_DiffuseSpecularColored20 Out = (VertexOutput_DiffuseSpecularColored20)0;      

	// First transform position with bones that affect this vertex
	// Use the 3 indices and blend weights we have precalculated.
	float4x4 skinMatrix =
  	RebuildSkinMatrix(In.blendIndices.x) * In.blendWeights.x +
		RebuildSkinMatrix(In.blendIndices.y) * In.blendWeights.y +
		RebuildSkinMatrix(In.blendIndices.z) * In.blendWeights.z;
	
  // Calculate local world matrix with help of the skinning matrix
	float4x4 localWorld = mul(skinMatrix,transpose(world));//mul(world, skinMatrix);
    

	//float4x4 localWorld = mul(skinMatrix,world);

//localWorld = world;
	//localWorld = tempSkinMatrix;
//localWorld = skinMatrix;
//localWorld = RebuildSkinMatrix(1);
    	// Now calculate final screen position with world and viewProj matrices.
	float4 worldPos = mul(localWorld, float4(In.pos, 1));
//worldPos = mul(float4(In.pos,1),localWorld);

	Out.pos = mul(worldPos, viewProj);
	//Out.pos = mul(float4(In.pos,1),mul(world,viewProj));

    //Out.pos = Out.pos / Out.pos.w;
	Out.texCoord = In.texCoord;
    
  	// Compute the 3x3 tranform from tangent space to object space
	float3x3 worldToTangentSpace =
		ComputeTangentMatrix(In.tangent, In.normal, localWorld);

	// Transform light and view vectors and pass them to the pixel shader
	//Out.lightVec = normalize(mul(worldToTangentSpace, lightDir));
	//Out.viewVec = mul(worldToTangentSpace, cameraPos - worldPos.xyz);

    
  //world matrix is transposed, invert order
	Out.normal = mul(localWorld,In.normal );
	// Eye pos
	float3 eyePos = cameraPos;
	// Eye vector
	float3 eyeVector = normalize(eyePos-worldPos);
	// Half vector
	Out.halfVec = normalize(eyeVector+lightDir);
	
    
    
    

	//Out.lightVec = Out.pos.xyz;
	// Rest of the calculation is done in pixel shader
	return Out;
} // VS_DiffuseSpecular20

float4 PS_SpecularPerPixelColored(VertexOutput_DiffuseSpecularColored20 In) : COLOR
{

	float3 normal = normalize(In.normal);
    //float3 halfVec = normalize(In.halfVec);

	float brightness = saturate(dot(normal, lightDir));

	float specular = pow(saturate(dot(normal, In.halfVec)), shininess);

	float4 Out = float4(0,0,0,0);
	Out += ambientColor;
	Out += (brightness * diffuseColor);
	Out += (specular * specularColor);
	return Out;

    
} // PS_SpecularPerPixel(In)




/**
 * Weights display mode!
 */
// Vertex output structure
 struct VertexOutput_DisplayWeights
{
	float4 pos      : POSITION;
	float2 texCoord	: TEXCOORD0;
	float3 lightVec : TEXCOORD1;
	float3 viewVec  : TEXCOORD2;
	float weightFactor : TEXCOORD3;
	
};

VertexOutput_DisplayWeights VS_DisplayWeights(VertexInput In)
{
	VertexOutput_DisplayWeights Out = (VertexOutput_DisplayWeights)0;      

	// First transform position with bones that affect this vertex
	// Use the 3 indices and blend weights we have precalculated.
	float4x4 skinMatrix =
  	RebuildSkinMatrix(In.blendIndices.x) * In.blendWeights.x +
		RebuildSkinMatrix(In.blendIndices.y) * In.blendWeights.y +
		RebuildSkinMatrix(In.blendIndices.z) * In.blendWeights.z;
	
    // Calculate local world matrix with help of the skinning matrix
	float4x4 localWorld = mul(skinMatrix,transpose(world));//mul(world, skinMatrix);
    
    // Now calculate final screen position with world and viewProj matrices.
	float4 worldPos = mul(localWorld, float4(In.pos, 1));
	Out.pos = mul(worldPos, viewProj);
	Out.texCoord = In.texCoord;
    
  	// Compute the 3x3 tranform from tangent space to object space
	float3x3 worldToTangentSpace =
		ComputeTangentMatrix(In.tangent, In.normal, localWorld);

	// Transform light and view vectors and pass them to the pixel shader
	Out.lightVec = normalize(mul(worldToTangentSpace, lightDir));
	Out.viewVec = mul(worldToTangentSpace, cameraPos - worldPos.xyz);

	// Trick: 
	//Note: indices are premultiplied by 3!!!
	/*Out.weightFactor = float3(
		(saturate(-abs( (In.blendIndices.x * (1/3.0f)) - (SelectedBoneIndex) )+1) ) * In.blendWeights.x ,
		(saturate(-abs( (In.blendIndices.y * (1/3.0f)) - (SelectedBoneIndex) )+1) ) * In.blendWeights.y ,
		(saturate(-abs( (In.blendIndices.z * (1/3.0f)) - (SelectedBoneIndex) )+1) ) * In.blendWeights.z );
	Out.weightFactor = float3(
		(saturate(-abs( (In.blendIndices.x * (1/3.0f)) - (SelectedBoneIndex) )+1) ) ,
		(saturate(-abs( (In.blendIndices.y * (1/3.0f)) - (SelectedBoneIndex) )+1) ) ,
		(saturate(-abs( (In.blendIndices.z * (1/3.0f)) - (SelectedBoneIndex) )+1) ));*/
	//Out.weightFactor = 1-abs( In.blendIndices - float3(SelectedBoneIndex*3,SelectedBoneIndex*3,SelectedBoneIndex*3));
	//Out.weightFactor = In.blendWeights;
	//Out.weightFactor = In.blendIndices;
	Out.weightFactor = 0;
	if (abs(In.blendIndices.x - SelectedBoneIndex * 3 ) < 0.001f)
		Out.weightFactor += In.blendWeights.x;
	if (abs(In.blendIndices.y - SelectedBoneIndex * 3 ) < 0.001f)
		Out.weightFactor += In.blendWeights.y;
	if (abs(In.blendIndices.z - SelectedBoneIndex * 3 ) < 0.001f)
		Out.weightFactor += In.blendWeights.z;		
	return Out;
} 

float4 PS_DisplayWeights(VertexOutput_DisplayWeights In) : COLOR
{
	float weight = In.weightFactor;
	//weight *= 0.5f;
	float3 gray = float3(0.5f,0.5f,0.5f);
	float3 yellow = float3(0.8f,0.8f,0);
	float3 red = float3(1,0,0);
	float fGray = 1-weight;
	float fYellow = 1 - abs(0.5-weight)*2;
	float fRed = weight;
	float3 factor = normalize(float3(fGray,fYellow,fRed));
	return float4(gray * factor.x + yellow*factor.y + red * factor.z,1);
	// Grab texture data
	float4 diffuseTexture = float4(weight * 0.5f,weight * 0.5f+0.5f,1-In.weightFactor,1);
	//float4 diffuseTexture = float4(In.weightFactor,1);
	return diffuseTexture;
	float3 normalVector = (2.0 * tex2D(normalTextureSampler, In.texCoord).agb) - 1.0;
	// Normalize normal to fix blocky errors
	normalVector = normalize(normalVector);

	// Additionally normalize the vectors
	float3 lightVector = In.lightVec;
	float3 viewVector = normalize(In.viewVec);

	// Compute the angle to the light
	float bump = saturate(dot(normalVector, lightVector));
	// Specular factor
	float3 reflect = normalize(2 * bump * normalVector - lightVector);
	float spec = pow(saturate(dot(reflect, viewVector)), shininess);

	float4 ambDiffColor = ambientColor + bump * diffuseColor;
diffuseTexture.rgb = (diffuseTexture.rgb-0.5f)*1.25f+0.5f;
	return diffuseTexture * (ambientColor +
		bump * (diffuseColor + spec * specularColor));
} 


technique DiffuseSpecularColored20
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_DiffuseSpecularColored20();
		PixelShader  = compile ps_2_0 PS_SpecularPerPixelColored();
	} 
} 

technique DiffuseSpecular20
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_DiffuseSpecular20();
		PixelShader  = compile ps_2_0 PS_DiffuseSpecular20();
	} // pass P0
} // technique DiffuseSpecular20

technique DisplayWeights
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_DisplayWeights();
		PixelShader  = compile ps_2_0 PS_DisplayWeights();
	} 
}

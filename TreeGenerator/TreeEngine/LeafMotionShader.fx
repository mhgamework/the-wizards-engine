

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;
shared float4x4 view : View;
shared float4x4 projection : Projection;

shared float4x4 viewInverse : ViewInverse;

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

// Vertex Output Declaration
struct VSOut
{
	float4 Position	: POSITION;
	float2 TexCoord	: TEXCOORD0;
	
};


struct VertexInput
{
	float3 position:POSITION0;
	float2 texCoords:TEXCOORD0;
	float3 normal:NORMAL0;
	float3 tangent:TANGENT0;
	float treeHeight:POINTSIZE0;
	float bendingCooficient:POINTSIZE1;
};
float3 windDirection;
float windStrength;
float time;
float frequency;
float waveSpeed;
float3 leafNormal;
float3 leafTangent;
float fBendScale;
float3 centre;
VSOut vs_main(VertexInput In)
{
	VSOut output;
	float fLength= dist(centre,pos);
	float4 pos=mul(float4(In.position,1),world);
  // float angleNormal=atan((windDirection.x-leafNormal.x)/(windDirection.x-leafNormal.x));
  //float angleTangent=atan((windDirection.x-leafTangent.x)/(windDirection.x-leafTangent.x));

   //pos.xyz+=leafNormal*windStrength*angleTangent*sin(frequency*(time-(1/waveSpeed)))*(1-In.texCoords.y)*(0,5-In.texCoords.x);
   //pos.xyz+=leafTangent*windStrength*angleNormal*sin(frequency*(time-(1/waveSpeed)))*(1-In.texCoords.y)*(0,5-In.texCoords.x);

      // Bend factor - Wind variation is done on the CPU.  
   float fBF = pos.z * fBendScale;  
	// Smooth bending factor and increase its nearby height limit.  
	fBF += 1.0;  
	fBF *= fBF;  
	fBF = fBF * fBF - fBF;  
	// Displace position  
	   float3 vNewPos = pos;  
	vNewPos.xy += windDirection.xy * fBF;  
	// Rescale  
	pos.xyz = normalize(vNewPos.xyz)* fLength;  

	output.Position=mul(pos,viewProjection);
	output.TexCoord=In.texCoords;
    return output;
}





// Pixel Shader
float4 ps_main(VSOut In) : COLOR0
{
	
	float4 Out;
	Out =tex2D(DiffuseTextureSampler, In.TexCoord);
	return Out;
}

// Technique
technique Billboard
{
    pass p0 
    {	
		// Shaders
		VertexShader = compile vs_3_0 vs_main();
		PixelShader  = compile ps_3_0 ps_main();
		
    }
}
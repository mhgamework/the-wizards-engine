

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;

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



//----------------------------------------------------



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
};
struct VertexTemp
{
	float3 pos:POSITION;
	float3 normal:NORMAL;
};
float frequency=1.0f;
float amplitude=1.0f;
float time=0;
float waveSpeed=1.0f;
float3 startPointWave=float3(0,0,0);
float3 WindDirection=float3(1,0,0);

VertexOutput VS_AnimatedCloud(VertexInput In)
{
	VertexOutput Out = (VertexOutput)0;
	VertexTemp temp;
	float3 translation=In.pos;
	float3 translation2=In.pos;
	

	if(In.texCoord.y<0.01)
	{
		float distX=startPointWave.x-In.pos.x;
		float distZ=startPointWave.z-In.pos.z;
		
		// this makes the grass move according to the wins direction
		float3 movement=amplitude*WindDirection*(sin(frequency*(time-(1/waveSpeed)))*0.2+1.2);
		translation=In.pos+movement*0.8;
        
		//this makes the grass move like there is a point from were all the wind commes from
		float dist=sqrt(distX*distX+distZ*distZ);
	   translation2.x=amplitude*sin(frequency*(time-(dist/waveSpeed)));
	   translation2.z=amplitude*sin(frequency*(time-(dist/waveSpeed)));
	   
	   translation += translation2 * 0.2f;
	   
		//float3 testpos=CalcTranslation(In.pos);
		//testpos.x += sin(testpos.x * frequency + (time/TimeScale)) * amplitude* 1-In.texCoord.y;
		//an failed attemd to down size the y to keep it stay the samelength
		//translation.y=dot(In.pos.y,movement.x);
		//translation.y=dot(translation.y,movement.z);
		
	 }
	temp.pos=translation;
	

    float4 pos=float4(temp.pos,1);
    pos = mul(pos, mul(world , viewProjection));
	Out.pos = pos;
	Out.texCoord = In.texCoord;
	Out.normal = mul(In.normal, world);    

	return Out;
} 

float4 PS_AnimatedCloud(VertexOutput In) : COLOR
{
	// Normalize after interpolation
	float3 vNormalWS = normalize(In.normal);

    	float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
        
    	//float3 vLightDirWS = normalize(-lightDir);
    
   
	return textureColor;
    		
	
} 


technique AnimatedCloud
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_AnimatedCloud();
		PixelShader = compile ps_3_0 PS_AnimatedCloud();
	} 
} 




//------- Technique: PointSprites --------
//by me
struct VertexInput
{
	float3 pos : POSITION;
	float Size:PSIZE;
};

struct SpritesVertexOut
{
    float4 Position: POSITION0;
    float Size : PSIZE;
};

struct SpritesPixelIn
{
    
		float2 TexCoord : TEXCOORD0;
	
};
struct PixelToFrame
{
    float4 Color : COLOR0;
};

SpritesVertexOut PointSpritesVS (VertexInput In)//float4 Position : POSITION, float4 Color : COLOR0, float Size : PSIZE)
{
    SpritesVertexOut Output = (SpritesVertexOut)0;
     
    //float4x4 preViewProjection = mul (viewInverse, viewProjection);
	//float4x4 preWorldViewProjection = mul (world, preViewProjection); 
    //Output.Position = mul(Position, preWorldViewProjection);
	Output.Position = mul(In.pos, mul(world , viewProjection));
    Output.Size = (1/pow(Output.Position.z,2)+1) * In.Size;
    
    return Output; 
}

PixelToFrame PointSpritesPS(SpritesPixelIn PSIn)
{ 
    PixelToFrame Output = (PixelToFrame)0;    

   
		float2 texCoord = PSIn.TexCoord.xy;
    

    Output.Color = tex2D(DiffuseTextureSampler, texCoord);
    
    return Output;
}

technique PointSprites_2_0
{
	pass Pass0
	{   
		PointSpriteEnable = true;
		VertexShader = compile vs_2_0 PointSpritesVS();
		PixelShader  = compile ps_2_0 PointSpritesPS();
	}
}

technique PointSprites
{
	pass Pass0
	{   
		PointSpriteEnable = true;
		VertexShader = compile vs_1_1 PointSpritesVS();
		PixelShader  = compile ps_1_1 PointSpritesPS();
	}
}





////radom chaos pointspride shader code////

texture particleTexture;
float4x4 WorldViewProj : WorldViewProjection;
struct PS_INPUT
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	
	float4 Color : COLOR0;
};
sampler Sampler = sampler_state
{
	Texture = <particleTexture>;
};
PS_INPUT VertexShaderChaos(float4 pos : POSITION0, float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
	PS_INPUT Output = (PS_INPUT)0;
	
	Output.Position = mul(pos, WorldViewProj);		
	
	Output.TexCoord = texCoord;
	Output.Color = color;

	return Output;
}
float4 PixelShaderChaos(PS_INPUT input) : COLOR0
{
	float2 texCoord;
	texCoord = input.TexCoord;
	float4 Color = tex2D(Sampler, texCoord);
	Color *= input.Color;
	return Color;	
}
technique PointSpriteTechnique
{
    pass P0
    {   
        vertexShader = compile vs_2_0 VertexShaderChaos();
        pixelShader = compile ps_2_0 PixelShaderChaos();
    }
}
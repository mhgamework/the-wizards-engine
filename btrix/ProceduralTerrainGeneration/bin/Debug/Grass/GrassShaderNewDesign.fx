

/*** Camera Info ***/

float4x4 world : World;
shared float4x4 viewProjection : ViewProjection;

/*shared float4x4 viewInverse : ViewInverse;*/



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
	float3 pos : POSITION0;
	float top:PSIZE0;
	float2 texCoord : TEXCOORD0;
	float3 normal : NORMAL0;
	float4 color:COLOR0;
};

struct VertexOutput
{
	float4 pos : POSITION0;
	float2 texCoord : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float4 color:COLOR0;
};
struct VertexTemp
{
	float3 pos:POSITION;
	float3 normal:NORMAL;
};
float frequency;
float amplitude;
float time;
float waveSpeed;
float3 startPointWave;
float3 WindDirection;

VertexOutput VS_AnimatedGrass(VertexInput In)
{
	VertexOutput Out = (VertexOutput)0;
	VertexTemp temp;
	float3 translation=In.pos;
	float3 translation2=float3(0,0,0);//In.pos;
	
	
		float distX=startPointWave.x-In.pos.x;
		float distZ=startPointWave.z-In.pos.z;
		
		// this makes the grass move according to the winds direction
		float3 movement=amplitude*WindDirection*(sin(frequency*(time-(1/waveSpeed)))*0.2+1.2);
		translation=In.pos+movement*0.8*In.top;
		
        
		//this makes the grass move like there is a point from were all the wind commes from
		float dist=sqrt(distX*distX+distZ*distZ);
	   translation2.x=amplitude*sin(frequency*(time-(dist/waveSpeed)));
	   translation2.z=amplitude*sin(frequency*(time-(dist/waveSpeed)));
	   
	   translation += translation2 * 0.2f*In.top;
	   
		
		
	
	temp.pos=translation;
	

    float4 pos=float4(temp.pos,1);
    pos = mul(pos, mul(world , viewProjection));
	Out.pos = pos;
	Out.texCoord = In.texCoord;
	Out.normal = mul(In.normal, world);    
	Out.color=In.color;
	return Out;
} 

float4 PS_AnimatedGrass(VertexOutput In) : COLOR
{
	// Normalize after interpolation
	float3 vNormalWS = normalize(In.normal);

    	float4 textureColor = tex2D(DiffuseTextureSampler, In.texCoord);
    	
        float3 color =lerp(textureColor.xyz,In.color.xyz,0.0);
    	//float3 vLightDirWS = normalize(-lightDir);
    
	 //return textureColor;
	return float4(color,textureColor.w);//textureColor;
    		
	
} 

 /*float3 CalcTranslation(float3 In)
{
	float3 translation;
	translation.x=In.x+amplitude*sin(frequency*(time-(In.x/waveSpeed)));
	
	
	
	return translation;
}*/
/* attempt to make a golf simulation using math

sin 0-->1
	  0?-->90?
	  
	  y(x,t)=y0*sin(w(t-(x/c)))
	  
	 y is the displacement of the point on the traveling sound wave;
	 x is the distance the point has traveled from the wave's source;
	 t is the time elapsed;
	 y0 is the amplitude of the oscillations,
	 c is the speed of the wave; and
	 w is the angular frequency of the wave.
			
*/
technique AnimatedGrass
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS_AnimatedGrass();
		PixelShader = compile ps_3_0 PS_AnimatedGrass();
	} 
} 











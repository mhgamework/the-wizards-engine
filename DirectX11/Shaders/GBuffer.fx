//
// This shader include contains the GBuffer format implementation
//

struct GBuffer
{
	//NOTE: if i got it correctly, half is not supported anymore in D3D10
    float4 Color : SV_TARGET0;
    float4 Normal : SV_TARGET1;    
};
struct GBuffer_Raw
{
	float3 Diffuse;
	float3 Normal;
	float SpecularIntensity;
	float SpecularPower;
	float Depth;
};
// diffuse color, and specularIntensity in the alpha channel
Texture2D GBuffer_Color;
// normals, and specularPower in the alpha channel
Texture2D GBuffer_Normal;
//depth
Texture2D GBuffer_Depth;



float3 CompressNormal(float3 value)
{
	return 0.5f * (normalize(value) + 1.0f); //Warning: should already be normalized, possible optimization
}
float3 DecompressNormal(float3 value)
{
	return 2.0f * value - 1.0f;
}
GBuffer CreateGBuffer(float3 diffuse, float3 normal, float specularIntensity, float specularPower)
{
	GBuffer ret;
	ret.Color.rgb = diffuse;
	ret.Color.a = specularIntensity;
	ret.Normal.rgb = CompressNormal(normal);
	ret.Normal.a = specularPower;

	return ret;
}
GBuffer_Raw SampleGBuffer(SamplerState s, float2 texcoord)
{
	float4 color = GBuffer_Color.Sample(s,texcoord);
	float4 normal = GBuffer_Normal.Sample(s,texcoord);
	float depth = GBuffer_Depth.Sample(s,texcoord).x;

	GBuffer_Raw ret;
	ret.Diffuse = color.rgb;
	ret.SpecularIntensity = color.a;
	ret.Normal = DecompressNormal( normal.rgb );
	ret.SpecularPower = normal.a;
	ret.Depth = depth;

	return ret;

}
#include <Deferred/DeferredMesh.fx>


GBuffer VoxelSurfacePS(VertexShaderOutput input)
{

	

	float4 diffuseAlbedo;
	float specularAlbedo;
	float3 normalWS;

//#ifdef DIFFUSE_MAPPING
	//diffuseAlbedo = txDiffuse.Sample(Sampler, input.PositionWS.xz);
//#else
	//diffuseAlbedo = diffuseColor;
//#endif

	// clip when alpha is below 95 %
	//clip(diffuseAlbedo.a - 0.95f);

//#ifdef SPECULAR_MAPPING
	//specularAlbedo = txSpecular.Sample(Sampler, input.TexCoord).r;
//#else
	specularAlbedo = specularIntensity;
//#endif

//#ifdef NORMAL_MAPPING
	/*// Normalize the tangent frame after interpolation
	float3x3 tangentFrameWS = float3x3(normalize(input.TangentWS), normalize(input.BitangentWS), normalize(input.NormalWS));

		// Sample the tangent-space normal map and decompress
		float3 normalTS = txNormal.Sample(Sampler, input.TexCoord).rgb;
		normalTS = normalize(normalTS * 2.0f - 1.0f);

	normalWS = mul(normalTS, tangentFrameWS);*/
//#else
	normalWS = normalize(input.NormalWS);
//#endif



	// Originally from GPUgems3 chapter 1

	float tex_scale = -0.5;
	// Determine the blend weights for the 3 planar projections.  
	// N_orig is the vertex-interpolated normal vector.  
	float3 blend_weights = abs(input.NormalWS.xyz);   // Tighten up the blending zone:  
		blend_weights = (blend_weights - 0.2) * 7;
	blend_weights = max(blend_weights, 0);      // Force weights to sum to 1.0 (very important!)  
	blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z).xxx;
	// Now determine a color value and bump vector for each of the 3  
	// projections, blend them, and store blended results in these two  
	// vectors:  
	float4 blended_color; // .w hold spec value  
	/*float3 blended_bump_vec;
	{*/
		// Compute the UV coords for each of the 3 planar projections.  
		// tex_scale (default ~ 1.0) determines how big the textures appear.  
	float2 coord1 = input.PositionWS.yz * tex_scale;
		float2 coord2 = input.PositionWS.zx * tex_scale;
		float2 coord3 = input.PositionWS.xy * tex_scale;
			// This is where you would apply conditional displacement mapping.  
			//if (blend_weights.x > 0) coord1 = . . .  
			//if (blend_weights.y > 0) coord2 = . . .  
			//if (blend_weights.z > 0) coord3 = . . .  
			// Sample color maps for each projection, at those UV coords.  
			float4 col1 = txDiffuse.Sample(Sampler, coord1);
			float4 col2 = txDiffuse.Sample(Sampler, coord2);
			float4 col3 = txDiffuse.Sample(Sampler, coord3);
			// Sample bump maps too, and generate bump vectors.  
			// (Note: this uses an oversimplified tangent basis.)  
			/*float2 bumpFetch1 = bumpTex1.Sample(coord1).xy - 0.5;
			float2 bumpFetch2 = bumpTex2.Sample(coord2).xy - 0.5;
			float2 bumpFetch3 = bumpTex3.Sample(coord3).xy - 0.5;
			float3 bump1 = float3(0, bumpFetch1.x, bumpFetch1.y);
			float3 bump2 = float3(bumpFetch2.y, 0, bumpFetch2.x);
			float3 bump3 = float3(bumpFetch3.x, bumpFetch3.y, 0);*/
			// Finally, blend the results of the 3 planar projections.  
			blended_color = col1.xyzw * blend_weights.xxxx +
			col2.xyzw * blend_weights.yyyy +
			col3.xyzw * blend_weights.zzzz;
		/*blended_bump_vec = bump1.xyz * blend_weights.xxx +
			bump2.xyz * blend_weights.yyy +
			bump3.xyz * blend_weights.zzz;
	}*/
	// Apply bump vector to vertex-interpolated normal vector.  
	//float3 N_for_lighting = normalize(N_orig + blended_bump);


		diffuseAlbedo = blended_color;








	return CreateGBuffer(diffuseAlbedo, normalWS, specularAlbedo, specularPower);
}



technique10 DCSurface
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader(CompileShader(ps_4_0, VoxelSurfacePS()));
    }

}

//
// InvertColorCS.hlsl
//
// Copyright (C) 2010  Jason Zink 

// Declare the input and output resources
Texture3D<float>        PerlinNoise : register( t0 );           
//RWTexture3D<float4>        OutputMap : register( u0 );
RWTexture3D<float4>        OutputMap : register( u0 );
RWTexture3D<float4>        IntersectionsMap : register( u1 );
RWTexture3D<float4>        NormalsMap1 : register( u2 );
RWTexture3D<float4>        NormalsMap2 : register( u3 );
RWTexture3D<float4>        NormalsMap3 : register( u4 );
SamplerState TrilinearRepeat : register (s0);/*{   
											 Filter = 
											 AddressU = Mirror;
											 AddressV = Mirror;
											 AddressW = Mirror;
											 };*/


// Image sizes
#define size_x 8
#define size_y 8
#define size_z 8

#define fullSize 64
#define noiseSize 64.0
#define noiseTexel 1/noiseSize

// Coord is in the 0,1 space (with wrapping)
float sampleNoise(float3 coord)
{
	//return PerlinNoise.SampleLevel(TrilinearRepeat,coord,0);
	//return f.x;
	//Do manual trilerp since for some reason HLSL wont give me more than 8-bit unorm precision out of the SampleLevel functions
	//TODO: checkout the source code for the GPUgems 3 sample, because they do this too
	//The reason this is important is for the normal calculation, but maybe normal calculation can be done by looking at gridpoints instead or some other technique?
	//TODO: for optimizing, only use this complex method for normal calculation, and use normal trilinear interp for signs and intersects

	float3 f = frac(coord*noiseSize);
	

	float q000 = PerlinNoise.SampleLevel(TrilinearRepeat,coord,0);
	float q001 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(0,0,1)*noiseTexel,0);
	float q010 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(0,1,0)*noiseTexel,0);
	float q011 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(0,1,1)*noiseTexel,0);
	float q100 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(1,0,0)*noiseTexel,0);
	float q101 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(1,0,1)*noiseTexel,0);
	float q110 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(1,1,0)*noiseTexel,0);
	float q111 = PerlinNoise.SampleLevel(TrilinearRepeat,coord + float3(1,1,1)*noiseTexel,0);


	float x00 = lerp(q000,q100,f.x);
	float x01 = lerp(q001,q101,f.x);
	float x10 = lerp(q010,q110,f.x);
	float x11 = lerp(q011,q111,f.x);
	float y0 = lerp(x00,x10,f.y);
	float y1 = lerp(x01,x11,f.y);
	return lerp(y0,y1,f.z);

	//coord = frac(coord);
	//return (coord.x*2-1+coord.z*2-1)/2.0;
	//return (sin(coord.x*10) + sin(coord.z*10))/2.0;

	// This for some reason returns 8-bit unorm instead of R32float, so we have serious problems with normals)
	return (PerlinNoise.SampleLevel(TrilinearRepeat, coord,0).x-0.5)*2.0;
}

float getDensity(float3 ws)
{
	//ws /= 64.0; //0-1
	//return sampleNoise(ws) ;
	float density;
	//ws *= 0.25f;
	//ws.y *=0.05;

	//ws -= uint3(fullSize,fullSize,fullSize)/2;
	//density = fullSize*0.4f - sqrt(ws.x*ws.x + ws.y*ws.y + ws.z*ws.z);
	//density = fullSize*0.4f - abs(ws.x)-abs(ws.y) -abs(ws.z);
	//return density;


	float scale =1.0f / 64;
	float3 samplePos = ws*scale*0.05f + float3(0.3f,0.3f,0.3f);

		float multiplier = 20;


	//density = 50.0-ws.y;
	density = 30.4-ws.y;

	/*float q000 = 1;
	float q001 = -1;
	float q010 = -1;
	float q011 = -1;
	float q100 = -1;
	float q101 = -1;
	float q110 = -1;
	float q111 = -1;

	ws /= 64.0;
	float x00 = lerp(q000,q100,ws.x);
	float x01 = lerp(q001,q101,ws.x);
	float x10 = lerp(q010,q110,ws.x);
	float x11 = lerp(q011,q111,ws.x);
	float y0 = lerp(x00,x10,ws.y);
	float y1 = lerp(x01,x11,ws.y);
	density = lerp(y0,y1,ws.z);
	return density;*/
	//density += sampleNoise( (ws+float3(0.5f,0.5f,0.5f))/64.0/4.0);  
	//density += sampleNoise( (ws+float3(0.5f,0.5f,0.5f))/64.0/4.0/2.1)*2;  
	//density += sampleNoise( (ws+float3(0.5f,0.5f,0.5f))/64.0/4.0/3.9)*4;  
	//density += sampleNoise( (ws+float3(0.5f,0.5f,0.5f))/64.0/4.0/8.05)*8;  
	//density /= 10000.0;
	//density += sin(ws.x/2)+sin(ws.z/2);
	//return density;
	//density += sin(DispatchThreadID.x);
	//density += sin(DispatchThreadID.z);
	//density = density + PerlinNoise.SampleLevel(TrilinearRepeat, samplePos,0).x * 3;  

	density += sampleNoise( samplePos*4.03f)*0.25f * multiplier;  
	density += sampleNoise(samplePos*1.96f)*0.50f * multiplier;  
	density += sampleNoise( samplePos*1.01)*1.00 * multiplier;  
	density += sampleNoise( samplePos*0.49)*2.00 * multiplier;  
	density += sampleNoise( samplePos*0.26)*4.00 * multiplier;  
	//density += sampleNoise( samplePos*0.120)*8.00 * multiplier;  
	//density += sampleNoise( samplePos*0.061)*16.00 * multiplier;  

	//density += sampleNoise( ws *scale*0.05f)*20.0;
	//density += sampleNoise( ws *scale)*5.0;


	/*density = density + PerlinNoise.SampleLevel(TrilinearRepeat, samplePos*(1.0f/15),0).x * 20; 
	density = density + PerlinNoise.SampleLevel(TrilinearRepeat, samplePos*(1.0f/55),0).x * 50; 
	density = density + PerlinNoise.SampleLevel(TrilinearRepeat, samplePos*(1.0f/90),0).x * 80; 
	density = density + PerlinNoise.SampleLevel(TrilinearRepeat, samplePos*(1.0f/200),0).x * 200; */

	return density;
}
#define diff  0.1
float3 calculateNormal(float3 pos)
{
	float3 n;
	float zero = getDensity(pos);
	// Note: not real dX, just increative naming
	float dX = getDensity(pos+float3(diff,0,0));
	float dY = getDensity(pos+float3(0,diff,0));
	float dZ = getDensity(pos+float3(0,0,diff));
	n.x = getDensity(pos+float3(diff,0,0)) - getDensity(pos-float3(diff,0,0)); // no divide since we normalize
	n.y = getDensity(pos+float3(0,diff,0)) - getDensity(pos-float3(0,diff,0));
	n.z = getDensity(pos+float3(0,0,diff)) - getDensity(pos-float3(0,0,diff));
	//n.x = (dX - zero)/diff;
	//n.y = (dY - zero)/diff;
	//n.z = (dZ - zero)/diff;
	//n.x = -n.x;
	//n = float3(zero>0?1:-1,dX>0?1:-1,dZ>0?1:-1);
	//n = float3(zero>0?1:-1,0,0);
	//return float3(0,1,0);
	//n.y = 1;
	return normalize(-n);
}

float helperFuncTestNoiseSamplingResolution(uint3 ws)
{
	
	// When having low precision, this shows banding
	float part = 16;
	float result = 1;
	//float sample = PerlinNoiseNew.SampleLevel(TrilinearRepeat, float3(ws.x/64.0/4/part+0.25,0+0.25,0+0.25),0).r;
	float sample = sampleNoise(float3(ws.x,0,0)/fullSize/part); // Sample noise, but select only an 1/part piece
	return (sample-((part-1)/part))*part*0.5+0.5;// Scale the sample value back up by 'part' so that it has same intensity when making smaller
}

[numthreads(size_x, size_y, size_z)]
void CSGridSigns( uint3 GroupID : SV_GroupID, uint3 DispatchThreadID : SV_DispatchThreadID, uint3 GroupThreadID : SV_GroupThreadID, uint GroupIndex : SV_GroupIndex )
{
	uint3 ws = uint3(DispatchThreadID.x,fullSize -1 -DispatchThreadID.y,DispatchThreadID.z);
	float density = getDensity(ws);

	float result = density>0 ? 0 : 1;
	//result = helperFuncTestNoiseSamplingResolution(ws);
	OutputMap[DispatchThreadID] = float4(result,result,result,1);
}



float linearZero(float y1, float y2)
{
	return -y1/(y2-y1);
}

[numthreads(size_x, size_y, size_z)]
void CSCalcIntersections( uint3 GroupID : SV_GroupID, uint3 DispatchThreadID : SV_DispatchThreadID, uint3 GroupThreadID : SV_GroupThreadID, uint GroupIndex : SV_GroupIndex )
{
	uint3 cell = uint3(DispatchThreadID.x,fullSize -1 -DispatchThreadID.y,DispatchThreadID.z);
		float3 intersects;

	/*float density = getDensity(cell);

	//density = 20.0-ws.y;
	//OutputMap[DispatchThreadID] = 

	float result = 1;
	if (density > 0) result = 0;
	OutputMap[DispatchThreadID] = float4(result,result,result,1);*/

	float densities[4] = {getDensity(cell),getDensity(cell + uint3(1,0,0)),getDensity(cell + uint3(0,1,0)),getDensity(cell + uint3(0,0,1)) };

	// Clamp could be removed since these edges should not be used by surface extraction
	intersects.x = saturate(linearZero(densities[0],densities[1]));
	intersects.y = saturate(linearZero(densities[0],densities[2]));
	intersects.z = saturate(linearZero(densities[0],densities[3]));

	IntersectionsMap[DispatchThreadID] = float4(intersects,1);

	float3 normalX = calculateNormal(lerp(cell,cell + uint3(1,0,0),intersects.x));
		float3 normalY =calculateNormal(lerp(cell,cell + uint3(0,1,0),intersects.y));
		float3 normalZ =calculateNormal(lerp(cell,cell + uint3(0,0,1),intersects.z));

		// Packing code, might be usefull later
		/*float4 normals1;
		float4 normals2;
		normals1.xy = normalX.xy*0.5f+0.5f;
		normals1.zw = normalY.xy*0.5f+0.5f;
		normals2.xy = normalZ.xy*0.5f+0.5f;
		normals2.z = (normalX.z>0 ? 1/255.0:0) + (normalY.z>0 ? 2/255.0:0) + (normalZ.z>0 ? 4/255.0:0); //TODO this might be a problem due to floating precision?
		normals2.w = 1;

		NormalsMap1[DispatchThreadID] = normals1;
		NormalsMap2[DispatchThreadID] = normals2;*/


		//normalX.x = 1;


		NormalsMap1[DispatchThreadID] = float4(normalX*0.5f+0.5f,1);
	NormalsMap2[DispatchThreadID] = float4(normalY*0.5f+0.5f,1);
	NormalsMap3[DispatchThreadID] = float4(normalZ*0.5f+0.5f,1);

	//NormalsMap1[DispatchThreadID] =float4(1,0,0,1);





	/*float3 n;
	float zero = getDensity(pos);
	// Note: not real dX, just increative naming
	float dX = getDensity(pos+float3(diff,0,0));
	float dY = getDensity(pos+float3(0,diff,0));
	float dZ = getDensity(pos+float3(0,0,diff));
	n.x = getDensity(pos+float3(diff,0,0)) - getDensity(pos-float3(diff,0,0)); // no divide since we normalize
	n.y = getDensity(pos+float3(0,diff,0)) - getDensity(pos-float3(0,diff,0));
	n.z = getDensity(pos+float3(0,0,diff)) - getDensity(pos-float3(0,0,diff));
	//n.x = (dX - zero)/diff;
	//n.y = (dY - zero)/diff;
	//n.z = (dZ - zero)/diff;
	//n.x = -n.x;
	//n = float3(zero>0?1:-1,dX>0?1:-1,dZ>0?1:-1);
	//n = float3(zero>0?1:-1,0,0);
	//return float3(0,1,0);
	NormalsMap1[DispatchThreadID] = float4(normalX*0.5f+0.5f,1);
	*/



}


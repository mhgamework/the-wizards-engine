//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using System.Collections;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxCookingParams
	{
		public NxPlatform	targetPlatform;			//!< Target platform
		public float		skinWidth;				//!< Skin width for convexes
		public bool			hintCollisionSpeed;		//!< Favorize speed or memory for collision structures

		public NxCookingParams(NxPlatform targetPlatform,float skinWidth,bool hintCollisionSpeed)
		{
			this.targetPlatform=targetPlatform;
			this.skinWidth=skinWidth;
			this.hintCollisionSpeed=hintCollisionSpeed;
		}
	};


	public class NxCooking
	{
		static public bool InitCooking()
			{return wrapper_Cooking_initCooking();}

		static public void CloseCooking()
			{wrapper_Cooking_closeCooking();}


		static public bool CookConvexMesh(NxConvexMeshDesc c,NxStream stream)
			{return wrapper_Cooking_cookConvexMeshByParameters(c.numVertices,c.numTriangles,c.pointStrideBytes,c.triangleStrideBytes,c.pointsPtr,c.trianglesPtr,c.flags,stream.NxStreamPtr);}

		static public bool CookTriangleMesh(NxTriangleMeshDesc t,NxStream stream)
		{
			if(t.pmap!=null)
				{return wrapper_Cooking_cookTriangleMeshByParameters(t.numVertices,t.numTriangles,t.pointStrideBytes,t.triangleStrideBytes,t.pointsPtr,t.trianglesPtr,t.flags,t.materialIndexStride,t.materialIndicesPtr,t.heightFieldVerticalAxis,t.heightFieldVerticalExtent,t.pmap.dataSize,t.pmap.dataPtr,t.convexEdgeThreshold,stream.NxStreamPtr);}
			return wrapper_Cooking_cookTriangleMeshByParameters(t.numVertices,t.numTriangles,t.pointStrideBytes,t.triangleStrideBytes,t.pointsPtr,t.trianglesPtr,t.flags,t.materialIndexStride,t.materialIndicesPtr,t.heightFieldVerticalAxis,t.heightFieldVerticalExtent,0,IntPtr.Zero,t.convexEdgeThreshold,stream.NxStreamPtr);
		}

		static public bool CookClothMesh(NxClothMeshDesc c,NxStream stream)
			{return wrapper_Cooking_cookClothMeshByParameters(c.numVertices,c.numTriangles,c.pointStrideBytes,c.triangleStrideBytes,c.pointsPtr,c.trianglesPtr,c.flags,c.target,stream.NxStreamPtr);}

		static public bool setCookingParams(NxCookingParams cookingParams)
			{return wrapper_Cooking_setCookingParams(cookingParams);}

		static public NxCookingParams getCookingParams()
		{
			NxCookingParams cookingParams=new NxCookingParams(NxPlatform.PLATFORM_PC,0,false);
			wrapper_Cooking_getCookingParams(cookingParams);
			return cookingParams;
		}

		static public bool platformMismatch()
			{return wrapper_Cooking_platformMismatch();}

		static public void reportCooking()
			{wrapper_Cooking_reportCooking();}

		static public uint ComputeFluidHardwareMeshCRC32(float restParticlesPerMeter,float kernelRadiusMultiplier,float motionLimitMultiplier,uint packetSizeMultiplier,ArrayList triangleMeshShapeList)
		{
			int numShapes=triangleMeshShapeList.Count;
			IntPtr[] shapesPtrs=new IntPtr[numShapes];
			for(int i=0;i<numShapes;i++)
				{shapesPtrs[i]=((NxTriangleMeshShape)triangleMeshShapeList[i]).NxShapePtr;}
			return wrapper_Cooking_ComputeFluidHardwareMeshCRC32(restParticlesPerMeter,kernelRadiusMultiplier,motionLimitMultiplier,packetSizeMultiplier,numShapes,shapesPtrs);
		}

		static public bool CookFluidHardwareMesh(float restParticlesPerMeter,float kernelRadiusMultiplier,float motionLimitMultiplier,uint packetSizeMultiplier,ArrayList triangleMeshShapeList,NxStream stream)
		{
			int numShapes=triangleMeshShapeList.Count;
			IntPtr[] shapesPtrs=new IntPtr[numShapes];
			for(int i=0;i<numShapes;i++)
				{shapesPtrs[i]=((NxTriangleMeshShape)triangleMeshShapeList[i]).NxShapePtr;}
			return wrapper_Cooking_CookFluidHardwareMesh(restParticlesPerMeter,kernelRadiusMultiplier,motionLimitMultiplier,packetSizeMultiplier,numShapes,shapesPtrs,stream.NxStreamPtr);
		}

		static public bool CookFluidHardwareMesh(float restParticlesPerMeter,float kernelRadiusMultiplier,float motionLimitMultiplier,uint packetSizeMultiplier,NxTriangle[] triangleArray,NxStream stream)
			{return wrapper_CookFluidHardwareTriangles(restParticlesPerMeter,kernelRadiusMultiplier,motionLimitMultiplier,packetSizeMultiplier,triangleArray,(uint)triangleArray.Length,stream.NxStreamPtr);}





		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_initCooking();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cooking_closeCooking();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_cookConvexMeshByParameters(int numVertices,int numTriangles,int pointStrideBytes,int triangleStrideBytes,IntPtr pointsPtr,IntPtr trianglesPtr,uint flags,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_cookTriangleMeshByParameters(int numVertices,int numTriangles,int pointStrideBytes,int triangleStrideBytes,IntPtr pointsPtr,IntPtr trianglesPtr,uint flags,int materialIndexStride,IntPtr materialIndicesPtr,NxHeightFieldAxis heightFieldVerticalAxis,float heightFieldVerticalExtent,uint pmapDataSize,IntPtr pmapData,float convexEdgeThreshold,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_cookClothMeshByParameters(int numVertices,int numTriangles,int pointStrideBytes,int triangleStrideBytes,IntPtr pointsPtr,IntPtr trianglesPtr,uint flags,NxClothMeshTarget target,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_setCookingParams(NxCookingParams cookingParams);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cooking_getCookingParams(NxCookingParams cookingParams);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_platformMismatch();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cooking_reportCooking();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Cooking_ComputeFluidHardwareMeshCRC32(float restParticlesPerMeter,float kernelRadiusMultiplier,float motionLimitMultiplier,uint packetSizeMultiplier,int numShapes,IntPtr[] triangleMeshShapePtrArray);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cooking_CookFluidHardwareMesh(float restParticlesPerMeter,float kernelRadiusMultiplier,float motionLimitMultiplier,uint packetSizeMultiplier,int numShapes,IntPtr[] triangleMeshShapePtrArray,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_CookFluidHardwareTriangles(float restParticlesPerMeter,float kernelRadiusMultiplier,float motionLimitMultiplier,uint packetSizeMultiplier,NxTriangle[] triangleArray,uint tcount,IntPtr streamPtr);
	}
}


/*
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxSetCookingParams(const NxCookingParams& params);
-	NX_C_EXPORT NXC_DLL_EXPORT const NxCookingParams& NxGetCookingParams();
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxPlatformMismatch();
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxInitCooking(NxUserAllocator* allocator = NULL, NxUserOutputStream* outputStream = NULL);
-	NX_C_EXPORT NXC_DLL_EXPORT void NxCloseCooking();
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxCookTriangleMesh(const NxTriangleMeshDesc& desc, NxStream& stream);
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxCookConvexMesh(const NxConvexMeshDesc& desc, NxStream& stream);
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxCookFluidHardwareMesh(NxReal restParticlesPerMeter, NxReal kernelRadiusMultiplier, NxReal motionLimitMultiplier, NxU32 packetSizeMultiplier, NxArray<NxTriangleMeshShape*>& descArray, NxStream& stream);
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxCookFluidHardwareTriangles(NxReal restParticlesPerMeter, NxReal kernelRadiusMultiplier, NxReal motionLimitMultiplier, NxU32 packetSizeMultiplier,const NxTriangle *descArray,NxU32 tcount,NxStream& stream);
-	NX_C_EXPORT NXC_DLL_EXPORT NxU32 NxComputeFluidHardwareMeshCRC32(NxReal restParticlesPerMeter, NxReal kernelRadiusMultiplier, NxReal motionLimitMultiplier, NxU32 packetSizeMultiplier, NxArray<NxTriangleMeshShape*>& descArray);
-	NX_C_EXPORT NXC_DLL_EXPORT void NxReportCooking();
-	NX_C_EXPORT NXC_DLL_EXPORT bool NxCookClothMesh(const NxClothMeshDesc& desc, NxStream& stream);
*/


//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxTriangleMesh
	{
		private IntPtr nxTriangleMeshPtr;
		
		static public NxTriangleMesh createFromPointer(IntPtr triangleMeshPtr)
		{
			if(triangleMeshPtr==IntPtr.Zero)
				{return null;}
			return new NxTriangleMesh(triangleMeshPtr);
		}
		
		public NxTriangleMesh(IntPtr triangleMeshPtr)
			{nxTriangleMeshPtr=triangleMeshPtr;}
		
		public IntPtr NxTriangleMeshPtr
			{get{return nxTriangleMeshPtr;}}






		virtual public bool saveToDesc(NxTriangleMeshDesc t)
		{
			NxPMap pmap=new NxPMap();
			bool result=wrapper_TriangleMesh_saveToDesc(nxTriangleMeshPtr,ref t.numVertices,ref t.numTriangles,ref t.pointStrideBytes,ref t.triangleStrideBytes,ref t.pointsPtr,ref t.trianglesPtr,ref t.flags,ref t.materialIndexStride,ref t.materialIndicesPtr,ref t.heightFieldVerticalAxis,ref t.heightFieldVerticalExtent,ref pmap.dataSize,ref pmap.dataPtr,ref t.convexEdgeThreshold);
			if(pmap.dataSize!=0 && pmap.dataPtr!=IntPtr.Zero)
				{t.pmap=pmap;}
			return result;
		}

		virtual public NxTriangleMeshDesc getTriangleMeshDesc()
		{
			NxTriangleMeshDesc meshDesc=NxTriangleMeshDesc.Default;
			saveToDesc(meshDesc);
			return meshDesc;
		}
		
		virtual public uint getSubmeshCount()
			{return wrapper_TriangleMesh_getSubmeshCount(nxTriangleMeshPtr);}

		virtual public uint getCount(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_TriangleMesh_getCount(nxTriangleMeshPtr,subMeshIndex,arrayType);}

		virtual public NxInternalFormat getFormat(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_TriangleMesh_getFormat(nxTriangleMeshPtr,subMeshIndex,arrayType);}

		virtual public IntPtr getBase(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_TriangleMesh_getBase(nxTriangleMeshPtr,subMeshIndex,arrayType);}

		virtual public uint getStride(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_TriangleMesh_getStride(nxTriangleMeshPtr,subMeshIndex,arrayType);}

		virtual public bool load(NxStream stream)
			{return wrapper_TriangleMesh_load(nxTriangleMeshPtr,stream.NxStreamPtr);}

		


		virtual public bool loadPMap(NxPMap pmap)
			{return wrapper_TriangleMesh_loadPMap(nxTriangleMeshPtr,pmap);}

		virtual public bool hasPMap()
			{return wrapper_TriangleMesh_hasPMap(nxTriangleMeshPtr);}
			
		virtual public uint getPMapSize()
			{return wrapper_TriangleMesh_getPMapSize(nxTriangleMeshPtr);}

		virtual public bool getPMapData(NxPMap pmap)
			{return wrapper_TriangleMesh_getPMapData(nxTriangleMeshPtr,pmap);}

		virtual public uint getPMapDensity(NxPMap pmap)
			{return wrapper_TriangleMesh_getPMapDensity(nxTriangleMeshPtr);}

		virtual public uint getPageCount()
			{return wrapper_TriangleMesh_getPageCount(nxTriangleMeshPtr);}
			
		virtual public NxBounds3 getPageBBox(uint pageIndex)
		{
			NxBounds3 boundingBox=new NxBounds3();
			wrapper_TriangleMesh_getPageBBox(nxTriangleMeshPtr,boundingBox,pageIndex);
			return boundingBox;
		}
	
	
	
		virtual public Vector3[] getPoints()
			{return getTriangleMeshDesc().getPoints();}
		
		virtual public int[] getTriangleIndices()
			{return getTriangleMeshDesc().getTriangleIndices();}
		
		virtual public Vector3[] getTrianglesAsVertexTriplets()
			{return getTriangleMeshDesc().getTrianglesAsVertexTriplets();}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMesh_saveToDesc(IntPtr triangleMesh,ref int numVertices,ref int numTriangles,ref int pointStrideBytes,ref int triangleStrideBytes,ref IntPtr pointsPtr,ref IntPtr trianglesPtr,ref uint flags,ref int materialIndexStride,ref IntPtr materialIndicesPtr,ref NxHeightFieldAxis heightFieldVerticalAxis,ref float heightFieldVerticalExtent,ref uint pmapDataSize,ref IntPtr pmapDataPtr,ref float convexEdgeThreshold);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMesh_loadPMap(IntPtr triangleMesh,NxPMap pmap);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMesh_hasPMap(IntPtr triangleMesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMesh_getPMapSize(IntPtr triangleMesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMesh_getPMapData(IntPtr triangleMesh,NxPMap pmap);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMesh_getPMapDensity(IntPtr triangleMesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMesh_getSubmeshCount(IntPtr triangleMesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMesh_getCount(IntPtr triangleMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxInternalFormat wrapper_TriangleMesh_getFormat(IntPtr triangleMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_TriangleMesh_getBase(IntPtr triangleMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMesh_getStride(IntPtr triangleMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMesh_load(IntPtr triangleMesh,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMesh_getPageCount(IntPtr triangleMesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_TriangleMesh_getPageBBox(IntPtr triangleMesh,NxBounds3 boundingBox,uint pageIndex);
	}
}




/*
-	virtual	bool				saveToDesc(NxTriangleMeshDesc&)	const	= 0;
-	virtual NxU32				getSubmeshCount()							const	= 0;
-	virtual NxU32				getCount(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual NxInternalFormat	getFormat(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual const void*			getBase(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual NxU32				getStride(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual	bool				loadPMap(const NxPMap&) = 0;
-	virtual	bool				hasPMap()					const	= 0;
-	virtual	NxU32				getPMapSize()				const	= 0;
-	virtual	bool				getPMapData(NxPMap& pmap)	const	= 0;
-	virtual	NxU32				getPMapDensity()			const	= 0;
-	virtual	bool				load(const NxStream& stream)		= 0;
-	virtual NxU32				getPageCount() const = 0;
-	virtual NxBounds3			getPageBBox(NxU32 pageIndex) const = 0;
*/





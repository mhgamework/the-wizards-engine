//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxConvexMesh
	{
		private IntPtr nxConvexMeshPtr;
		
		static public NxConvexMesh createFromPointer(IntPtr convexMeshPtr)
		{
			if(convexMeshPtr==IntPtr.Zero)
				{return null;}
			return new NxConvexMesh(convexMeshPtr);
		}
		
		public NxConvexMesh(IntPtr convexMeshPtr)
			{nxConvexMeshPtr=convexMeshPtr;}
		
		public IntPtr NxConvexMeshPtr
			{get{return nxConvexMeshPtr;}}






		virtual public bool saveToDesc(NxConvexMeshDesc c)
			{return wrapper_ConvexMesh_saveToDesc(nxConvexMeshPtr,ref c.numVertices,ref c.numTriangles,ref c.pointStrideBytes,ref c.triangleStrideBytes,ref c.pointsPtr,ref c.trianglesPtr,ref c.flags);}

		virtual public NxConvexMeshDesc getConvexMeshDesc()
		{
			NxConvexMeshDesc meshDesc=NxConvexMeshDesc.Default;
			saveToDesc(meshDesc);
			return meshDesc;
		}

		virtual public uint getSubmeshCount()
			{return wrapper_ConvexMesh_getSubmeshCount(nxConvexMeshPtr);}

		virtual public uint getCount(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_ConvexMesh_getCount(nxConvexMeshPtr,subMeshIndex,arrayType);}

		virtual public NxInternalFormat getFormat(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_ConvexMesh_getFormat(nxConvexMeshPtr,subMeshIndex,arrayType);}

		virtual public IntPtr getBase(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_ConvexMesh_getBase(nxConvexMeshPtr,subMeshIndex,arrayType);}

		virtual public uint getStride(uint subMeshIndex,NxInternalArray arrayType)
			{return wrapper_ConvexMesh_getStride(nxConvexMeshPtr,subMeshIndex,arrayType);}

		virtual public bool load(NxStream stream)
			{return wrapper_ConvexMesh_load(nxConvexMeshPtr,stream.NxStreamPtr);}


	
	
	
	
		virtual public Vector3[] getPoints()
			{return getConvexMeshDesc().getPoints();}
		
		virtual public int[] getTriangleIndices()
			{return getConvexMeshDesc().getTriangleIndices();}
		
		virtual public Vector3[] getTrianglesAsVertexTriplets()
			{return getConvexMeshDesc().getTrianglesAsVertexTriplets();}

		



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ConvexMesh_saveToDesc(IntPtr convexMesh,ref int numVertices,ref int numTriangles,ref int pointStrideBytes,ref int triangleStrideBytes,ref IntPtr pointsPtr,ref IntPtr trianglesPtr,ref uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ConvexMesh_getSubmeshCount(IntPtr convexMesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ConvexMesh_getCount(IntPtr convexMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxInternalFormat wrapper_ConvexMesh_getFormat(IntPtr convexMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ConvexMesh_getBase(IntPtr convexMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ConvexMesh_getStride(IntPtr convexMesh,uint subMeshIndex,NxInternalArray arrayType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ConvexMesh_load(IntPtr convexMesh,IntPtr streamPtr);
	}
}




/*
-	virtual	bool				saveToDesc(NxConvexMeshDesc&)	const	= 0;
-	virtual NxU32				getSubmeshCount()							const	= 0;
-	virtual NxU32				getCount(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual NxInternalFormat	getFormat(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual const void*			getBase(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual NxU32				getStride(NxSubmeshIndex, NxInternalArray)	const	= 0;
-	virtual	bool				load(const NxStream& stream)		= 0;
#	virtual void *				getInternal()
*/



//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxClothMesh
	{
		private IntPtr nxClothMeshPtr;
		
		static public NxClothMesh createFromPointer(IntPtr clothMeshPtr)
		{
			if(clothMeshPtr==IntPtr.Zero)
				{return null;}
			return new NxClothMesh(clothMeshPtr);
		}
		
		public NxClothMesh(IntPtr clothMeshPtr)
			{nxClothMeshPtr=clothMeshPtr;}
		
		public IntPtr NxClothMeshPtr
			{get{return nxClothMeshPtr;}}






		virtual public bool saveToDesc(NxClothMeshDesc c)
			{return wrapper_ClothMesh_saveToDesc(nxClothMeshPtr,ref c.numVertices,ref c.numTriangles,ref c.pointStrideBytes,ref c.triangleStrideBytes,ref c.pointsPtr,ref c.trianglesPtr,ref c.flags,ref c.target);}

		virtual public NxClothMeshDesc getClothMeshDesc()
		{
			NxClothMeshDesc meshDesc=NxClothMeshDesc.Default;
			saveToDesc(meshDesc);
			return meshDesc;
		}

		virtual public bool load(NxStream stream)
			{return wrapper_ClothMesh_load(nxClothMeshPtr,stream.NxStreamPtr);}

		virtual public uint getPageCount()
			{return wrapper_ClothMesh_getReferenceCount(nxClothMeshPtr);}
			

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ClothMesh_saveToDesc(IntPtr clothMesh,ref int numVertices,ref int numTriangles,ref int pointStrideBytes,ref int triangleStrideBytes,ref IntPtr pointsPtr,ref IntPtr trianglesPtr,ref uint flags,ref NxClothMeshTarget target);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_ClothMesh_load(IntPtr clothMesh,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_ClothMesh_getReferenceCount(IntPtr triangleMesh);
	}
}







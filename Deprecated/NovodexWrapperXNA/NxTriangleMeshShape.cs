//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


//Change from an array to an ArrayList for triangles to make it faster??



namespace NovodexWrapper
{
	public class NxTriangleMeshShape : NxShape
	{
		public NxTriangleMeshShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxTriangleMeshShape createFromPointer(NxActor actor,NxTriangleMeshShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxTriangleMeshShape(shapePointer);
		}
		
	

		new virtual public NxTriangleMeshShapeDesc getShapeDesc()
			{return (NxTriangleMeshShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxTriangleMeshShapeDesc shapeDesc=NxTriangleMeshShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}
		
		virtual public void saveToDesc(NxTriangleMeshShapeDesc shapeDesc)
			{wrapper_TriangleMeshShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.meshDataPtr,ref shapeDesc.meshFlags,ref shapeDesc.scale);}

		virtual public NxTriangleMesh getTriangleMesh()
			{return NxTriangleMesh.createFromPointer(wrapper_TriangleMeshShape_getTriangleMesh(nxShapePtr));}

		virtual public bool mapPageInstance(uint pageIndex)
			{return wrapper_TriangleMeshShape_mapPageInstance(nxShapePtr,pageIndex);}

		virtual public void unmapPageInstance(uint pageIndex)
			{wrapper_TriangleMeshShape_unmapPageInstance(nxShapePtr,pageIndex);}

		virtual public bool isPageInstanceMapped(uint pageIndex)
			{return wrapper_TriangleMeshShape_isPageInstanceMapped(nxShapePtr,pageIndex);}




		virtual public Vector3[] getPoints()
			{return getTriangleMesh().getPoints();}

		virtual public int[] getTriangleIndices()
			{return getTriangleMesh().getTriangleIndices();}
		
		virtual public Vector3[] getTrianglesAsVertexTriplets()
			{return getTriangleMesh().getTrianglesAsVertexTriplets();}



		virtual public NxTriangle getTriangle(uint triangleIndex,bool worldSpaceFlag)
		{
			uint flags;
			NxTriangle worldTriangle=new NxTriangle();
			NxTriangle edgeTriangle=new NxTriangle();
			getTriangle(ref worldTriangle,ref edgeTriangle,out flags,triangleIndex,worldSpaceFlag);
			return worldTriangle;
		}

		//edgeTri contains the worldSpace edgeNormals of the triangle		
		virtual public uint getTriangle(ref NxTriangle worldTri,ref NxTriangle edgeTri,out uint flags,uint triangleIndex,bool worldSpaceFlag)
			{return wrapper_TriangleMeshShape_getTriangle(nxShapePtr,ref worldTri,ref edgeTri,out flags,triangleIndex,worldSpaceFlag);}

		virtual public bool overlapAABBTriangles(NxBounds3 bounds,uint flags,out uint[] triangleIndexArray)
		{
			IntPtr trianglesPtr;
			uint numTriangles;
			bool result=wrapper_TriangleMeshShape_overlapAABBTriangles(nxShapePtr,bounds,flags,out numTriangles,out trianglesPtr);
			return copyTriangleIndexArray(result,out triangleIndexArray,trianglesPtr,numTriangles);
		}

		unsafe private bool copyTriangleIndexArray(bool result,out uint[] triangleIndexArray,IntPtr trianglesPtr,uint numTriangles)
		{
			if(result)
			{
				triangleIndexArray=new uint[numTriangles];
				uint* triangles=(uint*)trianglesPtr;
//crap
//Creating that array every damn time might be insane!! Use an arrayList instead and clear if null instead of setting to null??
				for(int i=0;i<numTriangles;i++)
					{triangleIndexArray[i]=triangles[i];}
			}
			else
				{triangleIndexArray=null;}

			return result;
		}

		virtual public int getNumTriangles()
			{return getTriangleMesh().getTriangleMeshDesc().numTriangles;}





		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_TriangleMeshShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref IntPtr meshDataPtr,ref uint meshFlags,ref float scale);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_TriangleMeshShape_getTriangleMesh(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_TriangleMeshShape_getTriangle(IntPtr shape,ref NxTriangle worldTri,ref NxTriangle edgeTri,out uint flags,uint triangleIndex,bool worldSpaceFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMeshShape_overlapAABBTriangles(IntPtr shape,NxBounds3 bounds,uint flags,out uint numTriangles,out IntPtr triangleIndices);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMeshShape_mapPageInstance(IntPtr shape,uint pageIndex);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_TriangleMeshShape_unmapPageInstance(IntPtr shape,uint pageIndex);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_TriangleMeshShape_isPageInstanceMapped(IntPtr shape,uint pageIndex);
	}
}

//Use NxQueryFlags
//I have no idea what the edge triangle is or what the edge flags are

/*
X	virtual	void				saveToDesc(NxTriangleMeshShapeDesc&)	const = 0;
X	virtual	NxTriangleMesh&		getTriangleMesh() = 0;
-	virtua	NxU32				getTriangle(NxTriangle& worldTri, NxTriangle* edgeTri, NxU32* flags, NxTriangleID triangleIndex, bool worldSpace) const	= 0;
X	virtual	bool				overlapAABBTriangles(const NxBounds3 bounds, NxU32 flags, NxU32& nb, const NxU32*& indices)	const	= 0;
-	virtual bool				mapPageInstance(NxU32 pageIndex) = 0;
-	virtual void				unmapPageInstance(NxU32 pageIndex) = 0;
-	virtual bool				isPageInstanceMapped(NxU32 pageIndex) const = 0;
*/



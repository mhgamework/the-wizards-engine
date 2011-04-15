//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	public class NxHeightFieldShape : NxShape
	{
		public NxHeightFieldShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxHeightFieldShape createFromPointer(NxActor actor,NxHeightFieldShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxHeightFieldShape(shapePointer);
		}
		
	




		public float HeightScale
		{
			get{return getHeightScale();}
			set{setHeightScale(value);}
		}

		public float RowScale
		{
			get{return getRowScale();}
			set{setRowScale(value);}
		}

		public float ColumnScale
		{
			get{return getColumnScale();}
			set{setColumnScale(value);}
		}








		virtual public void saveToDesc(NxHeightFieldShapeDesc shapeDesc)
			{wrapper_HeightFieldShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.heightFieldPtr,ref shapeDesc.heightScale,ref shapeDesc.rowScale,ref shapeDesc.columnScale,ref shapeDesc.materialIndexHighBits,ref shapeDesc.holeMaterial,ref shapeDesc.meshFlags);}

 		new virtual public NxHeightFieldShapeDesc getShapeDesc()
			{return (NxHeightFieldShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxHeightFieldShapeDesc shapeDesc=NxHeightFieldShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}

 		virtual public NxHeightField getHeightField()
			{return NxHeightField.createFromPointer(wrapper_HeightFieldShape_getHeightField(nxShapePtr));}


		virtual public float getHeightScale()
			{return wrapper_HeightFieldShape_getHeightScale(nxShapePtr);}		

		virtual public void setHeightScale(float scale)
			{wrapper_HeightFieldShape_setHeightScale(nxShapePtr,scale);}

		virtual public float getRowScale()
			{return wrapper_HeightFieldShape_getRowScale(nxShapePtr);}		

		virtual public void setRowScale(float scale)
			{wrapper_HeightFieldShape_setRowScale(nxShapePtr,scale);}

		virtual public float getColumnScale()
			{return wrapper_HeightFieldShape_getColumnScale(nxShapePtr);}		

		virtual public void setColumnScale(float scale)
			{wrapper_HeightFieldShape_setColumnScale(nxShapePtr,scale);}

		virtual public bool isShapePointOnHeightField(float x,float z)
			{return wrapper_HeightFieldShape_isShapePointOnHeightField(nxShapePtr,x,z);}

		virtual public float getHeightAtShapePoint(float x,float z)
			{return wrapper_HeightFieldShape_getHeightAtShapePoint(nxShapePtr,x,z);}		

		virtual public ushort getMaterialAtShapePoint(float x,float z)
			{return wrapper_HeightFieldShape_getMaterialAtShapePoint(nxShapePtr,x,z);}		

		virtual public Vector3 getNormalAtShapePoint(float x,float z)
		{
			Vector3 normal=new Vector3(0,0,0);
			wrapper_HeightFieldShape_getNormalAtShapePoint(nxShapePtr,x,z,ref normal);
			return normal;
		}

		virtual public Vector3 getSmoothNormalAtShapePoint(float x,float z)
		{
			Vector3 normal=new Vector3(0,0,0);
			wrapper_HeightFieldShape_getSmoothNormalAtShapePoint(nxShapePtr,x,z,ref normal);
			return normal;
		}






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
			{return wrapper_HeightFieldShape_getTriangle(nxShapePtr,ref worldTri,ref edgeTri,out flags,triangleIndex,worldSpaceFlag);}

		virtual public bool overlapAABBTriangles(NxBounds3 bounds,uint flags,out uint[] triangleIndexArray)
		{
			IntPtr trianglesPtr;
			uint numTriangles;
			bool result=wrapper_HeightFieldShape_overlapAABBTriangles(nxShapePtr,bounds,flags,out numTriangles,out trianglesPtr);
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

		public int getNumTriangles()
			{return getHeightField().getNumTriangles();}






		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_HeightFieldShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref IntPtr heightFieldPtr,ref float heightScale,ref float rowScale,ref float columnScale,ref ushort materialIndexHighBits,ref ushort holeMaterial,ref uint meshFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_HeightFieldShape_getHeightField(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightFieldShape_getHeightScale(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_HeightFieldShape_setHeightScale(IntPtr shape,float scale);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightFieldShape_getRowScale(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_HeightFieldShape_setRowScale(IntPtr shape,float scale);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightFieldShape_getColumnScale(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_HeightFieldShape_setColumnScale(IntPtr shape,float scale);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_HeightFieldShape_isShapePointOnHeightField(IntPtr shape,float x,float z);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightFieldShape_getHeightAtShapePoint(IntPtr shape,float x,float z);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_HeightFieldShape_getMaterialAtShapePoint(IntPtr shape,float x,float z);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_HeightFieldShape_getNormalAtShapePoint(IntPtr shape,float x,float z,ref Vector3 normal);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_HeightFieldShape_getSmoothNormalAtShapePoint(IntPtr shape,float x,float z,ref Vector3 normal);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_HeightFieldShape_getTriangle(IntPtr shape,ref NxTriangle worldTri,ref NxTriangle edgeTri,out uint flags,uint triangleIndex,bool worldSpaceFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_HeightFieldShape_overlapAABBTriangles(IntPtr shape,NxBounds3 bounds,uint flags,out uint numTriangles,out IntPtr triangleIndices);
	}
}



/*
-	virtual	void				saveToDesc(NxHeightFieldShapeDesc& desc)	const = 0;
-	virtual	NxHeightField&		getHeightField()							const = 0;
-	virtual NxReal				getHeightScale()							const = 0;
-	virtual NxReal				getRowScale()								const = 0;
-	virtual NxReal				getColumnScale()							const = 0;
-	virtual void				setHeightScale(NxReal scale) = 0;
-	virtual void				setRowScale(NxReal scale) = 0;
-	virtual void				setColumnScale(NxReal scale) = 0;
-	virtual bool				isShapePointOnHeightField(NxReal x, NxReal z) const = 0;
-	virtual NxReal				getHeightAtShapePoint(NxReal x, NxReal z) const = 0;
-	virtual NxMaterialIndex		getMaterialAtShapePoint(NxReal x, NxReal z) const = 0;
-	virtual NxVec3				getNormalAtShapePoint(NxReal x, NxReal z) const = 0;
-	virtual NxVec3				getSmoothNormalAtShapePoint(NxReal x, NxReal z) const = 0;
-	virtual	NxU32				getTriangle(NxTriangle& worldTri, NxTriangle* edgeTri, NxU32* flags, NxTriangleID triangleIndex, bool worldSpace) const	= 0;
-	virtual	bool				overlapAABBTriangles(const NxBounds3 bounds, NxU32 flags, NxU32& nb, const NxU32*& indices)	const	= 0;
*/





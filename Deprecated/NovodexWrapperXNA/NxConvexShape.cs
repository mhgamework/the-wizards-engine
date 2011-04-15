//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	public class NxConvexShape : NxShape
	{
		public NxConvexShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxConvexShape createFromPointer(NxActor actor,NxConvexShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxConvexShape(shapePointer);
		}
		
	



		virtual public void saveToDesc(NxConvexShapeDesc shapeDesc)
			{wrapper_ConvexShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.meshDataPtr,ref shapeDesc.meshFlags,ref shapeDesc.scale);}


		new virtual public NxConvexShapeDesc getShapeDesc()
			{return (NxConvexShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxConvexShapeDesc shapeDesc=NxConvexShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}

		virtual public NxConvexMesh getConvexMesh()
			{return NxConvexMesh.createFromPointer(wrapper_ConvexShape_getConvexMesh(nxShapePtr));}







		virtual public Vector3[] getPoints()
			{return getConvexMesh().getPoints();}

		virtual public int[] getTriangleIndices()
			{return getConvexMesh().getTriangleIndices();}
		
		virtual public Vector3[] getTrianglesAsVertexTriplets()
			{return getConvexMesh().getTrianglesAsVertexTriplets();}



		


		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ConvexShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref IntPtr meshDataPtr,ref uint meshFlags,ref float scale);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ConvexShape_getConvexMesh(IntPtr shape);
	}
}


/*
-	virtual	void			saveToDesc(NxConvexShapeDesc&)	const = 0;
-	virtual	NxConvexMesh&	getConvexMesh() = 0;
*/




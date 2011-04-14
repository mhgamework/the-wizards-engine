//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	public class NxBoxShape : NxShape
	{
		public NxBoxShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxBoxShape createFromPointer(NxActor actor,NxBoxShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxBoxShape(shapePointer);
		}
		
		
		public Vector3 Dimensions
		{
			get{return getDimensions();}
			set{setDimensions(value);}
		}
		
	

		public void setDimensions(Vector3 dimensions)
			{wrapper_BoxShape_setDimensions(nxShapePtr,ref dimensions);}

		public Vector3 getDimensions()
		{
			Vector3 dimensions;
			wrapper_BoxShape_getDimensions(nxShapePtr,out dimensions);
			return dimensions;
		}


		new virtual public NxBoxShapeDesc getShapeDesc()
			{return (NxBoxShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxBoxShapeDesc shapeDesc=NxBoxShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}
		
		virtual public void saveToDesc(NxBoxShapeDesc shapeDesc)
			{wrapper_BoxShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.dimensions);}


		virtual public NxBox getWorldOBB()
		{
			NxBox worldOBB=new NxBox();
			wrapper_BoxShape_getWorldOBB(nxShapePtr,worldOBB);
			return worldOBB;
		}


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_BoxShape_setDimensions(IntPtr shape,ref Vector3 dimensions);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_BoxShape_getDimensions(IntPtr shape,out Vector3 dimensions);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_BoxShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref Vector3 dimensions);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_BoxShape_getWorldOBB(IntPtr shape,NxBox worldOBB);
	}
}



/*
-	virtual void setDimensions(const NxVec3&) = 0;
-	virtual const NxVec3& getDimensions() const = 0;
-	virtual void getWorldOBB(NxBox&) const = 0;
-	virtual	bool saveToDesc(NxBoxShapeDesc&) const = 0;
*/


//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;





namespace NovodexWrapper
{
	public class NxPlaneShape : NxShape
	{
		public NxPlaneShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxPlaneShape createFromPointer(NxActor actor,NxPlaneShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxPlaneShape(shapePointer);
		}
		
		

		virtual public void setPlane(Vector3 normal,float d)
			{wrapper_PlaneShape_setPlane(nxShapePtr,ref normal,d);}

		virtual public void setPlane(NxPlane plane)
			{setPlane(plane.normal,plane.d);}

		virtual public NxPlane getPlane()
		{
			NxPlane plane=new NxPlane();
			wrapper_PlaneShape_getPlane(nxShapePtr,plane);
			return plane;
		}




		new virtual public NxPlaneShapeDesc getShapeDesc()
			{return (NxPlaneShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxPlaneShapeDesc shapeDesc=NxPlaneShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}
		
		virtual public void saveToDesc(NxPlaneShapeDesc shapeDesc)
			{wrapper_PlaneShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.normal,ref shapeDesc.d);}

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PlaneShape_setPlane(IntPtr shape,ref Vector3 normal,float d);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PlaneShape_getPlane(IntPtr shape,NxPlane plane);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PlaneShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref Vector3 normal,ref float d);
	}
}



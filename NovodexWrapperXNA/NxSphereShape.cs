//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	public class NxSphereShape : NxShape
	{
		public NxSphereShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxSphereShape createFromPointer(NxActor actor,NxSphereShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxSphereShape(shapePointer);
		}
		
		
		public float Radius
		{
			get{return getRadius();}
			set{setRadius(value);}
		}
		
	

		public void setRadius(float radius)
			{wrapper_SphereShape_setRadius(nxShapePtr,radius);}

		public float getRadius()
			{return wrapper_SphereShape_getRadius(nxShapePtr);}

		public NxSphere getWorldSphere()
		{
			NxSphere worldSphere=new NxSphere();
			wrapper_SphereShape_getWorldSphere(nxShapePtr,worldSphere);
			return worldSphere;
		}


		new virtual public NxSphereShapeDesc getShapeDesc()
			{return (NxSphereShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxSphereShapeDesc shapeDesc=NxSphereShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}
		
		virtual public void saveToDesc(NxSphereShapeDesc shapeDesc)
			{wrapper_SphereShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.radius);}

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphereShape_setRadius(IntPtr shape,float radius);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_SphereShape_getRadius(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphereShape_getWorldSphere(IntPtr shape,NxSphere worldSphere);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphereShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref float radius);
	}
}


/*
-	virtual void setRadius(NxReal radius) = 0;
-	virtual NxReal getRadius()					const = 0;
?	virtual void getWorldSphere(NxSphere& worldSphere)		const = 0;
-	virtual void saveToDesc(NxSphereShapeDesc& desc)	const = 0;
*/


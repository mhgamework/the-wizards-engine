//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	public class NxCapsuleShape : NxShape
	{
		public NxCapsuleShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxCapsuleShape createFromPointer(NxActor actor,NxCapsuleShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxCapsuleShape(shapePointer);
		}
		
		public float Radius
		{
			get{return getRadius();}
			set{setRadius(value);}
		}
		
		public float Height
		{
			get{return getHeight();}
			set{setHeight(value);}
		}		
	
		
		public void setDimensions(float radius,float height)
			{wrapper_CapsuleShape_setDimensions(nxShapePtr,radius,height);}

		public void setRadius(float radius)
			{wrapper_CapsuleShape_setRadius(nxShapePtr,radius);}

		public float getRadius()
			{return wrapper_CapsuleShape_getRadius(nxShapePtr);}

		public void setHeight(float height)
			{wrapper_CapsuleShape_setHeight(nxShapePtr,height);}

		public float getHeight()
			{return wrapper_CapsuleShape_getHeight(nxShapePtr);}

		public NxCapsule getWorldCapsule()
		{
			NxCapsule worldCapsule=new NxCapsule();
			wrapper_CapsuleShape_getWorldCapsule(nxShapePtr,worldCapsule);
			return worldCapsule;
		}
    
		new virtual public NxCapsuleShapeDesc getShapeDesc()
			{return (NxCapsuleShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxCapsuleShapeDesc shapeDesc=NxCapsuleShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}
		
		virtual public void saveToDesc(NxCapsuleShapeDesc shapeDesc)
			{wrapper_CapsuleShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.radius,ref shapeDesc.height,ref shapeDesc.flags);}


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_CapsuleShape_setDimensions(IntPtr shape,float radius,float height);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_CapsuleShape_setRadius(IntPtr shape,float radius);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_CapsuleShape_getRadius(IntPtr shape);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_CapsuleShape_setHeight(IntPtr shape,float height);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_CapsuleShape_getHeight(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_CapsuleShape_getWorldCapsule(IntPtr shape,NxCapsule worldCapsule);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_CapsuleShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref float radius,ref float height,ref uint flags);
	}
}


/*
-	virtual void setDimensions(NxReal radius, NxReal height) = 0;
-	virtual void setRadius(NxReal radius) = 0;
-	virtual NxReal getRadius() const = 0;
-	virtual void setHeight(NxReal height) = 0;	
-	virtual NxReal getHeight() const = 0;
-	virtual void getWorldCapsule(NxCapsule& worldCapsule)	const	= 0;
-	virtual	void	saveToDesc(NxCapsuleShapeDesc& desc)		const = 0;
*/



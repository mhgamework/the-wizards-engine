//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxCapsuleShapeDesc : NxShapeDesc
	{
		public float radius;	//!< radius of the capsule's hemispherical ends and its trunk.
		public float height;	//!< the distance between the two hemispherical ends of the capsule. The height is along the capsule's Y axis. 
		public uint	flags;		//!< Combination of ::NxCapsuleShapeFlag

		public static NxCapsuleShapeDesc Default
			{get{return new NxCapsuleShapeDesc();}}

		public NxCapsuleShapeDesc()
			{setToDefault();}

		//Uses default values for base class NxShapeDesc. The height is not the total height. It is the distance between the center of the rounded ends
		public NxCapsuleShapeDesc(float radius,float height)
		{
			setToDefault();
			this.radius=radius;
			this.height=height;
		}

		public NxCapsuleShapeDesc(float radius,float height,bool sweptShape)
		{
			setToDefault();
			this.radius=radius;
			this.height=height;
			this.FlagSweptShape=sweptShape;
		}

		public NxCapsuleShapeDesc(float radius,float height,Matrix localPose,bool sweptShape)
		{
			setToDefault();
			this.radius=radius;
			this.height=height;
			this.localPose=NovodexUtil.convertMatrixToNxMat34(localPose);
			this.FlagSweptShape=sweptShape;
		}
		
		//Pass in Matrix.Zero,NxShapeFlags.USE_DEFAULT,Vector3.Zero,and negative values to use default values
		public NxCapsuleShapeDesc(Matrix localPose,NxShapeFlag shapeFlags,int group,int materialIndex,string name,float radius,float height,bool sweptShape)
		{
			setToDefault();
			base.set(localPose,shapeFlags,group,materialIndex,name);
			if(radius>=0){this.radius=radius;}
			if(height>=0){this.height=height;}
			this.FlagSweptShape=sweptShape;
		}

		override public void setToDefault()
		{
			base.setToDefault();
			radius = 0.0f;
			height = 0.0f;
			flags  = 0;
			type=NxShapeType.NX_SHAPE_CAPSULE;
		}

		public bool FlagSweptShape
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxCapsuleShapeFlag.NX_SWEPT_SHAPE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxCapsuleShapeFlag.NX_SWEPT_SHAPE,value);}
		}
	}
}




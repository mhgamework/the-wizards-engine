//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSphereShapeDesc : NxShapeDesc
	{
		public float radius;	//!< radius of shape. Must be positive.

		public static NxSphereShapeDesc Default
			{get{return new NxSphereShapeDesc();}}

		public NxSphereShapeDesc()
			{setToDefault();}

		//Uses default values for base class NxShapeDesc.
		public NxSphereShapeDesc(float radius)
		{
			setToDefault();
			this.radius=radius;
		}

		public NxSphereShapeDesc(float radius,Matrix localPose)
		{
			setToDefault();
			this.radius=radius;
			this.localPose=NovodexUtil.convertMatrixToNxMat34(localPose);
		}

		//Pass in Matrix.Zero,NxShapeFlags.USE_DEFAULT,Vector3.Zero,and negative values to use default values
		public NxSphereShapeDesc(Matrix localPose,NxShapeFlag shapeFlags,int group,int materialIndex,string name,float radius)
		{
			setToDefault();
			base.set(localPose,shapeFlags,group,materialIndex,name);
			if(radius>=0){this.radius=radius;}
		}
		
		override public void setToDefault()
		{
			base.setToDefault();
			radius = 0.0f;
			type=NxShapeType.NX_SHAPE_SPHERE;
		}
	}
}
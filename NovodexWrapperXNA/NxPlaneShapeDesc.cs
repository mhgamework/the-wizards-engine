//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxPlaneShapeDesc : NxShapeDesc
	{
		public Vector3	normal;
		public float	d;

		public static NxPlaneShapeDesc Default
			{get{return new NxPlaneShapeDesc();}}	
	
		public NxPlaneShapeDesc()
			{setToDefault();}

		//Uses default values for base class NxShapeDesc
		public NxPlaneShapeDesc(Vector3 normal,float d)
		{
			setToDefault();
			this.normal=normal;
			this.d=d;
			normal.Normalize();
		}
		
		public NxPlaneShapeDesc(Vector3 normal,float d,Matrix localPose)
		{
			setToDefault();
			this.normal=normal;
			this.d=d;
			normal.Normalize();
			this.localPose=NovodexUtil.convertMatrixToNxMat34(localPose);
		}

		//Pass in Matrix.Zero,NxShapeFlags.USE_DEFAULT,Vector3.Zero,and negative values to use default values
		public NxPlaneShapeDesc(Matrix localPose,NxShapeFlag shapeFlags,int group,int materialIndex,string name,Vector3 normal,float d)
		{
			setToDefault();
			base.set(localPose,shapeFlags,group,materialIndex,name);
			if(normal!=Vector3.Zero)
			{
				this.normal=normal;
				this.d=d;
				normal.Normalize();
			}
		}
		
		new public void setToDefault()
		{
			base.setToDefault();
			normal=new Vector3(0,1,0);
			d=0;
			type=NxShapeType.NX_SHAPE_PLANE;
		}
	}
}




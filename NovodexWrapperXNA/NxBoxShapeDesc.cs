//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxBoxShapeDesc : NxShapeDesc
	{
		public Vector3 dimensions;

		public static NxBoxShapeDesc Default
			{get{return new NxBoxShapeDesc();}}
		
		public NxBoxShapeDesc()
			{setToDefault();}

		//Uses default values for base class NxShapeDesc. The Novodex dimensions are radii like so divide by two for true sizes
		public NxBoxShapeDesc(float width,float height,float depth)
		{
			setToDefault();
			this.dimensions=new Vector3(width,height,depth);
		}
	
		//Uses default values for base class NxShapeDesc. The Novodex dimensions are radii like so divide by two for true sizes
		public NxBoxShapeDesc(Vector3 dimensions)
		{
			setToDefault();
			this.dimensions=dimensions;
		}

		public NxBoxShapeDesc(Vector3 dimensions,Matrix localPose)
		{
			setToDefault();
			this.dimensions=dimensions;
			this.localPose=NovodexUtil.convertMatrixToNxMat34(localPose);
		}

		//Pass in Matrix.Zero,NxShapeFlags.USE_DEFAULT,Vector3.Zero,and negative values to use default values
		public NxBoxShapeDesc(Matrix localPose,NxShapeFlag shapeFlags,int group,int materialIndex,string name,Vector3 dimensions)
		{
			setToDefault();
			base.set(localPose,shapeFlags,group,materialIndex,name);
			if(dimensions!=Vector3.Zero)
				{this.dimensions=dimensions;}
		}
		
		override public void setToDefault()
		{
			base.setToDefault();
			dimensions=new Vector3(0,0,0);
			type=NxShapeType.NX_SHAPE_BOX;
		}
	}
}





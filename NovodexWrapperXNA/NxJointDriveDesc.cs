//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxJointDriveDesc
	{
		public uint			driveType;
		public float		spring;
		public float		damping;
		public float		forceLimit;

		public static NxJointDriveDesc Default
			{get{return NxJointDriveDesc.createDefault();}}
		
		private static NxJointDriveDesc createDefault()
		{
			NxJointDriveDesc driveDesc=new NxJointDriveDesc();
			driveDesc.setToDefault();
			return driveDesc;
		}

		public NxJointDriveDesc(uint driveType,float spring,float damping,float forceLimit)
		{
			this.driveType=driveType;
			this.spring=spring;
			this.damping=damping;
			this.forceLimit=forceLimit;
		}
			
		public void setToDefault()
		{
			driveType = 0;
			spring = 0;
			damping = 0;
			forceLimit = float.MaxValue;
		}
	}
}


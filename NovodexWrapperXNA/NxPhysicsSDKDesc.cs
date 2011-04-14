//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxPhysicsSDKDesc
	{
		public uint hwPageSize;		//!< size of hardware mesh pages. Currently only the value 65536 is supported.
		public uint hwPageMax;		//!< maximum number of hardware pages supported concurrently on hardware
		public uint hwConvexMax;	//!< maximum number of convex meshes which will be resident on hardware.
		public uint flags;			//!< scene creation flags. \see 

		public static NxPhysicsSDKDesc Default
			{get{return new NxPhysicsSDKDesc();}}

		public NxPhysicsSDKDesc()
			{setToDefault();}
			
		public NxPhysicsSDKDesc(uint hwPageSize,uint hwPageMax,uint hwConvexMax,uint flags)
			{set(hwPageSize,hwPageMax,hwConvexMax,flags);}
			
		public void set(uint hwPageSize,uint hwPageMax,uint hwConvexMax,uint flags)
		{
			this.hwPageSize=hwPageSize;
			this.hwPageMax=hwPageMax;
			this.hwConvexMax=hwConvexMax;
			this.flags=flags;
		}

		public void setToDefault()
		{
			hwPageSize = 65536;
			hwConvexMax = 2048;
			hwPageMax = 256;
			flags = 0;
		}
	}
}


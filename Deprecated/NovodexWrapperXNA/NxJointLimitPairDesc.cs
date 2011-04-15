//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxJointLimitPairDesc
	{
		public NxJointLimitDesc low;		//!< The low limit (smaller value)
		public NxJointLimitDesc high;		//!< the high limit (larger value)

		public static NxJointLimitPairDesc Default
			{get{return new NxJointLimitPairDesc();}}

		public NxJointLimitPairDesc()
			{setToDefault();}
		
		public NxJointLimitPairDesc(float lowValue,float lowRestitution,float lowHardness,float highValue,float highRestitution,float highHardness)
		{
			low=new NxJointLimitDesc(lowValue,lowRestitution,lowHardness);
			high=new NxJointLimitDesc(highValue,highRestitution,highHardness);
		}
	
		public void setToDefault()
		{
			low=NxJointLimitDesc.Default;
			high=NxJointLimitDesc.Default;
		}
	}
}





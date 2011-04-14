//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxJointLimitSoftPairDesc
	{
		public NxJointLimitSoftDesc low;		//!< The low limit (smaller value)
		public NxJointLimitSoftDesc high;		//!< the high limit (larger value)
		
		public static NxJointLimitSoftPairDesc Default
			{get{return new NxJointLimitSoftPairDesc();}}

		public NxJointLimitSoftPairDesc()
			{setToDefault();}
		
		public NxJointLimitSoftPairDesc(float lowValue,float lowRestitution,float lowSpring,float lowDamping,float highValue,float highRestitution,float highSpring,float highDamping)
		{
			low=new NxJointLimitSoftDesc(lowValue,lowRestitution,lowSpring,lowDamping);
			high=new NxJointLimitSoftDesc(highValue,highRestitution,highSpring,highDamping);
		}
	
		public void setToDefault()
		{
			low=NxJointLimitSoftDesc.Default;
			high=NxJointLimitSoftDesc.Default;
		}

	}
}

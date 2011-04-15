//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxJointLimitDesc
	{
		public float value;			//!< the angle / position beyond which the limit is active. Which side the limit restricts depends on whether this is a high or low limit.
		public float restitution;	//!< limit bounce
		public float hardness;		//!< [not yet implemented!] limit can be made softer by setting this to less than 1.

		public static NxJointLimitDesc Default
			{get{return NxJointLimitDesc.createDefault();}}
		
		private static NxJointLimitDesc createDefault()
		{
			NxJointLimitDesc limitDesc=new NxJointLimitDesc();
			limitDesc.setToDefault();
			return limitDesc;
		}

		public NxJointLimitDesc(float value,float restitution,float hardness)
		{
			this.value=value;
			this.restitution=restitution;
			this.hardness=hardness;
		}
			
		public void setToDefault()
		{
			value=0;
			restitution=0;
			hardness=1;
		}
	
	}
}

/*
class NxJointLimitDesc
{
	NxReal value;
	NxReal restitution;
	NxReal hardness;
};
*/

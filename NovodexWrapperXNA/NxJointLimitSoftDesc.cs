//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxJointLimitSoftDesc
	{
		public float value;			//!< the angle / position beyond which the limit is active. Which side the limit restricts depends on whether this is a high or low limit.
		public float restitution;	//!< limit bounce
		public float spring;		//!< if greater than zero, the limit is soft, i.e. a spring pulls the joint back to the limit
		public float damping;		//!< if spring is greater than zero, this is the damping of the spring

		public static NxJointLimitSoftDesc Default
			{get{return NxJointLimitSoftDesc.createDefault();}}
		
		private static NxJointLimitSoftDesc createDefault()
		{
			NxJointLimitSoftDesc limitSoftDesc=new NxJointLimitSoftDesc();
			limitSoftDesc.setToDefault();
			return limitSoftDesc;
		}

		public NxJointLimitSoftDesc(float value,float restitution,float spring,float damping)
		{
			this.value=value;
			this.restitution=restitution;
			this.spring=spring;
			this.damping=damping;
		}
			
		public void setToDefault()
		{
			value		= 0;
			restitution = 0;
			spring		= 0;
			damping		= 0;
		}
	}
}





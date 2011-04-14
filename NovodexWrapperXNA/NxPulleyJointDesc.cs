//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	public class NxPulleyJointDesc : NxJointDesc
	{
		public Vector3 pulley_0;	//!< suspension point of first body in world space.
		public Vector3 pulley_1;	//!< suspension point of second body in world space.
		public float distance;		//!< the rest length of the rope connecting the two objects.  The distance is computed as ||(pulley0 - anchor0)|| +  ||(pulley1 - anchor1)|| * ratio.
		public float stiffness;		//!< how stiff the constraint is, between 0 and 1 (stiffest)
		public float ratio;			//!< transmission ratio
		public uint flags;			//!< This is a combination of the bits defined by ::NxPulleyJointFlag. 
		public NxMotorDesc	motor;	//!< Optional motor.


		public static NxPulleyJointDesc Default
			{get{return new NxPulleyJointDesc();}}

		public NxPulleyJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();
			pulley_0	= new Vector3(0,0,0);
			pulley_1	= new Vector3(0,0,0);
			distance	= 0.0f;
			stiffness	= 1.0f;
			ratio		= 1.0f;
			flags		= 0;
			motor		= new NxMotorDesc();
		}
		
		
		public bool FlagIsRigid
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxPulleyJointFlag.NX_PJF_IS_RIGID);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxPulleyJointFlag.NX_PJF_IS_RIGID,value);}
		}
	}
}


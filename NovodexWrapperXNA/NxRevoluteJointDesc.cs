//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	public class NxRevoluteJointDesc : NxJointDesc
	{
		public NxJointLimitPairDesc		limit;					//!< Optional limits for the angular motion of the joint. 
		public NxMotorDesc				motor;					//!< Optional motor.
		public NxSpringDesc				spring;					//!< Optional spring.
		public float					projectionDistance;	
		public float					projectionAngle;
		public uint						flags;					//!< This is a combination of the bits defined by ::NxRevoluteJointFlag . 
		public NxJointProjectionMode	projectionMode;			//!< use this to enable joint projection


		public static NxRevoluteJointDesc Default
			{get{return new NxRevoluteJointDesc();}}

		public NxRevoluteJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();

			limit				= new NxJointLimitPairDesc();
			motor				= new NxMotorDesc();
			spring				= new NxSpringDesc();
			projectionDistance	= 1.0f;
			projectionAngle		= 5.0f * NovodexUtil.DEG_TO_RAD;
			flags				= 0;
			projectionMode		= NxJointProjectionMode.NX_JPM_NONE;
		}
		
		public bool FlagLimitEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxRevoluteJointFlag.NX_RJF_LIMIT_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxRevoluteJointFlag.NX_RJF_LIMIT_ENABLED,value);}
		}		
		
		public bool FlagMotorEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxRevoluteJointFlag.NX_RJF_MOTOR_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxRevoluteJointFlag.NX_RJF_MOTOR_ENABLED,value);}
		}		
		
		public bool FlagSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxRevoluteJointFlag.NX_RJF_SPRING_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxRevoluteJointFlag.NX_RJF_SPRING_ENABLED,value);}
		}
	}
}



/*
class NxRevoluteJointDesc : public NxJointDesc
{							{
	NxJointLimitPairDesc limit;
	NxMotorDesc			 motor; 
	NxSpringDesc		 spring;
	NxReal projectionDistance;	
	NxReal projectionAngle;
	NxU32 flags;
	NxJointProjectionMode projectionMode;
};
*/

//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSphericalJointDesc : NxJointDesc
	{
		public Vector3 swingAxis;						//!<swing limit axis defined in the joint space of actor 0 ([localNormal[0], localAxis[0]^localNormal[0],localAxis[0]])
		public float projectionDistance;				//projectionDistance: if flags.projectionMode is 1, the joint gets artificially projected together when it drifts more than this distance. Sometimes it is not possible to project (for example when the joints form a cycle)Should be nonnegative. However, it may be a bad idea to always project to a very small or zero distance because the solver *needs* some error in order to produce correct motion.
		public NxJointLimitPairDesc twistLimit;			//!< limits rotation around twist axis
		public NxJointLimitDesc swingLimit;				//!< limits swing of twist axis
		public NxSpringDesc	twistSpring;				//!< spring that works against twisting
		public NxSpringDesc	swingSpring;				//!< spring that works against swinging
		public NxSpringDesc	jointSpring;				//!< spring that lets the joint get pulled apart
		public uint flags;								//!< This is a combination of the bits defined by ::NxSphericalJointFlag . 
		public NxJointProjectionMode projectionMode;	//!< use this to enable joint projection

		public static NxSphericalJointDesc Default
			{get{return new NxSphericalJointDesc();}}

		public NxSphericalJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();

			swingAxis			= new Vector3(0,0,1);
			twistLimit			= new NxJointLimitPairDesc();
			swingLimit			= NxJointLimitDesc.Default;
			twistSpring			= new NxSpringDesc();
			swingSpring			= new NxSpringDesc();
			jointSpring			= new NxSpringDesc();
			projectionDistance	= 1.0f;
			flags				= 0;
			projectionMode		= NxJointProjectionMode.NX_JPM_NONE;
		}
		
		
		
		public bool FlagTwistLimitEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSphericalJointFlag.NX_SJF_TWIST_LIMIT_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSphericalJointFlag.NX_SJF_TWIST_LIMIT_ENABLED,value);}
		}			
		
		public bool FlagSwingLimitEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSphericalJointFlag.NX_SJF_SWING_LIMIT_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSphericalJointFlag.NX_SJF_SWING_LIMIT_ENABLED,value);}
		}			
		
		public bool FlagTwistSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSphericalJointFlag.NX_SJF_TWIST_SPRING_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSphericalJointFlag.NX_SJF_TWIST_SPRING_ENABLED,value);}
		}			
		
		public bool FlagSwingSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSphericalJointFlag.NX_SJF_SWING_SPRING_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSphericalJointFlag.NX_SJF_SWING_SPRING_ENABLED,value);}
		}			
		
		public bool FlagJointSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSphericalJointFlag.NX_SJF_JOINT_SPRING_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSphericalJointFlag.NX_SJF_JOINT_SPRING_ENABLED,value);}
		}	
		
	}
}










//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	public class NxDistanceJointDesc : NxJointDesc
	{
		public float		maxDistance;	//!< The maximum rest length of the rope or rod between the two anchor points 
		public float		minDistance;	//!< The minimum rest length of the rope or rod between the two anchor points
		public NxSpringDesc	spring;			//!< makes the joint springy.  The spring.targetValue field is not used.
		public uint			flags;			//!< This is a combination of the bits defined by ::NxDistanceJointFlag . 
		
		public static NxDistanceJointDesc Default
			{get{return new NxDistanceJointDesc();}}

		public NxDistanceJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();
			maxDistance = 0.0f;
			minDistance = 0.0f;
			spring		= NxSpringDesc.Default;
			flags		= 0;
		}
		
	
	
		public bool FlagMinDistanceEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxDistanceJointFlag.NX_DJF_MIN_DISTANCE_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxDistanceJointFlag.NX_DJF_MIN_DISTANCE_ENABLED,value);}
		}		

		public bool FlagMaxDistanceEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxDistanceJointFlag.NX_DJF_MAX_DISTANCE_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxDistanceJointFlag.NX_DJF_MAX_DISTANCE_ENABLED,value);}
		}		
		
		public bool FlagSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxDistanceJointFlag.NX_DJF_SPRING_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxDistanceJointFlag.NX_DJF_SPRING_ENABLED,value);}
		}
	}
}




//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


//The twistLimit is like the revoluteJoint. It uses the localAxis of actor0. It is displayed as a wedge.
// If you don't define a swingAxis the cone will be along the localAxis too

//Setting a swingAxis is hard to figure out because it is in this insane space:
// !<swing limit axis defined in the joint space of actor 0 ([localNormal[0], localAxis[0]^localNormal[0],localAxis[0]])

//Using a small twistLimit with the swingLimit enabled causes jittering



namespace NovodexWrapper
{
	public class NxSphericalJoint : NxJoint
	{
		public NxSphericalJoint(IntPtr jointPointer) : base(jointPointer)
			{}
		
		public override NxJoint copy(NxActor newActor0,NxActor newActor1)
		{
			NxSphericalJointDesc jointDesc=this.getJointDesc();
			jointDesc.setActors(newActor0,newActor1);
			return ParentScene.createJoint(jointDesc);
		}
			
		new public static NxSphericalJoint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}
			return new NxSphericalJoint(jointPointer);
		}
		

	
	
	
		public bool FlagTwistLimitEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_TWIST_LIMIT_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_TWIST_LIMIT_ENABLED,value));}		
		}			
		
		public bool FlagSwingLimitEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_SWING_LIMIT_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_SWING_LIMIT_ENABLED,value));}		
		}			
		
		public bool FlagTwistSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_TWIST_SPRING_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_TWIST_SPRING_ENABLED,value));}		
		}			
		
		public bool FlagSwingSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_SWING_SPRING_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_SWING_SPRING_ENABLED,value));}		
		}			
		
		public bool FlagJointSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_JOINT_SPRING_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxSphericalJointFlag.NX_SJF_JOINT_SPRING_ENABLED,value));}		
		}	
		
		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}

		public Vector3 SwingAxis
		{
			get{return getSwingAxis();}
			set{setSwingAxis(value);}
		}
		
		public float ProjectionDistance
		{
			get{return getProjectionDistance();}
			set{setProjectionDistance(value);}
		}

		public NxJointLimitPairDesc TwistLimit
		{
			get{return getTwistLimit();}
			set{setTwistLimit(value);}
		}
		
		public NxJointLimitDesc SwingLimit
		{
			get{return getSwingLimit();}
			set{setSwingLimit(value);}
		}
  
		public NxSpringDesc TwistSpring
		{
			get{return getTwistSpring();}
			set{setTwistSpring(value);}
		}

		public NxSpringDesc SwingSpring
		{
			get{return getSwingSpring();}
			set{setSwingSpring(value);}
		}

		public NxSpringDesc JointSpring
		{
			get{return getJointSpring();}
			set{setJointSpring(value);}
		}
  
	
		
			
			
		virtual public void loadFromDesc(NxSphericalJointDesc jointDesc)
			{wrapper_SphericalJoint_loadFromDesc(nxJointPtr,jointDesc.actor[0].NxActorPtr,jointDesc.actor[1].NxActorPtr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],jointDesc.maxForce,jointDesc.maxTorque,jointDesc.userData,jointDesc.internalNamePtr,jointDesc.jointFlags,ref jointDesc.swingAxis,jointDesc.projectionDistance,jointDesc.twistLimit,ref jointDesc.swingLimit,jointDesc.twistSpring,jointDesc.swingSpring,jointDesc.jointSpring,jointDesc.flags,jointDesc.projectionMode);}

		new virtual public NxSphericalJointDesc getJointDesc()
		{
			NxSphericalJointDesc jointDesc=NxSphericalJointDesc.Default;
			saveToDesc(jointDesc);
			return jointDesc;
		}
			
		virtual public void saveToDesc(NxSphericalJointDesc jointDesc)
		{
			IntPtr actor0_ptr=IntPtr.Zero;
			IntPtr actor1_ptr=IntPtr.Zero;
			wrapper_SphericalJoint_saveToDesc(nxJointPtr,ref actor0_ptr,ref actor1_ptr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],ref jointDesc.maxForce,ref jointDesc.maxTorque,ref jointDesc.userData,ref jointDesc.internalNamePtr,ref jointDesc.jointFlags,ref jointDesc.swingAxis,ref jointDesc.projectionDistance,jointDesc.twistLimit,ref jointDesc.swingLimit,jointDesc.twistSpring,jointDesc.swingSpring,jointDesc.jointSpring,ref jointDesc.flags,ref jointDesc.projectionMode);
			jointDesc.actor[0]=NxActor.createFromPointer(actor0_ptr);
			jointDesc.actor[1]=NxActor.createFromPointer(actor1_ptr);
			jointDesc.name=getName();
		}
		
		protected override void internalLoadFromDesc(NxJointDesc jointDesc)
			{loadFromDesc((NxSphericalJointDesc)jointDesc);}
			
		protected override NxJointDesc internalGetJointDesc()
			{return (NxSphericalJointDesc)getJointDesc();}






		virtual public void setFlags(uint flags)
			{wrapper_SphericalJoint_setFlags(nxJointPtr,flags);}
			
		virtual public uint getFlags()
			{return wrapper_SphericalJoint_getFlags(nxJointPtr);}

		virtual public void setProjectionMode(NxJointProjectionMode projectionMode)
			{wrapper_SphericalJoint_setProjectionMode(nxJointPtr,projectionMode);}

		virtual public NxJointProjectionMode getProjectionMode()
			{return wrapper_SphericalJoint_getProjectionMode(nxJointPtr);}




		virtual	public Vector3 getSwingAxis()
			{return getJointDesc().swingAxis;}
		
		virtual	public void setSwingAxis(Vector3 swingAxis)
		{
			swingAxis.Normalize();
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.swingAxis=swingAxis;
			loadFromDesc(jointDesc);
		}
		
		virtual	public float getProjectionDistance()
			{return getJointDesc().projectionDistance;}
		
		virtual	public void setProjectionDistance(float projectionDistance)
		{
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.projectionDistance=projectionDistance;
			loadFromDesc(jointDesc);
		}
		
		virtual	public NxJointLimitPairDesc getTwistLimit()
			{return getJointDesc().twistLimit;}
		
		virtual	public void setTwistLimit(NxJointLimitPairDesc twistLimit)
		{
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.twistLimit=twistLimit;
			loadFromDesc(jointDesc);
			FlagTwistLimitEnabled=true;
		}
		
		virtual	public NxJointLimitDesc getSwingLimit()
			{return getJointDesc().swingLimit;}
		
		virtual	public void setSwingLimit(NxJointLimitDesc swingLimit)
		{
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.swingLimit=swingLimit;
			loadFromDesc(jointDesc);
			FlagSwingLimitEnabled=true;
		}
		
		virtual	public NxSpringDesc getTwistSpring()
			{return getJointDesc().twistSpring;}
		
		virtual	public void setTwistSpring(NxSpringDesc twistSpring)
		{
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.twistSpring=twistSpring;
			loadFromDesc(jointDesc);
			FlagTwistSpringEnabled=true;
		}		
		
		virtual	public NxSpringDesc getSwingSpring()
			{return getJointDesc().swingSpring;}
		
		virtual	public void setSwingSpring(NxSpringDesc swingSpring)
		{
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.swingSpring=swingSpring;
			loadFromDesc(jointDesc);
			FlagSwingSpringEnabled=true;
		}		
		
		virtual	public NxSpringDesc getJointSpring()
			{return getJointDesc().jointSpring;}
		
		virtual	public void setJointSpring(NxSpringDesc jointSpring)
		{
			NxSphericalJointDesc jointDesc=getJointDesc();
			jointDesc.jointSpring=jointSpring;
			loadFromDesc(jointDesc);
			FlagJointSpringEnabled=true;
		}


		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphericalJoint_loadFromDesc(IntPtr joint,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,ref Vector3 swingAxis,float projectionDistance,NxJointLimitPairDesc twistLimit,ref NxJointLimitDesc swingLimit,NxSpringDesc twistSpring,NxSpringDesc swingSpring,NxSpringDesc jointSpring,uint flags,NxJointProjectionMode projectionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphericalJoint_saveToDesc(IntPtr joint,ref IntPtr actor_0,ref IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,ref float maxForce,ref float maxTorque,ref IntPtr userData,ref IntPtr name,ref uint jointFlags,ref Vector3 swingAxis,ref float projectionDistance,NxJointLimitPairDesc twistLimit,ref NxJointLimitDesc swingLimit,NxSpringDesc twistSpring,NxSpringDesc swingSpring,NxSpringDesc jointSpring,ref uint flags,ref NxJointProjectionMode projectionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphericalJoint_setFlags(IntPtr joint,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_SphericalJoint_getFlags(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_SphericalJoint_setProjectionMode(IntPtr joint,NxJointProjectionMode projectionMode);		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxJointProjectionMode wrapper_SphericalJoint_getProjectionMode(IntPtr joint);
	}
}




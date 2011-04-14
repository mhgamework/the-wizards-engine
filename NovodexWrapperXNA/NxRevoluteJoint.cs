//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	public class NxRevoluteJoint : NxJoint
	{
		public NxRevoluteJoint(IntPtr jointPointer) : base(jointPointer)
			{}
			
		public override NxJoint copy(NxActor newActor0,NxActor newActor1)
		{
			NxRevoluteJointDesc jointDesc=this.getJointDesc();
			jointDesc.setActors(newActor0,newActor1);
			return ParentScene.createJoint(jointDesc);
		}
			
		new public static NxRevoluteJoint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}
			return new NxRevoluteJoint(jointPointer);
		}



		public NxJointLimitPairDesc Limit
		{
			get{return getLimits();}
			set{setLimits(value);}
		}

		public NxMotorDesc Motor
		{
			get{return getMotor();}
			set{setMotor(value);}
		}

		public NxSpringDesc Spring
		{
			get{return getSpring();}
			set{setSpring(value);}
		}

		public bool FlagLimitEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxRevoluteJointFlag.NX_RJF_LIMIT_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxRevoluteJointFlag.NX_RJF_LIMIT_ENABLED,value));}
		}		
		
		public bool FlagMotorEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxRevoluteJointFlag.NX_RJF_MOTOR_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxRevoluteJointFlag.NX_RJF_MOTOR_ENABLED,value));}
		}		
		
		public bool FlagSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxRevoluteJointFlag.NX_RJF_SPRING_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxRevoluteJointFlag.NX_RJF_SPRING_ENABLED,value));}
		}
		
		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}

		public NxJointProjectionMode ProjectionMode
		{
			get{return getProjectionMode();}
			set{setProjectionMode(value);}
		}
		
		public float ProjectionDistance
		{
			get
			{
				NxRevoluteJointDesc jointDesc=getJointDesc();
				return jointDesc.projectionDistance;	
			}
			set
			{
				NxRevoluteJointDesc jointDesc=getJointDesc();
				jointDesc.projectionDistance=value;
				loadFromDesc(jointDesc);
			}
		}		
		
		public float ProjectionAngle
		{
			get
			{
				NxRevoluteJointDesc jointDesc=getJointDesc();
				return jointDesc.projectionAngle;	
			}
			set
			{
				NxRevoluteJointDesc jointDesc=getJointDesc();
				jointDesc.projectionAngle=value;
				loadFromDesc(jointDesc);
			}
		}
		
		public float HingeAngle
			{get{return getAngle();}}






		virtual public void loadFromDesc(NxRevoluteJointDesc jointDesc)
			{wrapper_RevoluteJoint_loadFromDesc(nxJointPtr,jointDesc.actor[0].NxActorPtr,jointDesc.actor[1].NxActorPtr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],jointDesc.maxForce,jointDesc.maxTorque,jointDesc.userData,jointDesc.internalNamePtr,jointDesc.jointFlags,jointDesc.limit,jointDesc.motor,jointDesc.spring,jointDesc.projectionDistance,jointDesc.projectionAngle,jointDesc.flags,jointDesc.projectionMode);}

		new virtual public NxRevoluteJointDesc getJointDesc()
		{
			NxRevoluteJointDesc jointDesc=NxRevoluteJointDesc.Default;
			saveToDesc(jointDesc);
			return jointDesc;
		}
			
		virtual public void saveToDesc(NxRevoluteJointDesc jointDesc)
		{
			IntPtr actor0_ptr=IntPtr.Zero;
			IntPtr actor1_ptr=IntPtr.Zero;
			wrapper_RevoluteJoint_saveToDesc(nxJointPtr,ref actor0_ptr,ref actor1_ptr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],ref jointDesc.maxForce,ref jointDesc.maxTorque,ref jointDesc.userData,ref jointDesc.internalNamePtr,ref jointDesc.jointFlags,jointDesc.limit,jointDesc.motor,jointDesc.spring,ref jointDesc.projectionDistance,ref jointDesc.projectionAngle,ref jointDesc.flags,ref jointDesc.projectionMode);
			jointDesc.actor[0]=NxActor.createFromPointer(actor0_ptr);
			jointDesc.actor[1]=NxActor.createFromPointer(actor1_ptr);
			jointDesc.name=getName();
		}
	
		protected override void internalLoadFromDesc(NxJointDesc jointDesc)
			{loadFromDesc((NxRevoluteJointDesc)jointDesc);}
			
		protected override NxJointDesc internalGetJointDesc()
			{return (NxRevoluteJointDesc)getJointDesc();}



		virtual public void setLimits(NxJointLimitPairDesc limit)
			{wrapper_RevoluteJoint_setLimits(nxJointPtr,limit);}

		virtual public NxJointLimitPairDesc getLimits()
		{
			NxJointLimitPairDesc limit=NxJointLimitPairDesc.Default;
			if(!getLimits(ref limit))
				{return null;}
			return limit;
		}		

		virtual public bool getLimits(ref NxJointLimitPairDesc limit)
			{return wrapper_RevoluteJoint_getLimits(nxJointPtr,limit);}

		virtual public void setMotor(NxMotorDesc motor)
			{wrapper_RevoluteJoint_setMotor(nxJointPtr,motor);}

		virtual public bool getMotor(ref NxMotorDesc motor)
			{return wrapper_RevoluteJoint_getMotor(nxJointPtr,motor);}
			
		virtual public NxMotorDesc getMotor()
		{
			NxMotorDesc motor=NxMotorDesc.Default;
			if(!getMotor(ref motor))
				{return null;}
			return motor;
		}	

		virtual public void setSpring(NxSpringDesc spring)
			{wrapper_RevoluteJoint_setSpring(nxJointPtr,spring);}
			
		virtual public NxSpringDesc getSpring()
		{
			NxSpringDesc spring=NxSpringDesc.Default;
			if(!getSpring(ref spring))
				{return null;}
			return spring;
		}	

		virtual public bool getSpring(ref NxSpringDesc spring)
			{return wrapper_RevoluteJoint_getSpring(nxJointPtr,spring);}

		virtual public float getAngle()
			{return wrapper_RevoluteJoint_getAngle(nxJointPtr);}

		virtual public float getVelocity()
			{return wrapper_RevoluteJoint_getVelocity(nxJointPtr);}
			
		virtual public void setFlags(uint flags)
			{wrapper_RevoluteJoint_setFlags(nxJointPtr,flags);}
			
		virtual public uint getFlags()
			{return wrapper_RevoluteJoint_getFlags(nxJointPtr);}

		virtual public void setProjectionMode(NxJointProjectionMode projectionMode)
			{wrapper_RevoluteJoint_setProjectionMode(nxJointPtr,projectionMode);}

		virtual public NxJointProjectionMode getProjectionMode()
			{return wrapper_RevoluteJoint_getProjectionMode(nxJointPtr);}




		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_loadFromDesc(IntPtr joint,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,NxJointLimitPairDesc limit,NxMotorDesc motor,NxSpringDesc spring,float projectionDistance,float projectionAngle,uint flags,NxJointProjectionMode projectionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_saveToDesc(IntPtr joint,ref IntPtr actor_0,ref IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,ref float maxForce,ref float maxTorque,ref IntPtr userData,ref IntPtr name,ref uint jointFlags,NxJointLimitPairDesc limit,NxMotorDesc motor,NxSpringDesc spring,ref float projectionDistance,ref float projectionAngle,ref uint flags,ref NxJointProjectionMode projectionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_setLimits(IntPtr joint,NxJointLimitPairDesc limit);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_RevoluteJoint_getLimits(IntPtr joint,NxJointLimitPairDesc limit);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_setMotor(IntPtr joint,NxMotorDesc motor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_RevoluteJoint_getMotor(IntPtr joint,NxMotorDesc motor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_setSpring(IntPtr joint,NxSpringDesc spring);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_RevoluteJoint_getSpring(IntPtr joint,NxSpringDesc spring);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_RevoluteJoint_getAngle(IntPtr joint);		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_RevoluteJoint_getVelocity(IntPtr joint);		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_setFlags(IntPtr joint,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_RevoluteJoint_getFlags(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_RevoluteJoint_setProjectionMode(IntPtr joint,NxJointProjectionMode projectionMode);		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxJointProjectionMode wrapper_RevoluteJoint_getProjectionMode(IntPtr joint);		
	}
}















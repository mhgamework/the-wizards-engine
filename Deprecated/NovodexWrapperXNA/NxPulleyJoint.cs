//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxPulleyJoint : NxJoint
	{
		public NxPulleyJoint(IntPtr jointPointer) : base(jointPointer)
			{}
			
		public override NxJoint copy(NxActor newActor0,NxActor newActor1)
		{
			NxPulleyJointDesc jointDesc=this.getJointDesc();
			jointDesc.setActors(newActor0,newActor1);
			return ParentScene.createJoint(jointDesc);
		}
			
		new public static NxPulleyJoint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}
			return new NxPulleyJoint(jointPointer);
		}




		public Vector3 Pulley_0
		{
			get{return getPulley_0();}
			set{setPulley_0(value);}
		}
		
		public Vector3 Pulley_1
		{
			get{return getPulley_1();}
			set{setPulley_1(value);}
		}

		public float Distance
		{
			get{return getDistance();}
			set{setDistance(value);}
		}
		
		public float Stiffness
		{
			get{return getStiffness();}
			set{setStiffness(value);}
		}
		
		public float Ratio
		{
			get{return getRatio();}
			set{setRatio(value);}
		}

		public bool FlagIsRigid
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxPulleyJointFlag.NX_PJF_IS_RIGID);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxPulleyJointFlag.NX_PJF_IS_RIGID,value));}
		}
		
		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}

		public NxMotorDesc Motor
		{
			get{return getMotor();}
			set{setMotor(value);}
		}





		virtual public void loadFromDesc(NxPulleyJointDesc jointDesc)
			{wrapper_PulleyJoint_loadFromDesc(nxJointPtr,jointDesc.actor[0].NxActorPtr,jointDesc.actor[1].NxActorPtr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],jointDesc.maxForce,jointDesc.maxTorque,jointDesc.userData,jointDesc.internalNamePtr,jointDesc.jointFlags,ref jointDesc.pulley_0,ref jointDesc.pulley_1,jointDesc.distance,jointDesc.stiffness,jointDesc.ratio,jointDesc.flags,jointDesc.motor);}

		new virtual public NxPulleyJointDesc getJointDesc()
		{
			NxPulleyJointDesc jointDesc=NxPulleyJointDesc.Default;
			saveToDesc(jointDesc);
			return jointDesc;
		}
			
		virtual public void saveToDesc(NxPulleyJointDesc jointDesc)
		{
			IntPtr actor0_ptr=IntPtr.Zero;
			IntPtr actor1_ptr=IntPtr.Zero;
			wrapper_PulleyJoint_saveToDesc(nxJointPtr,ref actor0_ptr,ref actor1_ptr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],ref jointDesc.maxForce,ref jointDesc.maxTorque,ref jointDesc.userData,ref jointDesc.internalNamePtr,ref jointDesc.jointFlags,ref jointDesc.pulley_0,ref jointDesc.pulley_1,ref jointDesc.distance,ref jointDesc.stiffness,ref jointDesc.ratio,ref jointDesc.flags,jointDesc.motor);
			jointDesc.actor[0]=NxActor.createFromPointer(actor0_ptr);
			jointDesc.actor[1]=NxActor.createFromPointer(actor1_ptr);
			jointDesc.name=getName();
		}

		protected override void internalLoadFromDesc(NxJointDesc jointDesc)
			{loadFromDesc((NxPulleyJointDesc)jointDesc);}
			
		protected override NxJointDesc internalGetJointDesc()
			{return (NxPulleyJointDesc)getJointDesc();}







		public Vector3[] getPulleys()
		{
			Vector3[] pulley=new Vector3[2];
			NxPulleyJointDesc jointDesc=getJointDesc();
			pulley[0]=jointDesc.pulley_0;
			pulley[1]=jointDesc.pulley_1;
			return pulley;			
		}
		
		public void setPulleys(Vector3 pulley_0,Vector3 pulley_1)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.pulley_0=pulley_0;
			jointDesc.pulley_1=pulley_1;
		}

		public Vector3 getPulley_1()
			{return getJointDesc().pulley_1;}
				
		public void setPulley_1(Vector3 pulley_1)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.pulley_1=pulley_1;
			loadFromDesc(jointDesc);
		}

		public Vector3 getPulley_0()
			{return getJointDesc().pulley_0;}
				
		public void setPulley_0(Vector3 pulley_0)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.pulley_0=pulley_0;
			loadFromDesc(jointDesc);
		}

		public float getDistance()
			{return getJointDesc().distance;}
				
		public void setDistance(float distance)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.distance=distance;
			loadFromDesc(jointDesc);
		}	

		public float getStiffness()
			{return getJointDesc().stiffness;}
				
		public void setStiffness(float stiffness)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.stiffness=stiffness;
			loadFromDesc(jointDesc);
		}	

		public float getRatio()
			{return getJointDesc().ratio;}
				
		public void setRatio(float ratio)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.ratio=ratio;
			loadFromDesc(jointDesc);
		}	

		public uint getFlags()
			{return getJointDesc().flags;}
			
		public void setFlags(uint flags)
		{
			NxPulleyJointDesc jointDesc=getJointDesc();
			jointDesc.flags=flags;
			loadFromDesc(jointDesc);
		}

		public void setMotor(NxMotorDesc motor)
			{wrapper_PulleyJoint_setMotor(nxJointPtr,motor);}

		virtual public bool getMotor(ref NxMotorDesc motor)
			{return wrapper_PulleyJoint_getMotor(nxJointPtr,motor);}

		virtual public NxMotorDesc getMotor()
		{
			NxMotorDesc motor=NxMotorDesc.Default;
			if(!getMotor(ref motor))
				{return null;}
			return motor;
		}


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PulleyJoint_loadFromDesc(IntPtr joint,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,ref Vector3 pulley_0,ref Vector3 pulley_1,float distance,float stiffness,float ratio,uint flags,NxMotorDesc motor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PulleyJoint_saveToDesc(IntPtr joint,ref IntPtr actor_0,ref IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,ref float maxForce,ref float maxTorque,ref IntPtr userData,ref IntPtr name,ref uint jointFlags,ref Vector3 pulley_0,ref Vector3 pulley_1,ref float distance,ref float stiffness,ref float ratio,ref uint flags,NxMotorDesc motor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PulleyJoint_setMotor(IntPtr joint,NxMotorDesc motor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_PulleyJoint_getMotor(IntPtr joint,NxMotorDesc motor);
	}
}



/*
-	virtual void loadFromDesc(const NxPulleyJointDesc& desc) = 0;
-	virtual void saveToDesc(NxPulleyJointDesc& desc) = 0;
-	virtual void setMotor(const NxMotorDesc &motorDesc) = 0;
-	virtual bool getMotor(NxMotorDesc &motorDesc) = 0;
-	virtual void setFlags(NxU32 flags) = 0;
-	virtual NxU32 getFlags() = 0;
*/
//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxDistanceJoint : NxJoint
	{
		public NxDistanceJoint(IntPtr jointPointer) : base(jointPointer)
			{}

		public override NxJoint copy(NxActor newActor0,NxActor newActor1)
		{
			NxDistanceJointDesc jointDesc=this.getJointDesc();
			jointDesc.setActors(newActor0,newActor1);
			return ParentScene.createJoint(jointDesc);
		}
			
		new public static NxDistanceJoint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}
			return new NxDistanceJoint(jointPointer);
		}






		public float MaxDistance
		{
			get{return getMaxDistance();}
			set{setMaxDistance(value);}
		}

		public float MinDistance
		{
			get{return getMinDistance();}
			set{setMinDistance(value);}
		}
		
		public NxSpringDesc Spring
		{
			get{return getSpring();}
			set{setSpring(value);}
		}

		public bool FlagMinDistanceEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxDistanceJointFlag.NX_DJF_MIN_DISTANCE_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxDistanceJointFlag.NX_DJF_MIN_DISTANCE_ENABLED,value));}
		}		

		public bool FlagMaxDistanceEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxDistanceJointFlag.NX_DJF_MAX_DISTANCE_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxDistanceJointFlag.NX_DJF_MAX_DISTANCE_ENABLED,value));}
		}			
		
		public bool FlagSpringEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxDistanceJointFlag.NX_DJF_SPRING_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxDistanceJointFlag.NX_DJF_SPRING_ENABLED,value));}
		}	

		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}









		virtual public void loadFromDesc(NxDistanceJointDesc jointDesc)
			{wrapper_DistanceJoint_loadFromDesc(nxJointPtr,jointDesc.actor[0].NxActorPtr,jointDesc.actor[1].NxActorPtr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],jointDesc.maxForce,jointDesc.maxTorque,jointDesc.userData,jointDesc.internalNamePtr,jointDesc.jointFlags,jointDesc.maxDistance,jointDesc.minDistance,jointDesc.spring,jointDesc.flags);}

		new virtual public NxDistanceJointDesc getJointDesc()
		{
			NxDistanceJointDesc jointDesc=NxDistanceJointDesc.Default;
			saveToDesc(jointDesc);
			return jointDesc;
		}
			
		virtual public void saveToDesc(NxDistanceJointDesc jointDesc)
		{
			IntPtr actor0_ptr=IntPtr.Zero;
			IntPtr actor1_ptr=IntPtr.Zero;
			wrapper_DistanceJoint_saveToDesc(nxJointPtr,ref actor0_ptr,ref actor1_ptr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],ref jointDesc.maxForce,ref jointDesc.maxTorque,ref jointDesc.userData,ref jointDesc.internalNamePtr,ref jointDesc.jointFlags,ref jointDesc.maxDistance,ref jointDesc.minDistance,jointDesc.spring,ref jointDesc.flags);
			jointDesc.actor[0]=NxActor.createFromPointer(actor0_ptr);
			jointDesc.actor[1]=NxActor.createFromPointer(actor1_ptr);
			jointDesc.name=getName();
		}
		
		protected override void internalLoadFromDesc(NxJointDesc jointDesc)
			{loadFromDesc((NxDistanceJointDesc)jointDesc);}
					
		protected override NxJointDesc internalGetJointDesc()
			{return (NxDistanceJointDesc)getJointDesc();}



		


		virtual public float getMaxDistance()
			{return getJointDesc().maxDistance;}
		
		//Passing in a negative value will disable it
		virtual public void setMaxDistance(float maxDistance)
		{
			NxDistanceJointDesc jointDesc=getJointDesc();
			jointDesc.maxDistance=maxDistance;
			jointDesc.FlagMaxDistanceEnabled=true;

			if(maxDistance<0)
				{jointDesc.maxDistance=0; jointDesc.FlagMaxDistanceEnabled=false;}

			loadFromDesc(jointDesc);
		}
			
		virtual public float getMinDistance()
			{return getJointDesc().minDistance;}
		
		//Passing in a negative value will disable it
		virtual public void setMinDistance(float minDistance)
		{
			NxDistanceJointDesc jointDesc=getJointDesc();
			jointDesc.minDistance=minDistance;
			jointDesc.FlagMinDistanceEnabled=true;

			if(minDistance<0)
				{jointDesc.minDistance=0; jointDesc.FlagMinDistanceEnabled=false;}

			loadFromDesc(jointDesc);
		}
		
		virtual public NxSpringDesc getSpring()
			{return getJointDesc().spring;}
			
		virtual public void setSpring(NxSpringDesc spring)
		{
			NxDistanceJointDesc jointDesc=getJointDesc();
			jointDesc.spring=spring;
			loadFromDesc(jointDesc);
		}	
			
		virtual public uint getFlags()
			{return getJointDesc().flags;}
			
		virtual public void setFlags(uint flags)
		{
			NxDistanceJointDesc jointDesc=getJointDesc();
			jointDesc.flags=flags;
			loadFromDesc(jointDesc);
		}	





		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_DistanceJoint_loadFromDesc(IntPtr joint,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,float maxDistance,float minDistance,NxSpringDesc spring,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_DistanceJoint_saveToDesc(IntPtr joint,ref IntPtr actor_0,ref IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,ref float maxForce,ref float maxTorque,ref IntPtr userData,ref IntPtr name,ref uint jointFlags,ref float maxDistance,ref float minDistance,NxSpringDesc spring,ref uint flags);
	}
}


//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxPointInPlaneJoint : NxJoint
	{
		public NxPointInPlaneJoint(IntPtr jointPointer) : base(jointPointer)
			{}
			
		public override NxJoint copy(NxActor newActor0,NxActor newActor1)
		{
			NxPointInPlaneJointDesc jointDesc=this.getJointDesc();
			jointDesc.setActors(newActor0,newActor1);
			return ParentScene.createJoint(jointDesc);
		}
			
		new public static NxPointInPlaneJoint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}
			return new NxPointInPlaneJoint(jointPointer);
		}


		virtual public void loadFromDesc(NxPointInPlaneJointDesc jointDesc)
			{wrapper_PointInPlaneJoint_loadFromDesc(nxJointPtr,jointDesc.actor[0].NxActorPtr,jointDesc.actor[1].NxActorPtr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],jointDesc.maxForce,jointDesc.maxTorque,jointDesc.userData,jointDesc.internalNamePtr,jointDesc.jointFlags);}

		new virtual public NxPointInPlaneJointDesc getJointDesc()
		{
			NxPointInPlaneJointDesc jointDesc=NxPointInPlaneJointDesc.Default;
			saveToDesc(jointDesc);
			return jointDesc;
		}
			
		virtual public void saveToDesc(NxPointInPlaneJointDesc jointDesc)
		{
			IntPtr actor0_ptr=IntPtr.Zero;
			IntPtr actor1_ptr=IntPtr.Zero;
			wrapper_PointInPlaneJoint_saveToDesc(nxJointPtr,ref actor0_ptr,ref actor1_ptr,ref jointDesc.localNormal[0],ref jointDesc.localNormal[1],ref jointDesc.localAxis[0],ref jointDesc.localAxis[1],ref jointDesc.localAnchor[0],ref jointDesc.localAnchor[1],ref jointDesc.maxForce,ref jointDesc.maxTorque,ref jointDesc.userData,ref jointDesc.internalNamePtr,ref jointDesc.jointFlags);
			jointDesc.actor[0]=NxActor.createFromPointer(actor0_ptr);
			jointDesc.actor[1]=NxActor.createFromPointer(actor1_ptr);
			jointDesc.name=getName();
		}

		protected override void internalLoadFromDesc(NxJointDesc jointDesc)
			{loadFromDesc((NxPointInPlaneJointDesc)jointDesc);}
			
		protected override NxJointDesc internalGetJointDesc()
			{return (NxPointInPlaneJointDesc)getJointDesc();}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PointInPlaneJoint_loadFromDesc(IntPtr joint,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PointInPlaneJoint_saveToDesc(IntPtr joint,ref IntPtr actor_0,ref IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,ref float maxForce,ref float maxTorque,ref IntPtr userData,ref IntPtr name,ref uint jointFlags);
	}
}
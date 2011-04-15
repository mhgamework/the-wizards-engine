//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;





namespace NovodexWrapper
{
	public class NxD6Joint : NxJoint
	{
		public NxD6Joint(IntPtr jointPointer) : base(jointPointer)
			{}
		
		public override NxJoint copy(NxActor newActor0,NxActor newActor1)
		{
			NxD6JointDesc jointDesc=this.getJointDesc();
			jointDesc.setActors(newActor0,newActor1);
			return ParentScene.createJoint(jointDesc);
		}
		
		new public static NxD6Joint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}
				
			NxD6Joint joint=new NxD6Joint(jointPointer);
			return joint;	
		}







		public NxD6JointMotion XMotion
		{
			get{return getXMotion();}
			set{setXMotion(value);}
		}
		
		public NxD6JointMotion YMotion
		{
			get{return getYMotion();}
			set{setYMotion(value);}
		}
		
		public NxD6JointMotion ZMotion
		{
			get{return getZMotion();}
			set{setZMotion(value);}
		}
		
		public NxD6JointMotion TwistMotion
		{
			get{return getTwistMotion();}
			set{setTwistMotion(value);}
		}
		
		public NxD6JointMotion Swing1Motion
		{
			get{return getSwing1Motion();}
			set{setSwing1Motion(value);}
		}
		
		public NxD6JointMotion Swing2Motion
		{
			get{return getSwing2Motion();}
			set{setSwing2Motion(value);}
		}

		public NxJointLimitSoftDesc LinearLimit
		{
			get{return getLinearLimit();}
			set{setLinearLimit(value);}
		}
		
		public NxJointLimitSoftDesc Swing1Limit
		{
			get{return getSwing1Limit();}
			set{setSwing1Limit(value);}
		}
		
		public NxJointLimitSoftDesc Swing2Limit
		{
			get{return getSwing2Limit();}
			set{setSwing2Limit(value);}
		}
		
		public NxJointLimitSoftPairDesc TwistLimit
		{
			get{return getTwistLimit();}
			set{setTwistLimit(value);}
		}

		public NxJointDriveDesc XDrive
		{
			get{return getXDrive();}
			set{setXDrive(value);}
		}

		public NxJointDriveDesc YDrive
		{
			get{return getYDrive();}
			set{setYDrive(value);}
		}

		public NxJointDriveDesc ZDrive
		{
			get{return getZDrive();}
			set{setZDrive(value);}
		}

		public NxJointDriveDesc SwingDrive
		{
			get{return getSwingDrive();}
			set{setSwingDrive(value);}
		}

		public NxJointDriveDesc TwistDrive
		{
			get{return getTwistDrive();}
			set{setTwistDrive(value);}
		}

		public NxJointDriveDesc SlerpDrive
		{
			get{return getSlerpDrive();}
			set{setSlerpDrive(value);}
		}

		public Vector3 DrivePosition
		{
			get{return getDrivePosition();}
			set{setDrivePosition(value);}
		}

		public NxQuat DriveOrientation
		{
			get{return getDriveOrientation();}
			set{setDriveOrientation(value);}
		}

		public Vector3 DriveLinearVelocity
		{
			get{return getDriveLinearVelocity();}
			set{setDriveLinearVelocity(value);}
		}

		public Vector3 DriveAngularVelocity
		{
			get{return getDriveAngularVelocity();}
			set{setDriveAngularVelocity(value);}
		}

		public NxJointProjectionMode ProjectionMode
		{
			get{return getProjectionMode();}
			set{setProjectionMode(value);}
		}

		public float ProjectionDistance
		{
			get{return getProjectionDistance();}
			set{setProjectionDistance(value);}
		}

		public float ProjectionAngle
		{
			get{return getProjectionAngle();}
			set{setProjectionAngle(value);}
		}

		public float GearRatio
		{
			get{return getGearRatio();}
			set{setGearRatio(value);}
		}

		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}


		public bool FlagGearEnabled
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxD6JointFlag.NX_D6JOINT_GEAR_ENABLED);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxD6JointFlag.NX_D6JOINT_GEAR_ENABLED,value));}
		}

		public bool FlagSlerpDrive
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxD6JointFlag.NX_D6JOINT_SLERP_DRIVE);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxD6JointFlag.NX_D6JOINT_SLERP_DRIVE,value));}
		}




		virtual public void loadFromDesc(NxD6JointDesc jointDesc)
		{
			NxD6JointDesc j=jointDesc;
			wrapper_D6Joint_loadFromDesc(nxJointPtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags,j.xMotion,j.yMotion,j.zMotion,j.swing1Motion,j.swing2Motion,j.twistMotion,ref j.linearLimit,ref j.swing1Limit,ref j.swing2Limit,j.twistLimit,ref j.xDrive,ref j.yDrive,ref j.zDrive,ref j.swingDrive,ref j.twistDrive,ref j.slerpDrive,ref j.drivePosition,ref j.driveOrientation,ref j.driveLinearVelocity,ref j.driveAngularVelocity,j.projectionMode,j.projectionDistance,j.projectionAngle,j.gearRatio,j.flags);
		}

		new virtual public NxD6JointDesc getJointDesc()
		{
			NxD6JointDesc jointDesc=NxD6JointDesc.Default;
			saveToDesc(jointDesc);
			return jointDesc;
		}
			
		virtual public void saveToDesc(NxD6JointDesc jointDesc)
		{
			NxD6JointDesc j=jointDesc;
			IntPtr actor0_ptr=IntPtr.Zero;
			IntPtr actor1_ptr=IntPtr.Zero;
			
			wrapper_D6Joint_saveToDesc(nxJointPtr,ref actor0_ptr,ref actor1_ptr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],ref j.maxForce,ref j.maxTorque,ref j.userData,ref j.internalNamePtr,ref j.jointFlags,ref j.xMotion,ref j.yMotion,ref j.zMotion,ref j.swing1Motion,ref j.swing2Motion,ref j.twistMotion,ref j.linearLimit,ref j.swing1Limit,ref j.swing2Limit,j.twistLimit,ref j.xDrive,ref j.yDrive,ref j.zDrive,ref j.swingDrive,ref j.twistDrive,ref j.slerpDrive,ref j.drivePosition,ref j.driveOrientation,ref j.driveLinearVelocity,ref j.driveAngularVelocity,ref j.projectionMode,ref j.projectionDistance,ref j.projectionAngle,ref j.gearRatio,ref j.flags);
			j.actor[0]=NxActor.createFromPointer(actor0_ptr);
			j.actor[1]=NxActor.createFromPointer(actor1_ptr);
			jointDesc.name=getName();
		}


		
		protected override void internalLoadFromDesc(NxJointDesc jointDesc)
			{loadFromDesc((NxD6JointDesc)jointDesc);}

		protected override NxJointDesc internalGetJointDesc()
			{return (NxD6JointDesc)getJointDesc();}






		public NxD6JointMotion getXMotion()
			{return getJointDesc().xMotion;}
			
		public void setXMotion(NxD6JointMotion xMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.xMotion=xMotion;
			loadFromDesc(jointDesc);
		}
		
		public NxD6JointMotion getYMotion()
			{return getJointDesc().yMotion;}
			
		public void setYMotion(NxD6JointMotion yMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.yMotion=yMotion;
			loadFromDesc(jointDesc);
		}
		
		public NxD6JointMotion getZMotion()
			{return getJointDesc().zMotion;}
			
		public void setZMotion(NxD6JointMotion zMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.zMotion=zMotion;
			loadFromDesc(jointDesc);
		}
		
		public NxD6JointMotion getTwistMotion()
			{return getJointDesc().twistMotion;}
			
		public void setTwistMotion(NxD6JointMotion twistMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.twistMotion=twistMotion;
			loadFromDesc(jointDesc);
		}
				
		public NxD6JointMotion getSwing1Motion()
			{return getJointDesc().swing1Motion;}
			
		public void setSwing1Motion(NxD6JointMotion swing1Motion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.swing1Motion=swing1Motion;
			loadFromDesc(jointDesc);
		}
					
		public NxD6JointMotion getSwing2Motion()
			{return getJointDesc().swing2Motion;}
			
		public void setSwing2Motion(NxD6JointMotion swing2Motion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.swing2Motion=swing2Motion;
			loadFromDesc(jointDesc);
		}

		public NxJointLimitSoftDesc getLinearLimit()
			{return getJointDesc().linearLimit;}
			
		public void setLinearLimit(NxJointLimitSoftDesc linearLimit)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.linearLimit=linearLimit;
			loadFromDesc(jointDesc);					
		}

		public NxJointLimitSoftDesc getSwing1Limit()
			{return getJointDesc().swing1Limit;}
			
		public void setSwing1Limit(NxJointLimitSoftDesc swing1Limit)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.swing1Limit=swing1Limit;
			jointDesc.swing1Motion=NxD6JointMotion.NX_D6JOINT_MOTION_LIMITED;
			loadFromDesc(jointDesc);					
		}
		
		public NxJointLimitSoftDesc getSwing2Limit()
			{return getJointDesc().swing2Limit;}
			
		public void setSwing2Limit(NxJointLimitSoftDesc swing2Limit)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.swing2Limit=swing2Limit;
			jointDesc.swing2Motion=NxD6JointMotion.NX_D6JOINT_MOTION_LIMITED;
			loadFromDesc(jointDesc);					
		}
		
		public NxJointLimitSoftPairDesc getTwistLimit()
			{return getJointDesc().twistLimit;}
			
		public void setTwistLimit(NxJointLimitSoftPairDesc twistLimit)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.twistLimit=twistLimit;
			jointDesc.twistMotion=NxD6JointMotion.NX_D6JOINT_MOTION_LIMITED;
			loadFromDesc(jointDesc);					
		}
		
		public void setAllMovementAndRotationMotions(NxD6JointMotion jointMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.xMotion=jointDesc.yMotion=jointDesc.zMotion=jointDesc.twistMotion=jointDesc.swing1Motion=jointDesc.swing2Motion=jointMotion; 
			loadFromDesc(jointDesc);
		}
		
		public void setAllMovementMotions(NxD6JointMotion jointMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.xMotion=jointDesc.yMotion=jointDesc.zMotion=jointMotion; 
			loadFromDesc(jointDesc);
		}
		
		public void setAllRotationMotions(NxD6JointMotion jointMotion)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.twistMotion=jointDesc.swing1Motion=jointDesc.swing2Motion=jointMotion; 
			loadFromDesc(jointDesc);
		}

		public NxJointDriveDesc getXDrive()
			{return getJointDesc().xDrive;}
			
		public void setXDrive(NxJointDriveDesc xDrive)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.xDrive=xDrive;
			loadFromDesc(jointDesc);					
		}

		public NxJointDriveDesc getYDrive()
			{return getJointDesc().yDrive;}
			
		public void setYDrive(NxJointDriveDesc yDrive)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.yDrive=yDrive;
			loadFromDesc(jointDesc);					
		}
		
		public NxJointDriveDesc getZDrive()
			{return getJointDesc().zDrive;}
			
		public void setZDrive(NxJointDriveDesc zDrive)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.zDrive=zDrive;
			loadFromDesc(jointDesc);					
		}

		public NxJointDriveDesc getSwingDrive()
			{return getJointDesc().swingDrive;}
			
		public void setSwingDrive(NxJointDriveDesc swingDrive)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.swingDrive=swingDrive;
			loadFromDesc(jointDesc);					
		}

		public NxJointDriveDesc getTwistDrive()
			{return getJointDesc().twistDrive;}
			
		public void setTwistDrive(NxJointDriveDesc twistDrive)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.twistDrive=twistDrive;
			loadFromDesc(jointDesc);					
		}

		public NxJointDriveDesc getSlerpDrive()
			{return getJointDesc().slerpDrive;}
			
		public void setSlerpDrive(NxJointDriveDesc slerpDrive)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.slerpDrive=slerpDrive;
			loadFromDesc(jointDesc);					
		}

		public Vector3 getDrivePosition()
			{return getJointDesc().drivePosition;}

		virtual public void setDrivePosition(Vector3 drivePosition)
			{wrapper_D6Joint_setDrivePosition(nxJointPtr,ref drivePosition);}

		public NxQuat getDriveOrientation()
			{return getJointDesc().driveOrientation;}

		virtual public void setDriveOrientation(NxQuat driveOrientation)
			{wrapper_D6Joint_setDriveOrientation(nxJointPtr,ref driveOrientation);}

		public Vector3 getDriveLinearVelocity()
			{return getJointDesc().driveLinearVelocity;}
		
		virtual public void setDriveLinearVelocity(Vector3 driveLinearVelocity)
			{wrapper_D6Joint_setDriveLinearVelocity(nxJointPtr,ref driveLinearVelocity);}

		public Vector3 getDriveAngularVelocity()
			{return getJointDesc().driveAngularVelocity;}

		virtual public void setDriveAngularVelocity(Vector3 driveAngularVelocity)
			{wrapper_D6Joint_setDriveAngularVelocity(nxJointPtr,ref driveAngularVelocity);}

		public NxJointProjectionMode getProjectionMode()
			{return getJointDesc().projectionMode;}
			
		public void setProjectionMode(NxJointProjectionMode projectionMode)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.projectionMode=projectionMode;
			loadFromDesc(jointDesc);					
		}

		public float getProjectionDistance()
			{return getJointDesc().projectionDistance;}
			
		public void setProjectionDistance(float projectionDistance)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.projectionDistance=projectionDistance;
			loadFromDesc(jointDesc);					
		}
		
		public float getProjectionAngle()
			{return getJointDesc().projectionAngle;}
			
		public void setProjectionAngle(float projectionAngle)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.projectionAngle=projectionAngle;
			loadFromDesc(jointDesc);					
		}

		public float getGearRatio()
			{return getJointDesc().gearRatio;}
			
		public void setGearRatio(float gearRatio)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.gearRatio=gearRatio;
			loadFromDesc(jointDesc);					
		}

		public uint getFlags()
			{return getJointDesc().flags;}
			
		public void setFlags(uint flags)
		{
			NxD6JointDesc jointDesc=getJointDesc();
			jointDesc.flags=flags;
			loadFromDesc(jointDesc);					
		}




		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_D6Joint_loadFromDesc(IntPtr joint,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,NxD6JointMotion xMotion,NxD6JointMotion yMotion,NxD6JointMotion zMotion,NxD6JointMotion swing1Motion,NxD6JointMotion swing2Motion,NxD6JointMotion twistMotion,ref NxJointLimitSoftDesc linearLimit,ref NxJointLimitSoftDesc swing1Limit,ref NxJointLimitSoftDesc swing2Limit,NxJointLimitSoftPairDesc twistLimit,ref NxJointDriveDesc xDrive,ref NxJointDriveDesc yDrive,ref NxJointDriveDesc zDrive,ref NxJointDriveDesc swingDrive,ref NxJointDriveDesc twistDrive,ref NxJointDriveDesc slerpDrive,ref Vector3 drivePosition,ref NxQuat driveOrientation,ref Vector3 driveLinearVelocity,ref Vector3 driveAngularVelocity,NxJointProjectionMode projectionMode,float projectionDistance,float projectionAngle,float gearRatio,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_D6Joint_saveToDesc(IntPtr joint,ref IntPtr actor_0,ref IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,ref float maxForce,ref float maxTorque,ref IntPtr userData,ref IntPtr name,ref uint jointFlags,ref NxD6JointMotion xMotion,ref NxD6JointMotion yMotion,ref NxD6JointMotion zMotion,ref NxD6JointMotion swing1Motion,ref NxD6JointMotion swing2Motion,ref NxD6JointMotion twistMotion,ref NxJointLimitSoftDesc linearLimit,ref NxJointLimitSoftDesc swing1Limit,ref NxJointLimitSoftDesc swing2Limit,NxJointLimitSoftPairDesc twistLimit,ref NxJointDriveDesc xDrive,ref NxJointDriveDesc yDrive,ref NxJointDriveDesc zDrive,ref NxJointDriveDesc swingDrive,ref NxJointDriveDesc twistDrive,ref NxJointDriveDesc slerpDrive,ref Vector3 drivePosition,ref NxQuat driveOrientation,ref Vector3 driveLinearVelocity,ref Vector3 driveAngularVelocity,ref NxJointProjectionMode projectionMode,ref float projectionDistance,ref float projectionAngle,ref float gearRatio,ref uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_D6Joint_setDrivePosition(IntPtr joint,ref Vector3 drivePosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_D6Joint_setDriveOrientation(IntPtr joint,ref NxQuat driveOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_D6Joint_setDriveLinearVelocity(IntPtr joint,ref Vector3 driveLinearVelocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_D6Joint_setDriveAngularVelocity(IntPtr joint,ref Vector3 driveAngularVelocity);
	}
}


/*
-	virtual void loadFromDesc(const NxD6JointDesc&) = 0;
-	virtual void saveToDesc(NxD6JointDesc&) = 0;
-	virtual void setDrivePosition(const NxVec3 &position) = 0;
-	virtual void setDriveOrientation(const NxQuat &orientation) = 0; 
-	virtual void setDriveLinearVelocity(const NxVec3 &linVel) = 0;
-	virtual void setDriveAngularVelocity(const NxVec3 &angVel) = 0;
*/





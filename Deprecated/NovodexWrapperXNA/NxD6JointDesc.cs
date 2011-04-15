//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxD6JointDesc : NxJointDesc
	{
		public NxD6JointMotion			xMotion, yMotion, zMotion;					//!< Define the linear degrees of freedom
		public NxD6JointMotion			swing1Motion, swing2Motion, twistMotion;	//!< Define the angular degrees of freedom
		public NxJointLimitSoftDesc		linearLimit;								//!< If some linear DOF are limited, linearLimit defines the characteristics of these limits
		public NxJointLimitSoftDesc		swing1Limit;								//!< If swing1Motion is NX_D6JOINT_MOTION_LIMITED, swing1Limit defines the characteristics of the limit
		public NxJointLimitSoftDesc		swing2Limit;								//!< If swing2Motion is NX_D6JOINT_MOTION_LIMITED, swing2Limit defines the characteristics of the limit
		public NxJointLimitSoftPairDesc	twistLimit;									//!< If twistMotion is NX_D6JOINT_MOTION_LIMITED, twistLimit defines the characteristics of the limit
		public NxJointDriveDesc			xDrive, yDrive, zDrive;						//!< Drive the three linear DOF
		public NxJointDriveDesc			swingDrive, twistDrive;						//!< These drives are used if the flag NX_D6JOINT_SLERP_DRIVE is not set
		public NxJointDriveDesc			slerpDrive;									//!< This drive is used if the flag NX_D6JOINT_SLERP_DRIVE is set
		public Vector3					drivePosition;								//!< If the type of xDrive (yDrive,zDrive) is NX_D6JOINT_DRIVE_POSITION, drivePosition defines the goal position
		public NxQuat					driveOrientation;							//!< If the type of swing1Limit (swing2Limit,twistLimit) is NX_D6JOINT_DRIVE_POSITION, driveOrientation defines the goal orientation
		public Vector3					driveLinearVelocity;						//!< If the type of xDrive (yDrive,zDrive) is NX_D6JOINT_DRIVE_VELOCITY, driveLinearVelocity defines the goal linear velocity
		public Vector3					driveAngularVelocity;						//!< If the type of swing1Limit (swing2Limit,twistLimit) is NX_D6JOINT_DRIVE_VELOCITY, driveAngularVelocity defines the goal angular velocity
		public NxJointProjectionMode	projectionMode;								//!< If projectionMode is NX_JPM_NONE, projection is disabled. If NX_JPM_POINT_MINDIST, bodies are projected to limits leaving an linear error of projectionDistance and an angular error of projectionAngle
		public float					projectionDistance;	
		public float					projectionAngle;	
		public float					gearRatio;									//!< when the flag NX_D6JOINT_GEAR_ENABLED is set, the angular velocity of the second actor is driven towards the angular velocity of the first actor times gearRatio (both w.r.t. their primary axis)
		public uint						flags;										//!< This is a combination of the bits defined by ::NxD6JointFlag 

		public static NxD6JointDesc Default
			{get{return new NxD6JointDesc();}}

		public NxD6JointDesc()
			{setToDefault();}
		
		public override void setToDefault()
		{
			base.setToDefault();
		
			xMotion				= NxD6JointMotion.NX_D6JOINT_MOTION_FREE;
			yMotion				= NxD6JointMotion.NX_D6JOINT_MOTION_FREE;
			zMotion				= NxD6JointMotion.NX_D6JOINT_MOTION_FREE;
			twistMotion			= NxD6JointMotion.NX_D6JOINT_MOTION_FREE;
			swing1Motion		= NxD6JointMotion.NX_D6JOINT_MOTION_FREE;
			swing2Motion		= NxD6JointMotion.NX_D6JOINT_MOTION_FREE;

			linearLimit			= NxJointLimitSoftDesc.Default;
			twistLimit			= new NxJointLimitSoftPairDesc();
			swing1Limit			= NxJointLimitSoftDesc.Default;
			swing2Limit			= NxJointLimitSoftDesc.Default;

			xDrive				= NxJointDriveDesc.Default;
			yDrive				= NxJointDriveDesc.Default;
			zDrive				= NxJointDriveDesc.Default;
			swingDrive			= NxJointDriveDesc.Default;
			twistDrive			= NxJointDriveDesc.Default;
			slerpDrive			= NxJointDriveDesc.Default;

			drivePosition		= new Vector3(0,0,0);
			driveOrientation	= new NxQuat(0,0,0,1);

			driveLinearVelocity	= new Vector3(0,0,0);
			driveAngularVelocity= new Vector3(0,0,0);

			projectionDistance	= 0.1f;
			projectionAngle		= 5.0f * NovodexUtil.DEG_TO_RAD;

			flags				= 0;
			gearRatio			= 1.0f;
		}
		
		
		public void setAllMovementAndRotationMotions(NxD6JointMotion jointMotion)
			{xMotion=yMotion=zMotion=twistMotion=swing1Motion=swing2Motion=jointMotion;}
		
		public void setAllMovementMotions(NxD6JointMotion jointMotion)
			{xMotion=yMotion=zMotion=jointMotion;}
		
		public void setAllRotationMotions(NxD6JointMotion jointMotion)
			{twistMotion=swing1Motion=swing2Motion=jointMotion;}


		public bool FlagGearEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxD6JointFlag.NX_D6JOINT_GEAR_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxD6JointFlag.NX_D6JOINT_GEAR_ENABLED,value);}
		}

		public bool FlagSlerpDrive
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxD6JointFlag.NX_D6JOINT_SLERP_DRIVE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxD6JointFlag.NX_D6JOINT_SLERP_DRIVE,value);}
		}
	}
}








//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxBodyDesc
	{
		public NxMat34		massLocalPose;		  //!< position and orientation of the center of mass
		public Vector3		massSpaceInertia;	  //!< Diagonal mass space inertia tensor in body space (all zeros to let the system compute it)
		public float		mass;				  //!< Mass of body
		public Vector3		linearVelocity;		  //!< Initial linear velocity
		public Vector3		angularVelocity;	  //!< Initial angular velocity
		public float		wakeUpCounter;		  //!< Initial wake-up counter
		public float		linearDamping;		  //!< Linear damping
		public float		angularDamping;		  //!< Angular damping
		public float		maxAngularVelocity;	  //!< Max. allowed angular velocity (negative values to use default)
		public float		CCDMotionThreshold;
		public uint			flags;				  //!< Combination of body flags
		public float		sleepLinearVelocity;  //!< maximum linear velocity at which body can go to sleep. If negative, the global default will be used.
		public float		sleepAngularVelocity; //!< maximum angular velocity at which body can go to sleep.  If negative, the global default will be used.
		public uint			solverIterationCount; //!< solver accuracy setting when dealing with this body.

		public static NxBodyDesc Default
			{get{return new NxBodyDesc();}}
		
		public NxBodyDesc()
			{setToDefault();}

		public NxBodyDesc(NxBodyDesc bodyDesc)
		{
			massLocalPose			= bodyDesc.massLocalPose;
			massSpaceInertia		= bodyDesc.massSpaceInertia;
			linearVelocity			= bodyDesc.linearVelocity;
			angularVelocity			= bodyDesc.angularVelocity;
			wakeUpCounter			= bodyDesc.wakeUpCounter;
			mass					= bodyDesc.mass;
			linearDamping			= bodyDesc.linearDamping;
			angularDamping			= bodyDesc.angularDamping;
			maxAngularVelocity		= bodyDesc.maxAngularVelocity;
			CCDMotionThreshold		= 0;
			flags					= bodyDesc.flags;
			sleepLinearVelocity		= bodyDesc.sleepLinearVelocity;
			sleepAngularVelocity	= bodyDesc.sleepAngularVelocity;
			solverIterationCount	= bodyDesc.solverIterationCount;
		}
		
		public void setToDefault()
		{
			massLocalPose			= NxMat34.Identity;
			massSpaceInertia		= new Vector3(0,0,0);
			linearVelocity			= new Vector3(0,0,0);
			angularVelocity			= new Vector3(0,0,0);
			wakeUpCounter			= 20.0f*0.02f;
			mass					= 0.0f;
			linearDamping			= 0.0f;
			angularDamping			= 0.05f;
			maxAngularVelocity		= -1.0f;
			flags					= (uint)NxBodyFlag.NX_BF_VISUALIZATION;
			sleepLinearVelocity		= -1.0f;
			sleepAngularVelocity	= -1.0f;
			solverIterationCount	= 4;
		}
		
		unsafe public IntPtr getAddress()
		{
			fixed(void* x=&massLocalPose.M.M11)
				{return new IntPtr(x);}
		}


		public bool FlagDisableGravity
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_DISABLE_GRAVITY);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_DISABLE_GRAVITY,value);}
		}

		public bool FlagFrozenPosX
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS_X);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS_X,value);}
		}

		public bool FlagFrozenPosY
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS_Y);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS_Y,value);}
		}

		public bool FlagFrozenPosZ
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS_Z);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS_Z,value);}
		}

		public bool FlagFrozenPos
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_POS,value);}
		}

		public bool FlagFrozenRotX
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT_X);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT_X,value);}
		}

		public bool FlagFrozenRotY
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT_Y);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT_Y,value);}
		}

		public bool FlagFrozenRotZ
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT_Z);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT_Z,value);}
		}

		public bool FlagFrozenRot
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN_ROT,value);}
		}

		public bool FlagFrozen
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FROZEN);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FROZEN,value);}
		}

		public bool FlagKinematic
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_KINEMATIC);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_KINEMATIC,value);}
		}

		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_VISUALIZATION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_VISUALIZATION,value);}
		}

		public bool FlagPoseSleepTest
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_POSE_SLEEP_TEST);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_POSE_SLEEP_TEST,value);}
		}

		public bool FlagFilterSleepVel
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxBodyFlag.NX_BF_FILTER_SLEEP_VEL);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxBodyFlag.NX_BF_FILTER_SLEEP_VEL,value);}
		}
	}
}


//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxFluidEmitterDesc
	{
		public NxMat34		relPose;
		public IntPtr		frameActorPtr;
		public uint			type;
		public uint			maxParticles;
		public uint			shape;
		public float		dimensionX;
		public float		dimensionY;
		public Vector3		randomPos;
		public float		randomAngle;
		public float		fluidVelocityMagnitude;
		public float		rate;
		public float		particleLifetime;
		public uint			flags;
		public IntPtr		userDataPtr;
		public IntPtr		namePtr;			//!< Possible debug name. The string is not copied by the SDK, only the pointer is stored.



		public static NxFluidEmitterDesc Default
			{get{return new NxFluidEmitterDesc();}}
		
		public NxFluidEmitterDesc()
			{setToDefault();}
		
		public void setToDefault()
		{
			relPose							= NxMat34.Identity;
			frameActorPtr					= IntPtr.Zero;
			type							= (uint)NxEmitterType.NX_FE_CONSTANT_PRESSURE;
			maxParticles					= 0;
			shape							= (uint)NxEmitterShape.NX_FE_RECTANGULAR;
			dimensionX						= 1.0f;
			dimensionY						= 1.0f;
			randomPos						= new Vector3(0,0,0);
			randomAngle						= 0.0f;
			fluidVelocityMagnitude			= 1.0f;
			rate							= 100.0f;
			particleLifetime				= 0.0f;
			flags							= (uint)(NxFluidEmitterFlag.NX_FEF_ENABLED|NxFluidEmitterFlag.NX_FEF_VISUALIZATION|NxFluidEmitterFlag.NX_FEF_ADD_ACTOR_VELOCITY);
			userDataPtr						= IntPtr.Zero;
			namePtr							= IntPtr.Zero;
		}
		
		unsafe public IntPtr getAddress()
		{
			fixed(void* x=&relPose.M.M11)
				{return new IntPtr(x);}
		}



		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidEmitterFlag.NX_FEF_VISUALIZATION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidEmitterFlag.NX_FEF_VISUALIZATION,value);}
		}

		public bool FlagBrokenActorRef
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidEmitterFlag.NX_FEF_BROKEN_ACTOR_REF);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidEmitterFlag.NX_FEF_BROKEN_ACTOR_REF,value);}
		}

		public bool FlagForceOnActor
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidEmitterFlag.NX_FEF_FORCE_ON_ACTOR);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidEmitterFlag.NX_FEF_FORCE_ON_ACTOR,value);}
		}

		public bool FlagAddActorVelocity
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidEmitterFlag.NX_FEF_ADD_ACTOR_VELOCITY);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidEmitterFlag.NX_FEF_ADD_ACTOR_VELOCITY,value);}
		}

		public bool FlagEnabled
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidEmitterFlag.NX_FEF_ENABLED);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidEmitterFlag.NX_FEF_ENABLED,value);}
		}
	}
}


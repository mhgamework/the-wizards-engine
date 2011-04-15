//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSceneDesc
	{
		public Vector3				gravity;				//!< Gravity vector
		public float				maxTimestep;			//!< Maximum integration time step size
		public uint					maxIter;				//!< Maximum number of substeps to take
		public NxTimeStepMethod		timeStepMethod;			//!< Integration method, see ::NxTimeStepMethod
		public NxBounds3			maxBounds;				//!< Max scene bounds, if available
		public NxSceneLimits		limits;					//!< Expected scene limits (or NULL)
		public NxSimulationType		simType;
		public NxHwSceneType		hwSceneType;
		public NxHwPipelineSpec		pipelineSpec;
		public bool					groundPlane;			//!< Enable/disable default ground plane
		public bool					boundsPlanes;			//!< Enable/disable 6 planes around maxBounds (if available)
		public uint 				flags;
		public uint					internalThreadCount;
		public uint					backgroundThreadCount;
		public uint					threadMask;
		public uint					backgroundThreadMask;
		public IntPtr				userData;
		public NxUserNotify			userNotify;
		public NxUserTriggerReport  userTriggerReport;
		public NxUserContactReport  userContactReport;
		public NxUserScheduler		customScheduler;


		public static NxSceneDesc Default
			{get{return new NxSceneDesc();}}

		public NxSceneDesc()
			{setToDefault();}

		//Pass in negative values, null values to use default NxSceneDesc values
		public NxSceneDesc(Vector3 gravity,float maxTimeStep,int maxIter,NxTimeStepMethod timeStepMethod,NxBounds3 maxBounds,NxSceneLimits limits,NxSimulationType simType,NxHwSceneType hwSceneType,NxHwPipelineSpec pipelineSpec,bool groundPlane,bool boundsPlanes,NxUserNotify userNotify,NxUserTriggerReport userTriggerReport,NxUserContactReport userContactReport,NxUserScheduler customScheduler)
		{
			setToDefault();
			this.gravity=gravity;
			this.maxTimestep=maxTimeStep;
			this.maxIter=(uint)maxIter;
			this.timeStepMethod=timeStepMethod;
			this.maxBounds=maxBounds;
			this.limits=limits;
			this.simType=simType;
			this.hwSceneType=hwSceneType;
			this.pipelineSpec=pipelineSpec;
			this.groundPlane=groundPlane;
			this.boundsPlanes=boundsPlanes;
			this.userNotify=userNotify;
			this.userTriggerReport=userTriggerReport;
			this.userContactReport=userContactReport;
			this.customScheduler=customScheduler;
		}
		
		public void setToDefault()
		{
			gravity					= new Vector3(0,0,0);
			maxTimestep				= 1.0f/60.0f;
			maxIter					= 8;
			timeStepMethod			= NxTimeStepMethod.NX_TIMESTEP_FIXED;
			maxBounds				= null;
			limits					= null;
			simType					= NxSimulationType.NX_SIMULATION_SW;
			hwSceneType				= NxHwSceneType.NX_HW_SCENE_TYPE_RB;
			pipelineSpec			= NxHwPipelineSpec.NX_HW_PIPELINE_FULL;
			groundPlane				= false;
			boundsPlanes			= false;
			userData				= IntPtr.Zero;

			flags					= (uint)NxSceneFlags.NX_SF_SIMULATE_SEPARATE_THREAD;
			internalThreadCount		= 0;
			backgroundThreadCount	= 0;

			//allocate every other hardware thread(2 threads per core...), 
			//excluding the first core(for main simulation thread)
			threadMask				= 0x55555554;
			backgroundThreadMask	= 0x55555554;
			
			userNotify				= null;
			userTriggerReport		= null;
			userContactReport		= null;
			customScheduler			= null;
		}



		public bool FlagDisableSSE
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSceneFlags.NX_SF_DISABLE_SSE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSceneFlags.NX_SF_DISABLE_SSE,value);}
		}

		public bool FlagDisableCollisions
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSceneFlags.NX_SF_DISABLE_COLLISIONS);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSceneFlags.NX_SF_DISABLE_COLLISIONS,value);}
		}

		public bool FlagSimulateSeparateThread
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSceneFlags.NX_SF_SIMULATE_SEPARATE_THREAD);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSceneFlags.NX_SF_SIMULATE_SEPARATE_THREAD,value);}
		}

		public bool FlagEnableMultiThread
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxSceneFlags.NX_SF_ENABLE_MULTITHREAD);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxSceneFlags.NX_SF_ENABLE_MULTITHREAD,value);}
		}
	}
}



//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Explicit)]
	public class NxFluidDesc
	{
		[FieldOffset(  0)]private uint			padding;	//To fill the space of the emmitters array
		[FieldOffset( 16)]public NxParticleData	initialParticleData;			//[16]
		[FieldOffset( 68)]public uint			maxParticles;					//[68]
		[FieldOffset( 72)]public float			restParticlesPerMeter;			//[72]
		[FieldOffset( 76)]public float			restDensity;					//[76]
		[FieldOffset( 80)]public float			kernelRadiusMultiplier;			//[80]
		[FieldOffset( 84)]public float			motionLimitMultiplier;			//[84]
		[FieldOffset( 88)]public uint			packetSizeMultiplier;			//[88]
		[FieldOffset( 92)]public float			stiffness;						//[92]
		[FieldOffset( 96)]public float			viscosity;						//[96]
		[FieldOffset(100)]public float			damping;						//[100]
		[FieldOffset(104)]public Vector3		externalAcceleration;			//[104]
		[FieldOffset(116)]public float			staticCollisionRestitution;		//[116]
		[FieldOffset(120)]public float			staticCollisionAdhesion;		//[120]
		[FieldOffset(124)]public float			dynamicCollisionRestitution;	//[124]
		[FieldOffset(128)]public float			dynamicCollisionAdhesion;		//[128]
		[FieldOffset(132)]public uint			simulationMethod;				//[132]
		[FieldOffset(136)]public uint			collisionMethod;				//[136]
		[FieldOffset(140)]public NxParticleData	particlesWriteData;				//[140]
		[FieldOffset(192)]public uint			flags;							//[192]
		[FieldOffset(196)]public IntPtr			userData;						//[196]			//!< Will be copied to NxFluid::userData
		[FieldOffset(200)]public IntPtr			namePtr;						//[200]			//!< Possible debug name.  The string is not copied by the SDK, only the pointer is stored.
		[FieldOffset(204)]public ArrayList		emitterDescList;
		[FieldOffset(208)]public string			name;
		
		

		public static NxFluidDesc Default
			{get{return new NxFluidDesc();}}
		
		public NxFluidDesc()
			{setToDefault();}
		
		public void setToDefault()
		{
			initialParticleData				= NxParticleData.Default;
			maxParticles					= 0xFFFF;
			restParticlesPerMeter			= 50.0f;
			restDensity						= 1000.0f;
			kernelRadiusMultiplier			= 1.2f;
			motionLimitMultiplier			= 3.0f * kernelRadiusMultiplier;
			packetSizeMultiplier			= 256;
			stiffness						= 20.0f;
			viscosity						= 6.0f;
			damping							= 0.0f;
			externalAcceleration			= new Vector3(0,0,0);
			staticCollisionRestitution		= 1.0f;
			staticCollisionAdhesion			= 0.05f;
			dynamicCollisionRestitution		= 1.0f;
			dynamicCollisionAdhesion		= 0.5f;
			simulationMethod				= (uint)NxFluidSimulationMethod.NX_F_NO_PARTICLE_INTERACTION;
			collisionMethod					= ((uint)NxFluidCollisionMethod.NX_F_STATIC)|((uint)NxFluidCollisionMethod.NX_F_DYNAMIC);

			particlesWriteData				= NxParticleData.Default;
			flags							= (uint)NxFluidFlag.NX_FF_VISUALIZATION;
			userData						= IntPtr.Zero;
			namePtr							= IntPtr.Zero;

			emitterDescList					= new ArrayList();
			name							= null;
		}
		
		
		

		public void addEmitterDesc(NxFluidEmitterDesc emitterDesc)
			{emitterDescList.Add(emitterDesc);}
			
		public int numEmitters()
			{return emitterDescList.Count;}

		public IntPtr[] getEmittersPtrs()
		{
			IntPtr[] emittersPtrs=new IntPtr[numEmitters()];
			for(int i=0;i<emitterDescList.Count;i++)
				{emittersPtrs[i]=((NxFluidEmitterDesc)emitterDescList[i]).getAddress();}
			return emittersPtrs;
		}
		
		unsafe public IntPtr getAddress()
		{
			fixed(void* x=&padding)
				{return new IntPtr(x);}
		}





		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidFlag.NX_FF_VISUALIZATION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidFlag.NX_FF_VISUALIZATION,value);}
		}

		public bool FlagDisableGravity
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxFluidFlag.NX_FF_DISABLE_GRAVITY);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxFluidFlag.NX_FF_DISABLE_GRAVITY,value);}
		}

		public bool FlagCollisionMethodStatic
		{
			get{return NovodexUtil.areBitsSet(collisionMethod,(uint)NxFluidCollisionMethod.NX_F_STATIC);}
			set{collisionMethod=NovodexUtil.setBits(flags,(uint)NxFluidCollisionMethod.NX_F_STATIC,value);}
		}

		public bool FlagCollisionMethodDynamic
		{
			get{return NovodexUtil.areBitsSet(collisionMethod,(uint)NxFluidCollisionMethod.NX_F_DYNAMIC);}
			set{collisionMethod=NovodexUtil.setBits(flags,(uint)NxFluidCollisionMethod.NX_F_DYNAMIC,value);}
		}

		public bool FlagSimulationMethodSPH
		{
			get{return NovodexUtil.areBitsSet(simulationMethod,(uint)NxFluidSimulationMethod.NX_F_SPH);}
			set{simulationMethod=NovodexUtil.setBits(flags,(uint)NxFluidSimulationMethod.NX_F_SPH,value);}
		}

		public bool FlagSimulationMethodNoParticleInteraction
		{
			get{return NovodexUtil.areBitsSet(simulationMethod,(uint)NxFluidSimulationMethod.NX_F_NO_PARTICLE_INTERACTION);}
			set{simulationMethod=NovodexUtil.setBits(flags,(uint)NxFluidSimulationMethod.NX_F_NO_PARTICLE_INTERACTION,value);}
		}

		public bool FlagSimulationMethodMixedMode
		{
			get{return NovodexUtil.areBitsSet(simulationMethod,(uint)NxFluidSimulationMethod.NX_F_MIXED_MODE);}
			set{simulationMethod=NovodexUtil.setBits(flags,(uint)NxFluidSimulationMethod.NX_F_MIXED_MODE,value);}
		}

	}
}


//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	public class NxFluid
	{
		protected IntPtr nxFluidPtr;

		public NxFluid(IntPtr fluidPointer)
			{nxFluidPtr=fluidPointer;}

		public static NxFluid createFromPointer(IntPtr fluidPointer)
		{
			if(fluidPointer==IntPtr.Zero)
				{return null;}
			return new NxFluid(fluidPointer);
		}

		virtual public void internalBeforeRelease()
			{setName(null);}	//This frees the unmanaged memory for the name

		virtual public void internalAfterRelease()
			{nxFluidPtr=IntPtr.Zero;}
		
		public IntPtr UserData
		{
			get{return wrapper_Fluid_getUserData(nxFluidPtr);}
			set{wrapper_Fluid_setUserData(nxFluidPtr,value);}
		}

		public IntPtr NxFluidPtr
			{get{return nxFluidPtr;}}
			
		public string Name
		{
			get{return getName();}
			set{setName(value);}
		}

		public bool FlagVisualization
		{
			get{return getFlag(NxFluidFlag.NX_FF_VISUALIZATION);}
			set{setFlag(NxFluidFlag.NX_FF_VISUALIZATION,value);}
		}

		public bool FlagDisableGravity
		{
			get{return getFlag(NxFluidFlag.NX_FF_DISABLE_GRAVITY);}
			set{setFlag(NxFluidFlag.NX_FF_DISABLE_GRAVITY,value);}
		}

		public float Stiffness
		{
			get{return getStiffness();}
			set{setStiffness(value);}
		}

		public float Viscosity
		{
			get{return getViscosity();}
			set{setViscosity(value);}
		}

		public float Damping
		{
			get{return getDamping();}
			set{setDamping(value);}
		}

		public NxVec3 ExternalAcceleration
		{
			get{return getExternalAcceleration();}
			set{setExternalAcceleration(value);}
		}

		public float StaticCollisionRestitution
		{
			get{return getStaticCollisionRestitution();}
			set{setStaticCollisionRestitution(value);}
		}

		public float StaticCollisionAdhesion
		{
			get{return getStaticCollisionAdhesion();}
			set{setStaticCollisionAdhesion(value);}
		}

		public float DynamicCollisionRestitution
		{
			get{return getDynamicCollisionRestitution();}
			set{setDynamicCollisionRestitution(value);}
		}

		public float DynamicCollisionAdhesion
		{
			get{return getDynamicCollisionAdhesion();}
			set{setDynamicCollisionAdhesion(value);}
		}

		public uint MaxParticles
			{get{return getMaxParticles();}}

		public float KernelRadiusMultiplier
			{get{return getKernelRadiusMultiplier();}}

		public float MotionLimitMultiplier
			{get{return getMotionLimitMultiplier();}}

		public uint PacketSizeMultiplier
			{get{return getPacketSizeMultiplier();}}

		public float RestParticlesPerMeter
			{get{return getRestParticlesPerMeter();}}

		public float RestDensity
			{get{return getRestDensity();}}

		public float RestParticleDistance
			{get{return getRestParticleDistance();}}

		public float ParticleMass
			{get{return getParticleMass();}}

		public uint SimulationMethod
			{get{return getSimulationMethod();}}

		public uint CollisionMethod
			{get{return getCollisionMethod();}}

		public NxBounds3 WorldBounds
			{get{return getWorldBounds();}}

		public NxParticleData ParticlesData
		{
			get{return getParticlesData();}
			set{setParticlesData(value);}
		}
			







		virtual public void setName(String name)
			{wrapper_Fluid_setName(nxFluidPtr,name);}		
		
		virtual public string getName()
			{return wrapper_Fluid_getName(nxFluidPtr);}		

		virtual public NxFluidEmitter createEmitter(NxFluidEmitterDesc emitterDesc)
			{return NxFluidEmitter.createFromPointer(wrapper_Fluid_createEmitter(nxFluidPtr,emitterDesc));}

		virtual public void releaseEmitter(NxFluidEmitter emitter)
		{
			emitter.internalBeforeRelease();
			wrapper_Fluid_releaseEmitter(nxFluidPtr,emitter.NxFluidEmitterPtr);
			emitter.internalAfterRelease();
		}

		virtual public int getNbEmitters()
			{return wrapper_Fluid_getNbEmitters(nxFluidPtr);}

		virtual public NxFluidEmitter[] getEmitters()
		{
			int numEmitters=getNbEmitters();
			NxFluidEmitter[] emitterArray=new NxFluidEmitter[numEmitters];

			IntPtr emittersPointer=wrapper_Fluid_getEmitters(nxFluidPtr);
			unsafe
			{
				int* p=(int*)emittersPointer.ToPointer();
				for(int i=0;i<numEmitters;i++)
					{emitterArray[i]=NxFluidEmitter.createFromPointer(new IntPtr(p[i]));}
			}
			
			return emitterArray;
		}

		virtual public void addParticles(NxParticleData particleData)
			{wrapper_Fluid_addParticles(nxFluidPtr,ref particleData);}




		virtual public float getStiffness()
			{return wrapper_Fluid_getStiffness(nxFluidPtr);}

		virtual public void setStiffness(float stiffness)
			{wrapper_Fluid_setStiffness(nxFluidPtr,stiffness);}

		virtual public float getViscosity()
			{return wrapper_Fluid_getViscosity(nxFluidPtr);}

		virtual public void setViscosity(float viscosity)
			{wrapper_Fluid_setViscosity(nxFluidPtr,viscosity);}

		virtual public float getDamping()
			{return wrapper_Fluid_getDamping(nxFluidPtr);}

		virtual public void setDamping(float damping)
			{wrapper_Fluid_setDamping(nxFluidPtr,damping);}

		virtual public NxVec3 getExternalAcceleration()
		{
			NxVec3 acceleration=new NxVec3(0,0,0);
			wrapper_Fluid_getExternalAcceleration(nxFluidPtr,ref acceleration);
			return acceleration;
		}

		virtual public void setExternalAcceleration(NxVec3 acceleration)
			{wrapper_Fluid_setExternalAcceleration(nxFluidPtr,ref acceleration);}

		virtual public float getStaticCollisionRestitution()
			{return wrapper_Fluid_getStaticCollisionRestitution(nxFluidPtr);}

		virtual public void setStaticCollisionRestitution(float rest)
			{wrapper_Fluid_setStaticCollisionRestitution(nxFluidPtr,rest);}

		virtual public float getStaticCollisionAdhesion()
			{return wrapper_Fluid_getStaticCollisionAdhesion(nxFluidPtr);}

		virtual public void setStaticCollisionAdhesion(float adhesion)
			{wrapper_Fluid_setStaticCollisionAdhesion(nxFluidPtr,adhesion);}

		virtual public float getDynamicCollisionRestitution()
			{return wrapper_Fluid_getDynamicCollisionRestitution(nxFluidPtr);}

		virtual public void setDynamicCollisionRestitution(float rest)
			{wrapper_Fluid_setDynamicCollisionRestitution(nxFluidPtr,rest);}

		virtual public float getDynamicCollisionAdhesion()
			{return wrapper_Fluid_getDynamicCollisionAdhesion(nxFluidPtr);}

		virtual public void setDynamicCollisionAdhesion(float adhesion)
			{wrapper_Fluid_setDynamicCollisionAdhesion(nxFluidPtr,adhesion);}

		virtual public bool getFlag(NxFluidFlag flag)
			{return wrapper_Fluid_getFlag(nxFluidPtr,flag);}

		virtual public void setFlag(NxFluidFlag flag,bool val)
			{wrapper_Fluid_setFlag(nxFluidPtr,flag,val);}

		virtual public uint getMaxParticles()
			{return wrapper_Fluid_getMaxParticles(nxFluidPtr);}

		virtual public float getKernelRadiusMultiplier()
			{return wrapper_Fluid_getKernelRadiusMultiplier(nxFluidPtr);}

		virtual public float getMotionLimitMultiplier()
			{return wrapper_Fluid_getMotionLimitMultiplier(nxFluidPtr);}

		virtual public uint getPacketSizeMultiplier()
			{return wrapper_Fluid_getPacketSizeMultiplier(nxFluidPtr);}

		virtual public float getRestParticlesPerMeter()
			{return wrapper_Fluid_getRestParticlesPerMeter(nxFluidPtr);}

		virtual public float getRestDensity()
			{return wrapper_Fluid_getRestDensity(nxFluidPtr);}

		virtual public float getRestParticleDistance()
			{return wrapper_Fluid_getRestParticleDistance(nxFluidPtr);}

		virtual public float getParticleMass()
			{return wrapper_Fluid_getParticleMass(nxFluidPtr);}

		virtual public uint getSimulationMethod()
			{return wrapper_Fluid_getSimulationMethod(nxFluidPtr);}

		virtual public uint getCollisionMethod()
			{return wrapper_Fluid_getCollisionMethod(nxFluidPtr);}

		virtual public NxBounds3 getWorldBounds()
		{
			NxBounds3 bounds=new NxBounds3();
			wrapper_Fluid_getWorldBounds(nxFluidPtr,bounds);
			return bounds;
		}

		virtual public void setParticlesData(NxParticleData particleData)
			{wrapper_Fluid_setParticlesWriteData(nxFluidPtr,ref particleData);}

		virtual public NxParticleData getParticlesData()
		{
			NxParticleData particleData=NxParticleData.Default;
			wrapper_Fluid_getParticlesWriteData(nxFluidPtr,ref particleData);
			return particleData;
		}

		virtual public bool loadFromDesc(NxFluidDesc fluidDesc)
		{
			IntPtr[] emittersPtrs=fluidDesc.getEmittersPtrs();
			return wrapper_Fluid_loadFromDescUsingEmitterDescArray(nxFluidPtr,fluidDesc.getAddress(),emittersPtrs.Length,emittersPtrs);
		}

		virtual public bool	saveToDesc(NxFluidDesc fluidDesc)
		{
			bool retValue=wrapper_Fluid_saveToDesc(nxFluidPtr,fluidDesc.getAddress());
			fluidDesc.name=getName();
			return retValue;
		}
		
		virtual public NxFluidDesc getFluidDesc(bool getEmitters)
		{
			NxFluidDesc fluidDesc=NxFluidDesc.Default;
			saveToDesc(fluidDesc);
			
			if(getEmitters)
				{saveEmittersToFluidDesc(fluidDesc);}

			return fluidDesc;
		}

		virtual public bool saveEmittersToFluidDesc(NxFluidDesc fluidDesc)
		{
			int numEmitters=0;
			bool retValue=wrapper_Fluid_saveEmittersToFluidDesc_start(nxFluidPtr,ref numEmitters);
			for(int i=0;i<numEmitters;i++)
			{
				NxFluidEmitterDesc fluidEmitterDesc=NxFluidEmitterDesc.Default;
				IntPtr fluidEmitterDescPtr=wrapper_Fluid_saveEmittersToFluidDesc_getEmitterByIndex(i);
				Marshal.PtrToStructure(fluidEmitterDescPtr,fluidEmitterDesc);
				fluidDesc.addEmitterDesc(fluidEmitterDesc);
			}
			
			wrapper_Fluid_saveEmittersToFluidDesc_end();
			return retValue;
		}





		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setUserData(IntPtr fluid,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Fluid_getUserData(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setName(IntPtr fluid,string name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern string wrapper_Fluid_getName(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Fluid_createEmitter(IntPtr fluid,NxFluidEmitterDesc emitterDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_releaseEmitter(IntPtr fluid,IntPtr emitterPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Fluid_getNbEmitters(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Fluid_getEmitters(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_addParticles(IntPtr fluid,ref NxParticleData particleData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_getExternalAcceleration(IntPtr fluid,ref NxVec3 acceleration);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setExternalAcceleration(IntPtr fluid,ref NxVec3 acceleration);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getStiffness(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setStiffness(IntPtr fluid,float stiffness);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getViscosity(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setViscosity(IntPtr fluid,float viscosity);
    
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getDamping(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setDamping(IntPtr fluid,float damping);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getStaticCollisionRestitution(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setStaticCollisionRestitution(IntPtr fluid,float rest);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getStaticCollisionAdhesion(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setStaticCollisionAdhesion(IntPtr fluid,float adhesion);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getDynamicCollisionRestitution(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setDynamicCollisionRestitution(IntPtr fluid,float rest);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getDynamicCollisionAdhesion(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setDynamicCollisionAdhesion(IntPtr fluid,float adhesion);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setFlag(IntPtr fluid,NxFluidFlag flag,bool val);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Fluid_getFlag(IntPtr fluid,NxFluidFlag flag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Fluid_getMaxParticles(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getKernelRadiusMultiplier(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getMotionLimitMultiplier(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Fluid_getPacketSizeMultiplier(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getRestParticlesPerMeter(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getRestDensity(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getRestParticleDistance(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Fluid_getParticleMass(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Fluid_getSimulationMethod(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Fluid_getCollisionMethod(IntPtr fluid);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_getWorldBounds(IntPtr fluid,NxBounds3 bounds);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_setParticlesWriteData(IntPtr fluid,ref NxParticleData particleData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_getParticlesWriteData(IntPtr fluid,ref NxParticleData particleData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Fluid_loadFromDescUsingEmitterDescArray(IntPtr fluid,IntPtr fluidDescPtr,int numEmitters,IntPtr[] emitterDescPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Fluid_saveToDesc(IntPtr fluid,IntPtr fluidDescPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Fluid_saveEmittersToFluidDesc_start(IntPtr fluid,ref int numEmitters);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Fluid_saveEmittersToFluidDesc_getEmitterByIndex(int index);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Fluid_saveEmittersToFluidDesc_end();
	}
}



//crap
//Doesn't have a getScene() [Effects destroy(), ParentScene, getScene()]
#region Stuff
/*
		virtual public void destroy()
		{
			ParentScene.releaseFluid(this);
			nxFluidPtr=IntPtr.Zero;
		}

		public NxFluid ParentScene
			{get{return getScene();}}

		virtual public NxFluid getScene()
		{
			return NxScene.createFromPointer(wrapper_Fluid_getScene(nxFluidPtr));
			return null;
		}
*/
#endregion


/*
-	virtual		NxFluidEmitter*		createEmitter(const NxFluidEmitterDesc& desc)				= 0;
-	virtual		void				releaseEmitter(NxFluidEmitter& emitter)							= 0;
-	virtual		NxU32				getNbEmitters()									const	= 0;
-	virtual		NxFluidEmitter**	getEmitters()									const	= 0;
-	virtual		void 				addParticles(const NxParticleData& pData)						= 0;
-	virtual		void 				setParticlesWriteData(const NxParticleData& pData)			= 0;
-	virtual		NxParticleData 		getParticlesWriteData()							const	= 0;
-	virtual		NxReal				getStiffness()									const	= 0;
-	virtual		void 				setStiffness(NxReal stiff)									= 0;
-	virtual		NxReal				getViscosity()									const	= 0;
-	virtual		void 				setViscosity(NxReal visc)									= 0;
-	virtual		NxReal				getDamping()									const	= 0;
-	virtual		void 				setDamping(NxReal damp)										= 0;
-	virtual		NxVec3				getExternalAcceleration()								const	= 0;
-	virtual		void 				setExternalAcceleration(const NxVec3&acceleration)				= 0;
-	virtual		NxReal				getStaticCollisionRestitution()					const	= 0;
-	virtual		void 				setStaticCollisionRestitution(NxReal rest)					= 0;
-	virtual		NxReal				getStaticCollisionAdhesion()					const	= 0;
-	virtual		void 				setStaticCollisionAdhesion(NxReal adhesion)						= 0;
-	virtual		NxReal				getDynamicCollisionRestitution()				const	= 0;
-	virtual		void 				setDynamicCollisionRestitution(NxReal rest)					= 0;
-	virtual		NxReal				getDynamicCollisionAdhesion()					const	= 0;
-	virtual		void 				setDynamicCollisionAdhesion(NxReal adhesion)						= 0;
-	virtual		void				setFlag(NxFluidFlag flag, bool val)								= 0;
-	virtual		NX_BOOL				getFlag(NxFluidFlag flag)							const	= 0;
-	virtual		NxU32 				getMaxParticles()							const	= 0;
-	virtual		NxReal				getKernelRadiusMultiplier()					const	= 0;
-	virtual		NxReal				getMotionLimitMultiplier()					const	= 0;
-	virtual		NxU32				getPacketSizeMultiplier()					const	= 0;
-	virtual		NxReal				getRestParticlesPerMeter()					const	= 0;
-	virtual		NxReal				getRestDensity()							const	= 0;
-	virtual		NxReal				getRestParticleDistance()					const	= 0;
-	virtual		NxReal				getParticleMass()							const	= 0;
-	virtual		NxU32				getSimulationMethod()						const	= 0;
-	virtual		NxU32				getCollisionMethod()						const	= 0;
-	virtual		void				getWorldBounds(NxBounds3& dest)				const	= 0;
-	virtual		bool				loadFromDesc(const NxFluidDesc& desc)				= 0;
-	virtual		bool				saveToDesc(NxFluidDesc &desc)				const	= 0;
-	virtual		void				setName(const char* name)		= 0;
-	virtual		const char*			getName()			const	= 0;
-	NX_INLINE	bool				saveEmittersToFluidDesc(NxFluidDesc &desc);
*/


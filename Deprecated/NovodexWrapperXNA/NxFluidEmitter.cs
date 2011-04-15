//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;





namespace NovodexWrapper
{
	public class NxFluidEmitter
	{
		protected IntPtr nxFluidEmitterPtr;

		public NxFluidEmitter(IntPtr fluidEmitterPointer)
			{nxFluidEmitterPtr=fluidEmitterPointer;}

		public static NxFluidEmitter createFromPointer(IntPtr fluidEmitterPointer)
		{
			if(fluidEmitterPointer==IntPtr.Zero)
				{return null;}
			return new NxFluidEmitter(fluidEmitterPointer);
		}

		virtual public void internalBeforeRelease()
			{setName(null);}	//This frees the unmanaged memory for the name

		virtual public void internalAfterRelease()
			{nxFluidEmitterPtr=IntPtr.Zero;}
		
		virtual public void destroy()
		{
			ParentFluid.releaseEmitter(this);
			nxFluidEmitterPtr=IntPtr.Zero;
		}

		public IntPtr UserData
		{
			get{return wrapper_FluidEmitter_getUserData(nxFluidEmitterPtr);}
			set{wrapper_FluidEmitter_setUserData(nxFluidEmitterPtr,value);}
		}

		public NxFluid ParentFluid
			{get{return getFluid();}}

		public IntPtr NxFluidEmitterPtr
			{get{return nxFluidEmitterPtr;}}
			
		public string Name
		{
			get{return getName();}
			set{setName(value);}
		}



		public NxMat34 GlobalPose
		{
			get{return getGlobalPose();}
			set{setGlobalPose(value);}
		}

		public NxVec3 GlobalPosition
		{
			get{return getGlobalPosition();}
			set{setGlobalPosition(value);}
		}

		public NxMat33 GlobalOrientation
		{
			get{return getGlobalOrientation();}
			set{setGlobalOrientation(value);}
		}

		public NxMat34 LocalPose
		{
			get{return getLocalPose();}
			set{setLocalPose(value);}
		}

		public NxVec3 LocalPosition
		{
			get{return getLocalPosition();}
			set{setLocalPosition(value);}
		}

		public NxMat33 LocalOrientation
		{
			get{return getLocalOrientation();}
			set{setLocalOrientation(value);}
		}

		public NxActor FrameActor
		{
			get{return getFrameActor();}
			set{setFrameActor(value);}
		}

		public float DimensionX
			{get{return getDimensionX();}}

		public float DimensionY
			{get{return getDimensionY();}}

		public NxVec3 RandomPos
		{
			get{return getRandomPos();}
			set{setRandomPos(value);}
		}

		public float RandomAngle
		{
			get{return getRandomAngle();}
			set{setRandomAngle(value);}
		}

		public float FluidVelocityMagnitude
		{
			get{return getFluidVelocityMagnitude();}
			set{setFluidVelocityMagnitude(value);}
		}

		public float Rate
		{
			get{return getRate();}
			set{setRate(value);}
		}

		public float ParticleLifetime
		{
			get{return getParticleLifetime();}
			set{setParticleLifetime(value);}
		}

		public bool FlagAddActorVelocity
		{
			get{return getFlag(NxFluidEmitterFlag.NX_FEF_ADD_ACTOR_VELOCITY);}
			set{setFlag(NxFluidEmitterFlag.NX_FEF_ADD_ACTOR_VELOCITY,value);}
		}

		public bool FlagBrokenActorReference
		{
			get{return getFlag(NxFluidEmitterFlag.NX_FEF_BROKEN_ACTOR_REF);}
			set{setFlag(NxFluidEmitterFlag.NX_FEF_BROKEN_ACTOR_REF,value);}
		}

		public bool FlagEnabled
		{
			get{return getFlag(NxFluidEmitterFlag.NX_FEF_ENABLED);}
			set{setFlag(NxFluidEmitterFlag.NX_FEF_ENABLED,value);}
		}

		public bool FlagForceOnActor
		{
			get{return getFlag(NxFluidEmitterFlag.NX_FEF_FORCE_ON_ACTOR);}
			set{setFlag(NxFluidEmitterFlag.NX_FEF_FORCE_ON_ACTOR,value);}
		}

		public bool FlagVisualization
		{
			get{return getFlag(NxFluidEmitterFlag.NX_FEF_VISUALIZATION);}
			set{setFlag(NxFluidEmitterFlag.NX_FEF_VISUALIZATION,value);}
		}

		virtual public NxEmitterShape getEmitterShape()
		{
			if(getShape(NxEmitterShape.NX_FE_ELLIPSE))
				{return NxEmitterShape.NX_FE_ELLIPSE;}
			else
				{return NxEmitterShape.NX_FE_RECTANGULAR;}
		}

		virtual public NxEmitterType getEmitterType()
		{
			if(getType(NxEmitterType.NX_FE_CONSTANT_FLOW_RATE))
				{return NxEmitterType.NX_FE_CONSTANT_FLOW_RATE;}
			else
				{return NxEmitterType.NX_FE_CONSTANT_PRESSURE;}
		}



			

		virtual public NxFluid getFluid()
			{return NxFluid.createFromPointer(wrapper_FluidEmitter_getFluid(nxFluidEmitterPtr));}

		virtual public void setName(String name)
			{wrapper_FluidEmitter_setName(nxFluidEmitterPtr,name);}		
		
		virtual public string getName()
			{return wrapper_FluidEmitter_getName(nxFluidEmitterPtr);}		

		virtual public void setGlobalPose(NxMat34 globalPose)
			{wrapper_FluidEmitter_setGlobalPose(nxFluidEmitterPtr,ref globalPose);}

		virtual public void setGlobalPosition(NxVec3 globalPosition)
			{wrapper_FluidEmitter_setGlobalPosition(nxFluidEmitterPtr,ref globalPosition);}

		virtual public void setGlobalOrientation(NxMat33 globalOrientation)
			{wrapper_FluidEmitter_setGlobalOrientation(nxFluidEmitterPtr,ref globalOrientation);}

		virtual public NxMat34 getGlobalPose()
		{
			NxMat34 matrix;
			wrapper_FluidEmitter_getGlobalPose(nxFluidEmitterPtr,out matrix);
			return matrix;
		}
		
		virtual public NxVec3 getGlobalPosition()
		{
			NxVec3 pos;
			wrapper_FluidEmitter_getGlobalPosition(nxFluidEmitterPtr,out pos);
			return pos;
		}
		
		virtual public NxMat33 getGlobalOrientation()
		{
			NxMat33 mat;
			wrapper_FluidEmitter_getGlobalOrientation(nxFluidEmitterPtr,out mat);
			return mat;
		}

		virtual public void setLocalPose(NxMat34 localPose)
			{wrapper_FluidEmitter_setLocalPose(nxFluidEmitterPtr,ref localPose);}

		virtual public void setLocalPosition(NxVec3 localPosition)
			{wrapper_FluidEmitter_setLocalPosition(nxFluidEmitterPtr,ref localPosition);}

		virtual public void setLocalOrientation(NxMat33 localOrientation)
			{wrapper_FluidEmitter_setLocalOrientation(nxFluidEmitterPtr,ref localOrientation);}

		virtual public NxMat34 getLocalPose()
		{
			NxMat34 mat;
			wrapper_FluidEmitter_getLocalPose(nxFluidEmitterPtr,out mat);
			return mat;
		}

		virtual public NxVec3 getLocalPosition()
		{
			NxVec3 localPosition;
			wrapper_FluidEmitter_getLocalPosition(nxFluidEmitterPtr,out localPosition);
			return localPosition;
		}

		virtual public NxMat33 getLocalOrientation()
		{
			NxMat33 mat;
			wrapper_FluidEmitter_getLocalOrientation(nxFluidEmitterPtr,out mat);
			return mat;
		}

		virtual public void setFrameActor(NxActor actor)
			{wrapper_FluidEmitter_setFrameActor(nxFluidEmitterPtr,actor.NxActorPtr);}

		virtual public NxActor getFrameActor()
		{
			IntPtr actorPtr=wrapper_FluidEmitter_getFrameActor(nxFluidEmitterPtr);
			return NxActor.createFromPointer(actorPtr);
		}

		virtual public float getDimensionX()
			{return wrapper_FluidEmitter_getDimensionX(nxFluidEmitterPtr);}

		virtual public float getDimensionY()
			{return wrapper_FluidEmitter_getDimensionY(nxFluidEmitterPtr);}

		virtual public void setRandomPos(NxVec3 disp)
			{wrapper_FluidEmitter_setRandomPos(nxFluidEmitterPtr,ref disp);}

		virtual public NxVec3 getRandomPos()
		{
			NxVec3 disp=new NxVec3(0,0,0);			
			wrapper_FluidEmitter_setRandomPos(nxFluidEmitterPtr,ref disp);
			return disp;
		}

		virtual public void setRandomAngle(float angle)
			{wrapper_FluidEmitter_setRandomAngle(nxFluidEmitterPtr,angle);}

		virtual public float getRandomAngle()
			{return wrapper_FluidEmitter_getRandomAngle(nxFluidEmitterPtr);}

		virtual public void setFluidVelocityMagnitude(float vel)
			{wrapper_FluidEmitter_setFluidVelocityMagnitude(nxFluidEmitterPtr,vel);}

		virtual public float getFluidVelocityMagnitude()
			{return wrapper_FluidEmitter_getFluidVelocityMagnitude(nxFluidEmitterPtr);}

		virtual public void setRate(float rate)
			{wrapper_FluidEmitter_setRate(nxFluidEmitterPtr,rate);}

		virtual public float getRate()
			{return wrapper_FluidEmitter_getRate(nxFluidEmitterPtr);}

		virtual public void setParticleLifetime(float lifetime)
			{wrapper_FluidEmitter_setParticleLifetime(nxFluidEmitterPtr,lifetime);}

		virtual public float getParticleLifetime()
			{return wrapper_FluidEmitter_getParticleLifetime(nxFluidEmitterPtr);}

		virtual public void setFlag(NxFluidEmitterFlag flag,bool val)
			{wrapper_FluidEmitter_setFlag(nxFluidEmitterPtr,flag,val);}

		virtual public bool getFlag(NxFluidEmitterFlag flag)
			{return wrapper_FluidEmitter_getFlag(nxFluidEmitterPtr,flag);}

		//This should really be named isShape() instead
		virtual public bool getShape(NxEmitterShape shape)
			{return wrapper_FluidEmitter_getShape(nxFluidEmitterPtr,shape);}

		//This should really be named isType() instead
		virtual public bool getType(NxEmitterType type)
			{return wrapper_FluidEmitter_getType(nxFluidEmitterPtr,type);}

		virtual public bool loadFromDesc(NxFluidEmitterDesc fluidEmitterDesc)
			{return wrapper_FluidEmitter_loadFromDesc(nxFluidEmitterPtr,fluidEmitterDesc);}

		virtual public bool saveToDesc(NxFluidEmitterDesc fluidEmitterDesc)
			{return wrapper_FluidEmitter_saveToDesc(nxFluidEmitterPtr,fluidEmitterDesc);}

		virtual public NxFluidEmitterDesc getFluidEmitterDesc()
		{
			NxFluidEmitterDesc fluidEmitterDesc=NxFluidEmitterDesc.Default;
			saveToDesc(fluidEmitterDesc);
			return fluidEmitterDesc;
		}




		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setUserData(IntPtr emitter,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_FluidEmitter_getUserData(IntPtr emitter);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_FluidEmitter_getFluid(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setName(IntPtr emitter,string name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern string wrapper_FluidEmitter_getName(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setGlobalPose(IntPtr actor,ref NxMat34 globalPose);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setGlobalPosition(IntPtr actor,ref NxVec3 globapPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setGlobalOrientation(IntPtr actor,ref NxMat33 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getGlobalPose(IntPtr actor,out NxMat34 globalPose);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getGlobalPosition(IntPtr actor,out NxVec3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getGlobalOrientation(IntPtr actor,out NxMat33 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setLocalPose(IntPtr shape,ref NxMat34 localPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setLocalPosition(IntPtr shape,ref NxVec3 localPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setLocalOrientation(IntPtr shape,ref NxMat33 localOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getLocalPose(IntPtr shape,out NxMat34 localPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getLocalPosition(IntPtr shape,out NxVec3 localPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getLocalOrientation(IntPtr shape,out NxMat33 localOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setFrameActor(IntPtr emitter,IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_FluidEmitter_getFrameActor(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_FluidEmitter_getDimensionX(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_FluidEmitter_getDimensionY(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setRandomPos(IntPtr emitter,ref NxVec3 disp);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_getRandomPos(IntPtr emitter,ref NxVec3 disp);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setRandomAngle(IntPtr emitter,float angle);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_FluidEmitter_getRandomAngle(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setFluidVelocityMagnitude(IntPtr emitter,float vel);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_FluidEmitter_getFluidVelocityMagnitude(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setRate(IntPtr emitter,float rate);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_FluidEmitter_getRate(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setParticleLifetime(IntPtr emitter,float lifetime);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_FluidEmitter_getParticleLifetime(IntPtr emitter);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FluidEmitter_setFlag(IntPtr emitter,NxFluidEmitterFlag flag,bool val);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_FluidEmitter_getFlag(IntPtr emitter,NxFluidEmitterFlag flag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_FluidEmitter_getShape(IntPtr emitter,NxEmitterShape shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_FluidEmitter_getType(IntPtr emitter,NxEmitterType type);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_FluidEmitter_loadFromDesc(IntPtr emitter,NxFluidEmitterDesc fluidEmitterDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_FluidEmitter_saveToDesc(IntPtr emitter,NxFluidEmitterDesc fluidEmitterDesc);
	}
}




/*
-	virtual		NxFluid &	getFluid() const = 0;
-	virtual		void		setGlobalPose(const NxMat34& mat)					= 0;
-	virtual		void		setGlobalPosition(const NxVec3& vec)				= 0;
-	virtual		void		setGlobalOrientation(const NxMat33& mat)			= 0;
-	NX_INLINE	NxMat34		getGlobalPose()							const	{ return getGlobalPoseVal();		}
-	NX_INLINE	NxVec3		getGlobalPosition()						const	{ return getGlobalPositionVal();	}
-	NX_INLINE	NxMat33		getGlobalOrientation()					const	{ return getGlobalOrientationVal();	}
-	virtual		void		setLocalPose(const NxMat34& mat)					= 0;
-	virtual		void		setLocalPosition(const NxVec3& vec)					= 0;
-	virtual		void		setLocalOrientation(const NxMat33& mat)				= 0;
-	NX_INLINE	NxMat34		getLocalPose()							const	{ return getLocalPoseVal();		}
-	NX_INLINE	NxVec3		getLocalPosition()						const	{ return getLocalPositionVal();	}
-	NX_INLINE	NxMat33		getLocalOrientation()					const	{ return getLocalOrientationVal();	}
-	virtual		void 		setFrameActor(NxActor* actor)							= 0;
-	virtual		NxActor * 	getFrameActor()							const	= 0;
-	virtual		NxReal 		getDimensionX()						const	= 0;
-	virtual		NxReal 		getDimensionY()						const	= 0;
-	virtual		void 		setRandomPos(NxVec3 disp)						= 0;
-	virtual		NxVec3 		getRandomPos()						const	= 0;
-	virtual		void 		setRandomAngle(NxReal angle)						= 0;
-	virtual		NxReal 		getRandomAngle()					const	= 0;
-	virtual		void 		setFluidVelocityMagnitude(NxReal vel)			= 0;
-	virtual		NxReal 		getFluidVelocityMagnitude()			const	= 0;
-	virtual		void 		setRate(NxReal rate)								= 0;
-	virtual		NxReal 		getRate()							const	= 0;
-	virtual		void 		setParticleLifetime(NxReal life)					= 0;
-	virtual		NxReal 		getParticleLifetime()				const	= 0;
-	virtual		void		setFlag(NxFluidEmitterFlag flag, bool val)			= 0;
-	virtual		NX_BOOL		getFlag(NxFluidEmitterFlag flag)			const	= 0;
-	virtual		NX_BOOL		getShape(NxEmitterShape shape)			const	= 0;
-	virtual		NX_BOOL		getType(NxEmitterType type)				const	= 0;
-	virtual		bool		loadFromDesc(const NxFluidEmitterDesc& desc)				= 0;
-	virtual		bool		saveToDesc(NxFluidEmitterDesc &desc)				const	= 0;
-	virtual		void		setName(const char* name)		= 0;
-	virtual		const char*	getName()			const	= 0;

-	void*		userData;	//!< user can assign this to whatever, usually to create a 1:1 relationship with a user object.
*/


//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;






namespace NovodexWrapper
{
	abstract public class NxShape
	{
		protected IntPtr nxShapePtr;

		public NxShape(IntPtr shapePointer)
			{nxShapePtr=shapePointer;}

		public static NxShape createFromPointer(IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}

            NxShapeType shapeType;

            try
            {
                shapeType = wrapper_Shape_getType(shapePointer);
            }
            catch (Exception)
            {
                return null;
            }

			
			
                
			if(shapeType==NxShapeType.NX_SHAPE_SPHERE)
				{return new NxSphereShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_CAPSULE)
				{return new NxCapsuleShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_BOX)
				{return new NxBoxShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_PLANE)
				{return new NxPlaneShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_MESH)
				{return new NxTriangleMeshShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_CONVEX)
				{return new NxConvexShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_WHEEL)
				{return new NxWheelShape(shapePointer);}
			else if(shapeType==NxShapeType.NX_SHAPE_HEIGHTFIELD)
				{return new NxHeightFieldShape(shapePointer);}

			return null;
		}

		virtual public void internalBeforeRelease()
			{setName(null);}	//This frees the unmanaged memory for the name
		
		virtual public void internalAfterRelease()
			{nxShapePtr=IntPtr.Zero;}
		
		virtual public void destroy()
		{
			ParentActor.releaseShape(this);
			nxShapePtr=IntPtr.Zero;
		}

		public IntPtr NxShapePtr
			{get{return nxShapePtr;}}

		public NxActor ParentActor
			{get{return getActor();}}

		public string Name
		{
			get{return getName();}
			set{setName(value);}
		}

		public IntPtr UserData
		{
			get{return wrapper_Shape_getUserData(nxShapePtr);}
			set{wrapper_Shape_setUserData(nxShapePtr,value);}
		}

		public IntPtr AppData
		{
			get{return wrapper_Shape_getAppData(nxShapePtr);}
			set{wrapper_Shape_setAppData(nxShapePtr,value);}
		}

		public bool FlagTriggerOnEnter
		{
			get{return getFlag(NxShapeFlag.NX_TRIGGER_ON_ENTER);}
			set{setFlag(NxShapeFlag.NX_TRIGGER_ON_ENTER,value);}
		}

		public bool FlagTriggerOnLeave
		{
			get{return getFlag(NxShapeFlag.NX_TRIGGER_ON_LEAVE);}
			set{setFlag(NxShapeFlag.NX_TRIGGER_ON_LEAVE,value);}
		}
		
		public bool FlagTriggerOnStay
		{
			get{return getFlag(NxShapeFlag.NX_TRIGGER_ON_STAY);}
			set{setFlag(NxShapeFlag.NX_TRIGGER_ON_STAY,value);}
		}

		public bool FlagTriggerEnable
		{
			get{return getFlag(NxShapeFlag.NX_TRIGGER_ENABLE);}
			set{setFlag(NxShapeFlag.NX_TRIGGER_ENABLE,value);}
		}

		public bool FlagVisualization
		{
			get{return getFlag(NxShapeFlag.NX_SF_VISUALIZATION);}
			set{setFlag(NxShapeFlag.NX_SF_VISUALIZATION,value);}
		}

		public bool FlagDisableCollision
		{
			get{return getFlag(NxShapeFlag.NX_SF_DISABLE_COLLISION);}
			set{setFlag(NxShapeFlag.NX_SF_DISABLE_COLLISION,value);}
		}

		public bool FlagFeatureIndices
		{
			get{return getFlag(NxShapeFlag.NX_SF_FEATURE_INDICES);}
			set{setFlag(NxShapeFlag.NX_SF_FEATURE_INDICES,value);}
		}

		public bool FlagDisableRaycasting
		{
			get{return getFlag(NxShapeFlag.NX_SF_DISABLE_RAYCASTING);}
			set{setFlag(NxShapeFlag.NX_SF_DISABLE_RAYCASTING,value);}
		}

		public bool FlagPointContactForce
		{
			get{return getFlag(NxShapeFlag.NX_SF_POINT_CONTACT_FORCE);}
			set{setFlag(NxShapeFlag.NX_SF_POINT_CONTACT_FORCE,value);}
		}
		
		public bool FlagFluidDrain
		{
			get{return getFlag(NxShapeFlag.NX_SF_FLUID_DRAIN);}
			set{setFlag(NxShapeFlag.NX_SF_FLUID_DRAIN,value);}
		}

		public bool FlagFluidDrainInvert
		{
			get{return getFlag(NxShapeFlag.NX_SF_FLUID_DRAIN_INVERT);}
			set{setFlag(NxShapeFlag.NX_SF_FLUID_DRAIN_INVERT,value);}
		}

		public bool FlagFluidDisableCollision
		{
			get{return getFlag(NxShapeFlag.NX_SF_FLUID_DISABLE_COLLISION);}
			set{setFlag(NxShapeFlag.NX_SF_FLUID_DISABLE_COLLISION,value);}
		}

		public bool FlagFluidActorReaction
		{
			get{return getFlag(NxShapeFlag.NX_SF_FLUID_ACTOR_REACTION);}
			set{setFlag(NxShapeFlag.NX_SF_FLUID_ACTOR_REACTION,value);}
		}

		public bool FlagDisableResponse
		{
			get{return getFlag(NxShapeFlag.NX_SF_DISABLE_RESPONSE);}
			set{setFlag(NxShapeFlag.NX_SF_DISABLE_RESPONSE,value);}
		}

		public bool FlagDynamicDynamicCCD
		{
			get{return getFlag(NxShapeFlag.NX_SF_DYNAMIC_DYNAMIC_CCD);}
			set{setFlag(NxShapeFlag.NX_SF_DYNAMIC_DYNAMIC_CCD,value);}
		}

		public ushort MaterialIndex
		{
			get{return getMaterial();}
			set{setMaterial(value);}
		}

		public float SkinWidth
		{
			get{return getSkinWidth();}
			set{setSkinWidth(value);}
		}

		public NxGroupsMask GroupsMask
		{
			get{return getGroupsMask();}
			set{setGroupsMask(value);}
		}

		public ushort Group
		{
			get{return getGroup();}
			set{setGroup(value);}
		}

		public NxBounds3 WorldBounds
			{get{return getWorldBounds();}}

		public Matrix LocalPose
		{
			get{return getLocalPose();}
			set{setLocalPose(value);}
		}

		public Vector3 LocalPosition
		{
			get{return getLocalPosition();}
			set{setLocalPosition(value);}
		}

		public Matrix LocalOrientation
		{
			get{return getLocalOrientation();}
			set{setLocalOrientation(value);}
		}

		public Matrix GlobalPose
		{
			get{return getGlobalPose();}
			set{setGlobalPose(value);}
		}

		public Vector3 GlobalPosition
		{
			get{return getGlobalPosition();}
			set{setGlobalPosition(value);}
		}

		public Matrix GlobalOrientation
		{
			get{return getGlobalOrientation();}
			set{setGlobalOrientation(value);}
		}





		public NxShapeDesc getShapeDesc()
			{return internalGetShapeDesc();}
		
		abstract protected NxShapeDesc internalGetShapeDesc();



		virtual public NxShapeType getShapeType()
			{return wrapper_Shape_getType(nxShapePtr);}

		virtual public bool isShapeType(NxShapeType shapeType)
		{
			if(wrapper_Shape_is(nxShapePtr,shapeType)==IntPtr.Zero)
				{return false;}
			return true;
		}

		virtual public NxActor getActor()
			{return NxActor.createFromPointer(wrapper_Shape_getActor(nxShapePtr));}

		virtual public void setGroup(ushort group)
			{wrapper_Shape_setGroup(nxShapePtr,group);}
			
		virtual public ushort getGroup()
			{return wrapper_Shape_getGroup(nxShapePtr);}
			
		virtual public NxBounds3 getWorldBounds()
		{
			NxBounds3 worldBounds=new NxBounds3();
			wrapper_Shape_getWorldBounds(nxShapePtr,worldBounds);
			return worldBounds;
		}
		
		virtual public void setFlag(NxShapeFlag shapeFlag,bool value)
			{wrapper_Shape_setFlag(nxShapePtr,shapeFlag,value);}

		virtual public bool getFlag(NxShapeFlag shapeFlag)
			{return wrapper_Shape_getFlag(nxShapePtr,shapeFlag);}

		virtual public void setLocalPose(Matrix localPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(localPose);
			wrapper_Shape_setLocalPose(nxShapePtr,ref mat);
		}

		virtual public void setLocalPosition(Vector3 localPosition)
			{wrapper_Shape_setLocalPosition(nxShapePtr,ref localPosition);}

		virtual public void setLocalOrientation(Matrix localOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(localOrientation);
			wrapper_Shape_setLocalOrientation(nxShapePtr,ref mat);
		}
		
		virtual public Matrix getLocalPose()
		{
			NxMat34 mat;
			wrapper_Shape_getLocalPose(nxShapePtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public Vector3 getLocalPosition()
		{
			Vector3 localPosition;
			wrapper_Shape_getLocalPosition(nxShapePtr,out localPosition);
			return localPosition;
		}

		virtual public Matrix getLocalOrientation()
		{
			NxMat34 mat;
			wrapper_Shape_getLocalOrientation(nxShapePtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}








		virtual public void setGlobalPose(Matrix globalPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalPose);
			wrapper_Shape_setGlobalPose(nxShapePtr,ref mat);
		}

		virtual public void setGlobalPosition(Vector3 globalPosition)
			{wrapper_Shape_setGlobalPosition(nxShapePtr,ref globalPosition);}

		virtual public void setGlobalOrientation(Matrix globalOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalOrientation);
			wrapper_Shape_setGlobalOrientation(nxShapePtr,ref mat);
		}
		
		virtual public Matrix getGlobalPose()
		{
			NxMat34 mat;
			wrapper_Shape_getGlobalPose(nxShapePtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public Vector3 getGlobalPosition()
		{
			Vector3 globalPosition;
			wrapper_Shape_getGlobalPosition(nxShapePtr,out globalPosition);
			return globalPosition;
		}

		virtual public Matrix getGlobalOrientation()
		{
			NxMat34 mat;
			wrapper_Shape_getGlobalOrientation(nxShapePtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public void setMaterial(ushort materialIndex)
			{wrapper_Shape_setMaterial(nxShapePtr,materialIndex);}

		virtual public ushort getMaterial()
			{return wrapper_Shape_getMaterial(nxShapePtr);}

		virtual public void setName(String name)
			{wrapper_Shape_setName(nxShapePtr,name);}		
		
		virtual public string getName()
			{return wrapper_Shape_getName(nxShapePtr);}		

		virtual public void setSkinWidth(float skinWidth)
			{wrapper_Shape_setSkinWidth(nxShapePtr,skinWidth);}

		virtual public float getSkinWidth()
			{return wrapper_Shape_getSkinWidth(nxShapePtr);}

		virtual public void setGroupsMask(NxGroupsMask groupsMask)
			{wrapper_Shape_setGroupsMask(nxShapePtr,ref groupsMask);}

		virtual public NxGroupsMask getGroupsMask()
		{
			NxGroupsMask groupsMask;
			wrapper_Shape_getGroupsMask(nxShapePtr,out groupsMask);
			return groupsMask;
		}

		virtual public bool raycast(NxRay worldRay,float maxDist,uint hintFlags,out NxRaycastHit hit,bool firstHit)
			{return wrapper_Shape_raycast(nxShapePtr,worldRay,maxDist,hintFlags,out hit,firstHit);}

		virtual public bool checkOverlapSphere(NxSphere worldSphere)
			{return wrapper_Shape_checkOverlapSphere(nxShapePtr,worldSphere);}

		virtual public bool checkOverlapSphere(NxBox worldBox)
			{return wrapper_Shape_checkOverlapOBB(nxShapePtr,worldBox);}

		virtual public bool checkOverlapAABB(NxBounds3 worldBounds)
			{return wrapper_Shape_checkOverlapAABB(nxShapePtr,worldBounds);}

		virtual public void setCCDSkeleton(NxCCDSkeleton skeleton)
			{wrapper_Shape_setCCDSkeleton(nxShapePtr,skeleton.NxCCDSkeletonPtr);}

		virtual public NxCCDSkeleton getCCDSkeleton()
		{	
			IntPtr skeletonPtr=wrapper_Shape_getCCDSkeleton(nxShapePtr);
			return NxCCDSkeleton.createFromPointer(skeletonPtr);
		}







		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxShapeType wrapper_Shape_getType(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Shape_is(IntPtr shape,NxShapeType shapeType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Shape_getActor(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setGroup(IntPtr shape,ushort group);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_Shape_getGroup(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getWorldBounds(IntPtr shape,NxBounds3 worldBounds);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setFlag(IntPtr shape,NxShapeFlag shapeFlag,bool value);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Shape_getFlag(IntPtr shape,NxShapeFlag shapeFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setLocalPose(IntPtr shape,ref NxMat34 localPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setLocalPosition(IntPtr shape,ref Vector3 localPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setLocalOrientation(IntPtr shape,ref NxMat34 localOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getLocalPose(IntPtr shape,out NxMat34 localPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getLocalPosition(IntPtr shape,out Vector3 localPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getLocalOrientation(IntPtr shape,out NxMat34 localOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setGlobalPose(IntPtr shape,ref NxMat34 globalPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setGlobalPosition(IntPtr shape,ref Vector3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setGlobalOrientation(IntPtr shape,ref NxMat34 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getGlobalPose(IntPtr shape,out NxMat34 globalPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getGlobalPosition(IntPtr shape,out Vector3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getGlobalOrientation(IntPtr shape,out NxMat34 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setMaterial(IntPtr shape,ushort materialIndex);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_Shape_getMaterial(IntPtr shape);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setName(IntPtr shape,string name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern string wrapper_Shape_getName(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setSkinWidth(IntPtr shape,float skinWidth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Shape_getSkinWidth(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setGroupsMask(IntPtr shape,ref NxGroupsMask groupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_getGroupsMask(IntPtr shape,out NxGroupsMask groupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Shape_raycast(IntPtr shape,NxRay worldRay,float maxDist,uint hintFlags,out NxRaycastHit hit,bool firstHit);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setUserData(IntPtr shape,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Shape_getUserData(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setAppData(IntPtr shape,IntPtr appData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Shape_getAppData(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Shape_checkOverlapSphere(IntPtr shape,NxSphere worldSphere);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Shape_checkOverlapOBB(IntPtr shape,NxBox worldBox);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Shape_checkOverlapAABB(IntPtr shape,NxBounds3 worldBounds);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Shape_setCCDSkeleton(IntPtr shape,IntPtr skeleton);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Shape_getCCDSkeleton(IntPtr shape);
	}
}


/*
-	virtual		NxActor&				getActor() const = 0;
-	virtual		void					setGroup(NxCollisionGroup collisionGroup) = 0;
-	virtual		NxCollisionGroup		getGroup() const = 0;
-	virtual		void					getWorldBounds(NxBounds3& dest) const = 0;	
X	virtual		void					setFlag(NxShapeFlag flag, bool value) = 0;
X	virtual		NX_BOOL					getFlag(NxShapeFlag flag) const = 0;
-	virtual		void					setLocalPose(const NxMat34& mat)			= 0;
-	virtual		void					setLocalPosition(const NxVec3& vec)			= 0;
-	virtual		void					setLocalOrientation(const NxMat33& mat)		= 0;
-	virtual		NxMat34					getLocalPose()					const	= 0;
-	virtual		NxVec3					getLocalPosition()				const	= 0;
-	virtual		NxMat33					getLocalOrientation()			const	= 0;
-	virtual		void					setGlobalPose(const NxMat34& mat)			= 0;
-	virtual		void					setGlobalPosition(const NxVec3& vec)		= 0;
-	virtual		void					setGlobalOrientation(const NxMat33& mat)	= 0;
-	virtual		NxMat34					getGlobalPose()					const	= 0;
-	virtual		NxVec3					getGlobalPosition()				const	= 0;
-	virtual		NxMat33					getGlobalOrientation()			const	= 0;
-	virtual		void					setMaterial(NxMaterialIndex matIndex)	= 0;
-	virtual		NxMaterialIndex			getMaterial() const				= 0;
-	virtual		void					setSkinWidth(NxReal skinWidth)	= 0;
-	virtual		NxReal					getSkinWidth() const	= 0;
-	virtual		void					setCCDSkeleton(NxCCDSkeleton *ccdSkel) = 0;
-	virtual		NxCCDSkeleton *			getCCDSkeleton() const = 0;
-	virtual		NxShapeType				getType() const = 0;
-	NX_INLINE	void*					is(NxShapeType type)		{ return (type == getType()) ? (void*)this : NULL;			}
#	NX_INLINE	NxPlaneShape*			isPlane()			{ return (NxPlaneShape*)		is(NX_SHAPE_PLANE);		}
#	NX_INLINE	NxSphereShape*			isSphere()			{ return (NxSphereShape*)		is(NX_SHAPE_SPHERE);	}
#	NX_INLINE	NxBoxShape*				isBox()				{ return (NxBoxShape*)			is(NX_SHAPE_BOX);		}
#	NX_INLINE	NxCapsuleShape*			isCapsule()			{ return (NxCapsuleShape*)		is(NX_SHAPE_CAPSULE);	}
#	NX_INLINE	NxWheelShape*			isWheel()			{ return (NxWheelShape*)		is(NX_SHAPE_WHEEL);		}
#	NX_INLINE	NxConvexShape*			isConvexMesh()		{ return (NxConvexShape*)		is(NX_SHAPE_CONVEX);	}
#	NX_INLINE	NxTriangleMeshShape*	isTriangleMesh()		{ return (NxTriangleMeshShape*)	is(NX_SHAPE_MESH);			}
#	NX_INLINE	NxHeightFieldShape*		isHeightField()			{ return (NxHeightFieldShape*)		is(NX_SHAPE_HEIGHTFIELD);	}
-	virtual		void					setName(const char* name)		= 0;
-	virtual		const char*				getName()			const	= 0;
-	virtual		bool					raycast(const NxRay& worldRay, NxReal maxDist, NxU32 hintFlags, NxRaycastHit& hit, bool firstHit)	const = 0;
-	virtual		bool					checkOverlapSphere(const NxSphere& worldSphere)														const = 0;
-	virtual		bool					checkOverlapOBB(const NxBox& worldBox)																const = 0;
-	virtual		bool					checkOverlapAABB(const NxBounds3& worldBounds)														const = 0;
-	virtual		void					setGroupsMask(const NxGroupsMask& mask)	= 0;
-	virtual		const NxGroupsMask		getGroupsMask()	const = 0;

-	void*		userData;	//!< user can assign this to whatever, usually to create a 1:1 relationship with a user object.
-	void*		appData;
*/




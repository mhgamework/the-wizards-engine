//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	public class NxCloth
	{
		protected IntPtr nxClothPtr;

		public NxCloth(IntPtr clothPointer)
			{nxClothPtr=clothPointer;}

		public static NxCloth createFromPointer(IntPtr clothPointer)
		{
			if(clothPointer==IntPtr.Zero)
				{return null;}
			return new NxCloth(clothPointer);
		}

		virtual public void internalBeforeRelease()
		{
			setName(null);				//This frees the unmanaged memory for the name
			getMeshData().freeData();	//Manually free the meshData which isn't managed memory.
		}

		virtual public void internalAfterRelease()
			{nxClothPtr=IntPtr.Zero;}
		



		public IntPtr NxClothPtr
			{get{return nxClothPtr;}}

		public IntPtr UserData
		{
			get{return wrapper_Cloth_getUserData(nxClothPtr);}
			set{wrapper_Cloth_setUserData(nxClothPtr,value);}
		}
		
		public string Name
		{
			get{return getName();}
			set{setName(value);}
		}




		public NxClothMesh ClothMesh
			{get{return getClothMesh();}}

		public float BendingStiffness
		{
			get{return getBendingStiffness();}
			set{setBendingStiffness(value);}
		}

		public float StretchingStiffness
		{
			get{return getStretchingStiffness();}
			set{setStretchingStiffness(value);}
		}

		public float DampingCoefficient
		{
			get{return getDampingCoefficient();}
			set{setDampingCoefficient(value);}
		}

		public float Friction
		{
			get{return getFriction();}
			set{setFriction(value);}
		}

		public float Pressure
		{
			get{return getPressure();}
			set{setPressure(value);}
		}

		public float TearFactor
		{
			get{return getTearFactor();}
			set{setTearFactor(value);}
		}

		public float Thickness
		{
			get{return getThickness();}
			set{setThickness(value);}
		}

		public float Density
			{get{return getDensity();}}

		public uint SolverIterations
		{
			get{return getSolverIterations();}
			set{setSolverIterations(value);}
		}

		public ushort Group
		{
			get{return getGroup();}
			set{setGroup(value);}
		}

		public NxBounds3 WorldBounds
			{get{return getWorldBounds();}}

		public NxGroupsMask GroupsMask
		{
			get{return getGroupsMask();}
			set{setGroupsMask(value);}
		}
		
		public float CollisionResponseCoefficient
		{
			get{return getCollisionResponseCoefficient();}
			set{setCollisionResponseCoefficient(value);}
		}

		public float AttachmentResponseCoefficient
		{
			get{return getAttachmentResponseCoefficient();}
			set{setAttachmentResponseCoefficient(value);}
		}

		public Vector3 ExternalAcceleration
		{
			get{return getExternalAcceleration();}
			set{setExternalAcceleration(value);}
		}

		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}

		public bool FlagPressure
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_PRESSURE);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_PRESSURE,value);}
		}

		public bool FlagStatic
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_STATIC);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_STATIC,value);}
		}

		public bool FlagDisableCollisin
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_DISABLE_COLLISION);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_DISABLE_COLLISION,value);}
		}

		public bool FlagSelfCollision
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_SELFCOLLISION);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_SELFCOLLISION,value);}
		}

		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_VISUALIZATION);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_VISUALIZATION,value);}
		}

		public bool FlagGravity
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_GRAVITY);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_GRAVITY,value);}
		}

		public bool FlagBending
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_BENDING);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_BENDING,value);}
		}

		public bool FlagBendingOrtho
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_BENDING_ORTHO);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_BENDING_ORTHO,value);}
		}

		public bool FlagDamping
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_DAMPING);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_DAMPING,value);}
		}

		public bool FlagCollisionTwoWay
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_COLLISION_TWOWAY);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_COLLISION_TWOWAY,value);}
		}

		public bool FlagTriangleCollision
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_TRIANGLE_COLLISION);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_TRIANGLE_COLLISION,value);}
		}

		public bool FlagTearable
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_TEARABLE);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_TEARABLE,value);}
		}

		public bool FlagHardware
		{
			get{return NovodexUtil.areBitsSet(Flags,(uint)NxClothFlag.NX_CLF_HARDWARE);}
			set{Flags=NovodexUtil.setBits(Flags,(uint)NxClothFlag.NX_CLF_HARDWARE,value);}
		}




		virtual public void setName(String name)
			{wrapper_Cloth_setName(nxClothPtr,name);}		
		
		virtual public string getName()
			{return wrapper_Cloth_getName(nxClothPtr);}		

		virtual public void saveToDesc(NxClothDesc clothDesc)
		{
			//For some reason I have to manually pass in the meshData for it to work.
			wrapper_Cloth_saveToDesc(nxClothPtr,clothDesc,ref clothDesc.meshData);
			clothDesc.name=getName();
		}

		virtual public NxClothDesc getClothDesc()
		{
			NxClothDesc clothDesc=NxClothDesc.Default;
			saveToDesc(clothDesc);
			return clothDesc;
		}

		virtual public void setMeshData(NxMeshData meshData)
			{wrapper_Cloth_setMeshData(nxClothPtr,ref meshData);}

		virtual public NxMeshData getMeshData()
		{
			NxMeshData meshData=NxMeshData.Default;
			wrapper_Cloth_getMeshData(nxClothPtr,ref meshData);
			return meshData;
		}

		virtual public NxClothMesh getClothMesh()
		{
			IntPtr clothMeshPtr=wrapper_Cloth_getClothMesh(nxClothPtr);
			return NxClothMesh.createFromPointer(clothMeshPtr);
		}

		virtual public void setBendingStiffness(float stiffness)
			{wrapper_Cloth_setBendingStiffness(nxClothPtr,stiffness);}

		virtual public float getBendingStiffness()
			{return wrapper_Cloth_getBendingStiffness(nxClothPtr);}

		virtual public void setStretchingStiffness(float stiffness)
			{wrapper_Cloth_setStretchingStiffness(nxClothPtr,stiffness);}

		virtual public float getStretchingStiffness()
			{return wrapper_Cloth_getStretchingStiffness(nxClothPtr);}

		virtual public void setDampingCoefficient(float dampingCoefficient)
			{wrapper_Cloth_setDampingCoefficient(nxClothPtr,dampingCoefficient);}

		virtual public float getDampingCoefficient()
			{return wrapper_Cloth_getDampingCoefficient(nxClothPtr);}

		virtual public void setFriction(float friction)
			{wrapper_Cloth_setFriction(nxClothPtr,friction);}

		virtual public float getFriction()
			{return wrapper_Cloth_getFriction(nxClothPtr);}

		virtual public void setPressure(float pressure)
			{wrapper_Cloth_setPressure(nxClothPtr,pressure);}

		virtual public float getPressure()
			{return wrapper_Cloth_getPressure(nxClothPtr);}

		virtual public void setTearFactor(float tearFactor)
			{wrapper_Cloth_setTearFactor(nxClothPtr,tearFactor);}

		virtual public float getTearFactor()
			{return wrapper_Cloth_getTearFactor(nxClothPtr);}

		virtual public void setThickness(float thickness)
			{wrapper_Cloth_setThickness(nxClothPtr,thickness);}

		virtual public float getThickness()
			{return wrapper_Cloth_getThickness(nxClothPtr);}

		virtual public float getDensity()
			{return wrapper_Cloth_getDensity(nxClothPtr);}

		virtual public void setSolverIterations(uint iterations)
			{wrapper_Cloth_setSolverIterations(nxClothPtr,iterations);}

		virtual public uint getSolverIterations()
			{return wrapper_Cloth_getSolverIterations(nxClothPtr);}

		virtual public void setGroup(ushort collisionGroup)
			{wrapper_Cloth_setGroup(nxClothPtr,collisionGroup);}

		virtual public ushort getGroup()
			{return wrapper_Cloth_getGroup(nxClothPtr);}

		virtual public NxBounds3 getWorldBounds()
		{
			NxBounds3 bounds=new NxBounds3();
			wrapper_Cloth_getWorldBounds(nxClothPtr,bounds);
			return bounds;
		}

		virtual public void setGroupsMask(NxGroupsMask groupsMask)
			{wrapper_Cloth_setGroupsMask(nxClothPtr,ref groupsMask);}

		virtual public NxGroupsMask getGroupsMask()
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);
			wrapper_Cloth_getGroupsMask(nxClothPtr,ref groupsMask);
			return groupsMask;
		}

		virtual public void setCollisionResponseCoefficient(float coefficient)
			{wrapper_Cloth_setCollisionResponseCoefficient(nxClothPtr,coefficient);}

		virtual public float getCollisionResponseCoefficient()
			{return wrapper_Cloth_getCollisionResponseCoefficient(nxClothPtr);}

		virtual public void setAttachmentResponseCoefficient(float coefficient)
			{wrapper_Cloth_setAttachmentResponseCoefficient(nxClothPtr,coefficient);}

		virtual public float getAttachmentResponseCoefficient()
			{return wrapper_Cloth_getAttachmentResponseCoefficient(nxClothPtr);}

		virtual public void setExternalAcceleration(Vector3 acceleration)
			{wrapper_Cloth_setExternalAcceleration(nxClothPtr,ref acceleration);}

		virtual public Vector3 getExternalAcceleration()
		{
			Vector3 acceleration=new Vector3(0,0,0);
			wrapper_Cloth_getExternalAcceleration(nxClothPtr,ref acceleration);
			return acceleration;
		}

		virtual public void setFlags(uint flags)
			{wrapper_Cloth_setFlags(nxClothPtr,flags);}

		virtual public uint getFlags()
			{return wrapper_Cloth_getFlags(nxClothPtr);}

		virtual public void addForceAtVertex(Vector3 force,uint vertexId,NxForceMode forceMode)
			{wrapper_Cloth_addForceAtVertex(nxClothPtr,ref force,vertexId,forceMode);}

		virtual public void attachToShape(NxShape shape,NxClothInteractionMode interactionMode)
			{wrapper_Cloth_attachToShape(nxClothPtr,shape.NxShapePtr,interactionMode);}

		virtual public void attachToCollidingShapes(NxClothInteractionMode interactionMode)
			{wrapper_Cloth_attachToCollidingShapes(nxClothPtr,interactionMode);}

		virtual public void detachFromShape(NxShape shape)
			{wrapper_Cloth_detachFromShape(nxClothPtr,shape.NxShapePtr);}

		virtual public void attachVertexToShape(uint vertexId,NxShape shape,Vector3 localPos,NxClothInteractionMode interactionMode)
			{wrapper_Cloth_attachVertexToShape(nxClothPtr,vertexId,shape.NxShapePtr,ref localPos,interactionMode);}

		virtual public void attachVertexToGlobalPosition(uint vertexId,Vector3 globalPos)
			{wrapper_Cloth_attachVertexToGlobalPosition(nxClothPtr,vertexId,ref globalPos);}

		virtual public void freeVertex(uint vertexId)
			{wrapper_Cloth_freeVertex(nxClothPtr,vertexId);}

		virtual public bool raycast(NxRay worldRay,out Vector3 hit,out uint vertexId)
			{return wrapper_Cloth_raycast(nxClothPtr,worldRay,out hit,out vertexId);}		

		virtual public bool overlapAABBTriangles(NxBounds3 bounds,out uint[] triangleIndexArray)
		{
			IntPtr trianglesPtr;
			uint numTriangles;
			bool result=wrapper_Cloth_overlapAABBTriangles(nxClothPtr,bounds,out numTriangles,out trianglesPtr);
			return copyTriangleIndexArray(result,out triangleIndexArray,trianglesPtr,numTriangles);
		}

		unsafe private bool copyTriangleIndexArray(bool result,out uint[] triangleIndexArray,IntPtr trianglesPtr,uint numTriangles)
		{
			if(result)
			{
				triangleIndexArray=new uint[numTriangles];
				uint* triangles=(uint*)trianglesPtr;
//crap
//Creating that array every damn time might be insane!! Use an arrayList instead and clear if null instead of setting to null??
				for(int i=0;i<numTriangles;i++)
					{triangleIndexArray[i]=triangles[i];}
			}
			else
				{triangleIndexArray=null;}

			return result;
		}





		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setName(IntPtr cloth,string name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern string wrapper_Cloth_getName(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_saveToDesc(IntPtr cloth,NxClothDesc clothDesc,ref NxMeshData meshData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_getMeshData(IntPtr cloth,ref NxMeshData meshData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setMeshData(IntPtr cloth,ref NxMeshData meshData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Cloth_getClothMesh(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setBendingStiffness(IntPtr cloth,float stiffness);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getBendingStiffness(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setStretchingStiffness(IntPtr cloth,float stiffness);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getStretchingStiffness(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setDampingCoefficient(IntPtr cloth,float dampingCoefficient);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getDampingCoefficient(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setFriction(IntPtr cloth,float friction);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getFriction(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setPressure(IntPtr cloth,float pressure);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getPressure(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setTearFactor(IntPtr cloth,float tearFactor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getTearFactor(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setThickness(IntPtr cloth,float thickness);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getThickness(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getDensity(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setSolverIterations(IntPtr cloth,uint iterations);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Cloth_getSolverIterations(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setGroup(IntPtr cloth,ushort collisionGroup);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_Cloth_getGroup(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_getWorldBounds(IntPtr cloth,NxBounds3 bounds);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_getGroupsMask(IntPtr cloth,ref NxGroupsMask groupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setGroupsMask(IntPtr cloth,ref NxGroupsMask groupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setCollisionResponseCoefficient(IntPtr cloth,float coefficient);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getCollisionResponseCoefficient(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setAttachmentResponseCoefficient(IntPtr cloth,float coefficient);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Cloth_getAttachmentResponseCoefficient(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setExternalAcceleration(IntPtr cloth,ref Vector3 acceleration);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_getExternalAcceleration(IntPtr cloth,ref Vector3 acceleration);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setFlags(IntPtr cloth,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Cloth_getFlags(IntPtr cloth);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_addForceAtVertex(IntPtr cloth,ref Vector3 force,uint vertexId,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_attachToShape(IntPtr cloth,IntPtr shape,NxClothInteractionMode interactionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_attachToCollidingShapes(IntPtr cloth,NxClothInteractionMode interactionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_detachFromShape(IntPtr cloth,IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_attachVertexToShape(IntPtr cloth,uint vertexId,IntPtr shape,ref Vector3 localPos,NxClothInteractionMode interactionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_attachVertexToGlobalPosition(IntPtr cloth,uint vertexId,ref Vector3 globalPos);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_freeVertex(IntPtr cloth,uint vertexId);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cloth_raycast(IntPtr cloth,NxRay worldRay,out Vector3 hit,out uint vertexId);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Cloth_overlapAABBTriangles(IntPtr cloth,NxBounds3 bounds,out uint numTriangles,out IntPtr triangleIndices);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Cloth_setUserData(IntPtr cloth,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Cloth_getUserData(IntPtr cloth);
	}
}



/*
		//This would need to scan through the scenes to find the parentScene
		virtual public void destroy()
		{
			ParentScene.releaseCloth(this);
			nxClothPtr=IntPtr.Zero;
		}
*/



/*
-	virtual	bool saveToDesc(NxClothDesc& desc) const = 0;
-	virtual	NxClothMesh* getClothMesh() const = 0;
-	virtual void setBendingStiffness(NxReal stiffness) = 0;
-	virtual NxReal getBendingStiffness() const = 0;
-	virtual void setStretchingStiffness(NxReal stiffness) = 0;
-	virtual NxReal getStretchingStiffness() const = 0;
-	virtual void setDampingCoefficient(NxReal dampingCoefficient) = 0;
-	virtual NxReal getDampingCoefficient() const = 0;
-	virtual void setFriction(NxReal friction) = 0;
-	virtual NxReal getFriction() const = 0;
-	virtual void setPressure(NxReal pressure) = 0;
-	virtual NxReal getPressure() const = 0;
-	virtual void setTearFactor(NxReal factor) = 0;
-	virtual NxReal getTearFactor() const = 0;
-	virtual void setThickness(NxReal thickness) = 0;
-	virtual NxReal getThickness() const = 0;
-	virtual NxReal getDensity() const = 0;
-	virtual NxU32 getSolverIterations() const = 0;
-	virtual void setSolverIterations(NxU32 iterations) = 0;
-	virtual void setGroup(NxCollisionGroup collisionGroup) = 0;
-	virtual NxCollisionGroup getGroup() const = 0;
-	virtual void getWorldBounds(NxBounds3& bounds) const = 0;
-	virtual void setGroupsMask(const NxGroupsMask& groupsMask) = 0;
-	virtual const NxGroupsMask getGroupsMask() const = 0;
-	virtual void setMeshData(NxMeshData& meshData) = 0;
-	virtual NxMeshData getMeshData() = 0;
-	virtual void setCollisionResponseCoefficient(NxReal coefficient) = 0;
-	virtual NxReal getCollisionResponseCoefficient() const = 0;
-	virtual void setAttachmentResponseCoefficient(NxReal coefficient) = 0;
-	virtual NxReal getAttachmentResponseCoefficient() const = 0;
-	virtual void setExternalAcceleration(NxVec3 acceleration) = 0;
-	virtual NxVec3 getExternalAcceleration() const = 0;
-	virtual void setFlags(NxU32 flags) = 0;
-	virtual NxU32 getFlags() const = 0;
-	virtual void setName(const char* name) = 0;
-	virtual const char* getName() const = 0;
-	virtual	void addForceAtVertex(const NxVec3& force, NxU32 vertexId, NxForceMode mode = NX_FORCE) = 0;
-	virtual void attachToShape(const NxShape *shape, NxClothInteractionMode interactionMode) = 0;
-	virtual void attachToCollidingShapes(NxClothInteractionMode interactionMode) = 0;
-	virtual void detachFromShape(const NxShape *shape) = 0;
-	virtual void attachVertexToShape(NxU32 vertexId, const NxShape *shape, const NxVec3 &localPos, NxClothInteractionMode interactionMode) = 0;
-	virtual void attachVertexToGlobalPosition(const NxU32 vertexId, const NxVec3 &pos) = 0;
-	virtual void freeVertex(const NxU32 vertexId) = 0;
X	virtual bool raycast(const NxRay& worldRay, NxVec3 &hit, NxU32 &vertexId) = 0;
-	virtual	bool overlapAABBTriangles(const NxBounds3 bounds, NxU32& nb, const NxU32*& indices)	const = 0;

-	void* userData; //!< user can assign this to whatever, usually to create a 1:1 relationship with a user object.
*/


//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;





namespace NovodexWrapper
{
	public class NxPhysicsSDK
	{
		private static NxPhysicsSDK staticSDK=null;
		

		protected IntPtr nxPhysicsSDKptr;


	
		public static NxPhysicsSDK StaticSDK
			{get{return staticSDK;}}
		
		public static string Version
		{
			get
			{
				uint versionNum=wrapper_PhysicsSDK_getVersion();
				return ((versionNum>>24)&0xFF)+"."+((versionNum>>16)&0xFF)+"."+((versionNum>>8)&0xFF);
			}
		}

		public IntPtr NxPhysicsSDKptr
			{get{return nxPhysicsSDKptr;}}

		public static NxPhysicsSDK Create()
			{return Create(null,null);}

		public static NxPhysicsSDK Create(NxUserOutputStream userOutputStream)
			{return Create(userOutputStream,null);}

		public static NxPhysicsSDK Create(NxUserOutputStream userOutputStream,NxPhysicsSDKDesc physicsSDKDesc)
		{
			if(staticSDK!=null)
				{return staticSDK;}

			if(physicsSDKDesc==null)
				{physicsSDKDesc=new NxPhysicsSDKDesc();}

			staticSDK=new NxPhysicsSDK();
			staticSDK.nxPhysicsSDKptr=wrapper_PhysicsSDK_createPhysicsSDK(physicsSDKDesc);
			if(staticSDK.nxPhysicsSDKptr==IntPtr.Zero)
					{return null;}
				
			staticSDK.setUserOutputStream(userOutputStream);
			return staticSDK;
		}

        virtual public void release()
        {
            wrapper_PhysicsSDK_release(nxPhysicsSDKptr);
            nxPhysicsSDKptr = IntPtr.Zero;
            staticSDK = null;
        }

        //MHGW Edited
        virtual public bool IsReleased
        {
            get { return nxPhysicsSDKptr == IntPtr.Zero; }
        }
	

		virtual public bool setParameter(NxParameter paramEnum,float paramValue)
			{return wrapper_PhysicsSDK_setParameter(nxPhysicsSDKptr,paramEnum,paramValue);}
			
		virtual public float getParameter(NxParameter paramEnum)
			{return wrapper_PhysicsSDK_getParameter(nxPhysicsSDKptr,paramEnum);}
			
		virtual public NxScene createScene(NxSceneDesc sceneDesc)
		{
			IntPtr customSchedulerPtr=IntPtr.Zero;
			if(sceneDesc.customScheduler!=null)
				{customSchedulerPtr=sceneDesc.customScheduler.NxUserSchedulerPtr;}

			IntPtr scenePtr=wrapper_PhysicsSDK_createSceneByParameters(nxPhysicsSDKptr,ref sceneDesc.gravity,sceneDesc.maxTimestep,sceneDesc.maxIter,sceneDesc.timeStepMethod,sceneDesc.maxBounds,sceneDesc.limits,sceneDesc.simType,sceneDesc.hwSceneType,sceneDesc.pipelineSpec,sceneDesc.groundPlane,sceneDesc.boundsPlanes,sceneDesc.flags,sceneDesc.internalThreadCount,sceneDesc.backgroundThreadCount,sceneDesc.threadMask,sceneDesc.backgroundThreadMask,sceneDesc.userData,customSchedulerPtr);
			if(scenePtr==IntPtr.Zero)
				{return null;}

			NxScene scene=NxScene.createFromPointer(scenePtr);
			if(sceneDesc.userNotify!=null)
				{scene.setUserNotify(sceneDesc.userNotify);}
			if(sceneDesc.userTriggerReport!=null)
				{scene.setUserTriggerReport(sceneDesc.userTriggerReport);}
			if(sceneDesc.userContactReport!=null)
				{scene.setUserContactReport(sceneDesc.userContactReport);}
			return scene;
		}
		
		virtual public NxScene createScene(Vector3 gravity)
		{
			NxSceneDesc sceneDesc=NxSceneDesc.Default;
			sceneDesc.gravity=gravity;
			return createScene(sceneDesc);
		}
		
		virtual public NxScene createScene(Vector3 gravity,float maxTimeStep,int maxIter,NxTimeStepMethod timeStepMethod,NxBounds3 maxBounds,NxSceneLimits limits,NxSimulationType simType,NxHwSceneType hwSceneType,NxHwPipelineSpec pipelineSpec,bool groundPlane,bool boundsPlanes,NxUserNotify userNotify,NxUserTriggerReport userTriggerReport,NxUserContactReport userContactReport,NxUserScheduler customScheduler)
		{
			NxSceneDesc sceneDesc=new NxSceneDesc(gravity,maxTimeStep,maxIter,timeStepMethod,maxBounds,limits,simType,hwSceneType,pipelineSpec,groundPlane,boundsPlanes,userNotify,userTriggerReport,userContactReport,customScheduler);
			return createScene(sceneDesc);
		}
		
		virtual public void releaseScene(NxScene scene)
		{
			wrapper_PhysicsSDK_releaseScene(nxPhysicsSDKptr,scene.NxScenePtr);
			scene.internalAfterRelease();
		}

		virtual public int getNbScenes()
			{return wrapper_PhysicsSDK_getNbScenes(nxPhysicsSDKptr);}

		virtual public NxScene getScene(int sceneIndex)
			{return NxScene.createFromPointer(wrapper_PhysicsSDK_getScene(nxPhysicsSDKptr,sceneIndex));}

		virtual public NxScene[] getScenes()
		{
			int numScenes=getNbScenes();
			NxScene[] sceneArray=new NxScene[numScenes];
			for(int i=0;i<numScenes;i++)
				{sceneArray[i]=getScene(i);}
			return sceneArray;
		}

		virtual public NxTriangleMesh createTriangleMesh(NxStream stream)
		{
			IntPtr triMeshPtr=wrapper_PhysicsSDK_createTriangleMesh(nxPhysicsSDKptr,stream.NxStreamPtr);
			return NxTriangleMesh.createFromPointer(triMeshPtr);
		}

		virtual public void releaseTriangleMesh(NxTriangleMesh triangleMesh)
			{wrapper_PhysicsSDK_releaseTriangleMesh(nxPhysicsSDKptr,triangleMesh.NxTriangleMeshPtr);}

		virtual public NxConvexMesh createConvexMesh(NxStream stream)
		{
			IntPtr convexMeshPtr=wrapper_PhysicsSDK_createConvexMesh(nxPhysicsSDKptr,stream.NxStreamPtr);
			return NxConvexMesh.createFromPointer(convexMeshPtr);
		}

		virtual public void releaseConvexMesh(NxConvexMesh convexMesh)
			{wrapper_PhysicsSDK_releaseConvexMesh(nxPhysicsSDKptr,convexMesh.NxConvexMeshPtr);}

		virtual public NxHeightField createHeightField(NxHeightFieldDesc heightFieldDesc)
		{
			IntPtr heightFieldPtr=wrapper_PhysicsSDK_createHeightField(nxPhysicsSDKptr,heightFieldDesc);
			return NxHeightField.createFromPointer(heightFieldPtr);
		}

		virtual public void releaseHeightField(NxHeightField heightField)
			{wrapper_PhysicsSDK_releaseHeightField(nxPhysicsSDKptr,heightField.NxHeightFieldPtr);}

		virtual public uint getInternalVersion(out uint apiRev,out uint descRev,out uint branchID)
			{return wrapper_PhysicsSDK_getInternalVersion(nxPhysicsSDKptr,out apiRev,out descRev,out branchID);}

		virtual public NxHWVersion getHWVersion()
			{return (NxHWVersion)wrapper_PhysicsSDK_getHWVersion(nxPhysicsSDKptr);}

		virtual public void setUserOutputStream(NxUserOutputStream outputStream)
		{
			if(outputStream!=null)
				{outputStream.internalSetCallbacks();}
			else
				{NxUserOutputStream.internalClearCallbacks();}
		}

		virtual public NxClothMesh createClothMesh(NxStream stream)
		{
			IntPtr clothMeshPtr=wrapper_PhysicsSDK_createClothMesh(nxPhysicsSDKptr,stream.NxStreamPtr);
			return NxClothMesh.createFromPointer(clothMeshPtr);
		}

		virtual public void releaseClothMesh(NxClothMesh clothMesh)
			{wrapper_PhysicsSDK_releaseClothMesh(nxPhysicsSDKptr,clothMesh.NxClothMeshPtr);}

		virtual public int getNbClothMeshes()
			{return wrapper_PhysicsSDK_getNbClothMeshes(nxPhysicsSDKptr);}

		virtual public NxClothMesh[] getClothMeshes()
		{
			int numClothMeshes=getNbClothMeshes();
			NxClothMesh[] clothMeshArray=new NxClothMesh[numClothMeshes];
			IntPtr clothMeshPtr=wrapper_PhysicsSDK_getClothMeshes(nxPhysicsSDKptr);

			unsafe
			{
				int* ptr=(int*)clothMeshPtr.ToPointer();
				for(int i=0;i<numClothMeshes;i++)
					{clothMeshArray[i]=new NxClothMesh(new IntPtr(ptr[i]));}
			}

			return clothMeshArray;
		}

		virtual public NxCCDSkeleton createCCDSkeleton(NxSimpleTriangleMesh mesh)
		{
			IntPtr skeletonPtr=wrapper_PhysicsSDK_createCCDSkeletonFromMesh(nxPhysicsSDKptr,mesh);
			return NxCCDSkeleton.createFromPointer(skeletonPtr);
		}

		unsafe virtual public NxCCDSkeleton createCCDSkeleton(byte[] memoryBuffer)
		{
			IntPtr memoryBufferPtr=IntPtr.Zero;
			fixed(void* x=&memoryBuffer[0])
				{memoryBufferPtr=new IntPtr(x);}

			IntPtr skeletonPtr=wrapper_PhysicsSDK_createCCDSkeletonFromBuffer(nxPhysicsSDKptr,memoryBufferPtr,(uint)memoryBuffer.Length);
			return NxCCDSkeleton.createFromPointer(skeletonPtr);
		}

		virtual public void releaseCCDSkeleton(NxCCDSkeleton skeleton)
		{
			wrapper_PhysicsSDK_releaseCCDSkeleton(nxPhysicsSDKptr,skeleton.NxCCDSkeletonPtr);
			skeleton.internalAfterRelease();
		}

		virtual public NxInterface getInterface(NxInterfaceType type,int versionNumber)
		{
			IntPtr interfacePtr=wrapper_PhysicsSDK_getInterface(nxPhysicsSDKptr,type,versionNumber);
			return NxInterface.createFromPointer(interfacePtr);
		}

		public virtual NxFoundationSDK getFoundationSDK()
		{
			IntPtr foundationSDKpointer=wrapper_PhysicsSDK_getFoundationSDK(nxPhysicsSDKptr);
			return NxFoundationSDK.createFromPointer(foundationSDKpointer);
		}




		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_PhysicsSDK_getVersion();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createPhysicsSDK(NxPhysicsSDKDesc physicsSDKDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_PhysicsSDK_release(IntPtr physicsSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_PhysicsSDK_setParameter(IntPtr physicsSDK,NxParameter paramEnum,float paramValue);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_PhysicsSDK_getParameter(IntPtr physicsSDK,NxParameter paramEnum);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createScene(IntPtr physicsSDK,NxSceneDesc sceneDesc);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createSceneByParameters(IntPtr physicsSDK,ref Vector3 gravity,float maxTimestep,uint maxIter,NxTimeStepMethod timeStepMethod,NxBounds3 maxBounds,NxSceneLimits limits,NxSimulationType simType,NxHwSceneType hwSceneType,NxHwPipelineSpec pipelineSpec,bool groundPlane,bool boundsPlanes,uint flags,uint internalThreadCount,uint backgroundThreadCount,uint threadMask,uint backgroundThreadMask,IntPtr userData,IntPtr customSchedulerPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PhysicsSDK_releaseScene(IntPtr physicsSDK,IntPtr NxScene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_PhysicsSDK_getNbScenes(IntPtr physicsSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_getScene(IntPtr physicsSDK,int sceneIndex);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createTriangleMesh(IntPtr physicsSDK,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PhysicsSDK_releaseTriangleMesh(IntPtr physicsSDK,IntPtr triangleMeshPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createConvexMesh(IntPtr physicsSDK,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PhysicsSDK_releaseConvexMesh(IntPtr physicsSDK,IntPtr convexMeshPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createHeightField(IntPtr physicsSDK,NxHeightFieldDesc heightFieldDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PhysicsSDK_releaseHeightField(IntPtr physicsSDK,IntPtr heightFieldPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_PhysicsSDK_getInternalVersion(IntPtr physicsSDK,out uint apiRev,out uint descRev,out uint branchID);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_PhysicsSDK_getHWVersion(IntPtr physicsSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createClothMesh(IntPtr physicsSDK,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PhysicsSDK_releaseClothMesh(IntPtr physicsSDK,IntPtr clothMeshPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_PhysicsSDK_getNbClothMeshes(IntPtr physicsSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_getClothMeshes(IntPtr physicsSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createCCDSkeletonFromMesh(IntPtr physicsSDK,NxSimpleTriangleMesh mesh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_createCCDSkeletonFromBuffer(IntPtr physicsSDK,IntPtr memoryBufferPtr,uint bufferSize);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_PhysicsSDK_releaseCCDSkeleton(IntPtr physicsSDK,IntPtr skeletonPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_getInterface(IntPtr physicsSDK,NxInterfaceType type,int versionNumber);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_PhysicsSDK_getFoundationSDK(IntPtr physicsSDK);
	}
}


/*
X	virtual	void release() = 0;
X	virtual bool setParameter(NxParameter paramEnum, NxReal paramValue) = 0;
X	virtual NxReal getParameter(NxParameter paramEnum) const = 0;
X	virtual NxScene* createScene(const NxSceneDesc& sceneDesc) = 0;
X	virtual void releaseScene(NxScene& scene) = 0;
X	virtual NxU32 getNbScenes()			const	= 0;
X	virtual NxScene* getScene(NxU32 i)			= 0;
X	virtual NxTriangleMesh* createTriangleMesh(const NxStream& stream) = 0;
X	virtual	void	releaseTriangleMesh(NxTriangleMesh& mesh) = 0;
-	virtual NxHeightField * createHeightField(const NxHeightFieldDesc& desc) = 0;
-	virtual	void	releaseHeightField(NxHeightField& heightField) = 0;
-	virtual NxCCDSkeleton * createCCDSkeleton(const NxSimpleTriangleMesh& mesh) = 0;
-	virtual NxCCDSkeleton * createCCDSkeleton(const void * memoryBuffer, NxU32 bufferSize) = 0;
-	virtual	void	releaseCCDSkeleton(NxCCDSkeleton& skel) = 0;
X	virtual NxConvexMesh* createConvexMesh(const NxStream& mesh) = 0;
X	virtual	void	releaseConvexMesh(NxConvexMesh& mesh) = 0;
X	virtual NxClothMesh*				createClothMesh(NxStream& stream) = 0;
X	virtual void						releaseClothMesh(NxClothMesh& cloth) = 0;
X	virtual	NxU32						getNbClothMeshes() const = 0;
-	virtual	NxClothMesh**				getClothMeshes() = 0;
-	virtual NxU32 getInternalVersion(NxU32 &apiRev,NxU32 &descRev,NxU32 &branchId)			const	= 0;
-   virtual NxHWVersion getHWVersion() const = 0;
X	virtual NxInterface *getInterface(NxInterfaceType type,int versionNumber) = 0;
X	virtual NxFoundationSDK& getFoundationSDK() const = 0;
*/


//NX_C_EXPORT NXP_DLL_EXPORT NxU32 NxGetValue(NxCookingValue cookValue);

/*
From PhysXLoader
NXPHYSXLOADERDLL_API NxPhysicsSDK		*NxCreatePhysicsSDK(NxU32 sdkVersion, NxUserAllocator* allocator = NULL, NxUserOutputStream* outputStream = NULL, const NxPhysicsSDKDesc &desc = NxPhysicsSDKDesc());
NXPHYSXLOADERDLL_API void				NxReleasePhysicsSDK(NxPhysicsSDK *sdk);
NXPHYSXLOADERDLL_API NxUserAllocator	*NxGetPhysicsSDKAllocator();
NXPHYSXLOADERDLL_API NxFoundationSDK*	NxGetFoundationSDK();
NXPHYSXLOADERDLL_API NxPhysicsSDK*		NxGetPhysicsSDK();
NXPHYSXLOADERDLL_API NxUtilLib			*NxGetUtilLib();
NXPHYSXLOADERDLL_API NxUserAllocator*	NxGetPhysicsSDKAllocator();
NXPHYSXLOADERDLL_API NxFoundationSDK*	NxGetFoundationSDK();
NXPHYSXLOADERDLL_API NxPhysicsSDK*		NxGetPhysicsSDK();
*/




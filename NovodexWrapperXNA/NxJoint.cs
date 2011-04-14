//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	abstract public class NxJoint
	{
		protected IntPtr nxJointPtr;
		
		public NxJoint(IntPtr jointPointer)
			{nxJointPtr=jointPointer;}
		
		abstract public NxJoint copy(NxActor newActor0,NxActor newActor1);
			
		public static NxJoint createFromPointer(IntPtr jointPointer)
		{
			if(jointPointer==IntPtr.Zero)
				{return null;}

			NxJointType jointType=wrapper_Joint_getType(jointPointer);
			
			if(jointType==NxJointType.NX_JOINT_CYLINDRICAL)
				{return new NxCylindricalJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_D6)
				{return new NxD6Joint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_DISTANCE)
				{return new NxDistanceJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_FIXED)
				{return new NxFixedJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_POINT_IN_PLANE)
				{return new NxPointInPlaneJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_POINT_ON_LINE)
				{return new NxPointOnLineJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_PRISMATIC)
				{return new NxPrismaticJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_PULLEY)
				{return new NxPulleyJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_REVOLUTE)
				{return new NxRevoluteJoint(jointPointer);}
			if(jointType==NxJointType.NX_JOINT_SPHERICAL)
				{return new NxSphericalJoint(jointPointer);}

			return null;
		}



		virtual public void internalBeforeRelease()
			{setName(null);}	//This frees the unmanaged memory for the name

		virtual public void internalAfterRelease()
			{nxJointPtr=IntPtr.Zero;}
		
		virtual public void destroy()
		{
			ParentScene.releaseJoint(this);
			nxJointPtr=IntPtr.Zero;
			//NxScene.releaseJoint() will remove this joint from it's actors' jointLists
		}


		abstract protected void internalLoadFromDesc(NxJointDesc jointDesc);
		abstract protected NxJointDesc internalGetJointDesc();

		public NxJointDesc getJointDesc()
			{return internalGetJointDesc();}
		


		public IntPtr NxJointPtr
			{get{return nxJointPtr;}}

		public NxScene ParentScene
			{get{return getScene();}}



		public IntPtr UserData
		{
			get{return wrapper_Joint_getUserData(nxJointPtr);}
			set{wrapper_Joint_setUserData(nxJointPtr,value);}
		}

		public IntPtr AppData
		{
			get{return wrapper_Joint_getAppData(nxJointPtr);}
			set{wrapper_Joint_setAppData(nxJointPtr,value);}
		}

		public string Name
		{
			get{return getName();}
			set{setName(value);}
		}

		public uint JointFlags
		{
			get{return getJointDesc().jointFlags;}
			set
			{
				NxJointDesc jointDesc=getJointDesc();
				jointDesc.jointFlags=value;
				internalLoadFromDesc(jointDesc);
			}
		}

		public bool FlagCollisionEnabled
		{
			get{return NovodexUtil.areBitsSet(JointFlags,(uint)NxJointFlag.NX_JF_COLLISION_ENABLED);}
			set{JointFlags=NovodexUtil.setBits(JointFlags,(uint)NxJointFlag.NX_JF_COLLISION_ENABLED,value);}
		}

		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(JointFlags,(uint)NxJointFlag.NX_JF_VISUALIZATION);}
			set{JointFlags=NovodexUtil.setBits(JointFlags,(uint)NxJointFlag.NX_JF_VISUALIZATION,value);}
		}
		
		public Vector3 GlobalAnchor
		{
			get{return getGlobalAnchor();}
			set{setGlobalAnchor(value);}
		}

		public Vector3 GlobalAxis
		{
			get{return getGlobalAxis();}
			set{setGlobalAxis(value);}
		}

		public float MaxForce
		{
			set
			{
				float mForce=0,mTorque=0;
				getBreakable(ref mForce,ref mTorque);
				setBreakable(value,mTorque);
			}
			get
			{
				float mForce=0,mTorque=0;
				getBreakable(ref mForce,ref mTorque);
				return mForce;
			}
		}

		public float MaxTorque
		{
			set
			{
				float mForce=0,mTorque=0;
				getBreakable(ref mForce,ref mTorque);
				setBreakable(mForce,value);
			}
			get
			{
				float mForce=0,mTorque=0;
				getBreakable(ref mForce,ref mTorque);
				return mTorque;
			}
		}
		
		public NxJointState State
			{get{return getState();}}
			
		public Vector3 LimitPoint
		{
			get{return getLimitPoint();}
			set{setLimitPoint(value);}
		}
		
		public NxActor Actor0
			{get{return (getActors())[0];}}
		
		public NxActor Actor1
			{get{return (getActors())[1];}}		



		//My method
		virtual public bool isJointBroken()
			{return getState()==NxJointState.NX_JS_BROKEN;}





		virtual public NxActor[] getActors()
		{
			NxActor[] actorArray=new NxActor[2];
			getActors(out actorArray[0],out actorArray[1]);
			return actorArray;
		}

		virtual public void getActors(out NxActor actor1,out NxActor actor2)
		{
			IntPtr actor1_ptr=IntPtr.Zero;
			IntPtr actor2_ptr=IntPtr.Zero;
			wrapper_Joint_getActors(nxJointPtr,out actor1_ptr,out actor2_ptr);
			actor1=NxActor.createFromPointer(actor1_ptr);
			actor2=NxActor.createFromPointer(actor2_ptr);
		}

		virtual public NxScene getScene()
			{return NxScene.createFromPointer(wrapper_Joint_getScene(nxJointPtr));}

		virtual public Vector3 getGlobalAnchor(int index)
		{
			if(index<0||index>1)
				{throw new System.IndexOutOfRangeException("NxJoint.getGlobalAnchor("+index+"): Only 0 and 1 are allowed.");}
			return getJointDesc().getGlobalAnchor(index);
		}

		virtual public Vector3 getGlobalAxis(int index)
		{
			if(index<0||index>1)
				{throw new System.IndexOutOfRangeException("NxJoint.getGlobalAxis("+index+"): Only 0 and 1 are allowed.");}
			return getJointDesc().getGlobalAxis(index);
		}

		virtual public void setGlobalAnchor(int index,Vector3 globalAnchor)
		{
			if(index<0||index>1)
				{throw new System.IndexOutOfRangeException("NxJoint.setGlobalAnchor("+index+"): Only 0 and 1 are allowed.");}
			NxJointDesc jointDesc=getJointDesc();
			jointDesc.setGlobalAnchor(index,globalAnchor);
			internalLoadFromDesc(jointDesc);
		}

		virtual public void setGlobalAxis(int index,Vector3 globalAxis)
		{
			if(index<0||index>1)
				{throw new System.IndexOutOfRangeException("NxJoint.setGlobalAxis("+index+"): Only 0 and 1 are allowed.");}
			NxJointDesc jointDesc=getJointDesc();
			jointDesc.setGlobalAxis(index,globalAxis);
			internalLoadFromDesc(jointDesc);
		}



		//this sets the localAchor in both actors to the value passed in
		virtual public void setGlobalAnchor(Vector3 globalAnchor)
			{wrapper_Joint_setGlobalAnchor(nxJointPtr,ref globalAnchor);}
		
		//this sets the localAxis in both actors to the value passed in
		virtual public void setGlobalAxis(Vector3 globalAxis)
			{wrapper_Joint_setGlobalAxis(nxJointPtr,ref globalAxis);}

		//this returns the localAnchor from actor0 transformed into worldSpace
		virtual public Vector3 getGlobalAnchor()
		{
			Vector3 globalAnchor;
			wrapper_Joint_getGlobalAnchor(nxJointPtr,out globalAnchor);
			return globalAnchor;
		}

		//this returns the localAxis from actor0 transformed into worldSpace
		virtual public Vector3 getGlobalAxis()
		{
			Vector3 globalAxis;
			wrapper_Joint_getGlobalAxis(nxJointPtr,out globalAxis);
			return globalAxis;
		}

		virtual public NxJointState getState()
			{return wrapper_Joint_getState(nxJointPtr);}

		virtual public void setBreakable(float maxForce,float maxTorque)
			{wrapper_Joint_setBreakable(nxJointPtr,maxForce,maxTorque);}
		
		virtual public void getBreakable(ref float maxForce,ref float maxTorque)
			{wrapper_Joint_getBreakable(nxJointPtr,ref maxForce,ref maxTorque);}

		virtual public void setLimitPoint(Vector3 worldLimitPoint)
			{setLimitPoint(worldLimitPoint,true);}
			
		virtual public void setLimitPoint(Vector3 worldLimitPoint,bool pointIsOnBody2)
			{wrapper_Joint_setLimitPoint(nxJointPtr,ref worldLimitPoint,pointIsOnBody2);}

		virtual public Vector3 getLimitPoint()
		{
			Vector3 worldLimitPoint=Vector3.Zero;
			wrapper_Joint_getLimitPoint(nxJointPtr,ref worldLimitPoint);
			return worldLimitPoint;
		}

		virtual public bool getLimitPoint(ref Vector3 worldLimitPoint)
			{return wrapper_Joint_getLimitPoint(nxJointPtr,ref worldLimitPoint);}

		virtual public bool addLimitPlane(NxPlane plane)
		{
			Vector3 pointInPlane=plane.pointInPlane();
			return wrapper_Joint_addLimitPlane(nxJointPtr,ref plane.normal,ref pointInPlane);
		}

		virtual public bool addLimitPlane(Vector3 normal,Vector3 pointInPlane)
			{return wrapper_Joint_addLimitPlane(nxJointPtr,ref normal,ref pointInPlane);}

		virtual public void purgeLimitPlanes()
			{wrapper_Joint_purgeLimitPlanes(nxJointPtr);}

		virtual public void resetLimitPlaneIterator()
			{wrapper_Joint_resetLimitPlaneIterator(nxJointPtr);}
			
		virtual public bool hasMoreLimitPlanes()
			{return wrapper_Joint_hasMoreLimitPlanes(nxJointPtr);}				

		virtual public bool getNextLimitPlane(ref Vector3 planeNormal,ref float planeD)
			{return wrapper_Joint_getNextLimitPlane(nxJointPtr,ref planeNormal,ref planeD);}
			
		virtual public bool getNextLimitPlane(ref NxPlane plane)
			{return wrapper_Joint_getNextLimitPlane(nxJointPtr,ref plane.normal,ref plane.d);}

		virtual public int getNbLimitPlanes()
		{
			resetLimitPlaneIterator();
			int numLimitPlanes=0;
			NxPlane plane=new NxPlane();
			
			while(getNextLimitPlane(ref plane))
				{numLimitPlanes++;}
				
			return numLimitPlanes;
		}
			
		virtual public NxPlane[] getLimitPlanes()
		{
			int numLimitPlanes=getNbLimitPlanes();
			NxPlane[] planeArray=new NxPlane[numLimitPlanes];
			
			resetLimitPlaneIterator();
			for(int i=0;i<numLimitPlanes;i++)
				{getNextLimitPlane(ref planeArray[i]);}
			return planeArray;
		}

		virtual public NxJointType getJointType()
			{return wrapper_Joint_getType(nxJointPtr);}

		virtual public bool isJointType(NxJointType jointType)
		{
			if(wrapper_Joint_is(nxJointPtr,jointType)==IntPtr.Zero)
				{return false;}
			return true;
		}

		virtual public void setName(String name)
			{wrapper_Joint_setName(nxJointPtr,name);}		
		
		virtual public string getName()
			{return wrapper_Joint_getName(nxJointPtr);}		








		//Returns the jointTransform as defined in Actor0
		virtual public Matrix getGlobalTransformForActor0()
			{return getGlobalTransformForActor(0);}
		
		//Returns the jointTransform as defined in Actor1
		virtual public Matrix getGlobalTransformForActor1()
			{return getGlobalTransformForActor(1);}

		//A joint is defined in both actor spaces. This returns how the joint is defined in actor0, and actor1 transformed into worldSpace
		virtual public Matrix[] getGlobalTransformsForActors()
		{
			Matrix[] matrixArray=new Matrix[2];
			matrixArray[0]=getGlobalTransformForActor(0);
			matrixArray[1]=getGlobalTransformForActor(1);
			return matrixArray;
		}

		virtual protected Matrix getGlobalTransformForActor(int num)
		{
			Matrix actorTransform;
			
			if(num==0)
				{actorTransform=Actor0.getGlobalPose();}
			else
				{actorTransform=Actor1.getGlobalPose();}
			
			NxJointDesc jointDesc=getJointDesc();
			Vector3 xAxis=Vector3.TransformNormal(jointDesc.localNormal[num],actorTransform);
			Vector3 zAxis=Vector3.TransformNormal(jointDesc.localAxis[num],actorTransform);
			Vector3 yAxis=Vector3.Cross(xAxis,zAxis);
			//Vector3 pos=Vector3.Transform(jointDesc.localAnchor[num],actorTransform);
			Vector3 pos = Vector3.Transform(jointDesc.localAnchor[num], actorTransform);

			return NovodexUtil.createMatrix(xAxis,yAxis,zAxis,pos);
		}


		virtual public void setGlobalTransformForActor0(Matrix jointTransform)
			{setGlobalTransformForActor(0,jointTransform);}
		
		virtual public void setGlobalTransformForActor1(Matrix jointTransform)
			{setGlobalTransformForActor(1,jointTransform);}

		virtual public void setGlobalTransformsForActors(Matrix jointTransform0,Matrix jointTransform1)
		{
			setGlobalTransformForActor(0,jointTransform0);
			setGlobalTransformForActor(1,jointTransform1);
		}

		virtual public void setGlobalTransformsForActors(Matrix[] jointTransforms)
		{
			setGlobalTransformForActor(0,jointTransforms[0]);
			setGlobalTransformForActor(1,jointTransforms[1]);
		}

		virtual protected void setGlobalTransformForActor(int num,Matrix jointTransform)
		{
			Matrix actorTransform,inverseActorTransform;
			
			if(num==0)
				{actorTransform=Actor0.getGlobalPose();}
			else
				{actorTransform=Actor1.getGlobalPose();}
			
			inverseActorTransform=actorTransform;
			inverseActorTransform = Matrix.Invert(inverseActorTransform);
			
			
			
			
			Vector3 xAxis=NovodexUtil.getMatrixXaxis(ref jointTransform);
			Vector3 zAxis=NovodexUtil.getMatrixZaxis(ref jointTransform);
			Vector3 pos=NovodexUtil.getMatrixPos(ref jointTransform);
			
			NxJointDesc jointDesc=getJointDesc();
			jointDesc.localNormal[num]=Vector3.TransformNormal(xAxis,inverseActorTransform);
			jointDesc.localAxis[num]=Vector3.TransformNormal(zAxis,inverseActorTransform);
			//jointDesc.localAnchor[num]=Vector3.Transform(pos,inverseActorTransform);
			jointDesc.localAnchor[num] = Vector3.Transform(pos, inverseActorTransform);
			
			internalLoadFromDesc(jointDesc);
		}







		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_getActors(IntPtr joint,out IntPtr actor1,out IntPtr actor2);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setGlobalAnchor(IntPtr joint,ref Vector3 globalAnchor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setGlobalAxis(IntPtr joint,ref Vector3 globalAxis);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_getGlobalAnchor(IntPtr joint,out Vector3 globalAnchor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_getGlobalAxis(IntPtr joint,out Vector3 globalAxis);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxJointState wrapper_Joint_getState(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setBreakable(IntPtr joint,float maxForce,float maxTorque);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_getBreakable(IntPtr joint,ref float maxForce,ref float maxTorque);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setLimitPoint(IntPtr joint,ref Vector3 worldLimitPoint,bool pointIsOnBody2);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Joint_getLimitPoint(IntPtr joint,ref Vector3 worldLimitPoint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Joint_addLimitPlane(IntPtr joint,ref Vector3 normal,ref Vector3 pointInPlane);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_purgeLimitPlanes(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_resetLimitPlaneIterator(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Joint_hasMoreLimitPlanes(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Joint_getNextLimitPlane(IntPtr joint,ref Vector3 planeNormal,ref float planeD);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxJointType wrapper_Joint_getType(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Joint_is(IntPtr joint,NxJointType jointType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setName(IntPtr joint,string name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern string wrapper_Joint_getName(IntPtr joint);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Joint_getScene(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setUserData(IntPtr joint,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Joint_getUserData(IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Joint_setAppData(IntPtr joint,IntPtr appData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Joint_getAppData(IntPtr joint);
	}
}


/*
X	virtual void getActors(NxActor** actor1, NxActor** actor2) = 0;
X	virtual void setGlobalAnchor(const NxVec3 &) = 0;
X	virtual void setGlobalAxis(const NxVec3 &) = 0;
X	NX_INLINE	NxVec3	getGlobalAnchor()	const {return getGlobalAnchorVal();}
X	NX_INLINE	NxVec3	getGlobalAxis()		const {return getGlobalAxisVal();}
-	virtual NxJointState getState() = 0;
X	virtual void setBreakable(NxReal maxForce, NxReal maxTorque) = 0;
-	virtual void getBreakable(NxReal & maxForce, NxReal & maxTorque) = 0;
-	virtual void setLimitPoint(const NxVec3 & point, bool pointIsOnBody2 = true) = 0;
-	virtual bool getLimitPoint(NxVec3 & worldLimitPoint) = 0;
-	virtual bool addLimitPlane(const NxVec3 & normal, const NxVec3 & pointInPlane) = 0;
-	virtual void purgeLimitPlanes() = 0;
-	virtual void resetLimitPlaneIterator() = 0;
-	virtual bool hasMoreLimitPlanes() = 0;
-	virtual bool getNextLimitPlane(NxVec3 & planeNormal, NxReal & planeD) = 0;
X	virtual NxJointType  getType() const = 0;
X	virtual void* is(NxJointType) const = 0;
#	virtual NxRevoluteJoint* isRevoluteJoint() { return (NxRevoluteJoint*)is(NX_JOINT_REVOLUTE);}
#	virtual NxPointInPlaneJoint* isPointInPlaneJoint() { return (NxPointInPlaneJoint*)is(NX_JOINT_POINT_IN_PLANE);}
#	virtual NxPointOnLineJoint* isPointOnLineJoint() { return (NxPointOnLineJoint*)is(NX_JOINT_POINT_ON_LINE);}
#	NX_INLINE NxD6Joint* isD6Joint() { return (NxD6Joint*)is(NX_JOINT_D6);}
#	virtual NxPrismaticJoint* isPrismaticJoint() { return (NxPrismaticJoint*)is(NX_JOINT_PRISMATIC);}
#	virtual NxCylindricalJoint* isCylindricalJoint() { return (NxCylindricalJoint*)is(NX_JOINT_CYLINDRICAL);}
#	virtual NxSphericalJoint* isSphericalJoint() { return (NxSphericalJoint*)is(NX_JOINT_SPHERICAL);}
#	virtual NxFixedJoint* isFixedJoint() { return (NxFixedJoint*)is(NX_JOINT_FIXED);}
#	virtual NxDistanceJoint* isDistanceJoint() { return (NxDistanceJoint*)is(NX_JOINT_DISTANCE);}
#	virtual NxPulleyJoint* isPulleyJoint() { return (NxPulleyJoint*)is(NX_JOINT_PULLEY);}
X	virtual	void			setName(const char*)		= 0;
X	virtual	const char*		getName()			const	= 0;
-	virtual	NxScene&		getScene() = 0;
*/


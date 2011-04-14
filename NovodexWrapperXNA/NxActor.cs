//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	public class NxActor
	{
		public static readonly float NX_NUM_SLEEP_FRAMES=(20.0f*0.02f);	//This is a 'legacy' setup that works out to 20 when using the standard time step.

		protected IntPtr nxActorPtr;

		public NxActor(IntPtr actorPointer)
			{nxActorPtr=actorPointer;}

		public static NxActor createFromPointer(IntPtr actorPointer)
		{
			if(actorPointer==IntPtr.Zero)
				{return null;}
			return new NxActor(actorPointer);
		}

		virtual public void internalBeforeRelease()
			{setName(null);}	//This frees the unmanaged memory for the name

		virtual public void internalAfterRelease()
			{nxActorPtr=IntPtr.Zero;}
		
		virtual public void destroy()
		{
			ParentScene.releaseActor(this);
			nxActorPtr=IntPtr.Zero;
			//NxScene.releaseActor() will remove the joints associated with this actor
		}

		public IntPtr UserData
		{
			get{return wrapper_Actor_getUserData(nxActorPtr);}
			set{wrapper_Actor_setUserData(nxActorPtr,value);}
		}

		public NxScene ParentScene
			{get{return getScene();}}

		public IntPtr NxActorPtr
			{get{return nxActorPtr;}}
			
		public string Name
		{
			get{return getName();}
			set{setName(value);}
		}
		
		public bool FlagDisableCollision
		{
			get{return readActorFlag(NxActorFlag.NX_AF_DISABLE_COLLISION);}
			set{setActorFlag(NxActorFlag.NX_AF_DISABLE_COLLISION,value);}
		}
		
		public bool FlagLockCom
		{
			get{return readActorFlag(NxActorFlag.NX_AF_LOCK_COM);}
			set{setActorFlag(NxActorFlag.NX_AF_LOCK_COM,value);}
		}

		public bool FlagDisableResponse
		{
			get{return readActorFlag(NxActorFlag.NX_AF_DISABLE_RESPONSE);}
			set{setActorFlag(NxActorFlag.NX_AF_DISABLE_RESPONSE,value);}
		}
		
		public bool FlagFluidActorReaction
		{
			get{return readActorFlag(NxActorFlag.NX_AF_FLUID_ACTOR_REACTION);}
			set{setActorFlag(NxActorFlag.NX_AF_FLUID_ACTOR_REACTION,value);}
		}

		public bool FlagFluidDisableCollision
		{
			get{return readActorFlag(NxActorFlag.NX_AF_FLUID_DISABLE_COLLISION);}
			set{setActorFlag(NxActorFlag.NX_AF_FLUID_DISABLE_COLLISION,value);}
		}

		public bool FlagDisableGravity
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_DISABLE_GRAVITY);}
			set{setBodyFlag(NxBodyFlag.NX_BF_DISABLE_GRAVITY,value);}
		}

		public bool FlagFrozen
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN,value);}
		}
		
		public bool FlagFrozenPos
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS,value);}
		}

		public bool FlagFrozenPosX
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS_X);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS_X,value);}
		}

		public bool FlagFrozenPosY
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS_Y);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS_Y,value);}
		}
		
		public bool FlagFrozenPosZ
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS_Z);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_POS_Z,value);}
		}
		
		public bool FlagFrozenRot
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT,value);}
		}

		public bool FlagFrozenRotX
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT_X);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT_X,value);}
		}

		public bool FlagFrozenRotY
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT_Y);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT_Y,value);}
		}
		
		public bool FlagFrozenRotZ
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT_Z);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FROZEN_ROT_Z,value);}
		}

		public bool FlagKinematic
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_KINEMATIC);}
			set{setBodyFlag(NxBodyFlag.NX_BF_KINEMATIC,value);}
		}

		public bool FlagVisualization
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_VISUALIZATION);}
			set{setBodyFlag(NxBodyFlag.NX_BF_VISUALIZATION,value);}
		}

		public bool FlagPoseSleepTest
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_POSE_SLEEP_TEST);}
			set{setBodyFlag(NxBodyFlag.NX_BF_POSE_SLEEP_TEST,value);}
		}

		public bool FlagFilterSleepVel
		{
			get{return readBodyFlag(NxBodyFlag.NX_BF_FILTER_SLEEP_VEL);}
			set{setBodyFlag(NxBodyFlag.NX_BF_FILTER_SLEEP_VEL,value);}
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

		public NxQuat GlobalOrientationQuat
		{
			get{return getGlobalOrientationQuat();}
			set{setGlobalOrientationQuat(value);}
		}

		public Matrix CMassGlobalPose
		{
			get{return getCMassGlobalPose();}
			set{setCMassGlobalPose(value);}
		}

		public Vector3 CMassGlobalPosition
		{
			get{return getCMassGlobalPosition();}
			set{setCMassGlobalPosition(value);}
		}

		public Matrix CMassGlobalOrientation
		{
			get{return getCMassGlobalOrientation();}
			set{setCMassGlobalOrientation(value);}
		}

		public Matrix CMassLocalPose
			{get{return getCMassLocalPose();}}

		public Vector3 CMassLocalPosition
			{get{return getCMassLocalPosition();}}

		public Matrix CMassLocalOrientation
			{get{return getCMassLocalOrientation();}}

		public float Mass
		{
			get{return getMass();}
			set{setMass(value);}
		}
		
		public Vector3 MassSpaceInertiaTensor
		{
			get{return getMassSpaceInertiaTensor();}
			set{setMassSpaceInertiaTensor(value);}
		}

		public Matrix GlobalInertiaTensor
			{get{return getGlobalInertiaTensor();}}

		public Matrix GlobalInertiaTensorInverse
			{get{return getGlobalInertiaTensorInverse();}}

		public float LinearDamping
		{
			get{return getLinearDamping();}
			set{setLinearDamping(value);}
		}

		public float AngularDamping
		{
			get{return getAngularDamping();}
			set{setAngularDamping(value);}
		}

		public Vector3 LinearVelocity
		{
			get{return getLinearVelocity();}
			set{setLinearVelocity(value);}
		}

		public Vector3 AngularVelocity
		{
			get{return getAngularVelocity();}
			set{setAngularVelocity(value);}
		}

		public float MaxAngularVelocity
		{
			get{return getMaxAngularVelocity();}
			set{setMaxAngularVelocity(value);}
		}

		public Vector3 LinearMomentum
		{
			get{return getLinearMomentum();}
			set{setLinearMomentum(value);}
		}

		public Vector3 AngularMomentum
		{
			get{return getAngularMomentum();}
			set{setAngularMomentum(value);}
		}

		public float KineticEnergy
			{get{return computeKineticEnergy();}}

		public float SleepLinearVelocity
		{
			get{return getSleepLinearVelocity();}
			set{setSleepLinearVelocity(value);}
		}

		public float SleepAngularVelocity
		{
			get{return getSleepAngularVelocity();}
			set{setSleepAngularVelocity(value);}
		}

		public ushort Group
		{
			get{return getGroup();}
			set{setGroup(value);}
		}

		public uint SolverIterationCount
		{
			get{return getSolverIterationCount();}
			set{setSolverIterationCount(value);}
		}

		public float CCDMotionThreshold
		{
			get{return getCCDMotionThreshold();}
			set{setCCDMotionThreshold(value);}
		}


			

		virtual public NxScene getScene()
			{return NxScene.createFromPointer(wrapper_Actor_getScene(nxActorPtr));}
	
		virtual public NxJoint[] getJoints()
		{
			ArrayList jointList=new ArrayList();
			NxJoint[] jointArray=ParentScene.getJoints();
			foreach(NxJoint joint in jointArray)
			{
				if(joint.Actor0.nxActorPtr==this.nxActorPtr)
					{jointList.Add(joint);}
				if(joint.Actor1.nxActorPtr==this.nxActorPtr)
					{jointList.Add(joint);}
			}
			
			jointArray=new NxJoint[jointList.Count];
			for(int i=0;i<jointList.Count;i++)
				{jointArray[i]=(NxJoint)jointList[i];}
			
			return jointArray;
		}
		
		//This gets all other actors connected to this actor by joints
		virtual public NxActor[] getJointSiblings()
		{
			ArrayList actorList=new ArrayList();
			
			NxJoint[] jointArray=getJoints();
			foreach(NxJoint joint in jointArray)
			{
				NxActor actor0=joint.Actor0;
				NxActor actor1=joint.Actor1;
				
				if(actor0.nxActorPtr!=this.nxActorPtr)
					{actorList.Add(actor0);}
				if(actor1.nxActorPtr!=this.nxActorPtr)
					{actorList.Add(actor1);}
			}
			
			NxActor[] actorArray=new NxActor[actorList.Count];
			for(int i=0;i<actorList.Count;i++)
				{actorArray[i]=(NxActor)actorList[i];}
			
			return actorArray;
		}
		
		//Imagine you grab an object and pull on it. All the objects interconnected by joints will move because they are associated into a common "blob". This will find that "blob" based upon this actor.
		//If you have something like a ragdoll and you call getActorBlob() on say the head it will return every actor in the ragdoll
		virtual public NxActor[] getActorBlob()
			{return getAllActorsConnectedByJoints();}
		
		//Identical to getActorBlob(). See getActorBlob() for description
		virtual public NxActor[] getAllActorsConnectedByJoints()
		{
			ArrayList blobPtrList=new ArrayList();
			recurseGetAllActorsConnectedByJoints(this,blobPtrList);
			
			NxActor[] actorArray=new NxActor[blobPtrList.Count];
			for(int i=0;i<blobPtrList.Count;i++)
				{actorArray[i]=new NxActor((IntPtr)blobPtrList[i]);}
			
			return actorArray;
		}
		
		private void recurseGetAllActorsConnectedByJoints(NxActor actor,ArrayList blobPtrList)
		{
			if(blobPtrList.Contains(actor.nxActorPtr))
				{return;}
			else
				{blobPtrList.Add(actor.nxActorPtr);}
				
			NxActor[] siblingArray=actor.getJointSiblings();
			foreach(NxActor siblingActor in siblingArray)
				{recurseGetAllActorsConnectedByJoints(siblingActor,blobPtrList);}
		}

		//This gets all the joints in an "actor blob". See getActorBlob() for description.
		virtual public NxJoint[] getJointBlob()
			{return getAllJointsFromConnectedActors();}
		
		//This gets all the joints in an "actor blob". See getActorBlob() for description.
		virtual public NxJoint[] getAllJointsFromConnectedActors()
		{
			ArrayList jointPtrList=new ArrayList();
			NxActor[] actorArray=getAllActorsConnectedByJoints();
			
			foreach(NxActor actor in actorArray)
			{
				NxJoint[] joints=actor.getJoints();
				foreach(NxJoint joint in joints)
				{
					if(!jointPtrList.Contains(joint.NxJointPtr))
						{jointPtrList.Add(joint.NxJointPtr);}
				}
			}
			
			NxJoint[] jointArray=new NxJoint[jointPtrList.Count];
			for(int i=0;i<jointPtrList.Count;i++)
				{jointArray[i]=NxJoint.createFromPointer((IntPtr)jointPtrList[i]);}
			return jointArray;
		}
		
		





		virtual public void setGlobalPose(Matrix globalPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalPose);
			wrapper_Actor_setGlobalPose(nxActorPtr,ref mat);
		}

		virtual public void setGlobalPosition(Vector3 globalPosition)
			{wrapper_Actor_setGlobalPosition(nxActorPtr,ref globalPosition);}

		virtual public void setGlobalOrientation(Matrix globalOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalOrientation);
			wrapper_Actor_setGlobalOrientation(nxActorPtr,ref mat);
		}

		virtual public Matrix getGlobalPose()
		{
			NxMat34 matrix;
			wrapper_Actor_getGlobalPose(nxActorPtr,out matrix);
			return NovodexUtil.convertNxMat34ToMatrix(matrix);
		}
		
		virtual public Vector3 getGlobalPosition()
		{
			Vector3 pos;
			wrapper_Actor_getGlobalPosition(nxActorPtr,out pos);
			return pos;
		}
		
		virtual public Matrix getGlobalOrientation()
		{
			NxMat34 mat;
			wrapper_Actor_getGlobalOrientation(nxActorPtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public void moveGlobalPose(Matrix globalPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalPose);
			wrapper_Actor_moveGlobalPose(nxActorPtr,ref mat);
		}

		virtual public void moveGlobalPosition(Vector3 globalPosition)
			{wrapper_Actor_moveGlobalPosition(nxActorPtr,ref globalPosition);}

		virtual public void moveGlobalOrientation(Matrix globalOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalOrientation);
			wrapper_Actor_moveGlobalOrientation(nxActorPtr,ref mat);
		}

		virtual public void raiseActorFlag(NxActorFlag actorFlag)
			{wrapper_Actor_raiseActorFlag(nxActorPtr,actorFlag);}

		virtual public void clearActorFlag(NxActorFlag actorFlag)
			{wrapper_Actor_clearActorFlag(nxActorPtr,actorFlag);}
		
		virtual public bool readActorFlag(NxActorFlag actorFlag)
			{return wrapper_Actor_readActorFlag(nxActorPtr,actorFlag);}

		virtual public void raiseBodyFlag(NxBodyFlag bodyFlag)
			{wrapper_Actor_raiseBodyFlag(nxActorPtr,bodyFlag);}

		virtual public void clearBodyFlag(NxBodyFlag bodyFlag)
			{wrapper_Actor_clearBodyFlag(nxActorPtr,bodyFlag);}
		
		virtual public bool readBodyFlag(NxBodyFlag bodyFlag)
			{return wrapper_Actor_readBodyFlag(nxActorPtr,bodyFlag);}

		private void setActorFlag(NxActorFlag actorFlag,bool value)
		{
			if(value)
				{raiseActorFlag(actorFlag);}
			else
				{clearActorFlag(actorFlag);}
		}

		private void setBodyFlag(NxBodyFlag bodyFlag,bool value)
		{
			if(value)
				{raiseBodyFlag(bodyFlag);}
			else
				{clearBodyFlag(bodyFlag);}
		}

		virtual public bool isGroupSleeping()
			{return wrapper_Actor_isGroupSleeping(nxActorPtr);}
		
		virtual	public bool isSleeping()
			{return wrapper_Actor_isSleeping(nxActorPtr);}
		
		virtual	public void wakeUp()
			{wakeUp(NX_NUM_SLEEP_FRAMES);}
		
		virtual	public void wakeUp(float wakeCounterValue)
			{wrapper_Actor_wakeUp(nxActorPtr,wakeCounterValue);}
		
		virtual	public void putToSleep()
			{wrapper_Actor_putToSleep(nxActorPtr);}

		virtual public void setGroup(ushort group)
			{wrapper_Actor_setGroup(nxActorPtr,group);}

		virtual public ushort getGroup()
			{return wrapper_Actor_getGroup(nxActorPtr);}

		virtual public bool isDynamic()
			{return  wrapper_Actor_isDynamic(nxActorPtr);}
		
		virtual public void setMass(float mass)
			{wrapper_Actor_setMass(nxActorPtr,mass);}
		
		virtual public float getMass()
			{return wrapper_Actor_getMass(nxActorPtr);}

		//If totalMass is zero it calculates using densitry. If density is 0 it calculates using totalMass
		virtual public void updateMassFromShapes(float density,float totalMass)
			{wrapper_Actor_updateMassFromShapes(nxActorPtr,density,totalMass);}

		virtual public void setDensity(float density)
			{updateMassFromShapes(density,0);}
			
		//This uses a trick to get the density and is probably really slow
		virtual public float getDensity()
		{
			float currentMass=getMass();
			setDensity(1);
			float unitMass=getMass();
			float density=currentMass/unitMass;
			updateMassFromShapes(0,currentMass);
			return density;
		}

		virtual public void addForceAtPos(Vector3 worldForce,Vector3 worldPosition,NxForceMode forceMode)
			{wrapper_Actor_addForceAtPos(nxActorPtr,ref worldForce,ref worldPosition,forceMode);}

		virtual public void addForceAtLocalPos(Vector3 worldForce,Vector3 localPosition,NxForceMode forceMode)
			{wrapper_Actor_addForceAtLocalPos(nxActorPtr,ref worldForce,ref localPosition,forceMode);}

		virtual public void addLocalForceAtPos(Vector3 localForce,Vector3 worldPosition,NxForceMode forceMode)
			{wrapper_Actor_addLocalForceAtPos(nxActorPtr,ref localForce,ref worldPosition,forceMode);}

		virtual public void addLocalForceAtLocalPos(Vector3 localForce,Vector3 localPosition,NxForceMode forceMode)
			{wrapper_Actor_addLocalForceAtLocalPos(nxActorPtr,ref localForce,ref localPosition,forceMode);}
		
		virtual public void addForce(Vector3 worldForce,NxForceMode forceMode)
			{wrapper_Actor_addForce(nxActorPtr,ref worldForce,forceMode);}

		virtual public void addLocalForce(Vector3 localForce,NxForceMode forceMode)
			{wrapper_Actor_addLocalForce(nxActorPtr,ref localForce,forceMode);}

		virtual public void addTorque(Vector3 worldTorque,NxForceMode forceMode)
			{wrapper_Actor_addTorque(nxActorPtr,ref worldTorque,forceMode);}

		virtual public void addLocalTorque(Vector3 localTorque,NxForceMode forceMode)
			{wrapper_Actor_addLocalTorque(nxActorPtr,ref localTorque,forceMode);}
		
		virtual public void setLinearVelocity(Vector3 linearVelocity)
			{wrapper_Actor_setLinearVelocity(nxActorPtr,ref linearVelocity);}

		virtual public Vector3 getLinearVelocity()
		{
			Vector3 linearVelocity;
			wrapper_Actor_getLinearVelocity(nxActorPtr,out linearVelocity);
			return linearVelocity;
		}
		
		virtual public void setAngularVelocity(Vector3 angularVelocity)
			{wrapper_Actor_setAngularVelocity(nxActorPtr,ref angularVelocity);}

		virtual public Vector3 getAngularVelocity()
		{
			Vector3 angularVelocity;
			wrapper_Actor_getAngularVelocity(nxActorPtr,out angularVelocity);
			return angularVelocity;
		}

		virtual public void setMaxAngularVelocity(float maxAngularVelocity)
			{wrapper_Actor_setMaxAngularVelocity(nxActorPtr,maxAngularVelocity);}

		virtual public float getMaxAngularVelocity()
			{return wrapper_Actor_getMaxAngularVelocity(nxActorPtr);}

		virtual public void setLinearMomentum(Vector3 linearMomentum)
			{wrapper_Actor_setLinearMomentum(nxActorPtr,ref linearMomentum);}

		virtual public Vector3 getLinearMomentum()
		{
			Vector3 linearMomentum;
			wrapper_Actor_getLinearMomentum(nxActorPtr,out linearMomentum);
			return linearMomentum;
		}

		virtual public void setAngularMomentum(Vector3 angularMomentum)
			{wrapper_Actor_setAngularMomentum(nxActorPtr,ref angularMomentum);}

		virtual public Vector3 getAngularMomentum()
		{
			Vector3 angularMomentum;
			wrapper_Actor_getAngularMomentum(nxActorPtr,out angularMomentum);
			return angularMomentum;
		}

		virtual public int getNbShapes()
			{return wrapper_Actor_getNbShapes(nxActorPtr);}

		virtual public NxShape createShape(NxShapeDesc shapeDesc)
		{
			IntPtr shapePointer=wrapper_Actor_createShape(nxActorPtr,shapeDesc);
			return NxShape.createFromPointer(shapePointer);
		}
		
		virtual public void releaseShape(NxShape shape)
		{
			shape.internalBeforeRelease();
			wrapper_Actor_releaseShape(nxActorPtr,shape.NxShapePtr);
			shape.internalAfterRelease();
		}

		virtual public NxShape[] getShapes()
		{
			int numShapes=getNbShapes();
			NxShape[] shapeArray=new NxShape[numShapes];

			IntPtr shapesPointer=wrapper_Actor_getShapes(nxActorPtr);
			unsafe
			{
				int* p=(int*)shapesPointer.ToPointer();
				for(int i=0;i<numShapes;i++)
					{shapeArray[i]=NxShape.createFromPointer(new IntPtr(p[i]));}
			}
			
			return shapeArray;
		}

		virtual public NxShape getShape(int shapeIndex)
		{
			if(shapeIndex<0 || shapeIndex>=getNbShapes())
				{return null;}

			IntPtr shapesPointer=wrapper_Actor_getShapes(nxActorPtr);
			unsafe
			{
				int* p=(int*)shapesPointer.ToPointer();
				return NxShape.createFromPointer(new IntPtr(p[shapeIndex]));
			}
		}

		virtual public NxShape getFirstShape()
			{return getShape(0);}

		virtual public NxShape getLastShape()
			{return getShape(getNbShapes()-1);}

		virtual public float computeKineticEnergy()
			{return wrapper_Actor_computeKineticEnergy(nxActorPtr);}

		virtual public void setLinearDamping(float linearDamping)
			{wrapper_Actor_setLinearDamping(nxActorPtr,linearDamping);}
		
		virtual public float getLinearDamping()
			{return wrapper_Actor_getLinearDamping(nxActorPtr);}

		virtual public void setAngularDamping(float angularDamping)
			{wrapper_Actor_setAngularDamping(nxActorPtr,angularDamping);}
		
		virtual public float getAngularDamping()
			{return wrapper_Actor_getAngularDamping(nxActorPtr);}

		virtual public void setName(String name)
			{wrapper_Actor_setName(nxActorPtr,name);}		
		
		virtual public string getName()
			{return wrapper_Actor_getName(nxActorPtr);}		

		virtual public Vector3 getPointVelocity(IntPtr actor,ref Vector3 point)
		{
			Vector3 velocity;
			wrapper_Actor_getPointVelocity(nxActorPtr,ref point,out velocity);
			return velocity;
		}

		virtual public Vector3 getLocalPointVelocity(IntPtr actor,ref Vector3 point)
		{
			Vector3 velocity;
			wrapper_Actor_getLocalPointVelocity(nxActorPtr,ref point,out velocity);
			return velocity;
		}

		virtual public float getSleepLinearVelocity()
			{return wrapper_Actor_getSleepLinearVelocity(nxActorPtr);}

		virtual public void setSleepLinearVelocity(float threshold)
			{wrapper_Actor_setSleepLinearVelocity(nxActorPtr,threshold);}

		virtual public float getSleepAngularVelocity()
			{return wrapper_Actor_getSleepAngularVelocity(nxActorPtr);}

		virtual public void setSleepAngularVelocity(float threshold)
			{wrapper_Actor_setSleepAngularVelocity(nxActorPtr,threshold);}

		virtual public bool saveBodyToDesc(NxBodyDesc bodyDesc)
			{return wrapper_Actor_saveBodyToDesc(nxActorPtr,bodyDesc);}
		
		virtual public NxBodyDesc getBodyDesc(bool resetBodyMassAndInertia)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			saveBodyToDesc(bodyDesc);
			
			if(resetBodyMassAndInertia)
			{
				bodyDesc.mass=0;
				bodyDesc.massSpaceInertia=new Vector3(0,0,0);
			}
				
			return bodyDesc;
		}

		virtual public void saveToDesc(NxActorDesc actorDesc)
		{
			wrapper_Actor_saveToDesc(nxActorPtr,ref actorDesc.globalPose,ref actorDesc.density,ref actorDesc.flags,ref actorDesc.group,ref actorDesc.userData,ref actorDesc.internalNamePtr);
			actorDesc.name=getName();
		}

		//Pass in true for resetRetrievedBodyMassAndInertia if you plan on using the actorDesc to create another actor. If you have a non-zero actorDesc.density the bodyDesc mass and inertia must be zero or else the actorDesc is invalid. If you want to keep the bodyDesc's mass and inertia then you could set actorDesc.density to zero before creating an actor with the actorDesc.
		virtual public NxActorDesc getActorDesc(bool getShapes,bool getBodyDesc,bool resetRetrievedBodyMassAndInertia)
		{
			NxActorDesc actorDesc=NxActorDesc.Default;
			saveToDesc(actorDesc);
			
			if(getBodyDesc)
				{actorDesc.BodyDesc=new NxBodyDesc(this.getBodyDesc(resetRetrievedBodyMassAndInertia));}
			
			if(getShapes)
			{
				NxShape[] shapeArray=this.getShapes();
				actorDesc.clearShapeDesc();
				foreach(NxShape shape in shapeArray)
					{actorDesc.addShapeDesc(shape.getShapeDesc());}
			}
			
			return actorDesc;
		}

		virtual public void setGlobalOrientationQuat(NxQuat globalOrientationQuat)
			{wrapper_Actor_setGlobalOrientationQuat(nxActorPtr,ref globalOrientationQuat);}

		virtual public NxQuat getGlobalOrientationQuat()
		{
			NxQuat globalOrientationQuat;
			wrapper_Actor_getGlobalOrientationQuat(nxActorPtr,out globalOrientationQuat);
			return globalOrientationQuat;
		}
	
		virtual public void setSolverIterationCount(uint iterationCount)
			{wrapper_Actor_setSolverIterationCount(nxActorPtr,iterationCount);}

		virtual public uint getSolverIterationCount()
			{return wrapper_Actor_getSolverIterationCount(nxActorPtr);}

		virtual public void setCMassOffsetLocalPose(Matrix offsetLocalPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(offsetLocalPose);
			wrapper_Actor_setCMassOffsetLocalPose(nxActorPtr,ref mat);
		}

		virtual public void setCMassOffsetLocalPosition(Vector3 offsetLocalPosition)
			{wrapper_Actor_setCMassOffsetLocalPosition(nxActorPtr,ref offsetLocalPosition);}

		virtual public void setCMassOffsetLocalOrientation(Matrix offsetLocalOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(offsetLocalOrientation);
			wrapper_Actor_setCMassOffsetLocalOrientation(nxActorPtr,ref mat);
		}

		virtual public void setCMassOffsetGlobalPose(Matrix offsetGlobalPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(offsetGlobalPose);
			wrapper_Actor_setCMassOffsetGlobalPose(nxActorPtr,ref mat);
		}

		virtual public void setCMassOffsetGlobalPosition(Vector3 offsetGlobalPosition)
			{wrapper_Actor_setCMassOffsetGlobalPosition(nxActorPtr,ref offsetGlobalPosition);}

		virtual public void setCMassOffsetGlobalOrientation(Matrix offsetGlobalOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(offsetGlobalOrientation);
			wrapper_Actor_setCMassOffsetGlobalOrientation(nxActorPtr,ref mat);
		}

		virtual public void setCMassGlobalPose(Matrix globalPose)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalPose);
			wrapper_Actor_setCMassGlobalPose(nxActorPtr,ref mat);
		}

		virtual public void setCMassGlobalPosition(Vector3 globalPosition)
			{wrapper_Actor_setCMassGlobalPosition(nxActorPtr,ref globalPosition);}

		virtual public void setCMassGlobalOrientation(Matrix globalOrientation)
		{
			NxMat34 mat=NovodexUtil.convertMatrixToNxMat34(globalOrientation);
			wrapper_Actor_setCMassGlobalOrientation(nxActorPtr,ref mat);
		}

		virtual public Matrix getCMassLocalPose()
		{
			NxMat34 matrix;
			wrapper_Actor_getCMassLocalPose(nxActorPtr,out matrix);
			return NovodexUtil.convertNxMat34ToMatrix(matrix);
		}
		
		virtual public Vector3 getCMassLocalPosition()
		{
			Vector3 pos;
			wrapper_Actor_getCMassLocalPosition(nxActorPtr,out pos);
			return pos;
		}
		
		virtual public Matrix getCMassLocalOrientation()
		{
			NxMat34 mat;
			wrapper_Actor_getCMassLocalOrientation(nxActorPtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public Matrix getCMassGlobalPose()
		{
			NxMat34 matrix;
			wrapper_Actor_getCMassGlobalPose(nxActorPtr,out matrix);
			return NovodexUtil.convertNxMat34ToMatrix(matrix);
		}
		
		virtual public Vector3 getCMassGlobalPosition()
		{
			Vector3 pos;
			wrapper_Actor_getCMassGlobalPosition(nxActorPtr,out pos);
			return pos;
		}
		
		virtual public Matrix getCMassGlobalOrientation()
		{
			NxMat34 mat;
			wrapper_Actor_getCMassGlobalOrientation(nxActorPtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}
		
		virtual public void setMassSpaceInertiaTensor(Vector3 massSpaceInertiaTensor)
			{wrapper_Actor_setMassSpaceInertiaTensor(nxActorPtr,ref massSpaceInertiaTensor);}

		virtual public Vector3 getMassSpaceInertiaTensor()
		{
			Vector3 massSpaceInertiaTensor;
			wrapper_Actor_getMassSpaceInertiaTensor(nxActorPtr,out massSpaceInertiaTensor);
			return massSpaceInertiaTensor;
		}

		virtual public Matrix getGlobalInertiaTensor()
		{
			NxMat34 mat;
			wrapper_Actor_getGlobalInertiaTensor(nxActorPtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public Matrix getGlobalInertiaTensorInverse()
		{
			NxMat34 mat;
			wrapper_Actor_getGlobalInertiaTensorInverse(nxActorPtr,out mat);
			return NovodexUtil.convertNxMat34ToMatrix(mat);
		}

		virtual public void moveGlobalOrientationQuat(NxQuat globalOrientationQuat)
			{wrapper_Actor_moveGlobalOrientationQuat(nxActorPtr,ref globalOrientationQuat);}

		virtual public void setCCDMotionThreshold(float thresh)
			{wrapper_Actor_setCCDMotionThreshold(nxActorPtr,thresh);}

		virtual public float getCCDMotionThreshold()
			{return wrapper_Actor_getCCDMotionThreshold(nxActorPtr);}
		
		
	
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setGlobalPose(IntPtr actor,ref NxMat34 globalPose);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setGlobalPosition(IntPtr actor,ref Vector3 globapPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setGlobalOrientation(IntPtr actor,ref NxMat34 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getGlobalPose(IntPtr actor,out NxMat34 globalPose);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getGlobalPosition(IntPtr actor,out Vector3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getGlobalOrientation(IntPtr actor,out NxMat34 globalPosition);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_moveGlobalPose(IntPtr actor,ref NxMat34 globalPose);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_moveGlobalPosition(IntPtr actor,ref Vector3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_moveGlobalOrientation(IntPtr actor,ref NxMat34 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_raiseActorFlag(IntPtr actor,NxActorFlag actorFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_clearActorFlag(IntPtr actor,NxActorFlag actorFlag);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Actor_readActorFlag(IntPtr actor,NxActorFlag actorFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_raiseBodyFlag(IntPtr actor,NxBodyFlag bodyFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_clearBodyFlag(IntPtr actor,NxBodyFlag bodyFlag);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Actor_readBodyFlag(IntPtr actor,NxBodyFlag bodyFlag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Actor_isGroupSleeping(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Actor_isSleeping(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_wakeUp(IntPtr actor,float wakeCounterValue);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_putToSleep(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setGroup(IntPtr actor,ushort group);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_Actor_getGroup(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Actor_isDynamic(IntPtr actor);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setMass(IntPtr actor,float mass);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getMass(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_updateMassFromShapes(IntPtr actor,float density,float totalMass);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addForceAtPos(IntPtr actor,ref Vector3 worldForce,ref Vector3 worldPosition,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addForceAtLocalPos(IntPtr actor,ref Vector3 worldForce,ref Vector3 localPosition,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addLocalForceAtPos(IntPtr actor,ref Vector3 localForce,ref Vector3 worldPosition,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addLocalForceAtLocalPos(IntPtr actor,ref Vector3 localForce,ref Vector3 localPosition,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addForce(IntPtr actor,ref Vector3 worldForce,NxForceMode forceMode);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addLocalForce(IntPtr actor,ref Vector3 localForce,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addTorque(IntPtr actor,ref Vector3 worldTorque,NxForceMode forceMode);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_addLocalTorque(IntPtr actor,ref Vector3 localTorque,NxForceMode forceMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setLinearVelocity(IntPtr actor,ref Vector3 linearVelocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getLinearVelocity(IntPtr actor,out Vector3 linearVelocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setAngularVelocity(IntPtr actor,ref Vector3 angularVelocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getAngularVelocity(IntPtr actor,out Vector3 angularVelocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setMaxAngularVelocity(IntPtr actor,float maxAngularVelocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getMaxAngularVelocity(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setLinearMomentum(IntPtr actor,ref Vector3 linearMomentum);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getLinearMomentum(IntPtr actor,out Vector3 linearMomentum);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setAngularMomentum(IntPtr actor,ref Vector3 angularMomentum);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getAngularMomentum(IntPtr actor,out Vector3 angularMomentum);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Actor_getNbShapes(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Actor_createShape(IntPtr actor,NxShapeDesc shapeDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_releaseShape(IntPtr actor,IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Actor_getShapes(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_computeKineticEnergy(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setLinearDamping(IntPtr actor,float linearDamping);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getLinearDamping(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setAngularDamping(IntPtr actor,float angularDamping);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getAngularDamping(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setName(IntPtr actor,string name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern string wrapper_Actor_getName(IntPtr actor);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getPointVelocity(IntPtr actor,ref Vector3 point,out Vector3 velocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getLocalPointVelocity(IntPtr actor,ref Vector3 point,out Vector3 velocity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getSleepLinearVelocity(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setSleepLinearVelocity(IntPtr actor,float threshold);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getSleepAngularVelocity(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setSleepAngularVelocity(IntPtr actor,float threshold);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Actor_saveBodyToDesc(IntPtr actor,NxBodyDesc bodyDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_saveToDesc(IntPtr actor,ref NxMat34 globalPose,ref float density,ref uint flags,ref ushort group,ref IntPtr userData,ref IntPtr name);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setGlobalOrientationQuat(IntPtr actor,ref NxQuat globalOrientationQuat);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getGlobalOrientationQuat(IntPtr actor,out NxQuat globalOrientationQuat);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Actor_getScene(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setSolverIterationCount(IntPtr actor,uint iterationCount);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Actor_getSolverIterationCount(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setUserData(IntPtr actor,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Actor_getUserData(IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassOffsetLocalPose(IntPtr actor,ref NxMat34 offsetLocalPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassOffsetLocalPosition(IntPtr actor,ref Vector3 offsetLocalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassOffsetLocalOrientation(IntPtr actor,ref NxMat34 offsetLocalOrienation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassOffsetGlobalPose(IntPtr actor,ref NxMat34 offsetGlobalPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassOffsetGlobalPosition(IntPtr actor,ref Vector3 offsetGlobalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassOffsetGlobalOrientation(IntPtr actor,ref NxMat34 offsetGlobalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassGlobalPose(IntPtr actor,ref NxMat34 globalPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassGlobalPosition(IntPtr actor,ref Vector3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCMassGlobalOrientation(IntPtr actor,ref NxMat34 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getCMassLocalPose(IntPtr actor,out NxMat34 localPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getCMassLocalPosition(IntPtr actor,out Vector3 localPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getCMassLocalOrientation(IntPtr actor,out NxMat34 localOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getCMassGlobalPose(IntPtr actor,out NxMat34 globalPose);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getCMassGlobalPosition(IntPtr actor,out Vector3 globalPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getCMassGlobalOrientation(IntPtr actor,out NxMat34 globalOrientation);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setMassSpaceInertiaTensor(IntPtr actor,ref Vector3 massSpaceInertiaTensor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getMassSpaceInertiaTensor(IntPtr actor,out Vector3 massSpaceInertiaTensor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getGlobalInertiaTensor(IntPtr actor,out NxMat34 globalInertiaTensor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_getGlobalInertiaTensorInverse(IntPtr actor,out NxMat34 globalInertiaTensorInverse);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_moveGlobalOrientationQuat(IntPtr actor,ref NxQuat globalOrientationQuat);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Actor_setCCDMotionThreshold(IntPtr actor,float thresh);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Actor_getCCDMotionThreshold(IntPtr actor);
	}
}




/*
-	virtual		NxScene&		getScene() = 0;
-	virtual		void			saveToDesc(NxActorDescBase& desc) = 0;
-	virtual		void			setName(const char* name)		= 0;
-	virtual		const char*		getName()			const	= 0;
-	virtual		void			setGlobalPose(const NxMat34& mat)			= 0;
-	virtual		void			setGlobalPosition(const NxVec3& vec)		= 0;
-	virtual		void			setGlobalOrientation(const NxMat33& mat)	= 0;
-	virtual		void			setGlobalOrientationQuat(const NxQuat& mat)	= 0;
-	virtual		NxMat34 		getGlobalPose()			  const	= 0;
-	virtual		NxVec3 			getGlobalPosition()		  const	= 0;
-	virtual		NxMat33 		getGlobalOrientation()	  const	= 0; 
-	virtual		NxQuat 			getGlobalOrientationQuat()const	= 0;
-	virtual		void			moveGlobalPose(const NxMat34& mat)			= 0;
-	virtual		void			moveGlobalPosition(const NxVec3& vec)		= 0;
-	virtual		void			moveGlobalOrientation(const NxMat33& mat)	= 0;
o	virtual		void			moveGlobalOrientationQuat(const NxQuat& quat)	= 0;
-	virtual		NxShape*		createShape(const NxShapeDesc& desc)	= 0;
-	virtual		void			releaseShape(NxShape& shape) = 0;
-	virtual		NxU32			getNbShapes()		const	= 0;
-	virtual		NxShape*const *	getShapes()			const	= 0;
-	virtual		void			setGroup(NxActorGroup actorGroup)		 = 0;
-	virtual		NxActorGroup	getGroup() const			 = 0;
-	virtual		void			raiseActorFlag(NxActorFlag actorFlag)			= 0;
-	virtual		void			clearActorFlag(NxActorFlag actorFlag)			= 0;
-	virtual		bool			readActorFlag(NxActorFlag actorFlag)	const	= 0;
-	virtual		bool			isDynamic()	const			= 0;
-	virtual		void			setCMassOffsetLocalPose(const NxMat34& mat)			= 0;
-	virtual		void			setCMassOffsetLocalPosition(const NxVec3& vec)		= 0;
-	virtual		void			setCMassOffsetLocalOrientation(const NxMat33& mat)	= 0;
-	virtual		void			setCMassOffsetGlobalPose(const NxMat34& mat)		= 0;
-	virtual		void			setCMassOffsetGlobalPosition(const NxVec3& vec)		= 0;
-	virtual		void			setCMassOffsetGlobalOrientation(const NxMat33& mat)	= 0;
-	virtual		void			setCMassGlobalPose(const NxMat34& mat)			= 0;
-	virtual		void			setCMassGlobalPosition(const NxVec3& vec)		= 0;
-	virtual		void			setCMassGlobalOrientation(const NxMat33& mat)	= 0;
-	virtual		NxMat34 		getCMassLocalPose()					const	= 0;
-	virtual		NxVec3 			getCMassLocalPosition()				const	= 0; 
-	virtual		NxMat33 		getCMassLocalOrientation()			const	= 0;
-	virtual		NxMat34 		getCMassGlobalPose()				const  = 0;
-	virtual		NxVec3 			getCMassGlobalPosition()			const  = 0;
-	virtual		NxMat33 		getCMassGlobalOrientation()			const = 0;
-	virtual		void			setMass(NxReal mass) = 0;
-	virtual		NxReal			getMass() const = 0;
-	virtual		void			setMassSpaceInertiaTensor(const NxVec3& m) = 0;
-	virtual		NxVec3			getMassSpaceInertiaTensor()			const = 0;
-	virtual		NxMat33			getGlobalInertiaTensor()			const = 0;
-	virtual		NxMat33			getGlobalInertiaTensorInverse()		const = 0;
-	virtual		void			updateMassFromShapes(NxReal density, NxReal totalMass)		= 0;
-	virtual		void			setLinearDamping(NxReal linDamp) = 0;
-	virtual		NxReal			getLinearDamping() const = 0;
-	virtual		void			setAngularDamping(NxReal angDamp) = 0;
-	virtual		NxReal			getAngularDamping() const = 0;
-	virtual		void			setLinearVelocity(const NxVec3& linVel) = 0;
-	virtual		void			setAngularVelocity(const NxVec3& angVel) = 0;
-	virtual		NxVec3			getLinearVelocity()		const = 0;
-	virtual		NxVec3			getAngularVelocity()	const = 0;
-	virtual		void			setMaxAngularVelocity(NxReal maxAngVel) = 0;
-	virtual		NxReal			getMaxAngularVelocity()	const = 0; 
o	virtual		void			setCCDMotionThreshold(NxReal thresh) = 0;
o	virtual		NxReal			getCCDMotionThreshold()	const = 0; 
-	virtual		void			setLinearMomentum(const NxVec3& linMoment) = 0;
-	virtual		void			setAngularMomentum(const NxVec3& angMoment) = 0;
-	virtual		NxVec3			getLinearMomentum()		const = 0;
-	virtual		NxVec3			getAngularMomentum()	const = 0;
-	virtual		void			addForceAtPos(const NxVec3& force, const NxVec3& pos, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addForceAtLocalPos(const NxVec3& force, const NxVec3& pos, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addLocalForceAtPos(const NxVec3& force, const NxVec3& pos, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addLocalForceAtLocalPos(const NxVec3& force, const NxVec3& pos, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addForce(const NxVec3& force, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addLocalForce(const NxVec3& force, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addTorque(const NxVec3& torque, NxForceMode mode = NX_FORCE) = 0;
-	virtual		void			addLocalTorque(const NxVec3& torque, NxForceMode mode = NX_FORCE) = 0;
-	virtual		NxReal			computeKineticEnergy() const = 0;
-   virtual		NxVec3			getPointVelocity(const NxVec3& point)	const = 0;
-	virtual		NxVec3			getLocalPointVelocity(const NxVec3& point)	const	= 0;
-	virtual		bool			isGroupSleeping() const = 0;
-	virtual		bool			isSleeping() const = 0;
-	virtual		NxReal			getSleepLinearVelocity() const = 0;
-	virtual		void			setSleepLinearVelocity(NxReal threshold) = 0;
-	virtual		NxReal			getSleepAngularVelocity() const = 0;
-	virtual		void			setSleepAngularVelocity(NxReal threshold) = 0;
-	virtual		void			wakeUp(NxReal wakeCounterValue=NX_NUM_SLEEP_FRAMES)	= 0;
-	virtual		void			putToSleep()	= 0;
-	virtual		void			raiseBodyFlag(NxBodyFlag bodyFlag)				= 0;
-	virtual		void			clearBodyFlag(NxBodyFlag bodyFlag)				= 0;
-	virtual		bool			readBodyFlag(NxBodyFlag bodyFlag)		const	= 0;
-	virtual		bool			saveBodyToDesc(NxBodyDesc& bodyDesc) = 0;
-	virtual		void			setSolverIterationCount(NxU32 iterCount) = 0;
-	virtual		NxU32			getSolverIterationCount() const = 0;

-	void*			userData;	//!< user can assign this to whatever, usually to create a 1:1 relationship with a user object.
*/





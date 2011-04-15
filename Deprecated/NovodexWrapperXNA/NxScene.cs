//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


            


namespace NovodexWrapper
{
	public class NxScene
	{
		protected IntPtr nxScenePtr;
		
		private IntPtr[] overlapPointerArray=new IntPtr[64]; //This is used in the internalOverlap...() methods
		private IntPtr[] cullPointerArray=new IntPtr[64];    //This is used in the internalCull...() methods


		public NxScene(IntPtr scenePointer)
			{nxScenePtr=scenePointer;}
			
		static public NxScene createFromPointer(IntPtr scenePointer)
		{
			if(scenePointer==IntPtr.Zero)
				{return null;}
			return new NxScene(scenePointer);
		}
			
		virtual public void internalAfterRelease()
			{nxScenePtr=IntPtr.Zero;}
		
		
		public IntPtr UserData
		{
			get{return wrapper_Scene_getUserData(nxScenePtr);}
			set{wrapper_Scene_setUserData(nxScenePtr,value);}
		}
		
		virtual public NxActor createActor(NxActorDesc actorDesc)
		{
			IntPtr[] shapeDescPtrs=actorDesc.getShapeDescPtrs();
			IntPtr actorPtr=wrapper_Scene_createActorByParameters(nxScenePtr,ref actorDesc.globalPose,actorDesc.bodyDescPtr,actorDesc.density,actorDesc.flags,actorDesc.group,actorDesc.userData,actorDesc.internalNamePtr,shapeDescPtrs.Length,shapeDescPtrs);
			NxActor actor=NxActor.createFromPointer(actorPtr);
			if(actor!=null && actorDesc.name!=null)
				{actor.setName(actorDesc.name);}
			return actor;
		}
		
		virtual public void releaseActor(NxActor actor)
		{
			NxJoint[] jointArray=actor.getJoints();
			foreach(NxJoint joint in jointArray)
				{joint.destroy();}

			actor.internalBeforeRelease();
			wrapper_Scene_releaseActor(nxScenePtr,actor.NxActorPtr);
			actor.internalAfterRelease();
		}

		virtual public int getNbActors()
			{return wrapper_Scene_getNbActors(nxScenePtr);}

		virtual public int getNbStaticShapes()
			{return wrapper_Scene_getNbStaticShapes(nxScenePtr);}

		virtual public int getNbDynamicShapes()
			{return wrapper_Scene_getNbDynamicShapes(nxScenePtr);}

		virtual public int getTotalNbShapes()
			{return wrapper_Scene_getTotalNbShapes(nxScenePtr);}

		virtual public int getNbMaterials()
			{return wrapper_Scene_getNbMaterials(nxScenePtr);}

		virtual public NxActor[] getActors()
		{
			int numActors=getNbActors();
			NxActor[] actorArray=new NxActor[numActors];
			IntPtr actorsPtr=wrapper_Scene_getActors(nxScenePtr);

			unsafe
			{
				int* ptr=(int*)actorsPtr.ToPointer();
				for(int i=0;i<numActors;i++)
					{actorArray[i]=new NxActor(new IntPtr(ptr[i]));}
			}

			return actorArray;
		}

		virtual public NxActor getActor(int actorIndex)
		{
			if(actorIndex<0 || actorIndex>=getNbActors())
				{return null;}

			IntPtr actorsPtr=wrapper_Scene_getActors(nxScenePtr);
			unsafe
			{
				int* ptr=(int*)actorsPtr.ToPointer();
				return new NxActor(new IntPtr(ptr[actorIndex]));
			}
		}

		
		virtual public void simulate(float elapsedTime)
			{wrapper_Scene_simulate(nxScenePtr,elapsedTime);}

		virtual public void flushStream()
			{wrapper_Scene_flushStream(nxScenePtr);}
			
		virtual public bool fetchResults(NxSimulationStatus simulationStatus,bool block)
			{return wrapper_Scene_fetchResults(nxScenePtr,simulationStatus,block);}

		virtual public bool checkResults(NxSimulationStatus simulationStatus,bool block)
			{return wrapper_Scene_checkResults(nxScenePtr,simulationStatus,block);}

		virtual public void setGravity(Vector3 gravity)
			{wrapper_Scene_setGravity(nxScenePtr,ref gravity);}
		
		virtual public Vector3 getGravity()
		{
			Vector3 gravity=new Vector3(0,0,0);
			wrapper_Scene_getGravity(nxScenePtr,ref gravity);
			return gravity;
		}

		virtual	public NxSceneStats getStats()
		{
			NxSceneStats stats=new NxSceneStats();
			wrapper_Scene_getStats(nxScenePtr,stats);
			return stats;
		}

		//This returns the number of all joints including broken joints which become null
		virtual public int getNbJoints()
			{return wrapper_Scene_getNbJoints(nxScenePtr);}

		virtual public void resetJointIterator()
			{wrapper_Scene_resetJointIterator(nxScenePtr);}
			
		virtual public NxJoint getNextJoint()
			{return NxJoint.createFromPointer(wrapper_Scene_getNextJoint(nxScenePtr));}

		virtual public NxJoint[] getJoints()
		{
			int numJoints=getNbJoints();
			NxJoint[] jointArray=new NxJoint[numJoints];

			resetJointIterator();
			for(int i=0;i<numJoints;i++)
				{jointArray[i]=getNextJoint();}
			return jointArray;
		}

		//This is really slow you should only use this if you want a specific joint. Don't use it to iterate through all the joints
		virtual public NxJoint getJoint(int jointIndex)
		{
			int numJoints=getNbJoints();
			if(jointIndex<0 || jointIndex>=numJoints)
				{return null;}

			resetJointIterator();
			for(int i=0;i<numJoints;i++)
			{
				NxJoint joint=getNextJoint();				
				if(i==jointIndex)
					{return joint;}
			}
			return null;
		}


		virtual public void releaseJoint(NxJoint joint)
		{
			joint.internalBeforeRelease();
			wrapper_Scene_releaseJoint(nxScenePtr,joint.NxJointPtr);
			joint.internalAfterRelease();
		}

		virtual public NxJoint createJoint(NxJointDesc jointDesc)
		{
			NxJoint joint=null;

			if(jointDesc is NxSphericalJointDesc)
			{
				NxSphericalJointDesc j=(NxSphericalJointDesc)jointDesc;
				joint=NxSphericalJoint.createFromPointer(wrapper_Scene_createSphericalJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags,ref j.swingAxis,j.projectionDistance,j.twistLimit,ref j.swingLimit,j.twistSpring,j.swingSpring,j.jointSpring,j.flags,j.projectionMode));
			}
			else if(jointDesc is NxRevoluteJointDesc)
			{
				NxRevoluteJointDesc j=(NxRevoluteJointDesc)jointDesc;
				joint=NxRevoluteJoint.createFromPointer(wrapper_Scene_createRevoluteJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags,j.limit,j.motor,j.spring,j.projectionDistance,j.projectionAngle,j.flags,j.projectionMode));
			}			
			else if(jointDesc is NxCylindricalJointDesc)
			{
				NxCylindricalJointDesc j=(NxCylindricalJointDesc)jointDesc;
				joint=NxCylindricalJoint.createFromPointer(wrapper_Scene_createCylindricalJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags));
			}			
			else if(jointDesc is NxPrismaticJointDesc)
			{
				NxPrismaticJointDesc j=(NxPrismaticJointDesc)jointDesc;
				joint=NxPrismaticJoint.createFromPointer(wrapper_Scene_createPrismaticJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags));
			}
			else if(jointDesc is NxFixedJointDesc)
			{
				NxFixedJointDesc j=(NxFixedJointDesc)jointDesc;
				joint=NxFixedJoint.createFromPointer(wrapper_Scene_createFixedJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags));
			}
			else if(jointDesc is NxDistanceJointDesc)
			{
				NxDistanceJointDesc j=(NxDistanceJointDesc)jointDesc;
				joint=NxDistanceJoint.createFromPointer(wrapper_Scene_createDistanceJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags,j.maxDistance,j.minDistance,j.spring,j.flags));
			}
			else if(jointDesc is NxPointInPlaneJointDesc)
			{
				NxPointInPlaneJointDesc j=(NxPointInPlaneJointDesc)jointDesc;
				joint=NxPointInPlaneJoint.createFromPointer(wrapper_Scene_createPointInPlaneJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags));
			}
			else if(jointDesc is NxPointOnLineJointDesc)
			{
				NxPointOnLineJointDesc j=(NxPointOnLineJointDesc)jointDesc;
				joint=NxPointOnLineJoint.createFromPointer(wrapper_Scene_createPointOnLineJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags));
			}
			else if(jointDesc is NxPulleyJointDesc)
			{
				NxPulleyJointDesc j=(NxPulleyJointDesc)jointDesc;
				joint=NxPulleyJoint.createFromPointer(wrapper_Scene_createPulleyJoint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags,ref j.pulley_0,ref j.pulley_1,j.distance,j.stiffness,j.ratio,j.flags,j.motor));
			}
			else if(jointDesc is NxD6JointDesc)
			{
				NxD6JointDesc j=(NxD6JointDesc)jointDesc;
				joint=NxD6Joint.createFromPointer(wrapper_Scene_createD6Joint(nxScenePtr,j.actor[0].NxActorPtr,j.actor[1].NxActorPtr,ref j.localNormal[0],ref j.localNormal[1],ref j.localAxis[0],ref j.localAxis[1],ref j.localAnchor[0],ref j.localAnchor[1],j.maxForce,j.maxTorque,j.userData,j.internalNamePtr,j.jointFlags,j.xMotion,j.yMotion,j.zMotion,j.swing1Motion,j.swing2Motion,j.twistMotion,ref j.linearLimit,ref j.swing1Limit,ref j.swing2Limit,j.twistLimit,ref j.xDrive,ref j.yDrive,ref j.zDrive,ref j.swingDrive,ref j.twistDrive,ref j.slerpDrive,ref j.drivePosition,ref j.driveOrientation,ref j.driveLinearVelocity,ref j.driveAngularVelocity,j.projectionMode,j.projectionDistance,j.projectionAngle,j.gearRatio,j.flags));
			}
			else
				{return null;}
				
			if(joint!=null)
			{
				joint.UserData=jointDesc.userData;
				joint.Name=jointDesc.Name;
			}
			
			return joint;
		}

		virtual public void setTiming(float maxTimeStep,uint maxIter,NxTimeStepMethod timeStepMethod)
			{wrapper_Scene_setTiming(nxScenePtr,maxTimeStep,maxIter,timeStepMethod);}

		virtual public void getTiming(out float maxTimeStep,out uint maxIter,out NxTimeStepMethod timeStepMethod)
			{wrapper_Scene_getTiming(nxScenePtr,out maxTimeStep,out maxIter,out timeStepMethod);}

		virtual public bool isWritable()
			{return wrapper_Scene_isWritable(nxScenePtr);}
			
		virtual public void setActorPairFlags(NxActor actor0,NxActor actor1,uint pairFlags)
			{wrapper_Scene_setActorPairFlags(nxScenePtr,actor0.NxActorPtr,actor1.NxActorPtr,pairFlags);}

		virtual public uint getActorPairFlags(NxActor actor0,NxActor actor1)
			{return wrapper_Scene_getActorPairFlags(nxScenePtr,actor0.NxActorPtr,actor1.NxActorPtr);}

		virtual public void setShapePairFlags(NxShape shape0,NxShape shape1,uint pairFlags)
			{wrapper_Scene_setShapePairFlags(nxScenePtr,shape0.NxShapePtr,shape1.NxShapePtr,pairFlags);}

		virtual public uint setShapePairFlags(NxShape shape0,NxShape shape1)
			{return wrapper_Scene_getShapePairFlags(nxScenePtr,shape0.NxShapePtr,shape1.NxShapePtr);}

		virtual public NxSceneLimits getLimits()
		{
			NxSceneLimits sceneLimits=new NxSceneLimits();
			wrapper_Scene_getLimits(nxScenePtr,sceneLimits);
			return sceneLimits;
		}

		virtual public void setUserContactReport(NxUserContactReport contactReport)
			{NxUserContactReportHelper.setContactReport(this,contactReport);}

		virtual public NxUserContactReport getUserContactReport()
			{return NxUserContactReportHelper.getContactReport(this);}

		virtual public void setUserNotify(NxUserNotify userNotify)
			{NxUserNotifyHelper.setUserNotify(this,userNotify);}

		virtual public NxUserNotify getUserNotify()
			{return NxUserNotifyHelper.getUserNotify(this);}

		virtual public void setUserTriggerReport(NxUserTriggerReport triggerReport)
			{NxUserTriggerReportHelper.setUserTriggerReport(this,triggerReport);}

		virtual public NxUserTriggerReport getUserTriggerReport()
			{return NxUserTriggerReportHelper.getUserTriggerReport(this);}

		virtual public int getHighestMaterialIndex()
			{return wrapper_Scene_getHighestMaterialIndex(nxScenePtr);}

		virtual public NxMaterial createMaterial(NxMaterialDesc materialDesc)
			{return NxMaterial.createFromPointer(wrapper_Scene_createMaterial(nxScenePtr,materialDesc));}

		virtual public void releaseMaterial(NxMaterial material)
			{wrapper_Scene_releaseMaterial(nxScenePtr,material.NxMaterialPtr);}

		virtual public NxMaterial getMaterialFromIndex(ushort materialIndex)
			{return NxMaterial.createFromPointer(wrapper_Scene_getMaterialFromIndex(nxScenePtr,materialIndex));}

		virtual public NxMaterial getDefaultMaterial()
			{return getMaterialFromIndex(0);}
			
		virtual public void setDefaultMaterial(NxMaterialDesc materialDesc)
			{getDefaultMaterial().loadFromDesc(materialDesc);}

		virtual public NxMaterial[] getMaterialArray()
		{
			uint numMaterials=(uint)getNbMaterials();
			uint iterator=0;
			NxMaterial[] materialArray=new NxMaterial[numMaterials];

			unsafe
			{
				IntPtr[] pointerArray=new IntPtr[numMaterials];
				fixed(void* buf=&pointerArray[0])
					{wrapper_Scene_getMaterialArray(nxScenePtr,new IntPtr(buf),numMaterials,ref iterator);}

				for(int i=0;i<numMaterials;i++)
					{materialArray[i]=new NxMaterial(pointerArray[i]);}
			}
			return materialArray;
		}
		
		virtual public bool raycastAnyBounds(NxRay worldRay,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_raycastAnyBounds(nxScenePtr,worldRay,shapesType,0xffffffff,float.MaxValue,ref groupsMask,true);
		}

		virtual public bool raycastAnyBounds(NxRay worldRay,NxShapesType shapesType,uint groups,float maxDist,NxGroupsMask groupsMask)
			{return wrapper_Scene_raycastAnyBounds(nxScenePtr,worldRay,shapesType,groups,maxDist,ref groupsMask,false);}

		virtual public bool raycastAnyShape(NxRay worldRay,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_raycastAnyShape(nxScenePtr,worldRay,shapesType,0xffffffff,float.MaxValue,ref groupsMask,true);
		}

		virtual public bool raycastAnyShape(NxRay worldRay,NxShapesType shapesType,uint groups,float maxDist,NxGroupsMask groupsMask)
			{return wrapper_Scene_raycastAnyShape(nxScenePtr,worldRay,shapesType,groups,maxDist,ref groupsMask,false);}

		virtual public bool raycastAllBounds(NxRay worldRay,NxUserRaycastReport raycastReport,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_raycastAllBounds(nxScenePtr,worldRay,raycastReport.NxUserRaycastReportPtr,shapesType,0xffffffff,float.MaxValue,0xffffffff,ref groupsMask,true);
		}

		virtual public bool raycastAllBounds(NxRay worldRay,NxUserRaycastReport raycastReport,NxShapesType shapesType,uint groups,float maxDist,uint hintFlags,NxGroupsMask groupsMask)
			{return wrapper_Scene_raycastAllBounds(nxScenePtr,worldRay,raycastReport.NxUserRaycastReportPtr,shapesType,groups,maxDist,hintFlags,ref groupsMask,false);}

		virtual public bool raycastAllShapes(NxRay worldRay,NxUserRaycastReport raycastReport,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_raycastAllShapes(nxScenePtr,worldRay,raycastReport.NxUserRaycastReportPtr,shapesType,0xffffffff,float.MaxValue,0xffffffff,ref groupsMask,true);
		}

		virtual public bool raycastAllShapes(NxRay worldRay,NxUserRaycastReport raycastReport,NxShapesType shapesType,uint groups,float maxDist,uint hintFlags,NxGroupsMask groupsMask)
			{return wrapper_Scene_raycastAllShapes(nxScenePtr,worldRay,raycastReport.NxUserRaycastReportPtr,shapesType,groups,maxDist,hintFlags,ref groupsMask,false);}

		virtual public NxShape raycastClosestBounds(NxRay worldRay,NxShapesType shapesType,out NxRaycastHit raycastHit)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return NxShape.createFromPointer(wrapper_Scene_raycastClosestBounds(nxScenePtr,worldRay,shapesType,out raycastHit,0xffffffff,float.MaxValue,0xffffffff,ref groupsMask,true));
		}

		virtual public NxShape raycastClosestBounds(NxRay worldRay,NxShapesType shapesType,out NxRaycastHit raycastHit,uint groups,float maxDist,uint hintFlags,NxGroupsMask groupsMask)
			{return NxShape.createFromPointer(wrapper_Scene_raycastClosestBounds(nxScenePtr,worldRay,shapesType,out raycastHit,groups,maxDist,hintFlags,ref groupsMask,false));}

		virtual public NxShape raycastClosestShape(NxRay worldRay,NxShapesType shapesType,out NxRaycastHit raycastHit)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return NxShape.createFromPointer(wrapper_Scene_raycastClosestShape(nxScenePtr,worldRay,shapesType,out raycastHit,0xffffffff,float.MaxValue,0xffffffff,ref groupsMask,true));
		}

		virtual public NxShape raycastClosestShape(NxRay worldRay,NxShapesType shapesType,out NxRaycastHit raycastHit,uint groups,float maxDist,uint hintFlags,NxGroupsMask groupsMask)
			{return NxShape.createFromPointer(wrapper_Scene_raycastClosestShape(nxScenePtr,worldRay,shapesType,out raycastHit,groups,maxDist,hintFlags,ref groupsMask,false));}



		virtual public bool checkOverlapSphere(NxSphere worldSphere,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_checkOverlapSphere(nxScenePtr,worldSphere,shapesType,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public bool checkOverlapSphere(NxSphere worldSphere,NxShapesType shapesType,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_checkOverlapSphere(nxScenePtr,worldSphere,shapesType,activeGroups,ref groupsMask,false);}



		virtual public bool checkOverlapAABB(NxBounds3 worldBounds,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_checkOverlapAABB(nxScenePtr,worldBounds,shapesType,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public bool checkOverlapAABB(NxBounds3 worldBounds,NxShapesType shapesType,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_checkOverlapAABB(nxScenePtr,worldBounds,shapesType,activeGroups,ref groupsMask,false);}



		virtual public bool checkOverlapOBB(NxBox worldBox,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_checkOverlapOBB(nxScenePtr,worldBox,shapesType,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public bool checkOverlapOBB(NxBox worldBox,NxShapesType shapesType,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_checkOverlapOBB(nxScenePtr,worldBox,shapesType,activeGroups,ref groupsMask,false);}



		virtual public bool checkOverlapCapsule(NxCapsule worldCapsule,NxShapesType shapesType)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_checkOverlapCapsule(nxScenePtr,worldCapsule,shapesType,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public bool checkOverlapCapsule(NxCapsule worldCapsule,NxShapesType shapesType,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_checkOverlapCapsule(nxScenePtr,worldCapsule,shapesType,activeGroups,ref groupsMask,false);}



		virtual public int getNbPairs()
			{return wrapper_Scene_getNbPairs(nxScenePtr);}

		virtual public NxPairFlag[] getPairFlagArray()
		{
			uint numPairs=(uint)getNbPairs();
			NxPairFlag[] pairFlagArray=new NxPairFlag[numPairs];

			unsafe
			{
				fixed(void* buf=&pairFlagArray[0])
					{wrapper_Scene_getPairFlagArray(nxScenePtr,new IntPtr(buf),numPairs);}
			}
			return pairFlagArray;
		}




		virtual public int overlapSphereShapesUsingEntityReport(NxSphere worldSphere,NxShapesType shapesType,NxUserEntityReport userEntityReport)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_overlapSphereShapes(nxScenePtr,worldSphere,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public int overlapSphereShapesUsingEntityReport(NxSphere worldSphere,NxShapesType shapesType,NxUserEntityReport userEntityReport,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_overlapSphereShapes(nxScenePtr,worldSphere,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,activeGroups,ref groupsMask,false);}
		
		virtual public int overlapSphereShapes(NxSphere worldSphere,NxShapesType shapesType,NxShape[] shapesArray)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return internalOverlapSphereShapes(worldSphere,shapesType,shapesArray,0xFFFFFFFF,groupsMask,true);
		}
		
		virtual public int overlapSphereShapes(NxSphere worldSphere,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask)
			{return internalOverlapSphereShapes(worldSphere,shapesType,shapesArray,activeGroups,groupsMask,true);}

		virtual protected int internalOverlapSphereShapes(NxSphere worldSphere,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask,bool ignoreGroupsMask)
		{
			unsafe
			{
				int numShapes=0;
				
				if(shapesArray.Length>overlapPointerArray.Length)
					{overlapPointerArray=new IntPtr[shapesArray.Length];}

				fixed(void* buf=&overlapPointerArray[0])
					{numShapes=wrapper_Scene_overlapSphereShapes(nxScenePtr,worldSphere,shapesType,(uint)shapesArray.Length,new IntPtr(buf),IntPtr.Zero,activeGroups,ref groupsMask,ignoreGroupsMask);}

				numShapes=Math.Min(numShapes,shapesArray.Length);

				for(int i=0;i<numShapes;i++)
					{shapesArray[i]=NxShape.createFromPointer(overlapPointerArray[i]);}
			
				return numShapes;
			}
		}




		virtual public int overlapAABBShapesUsingEntityReport(NxBounds3 worldBounds,NxShapesType shapesType,NxUserEntityReport userEntityReport)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_overlapAABBShapes(nxScenePtr,worldBounds,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public int overlapAABBShapesUsingEntityReport(NxBounds3 worldBounds,NxShapesType shapesType,NxUserEntityReport userEntityReport,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_overlapAABBShapes(nxScenePtr,worldBounds,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,activeGroups,ref groupsMask,false);}
		
		virtual public int overlapAABBShapes(NxBounds3 worldBounds,NxShapesType shapesType,NxShape[] shapesArray)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return internalOverlapAABBShapes(worldBounds,shapesType,shapesArray,0xFFFFFFFF,groupsMask,true);
		}
		
		virtual public int overlapAABBShapes(NxBounds3 worldBounds,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask)
			{return internalOverlapAABBShapes(worldBounds,shapesType,shapesArray,activeGroups,groupsMask,true);}

		virtual protected int internalOverlapAABBShapes(NxBounds3 worldBounds,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask,bool ignoreGroupsMask)
		{
			unsafe
			{
				int numShapes=0;
				
				if(shapesArray.Length>overlapPointerArray.Length)
					{overlapPointerArray=new IntPtr[shapesArray.Length];}

				fixed(void* buf=&overlapPointerArray[0])
					{numShapes=wrapper_Scene_overlapAABBShapes(nxScenePtr,worldBounds,shapesType,(uint)shapesArray.Length,new IntPtr(buf),IntPtr.Zero,activeGroups,ref groupsMask,ignoreGroupsMask);}

				numShapes=Math.Min(numShapes,shapesArray.Length);

				for(int i=0;i<numShapes;i++)
					{shapesArray[i]=NxShape.createFromPointer(overlapPointerArray[i]);}
			
				return numShapes;
			}
		}




		virtual public int overlapOBBShapesUsingEntityReport(NxBox worldBox,NxShapesType shapesType,NxUserEntityReport userEntityReport)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_overlapOBBShapes(nxScenePtr,worldBox,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public int overlapOBBShapesUsingEntityReport(NxBox worldBox,NxShapesType shapesType,NxUserEntityReport userEntityReport,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_overlapOBBShapes(nxScenePtr,worldBox,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,activeGroups,ref groupsMask,false);}
		
		virtual public int overlapOBBShapes(NxBox worldBox,NxShapesType shapesType,NxShape[] shapesArray)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return internalOverlapOBBShapes(worldBox,shapesType,shapesArray,0xFFFFFFFF,groupsMask,true);
		}
		
		virtual public int overlapOBBShapes(NxBox worldBox,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask)
			{return internalOverlapOBBShapes(worldBox,shapesType,shapesArray,activeGroups,groupsMask,true);}

		virtual protected int internalOverlapOBBShapes(NxBox worldBox,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask,bool ignoreGroupsMask)
		{
			unsafe
			{
				int numShapes=0;
				
				if(shapesArray.Length>overlapPointerArray.Length)
					{overlapPointerArray=new IntPtr[shapesArray.Length];}

				fixed(void* buf=&overlapPointerArray[0])
					{numShapes=wrapper_Scene_overlapOBBShapes(nxScenePtr,worldBox,shapesType,(uint)shapesArray.Length,new IntPtr(buf),IntPtr.Zero,activeGroups,ref groupsMask,ignoreGroupsMask);}

				numShapes=Math.Min(numShapes,shapesArray.Length);

				for(int i=0;i<numShapes;i++)
					{shapesArray[i]=NxShape.createFromPointer(overlapPointerArray[i]);}
			
				return numShapes;
			}
		}




		virtual public int overlapCapsulShapesUsingEntityReport(NxCapsule worldCapsule,NxShapesType shapesType,NxUserEntityReport userEntityReport)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return wrapper_Scene_overlapCapsuleShapes(nxScenePtr,worldCapsule,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,0xFFFFFFFF,ref groupsMask,true);
		}

		virtual public int overlapCapsuleShapesUsingEntityReport(NxCapsule worldCapsule,NxShapesType shapesType,NxUserEntityReport userEntityReport,uint activeGroups,NxGroupsMask groupsMask)
			{return wrapper_Scene_overlapCapsuleShapes(nxScenePtr,worldCapsule,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,activeGroups,ref groupsMask,false);}
		
		virtual public int overlapCapsuleShapes(NxCapsule worldCapsule,NxShapesType shapesType,NxShape[] shapesArray)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return internalOverlapCapsuleShapes(worldCapsule,shapesType,shapesArray,0xFFFFFFFF,groupsMask,true);
		}
		
		virtual public int overlapCapsuleShapes(NxCapsule worldCapsule,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask)
			{return internalOverlapCapsuleShapes(worldCapsule,shapesType,shapesArray,activeGroups,groupsMask,true);}

		virtual protected int internalOverlapCapsuleShapes(NxCapsule worldCapsule,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask,bool ignoreGroupsMask)
		{
			unsafe
			{
				int numShapes=0;
				
				if(shapesArray.Length>overlapPointerArray.Length)
					{overlapPointerArray=new IntPtr[shapesArray.Length];}

				fixed(void* buf=&overlapPointerArray[0])
					{numShapes=wrapper_Scene_overlapCapsuleShapes(nxScenePtr,worldCapsule,shapesType,(uint)shapesArray.Length,new IntPtr(buf),IntPtr.Zero,activeGroups,ref groupsMask,ignoreGroupsMask);}

				numShapes=Math.Min(numShapes,shapesArray.Length);

				for(int i=0;i<numShapes;i++)
					{shapesArray[i]=NxShape.createFromPointer(overlapPointerArray[i]);}
			
				return numShapes;
			}
		}




		virtual public int cullShapesUsingEntityReport(NxPlane[] planeArray,NxShapesType shapesType,NxUserEntityReport userEntityReport)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return internalCullShapes(planeArray,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,0xFFFFFFFF,groupsMask,true);
		}

		virtual public int cullShapesUsingEntityReport(NxPlane[] planeArray,NxShapesType shapesType,NxUserEntityReport userEntityReport,uint activeGroups,NxGroupsMask groupsMask)
			{return internalCullShapes(planeArray,shapesType,0,IntPtr.Zero,userEntityReport.NxUserEntityReportPtr,activeGroups,groupsMask,false);}

		virtual public int cullShapes(NxPlane[] planeArray,NxShapesType shapesType,NxShape[] shapesArray)
		{
			NxGroupsMask groupsMask=new NxGroupsMask(0,0,0,0);	//required because NxGroupsMask needed to be a struct and "null" can't be passed in where a ref is used
			return internalCullShapes_secondary(planeArray,shapesType,shapesArray,0xFFFFFFFF,groupsMask,true);
		}
		
		virtual public int cullShapes(NxPlane[] planeArray,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask)
			{return internalCullShapes_secondary(planeArray,shapesType,shapesArray,activeGroups,groupsMask,true);}

		virtual protected int internalCullShapes(NxPlane[] planeArray,NxShapesType shapeType,uint shapesBufferSize,IntPtr shapesBuffer,IntPtr userEntityReport,uint activeGroups,NxGroupsMask groupsMask,bool ignoreGroupsMask)
		{
			unsafe
			{
				int pI=0;
				int numPlanes=planeArray.Length;
				float[] planeBuffer=new float[numPlanes*4];

				for(int i=0;i<numPlanes;i++)
				{
					NxPlane plane=planeArray[i];
					planeBuffer[pI++]=plane.normal.X;
					planeBuffer[pI++]=plane.normal.Y;
					planeBuffer[pI++]=plane.normal.Z;
					planeBuffer[pI++]=plane.d;
				}
				
				fixed(void* pBuf=&planeBuffer[0])
					{return wrapper_Scene_cullShapes(nxScenePtr,numPlanes,new IntPtr(pBuf),shapeType,shapesBufferSize,shapesBuffer,userEntityReport,activeGroups,ref groupsMask,ignoreGroupsMask);}
			}
		}
		
		virtual protected int internalCullShapes_secondary(NxPlane[] planeArray,NxShapesType shapesType,NxShape[] shapesArray,uint activeGroups,NxGroupsMask groupsMask,bool ignoreGroupsMask)
		{
			unsafe
			{
				int numShapes=0;
				
				if(shapesArray.Length>cullPointerArray.Length)
					{cullPointerArray=new IntPtr[shapesArray.Length];}

				fixed(void* buf=&cullPointerArray[0])
					{numShapes=internalCullShapes(planeArray,shapesType,(uint)shapesArray.Length,new IntPtr(buf),IntPtr.Zero,activeGroups,groupsMask,ignoreGroupsMask);}

				numShapes=Math.Min(numShapes,shapesArray.Length);

				for(int i=0;i<numShapes;i++)
					{shapesArray[i]=NxShape.createFromPointer(cullPointerArray[i]);}
			
				return numShapes;
			}
		}




		virtual public void releaseEffector(NxEffector effector)
		{
			wrapper_Scene_releaseEffector(nxScenePtr,effector.NxEffectorPtr);
			effector.internalAfterRelease();
		}

		virtual public int getNbEffectors()
			{return wrapper_Scene_getNbEffectors(nxScenePtr);}

		virtual public void resetEffectorIterator()
			{wrapper_Scene_resetEffectorIterator(nxScenePtr);}
			
		virtual public NxEffector getNextEffector()
			{return NxEffector.createFromPointer(wrapper_Scene_getNextEffector(nxScenePtr));}

		virtual public NxEffector[] getEffectors()
		{
			int numEffectors=getNbEffectors();
			NxEffector[] effectorArray=new NxEffector[numEffectors];

			resetEffectorIterator();
			for(int i=0;i<numEffectors;i++)
				{effectorArray[i]=getNextEffector();}
			return effectorArray;
		}

		virtual public NxSpringAndDamperEffector createSpringAndDamperEffector(NxSpringAndDamperEffectorDesc effectorDesc)
			{return NxSpringAndDamperEffector.createFromPointer(wrapper_Scene_createSpringAndDamperEffector(nxScenePtr,effectorDesc));}

		virtual public NxFluid createFluid(NxFluidDesc fluidDesc)
		{
			IntPtr[] emittersPtrs=fluidDesc.getEmittersPtrs();
			IntPtr fluidPtr=wrapper_Scene_createFluidUsingEmitterDescArray(nxScenePtr,fluidDesc.getAddress(),emittersPtrs.Length,emittersPtrs);
			NxFluid fluid=NxFluid.createFromPointer(fluidPtr);
			if(fluid!=null && fluidDesc.name!=null)
				{fluid.setName(fluidDesc.name);}
			return fluid;
		}
	
		virtual public void releaseFluid(NxFluid fluid)
		{
			fluid.internalBeforeRelease();
			wrapper_Scene_releaseFluid(nxScenePtr,fluid.NxFluidPtr);
			fluid.internalAfterRelease();
		}

		virtual public int getNbFluids()
			{return wrapper_Scene_getNbFluids(nxScenePtr);}

		virtual public NxFluid[] getFluids()
		{
			int numFluids=getNbFluids();
			NxFluid[] fluidArray=new NxFluid[numFluids];

			IntPtr fluidsPointer=wrapper_Scene_getFluids(nxScenePtr);
			unsafe
			{
				int* p=(int*)fluidsPointer.ToPointer();
				for(int i=0;i<numFluids;i++)
					{fluidArray[i]=NxFluid.createFromPointer(new IntPtr(p[i]));}
			}
			
			return fluidArray;
		}

		virtual	public bool createFluidHardwareTriangleMesh(NxStream stream)
			{return wrapper_Scene_createFluidHardwareTriangleMesh(nxScenePtr,stream.NxStreamPtr);}

		virtual public NxCloth createCloth(NxClothDesc clothDesc)
		{
			IntPtr clothPtr=wrapper_Scene_createCloth(nxScenePtr,clothDesc);
			NxCloth cloth=NxCloth.createFromPointer(clothPtr);
			if(cloth!=null && clothDesc.name!=null)
				{cloth.setName(clothDesc.name);}
			return cloth;
		}
	
		virtual public void releaseCloth(NxCloth cloth)
		{
			cloth.internalBeforeRelease();
			wrapper_Scene_releaseCloth(nxScenePtr,cloth.NxClothPtr);
			cloth.internalAfterRelease();
		}

		virtual public int getNbCloths()
			{return wrapper_Scene_getNbCloths(nxScenePtr);}

		virtual public NxCloth[] getCloths()
		{
			int numCloths=getNbCloths();
			NxCloth[] clothArray=new NxCloth[numCloths];

			IntPtr clothsPointer=wrapper_Scene_getCloths(nxScenePtr);
			unsafe
			{
				int* p=(int*)clothsPointer.ToPointer();
				for(int i=0;i<numCloths;i++)
					{clothArray[i]=NxCloth.createFromPointer(new IntPtr(p[i]));}
			}
			
			return clothArray;
		}

		virtual public void setFilterOps(NxFilterOp op0,NxFilterOp op1,NxFilterOp op2)
			{wrapper_Scene_setFilterOps(nxScenePtr,op0,op1,op2);}

		virtual public void setFilterBool(bool flag)
			{wrapper_Scene_setFilterBool(nxScenePtr,flag);}

		virtual public void setFilterConstant0(NxGroupsMask mask)
			{wrapper_Scene_setFilterConstant0(nxScenePtr,ref mask);}

		virtual public void setFilterConstant1(NxGroupsMask mask)
			{wrapper_Scene_setFilterConstant1(nxScenePtr,ref mask);}

		virtual public NxDebugRenderable getDebugRenderable()
			{return NxDebugRenderable.createFromPointer(wrapper_Scene_getDebugRenderable(nxScenePtr));}

		virtual public void setGroupCollisionFlag(ushort group1,ushort group2,bool enable)
			{wrapper_Scene_setGroupCollisionFlag(nxScenePtr,group1,group2,enable);}

		virtual public bool getGroupCollisionFlag(ushort group1,ushort group2)
			{return wrapper_Scene_getGroupCollisionFlag(nxScenePtr,group1,group2);}

		virtual public void setActorGroupPairFlags(ushort group1,ushort group2,uint flags)
			{wrapper_Scene_setActorGroupPairFlags(nxScenePtr,group1,group2,flags);}

		virtual public uint getActorGroupPairFlags(ushort group1,ushort group2)
			{return wrapper_Scene_getActorGroupPairFlags(nxScenePtr,group1,group2);}

		virtual public NxSimulationType getSimType()
			{return wrapper_Scene_getSimType(nxScenePtr);}

		virtual public NxHwSceneType getHwSceneType()
			{return wrapper_Scene_getHwSceneType(nxScenePtr);}

		virtual public void flushCaches()
			{wrapper_Scene_flushCaches(nxScenePtr);}

		virtual public NxThreadPollResult pollForWork(NxThreadWait waitType)
			{return wrapper_Scene_pollForWork(nxScenePtr,waitType);}

		virtual public void resetPollForWork()
			{wrapper_Scene_resetPollForWork(nxScenePtr);}

		virtual public NxThreadPollResult pollForBackgroundWork(NxThreadWait waitType)
			{return wrapper_Scene_pollForBackgroundWork(nxScenePtr,waitType);}

		virtual public void shutdownWorkerThreads()
			{wrapper_Scene_shutdownWorkerThreads(nxScenePtr);}

		virtual public void lockQueries()
			{wrapper_Scene_lockQueries(nxScenePtr);}

		virtual public void unlockQueries()
			{wrapper_Scene_unlockQueries(nxScenePtr);}

		virtual public NxPhysicsSDK getPhysicsSDK()
			{return NxPhysicsSDK.StaticSDK;}

		virtual	public bool saveToDesc(NxSceneDesc d)
		{
			IntPtr maxBoundsPtr=IntPtr.Zero;
			IntPtr limitsPtr=IntPtr.Zero;
			IntPtr customSchedulerPtr=IntPtr.Zero;

			bool result=wrapper_Scene_saveToDesc(nxScenePtr,ref d.gravity,ref d.maxTimestep,ref d.maxIter,ref d.timeStepMethod,ref maxBoundsPtr,ref limitsPtr,ref d.simType,ref d.hwSceneType,ref d.pipelineSpec,ref d.groundPlane,ref d.boundsPlanes,ref d.flags,ref d.internalThreadCount,ref d.backgroundThreadCount,ref d.threadMask,ref d.backgroundThreadMask,ref d.userData,ref customSchedulerPtr);

			d.userNotify=getUserNotify();
			d.userTriggerReport=getUserTriggerReport();
			d.userContactReport=getUserContactReport();

			if(maxBoundsPtr!=IntPtr.Zero)
			{
				d.maxBounds=new NxBounds3();
				Marshal.PtrToStructure(maxBoundsPtr,d.maxBounds);
			}
			if(limitsPtr!=IntPtr.Zero)
			{
				d.limits=new NxSceneLimits();
				Marshal.PtrToStructure(limitsPtr,d.limits);
			}
			if(customSchedulerPtr!=IntPtr.Zero)
				{d.customScheduler=NxUserScheduler.internalGetUserSchedulerByPointer(customSchedulerPtr);}

			return result;
		}

		virtual public NxSceneDesc getSceneDesc()
		{
			NxSceneDesc sceneDesc=NxSceneDesc.Default;
			saveToDesc(sceneDesc);
			return sceneDesc;
		}





		public IntPtr NxScenePtr
			{get{return nxScenePtr;}}

		public float MaxTimeStep
		{
			set
			{
				float maxTimeStep; uint maxIter; NxTimeStepMethod timeStepMethod;
				getTiming(out maxTimeStep,out maxIter,out timeStepMethod);
				setTiming(value,maxIter,timeStepMethod);
			}
			get
			{
				float maxTimeStep; uint maxIter; NxTimeStepMethod timeStepMethod;
				getTiming(out maxTimeStep,out maxIter,out timeStepMethod);
				return maxTimeStep;
			}
		}
		
		public uint MaxIterations
		{
			set
			{
				float maxTimeStep; uint maxIter;	NxTimeStepMethod timeStepMethod;
				getTiming(out maxTimeStep,out maxIter,out timeStepMethod);
				setTiming(maxTimeStep,value,timeStepMethod);
			}
			get
			{
				float maxTimeStep; uint maxIter; NxTimeStepMethod timeStepMethod;
				getTiming(out maxTimeStep,out maxIter,out timeStepMethod);
				return maxIter;
			}
		}
		
		public NxTimeStepMethod TimeStepMethod
		{
			set
			{
				float maxTimeStep; uint maxIter; NxTimeStepMethod timeStepMethod;
				getTiming(out maxTimeStep,out maxIter,out timeStepMethod);
				setTiming(maxTimeStep,maxIter,value);
			}
			get
			{
				float maxTimeStep; uint maxIter; NxTimeStepMethod timeStepMethod;
				getTiming(out maxTimeStep,out maxIter,out timeStepMethod);
				return timeStepMethod;
			}
		}

		public Vector3 Gravity
		{
			set{setGravity(value);}
			get{return getGravity();}
		}

		virtual public NxProfileData readProfileData(bool clearData)
		{
			IntPtr profileDataPtr=wrapper_Scene_readProfileData(nxScenePtr,clearData);
			return NxProfileData.createFromPointer(profileDataPtr);
		}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createActorByParameters(IntPtr scene,ref NxMat34 globalPose,IntPtr bodyDescPtr,float density,uint flags,ushort group,IntPtr userData,IntPtr name,int numShapeDescs,IntPtr[] shapeDescPtrs);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_releaseActor(IntPtr scene,IntPtr actor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbActors(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getActors(IntPtr scene);
	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_simulate(IntPtr scene,float elapsedTime);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_flushStream(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_fetchResults(IntPtr scene,NxSimulationStatus simulationStatus,bool block);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_checkResults(IntPtr scene,NxSimulationStatus simulationStatus,bool block);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setGravity(IntPtr scene,ref Vector3 gravity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_getGravity(IntPtr scene,ref Vector3 gravity);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_getStats(IntPtr scene,NxSceneStats stats);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbJoints(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_resetJointIterator(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getNextJoint(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_releaseJoint(IntPtr scene,IntPtr joint);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createSphericalJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,ref Vector3 swingAxis,float projectionDistance,NxJointLimitPairDesc twistLimit,ref NxJointLimitDesc swingLimit,NxSpringDesc twistSpring,NxSpringDesc swingSpring,NxSpringDesc jointSpring,uint flags,NxJointProjectionMode projectionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createRevoluteJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,NxJointLimitPairDesc limit,NxMotorDesc motor,NxSpringDesc spring,float projectionDistance,float projectionAngle,uint flags,NxJointProjectionMode projectionMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createCylindricalJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createPrismaticJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createFixedJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createDistanceJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,float maxDistance,float minDistance,NxSpringDesc spring,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createPointInPlaneJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createPointOnLineJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createPulleyJoint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,ref Vector3 pulley_0,ref Vector3 pulley_1,float distance,float stiffness,float ratio,uint flags,NxMotorDesc motor);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createD6Joint(IntPtr scene,IntPtr actor_0,IntPtr actor_1,ref Vector3 localNormal_0,ref Vector3 localNormal_1,ref Vector3 localAxis_0,ref Vector3 localAxis_1,ref Vector3 localAnchor_0,ref Vector3 localAnchor_1,float maxForce,float maxTorque,IntPtr userData,IntPtr name,uint jointFlags,NxD6JointMotion xMotion,NxD6JointMotion yMotion,NxD6JointMotion zMotion,NxD6JointMotion swing1Motion,NxD6JointMotion swing2Motion,NxD6JointMotion twistMotion,ref NxJointLimitSoftDesc linearLimit,ref NxJointLimitSoftDesc swing1Limit,ref NxJointLimitSoftDesc swing2Limit,NxJointLimitSoftPairDesc twistLimit,ref NxJointDriveDesc xDrive,ref NxJointDriveDesc yDrive,ref NxJointDriveDesc zDrive,ref NxJointDriveDesc swingDrive,ref NxJointDriveDesc twistDrive,ref NxJointDriveDesc slerpDrive,ref Vector3 drivePosition,ref NxQuat driveOrientation,ref Vector3 driveLinearVelocity,ref Vector3 driveAngularVelocity,NxJointProjectionMode projectionMode,float projectionDistance,float projectionAngle,float gearRatio,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setTiming(IntPtr scene,float maxTimeStep,uint maxIter,NxTimeStepMethod timeStepMethod);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_getTiming(IntPtr scene,out float maxTimeStep,out uint maxIter,out NxTimeStepMethod timeStepMethod);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_isWritable(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setActorPairFlags(IntPtr scene,IntPtr actor0,IntPtr actor1,uint pairFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Scene_getActorPairFlags(IntPtr scene,IntPtr actor0,IntPtr actor1);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setShapePairFlags(IntPtr scene,IntPtr shape0,IntPtr shape1,uint pairFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Scene_getShapePairFlags(IntPtr scene,IntPtr shape0,IntPtr shape1);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_getLimits(IntPtr scene,NxSceneLimits sceneLimits);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbStaticShapes(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbDynamicShapes(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getTotalNbShapes(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbMaterials(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getHighestMaterialIndex(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createMaterial(IntPtr scene,NxMaterialDesc materialDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_releaseMaterial(IntPtr scene,IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getMaterialFromIndex(IntPtr scene,ushort materialIndex);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Scene_getMaterialArray(IntPtr scene,IntPtr userBuffer,uint bufferSize,ref uint usersIterator);
	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_raycastAnyBounds(IntPtr scene,NxRay worldRay,NxShapesType shapesType,uint groups,float maxDist,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_raycastAnyShape(IntPtr scene,NxRay worldRay,NxShapesType shapesType,uint groups,float maxDist,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_raycastAllBounds(IntPtr scene,NxRay worldRay,IntPtr raycastReport,NxShapesType shapesType,uint groups,float maxDist,uint hintFlags,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_raycastAllShapes(IntPtr scene,NxRay worldRay,IntPtr raycastReport,NxShapesType shapesType,uint groups,float maxDist,uint hintFlags,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_raycastClosestBounds(IntPtr scene,NxRay worldRay,NxShapesType shapesType,out NxRaycastHit raycastHit,uint groups,float maxDist,uint hintFlags,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_raycastClosestShape(IntPtr scene,NxRay worldRay,NxShapesType shapesType,out NxRaycastHit raycastHit,uint groups,float maxDist,uint hintFlags,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setUserData(IntPtr scene,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getUserData(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbPairs(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Scene_getPairFlagArray(IntPtr scene,IntPtr userArray,uint numPairs);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_checkOverlapSphere(IntPtr scene,NxSphere worldSphere,NxShapesType shapesType,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_checkOverlapAABB(IntPtr scene,NxBounds3 worldBounds,NxShapesType shapesType,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_checkOverlapOBB(IntPtr scene,NxBox worldBox,NxShapesType shapesType,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_checkOverlapCapsule(IntPtr scene,NxCapsule worldCapsule,NxShapesType shapesType,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_overlapSphereShapes(IntPtr scene,NxSphere worldSphere,NxShapesType shapesType,uint shapesBufferSize,IntPtr shapesBuffer,IntPtr userEntityReport,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_overlapAABBShapes(IntPtr scene,NxBounds3 worldBounds,NxShapesType shapesType,uint shapesBufferSize,IntPtr shapesBuffer,IntPtr userEntityReport,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_overlapOBBShapes(IntPtr scene,NxBox worldBox,NxShapesType shapesType,uint shapesBufferSize,IntPtr shapesBuffer,IntPtr userEntityReport,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_overlapCapsuleShapes(IntPtr scene,NxCapsule worldCapsule,NxShapesType shapesType,uint shapesBufferSize,IntPtr shapesBuffer,IntPtr userEntityReport,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_cullShapes(IntPtr scene,int numPlanes,IntPtr worldPlanes,NxShapesType shapeType,uint shapesBufferSize,IntPtr shapesBuffer,IntPtr userEntityReport,uint activeGroups,ref NxGroupsMask groupsMask,bool ignoreGroupsMask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_releaseEffector(IntPtr scene,IntPtr effectorPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbEffectors(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_resetEffectorIterator(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getNextEffector(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createSpringAndDamperEffector(IntPtr scene,NxSpringAndDamperEffectorDesc effectorDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createFluidUsingEmitterDescArray(IntPtr scene,IntPtr fluidDescPtr,int numEmitters,IntPtr[] emitterDescPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_createFluidHardwareTriangleMesh(IntPtr scene,IntPtr streamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_releaseFluid(IntPtr scene,IntPtr fluidPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbFluids(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getFluids(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_createCloth(IntPtr scene,NxClothDesc clothDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_releaseCloth(IntPtr scene,IntPtr clothPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Scene_getNbCloths(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getCloths(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setFilterOps(IntPtr scene,NxFilterOp op0,NxFilterOp op1,NxFilterOp op2);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setFilterBool(IntPtr scene,bool flag);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setFilterConstant0(IntPtr scene,ref NxGroupsMask mask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setFilterConstant1(IntPtr scene,ref NxGroupsMask mask);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setGroupCollisionFlag(IntPtr scene,ushort group1,ushort group2,bool enable);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_getGroupCollisionFlag(IntPtr scene,ushort group1,ushort group2);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_setActorGroupPairFlags(IntPtr scene,ushort group1,ushort group2,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Scene_getActorGroupPairFlags(IntPtr scene,ushort group1,ushort group2);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_getDebugRenderable(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxSimulationType wrapper_Scene_getSimType(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxHwSceneType wrapper_Scene_getHwSceneType(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_flushCaches(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxThreadPollResult wrapper_Scene_pollForWork(IntPtr scene,NxThreadWait waitType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_resetPollForWork(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxThreadPollResult wrapper_Scene_pollForBackgroundWork(IntPtr scene,NxThreadWait waitType);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_shutdownWorkerThreads(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_lockQueries(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Scene_unlockQueries(IntPtr scene);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Scene_saveToDesc(IntPtr scene,ref Vector3 gravity,ref float maxTimestep,ref uint maxIter,ref NxTimeStepMethod timeStepMethod,ref IntPtr maxBounds,ref IntPtr limits,ref NxSimulationType simType,ref NxHwSceneType hwSceneType,ref NxHwPipelineSpec pipelineSpec,ref bool groundPlane,ref bool boundsPlanes,ref uint flags,ref uint internalThreadCount,ref uint backgroundThreadCount,ref uint threadMask,ref uint backgroundThreadMask,ref IntPtr userData,ref IntPtr customSchedulerPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Scene_readProfileData(IntPtr scene,bool clearData);
	}
}





/*
X	virtual void						setGravity(const NxVec3& vec) = 0;
X	virtual void						getGravity(NxVec3& vec) = 0;
X	virtual NxActor*					createActor(const NxActorDescBase& desc) = 0;
X	virtual void						releaseActor(NxActor& actor) = 0;
X	virtual NxJoint *					createJoint(const NxJointDesc &jointDesc) = 0;
X	virtual void						releaseJoint(NxJoint &joint) = 0;
X	virtual NxSpringAndDamperEffector*	createSpringAndDamperEffector(const NxSpringAndDamperEffectorDesc& springDesc) = 0;
-	virtual void						releaseEffector(NxEffector& effector) = 0;
-	virtual NxU32						getNbEffectors()		const	= 0;
-	virtual void						resetEffectorIterator()	 = 0;
-	virtual NxEffector *				getNextEffector() = 0;
-	virtual NxMaterial *				createMaterial(const NxMaterialDesc &matDesc) = 0;
-	virtual void						releaseMaterial(NxMaterial &material) = 0;
X	virtual void						setActorPairFlags(NxActor& actorA, NxActor& actorB, NxU32 nxContactPairFlag) = 0;
-	virtual NxU32						getActorPairFlags(NxActor& actorA, NxActor& actorB) const = 0;
-	virtual	void						setShapePairFlags(NxShape& shapeA, NxShape& shapeB, NxU32 nxContactPairFlag) = 0;
-	virtual	NxU32						getShapePairFlags(NxShape& shapeA, NxShape& shapeB) const = 0;
X	virtual NxU32						getNbPairs() const = 0;
X	virtual NxU32						getPairFlagArray(NxPairFlag* userArray, NxU32 numPairs) const = 0;
-	virtual void						setGroupCollisionFlag(NxCollisionGroup group1, NxCollisionGroup group2, bool enable) = 0;
-	virtual bool						getGroupCollisionFlag(NxCollisionGroup group1, NxCollisionGroup group2) const = 0;
-	virtual void						setActorGroupPairFlags(NxActorGroup group1, NxActorGroup group2, NxU32 flags) = 0;
-	virtual NxU32						getActorGroupPairFlags(NxActorGroup group1, NxActorGroup group2) const = 0;
-	virtual	void						setFilterOps(NxFilterOp op0, NxFilterOp op1, NxFilterOp op2)	= 0;
-	virtual	void						setFilterBool(bool flag)										= 0;
-	virtual	void						setFilterConstant0(const NxGroupsMask& mask)					= 0;
-	virtual	void						setFilterConstant1(const NxGroupsMask& mask)					= 0;
X	virtual	NxU32						getNbActors()		const	= 0;
X	virtual	NxActor**					getActors()					= 0;
X	virtual	NxU32						getNbStaticShapes()		const	= 0;
X	virtual	NxU32						getNbDynamicShapes()	const	= 0;
-	virtual	NxU32						getTotalNbShapes()	const	= 0;
X	virtual NxU32						getNbJoints()		const	= 0;
X	virtual void						resetJointIterator()	 = 0;
X	virtual NxJoint *					getNextJoint()	 = 0;
X	virtual NxU32						getNbMaterials() const = 0;
X	virtual	NxU32						getMaterialArray(NxMaterial ** userBuffer, NxU32 bufferSize, NxU32 & usersIterator) = 0;
-	virtual NxMaterialIndex				getHighestMaterialIndex() const = 0;
-	virtual	NxMaterial *				getMaterialFromIndex(NxMaterialIndex matIndex) = 0;
X	virtual void						flushStream() = 0;
X	virtual void						setTiming(NxReal maxTimestep=1.0f/60.0f, NxU32 maxIter=8, NxTimeStepMethod method=NX_TIMESTEP_FIXED) = 0;
X	virtual void						getTiming(NxReal& maxTimestep, NxU32& maxIter, NxTimeStepMethod& method, NxU32* numSubSteps=NULL) const = 0;
-	virtual const NxDebugRenderable *	getDebugRenderable() = 0;
-	virtual	void						getStats(NxSceneStats& stats) const = 0;
X	virtual	void						getLimits(NxSceneLimits& limits) const = 0;
X	virtual void						setUserNotify(NxUserNotify* callback) = 0;
-	virtual NxUserNotify*				getUserNotify() const = 0;
X	virtual	void						setUserTriggerReport(NxUserTriggerReport* callback) = 0;
-	virtual	NxUserTriggerReport*		getUserTriggerReport() const = 0;
X	virtual	void						setUserContactReport(NxUserContactReport* callback) = 0;
-	virtual	NxUserContactReport*		getUserContactReport() const = 0;
X	virtual bool						raycastAnyBounds		(const NxRay& worldRay, NxShapesType shapesType, NxU32 groups=0xffffffff, NxReal maxDist=NX_MAX_F32, const NxGroupsMask* groupsMask=NULL) const = 0;
X	virtual bool						raycastAnyShape			(const NxRay& worldRay, NxShapesType shapesType, NxU32 groups=0xffffffff, NxReal maxDist=NX_MAX_F32, const NxGroupsMask* groupsMask=NULL) const = 0;
X	virtual NxU32						raycastAllBounds		(const NxRay& worldRay, NxUserRaycastReport& report, NxShapesType shapesType, NxU32 groups=0xffffffff, NxReal maxDist=NX_MAX_F32, NxU32 hintFlags=0xffffffff, const NxGroupsMask* groupsMask=NULL) const = 0;
X	virtual NxU32						raycastAllShapes		(const NxRay& worldRay, NxUserRaycastReport& report, NxShapesType shapesType, NxU32 groups=0xffffffff, NxReal maxDist=NX_MAX_F32, NxU32 hintFlags=0xffffffff, const NxGroupsMask* groupsMask=NULL) const = 0;
X	virtual NxShape*					raycastClosestBounds	(const NxRay& worldRay, NxShapesType shapeType, NxRaycastHit& hit, NxU32 groups=0xffffffff, NxReal maxDist=NX_MAX_F32, NxU32 hintFlags=0xffffffff, const NxGroupsMask* groupsMask=NULL) const = 0;
X	virtual NxShape*					raycastClosestShape		(const NxRay& worldRay, NxShapesType shapeType, NxRaycastHit& hit, NxU32 groups=0xffffffff, NxReal maxDist=NX_MAX_F32, NxU32 hintFlags=0xffffffff, const NxGroupsMask* groupsMask=NULL) const = 0;
X	virtual	NxU32						overlapSphereShapes		(const NxSphere& worldSphere, NxShapesType shapeType, NxU32 nbShapes, NxShape** shapes, NxUserEntityReport<NxShape*>* callback, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL, bool accurateCollision=false) = 0;
X	virtual	NxU32						overlapAABBShapes		(const NxBounds3& worldBounds, NxShapesType shapeType, NxU32 nbShapes, NxShape** shapes, NxUserEntityReport<NxShape*>* callback, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL, bool accurateCollision=false) = 0;
-	virtual	NxU32						overlapOBBShapes		(const NxBox& worldBox, NxShapesType shapeType, NxU32 nbShapes, NxShape** shapes, NxUserEntityReport<NxShape*>* callback, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL, bool accurateCollision=false) = 0;
-	virtual	NxU32						overlapCapsuleShapes		(const NxCapsule& worldCapsule, NxShapesType shapeType, NxU32 nbShapes, NxShape** shapes, NxUserEntityReport<NxShape*>* callback, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL, bool accurateCollision=false) = 0;
X	virtual	NxU32						cullShapes				(NxU32 nbPlanes, const NxPlane* worldPlanes, NxShapesType shapeType, NxU32 nbShapes, NxShape** shapes, NxUserEntityReport<NxShape*>* callback, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL) = 0;
-	virtual bool						checkOverlapSphere		(const NxSphere& worldSphere, NxShapesType shapeType=NX_ALL_SHAPES, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL)	= 0;
-	virtual bool						checkOverlapAABB		(const NxBounds3& worldBounds, NxShapesType shapeType=NX_ALL_SHAPES, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL)	= 0;
-	virtual bool						checkOverlapOBB			(const NxBox& worldBox, NxShapesType shapeType=NX_ALL_SHAPES, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL)	= 0;
-	virtual bool						checkOverlapCapsule		(const NxCapsule& worldCapsule, NxShapesType shapeType=NX_ALL_SHAPES, NxU32 activeGroups=0xffffffff, const NxGroupsMask* groupsMask=NULL)	= 0;
-	virtual NxFluid*					createFluid(const NxFluidDesc& fluidDesc) = 0;
-	virtual void						releaseFluid(NxFluid& fluid)			= 0;
-	virtual	NxU32						getNbFluids()		const		= 0;
-	virtual	NxFluid**					getFluids()						= 0;
-	virtual	bool						createFluidHardwareTriangleMesh(NxStream& stream)	= 0;
-	virtual	NxSimulationType			getSimType() const									= 0;
-	virtual	NxHwSceneType				getHwSceneType() const								= 0;
-	virtual NxCloth*					createCloth(const NxClothDesc& clothDesc) = 0;
-	virtual void						releaseCloth(NxCloth& cloth) = 0;
-	virtual	NxU32						getNbCloths() const = 0;
-	virtual	NxCloth**					getCloths() = 0;
X	virtual	bool						isWritable()	= 0;
X	virtual	void						simulate(NxReal elapsedTime)		= 0;
-	virtual	bool						checkResults(NxSimulationStatus status, bool block = false)	= 0;
X	virtual	bool						fetchResults(NxSimulationStatus status, bool block = false, NxU32 *errorState = 0)	= 0;
-	virtual	void						flushCaches()	= 0;
-	virtual NxThreadPollResult			pollForWork(NxThreadWait waitType)=0;
-	virtual void						resetPollForWork()=0;
-	virtual NxThreadPollResult			pollForBackgroundWork(NxThreadWait waitType)=0;
-	virtual void						shutdownWorkerThreads()=0;
-	virtual void						lockQueries()=0;
-	virtual void						unlockQueries()=0;
#	virtual void *						getInternal(void) = 0;
-	virtual	NxPhysicsSDK&				getPhysicsSDK() = 0;
X	virtual	bool						saveToDesc(NxSceneDesc& desc)	const	= 0;
-	virtual const NxProfileData *		readProfileData(bool clearData)	= 0;
*/
                  

#region Old Joints Code which had a separate getNumGoodJoints
/*
//Info:
//getNbJoints() returns all joints including broken joints which are null
//call getNumGoodJoints() to get the number of non-broken joints
// or just call Length on the array returned from getJoints()

		//This returns the number of good usable joints. Broken joints are excluded
		virtual public int getNumGoodJoints()
		{
			int numJoints=getNbJoints();
			int numGoodJoints=0;

			resetJointIterator();
			for(int i=0;i<numJoints;i++)
			{
				if(wrapper_Scene_getNextJoint(nxScenePtr)!=IntPtr.Zero)
					{numGoodJoints++;}
			}
			return numGoodJoints;
		}

		//This only returns good joints. Broken joints are excluded because they are null
		virtual public NxJoint[] getJoints()
		{
			int numJoints=getNbJoints();
			int numGoodJoints=getNumGoodJoints();
			int goodIndex=0;
			NxJoint[] jointArray=new NxJoint[numGoodJoints];

			resetJointIterator();
			for(int i=0;i<numJoints;i++)
			{
				//Broken joints are returned as null and are skipped
				NxJoint joint=getNextJoint();
				if(joint!=null)
					{jointArray[goodIndex++]=joint;}
			}
			return jointArray;
		}
*/
#endregion





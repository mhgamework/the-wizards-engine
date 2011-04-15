//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxJointDesc
	{
		public NxActor[] actor=new NxActor[2];			//!< The two actors connected by the joint. The actors must be in the same scene as this joint. At least one of the two pointers must be a dynamic actor. One of the two may be NULL to indicate the world frame.  Neither may be a static actor!
		public Vector3[] localNormal=new Vector3[2];	//!< X axis of joint space, in actor[i]'s space, orthogonal to localAxis[i]
		public Vector3[] localAxis=new Vector3[2];		//!< Z axis of joint space, in actor[i]'s space. This is the primary axis of the joint.
		public Vector3[] localAnchor=new Vector3[2];	//!< Attachment point of joint in actor[i]'s space
		public float maxForce;							//!< Maximum linear force that the joint can withstand before breaking, must be positive. Default: +inf. 
		public float maxTorque;							//!< Maximum angular force (torque) that the joint can withstand before breaking, must be positive. Default: +inf. 
		public IntPtr userData;							//!< Will be copied to NxJoint::userData.
		public string name;								//!< Possible debug name.  The string is not copied by the SDK, only the pointer is stored.
		public uint jointFlags;							//!< This is a combination of the bits defined by ::NxJointFlag .
		public IntPtr internalNamePtr=IntPtr.Zero;


		public NxJointDesc()
			{setToDefault();}
			
		virtual public void setToDefault()
		{
			for(int i=0;i<2;i++)
			{
				actor[i]		= null;
				localAxis[i]	= new Vector3(0,0,1);
				localNormal[i]	= new Vector3(1,0,0);
				localAnchor[i]	= new Vector3(0,0,0);
			}

			maxForce	= float.MaxValue;
			maxTorque	= float.MaxValue;
			userData	= IntPtr.Zero;
			name		= null;
			jointFlags	= (uint)NxJointFlag.NX_JF_VISUALIZATION;
		}

		public void setActors(NxActor actor_0,NxActor actor_1)
		{
			actor[0]=actor_0;
			actor[1]=actor_1;
		}	

		public void setGlobalAnchor(Vector3 worldSpaceAnchor)
		{
			setGlobalAnchor(0,worldSpaceAnchor);
			setGlobalAnchor(1,worldSpaceAnchor);
		}

		public void setGlobalAxis(Vector3 worldSpaceAxis)
		{
			setGlobalAxis(0,worldSpaceAxis);
			setGlobalAxis(1,worldSpaceAxis);
		}
		
		public void setGlobalAxis(int index,Vector3 worldSpaceAxis)
		{
			if(index<0||index>1)
				{throw new IndexOutOfRangeException("NxJointDesc.setGlobalAxis("+index+"): Only 0 and 1 are allowed.");}
			if(actor[index]==null)
				{throw new NullReferenceException("NxJointDesc.setGlobalAxis(): actor["+index+"] is null. Actor must be set before calling this.");}
			
			worldSpaceAxis.Normalize();
			Matrix matrix=actor[index].getGlobalPose();
			localAxis[index]=NovodexUtil.transformWorldNormalIntoLocalSpace(worldSpaceAxis,ref matrix);
			localNormal[index]=NovodexUtil.getPerpendicularVector(localAxis[index]);
		}
		
		public void setGlobalAnchor(int index,Vector3 worldSpaceAnchor)
		{
			if(index<0||index>1)
				{throw new IndexOutOfRangeException("NxJointDesc.setGlobalAnchor("+index+"): Only 0 and 1 are allowed.");}
			if(actor[index]==null)
				{throw new NullReferenceException("NxJointDesc.setGlobalAnchor(): actor["+index+"] is null. Actor must be set before calling this.");}
			
			Matrix matrix=actor[index].getGlobalPose();
			localAnchor[index]=NovodexUtil.transformWorldPointIntoLocalSpace(worldSpaceAnchor,ref matrix);
		}
		
		public Vector3 getGlobalAxis(int index)
		{
			if(index<0||index>1)
				{throw new IndexOutOfRangeException("NxJointDesc.getGlobalAxis("+index+"): Only 0 and 1 are allowed.");}
			if(actor[index]==null)
				{throw new NullReferenceException("NxJointDesc.getGlobalAxis(): actor["+index+"] is null. Actor must be set before calling this.");}
			return Vector3.TransformNormal(localAxis[index],actor[index].getGlobalPose());
		}
		
		public Vector3 getGlobalAnchor(int index)
		{
			if(index<0||index>1)
				{throw new IndexOutOfRangeException("NxJointDesc.getGlobalAnchor("+index+"): Only 0 and 1 are allowed.");}
			if(actor[index]==null)
				{throw new NullReferenceException("NxJointDesc.getGlobalAnchor(): actor["+index+"] is null. Actor must be set before calling this.");}
			//return Vector3.Transform(localAnchor[index],actor[index].getGlobalPose());
			return Vector3.Transform(localAnchor[index], actor[index].getGlobalPose());
			
			

		}
		

		//Change this??
		public String Name
		{
			get{return name;}
			set{name=value;}
		}
		
		
		public bool FlagCollisionEnabled
		{
			get{return NovodexUtil.areBitsSet(jointFlags,(uint)NxJointFlag.NX_JF_COLLISION_ENABLED);}
			set{jointFlags=NovodexUtil.setBits(jointFlags,(uint)NxJointFlag.NX_JF_COLLISION_ENABLED,value);}
		}

		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(jointFlags,(uint)NxJointFlag.NX_JF_VISUALIZATION);}
			set{jointFlags=NovodexUtil.setBits(jointFlags,(uint)NxJointFlag.NX_JF_VISUALIZATION,value);}
		}
	}
}


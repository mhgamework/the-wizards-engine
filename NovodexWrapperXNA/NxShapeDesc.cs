//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Explicit)]
	public class NxShapeDesc
	{
		[FieldOffset( 0)]private int				prefix;			//This is some unknown extra data that is needed
		[FieldOffset( 4)]public  NxShapeType		type;			//!< The type of the shape (see NxShape.h). This gets set by the derived class' ctor, the user should not have to change it.
		[FieldOffset( 8)]public  NxMat34			localPose;		//!< The pose of the shape in the coordinate frame of the owning actor.
		[FieldOffset(56)]public  uint				shapeFlags;		//!< A combination of ::NxShapeFlag values.
		[FieldOffset(60)]public  ushort				group;			//!< See the documentation for NxShape::setGroup().
		[FieldOffset(62)]public  ushort				materialIndex;	//!< The material index of the shape.  See NxPhysicsSDK::addMaterial().
		[FieldOffset(64)]public  IntPtr				ccdSkeletonPtr;
		[FieldOffset(68)]public  float				density;		//brief density of this individual shape when computing mass inertial properties for a rigidbody (unless a valid mass >0.0 is provided).
		[FieldOffset(72)]public  float				mass;			//brief mass of this individual shape when computing mass inertial properties for a rigidbody.  When mass<=0.0 then density and volume determine the mass.
		[FieldOffset(76)]public  float				skinWidth;		//!< The shape has been radially inflated by this much over the graphical representation. Two shapes will interpenetrate by the sum of their skin widths; this means that their graphical representations should just touch. The default skin width is the negative global parameter ::NX_MIN_SEPARATION_FOR_PENALTY.  This is used if skinWidth is -1.  A sum skin width of zero for two bodies is not permitted because it is unstable.  If you simulation jitters because resting bodies occasionally lose contact, increase the sizes of your collision volumes and the skin width. Units: distance.
		[FieldOffset(80)]public  IntPtr				userData;		//!< Will be copied to NxShape::userData.
		[FieldOffset(84)]public  IntPtr				internalNamePtr;
		[FieldOffset(88)]public  NxGroupsMask		groupsMask;		//!< Groups bitmask for collision filtering
		//The total size should be 104 bytes


		//Pass in Matrix.Zero,NxShapeFlags.USE_DEFAULT,and negative values to use default values
		public void set(Matrix localPose,NxShapeFlag shapeFlags,int group,int materialIndex,string name)
		{
			setToDefault();
			/////WARNING 'new Matrix()' was vroeger Matrix.Empty  en dit heb ik veranderd !!!!!!!!!!!!!!!!
			/////WARNING 'new Matrix()' was vroeger Matrix.Empty  en dit heb ik veranderd !!!!!!!!!!!!!!!!
			/////WARNING 'new Matrix()' was vroeger Matrix.Empty  en dit heb ik veranderd !!!!!!!!!!!!!!!!
			/////WARNING 'new Matrix()' was vroeger Matrix.Empty  en dit heb ik veranderd !!!!!!!!!!!!!!!!


			if(localPose!=new Matrix()){this.localPose=NovodexUtil.convertMatrixToNxMat34(localPose);}
			if(shapeFlags!=NxShapeFlag.USE_DEFAULT){this.shapeFlags=(uint)shapeFlags;}
			if(group>=0){this.group=(ushort)group;}
			if(materialIndex>=0){this.materialIndex=(ushort)materialIndex;}
		}

		public string Name
		{
			get
			{
				if(internalNamePtr==IntPtr.Zero)
					{return null;}
				return Marshal.PtrToStringAnsi(internalNamePtr);
			}
			//There's no place to put an extra string variable to keep the string from being
			// garbage collected, so there is no set accessor.
		}
		
		virtual public void setToDefault()
		{
			localPose		= NxMat34.Identity;
			shapeFlags		= (uint)NxShapeFlag.NX_SF_VISUALIZATION;
			group			= 0;
			materialIndex	= 0;
			ccdSkeletonPtr	= IntPtr.Zero;
			density			= 1;
			mass			= -1;
			skinWidth		= -1.0f;
			userData		= IntPtr.Zero;
			internalNamePtr	= IntPtr.Zero;
			groupsMask.bits0= 0;
			groupsMask.bits1= 0;
			groupsMask.bits2= 0;
			groupsMask.bits3= 0;
		}

	
		unsafe public IntPtr getAddress()
		{
			fixed(void* x=&prefix)
				{return new IntPtr(x);}
		}


		public NxCCDSkeleton CCDSkeleton
		{
			get{return NxCCDSkeleton.createFromPointer(ccdSkeletonPtr);}
			set{ccdSkeletonPtr=value.NxCCDSkeletonPtr;}
		}
		


		public bool FlagTriggerOnEnter
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ON_ENTER);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ON_ENTER,value);}
		}

		public bool FlagTriggerOnLeave
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ON_LEAVE);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ON_LEAVE,value);}
		}
		
		public bool FlagTriggerOnStay
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ON_STAY);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ON_STAY,value);}
		}

		public bool FlagTriggerEnable
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ENABLE);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_TRIGGER_ENABLE,value);}
		}

		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_VISUALIZATION);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_VISUALIZATION,value);}
		}

		public bool FlagDisableCollision
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DISABLE_COLLISION);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DISABLE_COLLISION,value);}
		}

		public bool FlagFeatureIndices
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_FEATURE_INDICES);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_DISABLE_RESPONSE,value);}
		}

		public bool FlagDisableRaycasting
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_DISABLE_RAYCASTING);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_DISABLE_RAYCASTING,value);}
		}

		public bool FlagPointContactForce
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_POINT_CONTACT_FORCE);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_POINT_CONTACT_FORCE,value);}
		}
		
		public bool FlagFluidDrain
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DRAIN);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DRAIN,value);}
		}

		public bool FlagFluidDrainInvert
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DRAIN_INVERT);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DRAIN_INVERT,value);}
		}

		public bool FlagFluidDisableCollision
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DISABLE_COLLISION);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_DISABLE_COLLISION,value);}
		}

		public bool FlagFluidActorReaction
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_ACTOR_REACTION);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_FLUID_ACTOR_REACTION,value);}
		}

		public bool FlagDisableResponse
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_DISABLE_RESPONSE);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_DISABLE_RESPONSE,value);}
		}

		public bool FlagDynamicDynamicCCD
		{
			get{return NovodexUtil.areBitsSet(shapeFlags,(uint)NxShapeFlag.NX_SF_DYNAMIC_DYNAMIC_CCD);}
			set{shapeFlags=NovodexUtil.setBits(shapeFlags,(uint)NxShapeFlag.NX_SF_DYNAMIC_DYNAMIC_CCD,value);}
		}
	}
}

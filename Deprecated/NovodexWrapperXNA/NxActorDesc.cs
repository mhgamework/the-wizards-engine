//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxActorDesc
	{
		public NxMat34			globalPose;			//!< The pose of the actor in the world.
		public IntPtr			bodyDescPtr;		//!< Body descriptor, null for static actors
		public float			density;			//!< We can compute the mass from a density and the shapes
		public uint				flags;				//!< Combination of ::NxActorFlag flags
		public ushort			group;				//!< Actor group.  See NxActor::setGroup().
		public IntPtr			userData;			//!< Will be copied to NxActor::userData
		public IntPtr			internalNamePtr;	//!< Possible debug name.  The string is not copied by the SDK, only the pointer is stored.
		public NxActorDescType	type;
		private ArrayList		shapeDescList;
		private NxBodyDesc		internalBodyDesc;	//This makes sure the bodyDesc isn't garbage collected and that there is a proper reference available in case you change the values of the object returned by BodyDesc
		public string			name;

		public static NxActorDesc Default
			{get{return new NxActorDesc();}}

		public NxActorDesc()
			{setToDefault();}

		//Pass in null for the bodyDesc to make a static actor
		public NxActorDesc(NxShapeDesc shapeDesc,NxBodyDesc bodyDesc,float density,Matrix globalPose)
		{
			setToDefault();
			if(shapeDesc!=null){addShapeDesc(shapeDesc);}
			this.BodyDesc=bodyDesc;
			this.density=density;
			this.globalPose=NovodexUtil.convertMatrixToNxMat34(globalPose);
		}

		public void setToDefault()
		{
			globalPose			= NxMat34.Identity;
			bodyDescPtr			= IntPtr.Zero;
			density				= 0.0f;
			flags				= 0;
			group				= 0;
			userData			= IntPtr.Zero;
			internalNamePtr		= IntPtr.Zero;
			type				= NxActorDescType.NX_ADT_DEFAULT;
			shapeDescList		= new ArrayList();
			internalBodyDesc	= null;
			name				= null;
		}
		
		public void addShapeDesc(NxShapeDesc shapeDesc)
			{shapeDescList.Add(shapeDesc);}
			
		public void clearShapeDesc()
			{shapeDescList.Clear();}
			
		public int numShapeDescs()
			{return shapeDescList.Count;}

		public IntPtr[] getShapeDescPtrs()
		{
			IntPtr[] shapeDescPtrs=new IntPtr[numShapeDescs()];
			for(int i=0;i<shapeDescList.Count;i++)
				{shapeDescPtrs[i]=((NxShapeDesc)shapeDescList[i]).getAddress();}
			return shapeDescPtrs;
		}

		public NxShapeDesc[] getShapeDescs()
		{
			NxShapeDesc[] shapeDescArray=new NxShapeDesc[numShapeDescs()];
			for(int i=0;i<shapeDescList.Count;i++)
				{shapeDescArray[i]=(NxShapeDesc)shapeDescList[i];}
			return shapeDescArray;
		}

		public NxBodyDesc BodyDesc
		{
			get{return internalBodyDesc;}
			set
			{
				if(value==null)
				{
					bodyDescPtr=IntPtr.Zero;
					internalBodyDesc=null;
				}
				else
				{
					internalBodyDesc=new NxBodyDesc(value);
					bodyDescPtr=internalBodyDesc.getAddress();
				}
			}
		}
		
		public bool FlagDisableCollision
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxActorFlag.NX_AF_DISABLE_COLLISION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxActorFlag.NX_AF_DISABLE_COLLISION,value);}
		}
		
		public bool FlagDisableResponse
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxActorFlag.NX_AF_DISABLE_RESPONSE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxActorFlag.NX_AF_DISABLE_RESPONSE,value);}
		}

		public bool FlagFluidActorReaction
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxActorFlag.NX_AF_FLUID_ACTOR_REACTION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxActorFlag.NX_AF_FLUID_ACTOR_REACTION,value);}
		}
		
		public bool FlagFluidDisableCollision
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxActorFlag.NX_AF_FLUID_DISABLE_COLLISION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxActorFlag.NX_AF_FLUID_DISABLE_COLLISION,value);}
		}

		public bool FlagLockCom
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxActorFlag.NX_AF_LOCK_COM);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxActorFlag.NX_AF_LOCK_COM,value);}
		}
	}
}







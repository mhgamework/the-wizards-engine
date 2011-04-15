//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxMaterialDesc
	{
		public float			dynamicFriction;
		public float			staticFriction;
		public float			restitution;
		public float			dynamicFrictionV;
		public float			staticFrictionV;
		public Vector3			dirOfAnisotropy;
		public uint				flags;
		public NxCombineMode	frictionCombineMode;
		public NxCombineMode	restitutionCombineMode;
		public IntPtr			springPtr;

		private NxSpringDesc	internalSpringDesc; //this is to keep a valid object for springPtr to point at.


		public static NxMaterialDesc Default
			{get{return new NxMaterialDesc();}}

		public NxMaterialDesc()
			{setToDefault();}
			
		public NxMaterialDesc(float dynamicFriction,float staticFriction,float restitution)
		{
			setToDefault();
			this.dynamicFriction=dynamicFriction;
			this.staticFriction=staticFriction;
			this.restitution=restitution;
		}
		
		public NxMaterialDesc(float dynamicFriction,float staticFriction,float restitution,NxSpringDesc springDesc)
		{
			setToDefault();
			this.dynamicFriction=dynamicFriction;
			this.staticFriction=staticFriction;
			this.restitution=restitution;
			this.setSpring(springDesc);
			
			FlagSpringContact=true;	//allow spring contact because a spring was passed in
		}
		
		public void setToDefault()
		{
			dynamicFriction			= 0.0f;
			staticFriction			= 0.0f;
			restitution				= 0.0f;
			dynamicFrictionV		= 0.0f;
			staticFrictionV			= 0.0f;
			dirOfAnisotropy			= new Vector3(1,0,0);
			flags					= 0;
			frictionCombineMode		= NxCombineMode.NX_CM_AVERAGE;
			restitutionCombineMode	= NxCombineMode.NX_CM_AVERAGE;
			springPtr				= IntPtr.Zero;

			internalSpringDesc = NxSpringDesc.Default;
		}
		
		public bool FlagAnisotropic
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxMaterialFlag.NX_MF_ANISOTROPIC);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxMaterialFlag.NX_MF_ANISOTROPIC,value);}
		}
		
		public bool FlagSpringContact
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxMaterialFlag.NX_MF_SPRING_CONTACT);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxMaterialFlag.NX_MF_SPRING_CONTACT,value);}
		}


		public NxSpringDesc getSpring()
			{return NxSpringDesc.createFromPointer(springPtr);}	//If springPtr==null it returns NxSpringDesc.Default

		public void setSpring(NxSpringDesc springDesc)
		{
			internalSpringDesc=springDesc;
			springPtr=internalSpringDesc.getAddress();
		}
	}
}

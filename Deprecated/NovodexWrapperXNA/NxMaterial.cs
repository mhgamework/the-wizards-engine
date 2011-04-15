//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	public class NxMaterial
	{
		protected IntPtr nxMaterialPtr;

		public IntPtr NxMaterialPtr
			{get{return nxMaterialPtr;}}

		public NxMaterial(IntPtr materialPointer)
			{nxMaterialPtr=materialPointer;}
			
		public static NxMaterial createFromPointer(IntPtr materialPointer)
		{
			if(materialPointer==IntPtr.Zero)
				{return null;}
			return new NxMaterial(materialPointer);
		}

		virtual public void destroy()
		{
			ParentScene.releaseMaterial(this);
			nxMaterialPtr=IntPtr.Zero;
		}




		public IntPtr UserData
		{
			get{return wrapper_Material_getUserData(nxMaterialPtr);}
			set{wrapper_Material_setUserData(nxMaterialPtr,value);}
		}
		
		public ushort MaterialIndex
			{get{return getMaterialIndex();}}
			
		public NxScene ParentScene
			{get{return getScene();}}

		public float DynamicFriction
		{
			get{return getDynamicFriction();}
			set{setDynamicFriction(value);}
		}

		public float StaticFriction
		{
			get{return getStaticFriction();}
			set{setStaticFriction(value);}
		}

		public float Restitution
		{
			get{return getRestitution();}
			set{setRestitution(value);}
		}

		public float DynamicFrictionV
		{
			get{return getDynamicFrictionV();}
			set{setDynamicFrictionV(value);}
		}

		public float StaticFrictionV
		{
			get{return getStaticFrictionV();}
			set{setStaticFrictionV(value);}
		}

		public Vector3 DirOfAnisotropy
		{
			get{return getDirOfAnisotropy();}
			set{setDirOfAnisotropy(value);}
		}

		public NxSpringDesc Spring
		{
			get{return getSpring();}
			set{setSpring(value);}
		}

		public uint Flags
		{
			get{return getFlags();}
			set{setFlags(value);}
		}

		public NxCombineMode FrictionCombineMode
		{
			get{return getFrictionCombineMode();}
			set{setFrictionCombineMode(value);}
		}

		public NxCombineMode RestitutionCombineMode
		{
			get{return getRestitutionCombineMode();}
			set{setRestitutionCombineMode(value);}
		}

		public bool FlagAnisotropic
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxMaterialFlag.NX_MF_ANISOTROPIC);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxMaterialFlag.NX_MF_ANISOTROPIC,value));}
		}
		
		public bool FlagSpringContact
		{
			get{return NovodexUtil.areBitsSet(getFlags(),(uint)NxMaterialFlag.NX_MF_SPRING_CONTACT);}
			set{setFlags(NovodexUtil.setBits(getFlags(),(uint)NxMaterialFlag.NX_MF_SPRING_CONTACT,value));}
		}








		public ushort getMaterialIndex()
			{return wrapper_Material_getMaterialIndex(nxMaterialPtr);}

		public void loadFromDesc(NxMaterialDesc materialDesc)
			{wrapper_Material_loadFromDesc(nxMaterialPtr,materialDesc);}

		public void saveToDesc(NxMaterialDesc materialDesc)
			{wrapper_Material_saveToDesc(nxMaterialPtr,materialDesc);}
			
		public NxMaterialDesc getMaterialDesc()
		{
			NxMaterialDesc materialDesc=NxMaterialDesc.Default;
			saveToDesc(materialDesc);
			return materialDesc;
		}

		public NxScene getScene()
			{return NxScene.createFromPointer(wrapper_Material_getScene(nxMaterialPtr));}
			
		public void setDynamicFriction(float dynamicFriction)
			{wrapper_Material_setDynamicFriction(nxMaterialPtr,dynamicFriction);}

		public float getDynamicFriction()
			{return wrapper_Material_getDynamicFriction(nxMaterialPtr);}

		public void setStaticFriction(float staticFriction)
			{wrapper_Material_setStaticFriction(nxMaterialPtr,staticFriction);}

		public float getStaticFriction()
			{return wrapper_Material_getStaticFriction(nxMaterialPtr);}

		public void setRestitution(float restitution)
			{wrapper_Material_setRestitution(nxMaterialPtr,restitution);}

		public float getRestitution()
			{return wrapper_Material_getRestitution(nxMaterialPtr);}

		public void setDynamicFrictionV(float dynamicFrictionV)
			{wrapper_Material_setDynamicFrictionV(nxMaterialPtr,dynamicFrictionV);}

		public float getDynamicFrictionV()
			{return wrapper_Material_getDynamicFrictionV(nxMaterialPtr);}

		public void setStaticFrictionV(float staticFrictionV)
			{wrapper_Material_setStaticFrictionV(nxMaterialPtr,staticFrictionV);}

		public float getStaticFrictionV()
			{return wrapper_Material_getStaticFrictionV(nxMaterialPtr);}


		public void setDirOfAnisotropy(Vector3 dirOfAnisotropy)
			{wrapper_Material_setDirOfAnisotropy(nxMaterialPtr,ref dirOfAnisotropy);}

		public Vector3 getDirOfAnisotropy()
		{
			Vector3 dirOfAnisotropy;
			wrapper_Material_getDirOfAnisotropy(nxMaterialPtr,out dirOfAnisotropy);
			return dirOfAnisotropy;
		}

		public void setFlags(uint flags)
			{wrapper_Material_setFlags(nxMaterialPtr,flags);}

		public uint getFlags()
			{return wrapper_Material_getFlags(nxMaterialPtr);}

		public void setFrictionCombineMode(NxCombineMode frictionCombineMode)
			{wrapper_Material_setFrictionCombineMode(nxMaterialPtr,frictionCombineMode);}

		public NxCombineMode getFrictionCombineMode()
			{return wrapper_Material_getFrictionCombineMode(nxMaterialPtr);}

		public void setRestitutionCombineMode(NxCombineMode restitutionCombineMode)
			{wrapper_Material_setRestitutionCombineMode(nxMaterialPtr,restitutionCombineMode);}

		public NxCombineMode getRestitutionCombineMode()
			{return wrapper_Material_getRestitutionCombineMode(nxMaterialPtr);}

		public void setSpring(NxSpringDesc springDesc)
			{wrapper_Material_setSpring(nxMaterialPtr,springDesc);}

		public NxSpringDesc getSpring()
		{
			NxSpringDesc springDesc=new NxSpringDesc();
			wrapper_Material_getSpring(nxMaterialPtr,springDesc);
			return springDesc;
		}





	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern ushort wrapper_Material_getMaterialIndex(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_loadFromDesc(IntPtr material,NxMaterialDesc materialDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_saveToDesc(IntPtr material,NxMaterialDesc materialDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Material_getScene(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setDynamicFriction(IntPtr material,float dynamicFriction);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Material_getDynamicFriction(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setStaticFriction(IntPtr material,float staticFriction);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Material_getStaticFriction(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setRestitution(IntPtr material,float restitution);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Material_getRestitution(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setDynamicFrictionV(IntPtr material,float dynamicFrictionV);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Material_getDynamicFrictionV(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setStaticFrictionV(IntPtr material,float staticFrictionV);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_Material_getStaticFrictionV(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setDirOfAnisotropy(IntPtr material,ref Vector3 dirOfAnisotropy);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_getDirOfAnisotropy(IntPtr material,out Vector3 dirOfAnisotropy);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setFlags(IntPtr material,uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_Material_getFlags(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setFrictionCombineMode(IntPtr material,NxCombineMode frictionCombineMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxCombineMode wrapper_Material_getFrictionCombineMode(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setRestitutionCombineMode(IntPtr material,NxCombineMode restitutionCombineMode);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxCombineMode wrapper_Material_getRestitutionCombineMode(IntPtr material);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setSpring(IntPtr material,NxSpringDesc springDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_getSpring(IntPtr material,NxSpringDesc springDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Material_setUserData(IntPtr material,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Material_getUserData(IntPtr material);
	}
}







/*
X	virtual		NxMaterialIndex getMaterialIndex() = 0;
X	virtual		void			loadFromDesc(const NxMaterialDesc&) = 0;
-	virtual		void			saveToDesc(NxMaterialDesc&) const	= 0;
-	virtual		NxScene&		getScene() = 0;
X	virtual		void			setDynamicFriction(NxReal) = 0;
X	virtual		NxReal			getDynamicFriction() const = 0;
X	virtual		void			setStaticFriction(NxReal) = 0;
X	virtual		NxReal			getStaticFriction() const = 0;
X	virtual		void			setRestitution(NxReal) = 0;
X	virtual		NxReal			getRestitution() const = 0;
-	virtual		void			setDynamicFrictionV(NxReal) = 0;
-	virtual		NxReal			getDynamicFrictionV() const = 0;
-	virtual		void			setStaticFrictionV(NxReal) = 0;
-	virtual		NxReal			getStaticFrictionV() const = 0;
-	virtual		void			setDirOfAnisotropy(const NxVec3 &) = 0;
-	virtual		NxVec3			getDirOfAnisotropy() const = 0;
-	virtual		void			setFlags(NxU32) = 0;
-	virtual		NxU32			getFlags() const = 0;
-	virtual		void			setFrictionCombineMode(NxCombineMode) = 0;
-	virtual		NxCombineMode	getFrictionCombineMode() const = 0;
-	virtual		void			setRestitutionCombineMode(NxCombineMode) = 0;
-	virtual		NxCombineMode	getRestitutionCombineMode() const = 0;
-	virtual		void			setSpring(const NxSpringDesc &desc) = 0;
-	virtual		NxSpringDesc	getSpring() const = 0;
*/

//	void*			userData;	//!< user can assign this to whatever, usually to create a 1:1 relationship with a user object.

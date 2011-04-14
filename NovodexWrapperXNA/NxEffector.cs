//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



//should it be abstract like joint??
//actually that doesn't matter yet because Novodex doesn't give proper access to Effectors yet.


namespace NovodexWrapper
{
	public class NxEffector
	{
		protected IntPtr nxEffectorPtr;
		
		public NxEffector(IntPtr effectorPointer)
			{nxEffectorPtr=effectorPointer;}
		
		public static NxEffector createFromPointer(IntPtr effectorPointer)
		{
			if(effectorPointer==IntPtr.Zero)
				{return null;}
				
//crap
//if it is abstract this needs to be specialized here
				
			return new NxEffector(effectorPointer);
		}
		
		virtual public void internalAfterRelease()
			{nxEffectorPtr=IntPtr.Zero;}
		
		virtual public void destroy()
		{
			ParentScene.releaseEffector(this);
			nxEffectorPtr=IntPtr.Zero;
		}



		public IntPtr NxEffectorPtr
			{get{return nxEffectorPtr;}}
			
		public NxScene ParentScene
			{get{return getScene();}}
			
		public IntPtr UserData
		{
			get{return wrapper_Effector_getUserData(nxEffectorPtr);}
			set{wrapper_Effector_setUserData(nxEffectorPtr,value);}
		}
		
		
		
		
		virtual public NxScene getScene()
			{return NxScene.createFromPointer(wrapper_Effector_getScene(nxEffectorPtr));}

		virtual public NxSpringAndDamperEffector isSpringAndDamperEffector()
			{return NxSpringAndDamperEffector.createFromPointer(wrapper_Effector_isSpringAndDamperEffector(nxEffectorPtr));}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Effector_getScene(IntPtr effector);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Effector_setUserData(IntPtr effector,IntPtr userData);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Effector_getUserData(IntPtr effector);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Effector_isSpringAndDamperEffector(IntPtr effector);
	}
}


/*
-	virtual		NxSpringAndDamperEffector*	isSpringAndDamperEffector() = 0;
-	virtual		NxScene&					getScene() = 0;
*/



//By Jason Zelsnack, All rights reserved
 
using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Collections;


namespace NovodexWrapper
{
	public class NxController
	{
		#region Whacky Region
		/////////////////////////////////////////////////////////////////////////
		private class PointerType
		{
			public IntPtr pointer;
			public NxControllerType type;
			
			public PointerType(IntPtr pointer,NxControllerType type)
			{
				this.pointer=pointer;
				this.type=type;
			}
		}

		private static NxControllerType findControllerType(IntPtr controllerPointer)
		{
			foreach(PointerType pT in pointerTypeList)
			{
				if(pT.pointer==controllerPointer)
					{return pT.type;}
			}
			return NxControllerType.UNKNOWN;
		}
		
		private static void addToPointerTypeList(IntPtr controllerPointer,NxControllerType type)
		{
			if(findControllerType(controllerPointer)==NxControllerType.UNKNOWN)
				{pointerTypeList.Add(new PointerType(controllerPointer,type));}
		}
				
		private static void removePointerTypeFromList(IntPtr controllerPointer)
		{
			for(int i=0;i<pointerTypeList.Count;i++)
			{
				PointerType pT=(PointerType)pointerTypeList[i];
				if(pT.pointer==controllerPointer)
				{
					pointerTypeList.RemoveAt(i);
					return;
				}
			}
		}
		
		static ArrayList pointerTypeList=new ArrayList();
		/////////////////////////////////////////////////////////////////////////
		#endregion


		protected IntPtr nxControllerPtr;

		public NxController(IntPtr controllerPointer)
			{nxControllerPtr=controllerPointer;}

		public static NxController createFromPointer(IntPtr controllerPointer)
		{
			if(controllerPointer==IntPtr.Zero)
				{return null;}
			return createFromPointerAndType(controllerPointer,findControllerType(controllerPointer));
		}


		public NxControllerType getControllerType()
			{return findControllerType(nxControllerPtr);}


		public static NxController createFromPointerAndType(IntPtr controllerPointer,NxControllerType type)
		{
			if(controllerPointer==IntPtr.Zero)
				{return null;}
				
			addToPointerTypeList(controllerPointer,type);

			if(type==NxControllerType.NX_CONTROLLER_CAPSULE)
				{return new NxCapsuleController(controllerPointer);}
			else if(type==NxControllerType.NX_CONTROLLER_BOX)
				{return new NxBoxController(controllerPointer);}
			else
				{return null;}
		}

		virtual public void internalAfterRelease()
		{
			removePointerTypeFromList(nxControllerPtr);
			nxControllerPtr=IntPtr.Zero;
		}
		
		virtual public void destroy()
		{
			ControllerManager.releaseController(this);
			nxControllerPtr=IntPtr.Zero;
		}



		public IntPtr NxControllerPtr
			{get{return nxControllerPtr;}}
		
		public IntPtr AppData
			{get{return wrapper_NxController_getAppData(nxControllerPtr);}}





		virtual public void move(Vector3 displacement,uint activeGroups,float minDist,out uint collisionFlags,float sharpness)
			{wrapper_NxController_move(nxControllerPtr,ref displacement,activeGroups,minDist,out collisionFlags,sharpness);}

		virtual public bool setPosition(Vector3 position)
			{return wrapper_NxController_setPosition(nxControllerPtr,ref position);} 

		virtual public Vector3 getPosition()
		{
			Vector3 position;
			wrapper_NxController_getPosition(nxControllerPtr,out position);
			return position;
		}

		virtual public Vector3 getFilteredPosition()
		{
			Vector3 filteredPosition;
			wrapper_NxController_getFilteredPosition(nxControllerPtr,out filteredPosition);
			return filteredPosition;
		}
			
		virtual public NxActor getActor()
			{return NxActor.createFromPointer(wrapper_NxController_getActor(nxControllerPtr));}

		virtual public void setStepOffset(float stepOffset)
			{wrapper_NxController_setStepOffset(nxControllerPtr,stepOffset);} 

		virtual public void setCollision(bool enabled)
			{wrapper_NxController_setCollision(nxControllerPtr,enabled);} 
			 
		virtual public void reportSceneChanged()
			{wrapper_NxController_reportSceneChanged(nxControllerPtr);}

		virtual public Vector3 getDebugPosition()
		{
			Vector3 debugPosition=new Vector3(0,0,0);
			wrapper_NxController_getDebugPosition(nxControllerPtr,ref debugPosition);
			return debugPosition;
		}

		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_NxController_getAppData(IntPtr controller);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_move(IntPtr controller,ref Vector3 displacement,uint activeGroups,float minDist,out uint collisionFlags,float sharpness);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_NxController_setPosition(IntPtr controller,ref Vector3 position);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_getPosition(IntPtr controller,out Vector3 position);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_getFilteredPosition(IntPtr controller,out Vector3 filteredPosition);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_NxController_getActor(IntPtr controller);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_setStepOffset(IntPtr controller,float stepOffset);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_setCollision(IntPtr controller,bool enabled);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_reportSceneChanged(IntPtr controller);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxController_getDebugPosition(IntPtr controller,ref Vector3 debugPosition);
	}
}

/*
-	virtual		void			move(const NxVec3& disp, NxU32 activeGroups, NxF32 minDist, NxU32& collisionFlags, NxF32 sharpness=1.0f)	= 0;
-	virtual		bool			setPosition(const NxVec3& position) = 0;
-	virtual		const NxVec3&	getPosition()			const	= 0;
-	virtual		const NxVec3&	getFilteredPosition()	const	= 0;
-	virtual		NxActor*		getActor(void) const = 0;
-	virtual	    void			setStepOffset(const float offs) =0;
-	virtual		void			setCollision(bool enabled)		= 0;
-	virtual		void			reportSceneChanged()			= 0;
-	NX_INLINE	void*			getAppData()	{ return appData;	}
-	virtual		NxExtendedVec3&	getDebugPosition()		const	= 0;
*/








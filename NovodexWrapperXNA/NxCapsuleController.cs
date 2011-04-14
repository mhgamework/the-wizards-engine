//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	public class NxCapsuleController : NxController
	{
		public NxCapsuleController(IntPtr controllerPointer) : base(controllerPointer)
			{}



		public float Radius
		{
			get{return getRadius();}
			set{setRadius(value);}
		}

		public float Height
		{
			get{return getHeight();}
			set{setHeight(value);}
		}







		virtual public float getRadius()
			{return wrapper_NxCapsuleController_getRadius(nxControllerPtr);}

		virtual public bool setRadius(float radius)
			{return wrapper_NxCapsuleController_setRadius(nxControllerPtr,radius);}

		virtual public float getHeight()
			{return wrapper_NxCapsuleController_getHeight(nxControllerPtr);}

		virtual public bool setHeight(float height)
			{return wrapper_NxCapsuleController_setHeight(nxControllerPtr,height);}

		public override void setStepOffset(float stepOffset)
			{wrapper_NxCapsuleController_setStepOffset(nxControllerPtr,stepOffset);}

		public override void reportSceneChanged()
			{wrapper_NxCapsuleController_reportSceneChanged(nxControllerPtr);}






		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_NxCapsuleController_getRadius(IntPtr capsuleController);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_NxCapsuleController_setRadius(IntPtr capsuleController,float radius);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_NxCapsuleController_getHeight(IntPtr capsuleController);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_NxCapsuleController_setHeight(IntPtr capsuleController,float height);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxCapsuleController_setStepOffset(IntPtr capsuleController,float stepOffset);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxCapsuleController_reportSceneChanged(IntPtr controller);
	}
}


/*
-	virtual		NxF32			getRadius() const = 0;
-	virtual		bool			setRadius(NxF32 radius) = 0;
-	virtual		NxF32			getHeight() const = 0;
-	virtual		bool			setHeight(NxF32 height) = 0;
-	virtual	    void			setStepOffset(const float offset) =0;
-	virtual		void			reportSceneChanged() = 0;
};
*/


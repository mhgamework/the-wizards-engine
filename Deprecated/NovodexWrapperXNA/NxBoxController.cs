//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	public class NxBoxController : NxController
	{
		public NxBoxController(IntPtr controllerPointer) : base(controllerPointer)
			{}



		public Vector3 Extents
		{
			get{return getExtents();}
			set{setExtents(value);}
		}



		public Vector3 getExtents()
		{
			Vector3 extents;
			wrapper_NxBoxController_getExtents(nxControllerPtr,out extents);
			return extents;
		}

		virtual public bool setExtents(Vector3 extents)
			{return wrapper_NxBoxController_setExtents(nxControllerPtr,ref extents);}

		public override void setStepOffset(float stepOffset)
			{wrapper_NxBoxController_setStepOffset(nxControllerPtr,stepOffset);}

		public override void reportSceneChanged()
			{wrapper_NxBoxController_reportSceneChanged(nxControllerPtr);}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxBoxController_getExtents(IntPtr boxController,out Vector3 extents);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_NxBoxController_setExtents(IntPtr boxController,ref Vector3 extents);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxBoxController_setStepOffset(IntPtr boxController,float stepOffset);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_NxBoxController_reportSceneChanged(IntPtr controller);
	}
}

/*
-	virtual		const NxVec3&	getExtents() const = 0;
-	virtual		bool			setExtents(const NxVec3& extents) = 0;
-	virtual	    void			setStepOffset(const float offs) =0;
-	virtual		void			reportSceneChanged() = 0;
*/

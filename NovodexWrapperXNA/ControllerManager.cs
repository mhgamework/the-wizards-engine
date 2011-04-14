//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	public class ControllerManager
	{
		static public int getNbControllers()
			{return wrapper_ControllerManager_getNbControllers();}

		static public Controller[] getControllers()
		{
			int numControllers=getNbControllers();
			Controller[] controllerArray=new Controller[numControllers];
			IntPtr controllersPtr=wrapper_ControllerManager_getControllers();

			for(int i=0;i<numControllers;i++)
			{
				controllerArray[i]=new Controller(controllersPtr);	//The first time this is set to the value above, subsequent times it is set to the value below.
				controllersPtr=wrapper_ControllerManager_getNextController(controllersPtr);
			}

			return controllerArray;
		}

		static public NxController createController(NxScene scene,NxControllerDesc controllerDesc)
		{
			if(controllerDesc is NxCapsuleControllerDesc)
			{
				NxCapsuleControllerDesc cD=(NxCapsuleControllerDesc)controllerDesc;
				IntPtr controllerPtr=wrapper_ControllerManager_createCapsuleController(scene.NxScenePtr,ref cD.position,cD.upDirection,cD.slopeLimit,cD.skinWidth,cD.stepOffset,cD.callbackPtr,cD.userData,cD.radius,cD.height);
				return NxController.createFromPointerAndType(controllerPtr,cD.getControllerType());
			}
			else if(controllerDesc is NxBoxControllerDesc)
			{
				NxBoxControllerDesc cD=(NxBoxControllerDesc)controllerDesc;
				IntPtr controllerPtr=wrapper_ControllerManager_createBoxController(scene.NxScenePtr,ref cD.position,cD.upDirection,cD.slopeLimit,cD.skinWidth,cD.stepOffset,cD.callbackPtr,cD.userData,ref cD.extents);
				return NxController.createFromPointerAndType(controllerPtr,cD.getControllerType());
			}
			else
				{return null;}
		}

		static public void purgeControllers()
			{wrapper_ControllerManager_purgeControllers();}
		
		static public void releaseController(NxController controller)
		{
			wrapper_ControllerManager_releaseController(controller.NxControllerPtr);
			controller.internalAfterRelease();
		}
		
		static public void removeController(Controller controller)
		{
			wrapper_ControllerManager_removeController(controller.ControllerPtr);
			controller.internalAfterRelease();
		}

		static public void updateControllers()
			{wrapper_ControllerManager_updateControllers();}
		
		
		
		
	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_ControllerManager_getNbControllers();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ControllerManager_getControllers();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ControllerManager_createCapsuleController(IntPtr scenePtr,ref Vector3 position,NxHeightFieldAxis upDirection,float slopeLimit,float skinWidth,float stepOffset,IntPtr callbackPtr,IntPtr userDataPtr,float radius,float height);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ControllerManager_createBoxController(IntPtr scenePtr,ref Vector3 position,NxHeightFieldAxis upDirection,float slopeLimit,float skinWidth,float stepOffset,IntPtr callbackPtr,IntPtr userDataPtr,ref Vector3 extents);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ControllerManager_purgeControllers();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ControllerManager_releaseController(IntPtr controllerPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ControllerManager_removeController(IntPtr controllerPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_ControllerManager_updateControllers();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_ControllerManager_getNextController(IntPtr controllerPtr);
	}
}

/*
-	NxU32			getNbControllers()			const	{ return nbControllers;		}
-	Controller*		getControllers()					{ return controllerList;	}
-	NxController*	createController(NxScene* scene, const NxControllerDesc&);
-	void			purgeControllers();
-	void			releaseController(NxController&);
-	void			removeController(Controller*);
-	void			updateControllers();
	void			printStats();

	NxU32			nbControllers;				//
	Controller*		controllerList;				//linked list of scene controllers.
*/



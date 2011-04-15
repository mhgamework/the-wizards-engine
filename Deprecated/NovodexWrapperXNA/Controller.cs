//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	public class Controller
	{
		protected IntPtr controllerPtr;

		public Controller(IntPtr controllerPointer)
			{controllerPtr=controllerPointer;}

		public static Controller createFromPointer(IntPtr controllerPointer)
		{
			if(controllerPointer==IntPtr.Zero)
				{return null;}
			return new Controller(controllerPointer);
		}

		virtual public void internalAfterRelease()
			{controllerPtr=IntPtr.Zero;}
		
		virtual public void destroy()
		{
			ControllerManager.removeController(this);
			controllerPtr=IntPtr.Zero;
		}

		public IntPtr ControllerPtr
			{get{return controllerPtr;}}
		
		
		
		
		public bool getWorldBox(NxBounds3 box)
			{return wrapper_Controller_getWorldBox(controllerPtr,box);}
		
		public NxController getController()
			{return NxController.createFromPointer(wrapper_Controller_getController(controllerPtr));}
		
		public NxActor getActor()
			{return NxActor.createFromPointer(wrapper_Controller_getActor(controllerPtr));}
		
			

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_Controller_getWorldBox(IntPtr controller,NxBounds3 box);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Controller_getController(IntPtr controller);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Controller_getActor(IntPtr controller);
	}
}





/*
// User params
	NxHeightFieldAxis			upDirection;
	NxF32						slopeLimit;
	NxF32						skinWidth;
	NxF32						stepOffset;
	NxUserControllerHitReport*	callback;
	void*						userData;

// Internal data
	void*						cctModule;			// Internal CCT object. Optim test for Ubi.
	NxActor*					kineActor;			// Associated kinematic actor
	NxExtendedVec3				position;			// Current position
	NxExtendedVec3				filteredPosition;	// Current position after feedback filter
	NxExtendedVec3				exposedPosition;	// Position visible from the outside at any given time
	Extended					memory;				// Memory variable for feedback filter
	NxScene*					scene;				// Handy scene owner
	Controller*					next;				// Linked list of controllers
	ControllerManager*			manager;			// Owner manager
	bool						handleSlope;		// True to handle walkable parts according to slope
*/

/*
-	virtual	bool				getWorldBox(NxExtendedBounds3& box)	const	= 0;
-	virtual	NxController*		getController()						= 0;
-	virtual	NxActor*			getActor()					const	{ return kineActor; };

//Protected
	bool						setPos(const NxExtendedVec3& pos);
	void						setCollision(bool enabled);
	void						move(SweptVolume& volume, const NxVec3& disp, NxU32 activeGroups, NxF32 minDist, NxU32& collisionFlags, NxF32 sharpness);

//NonExistant because NEW_CALLBACKS_DESIGN isn't defined
	virtual	void				ShapeHitCallback(const SweptContact& contact, const Point& dir, float length);
	virtual	void				UserHitCallback(const SweptContact& contact, const Point& dir, float length);
*/




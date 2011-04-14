//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;





//The Helper class is to make the interface cleaner to implement and to do some book keeping
// to keep track of the managed and native objects inheriting the interface

namespace NovodexWrapper
{
	public delegate bool OnJointBreakDelegate(float breakingForce,IntPtr brokenJointPtr);


	public class NxUserNotifyHelper
	{
		static ArrayList userNotifyHelperList=new ArrayList();

		private OnJointBreakDelegate breakDelegate=null;
		private NxScene parentScene=null;
		private NxUserNotify userNotify=null;

		private NxUserNotifyHelper(NxScene scene,NxUserNotify userNotify)
		{
			if(scene==null || userNotify==null)
				{return;}

			foreach(NxUserNotifyHelper userNotifyHelper in userNotifyHelperList)
			{
				if(userNotifyHelper.parentScene==scene)
				{
					userNotifyHelperList.Remove(userNotifyHelper);
					break;
				}
			}
			
			userNotifyHelperList.Add(this);
			parentScene=scene;
			this.userNotify=userNotify;
		}
			

		static public void setUserNotify(NxScene scene,NxUserNotify userNotify)
		{
			NxUserNotifyHelper userNotifyHelper=new NxUserNotifyHelper(scene,userNotify);

			if(userNotify==null)
				{wrapper_UserNotify_setOnJointBreakCallback(scene.NxScenePtr,null);}
			else
			{
				userNotifyHelper.breakDelegate=new OnJointBreakDelegate(userNotifyHelper.onJointBreak);
				wrapper_UserNotify_setOnJointBreakCallback(scene.NxScenePtr,userNotifyHelper.breakDelegate);
			}
		}

		static public NxUserNotify getUserNotify(NxScene scene)
		{
			foreach(NxUserNotifyHelper userNotifyHelper in userNotifyHelperList)
			{
				if(userNotifyHelper.parentScene==scene)
					{return userNotifyHelper.userNotify;}
			}
			return null;
		}
		
			
		public bool onJointBreak(float breakingForce,IntPtr brokenJointPtr)
		{
			if(userNotify!=null)
			{
				NxJoint brokenJoint=NxJoint.createFromPointer(brokenJointPtr);
				return userNotify.onJointBreak(breakingForce,brokenJoint);
			}
			return true;	//defaults to throwing the joint away completely
		}
	
	
	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserNotify_setOnJointBreakCallback(IntPtr scene,OnJointBreakDelegate breakDelegate);
	}




	public interface NxUserNotify
	{
		//It is recommended to always return true. Returning false keeps broken joints around and they are null
		bool onJointBreak(float breakingForce,NxJoint brokenJoint);
	}
}









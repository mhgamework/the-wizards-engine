//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;



//The Helper class is to make the interface cleaner to implement and to do some book keeping
// to keep track of the managed and native objects inheriting the interface

namespace NovodexWrapper
{
	public delegate void OnTriggerDelegate(IntPtr triggerShapePtr,IntPtr otherShapePtr,NxShapeFlag status);

	public class NxUserTriggerReportHelper
	{
		static ArrayList userTriggerReportHelperList=new ArrayList();
		
		private OnTriggerDelegate triggerDelegate=null;
		private NxScene parentScene=null;
		private NxUserTriggerReport userTriggerReport=null;



		private NxUserTriggerReportHelper(NxScene scene,NxUserTriggerReport userTriggerReport)
		{
			if(scene==null || userTriggerReport==null)
				{return;}

			foreach(NxUserTriggerReportHelper userTriggerReportHelper in userTriggerReportHelperList)
			{
				if(userTriggerReportHelper.parentScene==scene)
				{
					userTriggerReportHelperList.Remove(userTriggerReportHelper);
					break;
				}
			}
			
			userTriggerReportHelperList.Add(this);
			parentScene=scene;
			this.userTriggerReport=userTriggerReport;
		}




		static public void setUserTriggerReport(NxScene scene,NxUserTriggerReport userTriggerReport)
		{
			NxUserTriggerReportHelper userTriggerReportHelper=new NxUserTriggerReportHelper(scene,userTriggerReport);
			
			if(userTriggerReport==null)
				{wrapper_UserTriggerReport_setOnTriggerCallback(scene.NxScenePtr,null);}
			else
			{
				userTriggerReportHelper.triggerDelegate=new OnTriggerDelegate(userTriggerReportHelper.onTrigger);
				wrapper_UserTriggerReport_setOnTriggerCallback(scene.NxScenePtr,userTriggerReportHelper.triggerDelegate);
			}
		}

		static public NxUserTriggerReport getUserTriggerReport(NxScene scene)
		{
			foreach(NxUserTriggerReportHelper userTriggerReportHelper in userTriggerReportHelperList)
			{
				if(userTriggerReportHelper.parentScene==scene)
					{return userTriggerReportHelper.userTriggerReport;}
			}
			return null;
		}
	
	
	
			
		public void onTrigger(IntPtr triggerShapePtr,IntPtr otherShapePtr,NxShapeFlag status)
		{
			if(userTriggerReport!=null)
			{
				NxShape triggerShape=NxShape.createFromPointer(triggerShapePtr);
				NxShape otherShape=NxShape.createFromPointer(otherShapePtr);
				userTriggerReport.onTrigger(triggerShape,otherShape,status);
			}
		}
	
	
	
	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserTriggerReport_setOnTriggerCallback(IntPtr scene,OnTriggerDelegate triggerDelegate);
	}




	public interface NxUserTriggerReport
	{
		void onTrigger(NxShape triggerShape,NxShape otherShape,NxShapeFlag status);
	}
}



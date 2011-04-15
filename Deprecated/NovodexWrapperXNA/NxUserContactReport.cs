//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;



//The Helper class is to make the interface cleaner to implement and to do some book keeping
// to keep track of the managed and native objects inheriting the interface

namespace NovodexWrapper
{
	public delegate void OnContactNotifyDelegate(NxContactPair contactPair,uint events);


	public class NxUserContactReportHelper
	{
		static ArrayList userContactReportHelperList=new ArrayList();

		private OnContactNotifyDelegate contactNotifyDelegate=null;
		private NxScene parentScene=null;
		private NxUserContactReport userContactReport=null;

		private NxUserContactReportHelper(NxScene scene)
			{parentScene=scene;}




		private NxUserContactReportHelper(NxScene scene,NxUserContactReport userContactReport)
		{
			if(scene==null || userContactReport==null)
				{return;}

			foreach(NxUserContactReportHelper userContactReportHelper in userContactReportHelperList)
			{
				if(userContactReportHelper.parentScene==scene)
				{
					userContactReportHelperList.Remove(userContactReportHelper);
					break;
				}
			}
			
			userContactReportHelperList.Add(this);
			parentScene=scene;
			this.userContactReport=userContactReport;
		}







		static public void setContactReport(NxScene scene,NxUserContactReport userContactReport)
		{
			NxUserContactReportHelper userContactReportHelper=new NxUserContactReportHelper(scene,userContactReport);
						
			if(userContactReport==null)
				{wrapper_UserContactReport_setOnContactNotifyCallback(scene.NxScenePtr,null);}
			else
			{
				userContactReportHelper.contactNotifyDelegate=new OnContactNotifyDelegate(userContactReportHelper.onContactNotify);
				wrapper_UserContactReport_setOnContactNotifyCallback(scene.NxScenePtr,userContactReportHelper.contactNotifyDelegate);
			}
		}

		static public NxUserContactReport getContactReport(NxScene scene)
		{
			foreach(NxUserContactReportHelper userContactReportHelper in userContactReportHelperList)
			{
				if(userContactReportHelper.parentScene==scene)
					{return userContactReportHelper.userContactReport;}
			}
			return null;
		}



			
		public void onContactNotify(NxContactPair contactPair,uint events)
		{
			if(userContactReport!=null)
				{userContactReport.onContactNotify(contactPair,events);}
		}
	

	
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserContactReport_setOnContactNotifyCallback(IntPtr scene,OnContactNotifyDelegate contactNotifyDelegate);
	}




	public interface NxUserContactReport
	{
		void onContactNotify(NxContactPair contactPair,uint events);
	}
}



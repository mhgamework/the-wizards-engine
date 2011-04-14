//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	public delegate bool OnHitDelegate(ref NxRaycastHit raycastHit);
	

	abstract unsafe public class NxUserRaycastReport
	{
		static ArrayList userRaycastReportList=new ArrayList();

		private IntPtr nxUserRaycastReportPtr=IntPtr.Zero;
		private OnHitDelegate onHitDelegate=null;


		public NxUserRaycastReport()
			{create();}

		~NxUserRaycastReport()
			{destroy();}


		public IntPtr NxUserRaycastReportPtr
			{get{return nxUserRaycastReportPtr;}}


		private void create()
		{
			if(!userRaycastReportList.Contains(this))
			{
				setCallbacks();
				nxUserRaycastReportPtr=wrapper_UserRaycastReport_create(onHitDelegate);
				userRaycastReportList.Add(this);
			}
		}
		
		private void destroy()
		{
			if(userRaycastReportList.Contains(this))
			{
				userRaycastReportList.Remove(this);
				wrapper_UserRaycastReport_destroy(nxUserRaycastReportPtr);
			}
			nxUserRaycastReportPtr=IntPtr.Zero;
		}
		
		static public NxUserRaycastReport getFromPointer(IntPtr reportPtr)
		{
			foreach(NxUserRaycastReport hitReport in userRaycastReportList)
			{
				if(hitReport.nxUserRaycastReportPtr==reportPtr)
					{return hitReport;}
			}
			return null;
		}

		private void setCallbacks()
			{onHitDelegate=new OnHitDelegate(this.onHit);}

		public abstract bool onHit(ref NxRaycastHit raycastHit);


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_UserRaycastReport_create(OnHitDelegate onHitDelegate);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserRaycastReport_destroy(IntPtr userRaycastReportPtr);
	}
}


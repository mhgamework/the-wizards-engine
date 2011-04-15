//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;


//This NxUserEntityReport is specific for Shapes only.


namespace NovodexWrapper
{
	public delegate bool OnEventDelegate(uint nbShapes,IntPtr shapes);

	abstract unsafe public class NxUserEntityReport
	{
		static ArrayList userEntityReportList=new ArrayList();

		private IntPtr nxUserEntityReportPtr=IntPtr.Zero;
		private OnEventDelegate	onEventDelegate=null;


		public NxUserEntityReport()
			{create();}

		~NxUserEntityReport()
			{destroy();}


		public IntPtr NxUserEntityReportPtr
			{get{return nxUserEntityReportPtr;}}


		private void create()
		{
			if(!userEntityReportList.Contains(this))
			{
				setCallbacks();
				nxUserEntityReportPtr=wrapper_UserEntityReport_create(onEventDelegate);
				userEntityReportList.Add(this);
			}
		}
		
		private void destroy()
		{
			if(userEntityReportList.Contains(this))
			{
				userEntityReportList.Remove(this);
				wrapper_UserEntityReport_destroy(nxUserEntityReportPtr);
			}
			nxUserEntityReportPtr=IntPtr.Zero;
		}

		private void setCallbacks()
			{onEventDelegate=new OnEventDelegate(this.onEvent);}

		//It seems to call this in batches of 64. If there were 100 shapes this would be called twice. The first time nbShapes would be 64 the second time it would be 36.
		private bool onEvent(uint nbShapes,IntPtr shapes)
		{
			NxShape[] shapeArray=new NxShape[nbShapes];
			unsafe
			{
				int* buf=(int*)shapes.ToPointer();	//I cast from void* to int* because for some reason you can't index elements from a void*
				for(int i=0;i<nbShapes;i++)
					{shapeArray[i]=NxShape.createFromPointer(new IntPtr(buf[i]));}
			}
			return onEvent(shapeArray);
		}


		//Return true to continue processing, false to end processing.
		public abstract bool onEvent(NxShape[] shapeArray);



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_UserEntityReport_create(OnEventDelegate onEventDelegate);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserEntityReport_destroy(IntPtr userRaycastReportPtr);
	}
}



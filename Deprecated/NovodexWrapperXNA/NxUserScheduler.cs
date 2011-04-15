//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	public delegate void AddTaskDelegate(IntPtr taskPtr);
	public delegate void AddBackgroundTaskDelegate(IntPtr taskPtr);
	public delegate void WaitTasksCompleteDelegate();



	public class NxTask
	{
		protected IntPtr nxTaskPtr=IntPtr.Zero;

		public NxTask(IntPtr taskPointer)
			{nxTaskPtr=taskPointer;}

		public IntPtr NxTaskPtr
			{get{return nxTaskPtr;}}

		//Originally this was abstract, but the class couldn't be instantiated as abstract
		virtual public void execute()
			{wrapper_Task_execute(nxTaskPtr);}

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Task_execute(IntPtr task);
	}



	abstract public class NxUserScheduler
	{
		protected IntPtr nxUserSchedulerPtr=IntPtr.Zero;
		
		private AddTaskDelegate addTaskDelegate=null;
		private AddBackgroundTaskDelegate addBackgroundTaskDelegate=null;
		private WaitTasksCompleteDelegate waitTasksCompleteDelegate=null;

		static private ArrayList staticUserSchedulerList=new ArrayList();


		public NxUserScheduler()
			{internalCreate();}

		public IntPtr NxUserSchedulerPtr
			{get{return nxUserSchedulerPtr;}}

		private void internalCreate()
		{
			addTaskDelegate=new AddTaskDelegate(this.internalAddTask);
			addBackgroundTaskDelegate=new AddBackgroundTaskDelegate(this.internalAddBackgroundTask);
			waitTasksCompleteDelegate=new WaitTasksCompleteDelegate(this.waitTasksComplete);
	
			nxUserSchedulerPtr=wrapper_UserScheduler_create(addTaskDelegate,addBackgroundTaskDelegate,waitTasksCompleteDelegate);

			staticUserSchedulerList.Add(this);
		}

		//This should only be called after the scene is destroyed.
		public void internalDestroy()
		{
			wrapper_UserScheduler_destroy(nxUserSchedulerPtr);
			nxUserSchedulerPtr=IntPtr.Zero;
			
			staticUserSchedulerList.Remove(this);
		}

		static public NxUserScheduler internalGetUserSchedulerByPointer(IntPtr pointer)
		{
			foreach(NxUserScheduler s in staticUserSchedulerList)
			{
				unsafe
				{
					if(s.NxUserSchedulerPtr.ToPointer()==pointer.ToPointer())
						{return s;}
				}
			}
			return null;
		}


		private void internalAddTask(IntPtr taskPtr)
			{addTask(new NxTask(taskPtr));}

		private void internalAddBackgroundTask(IntPtr taskPtr)
			{addBackgroundTask(new NxTask(taskPtr));}




		abstract public void addTask(NxTask task);
		abstract public void addBackgroundTask(NxTask task);
		abstract public void waitTasksComplete();

		


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_UserScheduler_create(AddTaskDelegate addTaskDelegate,AddBackgroundTaskDelegate addBackgroundTaskDelegate,WaitTasksCompleteDelegate waitTasksCompleteDelegate);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_UserScheduler_destroy(IntPtr userScheduler);
	}
}


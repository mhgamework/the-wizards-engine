//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	public class NxFoundationSDK
	{
		protected IntPtr nxFoundationSDKptr;

		public NxFoundationSDK(IntPtr foundationSDKpointer)
			{nxFoundationSDKptr=foundationSDKpointer;}

		public static NxFoundationSDK createFromPointer(IntPtr foundationSDKpointer)
		{
			if(foundationSDKpointer==IntPtr.Zero)
				{return null;}
			return new NxFoundationSDK(foundationSDKpointer);
		}

		public IntPtr NxFoundationSDKptr
			{get{return nxFoundationSDKptr;}}



		virtual public void release()
			{wrapper_FoundationSDK_release(nxFoundationSDKptr);}

		virtual public NxErrorCode getLastError()
			{return wrapper_FoundationSDK_getLastError(nxFoundationSDKptr);}
		
		virtual public NxErrorCode getFirstError()
			{return wrapper_FoundationSDK_getFirstError(nxFoundationSDKptr);}

		virtual public NxRemoteDebugger getRemoteDebugger()
		{
			IntPtr remoteDebuggerPointer=wrapper_FoundationSDK_getRemoteDebugger(nxFoundationSDKptr);
			return NxRemoteDebugger.createFromPointer(remoteDebuggerPointer);
		}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FoundationSDK_release(IntPtr foundationSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_FoundationSDK_setErrorStream(IntPtr foundationSDK,IntPtr outputStreamPtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_FoundationSDK_getErrorStream(IntPtr foundationSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxErrorCode wrapper_FoundationSDK_getLastError(IntPtr foundationSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxErrorCode wrapper_FoundationSDK_getFirstError(IntPtr foundationSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_FoundationSDK_getAllocator(IntPtr foundationSDK);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_FoundationSDK_getRemoteDebugger(IntPtr foundationSDK);
	}
}


/*
-	virtual	void release() = 0;
-	virtual NxErrorCode getLastError() = 0;
-	virtual NxErrorCode getFirstError() = 0;
	virtual void setErrorStream(NxUserOutputStream *stream) = 0;
	virtual NxUserOutputStream * getErrorStream() = 0;
	virtual NxUserAllocator & getAllocator() = 0;
X	virtual NxRemoteDebugger * getRemoteDebugger() = 0;
*/






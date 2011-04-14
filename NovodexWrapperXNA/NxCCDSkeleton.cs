//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
 	public class NxCCDSkeleton
	{
		protected IntPtr nxCCDSkeletonPtr;

		public NxCCDSkeleton(IntPtr nxCCDSkeletonPointer)
			{nxCCDSkeletonPtr=nxCCDSkeletonPointer;}

		public static NxCCDSkeleton createFromPointer(IntPtr nxCCDSkeletonPointer)
		{
			if(nxCCDSkeletonPointer==IntPtr.Zero)
				{return null;}
			return new NxCCDSkeleton(nxCCDSkeletonPointer);
		}

		virtual public void internalAfterRelease()
			{nxCCDSkeletonPtr=IntPtr.Zero;}

		public IntPtr NxCCDSkeletonPtr
			{get{return nxCCDSkeletonPtr;}}



		virtual public uint getDataSize()
			{return wrapper_CCDSkeleton_getDataSize(nxCCDSkeletonPtr);}

		unsafe virtual public uint save(byte[] buffer)
		{
			if(buffer==null || buffer.Length==0)
				{return 0;}
				
			fixed(void* x=&buffer[0])
				{return wrapper_CCDSkeleton_save(nxCCDSkeletonPtr,new IntPtr(x),(uint)buffer.Length);}
		}


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_CCDSkeleton_getDataSize(IntPtr skeleton);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_CCDSkeleton_save(IntPtr skeleton,IntPtr destBuffer,uint bufferSize);
	}
}


//virtual NxU32 save(void * destBuffer, NxU32 bufferSize) = 0;
//virtual NxU32 getDataSize() = 0;











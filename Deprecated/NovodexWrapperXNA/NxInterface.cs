//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	public class NxInterface
	{
		protected IntPtr nxInterfacePtr;

		public NxInterface(IntPtr interfacePointer)
			{nxInterfacePtr=interfacePointer;}

		public static NxInterface createFromPointer(IntPtr interfacePointer)
		{
			if(interfacePointer==IntPtr.Zero)
				{return null;}

			NxInterfaceType interfaceType=wrapper_Interface_getInterfaceType(interfacePointer);
			if(interfaceType==NxInterfaceType.NX_INTERFACE_STATS)
				{return new NxInterfaceStats(interfacePointer);}

			return new NxInterface(interfacePointer);
		}


		public IntPtr NxInterfacePtr
			{get{return nxInterfacePtr;}}

		virtual public int getVersion()
			{return wrapper_Interface_getVersionNumber(nxInterfacePtr);}

		virtual public NxInterfaceType getInterfaceType()
			{return wrapper_Interface_getInterfaceType(nxInterfacePtr);}


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_Interface_getVersionNumber(IntPtr interfacePtr);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxInterfaceType wrapper_Interface_getInterfaceType(IntPtr interfacePtr);
	}





	public class NxInterfaceStats : NxInterface
	{
		public NxInterfaceStats(IntPtr interfacePointer) : base(interfacePointer)
		{}

		new public static NxInterfaceStats createFromPointer(IntPtr interfacePointer)
		{
			if(interfacePointer==IntPtr.Zero)
			{return null;}
			return new NxInterfaceStats(interfacePointer);
		}

		virtual public bool getHeapSize(out int used,out int unused)
		{return wrapper_InterfaceStats_getHeapSize(nxInterfacePtr,out used,out unused);}

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_InterfaceStats_getHeapSize(IntPtr interfaceStats,out int used,out int unused);
	}
}



/*
//From NxInterface
X	virtual int             getVersionNumber(void) const = 0;
X	virtual NxInterfaceType getInterfaceType(void) const = 0;
*/


/*
//From NxInterfaceStats
#	virtual int             getVersionNumber(void) const { return NX_INTERFACE_STATS_VERSION; };
#	virtual NxInterfaceType getInterfaceType(void) const { return NX_INTERFACE_STATS; };
X	virtual bool	        getHeapSize(int &used,int &unused) = 0;
*/

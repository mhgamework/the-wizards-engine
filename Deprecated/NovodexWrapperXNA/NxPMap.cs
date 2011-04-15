//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxPMap
	{
		public uint		dataSize;		//!< size of data buffer in bytes
		public IntPtr	dataPtr;		//!< data buffer that stores the PMap information.

		public static NxPMap Default
			{get{return new NxPMap();}}
			
		public NxPMap()
		{
			dataSize=0;
			dataPtr=IntPtr.Zero;
		}


		
		static public bool createPMap(NxPMap pmap,NxTriangleMesh triangleMesh,uint density)
			{return wrapper_PMap_createPMap(pmap,triangleMesh.NxTriangleMeshPtr,density);}
		
		static public bool releasePMap(NxPMap pmap)
			{return wrapper_PMap_releasePMap(pmap);}



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_PMap_createPMap(NxPMap pmap,IntPtr triangleMeshPtr,uint density);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_PMap_releasePMap(NxPMap pmap);
	}
}


















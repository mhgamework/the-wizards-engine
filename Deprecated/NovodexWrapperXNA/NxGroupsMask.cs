//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxGroupsMask
	{
		public static NxGroupsMask Default
			{get{return new NxGroupsMask(0,0,0,0);}}
    
		public NxGroupsMask(uint bits0,uint bits1,uint bits2,uint bits3)
		{
			this.bits0=bits0;
			this.bits1=bits1;
			this.bits2=bits2;
			this.bits3=bits3;
		}
	
		public uint bits0, bits1, bits2, bits3;
	}
}

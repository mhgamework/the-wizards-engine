//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxPairFlag
	{
		public IntPtr objectPtr_0;
		public IntPtr objectPtr_1;
		public uint flags;

		public uint isActorPair()
			{return flags&0x80000000;}
	}
}




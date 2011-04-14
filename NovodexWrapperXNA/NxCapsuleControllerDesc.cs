//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxCapsuleControllerDesc : NxControllerDesc
	{
		public float radius;
		public float height;
		
		
		public static NxCapsuleControllerDesc Default
			{get{return new NxCapsuleControllerDesc();}}

		public NxCapsuleControllerDesc()
			{setToDefault();}
			
		public override void setToDefault()
		{
			base.setToDefault();
			radius = 0;
			height = 0;
			base.type=NxControllerType.NX_CONTROLLER_CAPSULE;
		}
	}
}


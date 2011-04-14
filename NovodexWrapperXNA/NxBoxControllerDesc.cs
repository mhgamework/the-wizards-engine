//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxBoxControllerDesc : NxControllerDesc
	{
		public Vector3 extents;
		
				
		public static NxBoxControllerDesc Default
			{get{return new NxBoxControllerDesc();}}

		public NxBoxControllerDesc()
			{setToDefault();}
			
		public override void setToDefault()
		{
			base.setToDefault();
			extents=new Vector3(0.5f,1.0f,0.5f);
			base.type=NxControllerType.NX_CONTROLLER_BOX;
		}
	}
}

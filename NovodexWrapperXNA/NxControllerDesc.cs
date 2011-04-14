//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxControllerDesc
	{
		public uint					version;
		public NxControllerType		type;			//!< The type of the controller. This gets set by the derived class' ctor, the user should not have to change it.
		public Vector3				position;
		public NxHeightFieldAxis	upDirection;
		public float				slopeLimit;
		public float				skinWidth;
		public float				stepOffset;
		public IntPtr				callbackPtr;	//Originally NxUserControllerHitReport*
		public IntPtr				userData;

		public NxControllerDesc()
			{setToDefault();}

		virtual public void setToDefault()
		{
			version		= 0;
			upDirection	= NxHeightFieldAxis.NX_Y;
			slopeLimit	= 0.707f;
			skinWidth	= 0.1f;
			stepOffset	= 0.5f;
			callbackPtr	= IntPtr.Zero;
			userData	= IntPtr.Zero;
			position	= new Vector3(0,0,0);
		}

		public uint getVersion()
			{return version;}

		public NxControllerType getControllerType()
			{return type;}
			
		public NxUserControllerHitReport Callback
		{
			get{return NxUserControllerHitReport.getFromPointer(callbackPtr);}
			set{callbackPtr=value.NxUserControllerHitReportPtr;}
		}
	}
}





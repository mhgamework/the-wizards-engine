//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSpringDesc
	{
		public float spring;		//!< spring coefficient
		public float damper;		//!< damper coefficient
		public float targetValue;	//!< target value (angle/position) of spring where the spring force is zero.
		
		public static NxSpringDesc Default
			{get{return new NxSpringDesc();}}
		
		public NxSpringDesc()
			{setToDefault();}

		public NxSpringDesc(float spring,float damper,float targetValue)
			{set(spring,damper,targetValue);}
		
		public void set(float spring,float damper,float targetValue)
		{
			this.spring=spring;
			this.damper=damper;
			this.targetValue=targetValue;
		}
		
		public void setToDefault()
		{
			spring=0;
			damper=0;
			targetValue=0;
		}
		
		unsafe public IntPtr getAddress()
		{
			fixed(void* x=&spring)
				{return new IntPtr(x);}
		}
		
		unsafe static public int getSizeof()
			{return sizeof(float)*3;}

		unsafe static public NxSpringDesc createFromPointer(IntPtr ptr)
		{
			NxSpringDesc springDesc=NxSpringDesc.Default;
			if(ptr!=IntPtr.Zero)
			{			
				float* x=(float*)ptr.ToPointer();
				springDesc.spring=x[0];
				springDesc.damper=x[1];
				springDesc.targetValue=x[2];
			}
			return springDesc;
		}
	}









	//This was made for NxWheelShapeDesc because suspension needed to be a struct. Elsewhere NxSpringDesc needs to be a class.
	[StructLayout(LayoutKind.Sequential)]
	public struct NxSpringDesc2
	{
		public float spring;
		public float damper;
		public float targetValue;

		public void set(NxSpringDesc springDesc)
			{set(springDesc.spring,springDesc.damper,springDesc.targetValue);}
		
		public void set(float spring,float damper,float targetValue)
		{
			this.spring=spring;
			this.damper=damper;
			this.targetValue=targetValue;
		}

		public NxSpringDesc convertToNxSpringDesc()
			{return new NxSpringDesc(spring,damper,targetValue);}
	}

}



//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxMotorDesc
	{
		public float velTarget;		//target velocity of motor
		public float maxForce;		//maximum motor force/torque
		public bool freeSpin;		//If true, motor will not brake when it spins faster than velTarget

		public static NxMotorDesc Default
			{get{return new NxMotorDesc();}}

		public NxMotorDesc()
			{setToDefault();}
			
		public NxMotorDesc(float velTarget,float maxForce,bool freeSpin)
			{set(velTarget,maxForce,freeSpin);}
			
		public void set(float velTarget,float maxForce,bool freeSpin)
		{
			this.velTarget=velTarget;
			this.maxForce=maxForce;
			this.freeSpin=freeSpin;
		}

		public void setToDefault()
		{
			velTarget = float.MaxValue;
			maxForce = 0;
			freeSpin = false;		
		}
	}
}



/*
class NxMotorDesc
{
	NxReal velTarget;	//target velocity of motor
	NxReal maxForce;	//maximum motor force/torque
	NX_BOOL freeSpin;
};
*/
//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;


//You should use getSuspension() and setSuspension() which use NxSpringDesc class instead of the NxSpringDesc2 struct

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxWheelShapeDesc : NxShapeDesc
	{
		public float				radius;							//distance from wheel axle to a point on the contact surface.
		public float				suspensionTravel;				//maximum extension distance of suspension along shape's -Y axis.
		public NxSpringDesc2		internalSuspension2;			//data intended for car wheel suspension effects		
		public NxTireFunctionDesc	longitudalTireForceFunction;	//cubic hermite spline coefficients describing the longitudal tire force curve
		public NxTireFunctionDesc	lateralTireForceFunction;		//cubic hermite spline coefficients describing the lateral tire force curve
		public float				inverseWheelMass;				//inverse mass of the wheel. Determines the wheel velocity that wheel torques can achieve.
		public uint					wheelFlags;						//flags from NxWheelShapeFlags
		public float				motorTorque;					//Sum engine torque on the wheel axle. Positive or negative depending on direction.
		public float				brakeTorque;					//The amount of torque applied for braking. Must be positive. Very large values should lock wheel but should be stable.
		public float				steerAngle;						//steering angle, around shape Y axis
	
		public static NxWheelShapeDesc Default
			{get{return new NxWheelShapeDesc();}}

		public NxWheelShapeDesc()
			{setToDefault();}


		//Uses default values for base class NxShapeDesc.
		public NxWheelShapeDesc(float radius,float suspensionTravel,float inverseWheelMass,uint wheelFlags,float motorTorque,float brakeTorque,float steerAngle)
		{
			setToDefault();
			this.radius=radius;
			this.suspensionTravel=suspensionTravel;
			this.inverseWheelMass=inverseWheelMass;
			this.wheelFlags=wheelFlags;
			this.motorTorque=motorTorque;
			this.brakeTorque=brakeTorque;
			this.steerAngle=steerAngle;

			internalSuspension2.set(0,0,0);
		}
		
		public NxWheelShapeDesc(float radius,float suspensionTravel,float inverseWheelMass,uint wheelFlags,float motorTorque,float brakeTorque,float steerAngle,Matrix localPose)
		{
			setToDefault();
			this.radius=radius;
			this.suspensionTravel=suspensionTravel;
			this.inverseWheelMass=inverseWheelMass;
			this.wheelFlags=wheelFlags;
			this.motorTorque=motorTorque;
			this.brakeTorque=brakeTorque;
			this.steerAngle=steerAngle;

			internalSuspension2.set(0,0,0);

			this.localPose=NovodexUtil.convertMatrixToNxMat34(localPose);
		}


		override public void setToDefault()
		{
			base.setToDefault();

			radius						= 1.0f;				
			suspensionTravel			= 1.0f;	
			inverseWheelMass			= 1.0f;
			wheelFlags					= 0;
			motorTorque					= 0.0f;
			brakeTorque					= 0.0f;
			steerAngle					= 0.0f;
			longitudalTireForceFunction = NxTireFunctionDesc.Default;
			lateralTireForceFunction	= NxTireFunctionDesc.Default;

			internalSuspension2.set(NxSpringDesc.Default);

			type=NxShapeType.NX_SHAPE_WHEEL;
		}

		public bool FlagWheelAxisContactNormal
		{
			get{return NovodexUtil.areBitsSet(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_WHEEL_AXIS_CONTACT_NORMAL);}
			set{wheelFlags=NovodexUtil.setBits(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_WHEEL_AXIS_CONTACT_NORMAL,value);}
		}

		public bool FlagInputLateralSlipVelocity
		{
			get{return NovodexUtil.areBitsSet(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_INPUT_LAT_SLIPVELOCITY);}
			set{wheelFlags=NovodexUtil.setBits(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_INPUT_LAT_SLIPVELOCITY,value);}
		}

		public bool FlagInputLongitudalSlipVelocity
		{
			get{return NovodexUtil.areBitsSet(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_INPUT_LNG_SLIPVELOCITY);}
			set{wheelFlags=NovodexUtil.setBits(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_INPUT_LNG_SLIPVELOCITY,value);}
		}

		public bool FlagUnscaledSpringBehavior
		{
			get{return NovodexUtil.areBitsSet(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_UNSCALED_SPRING_BEHAVIOR);}
			set{wheelFlags=NovodexUtil.setBits(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_UNSCALED_SPRING_BEHAVIOR,value);}
		}

		public bool FlagAxleSpeedOverride
		{
			get{return NovodexUtil.areBitsSet(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_AXLE_SPEED_OVERRIDE);}
			set{wheelFlags=NovodexUtil.setBits(wheelFlags,(uint)NxWheelShapeFlags.NX_WF_AXLE_SPEED_OVERRIDE,value);}
		}


		//Two separate functions instead of an accessor because otherwise you can go: NxWheelShapeDesc.suspension.set(1,2,3) and would have no effect because you're setting the values of a newly created object that has nothing to do with NxWheelShapeDesc
		public void setSuspension(NxSpringDesc suspension)
			{internalSuspension2.set(suspension);}

		public NxSpringDesc getSuspension(NxSpringDesc suspension)
			{return internalSuspension2.convertToNxSpringDesc();}
	}
}

		





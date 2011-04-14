//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	public class NxWheelShape : NxShape
	{
		public NxWheelShape(IntPtr shapePointer) : base(shapePointer)
			{}

		public static NxWheelShape createFromPointer(NxActor actor,NxWheelShapeDesc shapeDesc,IntPtr shapePointer)
		{
			if(shapePointer==IntPtr.Zero)
				{return null;}
			return new NxWheelShape(shapePointer);
		}
		
		public float Radius
		{
			get{return getRadius();}
			set{setRadius(value);}
		}

		public float SuspensionTravel
		{
			get{return getSuspensionTravel();}
			set{setSuspensionTravel(value);}
		}

		public NxSpringDesc Suspension
		{
			get{return getSuspension();}
			set{setSuspension(value);}
		}

		public NxTireFunctionDesc LongitudalTireForceFunction
		{
			get{return getLongitudalTireForceFunction();}
			set{setLongitudalTireForceFunction(value);}
		}

		public NxTireFunctionDesc LateralTireForceFunction
		{
			get{return getLateralTireForceFunction();}
			set{setLateralTireForceFunction(value);}
		}

		public float InverseWheelMass
		{
			get{return getInverseWheelMass();}
			set{setInverseWheelMass(value);}
		}
		
		public uint WheelFlags
		{
			get{return getWheelFlags();}
			set{setWheelFlags(value);}
		}

		public float MotorTorque
		{
			get{return getMotorTorque();}
			set{setMotorTorque(value);}
		}

		public float BrakeTorque
		{
			get{return getBrakeTorque();}
			set{setBrakeTorque(value);}
		}
		
		public float SteerAngle
		{
			get{return getSteerAngle();}
			set{setSteerAngle(value);}
		}
		
		public float AxleSpeed
		{
			get{return getAxleSpeed();}
			set{setAxleSpeed(value);}
		}
		




		public bool FlagWheelAxisContactNormal
		{
			get{return NovodexUtil.areBitsSet(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_WHEEL_AXIS_CONTACT_NORMAL);}
			set{setWheelFlags(NovodexUtil.setBits(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_WHEEL_AXIS_CONTACT_NORMAL,value));}
		}

		public bool FlagInputLateralSlipVelocity
		{
			get{return NovodexUtil.areBitsSet(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_INPUT_LAT_SLIPVELOCITY);}
			set{setWheelFlags(NovodexUtil.setBits(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_INPUT_LAT_SLIPVELOCITY,value));}
		}

		public bool FlagInputLongitudalSlipVelocity
		{
			get{return NovodexUtil.areBitsSet(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_INPUT_LNG_SLIPVELOCITY);}
			set{setWheelFlags(NovodexUtil.setBits(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_INPUT_LNG_SLIPVELOCITY,value));}
		}

		public bool FlagUnscaledSpringBehavior
		{
			get{return NovodexUtil.areBitsSet(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_UNSCALED_SPRING_BEHAVIOR);}
			set{setWheelFlags(NovodexUtil.setBits(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_UNSCALED_SPRING_BEHAVIOR,value));}
		}

		public bool FlagAxleSpeedOverride
		{
			get{return NovodexUtil.areBitsSet(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_AXLE_SPEED_OVERRIDE);}
			set{setWheelFlags(NovodexUtil.setBits(getWheelFlags(),(uint)NxWheelShapeFlags.NX_WF_AXLE_SPEED_OVERRIDE,value));}
		}


		


		public float getRadius()
			{return wrapper_WheelShape_getRadius(nxShapePtr);}

		public void setRadius(float radius)
			{wrapper_WheelShape_setRadius(nxShapePtr,radius);}

		public float getSuspensionTravel()
			{return wrapper_WheelShape_getSuspensionTravel(nxShapePtr);}

		public void setSuspensionTravel(float suspensionTravel)
			{wrapper_WheelShape_setSuspensionTravel(nxShapePtr,suspensionTravel);}

		public NxSpringDesc getSuspension()
		{
			NxSpringDesc springDesc=NxSpringDesc.Default;
			wrapper_WheelShape_getSuspension(nxShapePtr,springDesc);
			return springDesc;
		}

		public void setSuspension(NxSpringDesc springDesc)
			{wrapper_WheelShape_setSuspension(nxShapePtr,springDesc);}

		public NxTireFunctionDesc getLongitudalTireForceFunction()
		{
			NxTireFunctionDesc longitudalTireDesc=NxTireFunctionDesc.Default;
			wrapper_WheelShape_getLongitudalTireForceFunction(nxShapePtr,out longitudalTireDesc);
			return longitudalTireDesc;
		}

		public void setLongitudalTireForceFunction(NxTireFunctionDesc longitudalTireDesc)
			{wrapper_WheelShape_setLongitudalTireForceFunction(nxShapePtr,ref longitudalTireDesc);}

		public NxTireFunctionDesc getLateralTireForceFunction()
		{
			NxTireFunctionDesc lateralTireDesc=NxTireFunctionDesc.Default;
			wrapper_WheelShape_getLateralTireForceFunction(nxShapePtr,out lateralTireDesc);
			return lateralTireDesc;
		}

		public void setLateralTireForceFunction(NxTireFunctionDesc lateralTireDesc)
			{wrapper_WheelShape_setLateralTireForceFunction(nxShapePtr,ref lateralTireDesc);}

		public float getInverseWheelMass()
			{return wrapper_WheelShape_getInverseWheelMass(nxShapePtr);}

		public void setInverseWheelMass(float inverseWheelMass)
			{wrapper_WheelShape_setInverseWheelMass(nxShapePtr,inverseWheelMass);}

		public uint getWheelFlags()
			{return wrapper_WheelShape_getWheelFlags(nxShapePtr);}

		public void setWheelFlags(uint wheelFlags)
			{wrapper_WheelShape_setWheelFlags(nxShapePtr,wheelFlags);}

		public float getMotorTorque()
			{return wrapper_WheelShape_getMotorTorque(nxShapePtr);}

		public void setMotorTorque(float motorTorque)
			{wrapper_WheelShape_setMotorTorque(nxShapePtr,motorTorque);}

		public float getBrakeTorque()
			{return wrapper_WheelShape_getBrakeTorque(nxShapePtr);}

		public void setBrakeTorque(float brakeTorque)
			{wrapper_WheelShape_setBrakeTorque(nxShapePtr,brakeTorque);}

		public float getSteerAngle()
			{return wrapper_WheelShape_getSteerAngle(nxShapePtr);}

		public void setSteerAngle(float steerAngle)
			{wrapper_WheelShape_setSteerAngle(nxShapePtr,steerAngle);}


		public float getAxleSpeed()
			{return wrapper_WheelShape_getAxleSpeed(nxShapePtr);}

		public void setAxleSpeed(float axleSpeed)
			{wrapper_WheelShape_setAxleSpeed(nxShapePtr,axleSpeed);}

		public NxShape getContact(out NxWheelContactData wheelContactData)
		{
			IntPtr shapePtr=wrapper_WheelShape_getContact(nxShapePtr,out wheelContactData);
			return NxShape.createFromPointer(shapePtr);
		}

		public NxWheelContactData getWheelContactData()
		{
			NxWheelContactData wheelContactData;
			getContact(out wheelContactData);
			return wheelContactData;
		}



		new virtual public NxWheelShapeDesc getShapeDesc()
			{return (NxWheelShapeDesc)internalGetShapeDesc();}

		override protected NxShapeDesc internalGetShapeDesc()
		{
			NxWheelShapeDesc shapeDesc=NxWheelShapeDesc.Default;
			saveToDesc(shapeDesc);
			return shapeDesc;
		}
		
		virtual public void saveToDesc(NxWheelShapeDesc shapeDesc)
			{wrapper_WheelShape_saveToDesc(nxShapePtr,ref shapeDesc.localPose,ref shapeDesc.shapeFlags,ref shapeDesc.group,ref shapeDesc.materialIndex,ref shapeDesc.userData,ref shapeDesc.internalNamePtr,ref shapeDesc.radius,ref shapeDesc.suspensionTravel,ref shapeDesc.internalSuspension2,ref shapeDesc.longitudalTireForceFunction,ref shapeDesc.lateralTireForceFunction,ref shapeDesc.inverseWheelMass,ref shapeDesc.wheelFlags,ref shapeDesc.motorTorque,ref shapeDesc.brakeTorque,ref shapeDesc.steerAngle);}


		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getRadius(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setRadius(IntPtr shape,float radius);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getSuspensionTravel(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setSuspensionTravel(IntPtr shape,float suspensionTravel);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_getSuspension(IntPtr shape,NxSpringDesc springDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setSuspension(IntPtr shape,NxSpringDesc springDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getLongitudalTireForceFunction(IntPtr shape,out NxTireFunctionDesc longitudalTireDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setLongitudalTireForceFunction(IntPtr shape,ref NxTireFunctionDesc longitudalTireDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getLateralTireForceFunction(IntPtr shape,out NxTireFunctionDesc lateralTireDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setLateralTireForceFunction(IntPtr shape,ref NxTireFunctionDesc lateralTireDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getInverseWheelMass(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setInverseWheelMass(IntPtr shape,float inverseWheelMass);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_WheelShape_getWheelFlags(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setWheelFlags(IntPtr shape,uint wheelFlags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getMotorTorque(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setMotorTorque(IntPtr shape,float motorTorque);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getBrakeTorque(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setBrakeTorque(IntPtr shape,float brakeTorque);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getSteerAngle(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setSteerAngle(IntPtr shape,float steerAngle);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_WheelShape_getAxleSpeed(IntPtr shape);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_setAxleSpeed(IntPtr shape,float axleSpeed);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_WheelShape_getContact(IntPtr shape,out NxWheelContactData wheelContactData);
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_WheelShape_saveToDesc(IntPtr shape,ref NxMat34 localPose,ref uint shapeFlags,ref ushort group,ref ushort materialIndex,ref IntPtr userData,ref IntPtr namePtr,ref float radius,ref float suspensionTravel,ref NxSpringDesc2 suspension,ref NxTireFunctionDesc longitudalTireForceFunction,ref NxTireFunctionDesc lateralTireForceFunction,ref float inverseWheelMass,ref uint wheelFlags,ref float motorTorque,ref float brakeTorque,ref float steerAngle);
	}
}




/*
-	virtual	void	saveToDesc(NxWheelShapeDesc& desc)		const = 0;
-	virtual	NxReal getRadius() const = 0;
-	virtual	void setRadius(NxReal radius) = 0;
-	virtual	NxReal getSuspensionTravel() const = 0;
-	virtual	void setSuspensionTravel(NxReal travel) = 0;
-	virtual	NxSpringDesc getSuspension() const = 0;
-	virtual	void setSuspension(NxSpringDesc spring) = 0;
-	virtual	NxTireFunctionDesc getLongitudalTireForceFunction() const = 0;
-	virtual	void setLongitudalTireForceFunction(NxTireFunctionDesc tireFunc) = 0;
-	virtual	NxTireFunctionDesc getLateralTireForceFunction() const = 0;
-	virtual	void setLateralTireForceFunction(NxTireFunctionDesc tireFunc) = 0;
-	virtual	NxReal	getInverseWheelMass() const = 0;
-	virtual	void setInverseWheelMass(NxReal invMass) = 0;
-	virtual	NxU32	getWheelFlags() const = 0;
-	virtual	void setWheelFlags(NxU32 flags) = 0;
-	virtual	NxReal getMotorTorque() const = 0;
-	virtual	void setMotorTorque(NxReal torque) = 0;
-	virtual	NxReal getBrakeTorque() const = 0;
-	virtual	void setBrakeTorque(NxReal torque) = 0;
-	virtual	NxReal getSteerAngle() const = 0;
-	virtual	void setSteerAngle(NxReal angle) = 0;
-	virtual	NxReal getAxleSpeed() const = 0;
-	virtual	void setAxleSpeed(NxReal speed) = 0;
-	virtual NxShape * getContact(NxWheelContactData & dest) const = 0;	
*/
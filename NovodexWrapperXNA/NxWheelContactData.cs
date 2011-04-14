//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	public struct NxWheelContactData
	{
		public Vector3 contactPoint;					//brief The point of contact between the wheel shape and the ground.
		public Vector3 contactNormal;					//brief The normal at the point of contact.
		public Vector3 longitudalDirection;				//brief The direction the wheel is pointing in.
		public Vector3 lateralDirection;				//brief The sideways direction for the wheel(at right angles to the longitudal direction).
		public float contactForce;						//brief The magnitude of the force being applied for the contact.
		public float longitudalSlip, lateralSlip;		//brief What these exactly are depend on NX_WF_INPUT_LAT_SLIPVELOCITY and NX_WF_INPUT_LNG_SLIPVELOCITY flags for the wheel.
		public float longitudalImpulse, lateralImpulse;	//brief the clipped impulses applied at the wheel.
		public ushort otherShapeMaterialIndex;			//brief The material index of the shape in contact with the wheel.
	}
}




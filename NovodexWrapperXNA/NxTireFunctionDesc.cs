//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NxTireFunctionDesc
	{
		//The Novodex stuff seems to be aligned at 8 bytes. Instead of defining Pack=8 I just explicitly place them where they belong
		[FieldOffset( 4)]public float extremumSlip;		//!< extremal point of curve.  Must be positive.
		[FieldOffset( 8)]public float extremumValue;		//!< extremal point of curve.  Must be positive.
		[FieldOffset(12)]public float asymptoteSlip;		//!< point on curve at which for all x > minumumX, function equals minimumY.  Must be positive.
		[FieldOffset(16)]public float asymptoteValue;	//!< point on curve at which for all x > minumumX, function equals minimumY.  Must be positive.
		[FieldOffset(20)]public float stiffnessFactor;	//!< this is an additional overall positive scaling that gets applied to the tire forces before passing them to the solver.  Higher values make for better grip.  If you raise the *Values above, you may need to lower this.

		public static NxTireFunctionDesc Default
			{get{return NxTireFunctionDesc.createDefault();}}
		
		private static NxTireFunctionDesc createDefault()
		{
			NxTireFunctionDesc tireDesc=new NxTireFunctionDesc();
			tireDesc.setToDefault();
			return tireDesc;
		}

		public NxTireFunctionDesc(float extremumSlip,float extremumValue,float asymptoteSlip,float asymptoteValue,float stiffnessFactor)
		{
			this.extremumSlip=extremumSlip;
			this.extremumValue=extremumValue;
			this.asymptoteSlip=asymptoteSlip;
			this.asymptoteValue=asymptoteValue;
			this.stiffnessFactor=stiffnessFactor;
		}

		public void setToDefault()
		{
			extremumSlip	= 1.0f;
			extremumValue	= 0.02f;
			asymptoteSlip	= 2.0f;
			asymptoteValue	= 0.01f;	
			stiffnessFactor = 1000000.0f;	//quite stiff by default.
		}
		
		public float hermiteEval(float t)
		{
			float sign;
			if(t<0.0f)
			{
				t=-t;	//function is mirrored around origin.
				sign=-1.0f;
			}
			else
				{sign = 1.0f;}

			if(t<extremumSlip)	//first curve
			{
				//0 at start, with tangent = line to first control point.
				//(x,y) at end, with tangent = 0;
				sign *= extremumValue;
				t /= extremumSlip;
				float A2 = t * t;
				float A3 = A2 * t;
				float c3 = -2*A3+3*A2;
				float c1 = A3-2*A2+t;
				return (c1 + c3) * sign;	//c1 has coeff = tangent == maximum, c3 has coeff maximum.
			}

			if(t>asymptoteSlip)	//beyond minimum
				{return asymptoteValue * sign;}
			else	//second curve
			{
				//between two points (extremumSlip,Y), minimum(X,Y), both with tangent 0.
				//remap t so that maximimX --> 0, asymptoteSlip --> 1
				t /= (asymptoteSlip - extremumSlip);
				t -= extremumSlip;
				float A2 = t * t;
				float A3 = A2 * t;
				float c3 = -2*A3+3*A2;
				float c0 = 2*A3-3*A2+1;
				return (c0 * extremumValue + c3 * asymptoteValue) * sign;
			}
		}

	}
}



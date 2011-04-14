//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSegment
	{
		public Vector3 p0;	//!< Start of segment
		public Vector3 p1;	//!< End of segment

		public NxSegment()
		{
			p0=new Vector3(0,0,0);
			p1=new Vector3(0,0,0);
		}
		
		public NxSegment(Vector3 p0,Vector3 p1)
		{
			this.p0=p0;
			this.p1=p1;
		}

		public NxSegment(NxSegment segment)
		{
			p0=segment.p0;
			p1=segment.p1;
		}

		public Vector3 getOrigin()
			{return p0;}

		public Vector3 computeDirection()
			{return (p1-p0);}
			
		public float computeLength()
			{return ((Vector3)(p1-p0)).Length();}

		public float computeSquareLength()
			{return ((Vector3)(p1-p0)).LengthSquared();}

		public void setOriginDirection(Vector3 origin,Vector3 direction)
		{
			p0=origin;
			p1=origin+direction;
		}

		Vector3 computePoint(float lengthRatio)
			{return (p0+((p1-p0)*lengthRatio));}

		public float squareDistance(Vector3 point)
		{
			float t=0;
			return NxExportedUtils.computeSquareDistance(this,point,ref t);
		}

		public float squareDistance(Vector3 point,ref float t)
			{return NxExportedUtils.computeSquareDistance(this,point,ref t);}

		public float distance(Vector3 point)
			{return (float)Math.Sqrt(squareDistance(point));}

		public float distance(Vector3 point,ref float t)
			{return (float)Math.Sqrt(squareDistance(point,ref t));}
	}
}









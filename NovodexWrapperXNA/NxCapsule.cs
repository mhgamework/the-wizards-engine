//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxCapsule : NxSegment
	{
		public float radius;

		public NxCapsule()
			{radius=0;}
		
		public NxCapsule(Vector3 p0,Vector3 p1,float radius)
		{
			this.p0=p0;
			this.p1=p1;
			this.radius=radius;
		}
			
		public NxCapsule(NxSegment segment,float radius)
		{
			p0=segment.p0;
			p1=segment.p1;
			this.radius=radius;
		}

		public void computeOBB(NxBox box)
			{NxExportedUtils.computeBoxAroundCapsule(this,box);}

		public bool contains(Vector3 point)
			{return squareDistance(point) <= radius*radius;}

		public bool contains(NxSphere sphere)
		{
			float d = radius - sphere.radius;
			if(d>=0.0f)
				{return squareDistance(sphere.center) <= d*d;}
			else
				{return false;}
		}

		public bool contains(NxCapsule capsule)
		   {return contains(new NxSphere(capsule.p0, capsule.radius)) && contains(new NxSphere(capsule.p1, capsule.radius));}
	}
}









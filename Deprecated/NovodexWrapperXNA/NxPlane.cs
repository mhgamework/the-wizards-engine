//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxPlane
	{
		public Vector3		normal;		//!< The normal to the plane
		public float		d;			//!< The distance from the origin 
		
		public NxPlane()
			{set(0,0,0,0);}
		
		public NxPlane(float normalX,float normalY,float normalZ,float d)
			{set(normalX,normalY,normalZ,d);}

		public NxPlane(Vector3 normal,float d)
			{set(normal.X,normal.Y,normal.Z,d);}

		public NxPlane(Vector3 point,Vector3 normal)
			{set(point,normal);}
			
		public NxPlane(Vector3 point0,Vector3 point1,Vector3 point2)
			{set(point0,point1,point2);}
			
		public NxPlane zero()
			{return set(0,0,0,0);}
		
		public NxPlane set(float normalX,float normalY,float normalZ,float d)
		{
			normal=new Vector3(normalX,normalY,normalZ);
			this.d=d;
			return this;
		}
		
		public NxPlane set(Vector3 normal,float d)
		{
			this.normal=normal;
			this.d=d;
			return this;
		}
		
		public NxPlane set(Vector3 point,Vector3 normal)
		{
			this.normal=normal;
			d=-Vector3.Dot(point,normal);
			return this;
		}
		
		public NxPlane set(Vector3 point0,Vector3 point1,Vector3 point2)
		{
			Vector3 Edge0 = point1 - point0;
			Vector3 Edge1 = point2 - point0;
			
			normal = Vector3.Cross(Edge0,Edge1);
			normal.Normalize();
			
			d = -Vector3.Dot(point0,normal);
			return this;
		}		

		public float distance(Vector3 point)
			{return Vector3.Dot(point,normal);}
			


		public bool belongs(Vector3 point)
			{return Math.Abs(distance(point)) < (1.0e-7f);}


		public Vector3 project(Vector3 point)
			{return point - (normal * distance(point));}


		public Vector3 pointInPlane()
			{return - normal * d;}

		public void normalize()
		{
			float Denom = 1.0f / normal.Length();
			normal.X	*= Denom;
			normal.Y	*= Denom;
			normal.Z	*= Denom;
			d			*= Denom;
		}
	}
}



/*
	NX_INLINE void TransformPlane(NxPlane& transformed, const NxPlane& plane, const NxMat34& transform)
	{
		// Rotate the normal using the rotation part of the 4x4 matrix
		transform.M.multiplyByTranspose(plane.normal, transformed.normal);

		// Compute new d
		transformed.d = plane.d - transform.t.dot(transformed.normal);
	}
*/




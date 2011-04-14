//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxSphere
	{
		public Vector3	center;		//!< Sphere's center
		public float	radius;		//!< Sphere's radius

		public NxSphere()
		{
			center=new Vector3(0,0,0);
			radius=0;
		}

		public NxSphere(Vector3 center,float radius)
		{
			this.center=center;
			this.radius=radius;
		}

		public NxSphere(float centerX,float centerY,float centerZ,float radius)
		{
			center=new Vector3(centerX,centerY,centerZ);
			this.radius=radius;
		}
		
		public NxSphere(Vector3[] vertArray)
			{NxExportedUtils.computeSphere(this,vertArray);}

		public NxSphere(NxSphere sphere0,NxSphere sphere1)
			{NxExportedUtils.mergeSpheres(this,sphere0,sphere1);}

		
		public bool isValid()
			{return radius>0;}
			
		
		public bool intersect(NxSphere sphere)
		{
			float r=radius+sphere.radius;
			float distanceSquared=((Vector3)(center-sphere.center)).LengthSquared();
			return distanceSquared <= (r*r);
		}
		
		public bool contains(Vector3 p)
			{return ((Vector3)(center-p)).LengthSquared() <= (radius*radius);}

		public bool contains(NxSphere sphere)
		{
			if(radius < sphere.radius)
				{return false;}
			float r=radius-sphere.radius;
			return ((Vector3)(center-sphere.center)).LengthSquared() <= (r*r);
		}

		public NxBSphereMethod compute(Vector3[] vertArray)
			{return NxExportedUtils.computeSphere(this,vertArray);}

		public bool fastCompute(Vector3[] vertArray)
			{return NxExportedUtils.fastComputeSphere(this,vertArray);}

		public bool contains(Vector3 min,Vector3 max)
		{
			// I assume if all 8 box vertices are inside the sphere, so does the whole box.
			
			float R2 = radius * radius;
			Vector3 p=new Vector3(0,0,0);
			p.X=max.X; p.Y=max.Y; p.Z=max.Z;	if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=min.X;							if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=max.X; p.Y=min.Y;				if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=min.X;							if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=max.X; p.Y=max.Y; p.Z=min.Z;	if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=min.X;							if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=max.X; p.Y=min.Y;				if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}
			p.X=min.X;							if(((Vector3)(center-p)).LengthSquared()>=R2){return false;}

			return true;
		}
	}
}




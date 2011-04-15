//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxRay
	{
		public Vector3		orig;	//!< Ray origin
		public Vector3		dir;	//!< Normalized direction
		
		public NxRay()
		{
			orig=new Vector3(0,0,0);
			dir=new Vector3(0,0,0);
		}
		
		public NxRay(Vector3 pos,Vector3 dir)
		{
			this.orig=pos;
			this.dir=dir;
		}

		public NxRay(float posX,float posY,float posZ,float dirX,float dirY,float dirZ)
		{
			this.orig=new Vector3(posX,posY,posZ);
			this.dir=new Vector3(dirX,dirY,dirZ);
		}



		public Vector3 ComputeReflexionVector(Vector3 incoming_dir,Vector3 outward_normal)
			{return (incoming_dir - outward_normal * 2.0f * Vector3.Dot(incoming_dir,outward_normal));}

		public Vector3 ComputeReflexionVector(Vector3 reflected,Vector3 source,Vector3 impact,Vector3 normal)
		{
			Vector3 V = impact - source;
			return (V - normal * 2.0f * Vector3.Dot(V,normal));
		}

		public Vector3 ComputeNormalCompo(Vector3 outward_dir,Vector3 outward_normal)
			{return outward_normal * Vector3.Dot(outward_dir,outward_normal);}

		public Vector3 ComputeTangentCompo(Vector3 outward_dir,Vector3 outward_normal)
			{return (outward_dir - (outward_normal * Vector3.Dot(outward_dir,outward_normal)));}

		public void DecomposeVector(out Vector3 normal_compo,out Vector3 tangent_compo,Vector3 outward_dir,Vector3 outward_normal)
		{
			normal_compo = outward_normal * Vector3.Dot(outward_dir,outward_normal);
			tangent_compo = outward_dir - normal_compo;
		}	
	}
}






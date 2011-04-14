//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxTriangle
	{
		//This inner struct was made because you can't name Indexers in C#
		[StructLayout(LayoutKind.Sequential)]
		public struct Verts
		{
			public Vector3 a,b,c;
		
			public Vector3 this[int index]
			{
				get 
				{
					if(index==0){return a;}
					else if(index==1){return b;}
					else if(index==2){return c;}
					else{throw new System.IndexOutOfRangeException("NxTriangle: tried to get element "+index);}
				}
				set 
				{
					if(index==0){a=value;}
					else if(index==1){b=value;}
					else if(index==2){c=value;}
					else{throw new System.IndexOutOfRangeException("NxTriangle: tried to set element "+index);}
				}
			}
		}


		public Verts verts;


		public NxTriangle(Vector3 p0,Vector3 p1,Vector3 p2)
		{
			verts.a=p0;
			verts.b=p1;
			verts.c=p2;
		}

		public NxTriangle(NxTriangle triangle)
		{
			verts.a=triangle.verts.a;
			verts.b=triangle.verts.b;
			verts.c=triangle.verts.c;
		}

		public void center(out Vector3 center)
			  {center=(verts[0] + verts[1] + verts[2])*0.33333333333f;}

		public void normal(out Vector3 normal)
		{
			normal=Vector3.Cross((verts[1]-verts[0]),(verts[2]-verts[0]));
			normal.Normalize();
		}

		public void inflate(float fatCoeff,bool constantBorder)
		{
			Vector3 triangleCenter;
			center(out triangleCenter);

			for(int i=0;i<3;i++)
			{
				Vector3 v = verts[i] - triangleCenter;
				if(constantBorder)
					{v.Normalize();}
				verts[i] += v * fatCoeff;
			}
		}

	}
}


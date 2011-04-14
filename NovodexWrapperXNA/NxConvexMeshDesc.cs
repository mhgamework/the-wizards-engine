//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxConvexMeshDesc
	{
		public int numVertices;				//!< Number of vertices.
		public int numTriangles;			//!< Number of triangles.
		public int pointStrideBytes;		//!< Offset between vertex points in bytes.
		public int triangleStrideBytes;		//!< Offset between triangles in bytes.
		public IntPtr pointsPtr;
		public IntPtr trianglesPtr;
		public uint flags;


		protected Vector3[] internalPointArray=null;	//in case the pointArray that the user passed in is deleted before the object is created
		protected int[] internalTriangleArray=null;		//in case the triangleArray that the user passed in is deleted before the object is created


		public static NxConvexMeshDesc Default
			{get{return new NxConvexMeshDesc();}}

		public NxConvexMeshDesc()
			{setToDefault();}

		virtual public void setToDefault()
		{
			numVertices			= 0;
			numTriangles		= 0;
			pointStrideBytes	= 0;
			triangleStrideBytes	= 0;
			pointsPtr			= IntPtr.Zero;
			trianglesPtr		= IntPtr.Zero;
			flags				= 0;
		}
		
		
		public void setPoints(Vector3[] pointArray,bool copyToInternalArray)
		{
			internalPointArray=pointArray;

			if(copyToInternalArray)
			{
				int length=pointArray.Length;
				internalPointArray=new Vector3[length];
				for(int i=0;i<length;i++)
					{internalPointArray[i]=pointArray[i];}
			}
			else
				{internalPointArray=pointArray;}
			
			numVertices=internalPointArray.Length;
			unsafe
			{
				pointStrideBytes=sizeof(Vector3);
				fixed(void* p=&internalPointArray[0])
					{pointsPtr=new IntPtr(p);}
			}
		}
		
	
		public void setTriangleIndices(int[] indiceTripletArray,bool copyToInternalArray)
		{
//crap
FlagSixteenBitIndices=false;
			internalTriangleArray=indiceTripletArray;
			
			if(copyToInternalArray)
			{
				int length=indiceTripletArray.Length;
				internalTriangleArray=new int[length];
				for(int i=0;i<length;i++)
					{internalTriangleArray[i]=indiceTripletArray[i];}
			}
			else
				{internalTriangleArray=indiceTripletArray;}
			
			numTriangles=internalTriangleArray.Length/3;
			unsafe
			{
				triangleStrideBytes=sizeof(int)*3;
				fixed(void* t=&internalTriangleArray[0])
					{trianglesPtr=new IntPtr(t);}
			}
		}






		virtual public Vector3[] getPoints()
		{
			Vector3[] pointArray=new Vector3[numVertices];
			
			unsafe
			{
				byte *b=(byte*)pointsPtr.ToPointer();
				for(int i=0;i<numVertices;i++)
				{
					float *p=(float*)&b[i*pointStrideBytes];
					pointArray[i].X=p[0];
					pointArray[i].Y=p[1];
					pointArray[i].Z=p[2];
				}
			}
			
			return pointArray;
		}
		
		virtual public int[] getTriangleIndices()
		{
			int[] indiceTripletArray=new int[numTriangles*3];
			
			unsafe
			{
				byte *b=(byte*)trianglesPtr.ToPointer();
				for(int i=0;i<numTriangles;i++)
				{
					if(FlagSixteenBitIndices)
					{
						ushort *t=(ushort*)&b[i*triangleStrideBytes];
						indiceTripletArray[(i*3)+0]=(int)t[0];
						indiceTripletArray[(i*3)+1]=(int)t[1];
						indiceTripletArray[(i*3)+2]=(int)t[2];
					}
					else
					{
						int *t=(int*)&b[i*triangleStrideBytes];
						indiceTripletArray[(i*3)+0]=t[0];
						indiceTripletArray[(i*3)+1]=t[1];
						indiceTripletArray[(i*3)+2]=t[2];
					}
				}
			}		
			
			return indiceTripletArray;	
		}
		
		virtual public Vector3[] getTrianglesAsVertexTriplets()
		{
			int[] indiceTripletArray=getTriangleIndices();
			Vector3[] pointArray=getPoints();
			Vector3[] triangleTripletArray=new Vector3[numTriangles*3];
			
			for(int i=0;i<numTriangles;i++)
			{
				int a=indiceTripletArray[(i*3)+0];
				int b=indiceTripletArray[(i*3)+1];
				int c=indiceTripletArray[(i*3)+2];
				
				triangleTripletArray[(i*3)+0]=pointArray[a];
				triangleTripletArray[(i*3)+1]=pointArray[b];
				triangleTripletArray[(i*3)+2]=pointArray[c];
			}
			
			return triangleTripletArray;
		}

	
		

		public bool FlagComputeConvex
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxConvexFlags.NX_CF_COMPUTE_CONVEX);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxConvexFlags.NX_CF_COMPUTE_CONVEX,value);}
		}
		
		public bool FlagFlipNormals
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxConvexFlags.NX_CF_FLIPNORMALS);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxConvexFlags.NX_CF_FLIPNORMALS,value);}
		}
		
		public bool FlagSixteenBitIndices
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxConvexFlags.NX_CF_16_BIT_INDICES);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxConvexFlags.NX_CF_16_BIT_INDICES,value);}
		}
	}
}






/*
class NxConvexMeshDesc
{
	NxU32 numVertices;
	NxU32 numTriangles;
	NxU32 pointStrideBytes;
	NxU32 triangleStrideBytes;
	const void* points;
	const void* triangles;
	NxU32 flags;
};
class NxConvexMeshDesc2
{
	NxU32 numVertices;			//!< Number of vertices.
	NxU32 numPolygons;			//!< Number of convex polygons.
	const void* points;
	const NxU32* polygons;		//First number is how many indices the polygon has, followed by that number of indices. This repeats numPolygons times.
	const bool* internalFlags;
};
*/
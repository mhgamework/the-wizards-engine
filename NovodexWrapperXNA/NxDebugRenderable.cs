//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


//POSSIBLE CHANGES
 //a memcpy would be much better than the ptrToStructure crap
 // actually pass in the managed pointer to the native code and just memcpy it
 //Keeping a static chunk of data of maxSize would be better than allocating memory every time
 // If a normal array is used Length won't be right. If ArrayList is used then the elements need casting


namespace NovodexWrapper
{

	public struct NxDebugPoint
	{
		public Vector3	p;
		public uint		color;
	}

	public struct NxDebugLine
	{
		public Vector3	p0;
		public Vector3	p1;
		public uint		color;
	}

	public struct NxDebugTriangle
	{
		public Vector3	p0;
		public Vector3	p1;
		public Vector3	p2;
		public uint		color;
	}




	public class NxDebugRenderable
	{
		private IntPtr nxDebugRenderablePtr=IntPtr.Zero;

		public NxDebugRenderable()
			{}
	
		public NxDebugRenderable(IntPtr debugRenderablePointer)
			{nxDebugRenderablePtr=debugRenderablePointer;}

		public static NxDebugRenderable createFromPointer(IntPtr debugRenderablePointer)
		{
			if(debugRenderablePointer==IntPtr.Zero)
				{return null;}
			return new NxDebugRenderable(debugRenderablePointer);
		}
	
	
		public IntPtr NxDebugRenderablePtr
		{
			get{return nxDebugRenderablePtr;}
			set{nxDebugRenderablePtr=value;}
		}
	
	
		public int getNbPoints()
			{return wrapper_DebugRenderable_getNbPoints(nxDebugRenderablePtr);}
	
		public int getNbLines()
			{return wrapper_DebugRenderable_getNbLines(nxDebugRenderablePtr);}
	
		public int getNbTriangles()
			{return wrapper_DebugRenderable_getNbTriangles(nxDebugRenderablePtr);}
		

		unsafe public NxDebugPoint[] getPoints()
		{
			int numPoints=getNbPoints();
			NxDebugPoint[] pointArray=new NxDebugPoint[numPoints];
			if(numPoints>0)
			{
				fixed(void* address=&pointArray[0])
					{wrapper_DebugRenderable_getPoints(nxDebugRenderablePtr,new IntPtr(address));}
			}
			return pointArray;
		}	

		unsafe public NxDebugLine[] getLines()
		{
			int numLines=getNbLines();
			NxDebugLine[] lineArray=new NxDebugLine[numLines];
			if(numLines>0)
			{
				fixed(void* address=&lineArray[0])
					{wrapper_DebugRenderable_getLines(nxDebugRenderablePtr,new IntPtr(address));}
			}
			return lineArray;
		}	
 
 		unsafe public NxDebugTriangle[] getTriangles()
		{
			int numTriangles=getNbTriangles();
			NxDebugTriangle[] triangleArray=new NxDebugTriangle[numTriangles];
			if(numTriangles>0)
			{
				fixed(void* address=&triangleArray[0])
					{wrapper_DebugRenderable_getTriangles(nxDebugRenderablePtr,new IntPtr(address));}
			}
			return triangleArray;
		}


	
		

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_DebugRenderable_getNbPoints(IntPtr debugRenderable);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_DebugRenderable_getNbLines(IntPtr debugRenderable);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern int wrapper_DebugRenderable_getNbTriangles(IntPtr debugRenderable);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_DebugRenderable_getPoints(IntPtr debugRenderable,IntPtr pointArray);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_DebugRenderable_getLines(IntPtr debugRenderable,IntPtr lineArray);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_DebugRenderable_getTriangles(IntPtr debugRenderable,IntPtr triangleArray);

 	}
}



/*
-	virtual NxU32 getNbPoints() const = 0;
-	virtual const NxDebugPoint* getPoints() const = 0;
-	virtual NxU32 getNbLines() const = 0;
-	virtual const NxDebugLine* getLines() const = 0;
-	virtual NxU32 getNbTriangles() const = 0;
-	virtual const NxDebugTriangle* getTriangles() const = 0;
*/







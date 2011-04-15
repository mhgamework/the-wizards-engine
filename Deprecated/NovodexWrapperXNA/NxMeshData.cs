//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


//crap
//Need to deal with the flags!!!!!!!!!
//NX_MDF_16_BIT_INDICES:			//Denotes the use of 16-bit vertex indices.
//NX_MDF_16_BIT_COMPRESSED_FLOATS:	//Specifies that all floats are written compressed to 16 bit.
//NX_MDF_INDEXED_MESH:				//Specifies that triangle indices are generated and adjacent triangles share common vertices. If this flag is not set, all triangles are described as vertex triplets in the vertex array.

//Pass in flags to allocateData()


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxMeshData
	{
		public IntPtr		verticesPosBeginPtr;
		public IntPtr		verticesNormalBeginPtr;
		public uint			verticesPosByteStride;
		public uint			verticesNormalByteStride;
		public uint			maxVertices;
		public IntPtr		numVerticesPtr;
		public IntPtr		indicesBeginPtr;
		public uint			indicesByteStride;
		public uint			maxIndices;
		public IntPtr		numTrianglesPtr;
		public IntPtr		parentIndicesBeginPtr;
		public uint			parentIndicesByteStride;
		public uint			maxParentIndices;
		public IntPtr		numParentIndicesPtr;
		public uint			flags;
		public IntPtr		namePtr;			//!< Possible debug name. The string is not copied by the SDK, only the pointer is stored.



		public static NxMeshData Default
			{get{return NxMeshData.createDefault();}}
		
		private static NxMeshData createDefault()
		{
			NxMeshData meshData=new NxMeshData();
			meshData.setToDefault();
			return meshData;
		}
		
		public void setToDefault()
		{
			verticesPosBeginPtr				= IntPtr.Zero;
			verticesNormalBeginPtr			= IntPtr.Zero;
			verticesPosByteStride			= 0;
			verticesNormalByteStride		= 0;
			maxVertices						= 0;
			numVerticesPtr					= IntPtr.Zero;
			indicesBeginPtr					= IntPtr.Zero;
			indicesByteStride				= 0;
			maxIndices						= 0;
			numTrianglesPtr					= IntPtr.Zero;
			parentIndicesBeginPtr			= IntPtr.Zero;
			parentIndicesByteStride			= 0;
			maxParentIndices				= 0;
			numParentIndicesPtr				= IntPtr.Zero;
			flags							= (uint)NxMeshDataFlags.NX_MDF_INDEXED_MESH;
			namePtr							= IntPtr.Zero;
		}

		//Usually you'll want to pass in (maxNumVerts,numTriangles*3,maxNumVerts)
		unsafe public void allocateData(int maxNumVertices,int maxNumIndices,int maxNumParentIndices)
		{
			freeData();

			maxVertices=(uint)maxNumVertices;
			maxIndices=(uint)maxNumIndices;
			maxParentIndices=(uint)maxNumParentIndices;

			numVerticesPtr=Marshal.AllocHGlobal((int)sizeof(int));
			numTrianglesPtr=Marshal.AllocHGlobal((int)sizeof(int));
			numParentIndicesPtr=Marshal.AllocHGlobal((int)sizeof(int));

			verticesPosByteStride=(uint)sizeof(Vector3);
			verticesNormalByteStride=(uint)sizeof(Vector3);
			indicesByteStride=(uint)sizeof(uint);
			parentIndicesByteStride=(uint)sizeof(uint);

			if(maxVertices>0)
			{
				verticesPosBeginPtr=Marshal.AllocHGlobal((int)(sizeof(Vector3)*maxVertices));
				verticesNormalBeginPtr=Marshal.AllocHGlobal((int)(sizeof(Vector3)*maxVertices));
			}
			if(maxIndices>0)
				{indicesBeginPtr=Marshal.AllocHGlobal((int)(sizeof(uint)*maxIndices));}
			if(maxParentIndices>0)
				{parentIndicesBeginPtr=Marshal.AllocHGlobal((int)(sizeof(uint)*maxNumParentIndices));}
			
			NumVertices=0;
			NumTriangles=0;
			NumParentIndices=0;
		}

		unsafe public void freeData()
		{
			maxVertices=0;
			maxIndices=0;
			maxParentIndices=0;
			Marshal.FreeHGlobal(numVerticesPtr);
			Marshal.FreeHGlobal(numTrianglesPtr);
			Marshal.FreeHGlobal(numParentIndicesPtr);
			Marshal.FreeHGlobal(verticesPosBeginPtr);
			Marshal.FreeHGlobal(verticesNormalBeginPtr);
			Marshal.FreeHGlobal(indicesBeginPtr);
			Marshal.FreeHGlobal(parentIndicesBeginPtr);

			numVerticesPtr=IntPtr.Zero;
			numTrianglesPtr=IntPtr.Zero;
			numParentIndicesPtr=IntPtr.Zero;
			verticesPosBeginPtr=IntPtr.Zero;
			verticesNormalBeginPtr=IntPtr.Zero;
			indicesBeginPtr=IntPtr.Zero;
			parentIndicesBeginPtr=IntPtr.Zero;
		}


		unsafe public Vector3[] getVertexPosArray()
		{
			int numVerts=NumVertices;
			Vector3[] pointArray=new Vector3[numVerts];

			byte *b=(byte*)verticesPosBeginPtr.ToPointer();
			for(int i=0;i<numVerts;i++)
			{
				float *p=(float*)&b[i*verticesPosByteStride];
				pointArray[i].X=p[0];
				pointArray[i].Y=p[1];
				pointArray[i].Z=p[2];
			}
			return pointArray;
		}
		
		unsafe public Vector3[] getVertexNormalArray()
		{
			int numVerts=NumVertices;
			Vector3[] pointArray=new Vector3[numVerts];

			byte *b=(byte*)verticesNormalBeginPtr.ToPointer();
			for(int i=0;i<numVerts;i++)
			{
				float *p=(float*)&b[i*verticesNormalByteStride];
				pointArray[i].X=p[0];
				pointArray[i].Y=p[1];
				pointArray[i].Z=p[2];
			}
			return pointArray;
		}

		unsafe public int[] getIndiceArray()
		{
			int numIndices=NumTriangles*3;
			int[] indiceArray=new int[numIndices];

			byte *b=(byte*)indicesBeginPtr.ToPointer();
			for(int i=0;i<numIndices;i++)
				{indiceArray[i]=*(int*)&b[i*indicesByteStride];}
			return indiceArray;
		}	

		unsafe public int[] getParentIndiceArray()
		{
			int numIndices=NumTriangles*3;
			int[] indiceArray=new int[numIndices];

			byte *b=(byte*)parentIndicesBeginPtr.ToPointer();
			for(int i=0;i<numIndices;i++)
				{indiceArray[i]=*(int*)&b[i*parentIndicesByteStride];}
			return indiceArray;
		}

		public Vector3[] getTrianglesAsVertexTriplets()
		{
			int numTris=NumTriangles;
			int[] indiceTripletArray=getIndiceArray();
			Vector3[] pointArray=getVertexPosArray();
			Vector3[] triangleTripletArray=new Vector3[numTris*3];
			
			for(int i=0;i<numTris;i++)
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





		unsafe public int NumVertices
		{
			get
			{
				if(numVerticesPtr==IntPtr.Zero)
					{return 0;}
				return *((int*)numVerticesPtr.ToPointer());
			}
			set
			{
				if(numVerticesPtr!=IntPtr.Zero)
					{*((int*)numVerticesPtr.ToPointer())=value;}
			}
		}

		unsafe public int NumTriangles
		{
			get
			{
				if(numTrianglesPtr==IntPtr.Zero)
					{return 0;}
				return *((int*)numTrianglesPtr.ToPointer());
			}
			set
			{
				if(numTrianglesPtr!=IntPtr.Zero)
					{*((int*)numTrianglesPtr.ToPointer())=value;}
			}
		}

		unsafe public int NumParentIndices
		{
			get
			{
				if(numParentIndicesPtr==IntPtr.Zero)
					{return 0;}
				return *((int*)numParentIndicesPtr.ToPointer());
			}
			set
			{
				if(numParentIndicesPtr!=IntPtr.Zero)
					{*((int*)numParentIndicesPtr.ToPointer())=value;}
			}
		}


		public bool FlagSixteenBitIndices
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxMeshDataFlags.NX_MDF_16_BIT_INDICES);}
//			set{flags=NovodexUtil.setBits(flags,(uint)NxMeshDataFlags.NX_MDF_16_BIT_INDICES,value);}
		}

		public bool FlagCompressedFloats
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxMeshDataFlags.NX_MDF_16_BIT_COMPRESSED_FLOATS);}
//			set{flags=NovodexUtil.setBits(flags,(uint)NxMeshDataFlags.NX_MDF_16_BIT_COMPRESSED_FLOATS,value);}
		}

		public bool FlagIndexedMesh
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxMeshDataFlags.NX_MDF_INDEXED_MESH);}
//			set{flags=NovodexUtil.setBits(flags,(uint)NxMeshDataFlags.NX_MDF_INDEXED_MESH,value);}
		}
	}
}


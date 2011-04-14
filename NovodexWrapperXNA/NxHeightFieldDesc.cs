//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


//The Novodex terminology for rows and columns is whacky.
//rows=x columns=z


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxHeightFieldDesc
	{
		public uint						nbRows;
		public uint						nbColumns;
		public NxHeightFieldFormat		format;
		public uint						sampleStride;
		public IntPtr					samplesPtr;
		public float					verticalExtent;
		public float					convexEdgeThreshold;
		public uint						flags;
		private NxHeightFieldSample[]	samplesArray=null;


		public static NxHeightFieldDesc Default
			{get{return new NxHeightFieldDesc();}}

		public NxHeightFieldDesc()
			{setToDefault();}

		public NxHeightFieldDesc(int numRows,int numColumns,float verticalExtent,float convexEdgeThreshold,uint flags)
		{
			setToDefault();
			this.verticalExtent=verticalExtent;
			this.convexEdgeThreshold=convexEdgeThreshold;
			this.flags=flags;
			setSampleDimensions(numRows,numColumns);
		}

		public void setToDefault()
		{
			nbColumns					= 0;
			nbRows						= 0;
			format						= NxHeightFieldFormat.NX_HF_S16_TM;
			sampleStride				= 0;
			samplesPtr					= IntPtr.Zero;
			verticalExtent				= 0;
			convexEdgeThreshold			= 0.0f;
			flags						= 0;
		}
		

		public void setSampleDimensions(int numRows,int numColumns)
		{
			this.nbRows=(uint)numRows;
			this.nbColumns=(uint)numColumns;
			
			samplesArray=new NxHeightFieldSample[numRows*numColumns];
			
			unsafe
			{
				fixed(void* x=&samplesArray[0])
					{samplesPtr=new IntPtr(x);}
				sampleStride=(uint)sizeof(NxHeightFieldSample);
			}
		}

		public void buildSamplesArrayFromSamplesPointer()
		{
			if(samplesPtr==IntPtr.Zero)
			{
				samplesArray=null;
				return;
			}

			int size=(int)(nbRows*nbColumns);
			samplesArray=new NxHeightFieldSample[size];

			unsafe
			{
				byte* b=(byte*)samplesPtr.ToPointer();
				for(int i=0;i<size;i++)
					{samplesArray[i]=*((NxHeightFieldSample*)&b[i*sampleStride]);}					
			}
		}
		
		public NxHeightFieldSample[] getSamplesArray()
			{return samplesArray;}
			
		public NxHeightFieldSample getSample(int row,int column)
			{return samplesArray[(row*nbColumns)+column];}

		public void setSample(int row,int column,NxHeightFieldSample sample)
			{samplesArray[(row*nbColumns)+column]=sample;}

		public bool FlagNoBoundaryEdges
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxHeightFieldFlags.NX_HF_NO_BOUNDARY_EDGES);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxHeightFieldFlags.NX_HF_NO_BOUNDARY_EDGES,value);}
		}		
	}
}







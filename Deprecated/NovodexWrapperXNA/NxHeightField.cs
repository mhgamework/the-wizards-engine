//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;


namespace NovodexWrapper
{
	public class NxHeightField
	{

		protected IntPtr nxHeightFieldPtr;

		public NxHeightField(IntPtr heightFieldPointer)
			{nxHeightFieldPtr=heightFieldPointer;}

		public static NxHeightField createFromPointer(IntPtr heightFieldPointer)
		{
			if(heightFieldPointer==IntPtr.Zero)
				{return null;}
			return new NxHeightField(heightFieldPointer);
		}

		virtual public void internalAfterRelease()
			{nxHeightFieldPtr=IntPtr.Zero;}

		public IntPtr NxHeightFieldPtr
			{get{return nxHeightFieldPtr;}}





		virtual public bool saveToDesc(NxHeightFieldDesc h)
			{return wrapper_HeightField_saveToDesc(nxHeightFieldPtr,ref h.nbRows,ref h.nbColumns,ref h.format,ref h.sampleStride,ref h.samplesPtr,ref h.verticalExtent,ref h.convexEdgeThreshold,ref h.flags);}

		virtual public NxHeightFieldDesc getHeightFieldDesc(bool buildSamplesArrayFlag)
		{
			NxHeightFieldDesc heightFieldDesc=NxHeightFieldDesc.Default;
			saveToDesc(heightFieldDesc);
			if(buildSamplesArrayFlag)
				{heightFieldDesc.buildSamplesArrayFromSamplesPointer();}
			return heightFieldDesc;
		}
		
		virtual public bool loadFromDesc(NxHeightFieldDesc heightFieldDesc)
			{return wrapper_HeightField_loadFromDesc(nxHeightFieldPtr,heightFieldDesc);}

		virtual public uint getNbRows()
			{return wrapper_HeightField_getNbRows(nxHeightFieldPtr);}

		virtual public uint getNbColumns()
			{return wrapper_HeightField_getNbColumns(nxHeightFieldPtr);}

		virtual public NxHeightFieldFormat getFormat()
			{return wrapper_HeightField_getFormat(nxHeightFieldPtr);}

		virtual public uint getSampleStride()
			{return wrapper_HeightField_getSampleStride(nxHeightFieldPtr);}

		virtual public float getVerticalExtent()
			{return wrapper_HeightField_getVerticalExtent(nxHeightFieldPtr);}

		virtual public float getConvexEdgeThreshold()
			{return wrapper_HeightField_getConvexEdgeThreshold(nxHeightFieldPtr);}

		virtual public uint getFlags()
			{return wrapper_HeightField_getFlags(nxHeightFieldPtr);}

		virtual public float getHeight(float x,float z)
			{return wrapper_HeightField_getHeight(nxHeightFieldPtr,x,z);}

		virtual public NxHeightFieldSample[] getCells()
			{return getHeightFieldDesc(true).getSamplesArray();}

		//Novodex returns the number of bytes written to the destination buffer. This returns the number of samples copied to destSampleArray.
		virtual public int saveCells(NxHeightFieldSample[] destSampleArray)
		{
			NxHeightFieldSample[] cellArray=getCells();
			
			int num=Math.Min(cellArray.Length,destSampleArray.Length);
			for(int i=0;i<num;i++)
				{destSampleArray[i]=cellArray[i];}
			
			return num;
		}

		//There are some whacky triangles somewhere. A 16x16 triangle grid does not have the expected 16*16*2=512 triangles. There are extra based upon the formula below.
		virtual public int getNumTriangles()
			{return (int)(( (getNbColumns()) * (getNbRows()-1) - 1) * 2);}




		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_HeightField_saveToDesc(IntPtr heightField,ref uint nbRows,ref uint nbColumns,ref NxHeightFieldFormat format,ref uint sampleStride,ref IntPtr samplesPtr,ref float verticalExtent,ref float convexEdgeThreshold,ref uint flags);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern bool wrapper_HeightField_loadFromDesc(IntPtr heightField,NxHeightFieldDesc heightFieldDesc);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_HeightField_getNbRows(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_HeightField_getNbColumns(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern NxHeightFieldFormat wrapper_HeightField_getFormat(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_HeightField_getSampleStride(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightField_getVerticalExtent(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightField_getConvexEdgeThreshold(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern uint wrapper_HeightField_getFlags(IntPtr heightField);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern float wrapper_HeightField_getHeight(IntPtr heightField,float x,float z);
	}
}


/*
-	virtual		bool						saveToDesc(NxHeightFieldDesc& desc)	const	= 0;
-	virtual		bool						loadFromDesc(const NxHeightFieldDesc& desc)			= 0;
-	virtual		NxU32						getNbRows()					const = 0;
-	virtual		NxU32						getNbColumns()				const = 0;
-	virtual		NxHeightFieldFormat			getFormat()					const = 0;
-	virtual		NxU32						getSampleStride()			const = 0;
-	virtual		NxReal						getVerticalExtent()			const = 0;
-	virtual		NxReal						getConvexEdgeThreshold()	const = 0;
-	virtual		NxU32						getFlags()					const = 0;
-	virtual		NxReal						getHeight(NxReal x, NxReal z) const = 0;
-   virtual		NxU32						saveCells(void * destBuffer, NxU32 destBufferSize) const = 0;
-	virtual		const void*					getCells()					const = 0;
*/

//getCells() and saveCells() are pretty much identical. Both are for GETTING the height data from NxHeightField, neither is for setting the height data.
//getCells() Returns a read only pointer directly to the samples array. The data format is identical to that in NxHeightFieldDesc.samples.
//saveCells() copies the data to your own buffer



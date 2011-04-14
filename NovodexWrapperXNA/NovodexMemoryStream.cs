//By Jason Zelsnack, All rights reserved

using System;


//Only readBuffer() and storeBuffer() need to be implemented
// All the other methods are implemented in NxStream by calling readBuffer() and storeBuffer()
//If you want to reimplement those you're free to do that


namespace NovodexWrapper
{
	public class NovodexMemoryStream : NxStream
	{
		private uint indexPos;
		private uint currentSize;
		private byte[] data;

		public NovodexMemoryStream()
		{
			indexPos=0;
			currentSize=0;
			data=new byte[256];
		}

		public NovodexMemoryStream(byte[] sourceData,uint dataSize)
			{setData(sourceData,dataSize);}

		public void clear()
		{
			indexPos=0;
			currentSize=0;
			data=null;
		}
		
		public void ensureCapacity(uint minCapacity)
		{
			if(minCapacity>data.Length)
			{
				byte[] newData=new byte[minCapacity];
				for(int i=0;i<currentSize;i++)
					{newData[i]=data[i];}
				data=newData;
			}
		}
		
		public void setData(byte[] sourceData,uint dataSize)
		{
			indexPos=0;
			currentSize=dataSize;
			data=new byte[dataSize];
			for(int i=0;i<dataSize;i++)
				{data[i]=sourceData[i];}			
		}
		
		public uint IndexPos
		{
			get{return indexPos;}
			set{indexPos=value;}
		}

		public uint CurrentSize
			{get{return currentSize;}}
			
		public byte[] Data
			{get{return data;}}


		unsafe public override void readBuffer(IntPtr buffer,uint size)
		{
			byte* b=(byte*)buffer.ToPointer();
			for(int i=0;i<size;i++)
				{b[i]=data[indexPos++];}
		}

		unsafe public override void storeBuffer(IntPtr buffer,uint size)
		{
			uint newSize=currentSize+size;
			if(newSize>=data.Length)
				{ensureCapacity(newSize*2);}	//double the capacity
		
			byte* b=(byte*)buffer.ToPointer();
			for(int i=0;i<size;i++)
				{data[currentSize++]=b[i];}
		}
	}
}

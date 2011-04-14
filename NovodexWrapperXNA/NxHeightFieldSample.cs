//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxHeightFieldSample
	{
		private uint data;
		//NxI16	height			: 16;
		//NxU8	materialIndex0	: 7;
		//NxU8	tessFlag		: 1;	//Determines which way the triangles in a quad are tessellated
		//NxU8	materialIndex1	: 7;
		//NxU8	unused			: 1;

		public NxHeightFieldSample(short height,byte materialIndex0,bool tessFlag,byte materialIndex1)
		{
			data=0;
			Height=height;
			MaterialIndex0=materialIndex0;
			TessFlag=tessFlag;
			MaterialIndex1=materialIndex1;
//			Unused=unused;
		}

		unsafe public short Height
		{
			get
			{
				fixed(void* x=&data)
					{return *(short*)x;}
			}
			set
			{
				fixed(void* x=&data)
					{*(short*)x=value;}
			}
		}

		unsafe public byte MaterialIndex0
		{
			get
			{
				fixed(void* x=&data)
					{return (byte)(((byte*)x)[2]&0x7F);}			//Return only the first 7-bits
			}
			set
			{
				fixed(void* x=&data)
				{
					byte b=(byte)(((byte*)x)[2]&0x80);				//Remove all bits but the first
						((byte*)x)[2]=(byte)(b|(byte)(value&0x7F));	//OR in the new 7-bits
				}
			}
		}

		unsafe public bool TessFlag
		{
			get
			{
				fixed(void* x=&data)
					{return (((byte*)x)[2]&0x80)!=0;}	//Only return the last bit
			}
			set
			{
				fixed(void* x=&data)
				{
					if(value)
						{((byte*)x)[2]|=0x80;}			//Set the last bit to 1
					else
						{((byte*)x)[2]&=0x7F;}			//Set the last bit to 0
				}
			}

		}

		unsafe public byte MaterialIndex1
		{
			get
			{
				fixed(void* x=&data)
					{return (byte)(((byte*)x)[3]&0x7F);}		//Return only the first 7-bits
			}
			set
			{
				fixed(void* x=&data)
				{
					byte b=(byte)(((byte*)x)[3]&0x80);			//Remove all bits but the first
					((byte*)x)[3]=(byte)(b|(byte)(value&0x7F));	//OR in the new 7-bits
				}
			}
		}

		unsafe public bool Unused
		{
			get
			{
				fixed(void* x=&data)
					{return (((byte*)x)[3]&0x80)!=0;}	//Only return the last bit
			}
			set
			{
				fixed(void* x=&data)
				{
					if(value)
						{((byte*)x)[3]|=0x80;}			//Set the last bit to 1
					else
						{((byte*)x)[3]&=0x7F;}			//Set the last bit to 0
				}
			}
		}
	}
}




//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

//crap
//Normally the user doesn't have to know anything the native memory if NxFluid automatically
// allocates and frees the native memory when it is created and released.
//I guess that NxFluid.setParticlesWriteData() and NxFluid.getParticlesWriteData()
// can be trouble makers.




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxParticleData
	{
		public uint		maxParticles;
		public IntPtr	numParticlesPtr;		//uint*
		public IntPtr	bufferPosPtr;			//float[3*maxParticles] //Set to null if you don't want these read or written
		public IntPtr	bufferVelPtr;			//float[3*maxParticles] //Set to null if you don't want these read or written
		public IntPtr	bufferLifePtr;			//float[1*maxParticles] //Set to null if you don't want these read or written
		public IntPtr	bufferDensityPtr;		//float[1*maxParticles] //Set to null if you don't want these read or written
		public IntPtr	bufferFlagsPtr;			//uint[1*maxParticles]  //Set to null if you don't want these read or written
		public uint		bufferPosByteStride;
		public uint		bufferVelByteStride;
		public uint		bufferLifeByteStride;
		public uint		bufferDensityByteStride;
		public uint		bufferFlagsByteStride;
		public IntPtr	namePtr;			//!< Possible debug name. The string is not copied by the SDK, only the pointer is stored.


		public static NxParticleData Default
			{get{return NxParticleData.createDefault();}}
		
		private static NxParticleData createDefault()
		{
			NxParticleData particleData=new NxParticleData();
			particleData.setToDefault();
			return particleData;
		}

		public void setToDefault()
		{
			maxParticles			= 0;
			numParticlesPtr			= IntPtr.Zero;
			bufferPosPtr			= IntPtr.Zero;
			bufferVelPtr			= IntPtr.Zero;
			bufferLifePtr			= IntPtr.Zero;
			bufferDensityPtr		= IntPtr.Zero;
			bufferFlagsPtr			= IntPtr.Zero;
			bufferPosByteStride		= 0;
			bufferVelByteStride		= 0;
			bufferLifeByteStride	= 0;
			bufferDensityByteStride = 0;
			bufferFlagsByteStride	= 0;
			namePtr					= IntPtr.Zero;
		}

		unsafe public void allocateData(int maxNumParticles,bool usePosition,bool useVelocity,bool useLifetime,bool useDensity,bool useFlags)
		{
			freeData();

			maxParticles=(uint)maxNumParticles;

			if(usePosition)
			{
				bufferPosByteStride=(uint)(sizeof(float)*3);
				bufferPosPtr=Marshal.AllocHGlobal((int)(bufferPosByteStride*maxParticles));
			}

			if(useVelocity)
			{
				bufferVelByteStride=(uint)(sizeof(float)*3);
				bufferVelPtr=Marshal.AllocHGlobal((int)(bufferVelByteStride*maxParticles));
			}

			if(useLifetime)
			{
				bufferLifeByteStride=(uint)(sizeof(float)*1);
				bufferLifePtr=Marshal.AllocHGlobal((int)(bufferLifeByteStride*maxParticles));
			}
			
			if(useDensity)
			{
				bufferDensityByteStride=(uint)(sizeof(float)*1);
				bufferDensityPtr=Marshal.AllocHGlobal((int)(bufferDensityByteStride*maxParticles));
			}
			
			if(useFlags)
			{
				bufferFlagsByteStride=(uint)(sizeof(uint)*1);
				bufferFlagsPtr=Marshal.AllocHGlobal((int)(bufferFlagsByteStride*maxParticles));
			}

			numParticlesPtr=Marshal.AllocHGlobal((int)sizeof(uint));
			NumParticles=0;
		}


		unsafe public void freeData()
		{
			Marshal.FreeHGlobal(numParticlesPtr);
			Marshal.FreeHGlobal(bufferPosPtr);
			Marshal.FreeHGlobal(bufferVelPtr);
			Marshal.FreeHGlobal(bufferLifePtr);
			Marshal.FreeHGlobal(bufferDensityPtr);
			Marshal.FreeHGlobal(bufferFlagsPtr);			

			maxParticles=0;
			numParticlesPtr=IntPtr.Zero;
			bufferPosPtr=IntPtr.Zero;
			bufferVelPtr=IntPtr.Zero;
			bufferLifePtr=IntPtr.Zero;
			bufferDensityPtr=IntPtr.Zero;
			bufferFlagsPtr=IntPtr.Zero;
		}








		unsafe public Vector3 getPosition(int index)
		{
			if(index<0 || index>=maxParticles || bufferPosPtr==IntPtr.Zero)
				{return new Vector3(0,0,0);}

			byte *b=(byte*)bufferPosPtr.ToPointer();
			float *p=(float*)&b[index*bufferPosByteStride];
			return new Vector3(p[0],p[1],p[2]);
		}

		unsafe public void setPosition(int index,Vector3 pos)
		{
			if(index>=0 && index<maxParticles && bufferPosPtr!=IntPtr.Zero)
			{
				byte *b=(byte*)bufferPosPtr.ToPointer();
				float *p=(float*)&b[index*bufferPosByteStride];
				p[0]=pos.X;
				p[1]=pos.Y;
				p[2]=pos.Z;
			}
		}

		unsafe public Vector3 getVelocity(int index)
		{
			if(index<0 || index>=maxParticles || bufferVelPtr==IntPtr.Zero)
				{return new Vector3(0,0,0);}

			byte *b=(byte*)bufferVelPtr.ToPointer();
			float *p=(float*)&b[index*bufferVelByteStride];
			return new Vector3(p[0],p[1],p[2]);
		}

		unsafe public void setVelocity(int index,Vector3 vel)
		{
			if(index>=0 && index<maxParticles && bufferVelPtr!=IntPtr.Zero)
			{
				byte *b=(byte*)bufferVelPtr.ToPointer();
				float *p=(float*)&b[index*bufferVelByteStride];
				p[0]=vel.X;
				p[1]=vel.Y;
				p[2]=vel.Z;
			}
		}

		unsafe public float getLifetime(int index)
		{
			if(index<0 || index>=maxParticles || bufferLifePtr==IntPtr.Zero)
				{return 0;}

			byte *b=(byte*)bufferLifePtr.ToPointer();
			return *(float*)&b[index*bufferLifeByteStride];
		}

		unsafe public void setLifetime(int index,float lifetime)
		{
			if(index>=0 && index<maxParticles && bufferLifePtr!=IntPtr.Zero)
			{
				byte *b=(byte*)bufferLifePtr.ToPointer();
				*(float*)&b[index*bufferLifeByteStride]=lifetime;
			}
		}

		unsafe public float getDensity(int index)
		{
			if(index<0 || index>=maxParticles || bufferDensityPtr==IntPtr.Zero)
				{return 0;}

			byte *b=(byte*)bufferDensityPtr.ToPointer();
			return *(float*)&b[index*bufferDensityByteStride];
		}

		unsafe public void setDensity(int index,float density)
		{
			if(index>=0 && index<maxParticles && bufferDensityPtr!=IntPtr.Zero)
			{
				byte *b=(byte*)bufferDensityPtr.ToPointer();
				*(float*)&b[index*bufferDensityByteStride]=density;
			}
		}

		unsafe public uint getFlags(int index)
		{
			if(index<0 || index>=maxParticles || bufferFlagsPtr==IntPtr.Zero)
				{return 0;}

			byte *b=(byte*)bufferFlagsPtr.ToPointer();
			return *(uint*)&b[index*bufferFlagsByteStride];
		}

		unsafe public void setFlags(int index,uint flags)
		{
			if(index>=0 && index<maxParticles && bufferFlagsPtr!=IntPtr.Zero)
			{
				byte *b=(byte*)bufferFlagsPtr.ToPointer();
				*(uint*)&b[index*bufferFlagsByteStride]=flags;
			}
		}










		unsafe public Vector3[] getPositionArray()
		{
			int size=(int)NumParticles;
			Vector3[] positionArray=new Vector3[size];

			byte *b=(byte*)bufferPosPtr.ToPointer();
			for(int i=0;i<size;i++)
			{
				float *p=(float*)&b[i*bufferPosByteStride];
				positionArray[i]=new Vector3(p[0],p[1],p[2]);
			}
			return positionArray;
		}	

		unsafe public Vector3[] getVelocityArray()
		{
			int size=(int)NumParticles;
			Vector3[] velocityArray=new Vector3[size];

			byte *b=(byte*)bufferVelPtr.ToPointer();
			for(int i=0;i<size;i++)
			{
				float *p=(float*)&b[i*bufferVelByteStride];
				velocityArray[i]=new Vector3(p[0],p[1],p[2]);
			}
			return velocityArray;
		}	

		unsafe public float[] getLifetimeArray()
		{
			int size=(int)NumParticles;
			float[] lifetimeArray=new float[size];

			byte *b=(byte*)bufferLifePtr.ToPointer();
			for(int i=0;i<size;i++)
				{lifetimeArray[i]=*(float*)&b[i*bufferLifeByteStride];}
			return lifetimeArray;
		}	

		unsafe public float[] getDensityArray()
		{
			int size=(int)NumParticles;
			float[] densityArray=new float[size];

			byte *b=(byte*)bufferDensityPtr.ToPointer();
			for(int i=0;i<size;i++)
				{densityArray[i]=*(float*)&b[i*bufferDensityByteStride];}
			return densityArray;
		}	

		unsafe public uint[] getFlagsArray()
		{
			int size=(int)NumParticles;
			uint[] flagsArray=new uint[size];

			byte *b=(byte*)bufferFlagsPtr.ToPointer();
			for(int i=0;i<size;i++)
				{flagsArray[i]=*(uint*)&b[i*bufferFlagsByteStride];}
			return flagsArray;
		}	







		unsafe public uint NumParticles
		{
			get
			{
				if(numParticlesPtr==IntPtr.Zero)
					{return 0;}
				return *((uint*)numParticlesPtr.ToPointer());
			}
			set
			{
				if(numParticlesPtr!=IntPtr.Zero)
					{*((uint*)numParticlesPtr.ToPointer())=value;}
			}
		}
	}
}


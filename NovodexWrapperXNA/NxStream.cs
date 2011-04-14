//By Jason Zelsnack, All rights reserved

using System;
using System.Collections;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	public delegate byte	ReadByteDelegate();
	public delegate ushort	ReadWordDelegate();
	public delegate uint	ReadDwordDelegate();
	public delegate float	ReadFloatDelegate();
	public delegate double	ReadDoubleDelegate();
	public delegate void	ReadBufferDelegate(IntPtr buffer,uint size);
	public delegate void	StoreByteDelegate(byte b);
	public delegate void	StoreWordDelegate(ushort w);
	public delegate void	StoreDwordDelegate(uint d);
	public delegate void	StoreFloatDelegate(float f);
	public delegate void	StoreDoubleDelegate(double f);
	public delegate void	StoreBufferDelegate(IntPtr buffer,uint size);
	

	abstract unsafe public class NxStream
	{
		private IntPtr nxStreamPtr=IntPtr.Zero;

		private ReadByteDelegate	readByteDelegate=null;
		private ReadWordDelegate	readWordDelegate=null;
		private ReadDwordDelegate	readDwordDelegate=null;
		private ReadFloatDelegate	readFloatDelegate=null;
		private ReadDoubleDelegate	readDoubleDelegate=null;
		private ReadBufferDelegate	readBufferDelegate=null;
		private StoreByteDelegate	storeByteDelegate=null;
		private StoreWordDelegate	storeWordDelegate=null;
		private StoreDwordDelegate	storeDwordDelegate=null;
		private StoreFloatDelegate	storeFloatDelegate=null;
		private StoreDoubleDelegate	storeDoubleDelegate=null;
		private StoreBufferDelegate	storeBufferDelegate=null;



		public NxStream()
			{create();}

		~NxStream()
			{destroy();}


		public IntPtr NxStreamPtr
			{get{return nxStreamPtr;}}

		private void create()
		{
			setCallbacks();
			nxStreamPtr=wrapper_Stream_create(readByteDelegate,readWordDelegate,readDwordDelegate,readFloatDelegate,readDoubleDelegate,readBufferDelegate,storeByteDelegate,storeWordDelegate,storeDwordDelegate,storeFloatDelegate,storeDoubleDelegate,storeBufferDelegate);
		}

		private void destroy()
		{
			wrapper_Stream_destroy(nxStreamPtr);
			nxStreamPtr=IntPtr.Zero;
		}

		private void setCallbacks()
		{
			readByteDelegate=new ReadByteDelegate(this.readByte);
			readWordDelegate=new ReadWordDelegate(this.readWord);
			readDwordDelegate=new ReadDwordDelegate(this.readDword);
			readFloatDelegate=new ReadFloatDelegate(this.readFloat);
			readDoubleDelegate=new ReadDoubleDelegate(this.readDouble);
			readBufferDelegate=new ReadBufferDelegate(this.readBuffer);

			storeByteDelegate=new StoreByteDelegate(this.storeByte);
			storeWordDelegate=new StoreWordDelegate(this.storeWord);
			storeDwordDelegate=new StoreDwordDelegate(this.storeDword);
			storeFloatDelegate=new StoreFloatDelegate(this.storeFloat);
			storeDoubleDelegate=new StoreDoubleDelegate(this.storeDouble);
			storeBufferDelegate=new StoreBufferDelegate(this.storeBuffer);
		}

		public abstract void readBuffer(IntPtr buffer,uint size);
		public abstract void storeBuffer(IntPtr buffer,uint size);

		public virtual byte		readByte()				{byte b;	readBuffer(new IntPtr(&b),1); return b;}
		public virtual ushort	readWord()				{ushort w;	readBuffer(new IntPtr(&w),2); return w;}
		public virtual uint		readDword()				{uint d;	readBuffer(new IntPtr(&d),4); return d;}
		public virtual float	readFloat()				{float f;	readBuffer(new IntPtr(&f),4); return f;}
		public virtual double	readDouble()			{double f;	readBuffer(new IntPtr(&f),8); return f;}

		public virtual void		storeByte(byte b)		{storeBuffer(new IntPtr(&b),1);}
		public virtual void		storeByte(sbyte b)		{storeBuffer(new IntPtr(&b),1);}
		public virtual void		storeWord(ushort w)		{storeBuffer(new IntPtr(&w),2);}
		public virtual void		storeWord(short w)		{storeBuffer(new IntPtr(&w),2);}
		public virtual void		storeDword(uint d)		{storeBuffer(new IntPtr(&d),4);}
		public virtual void		storeDword(int d)		{storeBuffer(new IntPtr(&d),4);}
		public virtual void		storeFloat(float f)		{storeBuffer(new IntPtr(&f),4);}
		public virtual void		storeDouble(double f)	{storeBuffer(new IntPtr(&f),8);}

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern IntPtr wrapper_Stream_create(ReadByteDelegate readByteDelegate,ReadWordDelegate readWordDelegate,ReadDwordDelegate readDwordDelegate,ReadFloatDelegate readFloatDelegate,ReadDoubleDelegate readDoubleDelegate,ReadBufferDelegate readBufferDelegate,StoreByteDelegate storeByteDelegate,StoreWordDelegate storeWordDelegate,StoreDwordDelegate storeDwordDelegate,StoreFloatDelegate storeFloatDelegate,StoreDoubleDelegate storeDoubleDelegate,StoreBufferDelegate storeBufferDelegate);

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_Stream_destroy(IntPtr streamPtr);
	}
}


#region Old create() and destroy() methods
//I don't know why I ever kept a list of the streams.
//This caused the memory streams to never be garbage collected which led to memory usage continually increasing.
/*
		static ArrayList streamList=new ArrayList();

		private void create()
		{
			if(!streamList.Contains(this))
			{
				setCallbacks();
				nxStreamPtr=wrapper_Stream_create(readByteDelegate,readWordDelegate,readDwordDelegate,readFloatDelegate,readDoubleDelegate,readBufferDelegate,storeByteDelegate,storeWordDelegate,storeDwordDelegate,storeFloatDelegate,storeDoubleDelegate,storeBufferDelegate);
				streamList.Add(this);
			}
		}
		
		private void destroy()
		{
			if(streamList.Contains(this))
			{
				streamList.Remove(this);
				wrapper_Stream_destroy(nxStreamPtr);
			}
			nxStreamPtr=IntPtr.Zero;
		}
*/
#endregion



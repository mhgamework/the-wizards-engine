//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxMat33
	{
		static bool staticTransposedFlag=false;

		public static bool StaticTransposedFlag
		{
			set{staticTransposedFlag=value;}
			get{return staticTransposedFlag;}
		}


		public float M11,M21,M31,M12,M22,M32,M13,M23,M33;
		

		public NxMat33(NxMatrixType type)
		{
			if(type==NxMatrixType.NX_IDENTITY_MATRIX)
				{this=Identity;}
			else
				{M11=M21=M31=M12=M22=M32=M13=M23=M33=0;}
		}

		public NxMat33(ref NxMat33 mat)
			{this=mat;}

		public NxMat33(Vector3 row0,Vector3 row1,Vector3 row2)
		{
			M11 = row0.X;  M12 = row0.Y;  M13 = row0.Z;
			M21 = row1.X;  M22 = row1.Y;  M23 = row1.Z;
			M31 = row2.X;  M32 = row2.Y;  M33 = row2.Z;
		}

		public NxMat33(NxQuat q)
		{
			M11=M21=M31=M12=M22=M32=M13=M23=M33=0;
			fromQuat(q);
		}


		public void zero()
			{M11=M21=M31=M12=M22=M32=M13=M23=M33=0;}

		public void id()
		{
			M11=1; M21=0; M31=0;
			M12=0; M22=1; M32=0;
			M13=0; M23=0; M33=1;
		}


		public bool isIdentity()
		{
			if(M11!=1){return false;}
			if(M12!=0){return false;}
			if(M13!=0){return false;}

			if(M21!=0){return false;}
			if(M22!=1){return false;}
			if(M23!=0){return false;}

			if(M31!=0){return false;}
			if(M32!=0){return false;}
			if(M33!=1){return false;}
			
			return true;
		}

		public bool isZero()
		{
			if(M11!=0){return false;}
			if(M12!=0){return false;}
			if(M13!=0){return false;}

			if(M21!=0){return false;}
			if(M22!=0){return false;}
			if(M23!=0){return false;}

			if(M31!=0){return false;}
			if(M32!=0){return false;}
			if(M33!=0){return false;}
			
			return true;
		}

		public bool isFinite()
		{
			if(float.IsInfinity(M11) || float.IsNaN(M11)){return false;}		
			if(float.IsInfinity(M12) || float.IsNaN(M12)){return false;}		
			if(float.IsInfinity(M13) || float.IsNaN(M13)){return false;}		

			if(float.IsInfinity(M21) || float.IsNaN(M21)){return false;}		
			if(float.IsInfinity(M22) || float.IsNaN(M22)){return false;}		
			if(float.IsInfinity(M23) || float.IsNaN(M23)){return false;}		

			if(float.IsInfinity(M31) || float.IsNaN(M31)){return false;}		
			if(float.IsInfinity(M32) || float.IsNaN(M32)){return false;}		
			if(float.IsInfinity(M33) || float.IsNaN(M33)){return false;}
			
			return true;
		}


		public void setNegative()
		{
			M11 = -M11;
			M12 = -M12;
			M13 = -M13;

			M21 = -M21;
			M22 = -M22;
			M23 = -M23;

			M31 = -M31;
			M32 = -M32;
			M33 = -M33;
		}

		public void setTransposed(ref NxMat33 other)
		{
			float tx, ty, tz;
			tx = M21;	M21 = other.M12;	M12 = tx;
			ty = M31;	M31 = other.M13;	M13 = ty;
			tz = M32;	M32 = other.M23;	M23 = tz;
		}

		public void setTransposed()
			{setTransposed(ref this);}

		public float determinant()
			{return  (M11*M22*M33) + (M12*M23*M31) + (M13*M21*M32) - (M13*M22*M31) - (M12*M21*M33) - (M11*M23*M32);}

		public void diagonal(Vector3 v)
		{
			M11 = v.X;
			M12 = 0;
			M13 = 0;

			M21 = 0;
			M22 = v.Y;
			M23 = 0;

			M31 = 0;
			M32 = 0;
			M33 = v.Z;
		}

		public void star(Vector3 v)
		{
			M11 = 0;	M12 =-v.Z;	M13 = v.Y;
			M21 = v.Z;	M22 = 0;	M23 =-v.X;
			M31 =-v.Y;	M32 = v.X;	M33 = 0;
		}

		public void fromQuat(NxQuat q)
		{
			float w = q.w;
			float x = q.x;
			float y = q.y;
			float z = q.z;

			M11 = 1.0f		- y*y*2.0f	- z*z*2.0f;
			M12 = x*y*2.0f	- w*z*2.0f;	
			M13 = x*z*2.0f	+ w*y*2.0f;	

			M21 = x*y*2.0f	+ w*z*2.0f;	
			M22 = 1.0f		- x*x*2.0f	- z*z*2.0f;	
			M23 = y*z*2.0f	- w*x*2.0f;	
			
			M31 = x*z*2.0f	- w*y*2.0f;	
			M32 = y*z*2.0f	+ w*x*2.0f;	
			M33 = 1.0f		- x*x*2.0f	- y*y*2.0f;	
		}


		public void toQuat(ref NxQuat q)
		{
			float tr, s;
			tr = M11 + M22 + M33;
			if(tr >= 0)
			{
				s = (float)Math.Sqrt(tr +1);
				q.w = 0.5f * s;
				s = 0.5f / s;
				q.x = (getElement(2,1) - getElement(1,2)) * s;
				q.y = (getElement(0,2) - getElement(2,0)) * s;
				q.z = (getElement(1,0) - getElement(0,1)) * s;
			}
			else
			{
				int i = 0; 
				if (M22 > M11)
				i = 1; 
				if(M33 > getElement(i,i))
				i=2; 
				switch(i)
				{
					case 0:
						s = (float)Math.Sqrt((M11 - (M22 + M33)) + 1);
						q.x = 0.5f * s;
						s = 0.5f / s;
						q.y = (getElement(0,1) + getElement(1,0)) * s; 
						q.z = (getElement(2,0) + getElement(0,2)) * s;
						q.w = (getElement(2,1) - getElement(1,2)) * s;
						break;
					case 1:
						s = (float)Math.Sqrt((M22 - (M33 + M11)) + 1);
						q.y = 0.5f * s;
						s = 0.5f / s;
						q.z = (getElement(1,2) + getElement(2,1)) * s;
						q.x = (getElement(0,1) + getElement(1,0)) * s;
						q.w = (getElement(0,2) - getElement(2,0)) * s;
						break;
					case 2:
						s = (float)Math.Sqrt((M33 - (M11 + M22)) + 1);
						q.z = 0.5f * s;
						s = 0.5f / s;
						q.x = (getElement(2,0) + getElement(0,2)) * s;
						q.y = (getElement(1,2) + getElement(2,1)) * s;
						q.w = (getElement(1,0) - getElement(0,1)) * s;
						break;
				}
			}
		}


		public void multiply(Vector3 src,out Vector3 dst)
		{
			float x,y,z;	//so it works if src == dst
			x = (M11*src.X) + (M12*src.Y) + (M13*src.Z);
			y = (M21*src.X) + (M22*src.Y) + (M23*src.Z);
			z = (M31*src.X) + (M32*src.Y) + (M33*src.Z);

			dst.X = x;
			dst.Y = y;
			dst.Z = z;	
		}

		public void multiplyByTranspose(Vector3 src,out Vector3 dst)
		{
			float x,y,z;	//so it works if src == dst
			x = (M11*src.X) + (M21*src.Y) + (M31*src.Z);
			y = (M12*src.X) + (M22*src.Y) + (M32*src.Z);
			z = (M13*src.X) + (M23*src.Y) + (M33*src.Z);

			dst.X = x;
			dst.Y = y;
			dst.Z = z;	
		}

		public void multiply(ref NxMat33 left,ref NxMat33 right)
		{
			float a,b,c, d,e,f, g,h,i;
			//note: temps needed so that x.multiply(x,y) works OK.
			a =left.M11 * right.M11 +left.M12 * right.M21 +left.M13 * right.M31;
			b =left.M11 * right.M12 +left.M12 * right.M22 +left.M13 * right.M32;
			c =left.M11 * right.M13 +left.M12 * right.M23 +left.M13 * right.M33;

			d =left.M21 * right.M11 +left.M22 * right.M21 +left.M23 * right.M31;
			e =left.M21 * right.M12 +left.M22 * right.M22 +left.M23 * right.M32;
			f =left.M21 * right.M13 +left.M22 * right.M23 +left.M23 * right.M33;

			g =left.M31 * right.M11 +left.M32 * right.M21 +left.M33 * right.M31;
			h =left.M31 * right.M12 +left.M32 * right.M22 +left.M33 * right.M32;
			i =left.M31 * right.M13 +left.M32 * right.M23 +left.M33 * right.M33;


			M11 = a;
			M12 = b;
			M13 = c;

			M21 = d;
			M22 = e;
			M23 = f;

			M31 = g;
			M32 = h;
			M33 = i;
		}

		public void multiply(float d,ref NxMat33 a)
		{
			M11 = a.M11 * d;
			M12 = a.M12 * d;
			M13 = a.M13 * d;

			M21 = a.M21 * d;
			M22 = a.M22 * d;
			M23 = a.M23 * d;

			M31 = a.M31 * d;
			M32 = a.M32 * d;
			M33 = a.M33 * d;
		}

		public void multiplyDiagonal(Vector3 d)
		{
			M11 *= d.X;
			M12 *= d.Y;
			M13 *= d.Z;

			M21 *= d.X;
			M22 *= d.Y;
			M23 *= d.Z;

			M31 *= d.X;
			M32 *= d.Y;
			M33 *= d.Z;
		}

		public void multiplyDiagonalTranspose(Vector3 d)
		{
			M11 *= d.X;
			M12 *= d.X;
			M13 *= d.X;

			M21 *= d.Y;
			M22 *= d.Y;
			M23 *= d.Y;

			M31 *= d.Z;
			M32 *= d.Z;
			M33 *= d.Z;
		}

		public void multiplyDiagonal(Vector3 d,ref NxMat33 dst)
		{
			dst.M11 = M11 * d.X;
			dst.M12 = M12 * d.Y;
			dst.M13 = M13 * d.Z;

			dst.M21 = M21 * d.X;
			dst.M22 = M22 * d.Y;
			dst.M23 = M23 * d.Z;

			dst.M31 = M31 * d.X;
			dst.M32 = M32 * d.Y;
			dst.M33 = M33 * d.Z;
		}

		public void multiplyDiagonalTranspose(Vector3 d,ref NxMat33 dst)
		{
			dst.M11 = M11 * d.X;
			dst.M12 = M21 * d.Y;
			dst.M13 = M31 * d.Z;

			dst.M21 = M12 * d.X;
			dst.M22 = M22 * d.Y;
			dst.M23 = M32 * d.Z;

			dst.M31 = M13 * d.X;
			dst.M32 = M23 * d.Y;
			dst.M33 = M33 * d.Z;
		}

		public void multiplyTransposeLeft(ref NxMat33 left,ref NxMat33 right)
		{
			float a,b,c, d,e,f, g,h,i;
			//note: temps needed so that x.multiply(x,y) works OK.
			a =left.M11 * right.M11 +left.M21 * right.M21 +left.M31 * right.M31;
			b =left.M11 * right.M12 +left.M21 * right.M22 +left.M31 * right.M32;
			c =left.M11 * right.M13 +left.M21 * right.M23 +left.M31 * right.M33;

			d =left.M12 * right.M11 +left.M22 * right.M21 +left.M32 * right.M31;
			e =left.M12 * right.M12 +left.M22 * right.M22 +left.M32 * right.M32;
			f =left.M12 * right.M13 +left.M22 * right.M23 +left.M32 * right.M33;

			g =left.M13 * right.M11 +left.M23 * right.M21 +left.M33 * right.M31;
			h =left.M13 * right.M12 +left.M23 * right.M22 +left.M33 * right.M32;
			i =left.M13 * right.M13 +left.M23 * right.M23 +left.M33 * right.M33;

			M11 = a;
			M12 = b;
			M13 = c;

			M21 = d;
			M22 = e;
			M23 = f;

			M31 = g;
			M32 = h;
			M33 = i;
		}

		public void multiplyTransposeRight(ref NxMat33 left,ref NxMat33 right)
		{
			float a,b,c, d,e,f, g,h,i;
			//note: temps needed so that x.multiply(x,y) works OK.
			a =left.M11 * right.M11 +left.M12 * right.M12 +left.M13 * right.M13;
			b =left.M11 * right.M21 +left.M12 * right.M22 +left.M13 * right.M23;
			c =left.M11 * right.M31 +left.M12 * right.M32 +left.M13 * right.M33;

			d =left.M21 * right.M11 +left.M22 * right.M12 +left.M23 * right.M13;
			e =left.M21 * right.M21 +left.M22 * right.M22 +left.M23 * right.M23;
			f =left.M21 * right.M31 +left.M22 * right.M32 +left.M23 * right.M33;

			g =left.M31 * right.M11 +left.M32 * right.M12 +left.M33 * right.M13;
			h =left.M31 * right.M21 +left.M32 * right.M22 +left.M33 * right.M23;
			i =left.M31 * right.M31 +left.M32 * right.M32 +left.M33 * right.M33;

			M11 = a;
			M12 = b;
			M13 = c;

			M21 = d;
			M22 = e;
			M23 = f;

			M31 = g;
			M32 = h;
			M33 = i;
		}

		public void multiplyTransposeRight(Vector3 left,Vector3 right)
		{
			M11 = left.X * right.X;
			M12 = left.X * right.Y;
			M13 = left.X * right.Z;

			M21 = left.Y * right.X;
			M22 = left.Y * right.Y;
			M23 = left.Y * right.Z;

			M31 = left.Z * right.X;
			M32 = left.Z * right.Y;
			M33 = left.Z * right.Z;
		}



		public void add(ref NxMat33 a,ref NxMat33 b)
		{
			M11 = a.M11 + b.M11;
			M12 = a.M12 + b.M12;
			M13 = a.M13 + b.M13;

			M21 = a.M21 + b.M21;
			M22 = a.M22 + b.M22;
			M23 = a.M23 + b.M23;

			M31 = a.M31 + b.M31;
			M32 = a.M32 + b.M32;
			M33 = a.M33 + b.M33;
		}
    
		public void subtract(ref NxMat33 a,ref NxMat33 b)
		{
			M11 = a.M11 - b.M11;
			M12 = a.M12 - b.M12;
			M13 = a.M13 - b.M13;

			M21 = a.M21 - b.M21;
			M22 = a.M22 - b.M22;
			M23 = a.M23 - b.M23;

			M31 = a.M31 - b.M31;
			M32 = a.M32 - b.M32;
			M33 = a.M33 - b.M33;
		}
    
		public bool getInverse(ref NxMat33 dest)
		{
			float b00,b01,b02,b10,b11,b12,b20,b21,b22;

			b00 = M22*M33-M23*M32;	b01 = M13*M32-M12*M33;	b02 = M12*M23-M13*M22;
			b10 = M23*M31-M21*M33;	b11 = M11*M33-M13*M31;	b12 = M13*M21-M11*M23;
			b20 = M21*M32-M22*M31;	b21 = M12*M31-M11*M32;	b22 = M11*M22-M12*M21;

			float d = b00*M11		+		b01*M21				 + b02 * M31;

			if (d == 0)		//singular?
			{
				dest.id();
				return false;
			}

			d = 1/d;

			//only do assignment at the end, in case dest == this:

			dest.M11 = b00*d; dest.M12 = b01*d; dest.M13 = b02*d;
			dest.M21 = b10*d; dest.M22 = b11*d; dest.M23 = b12*d;
			dest.M31 = b20*d; dest.M32 = b21*d; dest.M33 = b22*d;

			return true;
		}

		public void setRow(int row,Vector3 v)
		{
			setElement(v.X,row,0);
			setElement(v.Y,row,1);
			setElement(v.Z,row,2);
		}

		public void setColumn(int col,Vector3 v)
		{
			setElement(v.X,0,col);
			setElement(v.Y,1,col);
			setElement(v.Z,2,col);
		}

		public Vector3 getRow(int row)
		{
			Vector3 v=new Vector3(0,0,0);
			v.X=getElement(row,0);
			v.Y=getElement(row,1);
			v.Z=getElement(row,2);
			return v;
		}

		public Vector3 getColumn(int col)
		{
			Vector3 v=new Vector3(0,0,0);
			v.X=getElement(0,col);
			v.Y=getElement(1,col);
			v.Z=getElement(2,col);
			return v;
		}
   
		public void rotX(float angle)
		{
			float Cos = (float)Math.Cos(angle);
			float Sin = (float)Math.Sin(angle);
			id();
			setElement(Cos,1,1);
			setElement(Cos,2,2);
			setElement(-Sin,1,2);
			setElement(Sin,2,1);
		}

		public void rotY(float angle)
		{
			float Cos = (float)Math.Cos(angle);
			float Sin = (float)Math.Sin(angle);
			id();
			setElement(Cos,0,0);
			setElement(Cos,2,2);
			setElement(Sin,0,2);
			setElement(-Sin,2,0);
		}

		public void rotZ(float angle)
		{
			float Cos = (float)Math.Cos(angle);
			float Sin = (float)Math.Sin(angle);
			id();
			setElement(Cos,0,0);
			setElement(Cos,1,1);
			setElement(-Sin,0,1);
			setElement(Sin,1,0);
		}

		public static Vector3 operator %(NxMat33 mat,Vector3 src)
		{
			Vector3 dest;
			mat.multiplyByTranspose(src,out dest);
			return dest;
		}

		public static Vector3 operator *(NxMat33 mat,Vector3 src)
		{
			Vector3 dest;
			mat.multiply(src,out dest);
			return dest;
		}
		
		public static NxMat33 operator *(NxMat33 mat1,NxMat33 mat2)
		{
			NxMat33 temp=new NxMat33(NxMatrixType.NX_ZERO_MATRIX);
			temp.multiply(ref mat1,ref mat2);
			return temp;
		}

		public static NxMat33 operator *(NxMat33 mat,float s)
		{
			NxMat33 temp=new NxMat33(NxMatrixType.NX_ZERO_MATRIX);
			temp.multiply(s,ref mat);
			return temp;
		}

		public static NxMat33 operator /(NxMat33 mat,float x)
		{
			NxMat33 temp=mat;
			float f = 1/x;
			temp.M11 *= f;
			temp.M12 *= f;
			temp.M13 *= f;

			temp.M21 *= f;
			temp.M22 *= f;
			temp.M23 *= f;

			temp.M31 *= f;
			temp.M32 *= f;
			temp.M33 *= f;
			return temp;
		}

		public static NxMat33 operator -(NxMat33 mat1,NxMat33 mat2)
		{
			NxMat33 temp=new NxMat33(NxMatrixType.NX_ZERO_MATRIX);
			temp.subtract(ref mat1,ref mat2);
			return temp;
		}

		public static NxMat33 operator +(NxMat33 mat1,NxMat33 mat2)
		{
			NxMat33 temp=new NxMat33(NxMatrixType.NX_ZERO_MATRIX);
			temp.add(ref mat1,ref mat2);
			return temp;
		}

		public static explicit operator NxQuat(NxMat33 mat)
		{
			NxQuat q=NxQuat.Identity;
			mat.toQuat(ref q);
			return q;
		}



		public void setRowMajor(float[] d)
		{
			//we are also row major, so this is a direct copy
			M11 = d[0];
			M12 = d[1];
			M13 = d[2];

			M21 = d[3];
			M22 = d[4];
			M23 = d[5];

			M31 = d[6];
			M32 = d[7];
			M33 = d[8];
		}

		public void setColumnMajor(float[] d)
		{
			//we are column major, so copy transposed.
			M11 = d[0];
			M12 = d[3];
			M13 = d[6];

			M21 = d[1];
			M22 = d[4];
			M23 = d[7];

			M31 = d[2];
			M32 = d[5];
			M33 = d[8];
		}

		public void getRowMajor(float[] d)
		{
			//we are also row major, so this is a direct copy
			d[0] = M11;
			d[1] = M12;
			d[2] = M13;

			d[3] = M21;
			d[4] = M22;
			d[5] = M23;

			d[6] = M31;
			d[7] = M32;
			d[8] = M33;
		}

		public void getColumnMajor(float[] d)
		{
			//we are column major, so copy transposed.
			d[0] = M11;
			d[3] = M12;
			d[6] = M13;

			d[1] = M21;
			d[4] = M22;
			d[7] = M23;

			d[2] = M31;
			d[5] = M32;
			d[8] = M33;
		}

		public void setRowMajorStride4(float[] d)
		{
			//we are also row major, so this is a direct copy
			//however we've got to skip every 4th element.
			M11 = d[0];
			M12 = d[1];
			M13 = d[2];

			M21 = d[4];
			M22 = d[5];
			M23 = d[6];

			M31 = d[8];
			M32 = d[9];
			M33 = d[10];
		}

		public void setColumnMajorStride4(float[] d)
		{
			//we are column major, so copy transposed.
			//however we've got to skip every 4th element.
			M11 = d[0];
			M12 = d[4];
			M13 = d[8];

			M21 = d[1];
			M22 = d[5];
			M23 = d[9];

			M31 = d[2];
			M32 = d[6];
			M33 = d[10];
		}

		public void getRowMajorStride4(float[] d)
		{
			//we are also row major, so this is a direct copy
			//however we've got to skip every 4th element.
			d[0] = M11;
			d[1] = M12;
			d[2] = M13;

			d[4] = M21;
			d[5] = M22;
			d[6] = M23;

			d[8] = M31;
			d[9] = M32;
			d[10]= M33;
		}

		public void getColumnMajorStride4(float[] d)
		{
			//we are column major, so copy transposed.
			//however we've got to skip every 4th element.
			d[0] = M11;
			d[4] = M12;
			d[8] = M13;

			d[1] = M21;
			d[5] = M22;
			d[9] = M23;

			d[2] = M31;
			d[6] = M32;
			d[10]= M33;
		}







//////////////////////////////////////////////////////////////////////
//These were added because in C# you can't operator overload ()     //
//////////////////////////////////////////////////////////////////////

		public float getElement(int row,int col)
		{
			if(row<0 || row>=3 || col<0 || col>=3)
				{throw new Exception("NxMat33 getElement("+row+","+col+") out of bounds");}
			
			unsafe
			{
				fixed(void* x=&M11)
				{
					float* m=(float*)x;
					if(staticTransposedFlag)
						{return m[(row*3)+col];}
					else
						{return m[(col*3)+row];}
				}
			}
		}

		public void setElement(float value,int row,int col)
		{
			if(row<0 || row>=3 || col<0 || col>=3)
				{throw new Exception("NxMat33 setElement("+row+","+col+") out of bounds");}

			unsafe
			{
				fixed(void* x=&M11)
				{
					float* m=(float*)x;
					if(staticTransposedFlag)
						{m[(row*3)+col]=value;}
					else
						{m[(col*3)+row]=value;}
				}
			}
		}

////////////////////////////////////////////////////////////////////////////////////////////////////
//Everything below this is to make NxMat33 act more like DirectX.Matrix and is not part of Novodex//
////////////////////////////////////////////////////////////////////////////////////////////////////		
		public static NxMat33 Identity
		{
			get
			{
				NxMat33 mat;
				mat.M11=1; mat.M21=0; mat.M31=0;
				mat.M12=0; mat.M22=1; mat.M32=0;
				mat.M13=0; mat.M23=0; mat.M33=1;
				return mat;
			}
		}

		public static NxMat33 Zero
		{
			get
			{
				NxMat33 mat;
				mat.M11=0; mat.M21=0; mat.M31=0;
				mat.M12=0; mat.M22=0; mat.M32=0;
				mat.M13=0; mat.M23=0; mat.M33=0;
				return mat;
			}
		}

		//To match DirectX the angle needs to be negated. I though of flipping it inside the function but it make no sense for RotationX() to be the opposite of rotX()
		static public NxMat33 RotationX(float angleInRads)
		{
			NxMat33 mat=NxMat33.Identity;
			mat.rotX(angleInRads);
			return mat;
		}

		//To match DirectX the angle needs to be negated. I though of flipping it inside the function but it make no sense for RotationY() to be the opposite of rotY()
		static public NxMat33 RotationY(float angleInRads)
		{
			NxMat33 mat=NxMat33.Identity;
			mat.rotY(angleInRads);
			return mat;
		}

		//To match DirectX the angle needs to be negated. I though of flipping it inside the function but it make no sense for RotationZ() to be the opposite of rotZ()
		static public NxMat33 RotationZ(float angleInRads)
		{
			NxMat33 mat=NxMat33.Identity;
			mat.rotZ(angleInRads);
			return mat;
		}

		public static NxMat33 RotationYawPitchRoll(float yawInRads,float pitchInRads,float rollInRads)
		{
			//To make it match DirectX the angles are flipped
			return NxMat33.RotationZ(-rollInRads)*NxMat33.RotationX(-pitchInRads)*NxMat33.RotationY(-yawInRads);
		}

		public static NxMat33 Scaling(float sX,float sY,float sZ)
		{
			NxMat33 mat=NxMat33.Identity;
			mat.M11=sX;
			mat.M22=sY;
			mat.M33=sZ;
			return mat;
		}

		public static NxMat33 Scaling(Vector3 s)
		{
			NxMat33 mat=NxMat33.Identity;
			mat.M11=s.X;
			mat.M22=s.Y;
			mat.M33=s.Z;
			return mat;
		}





/////////////////////////////////////////////////////////////////
//These were added just because I find them nice to have       //
/////////////////////////////////////////////////////////////////

		public Vector3 getXaxis()
			{return getRow(0);}

		public Vector3 getYaxis()
			{return getRow(1);}

		public Vector3 getZaxis()
			{return getRow(2);}

		public void setXaxis(Vector3 xAxis)
			{setRow(0,xAxis);}

		public void setYaxis(Vector3 yAxis)
			{setRow(1,yAxis);}

		public void setZaxis(Vector3 zAxis)
			{setRow(2,zAxis);}



//These methods are only in the DirectX version.
////////////////////////////////////////////////////////////////////////////
//Implicit conversion operators to make interfacing with DirectX easier   //
////////////////////////////////////////////////////////////////////////////
		public static implicit operator Matrix(NxMat33 mat)
			{return NovodexUtil.convertNxMat33ToMatrix(mat);}

		public static implicit operator NxMat33(Matrix mat)
			{return NovodexUtil.convertMatrixToNxMat33(mat);}
	}
}








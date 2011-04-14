//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxVec3
	{
		public float x,y,z;


		static public NxVec3 Zero
			{get{return new NxVec3(0,0,0);}}


		public NxVec3(float a)
			{x=y=z=a;}

		public NxVec3(float nx,float ny,float nz)
		{
			x=nx;
			y=ny;
			z=nz;
		}

		public NxVec3(float[] v)
		{
			x=v[0];
			y=v[1];
			z=v[2];
		}

		public NxVec3(NxVec3 v)
		{
			x=v.x;
			y=v.y;
			z=v.z;
		}

		public void setx(float d)
			{x=d;}

		public void sety(float d)
			{y=d;}

		public void setz(float d)
			{z=d;}

		public void set(float[] v)
		{
			x=v[0];
			y=v[1];
			z=v[2];
		}

		public void set(float a)
			{x=y=z=a;}

		public void set(float nx,float ny,float nz)
		{
			x=nx;
			y=ny;
			z=nz;
		}

		public void set(NxVec3 v)
		{
			x=v.x;
			y=v.y;
			z=v.z;
		}

		public bool isZero()
		{
			if(x!=0 || y!=0 || z!=0){return false;}
			return true;
		}

		public void get(float[] v)
		{
			v[0]=x;
			v[1]=y;
			v[2]=z;
		}
		
		public float this [int index]
		{
			get 
			{
				if(index==0)
					{return x;}
				else if(index==1)
					{return y;}
				else if(index==2)
					{return z;}
				else
					{throw new Exception("NxVec3 get["+index+"] out of bounds");}
			}
			set 
			{
				if(index==0)
					{x=value;}
				else if(index==1)
					{y=value;}
				else if(index==2)
					{z=value;}
				else
					{throw new Exception("NxVec3 set["+index+"] out of bounds");}
			}
		}

		public void setNegative(NxVec3 v)
		{
			x = -v.x;
			y = -v.y;
			z = -v.z;
		}

		public void setNegative()
		{
			x = -x;
			y = -y;
			z = -z;
		}
 
		public void  zero()
			{x=y=z=0;}

		public void setPlusInfinity()
			{x=y=z=float.MaxValue;}

		public void setMinusInfinity()
			{x=y=z=float.MinValue;}

		public void max(NxVec3 v)
		{
			if(x < v.x){x = v.x;}
			if(y < v.y){y = v.y;}
			if(z < v.z){z = v.z;}
		}

		public void min(NxVec3  v)
		{
			if(x > v.x){x = v.x;}
			if(y > v.y){y = v.y;}
			if(z > v.z){z = v.z;}
		}

		public void add(NxVec3 a,NxVec3 b)
		{
			x = a.x + b.x;
			y = a.y + b.y;
			z = a.z + b.z;
		}

		public void subtract(NxVec3 a,NxVec3 b)
		{
			x = a.x - b.x;
			y = a.y - b.y;
			z = a.z - b.z;
		}

		public void arrayMultiply(NxVec3 a,NxVec3 b)
		{
			x = a.x * b.x;
			y = a.y * b.y;
			z = a.z * b.z;
		}

		public void multiply(float s,NxVec3 a)
		{
			x = a.x * s;
			y = a.y * s;
			z = a.z * s;
		}

		public void multiplyAdd(float s,NxVec3 a,NxVec3 b)
		{
			x = s * a.x + b.x;
			y = s * a.y + b.y;
			z = s * a.z + b.z;
		}

		public bool isFinite()
		{
			if(float.IsInfinity(x) || float.IsNaN(x)){return false;}
			if(float.IsInfinity(y) || float.IsNaN(y)){return false;}
			if(float.IsInfinity(z) || float.IsNaN(z)){return false;}
			return true;
		}
 
		public float dot(NxVec3 v)
			{return (x * v.x) + (y * v.y) + (z * v.z);}

		public NxVec3 cross(NxVec3 v)
		{
			NxVec3 temp=new NxVec3(0);
			temp.cross(this,v);
			return temp;
		}
 
		public bool sameDirection(NxVec3 v)
			{return ((x*v.x) + (y*v.y) + (z*v.z)) >= 0.0f;}

		public float magnitude()
			{return (float)Math.Sqrt((x*x) + (y*y) + (z*z));}

		public float magnitudeSquared()
			{return ((x*x) + (y*y) + (z*z));}
 
		public float distance(NxVec3 v)
		{
			float dx = x - v.x;
			float dy = y - v.y;
			float dz = z - v.z;
			return (float)Math.Sqrt((dx*dx) + (dy*dy) + (dz*dz));
		}
 
		public float distanceSquared(NxVec3 v)
		{
			float dx = x - v.x;
			float dy = y - v.y;
			float dz = z - v.z;
			return ((dx*dx) + (dy*dy) + (dz*dz));
		}
 
		public void cross(NxVec3 left,NxVec3 right)	//prefered version, w/o temp object.
		{
			// temps needed in case left or right is this.
			float a = (left.y * right.z) - (left.z * right.y);
			float b = (left.z * right.x) - (left.x * right.z);
			float c = (left.x * right.y) - (left.y * right.x);

			x = a;
			y = b;
			z = c;
		}

		public float normalize()
		{
			float m = magnitude();
			if(m!=0)
			{
				float il = 1.0f / m;
				x *= il;
				y *= il;
				z *= il;
			}
			return m;
		}
 
		public void setMagnitude(float length)
		{
			float m = magnitude();
			if(m!=0)
			{
				float newLength = length / m;
				x *= newLength;
				y *= newLength;
				z *= newLength;
			}
		}

		public void setNotUsed()
		{
			unsafe
			{
				fixed(void* a=&x)
				{
					uint* d=(uint*)a;
					d[0]=d[1]=d[2]=0xFFFFFFFF;
					// We use a particular integer pattern : 0xffffffff everywhere. This is a NAN.
				}
			}
		}
 
		public bool isNotUsed()
		{
			unsafe
			{
				fixed(void* a=&x)
				{
					uint* d=(uint*)a;
					if(d[0]!=0xFFFFFFFF || d[1]!=0xFFFFFFFF || d[2]!=0xFFFFFFFF)
						{return false;}
					return true;
				}
			}
		}

		public bool equals(NxVec3 v,float epsilon)
		{
			float dx=x-v.x;
			float dy=y-v.y;
			float dz=z-v.z;
			if(dx<-epsilon || dx>epsilon){return false;}
			if(dy<-epsilon || dz>epsilon){return false;}
			if(dy<-epsilon || dz>epsilon){return false;}
			return true;
		}

		NxAxisType snapToClosestAxis()
		{
			float almostOne = 0.999999f;
			if(x >=  almostOne)			{ set( 1.0f,  0.0f,  0.0f);	return NxAxisType.NX_AXIS_PLUS_X ; }
			else	if(x <= -almostOne) { set(-1.0f,  0.0f,  0.0f);	return NxAxisType.NX_AXIS_MINUS_X; }
			else	if(y >=  almostOne) { set( 0.0f,  1.0f,  0.0f);	return NxAxisType.NX_AXIS_PLUS_Y ; }
			else	if(y <= -almostOne) { set( 0.0f, -1.0f,  0.0f);	return NxAxisType.NX_AXIS_MINUS_Y; }
			else	if(z >=  almostOne) { set( 0.0f,  0.0f,  1.0f);	return NxAxisType.NX_AXIS_PLUS_Z ; }
			else	if(z <= -almostOne) { set( 0.0f,  0.0f, -1.0f);	return NxAxisType.NX_AXIS_MINUS_Z; }
			else													return NxAxisType.NX_AXIS_ARBITRARY;
		}

		//I'm not sure if this is correct. The conversion to C# was whacky.
		public uint closestAxis()
		{
			unsafe
			{
				fixed(void *a=&x)
				{
					uint *vals=(uint*)a;
					uint m=0;
					if((vals[1]&0x7FFFFFFF) > (vals[m]&0x7FFFFFFF)){m=1;}
					if((vals[2]&0x7FFFFFFF) > (vals[m]&0x7FFFFFFF)){m=2;}
					return m;
				}
			}
		}

		public static bool operator <(NxVec3 v1,NxVec3 v2)
			{return ((v1.x < v2.x)&&(v1.y < v2.y)&&(v1.z < v2.z));}

		public static bool operator >(NxVec3 v1,NxVec3 v2)
			{return ((v1.x > v2.x)&&(v1.y > v2.y)&&(v1.z > v2.z));}

		public static NxVec3 operator -(NxVec3 v)
			{return new NxVec3(-v.x,-v.y,-v.z);}

		public static NxVec3 operator +(NxVec3 v1,NxVec3 v2)
			{return new NxVec3(v1.x+v2.x,v1.y+v2.y,v1.z+v2.z);}

		public static NxVec3 operator -(NxVec3 v1,NxVec3 v2)
			{return new NxVec3(v1.x-v2.x,v1.y-v2.y,v1.z-v2.z);}

		public static NxVec3 operator *(NxVec3 v,float s)
			{return new NxVec3(v.x*s,v.y*s,v.z*s);}

		public static NxVec3 operator /(NxVec3 v,float s)
		{
			float f=1.0f/s;
			return new NxVec3(v.x*f,v.y*f,v.z*f);
		}

		public static NxVec3 operator^(NxVec3 v1,NxVec3 v2)
		{
			NxVec3 temp=new NxVec3(0);
			temp.cross(v1,v2);
			return temp;
		}

		public static float operator|(NxVec3 v1,NxVec3 v2)
			{return ((v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z));}

		public static NxVec3 operator *(float f,NxVec3 v)
			{return new NxVec3(f * v.x, f * v.y, f * v.z);}

		public static bool operator ==(NxVec3 v1,NxVec3 v2)
			{return ((v1.x==v2.x) && (v1.y==v2.y) && (v1.z==v2.z));}

		public static bool operator !=(NxVec3 v1,NxVec3 v2)
			{return ((v1.x!=v2.x) || (v1.y!=v2.y) || (v1.z!=v2.z));}

		public override bool Equals(object obj)
		{
			if(obj==null || GetType()!=obj.GetType())
				{return false;}
			NxVec3 v=(NxVec3)obj;
			return ((x==v.x) && (y==v.y) && (z==v.z));
		}

		public override int GetHashCode()
			{return (x.GetHashCode())^(y.GetHashCode())^(z.GetHashCode());}





////////////////////////////////////////////////////////////////////////////////////////////////////
//Everything below this is to make NxVec3 act more like DirectX.Vector3 and is not part of Novodex//
////////////////////////////////////////////////////////////////////////////////////////////////////		
		static public NxVec3 TransformCoordinate(NxVec3 worldPoint,NxMat34 mat)
		{
			//NxVec3 tran;
			//mat.M.multiplyByTranspose(worldPoint,out tran);
			//tran+=mat.t;
			//return tran;

			//This is the same as above just manually written out.
			NxVec3 tran;
			tran.x=(worldPoint.x*mat.M.M11)+(worldPoint.y*mat.M.M21)+(worldPoint.z*mat.M.M31)+(mat.t.X);
			tran.y=(worldPoint.x*mat.M.M12)+(worldPoint.y*mat.M.M22)+(worldPoint.z*mat.M.M32)+(mat.t.Y);
			tran.z=(worldPoint.x*mat.M.M13)+(worldPoint.y*mat.M.M23)+(worldPoint.z*mat.M.M33)+(mat.t.Z);
			return tran;
		}

		static public NxVec3 TransformNormal(NxVec3 worldNormal,NxMat34 mat)
		{
			//NxVec3 tran;
			//mat.M.multiplyByTranspose(worldNormal,out tran);
			//return tran;

			//This is the same as above just manually written out.
			NxVec3 tran;
			tran.x=(worldNormal.x*mat.M.M11)+(worldNormal.y*mat.M.M21)+(worldNormal.z*mat.M.M31);
			tran.y=(worldNormal.x*mat.M.M12)+(worldNormal.y*mat.M.M22)+(worldNormal.z*mat.M.M32);
			tran.z=(worldNormal.x*mat.M.M13)+(worldNormal.y*mat.M.M23)+(worldNormal.z*mat.M.M33);
			return tran;
		}




//These methods are only in the DirectX version.
////////////////////////////////////////////////////////////////////////////
//Implicit conversion operators to make interfacing with DirectX easier   //
////////////////////////////////////////////////////////////////////////////
		public static implicit operator Vector3(NxVec3 v)
			{return new Vector3(v.x,v.y,v.z);}

		public static implicit operator NxVec3(Vector3 v)
			{return new NxVec3(v.X,v.Y,v.Z);}
	}
}





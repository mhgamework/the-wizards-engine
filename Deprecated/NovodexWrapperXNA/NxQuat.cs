//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxQuat
	{
		public float x,y,z,w;
	
		public static NxQuat Identity
			{get{return new NxQuat(0,0,0,1);}}
		

		public NxQuat(NxQuat q)
			{this.x=q.x; this.y=q.y; this.z=q.z; this.w=q.w;}

		public NxQuat(Vector3 v,float w)
			{this.x=v.X; this.y=v.Y; this.z=v.Z; this.w=w;}

		public NxQuat(float x,float y,float z,float w)
			{this.x=x; this.y=y; this.z=z; this.w=w;}

		public NxQuat(float angleDegrees,Vector3 axis)
		{
			x=y=z=w=0;
			fromAngleAxis(angleDegrees,axis);
		}
		
		public void setXYZW(float x,float y,float z,float w)
			{this.x=x; this.y=y; this.z=z; this.w=w;}

		public void setWXYZ(float w,float x,float y,float z)
			{this.w=w; this.x=x; this.y=y; this.z=z;}

		public void id()
			{setXYZW(0,0,0,1);}
			
		public void zero()
			{setXYZW(0,0,0,1);}
			
		public void negate()
			{x=-x; y=-y; z=-z; w=-w;}

		public void conjugate()
			{x=-x; y=-y; z=-z;}
			
		public void setx(float x){this.x=x;}
		public void sety(float y){this.y=y;}
		public void setz(float z){this.z=z;}
		public void setw(float w){this.w=w;}

		public bool isIdentityRotation()
			{return x==0 && y==0 && z==0 && (Math.Abs(w)==1);}

		float magnitudeSquared()
			{return (x*x) + (y*y) + (z*z) + (w*w);}

		public void normalize()
		{
			float mag = (float)Math.Sqrt(magnitudeSquared());
			if(mag!=0)
			{
				float invMag = 1.0f/mag;
				x *= invMag;
				y *= invMag;
				z *= invMag;
				w *= invMag;
			}
		}
		
		public void random()
		{
			x = NovodexUtil.randomFloat(0,1);
			y = NovodexUtil.randomFloat(0,1);
			z = NovodexUtil.randomFloat(0,1);
			w = NovodexUtil.randomFloat(0,1);
			normalize();
		}

		public float getAngle()
			{return (float)(Math.Acos(w)*2.0f);}

		public float getAngle(NxQuat q)
			{return (float)(Math.Acos(dot(q))*2.0f);}

		public float dot(NxQuat v)
			{return (x*v.x) + (y*v.y) + (z*v.z) + (w*v.w);}

		public void fromAngleAxis(float angle,Vector3 axis)
		{
			axis.Normalize();

			float half = NovodexUtil.DEG_TO_RAD*(angle * 0.5f);
			float sin_theta_over_two = (float)Math.Sin(half);
			
			x = axis.X * sin_theta_over_two;
			y = axis.Y * sin_theta_over_two;
			z = axis.Z * sin_theta_over_two;
			w = (float)Math.Cos(half);
		}
		
		public void getAngleAxis(ref float angle,ref Vector3 axis)
		{
			angle = (float)(Math.Acos(w) * 2.0f);		//this is getAngle()
			float sa = (float)Math.Sqrt(1.0f - w*w);
			if(sa!=0)
			{
				axis=new Vector3(x/sa,y/sa,z/sa);
				angle = NovodexUtil.RAD_TO_DEG*angle;
			}
			else
				{axis=new Vector3(0,0,0);}
		}

		public Vector3 rot(Vector3 v)
		{
			Vector3 qv=new Vector3(x,y,z);
			return (v*(w*w-0.5f) + (Vector3.Cross(v,v))*w + qv*(Vector3.Dot(qv,v)))*2;
		}

		public Vector3 invRot(Vector3 v)
		{
			Vector3 qv=new Vector3(x,y,z);
			return (v*(w*w-0.5f) - (Vector3.Cross(v,v))*w + qv*(Vector3.Dot(qv,v)))*2;
		}

		public Vector3 transform(Vector3 v, Vector3 p)
			{return rot(v)+p;}

		public Vector3 invTransform(Vector3 v, Vector3 p)
			{return invRot(v-p);}

		public void multiply(NxQuat left,NxQuat right)		// this = a * b
		{
			x =left.w*right.x + right.w*left.x + left.y*right.z - right.y*left.z;
			y =left.w*right.y + right.w*left.y + left.z*right.x - right.z*left.x;
			z =left.w*right.z + right.w*left.z + left.x*right.y - right.x*left.y;
			w =left.w*right.w - left.x*right.x - left.y*right.y - left.z*right.z;
		}

		public void multiply(NxQuat left,Vector3 right)		// this = a * b
		{
			x =   left.w*right.X + left.y*right.Z - right.Y*left.z;
			y =   left.w*right.Y + left.z*right.X - right.Z*left.x;
			z =   left.w*right.Z + left.x*right.Y - right.X*left.y;
			w = - left.x*right.X - left.y*right.Y - left.z *right.Z;
		}
		
		public void rotate(Vector3 v)						//rotates passed vec by rot expressed by quaternion.  overwrites arg ith the result.
		{
			NxQuat myInverse=new NxQuat(-x,-y,-z,w);
			NxQuat left=new NxQuat(0,0,0,0);
			left.multiply(this,v);
			v.X =left.w*myInverse.x + myInverse.w*left.x + left.y*myInverse.z - myInverse.y*left.z;
			v.Y =left.w*myInverse.y + myInverse.w*left.y + left.z*myInverse.x - myInverse.z*left.x;
			v.Z =left.w*myInverse.z + myInverse.w*left.z + left.x*myInverse.y - myInverse.x*left.y;
		}

		public void inverseRotate(Vector3 v)				//rotates passed vec by opposite of rot expressed by quaternion.  overwrites arg ith the result.
		{
			NxQuat myInverse=new NxQuat(-x,-y,-z,w);
			NxQuat left=new NxQuat(0,0,0,0);
			left.multiply(myInverse,v);
			v.X =left.w*x + w*left.x + left.y*z - y*left.z;
			v.Y =left.w*y + w*left.y + left.z*x - z*left.x;
			v.Z =left.w*z + w*left.z + left.x*y - x*left.y;
		}
		
		public void slerp(float t,NxQuat left,NxQuat right)	//this = slerp(t, a, b)
		{
			float quatEpsilon = 1.0e-8f;
			this = left;
			float cosine = (x * right.x) + (y * right.y) + (z * right.z) + (w * right.w); //this is left.dot(right)
			float sign = 1.0f;
			if(cosine < 0)
			{
				cosine = - cosine;
				sign = -1.0f;
			}

			float Sin = 1.0f - cosine*cosine;
			if(Sin>=quatEpsilon*quatEpsilon)	
			{
				Sin = (float)Math.Sqrt(Sin);
				float angle = (float)Math.Atan2(Sin, cosine);
				float i_sin_angle = 1.0f / Sin;
				float lower_weight = (float)Math.Sin(angle*(1.0f-t)) * i_sin_angle;
				float upper_weight = (float)Math.Sin(angle * t) * i_sin_angle * sign;
				w = (w * (lower_weight)) + (right.w * (upper_weight));
				x = (x * (lower_weight)) + (right.x * (upper_weight));
				y = (y * (lower_weight)) + (right.y * (upper_weight));
				z = (z * (lower_weight)) + (right.z * (upper_weight));
			}
		}
	}
}



#region Incomplete Code
/*
NX_INLINE bool NxQuat::isFinite() const
	{return NxMath::isFinite(x) && NxMath::isFinite(y) && NxMath::isFinite(z) && NxMath::isFinite(w);}

NxQuat operator-()
	{return NxQuat(-x,-y,-z,-w);}

NxQuat operator=  (NxQuat q)
	{x = q.x; y = q.y; z = q.z; w = q.w; return *this;}

#if 0
NxQuat operator=  (Vector3 v)		//implicitly extends vector by a 0 w element.
{
	x = v.x;
	y = v.y;
	z = v.z;
	w = float(1.0);
	return *this;
}
#endif

NxQuat operator*= (NxQuat q)
{
	float xx[4]; //working Quaternion
	xx[0] = w*q.w - q.x*x - y*q.y - q.z*z;
	xx[1] = w*q.x + q.w*x + y*q.z - q.y*z;
	xx[2] = w*q.y + q.w*y + z*q.x - q.z*x;
	z=w*q.z + q.w*z + x*q.y - q.x*y;

	w = xx[0];
	x = xx[1];
	y = xx[2];
	return *this;
}

NxQuat operator+= (NxQuat q)
	{x+=q.x; y+=q.y; z+=q.z; w+=q.w; return *this;}

NxQuat operator-= (NxQuat q)
	{x-=q.x; y-=q.y; z-=q.z; w-=q.w; return *this;}

NxQuat operator*= (float s)
	{x*=s; y*=s; z*=s; w*=s; return *this;}

NxQuat operator*(NxQuatq)
{
	return NxQuat(w*q.x + q.w*x + y*q.z - q.y*z,
				  w*q.y + q.w*y + z*q.x - q.z*x,
				  w*q.z + q.w*z + x*q.y - q.x*y,
				  w*q.w - x*q.x - y*q.y - z*q.z);
}

NxQuat operator+(NxQuatq)
	{return NxQuat(x+q.x,y+q.y,z+q.z,w+q.w);}

NxQuat operator-(NxQuatq)
	{return NxQuat(x-q.x,y-q.y,z-q.z,w-q.w);}

NxQuat operator!()
	{return NxQuat(-x,-y,-z,w);}
*/
#endregion

//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;


namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxMat34
	{
		public NxMat33 M;
		public Vector3 t;
		
		public NxMat34(NxMat33 rot,Vector3 trans)
		{
			M=rot;
			t=trans;
		}

		public NxMat34(bool init)
		{
			t=new Vector3(0,0,0);
			
			if(init)
				{M=NxMat33.Identity;}
			else
				{M.M11=M.M21=M.M31=M.M12=M.M22=M.M32=M.M13=M.M23=M.M33=0;}
		}

		public void id()
		{
			M.id();
			t=new Vector3(0,0,0);
		}

		public void zero()
		{
			M.zero();
			t=new Vector3(0,0,0);
		}


		public bool isIdentity()
		{
			if(!M.isIdentity()){return false;}
			if(t.X!=0 || t.Y!=0 || t.Z!=0){return false;}
			return true;
		}

		public bool isZero()
		{
			if(!M.isZero()){return false;}
			if(t.X!=0 || t.Y!=0 || t.Z!=0){return false;}
			return true;
		}

		public bool isFinite()
		{
			if(!M.isFinite()){return false;}

			if(float.IsInfinity(t.X) || float.IsNaN(t.X)){return false;}
			if(float.IsInfinity(t.Y) || float.IsNaN(t.Y)){return false;}
			if(float.IsInfinity(t.Z) || float.IsNaN(t.Z)){return false;}

			return true;
		}

		public void multiply(Vector3 src,out Vector3 dst)
			{dst=(M*src)+t;}

		public void multiplyByInverseRT(Vector3 src,out Vector3 dst)
			{M.multiplyByTranspose(src-t,out dst);}


		public void multiply(ref NxMat34 left,ref NxMat34 right)
		{
			t=(left.M*right.t)+left.t;
			M.multiply(ref left.M,ref right.M);
		}

		public bool getInverse(ref NxMat34 dest)
		{
			bool status=M.getInverse(ref dest.M);
			dest.M.multiply(t * -1,out dest.t); 
			return status;
		}

		public bool getInverseRT(ref NxMat34 dest)
		{
			dest.M.setTransposed(ref M);
			dest.M.multiply(t * -1,out dest.t); 
			return true;
		}


		public void multiplyInverseRTLeft(ref NxMat34 left,ref NxMat34 right)
		{
			t = left.M % (right.t - left.t);
			M.multiplyTransposeLeft(ref left.M,ref right.M);
		}

		public void multiplyInverseRTRight(ref NxMat34 left,ref NxMat34 right)
		{
			M.multiplyTransposeRight(ref left.M,ref right.M);
			t = left.t - (M * right.t);
		}

		public void setColumnMajor44(float[] d) 
		{
			M.setColumnMajorStride4(d);
			t.X = d[12];
			t.Y = d[13];
			t.Z = d[14];
		}

		public void getColumnMajor44(float[] d)
		{
			M.getColumnMajorStride4(d);
			d[12] = t.X;
			d[13] = t.Y;
			d[14] = t.Z;
			d[3] = d[7] = d[11] = 0.0f;
			d[15] = 1.0f;
		}

		public void setRowMajor44(float[] d) 
		{
			M.setRowMajorStride4(d);
			t.X = d[3];
			t.Y = d[7];
			t.Z = d[11];
		}

		public void getRowMajor44(float[] d)
		{
			M.getRowMajorStride4(d);
			d[3] = t.X;
			d[7] = t.Y;
			d[11] = t.Z;
			d[12] = d[13] = d[14] = 0.0f;
			d[15] = 1.0f;
		}






////////////////////////////////////////////////////////////////////////////////////////////////////
//Everything below this is to make NxMat34 act more like DirectX.Matrix and is not part of Novodex//
////////////////////////////////////////////////////////////////////////////////////////////////////		
		public static NxMat34 Identity
		{
			get
			{
				NxMat34 mat;
				mat.M=NxMat33.Identity;
				mat.t=new Vector3(0,0,0);
				return mat;
			}
		}

		public static NxMat34 Zero
		{
			get
			{
				NxMat34 mat;
				mat.M=NxMat33.Zero;
				mat.t=new Vector3(0,0,0);
				return mat;
			}
		}

		static public NxMat34 Translation(Vector3 pos)
		{
			NxMat34 mat=NxMat34.Identity;
			mat.t=pos;
			return mat;
		}

		static public NxMat34 Translation(float x,float y,float z)
		{
			NxMat34 mat=NxMat34.Identity;
			mat.t=new Vector3(x,y,z);
			return mat;
		}
		
		//To match DirectX the angle needs to be negated. I though of flipping it inside the function but it make no sense for RotationX() to be the opposite of rotX()		
		static public NxMat34 RotationX(float angleInRads)
		{
			NxMat34 mat=NxMat34.Identity;
			mat.M.rotX(angleInRads);
			return mat;
		}

		//To match DirectX the angle needs to be negated. I though of flipping it inside the function but it make no sense for RotationY() to be the opposite of rotY()
		static public NxMat34 RotationY(float angleInRads)																				
		{
			NxMat34 mat=NxMat34.Identity;
			mat.M.rotY(angleInRads);
			return mat;
		}

		//To match DirectX the angle needs to be negated. I though of flipping it inside the function but it make no sense for RotationZ() to be the opposite of rotZ()
		static public NxMat34 RotationZ(float angleInRads)
		{
			NxMat34 mat=NxMat34.Identity;
			mat.M.rotZ(angleInRads);
			return mat;
		}

		public static NxMat34 RotationYawPitchRoll(float yawInRads,float pitchInRads,float rollInRads)
		{
			//To make it match DirectX the angles are flipped
			return NxMat34.RotationZ(-rollInRads)*NxMat34.RotationX(-pitchInRads)*NxMat34.RotationY(-yawInRads);
		}

		public static NxMat34 Scaling(float sX,float sY,float sZ)
		{
			NxMat34 mat=NxMat34.Identity;
			mat.M.M11=sX;
			mat.M.M22=sY;
			mat.M.M33=sZ;
			return mat;
		}

		public static NxMat34 Scaling(Vector3 s)
		{
			NxMat34 mat=NxMat34.Identity;
			mat.M.M11=s.X;
			mat.M.M22=s.Y;
			mat.M.M33=s.Z;
			return mat;
		}

		public static NxMat34 LookAtLH(Vector3 cameraPosition,Vector3 cameraTarget,Vector3 cameraUpVector)
		{
			Vector3 zAxis=cameraTarget-cameraPosition;
			zAxis.Normalize();
			Vector3 xAxis=Vector3.Cross(cameraUpVector,zAxis);
			xAxis.Normalize();
			Vector3 yAxis=Vector3.Cross(zAxis,xAxis);
			
			NxMat34 mat;
			mat.M=new NxMat33(xAxis,yAxis,zAxis);
			mat.t=cameraPosition;
			return mat;
		}

		public static NxMat34 LookAtRH(Vector3 cameraPosition,Vector3 cameraTarget,Vector3 cameraUpVector)
		{
			Vector3 zAxis=cameraPosition-cameraTarget;
			zAxis.Normalize();
			Vector3 xAxis=Vector3.Cross(cameraUpVector,zAxis);
			xAxis.Normalize();
			Vector3 yAxis=Vector3.Cross(zAxis,xAxis);
			
			NxMat34 mat;
			mat.M=new NxMat33(xAxis,yAxis,zAxis);
			mat.t=cameraPosition;
			return mat;
		}



		public static NxMat34 operator *(NxMat34 mat1,NxMat34 mat2)
		{
			NxMat34 temp=NxMat34.Zero;
			temp.multiply(ref mat1,ref mat2);
			return temp;
		}



/////////////////////////////////////////////////////////////////
//These were added just because I find them nice to have       //
/////////////////////////////////////////////////////////////////


		public Vector3 getXaxis()
			{return M.getRow(0);}

		public Vector3 getYaxis()
			{return M.getRow(1);}

		public Vector3 getZaxis()
			{return M.getRow(2);}

		public Vector3 getPos()
			{return t;}

		public void setXaxis(Vector3 xAxis)
			{M.setRow(0,xAxis);}

		public void setYaxis(Vector3 yAxis)
			{M.setRow(1,yAxis);}

		public void setZaxis(Vector3 zAxis)
			{M.setRow(2,zAxis);}

		public void setPos(Vector3 pos)
			{t=pos;}

		public Vector3 transformWorldPointIntoLocalSpace(Vector3 worldPoint)
		{
			float x=worldPoint.X-t.X;
			float y=worldPoint.Y-t.Y;
			float z=worldPoint.Z-t.Z;

			float tX=(x*M.M11) + (y*M.M12) + (z*M.M13);
			float tY=(x*M.M21) + (y*M.M22) + (z*M.M23);
			float tZ=(x*M.M31) + (y*M.M32) + (z*M.M33);
			
			return new Vector3(tX,tY,tZ);
		}
		
		public Vector3 transformWorldNormalIntoLocalSpace(Vector3 worldNormal)
		{
			float tX=(worldNormal.X*M.M11) + (worldNormal.Y*M.M12) + (worldNormal.Z*M.M13);
			float tY=(worldNormal.X*M.M21) + (worldNormal.Y*M.M22) + (worldNormal.Z*M.M23);
			float tZ=(worldNormal.X*M.M31) + (worldNormal.Y*M.M32) + (worldNormal.Z*M.M33);
			
			return new Vector3(tX,tY,tZ);
		}

		public Vector3 transformLocalPointIntoWorldSpace(Vector3 localPoint)
		{
			float tX=(localPoint.X*M.M11) + (localPoint.Y*M.M21) + (localPoint.Z*M.M31);
			float tY=(localPoint.X*M.M12) + (localPoint.Y*M.M22) + (localPoint.Z*M.M32);
			float tZ=(localPoint.X*M.M13) + (localPoint.Y*M.M23) + (localPoint.Z*M.M33);

			return new Vector3(tX+t.X,tY+t.Y,tZ+t.Z);
		}

		public Vector3 transformLocalNormalIntoWorldSpace(Vector3 localNormal)
		{
			float tX=(localNormal.X*M.M11) + (localNormal.Y*M.M21) + (localNormal.Z*M.M31);
			float tY=(localNormal.X*M.M12) + (localNormal.Y*M.M22) + (localNormal.Z*M.M32);
			float tZ=(localNormal.X*M.M13) + (localNormal.Y*M.M23) + (localNormal.Z*M.M33);
			
			return new Vector3(tX,tY,tZ);
		}



//These methods are only in the DirectX version.
////////////////////////////////////////////////////////////////////////////
//Implicit conversion operators to make interfacing with DirectX easier   //
////////////////////////////////////////////////////////////////////////////
		public static implicit operator Matrix(NxMat34 mat)
			{return NovodexUtil.convertNxMat34ToMatrix(mat);}

		public static implicit operator NxMat34(Matrix mat)
			{return NovodexUtil.convertMatrixToNxMat34(mat);}
	}
}





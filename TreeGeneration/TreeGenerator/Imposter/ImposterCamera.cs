using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TreeGenerator.Imposter
{
    public class ImposterCamera : ICamera
    {

        #region Fields

        private Matrix mView;
        private Matrix mViewInverse;
        private Matrix mProj;
        private Matrix mViewProj;

        private float mFieldOfView;
        private float mAspect;
        private float mNearZ;
        private float mFarZ;

        private Vector3 mPosition;
        private Vector3 mRight;
        private Vector3 mUp;
        private Vector3 mLook;

        private Plane[] mFrustumPlanes;
        private BoundingFrustum mBoundingFrustum;

        private float mDeltaX;
        private float mDeltaY;
        private float mLastMouseX;
        private float mLastMouseY;

        #endregion

        #region Properties

        public Matrix View
        {
            get { return mView; }
        }

        public Matrix Projection
        {
            get { return mProj; }
            set { mProj = value; }
        }

        public Matrix ViewProj
        {
            get { return mViewProj; }
        }

        public float FOV
        {
            get { return mFieldOfView; }
        }

        public float Aspect
        {
            get { return mAspect; }
        }

        public float NearZ
        {
            get { return mNearZ; }
        }

        public float FarZ
        {
            get { return mFarZ; }
        }

        public Vector3 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public Vector3 Look
        {
            get { return mLook; }
            set { mLook = value; }
        }

        public Vector3 Right
        {
            get { return mRight; }
            set { mRight = value; }
        }

        public Vector3 Up
        {
            get { return mUp; }
            set { mUp = value; }
        }


        #endregion

        public ImposterCamera()
        {
            mView = Matrix.Identity;
            mProj = Matrix.Identity;
            mViewProj = Matrix.Identity;

            mFieldOfView = 0.0f;
	        mAspect = 0.0f;
	        mNearZ  = 0.0f;
	        mFarZ   = 0.0f;

	        mPosition   = new Vector3(0.0f, 0.0f, 0.0f);
	        mRight = new Vector3(1.0f, 0.0f, 0.0f);
	        mUp    = new Vector3(0.0f, 1.0f, 0.0f);
	        mLook  = new Vector3(0.0f, 0.0f, 1.0f);

            mFrustumPlanes = new Plane[6];
            // [0] = near
            // [1] = far
            // [2] = left
            // [3] = right
            // [4] = top
            // [5] = bottom

            mDeltaX = 0.0f;
            mDeltaY = 0.0f;
            mLastMouseX = 0.0f;
            mLastMouseY = 0.0f;
        }

        //copy constructor
        public ImposterCamera(ImposterCamera camera)
        {
            mView = camera.View;
            mProj = camera.Projection;
            mViewProj = camera.ViewProj;

            mFieldOfView = camera.FOV;
            mAspect = camera.Aspect;
            mNearZ = camera.NearZ;
            mFarZ = camera.FarZ;

            mPosition = camera.Position;
            mRight = camera.Right;
            mUp = camera.Up;
            mLook = camera.Look;
        }
        
        public void LookAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            Vector3 L = pos - target;
            L = Vector3.Normalize(L);
        
	        Vector3 R;
            R = Vector3.Cross(up, L);
            //R.Normalize();

	        Vector3 U;
            U = Vector3.Cross(L, R);
            //U.Normalize();

	        mPosition   = pos;
	        mRight = R;
	        mUp    = U;
	        mLook  = L;
        }

        public void LookAt(Vector3 pos, Vector3 target)
        {
            Vector3 L = pos - target;
            L = Vector3.Normalize(L);

            Vector3 R, U;
            if (Math.Abs(Vector3.Dot(mUp, L)) < .5f)
            {
                R = Vector3.Cross(mUp, L);
                U = Vector3.Cross(L, R);
            }
            else
            {
                U = Vector3.Cross(L, mRight);
                R = Vector3.Cross(U, L);
            }

            mPosition = pos;
            mRight = R;
            mUp = U;
            mLook = L;
        }

        public void SetLens(float fov, float aspect, float nearZ, float farZ)
        {
            mFieldOfView = fov;
	        mAspect = aspect;
	        mNearZ  = nearZ;
	        mFarZ   = farZ;

            mProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearZ, farZ);
        }

        public void Place(Vector3 pos, Vector3 look, Vector3 up)
        {
            Vector3 Look = look;
            mLook.Normalize();

            Vector3 Up = up;
            Up.Normalize();

            Vector3 Right = Vector3.Cross(Up, Look);
            Right.Normalize();

            Right = Vector3.Multiply(Right, -1.0f);

            mPosition = pos;
            mRight = Right;
            mUp = Up;
            mLook = Look;
        }

        public void Walk(float units)
        {
            mPosition += mLook * units;
        }

        public void Fly(float units)
        {
            //for fly mode
            mPosition += mUp * units;
        }

        public void Strafe(float units)
        {
            mPosition += new Vector3(mRight.X, 0.0f, mRight.Z) * units;
        }

        public void Pitch(float angle)
        {
            Matrix TransformMatrix = Matrix.CreateFromAxisAngle(mRight, angle);

            // rotate mUp and mLook around mRight vector
            mUp = Vector3.Transform(mUp, TransformMatrix);
            mLook = Vector3.Transform(mLook, TransformMatrix);

            // rotate mUp and mLook around mRight vector
            //mUp.TransformCoordinate(TransformMatrix);
            //mLook.TransformCoordinate(TransformMatrix);
        }

        public void Yaw(float angle)
        {      
            // rotate around world y (0, 1, 0) always for land object
            Matrix TransformMatrix = Matrix.CreateRotationY(angle);
            //TransformMatrix.RotateY(angle);

            // rotate mRight and mLook around mUp or y-axis
            mRight = Vector3.Transform(mRight, TransformMatrix);
            mLook = Vector3.Transform(mLook, TransformMatrix);
            //mRight.TransformCoordinate(TransformMatrix);
            //mLook.TransformCoordinate(TransformMatrix);
        }

        public void UpdateMouse(MouseState state, float units)
        {
            mDeltaX = mLastMouseX - state.X;
            mDeltaY = mLastMouseY - state.Y;

            mLastMouseX = state.X;
            mLastMouseY = state.Y;

            Yaw(mDeltaX * units);
            Pitch(mDeltaY * units);
        }
        
        /// <summary>
        /// Builds the view projection matrix of the camera, and finds the bounding frustum
        /// from this matrix
        /// </summary>
        public void BuildView()
        {
            mLook.Normalize();

            mUp = Vector3.Cross(mLook, mRight);
            mUp.Normalize();

            mRight = Vector3.Cross(mUp, mLook);
            mRight.Normalize();

            // Build the view matrix:
            float x = -Vector3.Dot(mRight, mPosition);
            float y = -Vector3.Dot(mUp, mPosition);
            float z = -Vector3.Dot(mLook, mPosition);

            mView.M11 = mRight.X;
            mView.M21 = mRight.Y;
            mView.M31 = mRight.Z; 
	        mView.M41 = x;

            mView.M12 = mUp.X;
            mView.M22 = mUp.Y;
            mView.M32 = mUp.Z;
	        mView.M42 = y;

            mView.M13 = mLook.X;
            mView.M23 = mLook.Y;
            mView.M33 = mLook.Z; 
	        mView.M43 = z;   

	        mView.M14 = 0.0f;
	        mView.M24 = 0.0f;
	        mView.M34 = 0.0f;
	        mView.M44 = 1.0f;

            mViewProj = mView * mProj;
            mViewInverse = Matrix.Invert(mView);

            //buildFrustumPlanes();
            mBoundingFrustum = new BoundingFrustum(mViewProj);
      
      }



      #region ICamera Members

      public float FarClip
      {
          get { throw new Exception("The method or operation is not implemented."); }
      }

      public float NearClip
      {
          get { throw new Exception("The method or operation is not implemented."); }
      }

      public Matrix ViewInverse
      {
          get { return mViewInverse; }
      }

      public Matrix ViewProjection
      {
          get { return mViewProj; }
      }

      #endregion
  }
}

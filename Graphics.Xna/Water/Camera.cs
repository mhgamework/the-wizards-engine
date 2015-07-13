using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Graphics.Xna.Water
{
    public class Camera
    {
        #region Fields
        public static int LAND = 1;
        public static int FLY = 2;

        private Matrix mView;
        private Matrix mProj;
        private Matrix mViewProj;

        private float mFieldOfView;
        private float mAspect;
        private float mNearZ;
        private float mFarZ;

        private float mRFactor;
        private float mUFactor;

        private bool mIsLeaning;
        private bool mIsJumping;
        private bool mIsUp;
        private float mJumpMax;
        private float mFloor;
        private float mVelocity;
        private float mAcceleration;
        private int mCameraType = 0;

        private Vector3 mPosition;
        private Vector3 mRight;
        private Vector3 mUp;
        private Vector3 mLook;

        private Plane[] mFrustumPlanes;
        #endregion

        public int CameraMode { get { return mCameraType; } set { mCameraType = value; } }

        public Camera( int type )
        {
            mCameraType = type;

            mView = Matrix.Identity;
            mProj = Matrix.Identity;
            mViewProj = Matrix.Identity;

            mFieldOfView = 0.0f;
            mAspect = 0.0f;
            mNearZ = 0.0f;
            mFarZ = 0.0f;

            mPosition = new Vector3( 0.0f, 0.0f, 0.0f );
            mRight = new Vector3( 1.0f, 0.0f, 0.0f );
            mUp = new Vector3( 0.0f, 1.0f, 0.0f );
            mLook = new Vector3( 0.0f, 0.0f, 1.0f );

            mIsLeaning = false;
            mIsJumping = false;
            mIsUp = false;
            mFloor = 0;
            mJumpMax = 0;
            mVelocity = 0;
            mAcceleration = 0.15f;

            mFrustumPlanes = new Plane[ 6 ];
            // [0] = near
            // [1] = far
            // [2] = left
            // [3] = right
            // [4] = top
            // [5] = bottom
        }

        public Matrix View
        {
            get { return mView; }
        }

        public Matrix Projection
        {
            get { return mProj; }
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

        public Vector3 Look()
        {
            //mLook.Normalize();
            return mLook;
        }
        public Vector3 Right()
        {
            return mRight;
        }

        public void LookAt( Vector3 pos, Vector3 target, Vector3 up )
        {
            Vector3 L = target - pos;
            L = Vector3.Normalize( L );

            Vector3 R;
            R = Vector3.Cross( up, L );
            // Since 'up' and 'L' are unit vectors, 'R' will be a
            // unit vector also.

            Vector3 U;
            U = Vector3.Cross( L, R );
            // Since 'L' and 'right' are unit vectors, 'U' will be a
            // unit vector also.

            mPosition = pos;
            mRight = R;
            mUp = U;
            mLook = L;
        }

        public void SetLens( float fov, float aspect, float nearZ, float farZ )
        {
            mFieldOfView = fov;
            mAspect = aspect;
            mNearZ = nearZ;
            mFarZ = farZ;

            mRFactor = (float)Math.Tan( fov / 2 );
            mUFactor = mRFactor * aspect;

            //mProj = Matrix.PerspectiveFovLH( fov, aspect, nearZ, farZ );
            mProj = Matrix.CreatePerspectiveFieldOfView( fov, aspect, nearZ, farZ );
        }

        public float Floor
        {
            get { return mFloor; }
            set { mFloor = value; }
        }

        public void Walk( float units )
        {
            if ( mCameraType == FLY )
                mPosition += mLook * units;

            else //only move on xz plane for ground
                mPosition += new Vector3( mLook.X, 0.0f, mLook.Z ) * units;
        }

        public void Fly( float units )
        {
            //for fly mode
            mPosition += mUp * units;
        }

        public void Strafe( float units )
        {
            mPosition += new Vector3( mRight.X, 0.0f, mRight.Z ) * units;
        }

        public void Pitch( float angle )
        {
            Matrix TransformMatrix = new Matrix();
            //TransformMatrix.RotateAxis( mRight, angle );
            TransformMatrix = Matrix.CreateFromAxisAngle( mRight, angle );

            // rotate mUp and mLook around mRight vector
            //mUp.TransformCoordinate( TransformMatrix );
            mUp = Vector3.Transform( mUp, TransformMatrix );
            //mLook.TransformCoordinate( TransformMatrix );
            mLook = Vector3.Transform( mLook, TransformMatrix );

        }

        public void Yaw( float angle )
        {
            Matrix TransformMatrix = new Matrix();

            // rotate around world y (0, 1, 0) always for land object
            TransformMatrix = Matrix.CreateRotationY( angle );

            // rotate mRight and mLook around mUp or y-axis
            mRight = Vector3.Transform( mRight, TransformMatrix );
            mLook = Vector3.Transform( mLook, TransformMatrix );
        }

        public void Roll( float angle )
        {
            if ( angle > 0 )
                mIsLeaning = true;
            else
                mIsLeaning = false;

            if ( mIsLeaning )
            {
                Matrix T;
                T = Matrix.CreateFromAxisAngle( mLook, angle );

                // rotate _up and _right around _look vector
                mRight = Vector3.Transform( mRight, T );
                //D3DXVec3TransformCoord(&_right,&_right, &T);
                mUp = Vector3.Transform( mUp, T );
                //D3DXVec3TransformCoord(&_up,&_up, &T);
            }
        }

        public void Jump()
        {
            if ( !mIsJumping )
            {
                mJumpMax = 45 + mFloor;
                mVelocity = mFloor;
                mIsJumping = true;
                mIsUp = true;
            }
        }

        public void Update( float elapsedTime )
        {
            if ( mIsJumping )
            {
                if ( mVelocity < mJumpMax && mIsUp )
                    mVelocity += mAcceleration + ( ( elapsedTime * 90.0f ) * ( elapsedTime * 90.0f ) );
                else
                {
                    mIsUp = false;
                    mVelocity -= mAcceleration + ( ( elapsedTime * 90.0f ) * ( elapsedTime * 90.0f ) );
                }

                mPosition.Y = mVelocity;
            }
            if ( mVelocity <= mFloor )
            {
                mPosition.Y = mFloor;
                mIsJumping = false;
            }
        }

        public void BuildView()
        {
            mLook.Normalize();

            mUp = Vector3.Cross( mLook, mRight );
            mUp.Normalize();

            mRight = Vector3.Cross( mUp, mLook );
            mRight.Normalize();

            // Build the view matrix:
            float x = -Vector3.Dot( mRight, mPosition );
            float y = -Vector3.Dot( mUp, mPosition );
            float z = -Vector3.Dot( mLook, mPosition );

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

            buildFrustumPlanes();
        }

        public bool IsBoundingBoxVisible( BoundingBox box )
        {
            BoundingFrustum frustum = new BoundingFrustum( ViewProj );
            return frustum.Contains( box ) != ContainmentType.Disjoint;

            //      N  *Q                    *P
            //      | /                     /
            //      |/                     /
            // -----/----- Plane     -----/----- Plane    
            //     /                     / |
            //    /                     /  |
            //   *P                    *Q  N
            //
            // PQ forms diagonal most closely aligned with plane normal.

            Vector3 Q;
            //Vector3 P = new Vector3();

            for ( int i = 0; i < 5; i++ )
            {
                //P.X = mFrustumPlanes[i].A >= 0 ? box.min.X : box.max.X;
                //Q.X = mFrustumPlanes[i].A >= 0 ? box.Max.X : box.Min.X;
                Q.X = mFrustumPlanes[ i ].Normal.X >= 0 ? box.Max.X : box.Min.X;

                //P.Y = mFrustumPlanes[i].B >= 0 ? box.min.Y : box.max.Y;
                //Q.Y = mFrustumPlanes[ i ].B >= 0 ? box.Max.Y : box.Min.Y;
                Q.Y = mFrustumPlanes[ i ].Normal.Y >= 0 ? box.Max.Y : box.Min.Y;

                //P.Z = mFrustumPlanes[i].C >= 0 ? box.min.Z : box.max.Z;
                //Q.Z = mFrustumPlanes[ i ].C >= 0 ? box.Max.Z : box.Min.Z;
                Q.Z = mFrustumPlanes[ i ].Normal.Z >= 0 ? box.Max.Z : box.Min.Z;


                //PQ points roughly in the direction of the plane normal
                //therefore we only need to test for Q
                //if (mFrustumPlanes[i].Dot(Q) < 0.0f)
                if ( mFrustumPlanes[ i ].Dot( new Vector4( Q, 1 ) ) < 0.0f )
                    return false;
            }

            /*for (byte p = 0; p < 5; p++)
            {
                if (mFrustumPlanes[p].Dot(new Vector3(box.min.X, box.min.Y, box.min.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.max.X, box.min.Y, box.min.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.min.X, box.max.Y, box.min.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.max.X, box.max.Y, box.min.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.min.X, box.min.Y, box.max.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.max.X, box.min.Y, box.max.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.min.X, box.max.Y, box.max.Z)) >= 0.0f)
                    continue;
                if (mFrustumPlanes[p].Dot(new Vector3(box.max.X, box.max.Y, box.max.Z)) >= 0.0f)
                    continue;

                return false;
            }*/

            return true;
        }

        public bool IsBoundingBoxVisible2( Vector3 min, Vector3 max )
        {
            Vector3 P;
            int nOutOfLeft = 0, nOutOfRight = 0, nOutOfFar = 0, nOutOfNear = 0, nOutOfTop = 0, nOutOfBottom = 0;
            bool bIsInRightTest, bIsInUpTest, bIsInFrontTest;

            Vector3[] Corners = new Vector3[ 2 ];
            Corners[ 0 ] = min - mPosition;
            Corners[ 1 ] = max - mPosition;

            for ( int i = 0; i < 8; ++i )
            {
                bIsInRightTest = bIsInUpTest = bIsInFrontTest = false;
                P.X = Corners[ i & 1 ].X;
                P.Y = Corners[ ( i >> 2 ) & 1 ].Y;
                P.Z = Corners[ ( i >> 1 ) & 1 ].Z;

                float r = Vector3.Dot( mRight, P );
                float u = Vector3.Dot( mUp, P );
                float f = Vector3.Dot( mLook, P );

                //float r=RightVector.x*P.x + RightVector.y*P.y + RightVector.z*P.z;
                //float u=UpVector.x*P.x + UpVector.y*P.y + UpVector.z*P.z;
                //float f=ForwardVector.x*P.x + ForwardVector.y*P.y + ForwardVector.z*P.z;

                if ( r < -mRFactor * f )
                    ++nOutOfLeft;
                else if ( r > mRFactor * f )
                    ++nOutOfRight;
                else
                    bIsInRightTest = true;

                if ( u < -mUFactor * f )
                    ++nOutOfBottom;
                else if ( u > mUFactor * f )
                    ++nOutOfTop;
                else
                    bIsInUpTest = true;

                if ( f < NearZ )
                    ++nOutOfNear;
                else if ( f > FarZ )
                    ++nOutOfFar;
                else
                    bIsInFrontTest = true;

                if ( bIsInRightTest && bIsInFrontTest && bIsInUpTest ) return true;
            }

            if ( nOutOfLeft == 8 || nOutOfRight == 8 || nOutOfFar == 8 || nOutOfNear == 8
                || nOutOfTop == 8 || nOutOfBottom == 8 )
                return false;

            return true;
        }

        private void buildFrustumPlanes()
        {
            Vector4 col0 = new Vector4( mViewProj.M11, mViewProj.M21, mViewProj.M31, mViewProj.M41 );
            Vector4 col1 = new Vector4( mViewProj.M12, mViewProj.M22, mViewProj.M32, mViewProj.M42 );
            Vector4 col2 = new Vector4( mViewProj.M13, mViewProj.M23, mViewProj.M33, mViewProj.M43 );
            Vector4 col3 = new Vector4( mViewProj.M14, mViewProj.M24, mViewProj.M34, mViewProj.M44 );

            Vector4 temp;

            //right
            temp = col3 - col0;
            mFrustumPlanes[ 0 ] = new Plane( temp.X, temp.Y, temp.Z, temp.W );

            //left
            temp = col3 + col0;
            mFrustumPlanes[ 1 ] = new Plane( temp.X, temp.Y, temp.Z, temp.W );

            //top
            temp = col3 - col1;
            mFrustumPlanes[ 2 ] = new Plane( temp.X, temp.Y, temp.Z, temp.W );


            //bottom
            temp = col3 + col1;
            mFrustumPlanes[ 3 ] = new Plane( temp.X, temp.Y, temp.Z, temp.W );

            //far
            temp = col3 - col2;
            mFrustumPlanes[ 4 ] = new Plane( temp.X, temp.Y, temp.Z, temp.W );



            //Vector4 temp;
            ////near
            // mFrustumPlanes[0] = new Plane(col2.X, col2.Y, col2.Z, col2.W);

            ////far
            //temp = col3 - col2;
            //mFrustumPlanes[1] = new Plane(temp.X, temp.Y, temp.Z, temp.W);

            //left
            //temp = col3 + col0;
            //mFrustumPlanes[2] = new Plane(temp.X, temp.Y, temp.Z, temp.W);

            //right
            //temp = col3 - col0;
            //mFrustumPlanes[3] = new Plane(temp.X, temp.Y, temp.Z, temp.W);

            //top
            //temp = col3 - col1;
            //mFrustumPlanes[4] = new Plane(temp.X, temp.Y, temp.Z, temp.W);

            //bottom
            //temp = col3 + col1;
            //mFrustumPlanes[5] = new Plane(temp.X, temp.Y, temp.Z, temp.W);

            for ( int i = 0; i < 5; i++ )
                mFrustumPlanes[ i ].Normalize();
        }
    }
}

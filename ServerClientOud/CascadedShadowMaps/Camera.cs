//======================================================================
//
//	Common Sample Framework
//
//		by MJP (mpettineo@gmail.com)
//		09/20/08
//
//======================================================================
//
//	File:		Camera.cs
//
//	Desc:		Defines several classes for handling a 3D camera.
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace SampleCommon
{
	/// <summary>
	/// Abstract base class for all camera types
	/// </summary>
    public abstract class Camera
    {
        protected Matrix viewMatrix = Matrix.Identity;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Matrix projectionMatrix = Matrix.Identity;
        protected Matrix viewProjMatrix = Matrix.Identity;

        protected BoundingFrustum boundingFrustum;

        protected float nearClip;
        protected float farClip;

        public void GetWorldMatrix(out Matrix worldMatrix)
        {
            worldMatrix = this.worldMatrix;    
        }

		public void SetWorldMatrix(ref Matrix worldMatrix)
		{
			this.worldMatrix = worldMatrix;
			Update();
		}
        
        public void GetViewMatrix(out Matrix viewMatrix)
        {
			viewMatrix = this.viewMatrix;
        }

		public void SetViewMatrix(ref Matrix viewMatrix)
		{
			this.viewMatrix = viewMatrix;
            Matrix.Invert(ref viewMatrix, out worldMatrix);
			Update();
		}

        public void GetProjectionMatrix(out Matrix projectionMatrix)
        {
			projectionMatrix = this.projectionMatrix;
        }

        public void GetViewProjMatrix(out Matrix viewProjMatrix)
        {
            viewProjMatrix = this.viewProjMatrix;
        }

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set 
            { 
                worldMatrix = value;
                Update();
            }
        }

		public Matrix ViewMatrix
		{
			get { return viewMatrix; }
			set 
            { 
                viewMatrix = value;
                Matrix.Invert(ref viewMatrix, out worldMatrix);
                Update();
            }
		}

		public Matrix ProjectionMatrix
		{
			get { return projectionMatrix; }
			set { projectionMatrix = value; }
		}

		public Matrix ViewProjectionMatrix
		{
			get { return viewProjMatrix; }
			set { viewProjMatrix = value; }
		}

        public virtual float NearClip
        {
            get { return nearClip; }
            set { }
        }

        public virtual float FarClip
        {
            get { return farClip; }
            set { }
        }

		public Vector3 Position
		{
			get { return worldMatrix.Translation; }
            set
            {
                worldMatrix.Translation = value;
                Update();
            }
		}

		public BoundingFrustum BoundingFrustum
		{
			get { return boundingFrustum; }
		}

        public Quaternion Orientation
        {
            get
            {
                Quaternion orientation;
                Quaternion.CreateFromRotationMatrix(ref worldMatrix, out orientation);
                return orientation;
            }
            set
            {
                Quaternion orientation = value;
                Vector3 position = worldMatrix.Translation;
                Matrix.CreateFromQuaternion(ref orientation, out worldMatrix);
                worldMatrix.Translation = position;
                Update();
            }
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        public Camera()
        {
            boundingFrustum = new BoundingFrustum(viewProjMatrix);
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.Identity;
        }

		/// <summary>
		/// Applies a transform to the camera's world matrix,
		/// with the new transform applied first
		/// </summary>
		/// <param name="transform">The transform to be applied</param>
        public void PreTransform(ref Matrix transform)
        {
            Matrix.Multiply(ref transform, ref worldMatrix, out worldMatrix);
            Update();
        }

		/// <summary>
		/// Applies a transform to the camera's world matrix,
		/// with the new transform applied second
		/// </summary>
		/// <param name="transform">The transform to be applied</param>
        public void PostTransform(ref Matrix transform)
        {
            Matrix.Multiply(ref worldMatrix, ref transform, out worldMatrix);
            Update();
        }

        /// <summary>
        /// Updates the view-projection matrix and frustum coordinates based on
        /// the current camera position/orientation and projection parameters.
        /// </summary>
        protected void Update()
        {
            // Make our view matrix
            Matrix.Invert(ref worldMatrix, out viewMatrix);

            // Create the combined view-projection matrix
            Matrix.Multiply(ref viewMatrix, ref projectionMatrix, out viewProjMatrix);

            // Create the bounding frustum
            boundingFrustum.Matrix = viewProjMatrix;
        }
    }

	/// <summary>
	/// Camera that uses an orthographic projection
	/// </summary>
    public class OrthographicCamera : Camera
    {
        float width;
        float height;

        float xMin;
        float xMax;
        float yMin;
        float yMax;

        public float Width
        {
            get { return width; }
        }

        public float Height
        {
            get { return height; }
        }

        public float XMin
        {
            get { return xMin; }
            set
            {
                xMin = value;
                width = xMax - xMin;
                Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public float XMax
        {
            get { return xMax; }
            set
            {
                xMax = value;
                width = xMax - xMin;
                Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public float YMin
        {
            get { return xMin; }
            set
            {
                yMin = value;
                height = yMax - yMin;
                Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public float YMax
        {
            get { return xMin; }
            set
            {
                yMax = value;
                height = yMax - yMin;
                Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public override float NearClip
        {
            get { return nearClip; }
            set
            {
                nearClip = value;
                Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public override float FarClip
        {
            get { return farClip; }
            set
            {
                farClip = value;
                Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        /// <summary>
        /// Creates a camera using an orthographic projection
        /// </summary>
        /// <param name="width">Width of the projection volume</param>
        /// <param name="height">Height of the projection volume</param>
        /// <param name="nearClip">Distance to near clip plane</param>
        /// <param name="farClip">Distance to far clip plane</param>
        public OrthographicCamera(float width, float height, float nearClip, float farClip)
            : base()
        {
            this.width = width;
            this.height = height;
            this.nearClip = nearClip;
            this.farClip = farClip;
            this.xMax = width / 2;
            this.yMax = height / 2;
            this.xMin = -width / 2;
            this.yMin = -height / 2;
            Matrix.CreateOrthographic(width, height, nearClip, farClip, out projectionMatrix);
            Update();
        }

        public OrthographicCamera(float xMin, float xMax, float yMin, float yMax, float nearClip, float farClip)
            : base()
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;
            this.width = xMax - xMin;
            this.height = yMax - yMin;
            this.nearClip = nearClip;
            this.farClip = farClip;
            Matrix.CreateOrthographicOffCenter(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
            Update();

            Debug.Assert(xMax > xMin && yMax > yMin, "Invalid ortho camera params");
        }
    }

	/// <summary>
	/// Camera using a perspective projection
	/// </summary>
    public class PerspectiveCamera : Camera
    {
        protected float fieldOfView;
        protected float aspectRatio;

        public float FieldOfView
        {
            get { return fieldOfView; }
			set
			{
				fieldOfView = value;
                Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip, out projectionMatrix);
				Update();
			}
        }

        public float AspectRatio
        {
            get { return aspectRatio; }
			set
			{
				aspectRatio = value;
                Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip, out projectionMatrix);
				Update();
			}
        }

		public override float NearClip
		{
			get { return nearClip; }
			set
			{
				nearClip = value;
                Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip, out projectionMatrix);
				Update();
			}
		}

		public override float FarClip
		{
			get { return farClip; }
			set
			{
				farClip = value;
                Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip, out projectionMatrix);
				Update();
			}
		}

        /// <summary>
        /// Creates a camera using a perspective projection
        /// </summary>
        /// <param name="fieldOfView">The vertical field of view</param>
        /// <param name="aspectRatio">Aspect ratio of the projection</param>
        /// <param name="nearClip">Distance to near clipping plane</param>
        /// <param name="farClip">Distance to far clipping plane</param>
        public PerspectiveCamera(float fieldOfView, float aspectRatio, float nearClip, float farClip)
            : base()
        {
            this.fieldOfView = fieldOfView;
            this.aspectRatio = aspectRatio;
            this.nearClip = nearClip;
            this.farClip = farClip;
            Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip, out projectionMatrix);
            Update();
        }

    }

    public class FirstPersonCamera : PerspectiveCamera
    {
        protected float xRotation;
        protected float yRotation;
        protected Matrix baseOrientation = Matrix.Identity;

        public float XRotation
        {
            get { return xRotation; }
            set
            {
                xRotation = value;
                if (xRotation > Math.PI / 2.0f)
                    xRotation = (float)Math.PI / 2.0f;
                else if (xRotation < -Math.PI / 2.0f)
                    xRotation = (float)-Math.PI / 2.0f; 
                Matrix rotationMatrix, translationMatrix;
                Matrix.CreateFromYawPitchRoll(yRotation, xRotation, 0, out rotationMatrix);
                translationMatrix = Matrix.CreateTranslation(Position);
                WorldMatrix = rotationMatrix * baseOrientation * translationMatrix;
            }
        }

        public float YRotation
        {
            get { return yRotation; }
            set
            {
                yRotation = value;
                Matrix rotationMatrix, translationMatrix;
                Matrix.CreateFromYawPitchRoll(yRotation, xRotation, 0, out rotationMatrix);
                translationMatrix = Matrix.CreateTranslation(Position);
                WorldMatrix = rotationMatrix * baseOrientation * translationMatrix;
            }
        }

        public void SetBaseOrientation()
        {
            baseOrientation = worldMatrix;
            baseOrientation.Translation = Vector3.Zero;
            XRotation = xRotation;
            YRotation = yRotation;
        }

        public FirstPersonCamera(float fieldOfView, float aspectRatio, float nearClip, float farClip)
            : base(fieldOfView, aspectRatio, nearClip, farClip)
        {      
        }

    }
}

using System.Diagnostics;
using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Rendering.CSM
{
    public class OrthographicCamera : ICamera
    {

        protected Matrix viewMatrix = Matrix.Identity;
        protected Matrix worldMatrix = Matrix.Identity;
        protected Matrix projectionMatrix = Matrix.Identity;
        protected Matrix viewProjMatrix = Matrix.Identity;

        protected BoundingFrustum boundingFrustum;

        protected float nearClip;
        protected float farClip;



        /// <summary>
        /// Updates the view-projection matrix and frustum coordinates based on
        /// the current camera position/orientation and projection parameters.
        /// </summary>
        protected void Update()
        {
            // Make our view matrix
            worldMatrix = Matrix.Invert( viewMatrix );
            

            // Create the combined view-projection matrix
            Matrix.Multiply( ref viewMatrix, ref projectionMatrix, out viewProjMatrix );

            // Create the bounding frustum
            boundingFrustum.Matrix = viewProjMatrix;
        }

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
                Matrix.OrthoOffCenterRH( xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix );
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
                Matrix.OrthoOffCenterRH(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
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
                Matrix.OrthoOffCenterRH(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
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
                Matrix.OrthoOffCenterRH(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public float NearClip
        {
            get { return nearClip; }
            set
            {
                nearClip = value;
                Matrix.OrthoOffCenterRH(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
                Update();
            }
        }

        public float FarClip
        {
            get { return farClip; }
            set
            {
                farClip = value;
                Matrix.OrthoOffCenterRH(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
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
        public OrthographicCamera( float width, float height, float nearClip, float farClip )
        {
            boundingFrustum = new BoundingFrustum( viewProjMatrix );
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.Identity;
            this.width = width;
            this.height = height;
            this.nearClip = nearClip;
            this.farClip = farClip;
            this.xMax = width / 2;
            this.yMax = height / 2;
            this.xMin = -width / 2;
            this.yMin = -height / 2;
            Matrix.OrthoRH( width, height, nearClip, farClip, out projectionMatrix );
            Update();
        }

        public OrthographicCamera( float xMin, float xMax, float yMin, float yMax, float nearClip, float farClip )
        {
            boundingFrustum = new BoundingFrustum( viewProjMatrix );
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.Identity;
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;
            this.width = xMax - xMin;
            this.height = yMax - yMin;
            this.nearClip = nearClip;
            this.farClip = farClip;
            Matrix.OrthoOffCenterRH(xMin, xMax, yMin, yMax, nearClip, farClip, out projectionMatrix);
            Update();

            Debug.Assert( xMax > xMin && yMax > yMin, "Invalid ortho camera params" );
        }

        #region ICamera Members

        public Matrix View
        {
            get { return viewMatrix; }
            set { viewMatrix = value; Update(); }
        }

        public Matrix Projection
        {
            get { return projectionMatrix; }
        }

        public Matrix ViewProjection
        {
            get { return viewProjMatrix; }
        }

        public Matrix ViewInverse
        {
            get { return worldMatrix; }
        }

        #endregion
    }
}

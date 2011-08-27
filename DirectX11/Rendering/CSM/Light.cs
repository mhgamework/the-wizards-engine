//========================================================================
//
//	Common Sample Framework
//
//		by MJP (mpettineo@gmail.com)
//		09/20/08
//
//========================================================================
//
//	File:		Light.cs
//
//	Desc:		Defines several classes for various types of light sources
//
//========================================================================

using System;
using SlimDX;

namespace DirectX11.Rendering.CSM
{

    /// <summary>
    /// Abstract base class for all lights
    /// </summary>
    public abstract class Light
    {
        protected Vector3 color = Vector3.Zero;
        protected Matrix worldMatrix = Matrix.Identity;

        /// <summary>
        /// Gets or sets the color of the light
        /// </summary>
        public Vector3 Color
        {
            get { return color; }
            set { color = value; }
        }

        public Light()
        {
        }

        /// <summary>
        /// Gets the world matrix for the light
        /// </summary>
        /// <param name="worldMatrix">Receives the world matrix</param>
        public void GetWorldMatrix(out Matrix worldMatrix)
        {
            worldMatrix = this.worldMatrix;
        }

        /// <summary>
        /// Sets the world matrix for the light
        /// </summary>
        /// <param name="worldMatrix">The matrix to use as the world matrix</param>
        public void SetWorldMatrix(ref Matrix worldMatrix)
        {
            this.worldMatrix = worldMatrix;
        }
    }

    /// <summary>
    /// A point(omni) light source
    /// </summary>
    public class PointLight : Light
    {
        protected float range = 0.0f;

        /// <summary>
        /// Gets or sets the position of the light
        /// </summary>
        public Vector3 Position
        {
            get { return worldMatrix.GetTranslation(); }
            set
            {
                throw new NotImplementedException(); // worldMatrix.Translation = value;
            }
        }

        /// <summary>
        /// Gets or sets the range of the light
        /// </summary>
        public float Range
        {
            get { return range; }
            set { range = value; }
        }

        public PointLight()
            : base()
        {

        }
    }

    /// <summary>
    /// A spotlight
    /// </summary>
    public class SpotLight : PointLight
    {
        protected float innerCone;
        protected float outerCone;

        /// <summary>
        /// Gets or sets the width of the inner cone, in radians
        /// </summary>
        public float InnerCone
        {
            get { return innerCone; }
            set { innerCone = value; }
        }

        /// <summary>
        /// Gets or sets the width of the outer cone, in radians
        /// </summary>
        public float OuterCone
        {
            get { return outerCone; }
            set { outerCone = value; }
        }

        ///// <summary>
        ///// Gets or sets the direction of the light
        ///// </summary>
        //public Vector3 Direction
        //{
        //    get { return worldMatrix.Forward; }
        //    set
        //    {
        //        Vector3 position = worldMatrix.Translation;
        //        Vector3 forward = value;
        //        Vector3 up = Vector3.Up;
        //        if (forward == Vector3.Up || forward == Vector3.Down)
        //        {
        //            Vector3 right = Vector3.Right;
        //            Vector3.Cross(ref right, ref forward, out up);
        //        }
        //        Matrix.CreateWorld(ref position, ref forward, ref up, out worldMatrix);
        //    }
        //}

        public SpotLight()
            : base()
        {
        }
    }

    /// <summary>
    /// A directional(infinite) light source
    /// </summary>
    public class DirectionalLight : Light
    {
        /// <summary>
        /// Gets or sets the direction of the light
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                //return worldMatrix.Forward;
                return Vector3.TransformNormal(MathHelper.Forward, worldMatrix);
            }
            set
            {
                Vector3 position = worldMatrix.GetTranslation();
                Vector3 forward = value;
                Vector3 up = MathHelper.Up;
                if (forward == MathHelper.Up || forward == MathHelper.Down)
                {
                    Vector3 right = MathHelper.Right;
                    Vector3.Cross(ref right, ref forward, out up);
                }
                //TODO: check
                Matrix.LookAtRH(ref position, ref forward, ref up, out worldMatrix);

                worldMatrix = Matrix.Invert(worldMatrix);
            }
        }

        public DirectionalLight()
            : base()
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface ICamera
    {
        Matrix View { get;}
        Matrix Projection { get;}
        Matrix ViewProjection { get;}
        /// <summary>
        /// This could also be called the camera's world matrix
        /// </summary>
        Matrix ViewInverse { get;}
        float NearClip { get;}
        float FarClip { get;}
    }
}

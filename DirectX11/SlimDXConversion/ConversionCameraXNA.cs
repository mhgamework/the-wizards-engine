using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11.Graphics;

namespace MHGameWork.TheWizards
{
 public    class ConversionCameraXNA: MHGameWork.TheWizards.ServerClient.ICamera , ICamera
    {
     private readonly ICamera cam;

     public ConversionCameraXNA(ICamera cam)
     {
         this.cam = cam;
     }

     #region ICamera Members

        SlimDX.Matrix ICamera.View
        {
            get { return cam.View;   }
        }

        SlimDX.Matrix ICamera.Projection
        {
            get { return cam.Projection; }
        }

        SlimDX.Matrix ICamera.ViewProjection
        {
            get { return cam.ViewProjection; }
        }

        SlimDX.Matrix ICamera.ViewInverse
        {
            get { return cam.ViewInverse; }
        }

        float ICamera.NearClip
        {
            get { return cam.NearClip; }
        }

        float ICamera.FarClip
        {
            get { return cam.FarClip; }
        }

        #endregion

        #region ICamera Members

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.View
        {
            get { return cam.View.xna();   }
        }

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.Projection
        {
            get { return cam.Projection.xna(); }
        }

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.ViewProjection
        {
            get { return cam.ViewProjection.xna(); }
        }

        Microsoft.Xna.Framework.Matrix ServerClient.ICamera.ViewInverse
        {
            get { return cam.ViewInverse.xna(); }
        }

        float ServerClient.ICamera.NearClip
        {
            get { return cam.NearClip; }
        }

        float ServerClient.ICamera.FarClip
        {
            get { return cam.FarClip; }
        }

        #endregion
    }
}

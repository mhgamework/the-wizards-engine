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
        Matrix ViewInverse { get;}
    }
}

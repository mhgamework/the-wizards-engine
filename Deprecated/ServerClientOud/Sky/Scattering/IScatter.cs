using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Data;
using System.Windows.Forms;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Sky.Scattering
{

    public interface IScatter
    {
        void BeginSkyLightRender( IXNAGame game, Camera camera, Matrix wvp, Vector3 sunDirection );
        void EndSkyLightRender();

        void BeginAerialPerspectiveRender( IXNAGame game, Camera camera, Matrix world, Vector3 sunDirection, bool useWaterTech );
        void EndAerialPerspectiveRender();

        Vector4 SunColorAndIntensity
        {
            get;
        }

        float Inscattering { get;set;}
        float Mie { get;set;}
        float Rayleigh { get;set;}
    }
}

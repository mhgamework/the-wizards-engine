using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Sky.Scattering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Sky
{
    public class SkyRenderer : TWClient.IRenderer
    {
        SkyModel skyModel;

        Camera camTemp = new Camera( Camera.FLY );
        SpectaterCamera specCam;

        public SkyRenderer( TheWizards.Database.Database _database )
        {
            _database.FindService<TWClient.RendererService>().AddIRenderer( this );

        }

        #region IRenderer Members

        public void Render( XNAGame game )
        {
            camTemp.LookAt( specCam.CameraPosition, specCam.CameraPosition + specCam.CameraDirection, Vector3.Up );
            camTemp.BuildView();
            skyModel.Render( game, camTemp, new Plane() );
        }

        public void Update( XNAGame game )
        {
        }

        public void Initialize( XNAGame game )
        {
            specCam = (SpectaterCamera)game.Camera;
            Texture2D[] textures = new Texture2D[ 0 ];
            //use a sky model with a Hoffman-Preethem scattering method
            //SkyModel = new SkyModel(game, 2000, typeof( Hoffman_Preethem ), mTerrain.Textures );
            skyModel = new SkyModel( game, 2000, typeof( Hoffman_Preethem ), textures );
        }

        #endregion
    }
}

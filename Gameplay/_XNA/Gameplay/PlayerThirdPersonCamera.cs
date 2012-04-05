using System;
using DirectX11.Graphics;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Player;
using ICamera = MHGameWork.TheWizards.ServerClient.ICamera;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public class PlayerThirdPersonCamera : ICamera, IXNAObject
    {
        private ThirdPersonCamera camera = new ThirdPersonCamera();
        public bool Enabled;
        public float LookAngleHorizontal;

        public PlayerThirdPersonCamera(IXNAGame _game, PlayerData _player)
        {
            throw new NotImplementedException();
        }
        

        #region ICamera Members

        public Microsoft.Xna.Framework.Matrix View
        {
            get { return camera.View.xna(); }
        }

        public Microsoft.Xna.Framework.Matrix Projection
        {
            get { return camera.Projection.xna(); }
        }

        public Microsoft.Xna.Framework.Matrix ViewProjection
        {
            get { return camera.ViewProjection.xna(); }
        }

        public Microsoft.Xna.Framework.Matrix ViewInverse
        {
            get { return camera.ViewInverse.xna(); }
        }

        public float NearClip
        {
            get { return camera.NearClip; }
        }

        public float FarClip
        {
            get { return camera.FarClip; }
        }

        #endregion

        #region IXNAObject Members

        public void Initialize(IXNAGame _game)
        {
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {

        }

        #endregion
    }
}

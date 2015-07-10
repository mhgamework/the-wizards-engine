using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    public class CustomCamera : ICamera
    {
        private IXNAGame game;


        private bool enabled = true;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        private Matrix view;
        public Matrix View
        {
            get { return view; }
        }

        private Matrix projection;
        public Matrix Projection
        {
            get { return projection; }
        }

        private Matrix viewProjection;
        public Matrix ViewProjection
        {
            get { return viewProjection; }
        }

        private Matrix viewInverse;
        public Matrix ViewInverse
        {
            get { return viewInverse; }
        }

        private float nearClip;
        public float NearClip
        {
            get { return nearClip; }
            set { nearClip = value; }
        }

        private float farClip;
        public float FarClip
        {
            get { return farClip; }
            set { farClip = value; }
        }

        public CustomCamera( IXNAGame nGame )
        {
            game = nGame;
            projection = Matrix.Identity;
            view = Matrix.Identity;
            viewInverse = Matrix.Identity;
            viewProjection = Matrix.Identity;
            nearClip = 1.0f;
            farClip = 1000f;


        }

        public void SetViewMatrix( Matrix nView )
        {
            view = nView;
            viewInverse = Matrix.Invert( view );
            viewProjection = view * projection;

        }
        public void SetProjectionMatrix( Matrix nProj )
        {
            projection = nProj;
            viewProjection = view * projection;

        }
        public void SetViewProjectionMatrix( Matrix nView, Matrix nProj )
        {
            view = nView;
            projection = nProj;
            viewInverse = Matrix.Invert( view );
            viewProjection = view * projection;

        }

        public IXNAGame Game { get { return game; } }

    }
}

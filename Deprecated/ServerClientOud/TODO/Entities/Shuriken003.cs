using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Server;
using Wereld = MHGameWork.TheWizards.Server.Wereld;


namespace MHGameWork.TheWizards.ServerClient.Entities
{
    public class Shuriken003 : Wereld.IServerClientEntity
    {
        private Engine.Model model;

        private Wereld.Body body;

        private ServerClientMainOud main;

        public Shuriken003( ServerClientMainOud nMain )
        {
            main = nMain;

            model = new Engine.Model( ProgramOud.SvClMain, "Content\\Shuriken001" );



            body = new MHGameWork.TheWizards.ServerClient.Wereld.Body(); //MHGameWork.TheWizards.Server.Wereld.Body();

            body.Positie = new Vector3( 0, 5, 0 );
            body.Scale = new Vector3( 1f );




        }




        #region IClientEntity Members

        public void Render()
        {
            main.XNAGame.GraphicsDevice.RenderState.FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid;
            main.XNAGame.GraphicsDevice.RenderState.DepthBufferEnable = true;
            model.TempRender( Matrix.CreateScale( body.Scale ) //* Matrix.CreateFromQuaternion(tempRot)
                 * Matrix.CreateRotationX( MathHelper.PiOver2 ) * body.Rotatie
                * Matrix.CreateTranslation( body.Positie ) );
        }
        private Quaternion tempRot = Quaternion.Identity;
        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            tempRot = tempRot * Quaternion.CreateFromAxisAngle( Vector3.Up, 0.005f );
            tempRot.Normalize();
            Vector3 v = Vector3.Forward;
            v = Vector3.Transform( v, tempRot );
            if ( v.Length() - 1 < -0.01f || v.Length() - 1 > 0.01f )
            {
                throw new Exception();
            }
        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

        }


        public Microsoft.Xna.Framework.BoundingSphere BoundingSphere
        {
            get
            {
                //TODO: scale.Length() gebruikt veel tijd
                return new BoundingSphere( body.Positie, body.Scale.Length() );
            }
        }

        #endregion



        public Wereld.Body Body { get { return body; } }


        public static Wereld.ClientEntityHolder CreateShuriken003Entity( Entities.Shuriken003 nEnt )
        {
            Wereld.ClientEntityHolder entH = new Wereld.ClientEntityHolder( nEnt );

            entH.AddElement( nEnt.body );

            return entH;

        }









        #region IServerEntity Members

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.EnablePhysics()
        {

        }

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.DisablePhysics()
        {

        }

        #endregion
    }
}

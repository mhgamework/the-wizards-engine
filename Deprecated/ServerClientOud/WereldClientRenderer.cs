using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient
{
    public class WereldClientRenderer
    {
        private TWWereld wereld;

        public TWWereld Wereld
        {
            get { return wereld; }
            set { wereld = value; }
        }


        public WereldClientRenderer( TWWereld nWereld )
        {
            wereld = nWereld;

        }

        BasicEffect effect;

        public void Render()
        {
            wereld.Game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            if ( effect == null ) effect = new BasicEffect( wereld.Game.GraphicsDevice, null );

            effect.View = wereld.Game.Camera.View;
            effect.Projection = wereld.Game.Camera.Projection;

            effect.Begin();
            for ( int iPass = 0; iPass < effect.CurrentTechnique.Passes.Count; iPass++ )
            {
                effect.CurrentTechnique.Passes[ iPass ].Begin();
                for ( int i = 0; i < wereld.Models.Count; i++ )
                {
                    wereld.Models[ i ].Mesh.DrawPrimitives();


                }
                effect.CurrentTechnique.Passes[ iPass ].End();

            }
            effect.End();
        }


        public static void TestRenderModel()
        {
            TestXNAGame game = null;
            TWWereld wereld = null;
            WereldClientRenderer renderer = null;

            TestXNAGame.Start( "WereldClientRenderer.TestRenderModel",
                delegate
                {
                    game = TestXNAGame.Instance;

                    wereld = new TWWereld( game );

                    renderer = new WereldClientRenderer( wereld );

                    WereldMesh mesh = new WereldMesh( wereld );

                    mesh.LoadFromXml( TWXmlNode.GetRootNodeFromFile(
                        game.EngineFiles.DirUnitTests + "Mesh_TestSerialize.xml" ) );

                    WereldEntity entity = new WereldEntity( wereld );
                    WereldModel model = new WereldModel();
                    model.Mesh = mesh;

                    entity.AddNewModel( model );
                },
                delegate
                {


                    renderer.Render();

                } );
        }
    }
}

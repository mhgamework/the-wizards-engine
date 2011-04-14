using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class ShaderManager
    {
        private ServerClientMainOud engine;
        private List<ShaderEffect> shaders;
        private bool autoLoad;

        public ShaderManager( ServerClientMainOud nEngine )
        {
            engine = nEngine;

            shaders = new List<ShaderEffect>();

            if ( autoLoad )
            {
            }

        }

        public void AddShader( ShaderEffect nShader )
        {
            shaders.Add( nShader );
            if ( engine.XNAGame.GraphicsDevice != null ) nShader.Load( engine.XNAGame.Content );
        }
        public bool RemoveShader( ShaderEffect nShader )
        {
            return shaders.Remove( nShader );
        }


        public virtual void Load( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            if ( e.AllContent )
            {
                for ( int i = 0; i < shaders.Count; i++ )
                {
                    shaders[ i ].Load( engine.XNAGame._content );
                }
            }


            autoLoad = true;

        }

        public virtual void Unload( object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e )
        {
            autoLoad = false;

            if ( e.AllContent )
            {
                for ( int i = 0; i < shaders.Count; i++ )
                {
                    //shaders[ i ].Unload( engine.XNAGame._content );
                }
            }
        }






    }


}

using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public class RendererService : Database.IGameService
    {
        public IXNAGame Game;
        public TheWizards.Database.Database Database;
        public List<IRenderer> Renderers = new List<IRenderer>();

        public RendererService( IXNAGame _game, TheWizards.Database.Database _database )
        {
            Game = _game;
            Database = _database;
        }

        public void AddIRenderer( IRenderer renderer )
        {
            Renderers.Add( renderer );
        }

        public void Render( XNAGame game )
        {
            for ( int i = 0; i < Renderers.Count; i++ )
            {
                Renderers[ i ].Render( game );
            }
        }
        public void Update( XNAGame game )
        {
            for ( int i = 0; i < Renderers.Count; i++ )
            {
                Renderers[ i ].Update( game );
            }
        }
        public void Initialize( XNAGame game )
        {
            for ( int i = 0; i < Renderers.Count; i++ )
            {
                Renderers[ i ].Initialize( game );
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //TODO
        }

        #endregion
    }
}

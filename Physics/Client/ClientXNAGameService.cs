using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.Client
{
    public class ClientXNAGameService: IGameService002
    {
        public XNAGame XNAGame
        {
            get
            {
                return xnaGame;
            }
            set
            {
                xnaGame = value;
            }
        }
        private XNAGame xnaGame;
        private Database.Database database;

        public ClientXNAGameService(Database.Database  _database)
        {
            database = _database;
            database.AddService( this );

            xnaGame = new XNAGame();
        }



        #region IGameService002 Members

        public void Load( MHGameWork.TheWizards.Database.Database _database )
        {
            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
          
        }

        #endregion

        public void Run()
        {
            xnaGame.Run();
        }
    }
}

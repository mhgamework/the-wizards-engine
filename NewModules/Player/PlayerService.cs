using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// Not yet, but his module is supposed to REQUIRE PhysX to operate!
    /// </summary>
    public class PlayerService : IGameService002
    {
        private List<PlayerData> players = new List<PlayerData>();
        private Database.Database database;
        private ClientXNAGameService clientXNAService;


        private PlayerData gamePlayer;
        private PlayerController playerController;


        public PlayerService( Database.Database _database )
        {
            database = _database;
            clientXNAService = database.FindService<ClientXNAGameService>();
        }


        public void AddPlayer( PlayerData data )
        {
            players.Add( data );
        }

        public PlayerData FindPlayerByName( string name )
        {
            for (int i = 0; i < players.Count; i++)
            {
                if ( players[ i ].Name == name )
                    return players[ i ];
            }
            return null;
        }

        /// <summary>
        /// Sets the player that is the player controlled by this game instance. (So the player currently playing this game)
        /// TODO: parameters necessary
        /// </summary>
        public void SetGamePlayer( PlayerData player )
        {
            if ( clientXNAService == null ) throw new InvalidOperationException( "A client game must be created in order to set a game player. (Load ClientXNAGameService)" );
            gamePlayer = player;
            //TODO
            //  ----   playerController = new PlayerController( clientXNAService.XNAGame, gamePlayer );
        }

        /// <summary>
        /// Sets the camera to follow the player, and allow control over the player via mouse/keyboard input
        /// </summary>

        public void EnablePlayerControl()
        {
            if ( playerController == null ) throw new Exception( "No gameplayer was set, set one first" );
            
        }

        #region IGameService002 Members

        public void Load( MHGameWork.TheWizards.Database.Database _database )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.Server.Wereld
{
    public class Wereld
    {
        private QuadTree tree;
        private List<ServerEntityHolder> entities;
        private ServerMainNew main;
        private int nextEntityID = 100;

        private List<GameClient> activePlayers;

        //private List<GeoMipMap.Terrain> terrains = new List<GeoMipMap.Terrain>();
        private Engine.TerrainManager terrainManager;

        public Engine.TerrainManager TerrainManager
        {
            get { return terrainManager; }
            set { terrainManager = value; }
        }


        //Entity synchronization
        private int nextEntityUpdate;
        private int entityUpdateInterval;
        //private List<Common.Networking.INetworkSerializable> entityUpdatePackets;
        private ByteWriter entityUpdatesBuffer;
        //private int entityUpdatesBufferSize = 1024;
        //private int maxEntityUpdateSize = 100;

        private int nextTimeUpdate;
        private int timeUpdateInterval;

        public Wereld( ServerMainNew nMain )
        {
            main = nMain;

            int quadTreeSize = 1 << 11;

            tree = new QuadTree( new BoundingBox( new Vector3( -quadTreeSize / 2, -4000, -quadTreeSize / 2 ), new Vector3( quadTreeSize / 2, 4000, quadTreeSize / 2 ) ) );
            //entityUpdatePackets = new List<MHGameWork.TheWizards.Common.Networking.INetworkSerializable>();
            entityUpdatesBuffer = new ByteWriter();

            entities = new List<ServerEntityHolder>();

            entityUpdateInterval = 1000 / 20;
            timeUpdateInterval = 1000;

            activePlayers = new List<GameClient>();

            terrainManager = new MHGameWork.TheWizards.Server.Engine.TerrainManager( main );

        }


        public void AddEntity( ServerEntityHolder nEntH )
        {
            nEntH.SetID( nextEntityID );
            nextEntityID++;

            nEntH.SetWereld( this );

            entities.Add( nEntH );
            tree.OrdenEntity( nEntH );
        }

        //public void AddTerrain( GeoMipMap.Terrain terr )
        //{
        //    terrains.Add( terr );
        //}

        public void SendTimeUpdates()
        {
            if ( nextTimeUpdate < main.Time )
            {
                //for ( int i = 0; i < main.Communication.Clients.Count; i++ )
                for ( int i = 0; i < activePlayers.Count; i++ )
                {


                    activePlayers[ i ].Proxy.Wereld.UpdateTime( main.Time );

                    //main.Communication.Clients[ i ].Wereld.UpdateTime( p );


                }
                nextTimeUpdate = main.Time + timeUpdateInterval;


            }
        }

        public void SendEntityUpdates()
        {
            if ( nextEntityUpdate < main.Time )
            {
                for ( int i = 0; i < activePlayers.Count; i++ )
                {
                    Common.Network.DeltaSnapshotPacket p = new MHGameWork.TheWizards.Common.Network.DeltaSnapshotPacket();
                    p.Tick = main.Tick;
                    //entityUpdatePackets.Clear();

                    p.StartWriting( entityUpdatesBuffer );

                    for ( int iEnt = 0; iEnt < entities.Count; iEnt++ )
                    {


                        //Common.Networking.INetworkSerializable p = entities[ iEnt ].GetEntityUpdatePacket();
                        //p.EntityID = entities[iEnt].ID;
                        //p.Positie = entities. shurServer.Body.Positie;
                        //p.Rotatie = shurServer.Body.Rotatie;





                        //entityUpdatePackets.Add( p );




                        //IMPORTANT NOTE: De te synchroniseren entities moeten een id hebben tussen 0 en ushort.maxvalue !!!!

                        p.AddEntityUpdatePacket( (ushort)entities[ iEnt ].ID, entities[ iEnt ].GetEntityUpdatePacket() );
                    }

                    activePlayers[ i ].Proxy.Wereld.UpdateWorld( p );

                    //main.ServerMain.Communication.Clients[i].Wereld.UpdateEntity(p);

                }
                nextEntityUpdate = main.Time + entityUpdateInterval;


            }
        }




        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            for ( int i = 0; i < entities.Count; i++ )
            {
                entities[ i ].Process( e );
            }


        }
        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            for ( int i = 0; i < activePlayers.Count; i++ )
            {
                activePlayers[ i ].Tick( e );
            }

            for ( int i = 0; i < entities.Count; i++ )
            {
                entities[ i ].Tick( e );
            }

            SendEntityUpdates();
            SendTimeUpdates();
        }




        public void Login( GameClient nCL, Common.LoginPacket p )
        {
            if ( nCL.TryLogin( p.Username, p.Password ) )
            {
                nCL.Proxy.Wereld.OnSuccessfulLogin( nCL.GetTemporaryLoginKey() );
                nCL.OnLoginSuccesfull();
                activePlayers.Add( nCL );


            }
            else
            {
                nCL.LoggedIn = false;
            }


        }

        public void Ping( GameClient nCL )
        {
            nCL.Proxy.Wereld.OnPingReply();
        }

        public void ApplyPlayerInput( GameClient nCL, Common.PlayerInputData input )
        {
            nCL.ApplyPlayerInput( input );
        }

        private int tempNextShoot;
        public void TempShootShuriken( GameClient nCL )
        {
            if ( main.Time < tempNextShoot ) return;
            tempNextShoot = main.Time + 1000;
            //entities[ 0 ].Body.Actor.addLocalForce( new Vector3( 0, -100, 0 ), NovodexWrapper.NxForceMode.NX_ACCELERATION );
            if ( nCL.PlayerEntity != null )
            {
                Vector3 forward = Vector3.Transform( Vector3.Backward, nCL.LastPlayerInput.Orientation );
                forward.Y = 0;
                forward.Normalize();



                Server.Entities.Shuriken003 serverEnt = new MHGameWork.TheWizards.Server.Entities.Shuriken003( main );
                Server.Wereld.ServerEntityHolder serverEntH = Server.Entities.Shuriken003.CreateShuriken003Entity( serverEnt );



                main.Wereld.AddEntity( serverEntH );
                //serverEntH.SetID( i );
                serverEntH.Body.Positie = nCL.PlayerEntity.body.Positie + forward * 100 + new Vector3( 0, 50, 0 );
                serverEntH.Body.Actor.addLocalForce( Vector3.Normalize( new Vector3( forward.X, 0.02f, forward.Z ) ) * 1000f, NovodexWrapper.NxForceMode.NX_ACCELERATION );

                /*clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( main );
                clientEntH = Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                main.Wereld.AddEntity( clientEntH );
                clientEntH.SetID( serverEntH.ID );*/
                // shurServer.Body.serverBody.Rotatie = Matrix.CreateRotationX(MathHelper.PiOver2);
            }
        }

        /// <summary>
        /// Client available
        /// </summary>
        /// <returns></returns>
        public byte[] GetTerrainsList()
        {
            ByteWriter br = new ByteWriter();

            terrainManager.Save( br.MemStrm );

            return br.ToBytesAndClose();


        }

        public void SetBlockHeightMapData( int terrainID, int x, int z, int prevVersion, float[] data )//, out int newVersion, out float[] newData )
        {
            int newVersion;
            float[] newData;

            GeoMipMap.Terrain terr = terrainManager.FindTerrain( terrainID );
            if ( terr == null ) return; //TODO: log exception

            terr.SetBlockHeightMapData( x, z, prevVersion, data, out newVersion, out newData );

        }



        public QuadTree Tree
        { get { return tree; } }

        public List<GameClient> ActivePlayers { get { return activePlayers; } }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common;

namespace MHGameWork.TheWizards.ServerClient.Network
{
    public class ProxyServerWereld //: Client.ProxyServerWereld
    {
        private ProxyServer server;
        public ProxyServerWereld( ProxyServer nServer )
        //: base(nServer)
        {
            server = nServer;
        }



        public void LoginAsync( string nUsername, string nPassword )
        {
            byte[] PasswordBytes;
            //Dim B As Byte() = New Byte(16 + nUsername.Length - 1) {} 
            System.Security.Cryptography.MD5 Crypt;
            Crypt = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            PasswordBytes = Crypt.ComputeHash( System.Text.Encoding.ASCII.GetBytes( nPassword ) );

            //HashedBytes.CopyTo(B, 0) 
            //System.Text.Encoding.ASCII.GetBytes(nUsername).CopyTo(B, HashedBytes.Length) 


            LoginPacket P = new LoginPacket( nUsername, PasswordBytes );

            this.Server.Communication.SendPacketTCP( Common.Communication.ServerCommands.Login, P );

        }

        public class LoginCallbackEventArgs : EventArgs
        {
            public string LoginKey;

            public LoginCallbackEventArgs( string nLoginKey )
            {
                LoginKey = nLoginKey;
            }
        }

        public event EventHandler<LoginCallbackEventArgs> LoginCallback;

        public void OnLoginCallback( object sender, LoginCallbackEventArgs e )
        {
            if ( LoginCallback != null ) LoginCallback( sender, e );
        }

        public void Ping()
        {
            server.Communication.SendPacketUDP( Common.Communication.ServerCommands.Ping );
        }

        public void ApplyPlayerInput( PlayerInputData input )
        {
            server.Communication.SendPacketUDP( Communication.ServerCommands.ApplyPlayerInput, input );
        }



        /// <summary>
        /// Deprecated
        /// </summary>
        public void GetTerrainsListAsync()
        {
            server.Communication.SendPacketTCP( Communication.ServerCommands.GetTerrainsList );
        }

        public void OnGetTerrainsListCompleted( ByteReader br )
        {
            RaiseEvent( GetTerrainsListCompleted, this, new GetTerrainsListEventArgs( br ) );
        }

        public class GetTerrainsListEventArgs : EventArgs
        {
            public ByteReader BR;
            public GetTerrainsListEventArgs( ByteReader br )
            {
                BR = br;
            }
        }

        public event EventHandler<GetTerrainsListEventArgs> GetTerrainsListCompleted;




        /// <summary>
        /// Deprectated
        /// </summary>
        /// <param name="terrainID"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="prevVersion"></param>
        /// <param name="data"></param>
        public void SetBlockHeightMapDataAsync( int terrainID, int x, int z, int prevVersion, float[] data )//, out int newVersion, out float[] newData )
        {
            ByteWriter bw = new ByteWriter();
            bw.Write( terrainID );
            bw.Write( x );
            bw.Write( z );
            bw.Write( prevVersion );
            for ( int i = 0; i < data.Length; i++ )
            {
                bw.Write( data[ i ] );
            }


            server.Communication.SendPacketTCP( Communication.ServerCommands.SetBlockHeightMapData, bw.ToBytesAndClose() );

        }













        public void RaiseEvent<TEventArgs>( EventHandler<TEventArgs> eventHandler, Object sender, TEventArgs e ) where TEventArgs : EventArgs
        {
            if ( eventHandler != null )
                eventHandler( sender, e );
        }


        public ProxyServer Server { get { return server; } }
    }
}

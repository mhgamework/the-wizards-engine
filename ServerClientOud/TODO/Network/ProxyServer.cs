using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.Networking;

namespace MHGameWork.TheWizards.ServerClient.Network
{
    public class ProxyServer //: Client.ProxyServer
    {
        protected ClientCommunication communication;
        protected ProxyServerWereld wereld;
        protected ServerClientMainOud main;

        private string serverIP;

        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        public ProxyServer( ServerClientMainOud nMain )
        //: base(nParent)
        {
            main = nMain;
            serverIP = "127.0.0.1";

            communication = new ClientCommunication( this, System.Net.IPAddress.Parse( "127.0.0.1" ), 5012, 5014 );

            //communication = new ClientCommunication(this, System.Net.IPAddress.Parse("81.164.212.253"), 5012, 5014);

            CreateProxyWereld();
        }

        protected virtual void CreateProxyWereld()
        {
            wereld = new ProxyServerWereld( this );
        }


        public void ConnectAsync()
        {
            communication.ServerAddress = System.Net.IPAddress.Parse( serverIP );
            communication.ConnectAsync();

        }

        public void GetGameFilesListAsync()
        {
            communication.SendPacketTCP( Common.Communication.ServerCommands.GetGameFilesList );
        }

        public void OnGetGameFilesListCompleted( ByteReader br )
        {
            RaiseEvent( GetGameFilesListCompleted, this, new GetGameFilesListEventArgs( br ) );
            //   if ( GetGameFilesListCompleted != null )
            //      GetGameFilesListCompleted( this, new GetGameFilesListEventArgs( br ) );
        }

        public class GetGameFilesListEventArgs : EventArgs
        {
            public ByteReader BR;
            public GetGameFilesListEventArgs( ByteReader br )
            {
                BR = br;
            }
        }

        public event EventHandler<GetGameFilesListEventArgs> GetGameFilesListCompleted;


        public void GetGameIniFileIDAsync()
        {
            communication.SendPacketTCP( Common.Communication.ServerCommands.GetGameIniFileID );
        }





        public void GetCoreFilesListAsync()
        {
            communication.SendPacketTCP( Common.Communication.ServerCommands.GetCoreFilesList );
        }

        public void OnGetCoreFilesListCompleted( ByteReader br )
        {
            RaiseEvent( GetCoreFilesListCompleted, this, new GetCoreFilesListEventArgs( br ) );
            //   if ( GetGameFilesListCompleted != null )
            //      GetGameFilesListCompleted( this, new GetGameFilesListEventArgs( br ) );
        }

        public class GetCoreFilesListEventArgs : EventArgs
        {
            public ByteReader BR;
            public GetCoreFilesListEventArgs( ByteReader br )
            {
                BR = br;
            }
        }

        public event EventHandler<GetCoreFilesListEventArgs> GetCoreFilesListCompleted;


        public void GetGameFileDataAsync( Engine.GameFileOud file )
        {
            ByteWriter br = new ByteWriter();
            br.Write( file.ID );
            br.Write( file.Version );

            communication.SendPacketTCP( Common.Communication.ServerCommands.GetGameFileData, br.ToBytesAndClose() );
        }


        public class GetGameFileDataEventArgs : EventArgs
        {
            public int ID;
            public int Version;
            public string AssetName;
            public byte[] data;

        }


        private void OnGetGameFileDataCompleted( object sender, GetGameFileDataEventArgs e )
        {
            RaiseEvent( GetGameFileDataCompleted, sender, e );
        }

        public event EventHandler<GetGameFileDataEventArgs> GetGameFileDataCompleted;



        public void RaiseEvent<TEventArgs>( EventHandler<TEventArgs> eventHandler, Object sender, TEventArgs e ) where TEventArgs : EventArgs
        {
            if ( eventHandler != null )
                eventHandler( sender, e );
        }






        public void OnPacketRecieved( object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e )
        {
            byte cmd = e.Dgram[ 0 ];
            byte[] data = new byte[ e.Dgram.Length - 1 ];
            Array.Copy( e.Dgram, 1, data, 0, data.Length );
            if ( !System.Enum.IsDefined( typeof( Communication.ClientCommands ), cmd ) )
            {
                System.Diagnostics.Debugger.Break();
            }
            Communication.ClientCommands Command = (Communication.ClientCommands)cmd;

            ByteReader BR = new ByteReader( data );
            this.ProcessCommand( Command, BR );
            //RaiseEvent CommandRecievedFromClient(Me, New CommandRecievedFromClientEventArgs(Command, BR, nCL, e.EP)) 
            BR.Close();


        }


        private void ProcessCommand( Communication.ClientCommands cmd, ByteReader br )
        {
            switch ( cmd )
            {
                case Common.Communication.ClientCommands.OnSuccessfulLogin:
                    string LoginKey = System.Text.ASCIIEncoding.ASCII.GetString( br.ReadBytes( (int)br.BytesLeft ) );
                    Wereld.OnLoginCallback( null, new ProxyServerWereld.LoginCallbackEventArgs( LoginKey ) );
                    //main.Wereld.OnSuccessfulLogin( LoginKey );
                    break;

                case Common.Communication.ClientCommands.OnUDPConnectionLinked:
                    OnUDPConnectionLinked();
                    break;

                case Common.Communication.ClientCommands.PingReply:
                    main.Wereld.OnPingReply();
                    break;

                case Common.Communication.ClientCommands.EntityUpdate:
                    main.Wereld.UpdateEntity( Common.Network.UpdateEntityPacket.FromNetworkBytes( br ) );
                    break;

                case Common.Communication.ClientCommands.UpdateWorld:
                    main.Wereld.UpdateWorld( Common.Network.DeltaSnapshotPacket.FromNetworkBytes( br ) );
                    break;

                case Common.Communication.ClientCommands.UpdateTime:
                    main.Wereld.UpdateTime( Common.Network.TimeUpdatePacket.FromNetworkBytes( br ) );
                    break;

                case Common.Communication.ClientCommands.SetGameClientData:
                    main.GameClient.SetGameClientData( Common.GameClientData.FromNetworkBytes( br ) );
                    break;

                case Common.Communication.ClientCommands.SetGameFilesList:
                    main.SetGameFilesList( br );
                    break;

                case Common.Communication.ClientCommands.GetGameFilesListCompleted:
                    OnGetGameFilesListCompleted( br );
                    break;

                case Common.Communication.ClientCommands.GetTerrainsListCompleted:
                    Wereld.OnGetTerrainsListCompleted( br );
                    break;

                case Common.Communication.ClientCommands.GetGameFileDataCompleted:
                    GetGameFileDataEventArgs e = new GetGameFileDataEventArgs();
                    e.ID = br.ReadInt32();
                    e.Version = br.ReadInt32();
                    e.AssetName = br.ReadString();
                    e.data = br.ReadBytes( (int)br.BytesLeft );
                    OnGetGameFileDataCompleted( this, e );
                    break;

                case Common.Communication.ClientCommands.GetCoreFilesListCompleted:
                    OnGetCoreFilesListCompleted( br );
                    break;

                case Common.Communication.ClientCommands.SetGameIniFileID:
                    main.SetGameIniFileID( br.ReadInt32() );
                    break;

                default:
                    throw new Exception( "Unknown Command! " + cmd.ToString() );
            }
        }






        public void LinkUDPConnection( string LoginKey )
        {
            this.Communication.SendPacketUDP( Common.Communication.ServerCommands.LinkUDPConnection, System.Text.ASCIIEncoding.ASCII.GetBytes( LoginKey ) );
        }

        public void OnUDPConnectionLinked()
        {
            if ( UDPConnectionLinked != null ) UDPConnectionLinked( this, null );
        }

        public event EventHandler UDPConnectionLinked;

        public void OnPingReply()
        {
        }




        public ProxyServerWereld Wereld
        { get { return wereld; } }

        public ClientCommunication Communication
        {
            get { return communication; }
        }
        public ServerClientMainOud Main { get { return main; } }

        public bool ConnectedToServer
        {
            get
            {
                if ( communication == null ) return false;
                return communication.Connected;
            }
        }
    }





}

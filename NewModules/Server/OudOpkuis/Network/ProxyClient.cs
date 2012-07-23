using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Server;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.Networking;
using ICSharpCode.SharpZipLib.Zip;

namespace MHGameWork.TheWizards.Server.Network
{
    public class ProxyClient : MHGameWork.Game3DPlay.Core.SpelObject
    {
        //Inherits BaseClient
        private ProxyClientWereld mWereld;
        private ProxyGameClient proxyGameClient;
        private GameClient gameClient;
        private ServerMainNew main;
        private TCPConnection tcpConnection;

        public ProxyClient( ServerMainNew nParent, TCPConnection nTCPConn )
            : base( nParent )
        {
            main = nParent;

            setWereld( new ProxyClientWereld( this ) );
            proxyGameClient = new ProxyGameClient( this );
            setGameClient( new GameClient( this, main ) );

            tcpConnection = nTCPConn;

            tcpConnection.NetworkErrorAsync += mTCPConnection_NetworkErrorAsync;
            tcpConnection.PacketRecievedAsync += mTCPConnection_PacketRecievedAsync;


            TCPConnection.Receiving = true;

            setUDPEndPoint( null );
        }




        private void setWereld( ProxyClientWereld value )
        {
            mWereld = value;
        }



        public void SetGameFilesList( Engine.GameFileManager manager )
        {
            throw new Exception( "Deprecated" );
            //ByteWriter br = new ByteWriter();

            //manager.Save( br.MemStrm );

            ////TODO: zip this
            //SendPacketTCP( Communication.ClientCommands.SetGameFilesList, br.ToBytesAndClose() );
        }



        public void OnUDPConnectionLinked()
        {
            SendPacketTCP( Communication.ClientCommands.OnUDPConnectionLinked );
        }



































        private void setGameClient( GameClient value )
        {
            gameClient = value;
        }



        // //// ////public void new( Spelobject nParent,  TCPConnection nTCPConn )
        // //// ////    MyBase.new(nParent)
        // //// ////    //setEndPoint(EP)
        // //// ////    setTCPConn(nTCPConn)
        // //// ////    setLastPlayerCommand(new PlayerCommandBuilder)
        // //// ////    TCPConn.Receiving = True
        // //// ////}

        public override void Dispose()
        {
            base.Dispose();

            if ( TCPConnection != null )
            {
                TCPConnection.Dispose();
                tcpConnection = null;
            }
        }

        private System.Net.IPEndPoint mUDPEndPoint;
        public System.Net.IPEndPoint UDPEndPoint
        {
            get
            {
                return mUDPEndPoint;
            }
        }
        public void setUDPEndPoint( System.Net.IPEndPoint value )
        {
            mUDPEndPoint = value;
        }



        public TCPConnection TCPConnection
        {
            get
            {
                return tcpConnection;
            }
        }




        public void SendPacketUDP( Communication.ClientCommands cmd )
        {
            SendPacketUDP( cmd, new byte[ 0 ] );
        }
        public void SendPacketUDP( Communication.ClientCommands cmd, INetworkSerializable nINS )
        {
            SendPacketUDP( cmd, nINS.ToNetworkBytes() );
        }

        public void SendPacketUDP( Communication.ClientCommands cmd, byte[] data )
        {


#if (DEBUG)

            if ( UDPEndPoint == null )
            {
                return;
                //throw new Exception("De client heeft nog geen UDP verbinding gemaakt!");
            }

# endif
            main.Communication.SendPacketUDP( cmd, data, UDPEndPoint );
        }

        public void SendPacketTCP( Communication.ClientCommands cmd )
        {
            SendPacketTCP( cmd, new byte[ 0 ] );
        }
        public void SendPacketTCP( Communication.ClientCommands cmd, INetworkSerializable nINS )
        {
            SendPacketTCP( cmd, nINS.ToNetworkBytes() );
        }

        public void SendPacketTCP( Communication.ClientCommands cmd, byte[] data )
        {
#if (DEBUG)
            if ( data == null ) throw new ArgumentNullException( "data" );
#endif
            byte[] B;

            B = new byte[ data.Length + 1 ]; { } //new byte(data.Length +1-1) {}

            B[ 0 ] = (byte)cmd;
            data.CopyTo( B, 1 );
            TCPConnection.SendPacket( B, TCPPacketBuilder.TCPPacketFlags.None );

        }







        private void mTCPConnection_NetworkErrorAsync( object sender, Common.Networking.BaseConnection.NetworkErrorEventArgs e )// Handles mTCPConnection.NetworkErrorAsync
        {

        }


        private void mTCPConnection_PacketRecievedAsync( object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e ) //Handles mTCPConnection.PacketRecievedAsync
        {
            HoofdObj.Invoker.Invoke( OnPacketRecieved, sender, e );


        }

        public void OnPacketRecieved( object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e )
        {
            byte cmd = e.Dgram[ 0 ];
            //byte[] data = new byte[ e.Dgram.Length - 1 - 1 ];
            byte[] data = new byte[ e.Dgram.Length - 1 ];
            Array.Copy( e.Dgram, 1, data, 0, data.Length );

            if ( !System.Enum.IsDefined( typeof( Communication.ServerCommands ), cmd ) )
            {
                throw new Exception();// Stop;
            }
            Communication.ServerCommands Command = (Communication.ServerCommands)cmd;

            ByteReader BR = new ByteReader( data );
            ProcessCommand( Command, BR );
            //RaiseEvent CommandRecievedFromClient(this, new CommandRecievedFromClientEventArgs(Command, BR, nCL, e.EP))
            BR.Close();


        }

        private void ProcessCommand( Communication.ServerCommands cmd, ByteReader br )
        {
            byte[] ret;
            ByteWriter bw;

            //System.IO.StreamWriter w = System.IO.File.AppendText( System.Windows.Forms.Application.StartupPath + @"\ServerNetworkLog.txt" );
            //w.WriteLine( ( (int)cmd ).ToString() + ": " + cmd.ToString() );
            //w.Close();
            switch ( cmd )
            {
                case Communication.ServerCommands.Login:
                    main.Wereld.Login( gameClient, LoginPacket.FromNetworkBytes( br ) );
                    break;

                case Communication.ServerCommands.Ping:
                    main.Wereld.Ping( GameClient );
                    //Wereld.OnPingReply();
                    break;

                case Communication.ServerCommands.TempShootShuriken:
                    main.Wereld.TempShootShuriken( gameClient );
                    break;

                case Communication.ServerCommands.ApplyPlayerInput:
                    main.Wereld.ApplyPlayerInput( gameClient, Common.PlayerInputData.FromNetworkBytes( br ) );
                    break;

                case Communication.ServerCommands.GetGameFilesList:
                    ret = main.GetGameFilesList();
                    SendPacketTCP( Communication.ClientCommands.GetGameFilesListCompleted, ret );
                    break;

                case Communication.ServerCommands.GetCoreFilesList:
                    ret = main.GetCoreFilesList();
                    SendPacketTCP( Communication.ClientCommands.GetCoreFilesListCompleted, ret );
                    break;

                case Communication.ServerCommands.GetTerrainsList:
                    ret = main.Wereld.GetTerrainsList();
                    SendPacketTCP( Communication.ClientCommands.GetTerrainsListCompleted, ret );
                    break;

                case Communication.ServerCommands.GetGameFileData:
                    int id = br.ReadInt32();
                    int clientVersion = br.ReadInt32();

                    Engine.GameFile file;

                    ret = main.GetGameFileData( id, clientVersion, out file );
                    bw = new ByteWriter();
                    bw.Write( id );
                    bw.Write( file.Version );
                    bw.Write( file.AssetName );
                    if ( ret != null ) bw.Write( ret );
                    SendPacketTCP( Communication.ClientCommands.GetGameFileDataCompleted, bw.ToBytesAndClose() );
                    break;

                case Communication.ServerCommands.SetBlockHeightMapData:
                    int terrainID = br.ReadInt32();
                    int x = br.ReadInt32();
                    int z = br.ReadInt32();
                    int prevVersion = br.ReadInt32();
                    float[] data = new float[ ( br.BytesLeft >> 2 ) ];
                    for ( int i = 0; i < data.Length; i++ )
                    {
                        data[ i ] = br.ReadSingle();
                    }
                    main.Wereld.SetBlockHeightMapData( terrainID, x, z, prevVersion, data );
                    break;

                case Communication.ServerCommands.GetGameIniFileID:
                    SendPacketTCP( Communication.ClientCommands.SetGameIniFileID, BitConverter.GetBytes( main.GetGameIniFileID() ) );
                    break;

                default:
                    break;
            }


        }











        // //// ////private WithEvents new TickElement TickElement(this)
        // //// ////private WithEvents new Scheduler SchedulerElement(this)

        // //// ////public override Function Login( String nUsername,  byte nPassword()) As LoginResult
        // //// ////    if (nPassword.Length <> 16) { Throw new Exception


        // //// ////    Dim new adapter TheWizardsDataSetTableAdapters.LoginsTableAdapter

        // //// ////    Dim TheWizardsDataSet.LoginsDataTable Tbl
        // //// ////    Tbl = adapter.GeUserByUsernamePassword(nUsername, System.Text.Encoding.ASCII.get{String(nPassword))
        // //// ////    if (Tbl.Count <> 1) {
        // //// ////        //CreateLogin(nUsername, nPassword)
        // //// ////        setLoggedIn(False)
        // //// ////        return LoginResult.UsernamePasswordDontMatch
        // //// ////    Else


        // //// ////        setUsername(nUsername)
        // //// ////        DisplayName = Tbl(0).displayname


        // //// ////        Dim Player PL = CType(SVMain.Wereld.Tree.RootNode.FindEntity(Tbl(0).PlayerEntity), Player)


        // //// ////        if (PL Is null) {
        // //// ////            PL = CreatePlayerEntity
        // //// ////            //return LoginResult.PlayerEntityNotFound
        // //// ////            PL.Functions.Positie = new Vector3(0, 20, 0)
        // //// ////            Tbl(0).PlayerEntity = PL.EntityID
        // //// ////            SVMain.DB.Logins.Update(Tbl(0))


        // //// ////        }

        // //// ////        setLinkedPlayerEntity(PL)


        // //// ////        CreateLoginAttempt()


        // //// ////        return LoginResult.Succes


        // //// ////    }


        // //// ////    return null
        // //// ////}


        // //// ////public Function TryCompleteLoginAttempt( Integer nAttemptID, System.NetIPEndPoint nUDPEndPoint) As Boolean
        // //// ////    if (AttemptingLogin = False) { return False
        // //// ////    if (LoginAttemptID = nAttemptID) {
        // //// ////        setAttemptingLogin(False)
        // //// ////        setUDPEndPoint(nUDPEndPoint)
        // //// ////        setLoggedIn(True)
        // //// ////        return True
        // //// ////    }


        // //// ////    return False
        // //// ////}

        // //// ////private Integer mLoginAttemptID
        // //// ////public ReadOnly Integer LoginAttemptID()
        // //// ////    <System.Diagnostics.DebuggerStepThrough()> get{
        // //// ////        return mLoginAttemptID
        // //// ////    }
        // //// ////}
        // //// ////private void setLoginAttemptID( Integer value)
        // //// ////    mLoginAttemptID = value
        // //// ////}


        // //// ////private Boolean mAttemptingLogin
        // //// ////public ReadOnly Boolean AttemptingLogin()
        // //// ////    <System.Diagnostics.DebuggerStepThrough()> get{
        // //// ////        return mAttemptingLogin
        // //// ////    }
        // //// ////}
        // //// ////private void setAttemptingLogin( Boolean value)
        // //// ////    mAttemptingLogin = value
        // //// ////}


        // //// ////private Integer mLoginAttemptTimeout
        //////////////// <summary>
        //////////////// not coded yet
        //////////////// </summary>
        //////////////// <value></value>
        //////////////// <returns></returns>
        //////////////// <remarks></remarks>
        // //// ////public ReadOnly Integer LoginAttemptTimeout()
        // //// ////    <System.Diagnostics.DebuggerStepThrough()> get{
        // //// ////        return mLoginAttemptTimeout
        // //// ////    }
        // //// ////}
        // //// ////private void setLoginAttemptTimeout( Integer value)
        // //// ////    mLoginAttemptTimeout = value
        // //// ////}


        // //// ////Protected override void setLinkedPlayerEntity( Common.Player value)
        // //// ////    MyBase.setLinkedPlayerEntity(value)
        // //// ////    if (value IsNot null) {
        // //// ////        value.PlayerFunctions.DisplayName = DisplayName
        // //// ////    }
        // //// ////}


        ////////////////// <summary>
        ////////////////// Temporary sub
        ////////////////// </summary>
        ////////////////// <remarks></remarks>
        //// //// ////public void CreateLogin( String nUsername,  byte nPassword())

        //// //// ////    Dim Player P = CType(SVMain.CreateEntity(EntityType.Player, SVMain.Wereld.Tree), Player)

        //// //// ////    //SVMain.Wereld.Tree.AddEntity(P)

        //// //// ////    SVMain.DB.Logins.Insert(nUsername, System.Text.Encoding.ASCII.get{String(nPassword), nUsername, P.EntityID)

        //// //// ////}

        // //// ////public Function CreatePlayerEntity() As Player
        // //// ////    Dim Player P = CType(SVMain.CreateEntity(EntityType.Player, SVMain.Wereld.Tree), Player)

        // //// ////    //SVMain.Wereld.Tree.AddEntity(P)
        // //// ////    return P
        // //// ////}

        //// //// ////public void UpdateAngles( Vector3 nAngles)
        //// //// ////    if (LinkedPlayerEntity Is null) {
        //// //// ////        Exit void
        //// //// ////    }
        //// //// ////    //LinkedPlayerEntity.Positie = Vector3.Empty
        //// //// ////    setAngles(nAngles)


        //// //// ////    //UpdatePlayerEntSnelheid()

        //// //// ////}

        //// //// ////public override void SignaleerVerandering( object sender,  VeranderingEventArgs e)
        //// //// ////    SVMain.ClientComm.SignaleerVeranderingAsync(e.Ent.EntityID, e.Ent.Versie, this)
        //// //// ////}

        // //// ////public override void OnVerandering( object sender,  Common.VeranderingEventArgs e)
        // //// ////    SVMain.ClientComm.OnVeranderingAsync(e.Entity.EntityID, e.Type, e.Data, this)
        // //// ////}



        // //// ////private PlayerCommandBuilder mLastPlayerCommand
        // //// ////public ReadOnly PlayerCommandBuilder LastPlayerCommand()
        // //// ////    <System.Diagnostics.DebuggerStepThrough()> get{
        // //// ////        return mLastPlayerCommand
        // //// ////    }
        // //// ////}
        // //// ////private void setLastPlayerCommand( PlayerCommandBuilder value)
        // //// ////    mLastPlayerCommand = value
        // //// ////}


        // //// ////public Overridable void ProcessClientCommand( PlayerCommandBuilder nBuilder)
        // //// ////    setLastPlayerCommand(nBuilder)

        // //// ////    //if (LinkedPlayerEntity Is null) {
        // //// ////    //    Exit void
        // //// ////    //}
        // //// ////    ////LinkedPlayerEntity.Positie = Vector3.Empty
        // //// ////    //setAngles(nAngles)
        // //// ////    //Select Case nPlCmd
        // //// ////    //    Case PlayerCommands.VooruitStart
        // //// ////    //        setBeweegdVooruit(True)
        // //// ////    //    Case PlayerCommands.VooruitEnd
        // //// ////    //        setBeweegdVooruit(False)
        // //// ////    //    Case PlayerCommands.AchteruitStart
        // //// ////    //        setBeweegdAchteruit(True)
        // //// ////    //    Case PlayerCommands.AchteruitEnd
        // //// ////    //        setBeweegdAchteruit(False)
        // //// ////    //    Case Else
        // //// ////    //        Stop //kan niet
        // //// ////    //End Select


        // //// ////    //UpdatePlayerEntSnelheid()


        // //// ////}


        // //// ////private Single mJumpTime
        // //// ////public Single JumpTime()
        // //// ////    <System.Diagnostics.DebuggerStepThrough()> get{
        // //// ////        return mJumpTime
        // //// ////    }
        // //// ////    <System.Diagnostics.DebuggerStepThrough()> Set( Single value)
        // //// ////        mJumpTime = value
        // //// ////    End Set
        // //// ////}


        // //// ////private void TickElement_Tick( object sender,  MHGameWork.Game3DPlay.TickBaseElement.TickEventArgs e) Handles TickElement.Tick
        // //// ////    if (LinkedPlayerEntity Is null) { Exit void
        // //// ////    if (LinkedPlayerEntity.Enabled = False) { Exit void

        // //// ////    Dim Vector3 TotalDisp //Total Displacement
        // //// ////    Dim Vector3 LocalMov //Local Movement
        // //// ////    Dim Vector3 WorldMov  //World Movement
        // //// ////    if (LastPlayerCommand.NoClip) {

        // //// ////        With LastPlayerCommand
        // //// ////            if (.Vooruit) {
        // //// ////                LocalMov += new Vector3(0, 0, 1)
        // //// ////            }
        // //// ////            if (.Achteruit) {
        // //// ////                LocalMov += new Vector3(0, 0, -1)
        // //// ////            }
        // //// ////            if (.StrafeLinks) {
        // //// ////                LocalMov += new Vector3(-1, 0, 0)
        // //// ////            }
        // //// ////            if (.StrafeRechts) {
        // //// ////                LocalMov += new Vector3(1, 0, 0)
        // //// ////            }
        // //// ////            if (.Jump) {
        // //// ////                WorldMov += new Vector3(0, 1, 0)
        // //// ////            }
        // //// ////            if (.Crouch) {
        // //// ////                WorldMov += new Vector3(0, -1, 0)
        // //// ////            }

        // //// ////        End With
        // //// ////        //LocalDisp.Normalize()
        // //// ////        WorldMov += Vector3.TransformCoordinate(LocalMov, Matrix.RotationYawPitchRoll(LastPlayerCommand.CameraAngles.Y, -LastPlayerCommand.CameraAngles.X, LastPlayerCommand.CameraAngles.Z))
        // //// ////        WorldMov.Normalize()

        // //// ////        if (LastPlayerCommand.PrimaryAttack) {
        // //// ////            WorldMov *= 100
        // //// ////        }
        // //// ////        if (LastPlayerCommand.Run) {
        // //// ////            WorldMov *= 5
        // //// ////        }

        // //// ////        TotalDisp += WorldMov * CType(HoofdObj, BaseMain).ClientSpeed


        // //// ////    Else
        // //// ////        With LastPlayerCommand
        // //// ////            if (.Vooruit) {
        // //// ////                LocalMov += new Vector3(0, 0, 1)
        // //// ////            }
        // //// ////            if (.Achteruit) {
        // //// ////                LocalMov += new Vector3(0, 0, -1)
        // //// ////            }
        // //// ////            if (.StrafeLinks) {
        // //// ////                LocalMov += new Vector3(-1, 0, 0)
        // //// ////            }
        // //// ////            if (.StrafeRechts) {
        // //// ////                LocalMov += new Vector3(1, 0, 0)
        // //// ////            }
        // //// ////            if (.Jump) {
        // //// ////                if (JumpTime = 0) { JumpTime = 1000
        // //// ////            Else
        // //// ////                JumpTime = 0
        // //// ////            }
        // //// ////            if (.Crouch) {
        // //// ////                LinkedPlayerEntity.Controller.Height = 0.2
        // //// ////            Else
        // //// ////                LinkedPlayerEntity.Controller.Height = 2
        // //// ////                //    WorldMov += new Vector3(0, -1, 0)
        // //// ////            }

        // //// ////        End With
        // //// ////        //LocalDisp.Normalize()
        // //// ////        WorldMov += Vector3.TransformCoordinate(LocalMov, Matrix.RotationYawPitchRoll(LastPlayerCommand.CameraAngles.Y, -LastPlayerCommand.CameraAngles.X, LastPlayerCommand.CameraAngles.Z))
        // //// ////        WorldMov.Normalize()

        // //// ////        if (LastPlayerCommand.Run) {
        // //// ////            WorldMov *= 5
        // //// ////        }
        // //// ////        if (JumpTime > 0) {
        // //// ////            JumpTime -= e.Elapsed * 1000.0F
        // //// ////            WorldMov += new Vector3(0, 4, 0)

        // //// ////        }

        // //// ////        TotalDisp += WorldMov * CType(HoofdObj, BaseMain).ClientSpeed




        // //// ////        if (JumpTime <= 0) {
        // //// ////            JumpTime = 0
        // //// ////            TotalDisp += CType(HoofdObj, BaseMain).ClientGravity
        // //// ////        }





        // //// ////    }

        // //// ////    Dim UInteger CollisionFlags
        // //// ////    LinkedPlayerEntity.Controller.move(TotalDisp * e.Elapsed, UInteger.MaxValue, 0.001F, CollisionFlags, 1.0F)




        // //// ////}

        // //// ////private void OnNetworkError( object sender,  Common.Networking.TCPConnection.NetworkErrorEventArgs e)
        // //// ////    if (TypeOf e.Ex IsSystem.NetSockets.SocketException) {
        // //// ////        Select Case CType(e.Ex,System.NetSockets.SocketException).SocketErrorCode
        // //// ////            CaseSystem.NetSockets.SocketError.ConnectionAborted
        // //// ////                SVMain.ClientComm.OnClientDisconnected(this, new ClientCommunication.ClientDisconnectedEventArgs(this))
        // //// ////        End Select
        // //// ////    }
        // //// ////}

        // //// ////private void mTCPConn_NetworkError( object sender,  Common.Networking.TCPConnection.NetworkErrorEventArgs e) Handles mTCPConn.NetworkError
        // //// ////    Scheduler.ScheduleAction(AddressOf OnNetworkError, sender, e, 0)
        // //// ////}

        // //// ////private void OnPacketRecieved( object sender,  Common.Networking.TCPConnection.PacketRecievedEventArgs e)
        // //// ////    SVMain.ClientComm.OnPacketRecieved(new Common.Communication.PacketRecievedEventArgs(e.Dgram, TCPConn.EndPoint), this)
        // //// ////}

        // //// ////private void mTCPConn_PacketRecieved( object sender,  Common.Networking.TCPConnection.PacketRecievedEventArgs e) Handles mTCPConn.PacketRecieved
        // //// ////    Scheduler.ScheduleAction(AddressOf OnPacketRecieved, sender, e, 0)
        // //// ////}

        public ProxyClientWereld Wereld
        {
            get
            {

                return mWereld;
            }
        }
        public ProxyGameClient ProxyGameClient
        { get { return proxyGameClient; } }
        public GameClient GameClient
        {
            get
            {
                return gameClient;
            }
        }

    }
}
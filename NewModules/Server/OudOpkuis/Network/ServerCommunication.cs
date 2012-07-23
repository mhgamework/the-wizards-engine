using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.Game3DPlay.Core.Elements;
using MHGameWork.Game3DPlay.SpelObjecten;


namespace MHGameWork.TheWizards.Server.Network
{

    public class ServerCommunication : Communication
    {
        private TCPConnectionListener tcpListener;



        public ServerCommunication( SpelObject nParent )
            : base( nParent )
        {
            //Me.setClients(New Generic.Dictionary(Of IPEndPoint, Client)) 
            ///''Me.setClients(New List(Of Client)) 


            this.setClients( new List<ProxyClient>() );


        }

        public void StartListening()
        {
            if ( tcpListener != null ) tcpListener.Dispose();
            tcpListener = null;


            try
            {

                this.UDPConn.UDP.Client.Bind( new IPEndPoint( IPAddress.Any, UDPPort ) );
            }
            catch
            {
#if (DEBUG)
                //When in debug mode, allow the server to not start listening
                // this is because there may be already another instance of the server running on this debug pc
                return;
#else
                throw new Exception();
#endif

            }
            this.UDPConn.Receiving = true;

            tcpListener = new TCPConnectionListener( ListenerPort );

            tcpListener.ClientConnected += mTCPListener_ClientConnected;
            tcpListener.ListenerError += mTCPListener_ListenerError;

            this.TCPListener.Listening = true;
        }


        public TCPConnectionListener TCPListener { get { return tcpListener; } }


        private List<ProxyClient> mClients;
        public List<ProxyClient> Clients
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.mClients; }
        }
        private void setClients( List<ProxyClient> value )
        {
            this.mClients = value;
        }



        ///'''Private mClients As Generic.Dictionary(Of IPEndPoint, Client) 
        ///'''Public ReadOnly Property Clients() As Generic.Dictionary(Of IPEndPoint, Client) 
        ///''' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///''' Return Me.mClients 
        ///''' End Get 
        ///'''End Property 
        ///'''Private Sub setClients(ByVal value As Generic.Dictionary(Of IPEndPoint, Client)) 
        ///''' Me.mClients = value 
        ///'''End Sub 



        ///''Public Sub TimeAsync(ByVal nTime As Integer, ByVal nTick As Integer, ByVal CL As Client) 
        ///'' Dim B As Byte() = New Byte(4 + 4 - 1) {} 
        ///'' BitConverter.GetBytes(nTime).CopyTo(B, 0) 
        ///'' BitConverter.GetBytes(nTick).CopyTo(B, 4) 

        ///'' Me.SendPacketToClient(ClientCommands.Time, B, CL, Protocol.UDP) 
        ///''End Sub 

        ///''Public Sub DeltaSnapshotAsync(ByVal nTick As Integer, ByVal nDS As DeltaSnapshot, ByVal CL As Client) 
        ///'' Dim BW As New ByteWriter 
        ///'' BW.Write(nTick) 
        ///'' nDS.WriteBytes(BW) 

        ///'' Dim B As Byte() = BW.ToBytes 
        ///'' BW.Close() 
        ///'' Me.SendPacketToClient(ClientCommands.DeltaSnapshot, B, CL, Protocol.UDP) 

        ///''End Sub 

        ///''Public Sub SignaleerVeranderingAsync(ByVal nObjID As Integer, ByVal nVersie As Integer, ByVal CL As Client) 
        ///'' Dim B As Byte() = New Byte(4 + 4) {} 
        ///'' BitConverter.GetBytes(nObjID).CopyTo(B, 0) 
        ///'' BitConverter.GetBytes(nVersie).CopyTo(B, 4) 

        ///'' Me.SendPacketToClient(ClientCommands.SignaleerVerandering, B, CL, Protocol.UDP) 
        ///''End Sub 

        ///''Public Sub OnVeranderingAsync(ByVal nEntID As Integer, ByVal nType As Integer, ByVal nData As Byte(), ByVal CL As Client) 
        ///'' Dim BW As New ByteWriter 
        ///'' BW.Write(nEntID) 
        ///'' BW.Write(nType) 
        ///'' BW.Write(nData) 
        ///'' Dim B As Byte() = BW.ToBytes 
        ///'' BW.Close() 

        ///'' Me.SendPacketToClient(ClientCommands.Verandering, B, CL, Protocol.UDP) 
        ///''End Sub 

        ///''Public Sub OnLaadWereldProgressChanged(ByVal nEnt As Entity, ByVal nCL As Client) 
        ///'' ' Dim B(4 + 4 + 4 - 1) As Byte 
        ///'' Dim B(4 + 4 - 1) As Byte 
        ///'' BitConverter.GetBytes(nEnt.EntityID).CopyTo(B, 0) 
        ///'' 'BitConverter.GetBytes(nEnt.EntityType).CopyTo(B, 4) 
        ///'' 'BitConverter.GetBytes(nEnt.Versie).CopyTo(B, 8) 
        ///'' BitConverter.GetBytes(nEnt.Versie).CopyTo(B, 4) 
        ///'' Me.SendPacketToClient(ClientCommands.LaadWereldReplyEntity, B, nCL, Protocol.UDP) 
        ///''End Sub 

        ///'''Public Sub SendPacketToClient(ByVal cmd As BaseClientCommands, ByVal data As Byte()) 
        ///''' Me.SendPacketToServer(cmd, data, Me.ServerEndPoint) 
        ///'''End Sub 


        ///''Protected Sub SendPacket(ByVal dgram As Byte(), ByVal nCL As Client, ByVal nProtocol As Protocol) 
        ///'' Select Case nProtocol 
        ///'' Case Protocol.TCP 
        ///'' nCL.TCPConn.SendPacket(dgram, TCPPacketBuilder.TCPPacketFlags.GZipCompressed) 'TODO: flags 
        ///'' Case Protocol.UDP 
        ///'' If nCL.UDPEndPoint Is Nothing Then Exit Sub 'Should give a warning or a message in the console 
        ///'' Me.SendUDPPacket(dgram, nCL.UDPEndPoint) 
        ///'' End Select 
        ///''End Sub 


        ///''Protected Sub SendPacketToClient(ByVal cmd As ClientCommands, ByVal data As Byte(), ByVal CL As Client, ByVal nProtocol As Protocol) 
        ///'' ' '' Dim TempName As String 
        ///'' ' '' If CL Is Nothing Then 
        ///'' ' '' TempName = "None" 
        ///'' ' '' Else 
        ///'' ' '' TempName = CL.Username 
        ///'' ' '' End If 
        ///'' ' '' System.IO.File.AppendAllText(Forms.Application.StartupPath & "\Logs\ServerNetwork.log", String.Format( _ 
        ///'' ' ''"{2,12}: Client: {3,-10} Packet Sending: {0,-10}: {1}", _ 
        ///'' ' ''data.Length & " bytes", cmd.ToString, Me.HoofdObj.Time, TempName) _ 
        ///'' ' ''& vbCrLf) 

        ///'' Dim B As Byte() = New Byte(data.Length) {} 'New Byte(data.Length +1-1) {} 
        ///'' B(0) = cmd 
        ///'' data.CopyTo(B, 1) 
        ///'' Me.SendPacket(B, CL, nProtocol) 
        ///''End Sub 

        ///''Protected Sub SendPacketToClient(ByVal cmd As ClientCommands, ByVal nINS As INetworkSerializable, ByVal CL As Client, ByVal nProtocol As Protocol) 


        ///'' Dim data As Byte() = nINS.ToNetworkBytes 


        ///'' ' '' Dim TempName As String 
        ///'' ' '' If CL Is Nothing Then 
        ///'' ' '' TempName = "None" 
        ///'' ' '' Else 
        ///'' ' '' TempName = CL.Username 
        ///'' ' '' End If 
        ///'' ' '' System.IO.File.AppendAllText(Forms.Application.StartupPath & "\Logs\ServerNetwork.log", String.Format( _ 
        ///'' ' ''"{2,12}: Client: {3,-10} Packet Sending: {0,-10}: {1}", _ 
        ///'' ' ''data.Length & " bytes", cmd.ToString, Me.HoofdObj.Time, TempName) _ 
        ///'' ' ''& vbCrLf) 


        ///'' Dim B As Byte() = New Byte(data.Length) {} 'New Byte(data.Length +1-1) {} 
        ///'' B(0) = cmd 
        ///'' data.CopyTo(B, 1) 

        ///'' Me.SendPacket(B, CL, nProtocol) 
        ///''End Sub 


        ///'''Public Sub PingAsync(ByVal Server As IPEndPoint) 
        ///''' 'Dim B As Byte() = New Byte(0) {} 
        ///''' 'B(0) = Communication.ServerCommands.Ping 

        ///''' Me.SendPacketToServer(ServerCommands.Ping, New Byte() {}, Server) 

        ///'''End Sub 
        ///'''Public Event PingCompleted As PingCompletedEventHandler 
        ///'''Public Delegate Sub PingCompletedEventHandler(ByVal sender As Object, ByVal e As PingCompletedEventArgs) 
        ///'''Public Class PingCompletedEventArgs 
        ///''' Inherits System.ComponentModel.AsyncCompletedEventArgs 

        ///''' Public Sub New(ByVal nError As Exception, ByVal cancelled As Boolean, ByVal userState As Object, ByVal nPingTime As Integer) 
        ///''' MyBase.New(nError, cancelled, userState) 
        ///''' Me.setPingTime(nPingTime) 
        ///''' End Sub 

        ///''' Private mPingTime As Integer 
        ///''' Public ReadOnly Property PingTime() As Integer 
        ///''' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///''' Return Me.mPingTime 
        ///''' End Get 
        ///''' End Property 
        ///''' Private Sub setPingTime(ByVal value As Integer) 
        ///''' Me.mPingTime = value 
        ///''' End Sub 

        ///'''End Class 





        ///''Public Event CommandRecievedFromClient(ByVal sender As Object, ByVal e As CommandRecievedFromClientEventArgs) 
        ///''Public Class CommandRecievedFromClientEventArgs 
        ///'' Inherits EventArgs 
        ///'' Public Sub New(ByVal cmd As ServerCommands, ByVal nBR As ByteReader, ByVal CL As Client, ByVal nEP As IPEndPoint) 'ByVal nData As Byte(), ByVal CL As Client) 
        ///'' Me.setCommand(cmd) 
        ///'' 'Me.setData(nData) 
        ///'' Me.setBR(nBR) 
        ///'' Me.setCL(CL) 
        ///'' Me.setEP(nEP) 
        ///'' End Sub 


        ///'' Private mCommand As ServerCommands 
        ///'' Public ReadOnly Property Command() As ServerCommands 
        ///'' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///'' Return Me.mCommand 
        ///'' End Get 
        ///'' End Property 
        ///'' Private Sub setCommand(ByVal value As ServerCommands) 
        ///'' Me.mCommand = value 
        ///'' End Sub 

        ///'' 'Private mData As Byte() 
        ///'' 'Public ReadOnly Property Data() As Byte() 
        ///'' ' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///'' ' Return Me.mData 
        ///'' ' End Get 
        ///'' 'End Property 
        ///'' 'Private Sub setData(ByVal value As Byte()) 
        ///'' ' Me.mData = value 
        ///'' 'End Sub 


        ///'' Private mBR As ByteReader 
        ///'' Public ReadOnly Property BR() As ByteReader 
        ///'' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///'' Return Me.mBR 
        ///'' End Get 
        ///'' End Property 
        ///'' Private Sub setBR(ByVal value As ByteReader) 
        ///'' Me.mBR = value 
        ///'' End Sub 


        ///'' Private mCL As Client 
        ///'' Public ReadOnly Property CL() As Client 
        ///'' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///'' Return Me.mCL 
        ///'' End Get 
        ///'' End Property 
        ///'' Private Sub setCL(ByVal value As Client) 
        ///'' Me.mCL = value 
        ///'' End Sub 


        ///'' Private mEP As IPEndPoint 
        ///'' Public ReadOnly Property EP() As IPEndPoint 
        ///'' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///'' Return Me.mEP 
        ///'' End Get 
        ///'' End Property 
        ///'' Private Sub setEP(ByVal value As IPEndPoint) 
        ///'' Me.mEP = value 
        ///'' End Sub 




        ///''End Class 




        ///''Public Sub DoGetEntityReplyAsync(ByVal Ent As Entity, ByVal CL As Client) 
        ///'' Dim BW As New ByteWriter 
        ///'' BW.Write(Ent.EntityID) 
        ///'' BW.Write(Ent.EntityType) 
        ///'' BW.Write(Ent.Versie) 

        ///'' Dim D As Byte() = Ent.GetData 
        ///'' BW.Write(D.Length) 
        ///'' BW.Write(D) 

        ///'' 'Dim B(4 + Ret.Length - 1) As Byte 
        ///'' 'e.Data.CopyTo(B, 0) 
        ///'' 'Ret.CopyTo(B, 4) 

        ///'' Dim B As Byte() = BW.ToBytes 
        ///'' BW.Close() 

        ///'' Me.SendPacketToClient(ClientCommands.GetEntityReply, B, CL, Protocol.TCP) 

        ///''End Sub 

        ///''Private Sub BaseClientCommunication_CommandRecievedFromClient(ByVal sender As Object, ByVal e As CommandRecievedFromClientEventArgs) Handles Me.CommandRecievedFromClient 
        ///'' ' Dim TempName As String 
        ///'' ' If e.CL Is Nothing Then 
        ///'' ' TempName = "None" 
        ///'' ' Else 
        ///'' ' TempName = e.CL.Username 
        ///'' ' End If 
        ///'' ' System.IO.File.AppendAllText(Forms.Application.StartupPath & "\Logs\ServerNetwork.log", String.Format( _ 
        ///'' '"{2,12}: Client: {3,-10} Packet Recieved: {0,-10}: {1}", _ 
        ///'' 'e.BR.BytesLeft.ToString & " bytes", e.Command.ToString, Me.HoofdObj.Time, TempName) _ 
        ///'' '& vbCrLf) 
        ///'' Select Case e.Command 
        ///'' Case ServerCommands.Ping 
        ///'' Me.SendPacketToClient(ClientCommands.PingReply, e.BR.ReadBytes(CInt(e.BR.BytesLeft)), e.CL, Protocol.UDP) 
        ///'' Case ServerCommands.PingReply 
        ///'' 'RaiseEvent PingCompleted(Me, New PingCompletedEventArgs(Nothing, False, Nothing, 0)) 

        ///'' Case ServerCommands.Login 
        ///'' 'Dim Username As Byte() = New Byte(e.Data.Length - 16 - 1) {} 
        ///'' 'Dim Password As Byte() = New Byte(15) {} 
        ///'' 'Array.Copy(e.Data, 0, Password, 0, 16) 
        ///'' 'Array.Copy(e.Data, 16, Username, 0, Username.Length) 


        ///'' Dim Ret As LoginResult 
        ///'' Ret = e.CL.Login(e.BR.ReadString, e.BR.ReadBytes(e.BR.ReadInt32)) 

        ///'' Dim BW As New ByteWriter 
        ///'' BW.Write(Ret) 
        ///'' If Ret = LoginResult.Succes Then 
        ///'' BW.Write(e.CL.LinkedPlayerEntity.EntityID) 
        ///'' BW.Write(e.CL.LoginAttemptID) 
        ///'' End If 

        ///'' Dim B As Byte() = BW.ToBytes 
        ///'' BW.Close() 

        ///'' Me.SendPacketToClient(ClientCommands.LoginReply, B, e.CL, Protocol.TCP) 

        ///'' Case ServerCommands.LinkUDPConnection 
        ///'' If e.CL IsNot Nothing Then Throw New Exception("Wierd exception at ClientCommunication; linkudpconnection") 
        ///'' Dim LoginAttemptID As Integer = e.BR.ReadInt32 
        ///'' Dim Count As Integer = 0 
        ///'' Dim LinkedClient As Client = Nothing 
        ///'' For Each CL As Client In Me.Clients 
        ///'' If CL.TryCompleteLoginAttempt(LoginAttemptID, e.EP) Then 
        ///'' Count += 1 
        ///'' LinkedClient = CL 
        ///'' End If 
        ///'' Next CL 

        ///'' If Count > 1 Then Throw New Exception("Wierd exception at ClientCommunication; linkudpconnection") 

        ///'' Me.SendPacketToClient(ClientCommands.LinkUDPConnectionReply, New Byte() {}, LinkedClient, Protocol.UDP) 

        ///'' Case ServerCommands.LaadWereld 
        ///'' SVMain.Wereld.LaadWereld(e.CL) 

        ///'' Case ServerCommands.GetEntity 
        ///'' Dim Ent As Entity 
        ///'' Ent = SVMain.Wereld.GetEntity(e.BR.ReadInt32) 
        ///'' If Ent IsNot Nothing Then 
        ///'' Me.DoGetEntityReplyAsync(Ent, e.CL) 
        ///'' End If 

        ///'' Case ServerCommands.PlayerCommand 
        ///'' Dim Builder As New PlayerCommandBuilder 
        ///'' 'Dim BR As New ByteReader(e.Data) 
        ///'' Builder.Read(e.BR) 
        ///'' 'BR.Close() 

        ///'' 'Dim PlCmd As PlayerCommands = CType(BR.ReadByte, PlayerCommands) 
        ///'' 'Dim Angles As Vector3 = BR.ReadVector3 

        ///'' 'BR.Close() 

        ///'' e.CL.ProcessClientCommand(Builder) 'PlCmd, Angles) 

        ///'' 'If e.CL.LinkedPlayerEntity IsNot Nothing Then 

        ///'' 'End If 

        ///'' 'Case ServerCommands.UpdateAngles 
        ///'' ' If e.CL.LinkedPlayerEntity IsNot Nothing Then 
        ///'' ' Dim BR As New ByteReader(e.Data) 
        ///'' ' Dim Angles As Vector3 = BR.ReadVector3 
        ///'' ' BR.Close() 

        ///'' ' e.CL.UpdateAngles(Angles) 
        ///'' ' End If 


        ///'' Case ServerCommands.GetModel 
        ///'' Dim ModelID As Integer = e.BR.ReadInt32 

        ///'' Dim MD As ModelData = SVMain.ModelManager.ModelData(ModelID) 
        ///'' If MD Is Nothing Then 
        ///'' 'Error: de client heeft een model opgegevraagd die niet bestaat 
        ///'' Exit Sub 
        ///'' Else 
        ///'' Me.SendPacketToClient(ClientCommands.GetModelReply, MD, e.CL, Protocol.UDP) 
        ///'' End If 



        ///'' '''''' 
        ///'' '''''ADMIN 
        ///'' '''''' 



        ///'' Case ServerCommands.ADMINGetGameFilesList 
        ///'' Dim table As TheWizardsDataSet.GameFilesDataTable 
        ///'' table = SVMain.DB.GameFiles.GetData 

        ///'' Dim CFL As New GameFilesList 
        ///'' Dim Description As String 
        ///'' Dim LocalFile As String 
        ///'' Dim Hash As Byte() 
        ///'' For Each R As TheWizardsDataSet.GameFilesRow In table 
        ///'' If R.IsDescriptionNull Then 
        ///'' Description = "(none)" 
        ///'' Else 
        ///'' Description = R.Description 
        ///'' End If 
        ///'' If R.IsLocalFileNull Then 
        ///'' LocalFile = "(standard)" 
        ///'' Else 
        ///'' LocalFile = R.LocalFile 
        ///'' End If 
        ///'' If R.IsHashNull Then 
        ///'' Hash = New Byte(0) {} 
        ///'' Else 
        ///'' Hash = R.Hash 
        ///'' End If 
        ///'' CFL.Files.Add(R.ID, New GameFile( _ 
        ///'' R.ID, R.Filename, Description, R.RelativePath, LocalFile, _ 
        ///'' R.Version, CType(R.Type, GameFile.GameFileType), R.Enabled > 0, Hash)) 

        ///'' Next 

        ///'' Me.SendPacketToClient(ClientCommands.ADMINGetGameFilesListReply, CFL, e.CL, Protocol.TCP) 












        ///'' Case ServerCommands.ADMINAddGameFile 
        ///'' Dim ACFP As AddGameFilePacket = AddGameFilePacket.FromNetworkBytes(e.BR) 
        ///'' Dim FullDirectory As String = SVMain.ServerSettings.GameFilesPath & "\GameDataTest" & ACFP.RelativePath & "\" 
        ///'' IO.Directory.CreateDirectory(FullDirectory) 
        ///'' Dim FullPath As String = FullDirectory & ACFP.FileName 

        ///'' IO.File.WriteAllBytes(FullPath, ACFP.FileData) 
        ///'' Dim GameFileID As Integer 
        ///'' GameFileID = SVMain.UpdateGameFile(New IO.FileInfo(FullPath)) 

        ///'' SVMain.DB.GameFiles.UpdateEnabled(1, GameFileID) 


        ///'' Me.SendPacketToClient(ClientCommands.ADMINAddGameFileReply, BitConverter.GetBytes(GameFileID), e.CL, Protocol.TCP) 

        ///'' Case ServerCommands.ADMINGetModelList 
        ///'' Dim table As TheWizardsDataSet.ModelsDataTable 
        ///'' table = SVMain.DB.Models.GetData 

        ///'' Dim ML As New ModelsList 

        ///'' Dim Name As String 
        ///'' Dim GameFileID As Integer 
        ///'' Dim CollisionFileID As Integer 
        ///'' Dim CustomData As Byte() 
        ///'' Dim Versie As Integer 

        ///'' For Each R As TheWizardsDataSet.ModelsRow In table 
        ///'' If R.IsnameNull Then 
        ///'' Name = "(geen naam)" 
        ///'' Else 
        ///'' Name = R.name 
        ///'' End If 
        ///'' If R.IsMeshFileNull Then 
        ///'' GameFileID = -1 
        ///'' Else 
        ///'' GameFileID = R.MeshFile 
        ///'' End If 
        ///'' If R.IsCollisionFileNull Then 
        ///'' CollisionFileID = -1 
        ///'' Else 
        ///'' CollisionFileID = R.CollisionFile 
        ///'' End If 
        ///'' If R.IsCustomDataNull Then 
        ///'' CustomData = New Byte() {} 
        ///'' Else 
        ///'' CustomData = R.CustomData 
        ///'' End If 
        ///'' If R.IsVersieNull Then 
        ///'' Versie = -1 
        ///'' Else 
        ///'' Versie = R.Versie 
        ///'' End If 
        ///'' Dim AMD As New AdminModelData(R.ID, Name, GameFileID, CollisionFileID, CustomData, Versie) 
        ///'' ML.Models.Add(AMD) 

        ///'' Next 

        ///'' Me.SendPacketToClient(ClientCommands.ADMINGetModelListReply, ML, e.CL, Protocol.TCP) 

        ///'' Case ServerCommands.ADMINUpdateModel 
        ///'' Dim AMD As AdminModelData = AdminModelData.FromNetworkBytes(e.BR) 

        ///'' Dim GameFile As Nullable(Of Integer) 
        ///'' Dim CollisionFile As Nullable(Of Integer) 
        ///'' If AMD.GameFile = -1 Then 
        ///'' GameFile = New Nullable(Of Integer) 
        ///'' Else 
        ///'' GameFile = AMD.GameFile 
        ///'' End If 
        ///'' If AMD.CollisionFile = -1 Then 
        ///'' CollisionFile = New Nullable(Of Integer) 
        ///'' Else 
        ///'' CollisionFile = AMD.CollisionFile 
        ///'' End If 


        ///'' If AMD.ModelID = -1 Then 
        ///'' 'add a new model 

        ///'' SVMain.DB.Models.AddModel(AMD.Name, GameFile, CollisionFile, AMD.CustomData, 1) 
        ///'' Else 
        ///'' Dim Table As TheWizardsDataSet.ModelsDataTable 
        ///'' Table = SVMain.DB.Models.GetModelData(AMD.ModelID) 
        ///'' If Table.Count = 0 Then 
        ///'' Throw New Exception("Error") 
        ///'' ElseIf Table.Count > 1 Then 
        ///'' Throw New Exception("Error") 
        ///'' Else 
        ///'' Dim R As TheWizardsDataSet.ModelsRow 
        ///'' R = Table(0) 

        ///'' SVMain.DB.Models.UpdateModel(AMD.Name, GameFile, CollisionFile, AMD.CustomData, R.Versie + 1, R.ID) 
        ///'' End If 

        ///'' End If 

        ///'' SVMain.ModelManager.ReloadStaticEntities() 

        ///'' Me.SendPacketToClient(ClientCommands.ADMINUpdateModelReply, New Byte() {}, e.CL, Protocol.TCP) 

        ///'' Case ServerCommands.ADMINPutStaticEntity 
        ///'' Dim P As PutStaticEntityPacket = PutStaticEntityPacket.FromNetworkBytes(e.BR) 


        ///'' Dim S As StaticEntity = DirectCast(SVMain.CreateEntity(EntityType.StaticEntity, SVMain.Wereld.Tree), StaticEntity) 
        ///'' CType(S.StaticEntityFunctions, ServerStaticEntityFunctions).ServerLaadModel(P.ModelID) 
        ///'' S.Functions.Positie = P.Positie 




        ///'' End Select 

        ///''End Sub 



        ///''Public Class ClientDisconnectedEventArgs 
        ///'' Inherits System.EventArgs 

        ///'' Public Sub New(ByVal nClient As Client) 
        ///'' Me.setClient(nClient) 
        ///'' End Sub 

        ///'' Private mClient As Client 
        ///'' Public ReadOnly Property Client() As Client 
        ///'' <System.Diagnostics.DebuggerStepThrough()> Get 
        ///'' Return Me.mClient 
        ///'' End Get 
        ///'' End Property 
        ///'' Private Sub setClient(ByVal value As Client) 
        ///'' Me.mClient = value 
        ///'' End Sub 

        ///''End Class 
        ///''Public Sub OnClientDisconnected(ByVal sender As Object, ByVal e As ClientDisconnectedEventArgs) 
        ///'' RaiseEvent ClientDisconnected(Me, e) 
        ///''End Sub 
        ///''Public Event ClientDisconnected(ByVal sender As Object, ByVal e As ClientDisconnectedEventArgs) 



        ///''Private Sub ClientCommunication_ClientDisconnected(ByVal sender As Object, ByVal e As ClientDisconnectedEventArgs) Handles Me.ClientDisconnected 
        ///'' Me.Clients.Remove(e.Client) 
        ///'' e.Client.Dispose() 

        ///''End Sub 

        public void OnClientConnected( object sender, Common.Networking.TCPConnectionListener.ClientConnectedEventArgs e )
        {
            ProxyClient CL = new ProxyClient( (ServerMainNew)this.HoofdObj, new TCPConnection( e.CL ) );
            this.Clients.Add( CL );
            ///''RaiseEvent ClientConnected(Me, e) 
        }
        ///''Public Event ClientConnected(ByVal sender As Object, ByVal e As Common.Networking.TCPConnectionListener.ClientConnectedEventArgs) 

        ///''Protected Overrides Sub DisposeObject() 
        ///'' MyBase.DisposeObject() 
        ///'' Me.TCPConnListener.Dispose() 
        ///''End Sub 

        private void mTCPListener_ClientConnected( object sender, Common.Networking.TCPConnectionListener.ClientConnectedEventArgs e )
        {
            this.HoofdObj.Invoker.Invoke( OnClientConnected, sender, e );

            ///'' Me.Scheduler.ScheduleAction(AddressOf Me.OnClientConnected, sender, e, 0) 
        }

        private void mTCPListener_ListenerError( object sender, Common.Networking.TCPConnectionListener.ListenerErrorEventArgs e )
        {


        }


        public void SendPacketUDP( Communication.ClientCommands cmd, byte[] data, IPEndPoint nClientEndPoint )
        {
#if DEBUG
            if ( data == null )
                throw new ArgumentNullException( "data" );
#endif
            byte[] B;

            B = new byte[ data.Length + 1 ];
            //New Byte(data.Length +1-1) {} 

            B[ 0 ] = (byte)cmd;

            data.CopyTo( B, 1 );

            this.UDPConn.SendPacket( B, nClientEndPoint );

        }

        protected override void OnUDPPacketRecieved( object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e )
        {
            base.OnUDPPacketRecieved( sender, e );
            ProxyClient CL = null;
            ProxyClient iCL = null;


            for ( int I = 0; I <= this.Clients.Count - 1; I++ )
            {
                iCL = this.Clients[ I ];
                if ( iCL.UDPEndPoint != null )
                {
                    if ( iCL.UDPEndPoint.Equals( e.EP ) )
                    {
                        CL = iCL;
                        break; // TODO: might not be correct. Was : Exit For 
                    }
                }
            }

            this.OnPacketRecieved( e, CL );

        }


        protected void OnPacketRecieved( BaseConnection.PacketRecievedEventArgs e, ProxyClient nCL )
        {
            if ( nCL == null )
            {
                if ( e.Dgram.Length > 0 & e.Dgram[ 0 ] == (byte)Communication.ServerCommands.LinkUDPConnection )
                {
                    string LoginKey = System.Text.ASCIIEncoding.ASCII.GetString( e.Dgram, 1, e.Dgram.Length - 1 );
                    ProxyClient ICL;
                    for ( int I = 0; I <= this.Clients.Count - 1; I++ )
                    {
                        ICL = this.Clients[ I ];
                        if ( ICL.GameClient.HasTemporaryLoginKey )
                        {
                            if ( ICL.GameClient.GetTemporaryLoginKey() == LoginKey )
                            {
                                ICL.setUDPEndPoint( e.EP );
                                ICL.GameClient.ClearTemporaryLoginKey();
                                ICL.OnUDPConnectionLinked();

                            }
                        }
                    }
                }
            }
            else
            {
                nCL.OnPacketRecieved( this, e );
            }




        }


        public override void Dispose()
        {
            base.Dispose();
            if ( this.TCPListener != null )
            {
                tcpListener.Dispose();
            }
            tcpListener = null;
        }
    }

}

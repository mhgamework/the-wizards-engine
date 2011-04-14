using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Common.Networking;

namespace MHGameWork.TheWizards.ServerClient.Network
{

    public class ClientCommunication : Communication
    {
        private ProxyServer proxy;
        private TCPConnection tcpConnection;

        private IPAddress serverAddress;

        public IPAddress ServerAddress
        {
            get { return serverAddress; }
            set 
            {
                serverAddress = value;
                setServerUDPEndPoint( new IPEndPoint( serverAddress, serverUDPPort ) );
            }
        }
        private int serverUDPPort;
        private int serverTCPPort;

        private bool connected;

        public ClientCommunication(ProxyServer nProxy, IPAddress nServerAddress, int nServerUDPPort, int nServerTCPPort)
            : base(nProxy.Main)
        {
            proxy = nProxy;

            
            serverUDPPort = nServerUDPPort;
            serverTCPPort = nServerTCPPort;

            ServerAddress = nServerAddress;
            

            



            ///''Me.setClient(New Client(Me, nServerAddress, nServerUDPPort, nServerTCPPort)) 
            //Me.Client.setUDPEndPoint(New IPEndPoint(nServerAddress, nServerUDPPort)) 
            //Me.Client.TCPConn.Connect(nServerAddress, nServerTCPPort) 


        }


        // '' ''Private mClient As Client 
        // '' ''Public ReadOnly Property Client() As Client 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mClient 
        // '' '' End Get 
        // '' ''End Property 
        // '' ''Private Sub setClient(ByVal value As Client) 
        // '' '' Me.mClient = value 
        // '' ''End Sub 


        public void ConnectAsync()
        {
            if (tcpConnection != null) tcpConnection.Dispose();
            tcpConnection = null;
            connected = false;


            tcpConnection = new TCPConnection(new TcpClient());

            tcpConnection.ConnectedToServer += mTCPConnection_ConnectedToServer;
            tcpConnection.ConnectError += mTCPConnection_ConnectError;
            tcpConnection.NetworkErrorAsync += mTCPConnection_NetworkErrorAsync;
            tcpConnection.PacketRecievedAsync += mTCPConnection_PacketRecievedAsync;



            tcpConnection.Connect(serverAddress, serverTCPPort);
        }











        private IPEndPoint mServerUDPEndPoint;
        public IPEndPoint ServerUDPEndPoint
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.mServerUDPEndPoint; }
        }
        private void setServerUDPEndPoint(IPEndPoint value)
        {
            this.mServerUDPEndPoint = value;
        }

        public void SendPacketUDP(ServerCommands cmd)
        {
            this.SendPacketUDP(cmd, new byte[1]);
        }
        public void SendPacketUDP(ServerCommands cmd, INetworkSerializable nINS)
        {
            this.SendPacketUDP(cmd, nINS.ToNetworkBytes());
        }

        public void SendPacketUDP(ServerCommands cmd, byte[] data)
        {
#if DEBUG
            if (data == null)
                throw new ArgumentNullException("data");
#endif
            byte[] B;

            B = new byte[data.Length + 1];
            //New Byte(data.Length +1-1) {} 

            B[0] = (byte)cmd;
            data.CopyTo(B, 1);

            this.UDPConn.SendPacket(B, this.ServerUDPEndPoint);
            //TODO: this is an akward place to start receiving
            if (UDPConn.Receiving == false) UDPConn.Receiving = true;

        }

        public void SendPacketTCP(ServerCommands cmd)
        {
            this.SendPacketTCP(cmd, new byte[1]);
        }
        public void SendPacketTCP(ServerCommands cmd, INetworkSerializable nINS)
        {
            this.SendPacketTCP(cmd, nINS.ToNetworkBytes());
        }

        public void SendPacketTCP(ServerCommands cmd, byte[] data)
        {
#if DEBUG
            if (data == null)
                throw new ArgumentNullException("data");
#endif
            byte[] B;

            B = new byte[data.Length + 1];
            //New Byte(data.Length +1-1) {} 

            B[0] = (byte)cmd;
            data.CopyTo(B, 1);
            this.TCPConnection.SendPacket(B, TCPPacketBuilder.TCPPacketFlags.None);

        }

        // '' ''Protected Overrides Sub SendUDPPacket(ByVal dgram() As Byte, ByVal nEndPoint As System.Net.IPEndPoint) 
        // '' '' MyBase.SendUDPPacket(dgram, nEndPoint) 
        // '' '' Me.UDPConn.Receiving = True 'Socket.Send binds de socket aan een willekeurige poort bij de client, dus zorgt dit ervoor dat de client begint te receiven van wanneer hij zo een random poort heeft gekregen 
        // '' ''End Sub 

        // '' ''Protected Sub SendPacket(ByVal dgram As Byte(), ByVal nProtocol As Protocol) 
        // '' '' Select Case nProtocol 
        // '' '' Case Protocol.TCP 
        // '' '' If Me.Client.TCPConn.TCP.Connected = False Then Exit Sub 'TODO: packets zouden moeten worden gequeed wanneer de socket nog niet verbonden is. 
        // '' '' Me.Client.TCPConn.SendPacket(dgram, TCPPacketBuilder.TCPPacketFlags.GZipCompressed) 'TODO: flags 
        // '' '' Case Protocol.UDP 
        // '' '' Me.SendUDPPacket(dgram, Me.Client.UDPEndPoint) 
        // '' '' End Select 
        // '' ''End Sub 

        // '' ''Public Sub SendPacketToServer(ByVal cmd As ServerCommands, ByVal nINS As INetworkSerializable, ByVal nProtocol As Protocol) 
        // '' '' Dim data As Byte() = nINS.ToNetworkBytes 
        // '' '' Me.SendPacketToServer(cmd, data, nProtocol) 

        // '' ''End Sub 
        // '' ''Public Sub SendPacketToServer(ByVal cmd As ServerCommands, ByVal data As Byte(), ByVal nProtocol As Protocol) 
        // '' '' Dim B As Byte() 
        // '' '' If data Is Nothing Then 
        // '' '' B = New Byte(0) {} 
        // '' '' Else 
        // '' '' B = New Byte(data.Length) {} 'New Byte(data.Length +1-1) {} 
        // '' '' End If 
        // '' '' B(0) = cmd 
        // '' '' If data IsNot Nothing Then data.CopyTo(B, 1) 
        // '' '' Me.SendPacket(B, nProtocol) 
        // '' ''End Sub 

        //' '' ''Public Sub SendPacketToServer(ByVal cmd As ServerCommands, ByVal data As Byte(), ByVal Server As IPEndPoint) 
        //' '' '' Dim B As Byte() 
        //' '' '' If data Is Nothing Then 
        //' '' '' B = New Byte(0) {} 
        //' '' '' Else 
        //' '' '' B = New Byte(data.Length) {} 'New Byte(data.Length +1-1) {} 
        //' '' '' End If 
        //' '' '' B(0) = cmd 
        //' '' '' If data IsNot Nothing Then data.CopyTo(B, 1) 
        //' '' '' Me.SendPacket(B, Server) 
        //' '' ''End Sub 


        // '' ''Public Sub OnPacketRecieved(ByVal e As Common.Communication.PacketRecievedEventArgs) 
        // '' '' Dim cmd As Byte = e.Dgram(0) 
        // '' '' Dim data As Byte() = New Byte(e.Dgram.Length - 1 - 1) {} 
        // '' '' Array.Copy(e.Dgram, 1, data, 0, data.Length) 
        // '' '' If Not System.Enum.IsDefined(GetType(Communication.ClientCommands), cmd) Then 
        // '' '' Stop 
        // '' '' End If 
        // '' '' Dim Command As Communication.ClientCommands = CType(cmd, Communication.ClientCommands) 

        // '' '' Dim BR As New ByteReader(data) 
        // '' '' RaiseEvent CommandRecievedFromServer(Me, New CommandRecievedFromServerEventArgs(Command, BR)) 
        // '' '' If BR IsNot Nothing Then BR.Close() 





        // '' ''End Sub 

        // '' ''Protected Overrides Sub OnUDPPacketRecieved(ByVal e As Common.Communication.PacketRecievedEventArgs) 
        // '' '' MyBase.OnUDPPacketRecieved(e) 
        // '' '' Me.OnPacketRecieved(e) 
        // '' ''End Sub 

        // '' ''Public Event CommandRecievedFromServer(ByVal sender As Object, ByVal e As CommandRecievedFromServerEventArgs) 
        // '' ''Public Class CommandRecievedFromServerEventArgs 
        // '' '' Inherits EventArgs 
        // '' '' Public Sub New(ByVal cmd As ClientCommands, ByVal nBR As ByteReader) ', ByVal nData As Byte()) 
        // '' '' Me.setCommand(cmd) 
        // '' '' Me.setBR(nBR) 
        // '' '' 'Me.setData(nData) 
        // '' '' End Sub 


        // '' '' Private mCommand As ClientCommands 
        // '' '' Public ReadOnly Property Command() As ClientCommands 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mCommand 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setCommand(ByVal value As ClientCommands) 
        // '' '' Me.mCommand = value 
        // '' '' End Sub 

        // '' '' 'Private mData As Byte() 
        // '' '' 'Public ReadOnly Property Data() As Byte() 
        // '' '' ' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' ' Return Me.mData 
        // '' '' ' End Get 
        // '' '' 'End Property 
        // '' '' 'Private Sub setData(ByVal value As Byte()) 
        // '' '' ' Me.mData = value 
        // '' '' 'End Sub 


        // '' '' Private mBR As ByteReader 
        // '' '' Public ReadOnly Property BR() As ByteReader 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mBR 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setBR(ByVal value As ByteReader) 
        // '' '' Me.mBR = value 
        // '' '' End Sub 



        // '' ''End Class 




        // '' ''Public Sub GetGameFilesListAsync() 
        // '' '' Me.SendPacketToServer(ServerCommands.ADMINGetGameFilesList, New Byte(0) {}, Protocol.UDP) 
        // '' ''End Sub 




        // '' ''Public Sub PingAsync() 'ByVal Server As IPEndPoint) 
        // '' '' 'Dim B As Byte() = New Byte(0) {} 
        // '' '' 'B(0) = Communication.ServerCommands.Ping 

        // '' '' Me.SendPacketToServer(ServerCommands.Ping, System.BitConverter.GetBytes(Me.HoofdObj.Time), Protocol.UDP) ', Server) 

        // '' ''End Sub 
        // '' ''Public Event PingCompleted As PingCompletedEventHandler 
        // '' ''Public Delegate Sub PingCompletedEventHandler(ByVal sender As Object, ByVal e As PingCompletedEventArgs) 
        // '' ''Public Class PingCompletedEventArgs 
        // '' '' Inherits System.ComponentModel.AsyncCompletedEventArgs 

        // '' '' Public Sub New(ByVal nError As Exception, ByVal cancelled As Boolean, ByVal userState As Object, ByVal nPingTime As Integer) 
        // '' '' MyBase.New(nError, cancelled, userState) 
        // '' '' Me.setPingTime(nPingTime) 
        // '' '' End Sub 

        // '' '' Private mPingTime As Integer 
        // '' '' Public ReadOnly Property PingTime() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mPingTime 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setPingTime(ByVal value As Integer) 
        // '' '' Me.mPingTime = value 
        // '' '' End Sub 

        // '' ''End Class 


        // '' ''Public Sub LoginAsync(ByVal nUsername As String, ByVal nPassword As String) 
        // '' '' 'Dim B As Byte() = New Byte(16 + nUsername.Length - 1) {} 
        // '' '' Dim Crypt As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create() 
        // '' '' Dim HashedBytes As Byte() 
        // '' '' HashedBytes = Crypt.ComputeHash(System.Text.Encoding.ASCII.GetBytes(nPassword)) 

        // '' '' 'HashedBytes.CopyTo(B, 0) 
        // '' '' 'System.Text.Encoding.ASCII.GetBytes(nUsername).CopyTo(B, HashedBytes.Length) 

        // '' '' Dim BW As New ByteWriter 
        // '' '' BW.Write(nUsername) 
        // '' '' BW.Write(HashedBytes.Length) 
        // '' '' BW.Write(HashedBytes) 

        // '' '' Dim B As Byte() = BW.ToBytes 
        // '' '' BW.Close() 
        // '' '' Me.SendPacketToServer(ServerCommands.Login, B, Protocol.TCP) 
        // '' ''End Sub 
        // '' ''Public Event LoginCompleted As LoginCompletedEventHandler 
        // '' ''Public Delegate Sub LoginCompletedEventHandler(ByVal sender As Object, ByVal e As LoginCompletedEventArgs) 
        // '' ''Public Class LoginCompletedEventArgs 
        // '' '' Inherits System.ComponentModel.AsyncCompletedEventArgs 
        // '' '' Public Sub New(ByVal nError As Exception, ByVal cancelled As Boolean, ByVal userState As Object, ByVal nResult As LoginResult, ByVal nPlayerEntityID As Integer, ByVal nLoginAttemptID As Integer) 
        // '' '' MyBase.New(nError, cancelled, userState) 
        // '' '' Me.setResult(nResult) 
        // '' '' Me.setPlayerEntityID(nPlayerEntityID) 
        // '' '' Me.setLoginAttemptID(nLoginAttemptID) 
        // '' '' End Sub 

        // '' '' Private mResult As LoginResult 
        // '' '' Public ReadOnly Property Result() As LoginResult 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mResult 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setResult(ByVal value As LoginResult) 
        // '' '' Me.mResult = value 
        // '' '' End Sub 


        // '' '' Private mPlayerEntityID As Integer 
        // '' '' Public ReadOnly Property PlayerEntityID() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mPlayerEntityID 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setPlayerEntityID(ByVal value As Integer) 
        // '' '' Me.mPlayerEntityID = value 
        // '' '' End Sub 


        // '' '' Private mLoginAttemptID As Integer 
        // '' '' Public ReadOnly Property LoginAttemptID() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mLoginAttemptID 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setLoginAttemptID(ByVal value As Integer) 
        // '' '' Me.mLoginAttemptID = value 
        // '' '' End Sub 



        // '' ''End Class 

        // '' ''Public Sub LinkUDPConnectionAsync(ByVal nLoginAttemptID As Integer) 
        // '' '' Dim BW As New ByteWriter 
        // '' '' Dim B As Byte() 
        // '' '' BW.Write(nLoginAttemptID) 
        // '' '' B = BW.ToBytes 

        // '' '' BW.Close() 

        // '' '' Me.SendPacketToServer(ServerCommands.LinkUDPConnection, B, Protocol.UDP) 
        // '' ''End Sub 


        // '' ''Public Sub OnLinkUDPConnectionCompleted(ByVal sender As Object, ByVal e As EventArgs) 
        // '' '' RaiseEvent LinkUDPConnectionCompleted(sender, e) 
        // '' ''End Sub 
        // '' ''Public Event LinkUDPConnectionCompleted(ByVal sender As Object, ByVal e As EventArgs) 



        // '' ''Public Sub LaadWereldAsync() 
        // '' '' Me.SendPacketToServer(ServerCommands.LaadWereld, New Byte(0) {}, Protocol.UDP) 
        // '' ''End Sub 



        // '' ''Public Event LaadWereldCompleted As LaadWereldCompletedEventHandler 
        // '' ''Public Event LaadWereldProgressChanged As LaadWereldProgressChangedEventHandler 



        // '' ''Public Sub SendPlayerCommandAsync(ByVal nBuilder As PlayerCommandBuilder) 'nPlayerCMD As PlayerCommands, ByVal nAngles As Vector3) 
        // '' '' Dim BW As New ByteWriter 
        // '' '' BW.Write(nBuilder.ToBytes) 
        // '' '' 'BW.Write(nAngles) 
        // '' '' Dim B As Byte() = BW.ToBytes 
        // '' '' BW.Close() 

        // '' '' Me.SendPacketToServer(ServerCommands.PlayerCommand, B, Protocol.UDP) 
        // '' ''End Sub 

        //' '' ''Public Sub UpdateAngles(ByVal nAngles As Vector3) 
        //' '' '' Dim BW As New ByteWriter 
        //' '' '' BW.Write(nAngles) 
        //' '' '' Dim B As Byte() = BW.ToBytes 
        //' '' '' BW.Close() 

        //' '' '' Me.SendPacketToServer(ServerCommands.UpdateAngles, B) 
        //' '' ''End Sub 







        // '' ''Public Class SignaleerVeranderingEventArgs 
        // '' '' Inherits System.EventArgs 
        // '' '' Public Sub New(ByVal nEntID As Integer, ByVal nVersie As Integer) 
        // '' '' Me.setEntityID(nEntID) 
        // '' '' Me.setVersie(nVersie) 
        // '' '' End Sub 


        // '' '' Private mEntityID As Integer 
        // '' '' Public ReadOnly Property EntityID() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mEntityID 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setEntityID(ByVal value As Integer) 
        // '' '' Me.mEntityID = value 
        // '' '' End Sub 



        // '' '' Private mVersie As Integer 
        // '' '' Public ReadOnly Property Versie() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mVersie 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setVersie(ByVal value As Integer) 
        // '' '' Me.mVersie = value 
        // '' '' End Sub 


        // '' ''End Class 
        // '' ''Public Sub OnSignaleerVerandering(ByVal sender As Object, ByVal e As SignaleerVeranderingEventArgs) 
        // '' '' RaiseEvent SignaleerVerandering(Me, e) 
        // '' ''End Sub 
        // '' ''Public Event SignaleerVerandering(ByVal sender As Object, ByVal e As SignaleerVeranderingEventArgs) 



        // '' ''Public Sub GetEntityAsync(ByVal nEntID As Integer) 
        // '' '' Dim B As Byte() = New Byte(4 - 1) {} 
        // '' '' BitConverter.GetBytes(nEntID).CopyTo(B, 0) 
        // '' '' Me.SendPacketToServer(ServerCommands.GetEntity, B, Protocol.UDP) 
        // '' ''End Sub 
        // '' ''Public Event GetEntityCompleted As GetEntityCompletedEventHandler 
        // '' ''Public Delegate Sub GetEntityCompletedEventHandler(ByVal sender As Object, ByVal e As GetEntityCompletedEventArgs) 
        // '' ''Public Class GetEntityCompletedEventArgs 
        // '' '' Inherits System.ComponentModel.AsyncCompletedEventArgs 
        // '' '' Public Sub New(ByVal nError As Exception, ByVal cancelled As Boolean, ByVal userState As Object, ByVal nEntityID As Integer, ByVal nType As EntityType, ByVal nVersie As Integer, ByVal nData As Byte()) 
        // '' '' MyBase.New(nError, cancelled, userState) 
        // '' '' Me.setEntityID(nEntityID) 
        // '' '' Me.setEntityType(nType) 
        // '' '' Me.setVersie(nVersie) 
        // '' '' Me.setData(nData) 
        // '' '' End Sub 


        // '' '' Private mEntityID As Integer 
        // '' '' Public ReadOnly Property EntityID() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mEntityID 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setEntityID(ByVal value As Integer) 
        // '' '' Me.mEntityID = value 
        // '' '' End Sub 


        // '' '' Private mEntityType As EntityType 
        // '' '' Public ReadOnly Property EntityType() As EntityType 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mEntityType 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setEntityType(ByVal value As EntityType) 
        // '' '' Me.mEntityType = value 
        // '' '' End Sub 


        // '' '' Private mVersie As Integer 
        // '' '' Public ReadOnly Property Versie() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mVersie 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setVersie(ByVal value As Integer) 
        // '' '' Me.mVersie = value 
        // '' '' End Sub 



        // '' '' Private mData As Byte() 
        // '' '' Public ReadOnly Property Data() As Byte() 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mData 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setData(ByVal value As Byte()) 
        // '' '' Me.mData = value 
        // '' '' End Sub 


        // '' ''End Class 


        // '' ''Public Class VeranderingEventArgs 
        // '' '' Inherits System.EventArgs 
        // '' '' Public Sub New(ByVal nEntID As Integer, ByVal nType As Integer, ByVal nData As Byte()) 
        // '' '' Me.setEntityID(nEntID) 
        // '' '' Me.setType(nType) 
        // '' '' Me.setData(nData) 
        // '' '' End Sub 


        // '' '' Private mEntityID As Integer 
        // '' '' Public ReadOnly Property EntityID() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mEntityID 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setEntityID(ByVal value As Integer) 
        // '' '' Me.mEntityID = value 
        // '' '' End Sub 




        // '' '' Private mType As Integer 
        // '' '' Public ReadOnly Property Type() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mType 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setType(ByVal value As Integer) 
        // '' '' Me.mType = value 
        // '' '' End Sub 


        // '' '' Private mData As Byte() 
        // '' '' Public ReadOnly Property Data() As Byte() 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mData 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setData(ByVal value As Byte()) 
        // '' '' Me.mData = value 
        // '' '' End Sub 


        // '' ''End Class 
        // '' ''Public Sub OnVerandering(ByVal sender As Object, ByVal e As VeranderingEventArgs) 
        // '' '' RaiseEvent Verandering(Me, e) 
        // '' ''End Sub 
        // '' ''Public Event Verandering(ByVal sender As Object, ByVal e As VeranderingEventArgs) 



        // '' ''Public Class DeltaSnapshotEventArgs 
        // '' '' Inherits System.EventArgs 
        // '' '' Public Sub New(ByVal nTick As Integer, ByVal nDS As DeltaSnapshot) 
        // '' '' Me.setTick(nTick) 
        // '' '' Me.setDeltaSnapshot(nDS) 
        // '' '' End Sub 

        // '' '' Private mTick As Integer 
        // '' '' Public ReadOnly Property Tick() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mTick 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setTick(ByVal value As Integer) 
        // '' '' Me.mTick = value 
        // '' '' End Sub 


        // '' '' Private mDeltaSnapshot As DeltaSnapshot 
        // '' '' Public ReadOnly Property DeltaSnapshot() As DeltaSnapshot 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mDeltaSnapshot 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setDeltaSnapshot(ByVal value As DeltaSnapshot) 
        // '' '' Me.mDeltaSnapshot = value 
        // '' '' End Sub 


        // '' ''End Class 
        // '' ''Public Sub OnDeltaSnapshot(ByVal sender As Object, ByVal e As DeltaSnapshotEventArgs) 
        // '' '' RaiseEvent DeltaSnapshot(Me, e) 
        // '' ''End Sub 
        // '' ''Public Event DeltaSnapshot(ByVal sender As Object, ByVal e As DeltaSnapshotEventArgs) 


        // '' ''Public Class TimeEventArgs 
        // '' '' Inherits System.EventArgs 

        // '' '' Public Sub New(ByVal nTime As Integer, ByVal nTick As Integer) 
        // '' '' Me.setTime(nTime) 
        // '' '' Me.setTick(nTick) 
        // '' '' End Sub 

        // '' '' Private mTime As Integer 
        // '' '' Public ReadOnly Property Time() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mTime 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setTime(ByVal value As Integer) 
        // '' '' Me.mTime = value 
        // '' '' End Sub 


        // '' '' Private mTick As Integer 
        // '' '' Public ReadOnly Property Tick() As Integer 
        // '' '' <System.Diagnostics.DebuggerStepThrough()> Get 
        // '' '' Return Me.mTick 
        // '' '' End Get 
        // '' '' End Property 
        // '' '' Private Sub setTick(ByVal value As Integer) 
        // '' '' Me.mTick = value 
        // '' '' End Sub 

        // '' ''End Class 
        // '' ''Public Sub OnTime(ByVal sender As Object, ByVal e As TimeEventArgs) 
        // '' '' RaiseEvent Time(Me, e) 
        // '' ''End Sub 
        // '' ''Public Event Time(ByVal sender As Object, ByVal e As TimeEventArgs) 


        // '' ''Public Sub GetModelAsync(ByVal nModelID As Integer) 
        // '' '' Me.SendPacketToServer(ServerCommands.GetModel, BitConverter.GetBytes(nModelID), Protocol.UDP) 

        // '' ''End Sub 



        // '' ''Private Sub ServerCommunication_CommandRecievedFromServer(ByVal sender As Object, ByVal e As CommandRecievedFromServerEventArgs) Handles Me.CommandRecievedFromServer 
        // '' '' 'System.IO.File.AppendAllText(Forms.Application.StartupPath & "\Logs\ClientNetwork.log", String.Format( _ 
        // '' '' '"{2,12}:Packet Recieved: {0,-10}: {1}", _ 
        // '' '' 'e.BR.BytesLeft.ToString & " bytes", e.Command.ToString, Me.HoofdObj.Time) _ 
        // '' '' '& vbCrLf) 



        // '' '' Select Case e.Command 
        // '' '' Case ClientCommands.Ping 
        // '' '' 'Me.SendPacketToServer(ServerCommands.PingReply, New Byte() {}) 
        // '' '' Case ClientCommands.PingReply 
        // '' '' Stop 
        // '' '' 'RaiseEvent PingCompleted(Me, New PingCompletedEventArgs(Nothing, False, Nothing, Me.HoofdObj.Time - BitConverter.ToInt32(e.Data, 0))) 
        // '' '' Case ClientCommands.LoginReply 
        // '' '' 'Dim BR As New ByteReader(e.Data) 
        // '' '' Dim Result As LoginResult = CType(e.BR.ReadByte, LoginResult) 
        // '' '' Dim PLEntID As Integer = -1 
        // '' '' Dim LoginAttemptID As Integer = -1 
        // '' '' If Result = LoginResult.Succes Then 
        // '' '' PLEntID = e.BR.ReadInt32 
        // '' '' LoginAttemptID = e.BR.ReadInt32 
        // '' '' End If 

        // '' '' Dim ev As LoginCompletedEventArgs = New LoginCompletedEventArgs(Nothing, False, Nothing, Result, PLEntID, LoginAttemptID) 
        // '' '' 'BR.Close() 

        // '' '' RaiseEvent LoginCompleted(Me, ev) 
        // '' '' Case ClientCommands.LinkUDPConnectionReply 
        // '' '' Me.OnLinkUDPConnectionCompleted(Me, Nothing) 

        // '' '' Case ClientCommands.SignaleerVerandering 
        // '' '' Me.OnSignaleerVerandering(Me, New SignaleerVeranderingEventArgs(e.BR.ReadInt32, e.BR.ReadInt32)) 

        // '' '' Case ClientCommands.LaadWereldReplyEntity 

        // '' '' RaiseEvent LaadWereldProgressChanged(Me, New LaadWereldProgressChangedEventArgs(Nothing, False, Nothing, e.BR.ReadInt32, e.BR.ReadInt32)) 
        // '' '' Case ClientCommands.LaadWereldReplyCompleted 
        // '' '' RaiseEvent LaadWereldCompleted(Me, New LaadWereldCompletedEventArgs(Nothing, False, Nothing)) 
        // '' '' Case ClientCommands.GetEntityReply 
        // '' '' 'Dim BR As New ByteReader(e.Data) 
        // '' '' Dim EArgs As New GetEntityCompletedEventArgs(Nothing, False, Nothing, e.BR.ReadInt32, CType(e.BR.ReadInt32, EntityType), e.BR.ReadInt32, e.BR.ReadBytes(e.BR.ReadInt32)) 
        // '' '' 'BR.Close() 
        // '' '' RaiseEvent GetEntityCompleted(Me, EArgs) 

        // '' '' Case ClientCommands.Verandering 
        // '' '' 'Dim BR As New ByteReader(e.Data) 
        // '' '' Dim EArgs As New VeranderingEventArgs(e.BR.ReadInt32, e.BR.ReadInt32, e.BR.ReadBytes(CInt(e.BR.BytesLeft))) 

        // '' '' 'BR.Close() 

        // '' '' Me.OnVerandering(Me, EArgs) 

        // '' '' Case ClientCommands.DeltaSnapshot 
        // '' '' ' Dim BR As New ByteReader(e.Data) 
        // '' '' Dim EArgs As DeltaSnapshotEventArgs = New DeltaSnapshotEventArgs(e.BR.ReadInt32, Common.DeltaSnapshot.CreateFromByteReader(e.BR)) 

        // '' '' 'BR.Close() 

        // '' '' Me.OnDeltaSnapshot(Me, EArgs) 

        // '' '' Case ClientCommands.Time 
        // '' '' Dim nTime As Integer = e.BR.ReadInt32 '.ToInt32(e.Data, 0) 
        // '' '' Dim nTick As Integer = e.BR.ReadInt32 'BitConverter.ToInt32(e.Data, 4) 

        // '' '' Me.OnTime(Me, New TimeEventArgs(nTime, nTick)) 


        // '' '' Case ClientCommands.GetModelReply 
        // '' '' Dim MD As ModelData = ModelData.FromNetworkBytes(e.BR) 
        // '' '' CLMain.ModelManager.UpdateModel(MD) 


        // '' '' End Select 
        // '' ''End Sub 

        // '' ''Protected Overrides Sub DisposeObject() 
        // '' '' MyBase.DisposeObject() 

        // '' ''End Sub 

        private void mTCPConnection_ConnectedToServer(object sender, Common.Networking.TCPConnection.ConnectedToServerEventArgs e)
        {
            connected = true;
            this.TCPConnection.Receiving = true;

        }

        private void mTCPConnection_ConnectError(object sender, Common.Networking.TCPConnection.ConnectErrorEventArgs e)
        {

        }

        private void mTCPConnection_NetworkErrorAsync(object sender, Common.Networking.BaseConnection.NetworkErrorEventArgs e)
        {

        }

        private void mTCPConnection_PacketRecievedAsync(object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e)
        {
            this.HoofdObj.Invoker.Invoke(this.OnPacketRecieved, sender, e);

        }



        protected void OnPacketRecieved(object sender, BaseConnection.PacketRecievedEventArgs e)
        {
            this.Proxy.OnPacketRecieved(sender, e);


        }

        protected override void OnUDPPacketRecieved(object sender, Common.Networking.BaseConnection.PacketRecievedEventArgs e)
        {
            base.OnUDPPacketRecieved(sender, e);

            //TODO: verify if the data comes from the server

            this.OnPacketRecieved(sender, e);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (this.TCPConnection != null)
            {
                this.TCPConnection.Dispose();


            }
            tcpConnection = null;
        }



        public ProxyServer Proxy { get { return this.proxy; } }
        public TCPConnection TCPConnection { get { return tcpConnection; } }
        public bool Connected { get { return connected; } }
    }
}

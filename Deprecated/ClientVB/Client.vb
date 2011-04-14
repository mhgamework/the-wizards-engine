Public Class Client
    Inherits SpelObject
    'Inherits BaseClient


    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

    End Sub
    ' '' ''Public Sub New(ByVal nParent As SpelObject, ByVal nServerAddress As Net.IPAddress, ByVal nServerUDPPort As Integer, ByVal nServerTCPPort As Integer)
    ' '' ''    MyBase.New(nParent)
    ' '' ''    Me.setTCPConn(New TCPConnection(New Net.Sockets.TcpClient))
    ' '' ''    Me.setUDPEndPoint(New Net.IPEndPoint(nServerAddress, nServerUDPPort))
    ' '' ''    Me.TCPConn.Connect(nServerAddress, nServerTCPPort)
    ' '' ''End Sub

    ' '' ''Protected Overrides Sub setTCPConn(ByVal value As Common.Networking.TCPConnection)
    ' '' ''    MyBase.setTCPConn(value)
    ' '' ''    Me.mTCPConn = value
    ' '' ''End Sub
    ' '' ''Private WithEvents mTCPConn As TCPConnection
    ' '' ''Private Scheduler As New SchedulerElement(Me)

    ' '' ''Private WithEvents TickElement As New TickElement(Me)

    ' '' ''Public Overrides Function Login(ByVal nUsername As String, ByVal nPassword() As Byte) As Common.LoginResult

    ' '' ''End Function

    ' '' ''Public Overrides Sub OnVerandering(ByVal sender As Object, ByVal e As Common.VeranderingEventArgs)

    ' '' ''End Sub

    '' '' ''Public Overrides Sub ProcessClientCommand(ByVal nPlCmd As Common.PlayerCommands, ByVal nAngles As Microsoft.DirectX.Vector3)
    '' '' ''    'MyBase.ProcessClientCommand(nPlCmd, nAngles)
    '' '' ''    CLMain.ServerComm.SendPlayerCommandAsync(nPlCmd, nAngles)
    '' '' ''End Sub

    ' '' ''Public Sub SetPlayerData(ByVal nLoggedIn As Boolean, ByVal nLinkedPlayerEntityID As Integer)
    ' '' ''    Me.setLoggedIn(nLoggedIn)
    ' '' ''    Me.setLinkedPlayerEntityID(nLinkedPlayerEntityID)
    ' '' ''End Sub


    ' '' ''Private mLinkedPlayerEntityID As Integer
    ' '' ''Public ReadOnly Property LinkedPlayerEntityID() As Integer
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mLinkedPlayerEntityID
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Public Sub setLinkedPlayerEntityID(ByVal value As Integer)
    ' '' ''    Me.mLinkedPlayerEntityID = value
    ' '' ''    Me.TryLinkPlayerEntity()
    ' '' ''End Sub

    ' '' ''Public Sub TryLinkPlayerEntity()
    ' '' ''    If Me.LinkedPlayerEntityID <> -1 Then
    ' '' ''        Dim Ent As Entity = CLMain.Wereld.Tree.RootNode.FindEntity(Me.LinkedPlayerEntityID)
    ' '' ''        If Ent IsNot Nothing Then Me.setLinkedPlayerEntity(CType(Ent, Player))
    ' '' ''    Else
    ' '' ''        Me.setLinkedPlayerEntity(Nothing)
    ' '' ''    End If
    ' '' ''End Sub


    '' '' ''Private Sub TickElement_Tick(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.TickBaseElement.TickEventArgs) Handles TickElement.Tick
    '' '' ''    If Me.LinkedPlayerEntity Is Nothing Then Exit Sub
    '' '' ''    If Me.LinkedPlayerEntity.Functions.Actor Is Nothing Then Exit Sub
    '' '' ''    Me.LinkedPlayerEntity.Functions.Actor.setGlobalPosition(New Vector3(0, 300, 0))
    '' '' ''End Sub

    ' '' ''Public Sub OnPacketRecieved(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.PacketRecievedEventArgs)
    ' '' ''    CLMain.ServerComm.OnPacketRecieved(New Communication.PacketRecievedEventArgs(e.Dgram, Me.TCPConn.EndPoint))
    ' '' ''End Sub

    ' '' ''Private Sub mTCPConn_PacketRecieved(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.PacketRecievedEventArgs) Handles mTCPConn.PacketRecieved
    ' '' ''    Me.Scheduler.ScheduleAction(AddressOf Me.OnPacketRecieved, sender, e, 0)
    ' '' ''End Sub

    ' '' ''Public Sub OnConnectedToServer(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.ConnectedToServerEventArgs)
    ' '' ''    Me.TCPConn.Receiving = True
    ' '' ''End Sub
    ' '' ''Private Sub mTCPConnection_ConnectedToServer(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.ConnectedToServerEventArgs) Handles mTCPConn.ConnectedToServer
    ' '' ''    Me.Scheduler.ScheduleAction(AddressOf Me.OnConnectedToServer, sender, e, 0)
    ' '' ''End Sub


    ' '' ''Public Sub OnConnectError(ByVal sender As Object, ByVal e As TCPConnection.ConnectErrorEventArgs)
    ' '' ''    RaiseEvent ConnectError(sender, e)
    ' '' ''End Sub
    ' '' ''Public Event ConnectError(ByVal sender As Object, ByVal e As TCPConnection.ConnectErrorEventArgs)

    ' '' ''Private Sub mTCPConnection_ConnectError(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.ConnectErrorEventArgs) Handles mTCPConn.ConnectError
    ' '' ''    Me.Scheduler.ScheduleAction(AddressOf OnConnectError, sender, e, 0)
    ' '' ''End Sub

    ' '' ''Private Sub mTCPConnection_NetworkError(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.NetworkErrorEventArgs) Handles mTCPConn.NetworkError

    ' '' ''End Sub



    ' '' ''Private Sub ServerCommunication_ConnectError(ByVal sender As Object, ByVal e As Common.Networking.TCPConnection.ConnectErrorEventArgs) Handles Me.ConnectError
    ' '' ''    If TypeOf e.Ex Is Net.Sockets.SocketException Then
    ' '' ''        Select Case CType(e.Ex, Net.Sockets.SocketException).SocketErrorCode
    ' '' ''            Case Net.Sockets.SocketError.ConnectionRefused
    ' '' ''                Me.Scheduler.ScheduleAction(AddressOf Me.TCPConn.ReConnect, 1000)

    ' '' ''            Case Net.Sockets.SocketError.TimedOut
    ' '' ''                Me.Scheduler.ScheduleAction(AddressOf Me.TCPConn.ReConnect, 1000)


    ' '' ''        End Select
    ' '' ''    End If
    ' '' ''End Sub

    ' '' ''Protected Overrides Sub DisposeObject()
    ' '' ''    MyBase.DisposeObject()
    ' '' ''    If Me.TCPConn IsNot Nothing Then
    ' '' ''        Me.TCPConn.Dispose()
    ' '' ''        Me.setTCPConn(Nothing)
    ' '' ''    End If

    ' '' ''End Sub
End Class

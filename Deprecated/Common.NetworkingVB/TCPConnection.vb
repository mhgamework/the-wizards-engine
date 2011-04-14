Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

<Obsolete()>
Public Class TCPConnection
    Inherits BaseConnection

    Public Sub New()
        Me.New(New TcpClient)
    End Sub

    Public Sub New(ByVal nTCPClient As TcpClient)
        MyBase.New()


        Me.setTCP(nTCPClient)
        'Me.TCP.LingerState = New LingerOption(True, 0)



        'Me.TCP.LingerState = New LingerOption(True, 0)
        'Me.TCP.ReceiveBufferSize = 12 * 1024 'Mag dit?
        Me.TCP.ReceiveBufferSize = 16 * 1024 'Mag dit?

        Me.setPB(New TCPPacketBuilder)

        Console.WriteLine("WARNING: Using old TCPConnection!!!!")

        'Throw New Exception("This is an old version of this class, and is disabled to prevent wierd things happening")

    End Sub

    Private mTCP As TcpClient
    Public ReadOnly Property TCP() As TcpClient
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTCP
        End Get
    End Property
    Private Sub setTCP(ByVal value As TcpClient)
        Me.mTCP = value
    End Sub



    ''' <summary>
    ''' MAY ONLY BE USED BY ReceiveMessageJob !!!!!
    ''' </summary>
    ''' <remarks></remarks>
    Private mBuffer As Byte() = New Byte(1024 * 4 - 1) {}

    Protected Overrides Sub ReceiveMessageJob()
        Dim ret As Integer

        ret = Me.TCP.Client.Receive(mBuffer, 0, 1024 * 4 - 1, SocketFlags.None)

        If ret = 0 Then
            Receiving = False

            Throw New SocketException(SocketError.ConnectionAborted)
        Else
            Dim BytesRec As Byte() = New Byte(ret - 1) {}
            Array.Copy(mBuffer, BytesRec, ret)
            Me.OnBytesRecievedAsync(New BytesRecievedEventArgs(BytesRec))
        End If


    End Sub



    Public Class BytesRecievedEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal nBytes As Byte())
            Me.setBytes(nBytes)
        End Sub

        Private mBytes As Byte()
        Public ReadOnly Property Bytes() As Byte()
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mBytes
            End Get
        End Property
        Private Sub setBytes(ByVal value As Byte())
            Me.mBytes = value
        End Sub

    End Class
    Public Sub OnBytesRecievedAsync(ByVal e As BytesRecievedEventArgs)
        RaiseEvent BytesRecievedAsync(Me, e)
    End Sub
    Public Event BytesRecievedAsync(ByVal sender As Object, ByVal e As BytesRecievedEventArgs)



    Protected Overrides Sub SendMessageJob(ByVal nPacket As BaseConnection.QueuedSendPacket)
        Me.TCP.GetStream.Write(nPacket.Dgram, 0, nPacket.Dgram.Length)

    End Sub


    Public Overloads Sub SendPacket(ByVal dgram As Byte(), ByVal Flags As TCPPacketBuilder.TCPPacketFlags)
        Dim bytes As Byte() = Me.PB.BuildPacket(dgram, Flags)

        Me.SendPacket(bytes, Me.TempEndPoint)

    End Sub
    'Public Sub SendPacketZipped(ByVal dgram As Byte())
    '    Dim InDS As New DataStream(dgram)
    '    Dim GZ As New IO.Compression.GZipStream(InDS, IO.Compression.CompressionMode.Compress, True)




    'End Sub



    Private mPB As TCPPacketBuilder
    Public ReadOnly Property PB() As TCPPacketBuilder
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPB
        End Get
    End Property
    Private Sub setPB(ByVal value As TCPPacketBuilder)
        Me.mPB = value
    End Sub



    Private Sub UpdaterCommunication_BytesRecievedAsync(ByVal sender As Object, ByVal e As BytesRecievedEventArgs) Handles Me.BytesRecievedAsync
        Dim Buffer As New DataStream(e.Bytes)

        Do While Buffer.BytesLeft > 0
            PB.AppendBytes(Buffer)
            If PB.State = TCPPacketBuilder.PacketState.Complete Then
                Me.OnPacketRecievedAsync(Nothing, New PacketRecievedEventArgs(PB.GetPacketDgram, Me.TempEndPoint))
            Else
                Exit Do
            End If
        Loop




    End Sub





    Public Sub Connect(ByVal nIP As String, ByVal nPort As Integer)
        Me.Connect(IPAddress.Parse(nIP), nPort)

    End Sub
    Public Sub Connect(ByVal nIP As IPAddress, ByVal nPort As Integer)
        If Me.TCP.Connected Then Throw New Exception("Al connected")
        If Me.ConnectThread IsNot Nothing Then Throw New Exception("Kan niet")
        Me.setTempEndPoint(New IPEndPoint(nIP, nPort))
        Me.setConnecting(True)

        Me.setConnectThread(New Thread(AddressOf Me.ConnectToServer))

        Me.ConnectThread.Start()



    End Sub

    Public Sub ReConnect()
        If Me.TCP.Connected Then Throw New Exception("Al connected")
        If Me.ConnectThread IsNot Nothing Then Throw New Exception("Nog aan het connecten.")
        Me.setConnecting(True)

        Me.setConnectThread(New Thread(AddressOf Me.ConnectToServer))

        Me.ConnectThread.Start()
    End Sub


    Private mConnectThread As Thread
    Public ReadOnly Property ConnectThread() As Thread
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mConnectThread
        End Get
    End Property
    Private Sub setConnectThread(ByVal value As Thread)
        Me.mConnectThread = value
    End Sub


    Private mTempEndPointLock As New Object
    Private mTempEndPoint As IPEndPoint
    ''' <summary>
    ''' Thread Safe
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TempEndPoint() As IPEndPoint
        <System.Diagnostics.DebuggerStepThrough()> Get
            SyncLock mTempEndPointLock
                Return Me.mTempEndPoint
            End SyncLock
        End Get
    End Property
    Private Sub setTempEndPoint(ByVal value As IPEndPoint)
        SyncLock mTempEndPointLock
            Me.mTempEndPoint = value
        End SyncLock
    End Sub


    'Private mEndPoint As IPEndPoint
    'Public ReadOnly Property EndPoint() As IPEndPoint
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mEndPoint
    '    End Get
    'End Property
    'Private Sub setEndPoint(ByVal value As IPEndPoint)
    '    Me.mEndPoint = value
    'End Sub




    Private mConnecting As Boolean
    Public ReadOnly Property Connecting() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            SyncLock Me
                Return Me.mConnecting
            End SyncLock
        End Get
    End Property
    Private Sub setConnecting(ByVal value As Boolean)
        Me.mConnecting = value
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Not Thread safe! draait niet in main thread</remarks>
    Private Sub ConnectToServer()
        'Do While Me.Connecting
        If Me.Connecting = False Then Exit Sub
        Try
            Dim EP As IPEndPoint = Me.TempEndPoint
            Me.TCP.Connect(EP)

            If Me.Connecting = False Then Stop 'klopt niet

            Me.OnConnectedToServer(New ConnectedToServerEventArgs(EP))



        Catch se As SocketException
            Select Case se.SocketErrorCode
                'Case SocketError.Interrupted
                '    If Me.Receiving = False Then
                '        Exit Sub
                '    End If

            End Select
            Me.OnConnectError(New ConnectErrorEventArgs(se))

        Catch ex As Exception
            Me.OnConnectError(New ConnectErrorEventArgs(ex))
        Finally


        End Try
        'Loop
    End Sub


    Public Class ConnectedToServerEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal nServerEP As IPEndPoint)
            Me.setServerEndPoint(nServerEP)
        End Sub
        Private mServerEndPoint As IPEndPoint
        Public ReadOnly Property ServerEndPoint() As IPEndPoint
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mServerEndPoint
            End Get
        End Property
        Private Sub setServerEndPoint(ByVal value As IPEndPoint)
            Me.mServerEndPoint = value
        End Sub

    End Class
    Public Sub OnConnectedToServer(ByVal e As ConnectedToServerEventArgs)
        RaiseEvent ConnectedToServer(Me, e)
    End Sub
    Public Event ConnectedToServer(ByVal sender As Object, ByVal e As ConnectedToServerEventArgs)


    Public Class ConnectErrorEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal nEx As Exception)
            Me.setEx(nEx)
        End Sub

        Private mEx As Exception
        Public ReadOnly Property Ex() As Exception
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mEx
            End Get
        End Property
        Private Sub setEx(ByVal value As Exception)
            Me.mEx = value
        End Sub

    End Class
    Public Sub OnConnectError(ByVal e As ConnectErrorEventArgs)
        Me.ConnectThread.Join(5000)
        Me.setConnectThread(Nothing)
        Me.setConnecting(False)

        RaiseEvent ConnectError(Me, e)
    End Sub
    Public Event ConnectError(ByVal sender As Object, ByVal e As ConnectErrorEventArgs)










    Protected Overrides Sub CloseSocket()
        MyBase.CloseSocket()
        If TCP.Connected Then Me.TCP.Client.Shutdown(SocketShutdown.Both)
        Me.TCP.Client.Close()

        Me.TCP.Close() 'TODO
    End Sub



End Class

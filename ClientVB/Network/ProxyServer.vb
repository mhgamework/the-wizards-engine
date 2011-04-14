Public Class ProxyServer
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.mCommunication = New ClientCommunication(Me, System.Net.IPAddress.Parse("127.0.0.1"), 5012, 5014)
        Me.CreateProxyWereld()
    End Sub

    Protected Overridable Sub CreateProxyWereld()
        Me.setWereld(New ProxyServerWereld(Me))
    End Sub

    Protected mWereld As ProxyServerWereld
    Public ReadOnly Property Wereld() As ProxyServerWereld
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mWereld
        End Get
    End Property
    Private Sub setWereld(ByVal value As ProxyServerWereld)
        Me.mWereld = value
    End Sub


    Protected mCommunication As ClientCommunication
    Public ReadOnly Property Communication() As ClientCommunication
        Get
            Return Me.mCommunication
        End Get
    End Property


    Public Sub OnPacketRecieved(ByVal sender As Object, ByVal e As Common.Networking.BaseConnection.PacketRecievedEventArgs)
        Dim cmd As Byte = e.Dgram(0)
        Dim data As Byte() = New Byte(e.Dgram.Length - 1 - 1) {}
        Array.Copy(e.Dgram, 1, data, 0, data.Length)
        If Not System.Enum.IsDefined(GetType(Communication.ServerCommands), cmd) Then
            Stop
        End If
        Dim Command As Communication.ClientCommands = CType(cmd, Communication.ClientCommands)

        Dim BR As New ByteReader(data)
        Me.ProcessCommand(Command, BR)
        'RaiseEvent CommandRecievedFromClient(Me, New CommandRecievedFromClientEventArgs(Command, BR, nCL, e.EP))
        BR.Close()


    End Sub


    Private Sub ProcessCommand(ByVal cmd As Communication.ClientCommands, ByVal BR As ByteReader)
        Select Case cmd
            Case Common.Communication.ClientCommands.OnSuccessfulLogin
                Dim LoginKey As String = System.Text.ASCIIEncoding.ASCII.GetString(BR.ReadBytes(CInt(BR.BytesLeft)))
                CLMain.Wereld.OnSuccessfulLogin(LoginKey)
        End Select
    End Sub




    Public Sub Ping()
        Me.Communication.SendPacketUDP(Common.Communication.ServerCommands.Ping)
    End Sub

    Public Sub LinkUDPConnection(ByVal LoginKey As String)
        Me.Communication.SendPacketUDP(Common.Communication.ServerCommands.LinkUDPConnection, System.Text.ASCIIEncoding.ASCII.GetBytes(LoginKey))
    End Sub



End Class

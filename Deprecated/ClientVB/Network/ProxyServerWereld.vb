Public Class ProxyServerWereld

    Public Sub New(ByVal nServer As ProxyServer)
        Me.setServer(nServer)
    End Sub


    Private mServer As ProxyServer
    Public ReadOnly Property Server() As ProxyServer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mServer
        End Get
    End Property
    Private Sub setServer(ByVal value As ProxyServer)
        Me.mServer = value
    End Sub


    Public Sub LoginAsync(ByVal nUsername As String, ByVal nPassword As String)
        Dim PasswordBytes As Byte()
        'Dim B As Byte() = New Byte(16 + nUsername.Length - 1) {}
        Dim Crypt As System.Security.Cryptography.MD5
        Crypt = System.Security.Cryptography.MD5CryptoServiceProvider.Create()
        PasswordBytes = Crypt.ComputeHash(System.Text.Encoding.ASCII.GetBytes(nPassword))

        'HashedBytes.CopyTo(B, 0)
        'System.Text.Encoding.ASCII.GetBytes(nUsername).CopyTo(B, HashedBytes.Length)


        Dim P As New LoginPacket(nUsername, PasswordBytes)

        Me.Server.Communication.SendPacketTCP(Common.Communication.ServerCommands.Login, P)

    End Sub

End Class

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Namespace Networking

    Public Class TCPClientConnection
        Inherits TCPConnection
        Public Sub New()
            MyBase.New(New TcpClient)
        End Sub




    End Class
End Namespace
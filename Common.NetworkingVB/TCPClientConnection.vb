Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Class TCPClientConnection
    Inherits TCPConnection
    Public Sub New()
        MyBase.New(New TcpClient)
    End Sub

  


End Class

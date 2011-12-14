Imports System.Net
Imports System.Net.Sockets
Imports System.Threading



Namespace Networking
    <Obsolete("This is an old version of this class!!")>
    Public Class UDPConnection
        Inherits BaseConnection

        Public Sub New()
            MyBase.New()
            Me.setUDP(New UdpClient)
            Throw New Exception("This class  is deprecated, use the new implementation instead")

        End Sub

        Private mUDP As UdpClient
        Public ReadOnly Property UDP() As UdpClient
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mUDP
            End Get
        End Property
        Private Sub setUDP(ByVal value As UdpClient)
            Me.mUDP = value
        End Sub

        Protected Overrides Sub ReceiveMessageJob()
            Dim EP As New IPEndPoint(IPAddress.Any, 0)
            Dim dgram As Byte()
            dgram = Me.UDP.Receive(EP)

            Me.OnPacketRecievedAsync(Nothing, New PacketRecievedEventArgs(dgram, EP)) 'TODO: EDIT: what todo?
        End Sub

        Protected Overrides Sub SendMessageJob(ByVal nPacket As BaseConnection.QueuedSendPacket)
            Dim pRet As Integer = -1

            pRet = Me.UDP.Send(nPacket.Dgram, nPacket.Dgram.Length, nPacket.EP)

            If pRet <> nPacket.Dgram.Length Then Throw New Exception("Was unabled to send the whole package. " & pRet & " of the " & nPacket.Dgram.Length)
        End Sub


        Public Sub Bind(ByVal nEP As IPEndPoint)
            Me.UDP.Client.Bind(nEP)

        End Sub

        Protected Overrides Sub CloseSocket()
            MyBase.CloseSocket()

            Me.UDP.Close() 'Is this ok?
        End Sub


    End Class
End Namespace
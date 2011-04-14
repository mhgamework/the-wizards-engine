Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Public Class Communication
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.setUDPConn(New UDPConnection)
    End Sub

    Public Const UDPPort As Int16 = 5012
    Public Const ListenerPort As Integer = 5014

    Private WithEvents mUDPConn As UDPConnection
    Public ReadOnly Property UDPConn() As UDPConnection
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mUDPConn
        End Get
    End Property
    Private Sub setUDPConn(ByVal value As UDPConnection)
        Me.mUDPConn = value
    End Sub

    Public Enum Protocol
        TCP = 1
        UDP = 2
    End Enum


    Public Enum ClientCommands As Byte
        Ping = 1
        OnSuccessfulLogin
        EntityUpdate
        UpdateWorld
        UpdateTime

        'SignaleerVerandering

        'Verandering

        'DeltaSnapshot

        'Time








        'Reply's
        PingReply = 100

        'LoginReply
        'LinkUDPConnectionReply

        'LaadWereldReplyEntity
        'LaadWereldReplyCompleted

        'GetEntityReply


        'GetModelReply





        'ADMINGetGameFilesListReply
        'ADMINAddGameFileReply

        'ADMINGetModelListReply
        'ADMINUpdateModelReply











    End Enum

    Public Enum ServerCommands As Byte
        Ping = 1
        Login
        LinkUDPConnection
        ApplyPlayerInput

        TempShootShuriken

        'Login
        'LinkUDPConnection

        'LaadWereld

        'GetEntity

        'PlayerCommand

        'UpdateAngles



        'GetModel






        ''ADMIN
        'ADMINGetGameFilesList
        'ADMINAddGameFile

        'ADMINGetModelList
        'ADMINUpdateModel

        'ADMINPutStaticEntity

















        'Reply's
        PingReply = 100

    End Enum


    ' '' ''Private mScheduler As New SchedulerElement(Me)
    ' '' ''Public ReadOnly Property Scheduler() As SchedulerElement
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mScheduler
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setScheduler(ByVal value As SchedulerElement)
    ' '' ''    Me.mScheduler = value
    ' '' ''End Sub




    'Protected Overridable Sub SendUDPPacket(ByVal dgram As Byte(), ByVal nEndPoint As IPEndPoint)
    '    Me.UDPConn.SendPacket(dgram, nEndPoint)

    'End Sub

    Public Overrides Sub Dispose()
        MyBase.Dispose()
        Me.UDPConn.Dispose()

    End Sub



    Protected Overridable Sub OnUDPPacketRecieved(ByVal sender As Object, ByVal e As BaseConnection.PacketRecievedEventArgs)

    End Sub


    ' '' ''Private Sub mUDPConn_PacketRecieved(ByVal sender As Object, ByVal e As UDPConnection.PacketRecievedEventArgs) Handles mUDPConn.PacketRecieved
    ' '' ''    Me.Scheduler.ScheduleAction(AddressOf Me.OnUDPPacketRecieved, New PacketRecievedEventArgs(e.Dgram, e.EP), 0)
    ' '' ''End Sub

    Private Sub mUDPConn_NetworkErrorAsync(ByVal sender As Object, ByVal e As Networking.BaseConnection.NetworkErrorEventArgs) Handles mUDPConn.NetworkErrorAsync

    End Sub

    Private Sub mUDPConn_PacketRecievedAsync(ByVal sender As Object, ByVal e As Networking.BaseConnection.PacketRecievedEventArgs) Handles mUDPConn.PacketRecievedAsync
        HoofdObj.Invoker.Invoke(AddressOf OnUDPPacketRecieved, sender, e)
    End Sub
End Class

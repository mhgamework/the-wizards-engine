Public Class AdminServerCommunication
    Inherits ServerCommunication

    Public Sub New(ByVal nParent As SpelObject, ByVal nServerAddress As Net.IPAddress, ByVal nServerUDPPort As Integer, ByVal nServerTCPPort As Integer)
        MyBase.New(nParent, nServerAddress, nServerUDPPort, nServerTCPPort)

    End Sub

    Public Sub AddGameFileAsync(ByVal nACFP As AddGameFilePacket)
        Me.SendPacketToServer(ServerCommands.ADMINAddGameFile, nACFP, Protocol.TCP)
    End Sub

    Public Sub GetModelListAsync()
        Me.SendPacketToServer(ServerCommands.ADMINGetModelList, New Byte() {}, Protocol.TCP)

    End Sub

    Public Sub UpdateModelAsync(ByVal nAMD As AdminModelData)
        Me.SendPacketToServer(ServerCommands.ADMINUpdateModel, nAMD, Protocol.TCP)
    End Sub

    Public Sub PutStaticEntityAsync(ByVal nP As PutStaticEntityPacket)
        Me.SendPacketToServer(ServerCommands.ADMINPutStaticEntity, nP, Protocol.TCP)
    End Sub

    Public Class GameFilesListReceivedEventArgs
        Inherits System.EventArgs

        Public Sub New(ByVal nCFL As GameFilesList)
            Me.setCFL(nCFL)
        End Sub


        Private mCFL As GameFilesList
        Public ReadOnly Property CFL() As GameFilesList
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCFL
            End Get
        End Property
        Private Sub setCFL(ByVal value As GameFilesList)
            Me.mCFL = value
        End Sub

    End Class
    Public Sub OnGameFilesListReceived(ByVal sender As Object, ByVal e As GameFilesListReceivedEventArgs)
        RaiseEvent GameFilesListReceived(Me, e)
    End Sub
    Public Event GameFilesListReceived(ByVal sender As Object, ByVal e As GameFilesListReceivedEventArgs)



    Public Class ModelListRecievedEventArgs
        Inherits System.EventArgs

        Public Sub New(ByVal nML As ModelsList)
            Me.setML(nML)
        End Sub


        Private mML As ModelsList
        Public ReadOnly Property ML() As ModelsList
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mML
            End Get
        End Property
        Private Sub setML(ByVal value As ModelsList)
            Me.mML = value
        End Sub

    End Class
    Public Sub OnModelListRecieved(ByVal sender As Object, ByVal e As ModelListRecievedEventArgs)
        RaiseEvent ModelListRecieved(Me, e)
    End Sub
    Public Event ModelListRecieved(ByVal sender As Object, ByVal e As ModelListRecievedEventArgs)


    Public Class UpdateModelCompletedEventArgs
        Inherits System.EventArgs
    End Class
    Public Sub OnUpdateModelCompleted(ByVal sender As Object, ByVal e As UpdateModelCompletedEventArgs)
        RaiseEvent UpdateModelCompleted(Me, e)
    End Sub
    Public Event UpdateModelCompleted(ByVal sender As Object, ByVal e As UpdateModelCompletedEventArgs)



    Public Class AddGameFileCompletedEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal nCLFID As Integer)
            Me.setGameFileID(nCLFID)
        End Sub

        Private mGameFileID As Integer
        Public ReadOnly Property GameFileID() As Integer
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mGameFileID
            End Get
        End Property
        Private Sub setGameFileID(ByVal value As Integer)
            Me.mGameFileID = value
        End Sub


    End Class
    Public Sub OnAddGameFileCompleted(ByVal sender As Object, ByVal e As AddGameFileCompletedEventArgs)
        RaiseEvent AddGameFileCompleted(Me, e)
    End Sub
    Public Event AddGameFileCompleted(ByVal sender As Object, ByVal e As AddGameFileCompletedEventArgs)


    Private Sub AdminServerCommunication_CommandRecievedFromServer(ByVal sender As Object, ByVal e As Client.ServerCommunication.CommandRecievedFromServerEventArgs) Handles Me.CommandRecievedFromServer
        Select Case e.Command
            Case ClientCommands.ADMINGetGameFilesListReply
                Dim CFL As GameFilesList = GameFilesList.FromBytes(e.BR)
                Me.OnGameFilesListReceived(Me, New GameFilesListReceivedEventArgs(CFL))

            Case ClientCommands.ADMINAddGameFileReply

                Me.OnAddGameFileCompleted(Me, New AddGameFileCompletedEventArgs(e.BR.ReadInt32))

            Case ClientCommands.ADMINGetModelListReply
                Dim ML As ModelsList = ModelsList.FromNetworkBytes(e.BR)

                Me.OnModelListRecieved(Me, New ModelListRecievedEventArgs(ML))

            Case ClientCommands.ADMINUpdateModelReply

                Me.OnUpdateModelCompleted(Me, New UpdateModelCompletedEventArgs)



        End Select
    End Sub
End Class

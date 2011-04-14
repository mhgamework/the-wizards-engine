Public Class FileManagerMenu
    Inherits Panel

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.mServerComm = DirectCast(Client.CLMain.ServerComm, AdminServerCommunication)


        Dim M As New DesignFileManager()
        M.FillPanel(Me)

        Me.Align = AlignType.MiddleCenter

        Me.setlstGameFiles(M.lstFiles.GetListBox2D)
        Me.setknpGetGameFiles(M.knpUpdateGameFiles.GetKnop001)


        Me.lblID = M.lblID.GetLabel
        Me.txtFilename = M.txtFileName.GetTextBox2D
        Me.txtDescription = M.txtDescription.GetTextBox2D
        Me.txtRelativePath = M.txtRelativePath.GetTextBox2D
        Me.txtLocalFile = M.txtLocalFile.GetTextBox2D
        Me.txtVersion = M.txtVersie.GetTextBox2D
        Me.txtType = M.txtType.GetTextBox2D
        Me.txtEnabled = M.txtEnabled.GetTextBox2D
        Me.txtHash = M.txtHash.GetTextBox2D


        Me.knpAddFile = M.knpAddFile.GetKnop001

        Me.setAddFileMenu(New AddGameFileMenu(Me.HoofdObj))
        Me.AddFileMenu.Enabled = False

        Me.lstGameFiles.ClearItems()

    End Sub


    Private mAddFileMenu As AddGameFileMenu
    Public ReadOnly Property AddFileMenu() As AddGameFileMenu
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAddFileMenu
        End Get
    End Property
    Private Sub setAddFileMenu(ByVal value As AddGameFileMenu)
        Me.mAddFileMenu = value
    End Sub


    Private WithEvents mServerComm As AdminServerCommunication

    Private WithEvents TickElement As New TickElement(Me)
    Private WithEvents ProcessElement As New ProcessEventElement(Me)


    Private WithEvents mlstGameFiles As ListBox2D
    Public ReadOnly Property lstGameFiles() As ListBox2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mlstGameFiles
        End Get
    End Property
    Private Sub setlstGameFiles(ByVal value As ListBox2D)
        Me.mlstGameFiles = value
    End Sub


    Private WithEvents mknpGetGameFiles As Knop
    Public ReadOnly Property knpGetGameFiles() As Knop
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mknpGetGameFiles
        End Get
    End Property
    Private Sub setknpGetGameFiles(ByVal value As Knop)
        Me.mknpGetGameFiles = value
    End Sub


    Private WithEvents knpAddFile As Knop

    Private WithEvents lblID As Label
    Private WithEvents txtFilename As TextBox2D
    Private WithEvents txtDescription As TextBox2D
    Private WithEvents txtRelativePath As TextBox2D
    Private WithEvents txtLocalFile As TextBox2D
    Private WithEvents txtVersion As TextBox2D
    Private WithEvents txtType As TextBox2D
    Private WithEvents txtEnabled As TextBox2D
    Private WithEvents txtHash As TextBox2D



    Private Sub TickElement_Tick(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.TickBaseElement.TickEventArgs) Handles TickElement.Tick

    End Sub

    Private Sub ProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessElement.Process
        With Me.HoofdObj.DevContainer.DX.Input
            If .KeyDown(DirectInput.Key.Escape) Then
                Me.Enabled = False
                ACLMain.HoofdMenu.Enabled = True
            End If
        End With
    End Sub

    Private Sub mServerComm_GameFilesListReceived(ByVal sender As Object, ByVal e As AdminServerCommunication.GameFilesListReceivedEventArgs) Handles mServerComm.GameFilesListReceived
        If Me.Enabled = False Then Exit Sub

        Me.lstGameFiles.ClearItems()
        For Each CL As GameFile In e.CFL.Files.Values
            Me.lstGameFiles.AddItem(CL)
        Next


    End Sub

    Private Sub mknpGetGameFiles_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mknpGetGameFiles.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        ACLMain.ServerComm.GetGameFilesListAsync()
    End Sub

    Private Sub mlstGameFiles_SelectedIndexChanged(ByVal sender As Object, ByVal e As Common.ListBox2D.SelectedIndexChangedEventArgs) Handles mlstGameFiles.SelectedIndexChanged
        If Me.lstGameFiles.SelectedIndex = -1 Then Exit Sub
        Dim CL As GameFile = CType(Me.lstGameFiles.Item(Me.lstGameFiles.SelectedIndex), GameFile)

        Me.lblID.Text = CL.ID.ToString
        Me.txtFilename.Text = CL.FileName
        Me.txtDescription.Text = CL.Description
        Me.txtRelativePath.Text = CL.RelativePath
        Me.txtLocalFile.Text = CL.LocalFile
        Me.txtVersion.Text = CL.Version.ToString
        Me.txtType.Text = CL.Type.ToString
        Me.txtEnabled.Text = CL.Enabled.ToString
        Me.txtHash.Text = CL.Hash.ToString



    End Sub

    Private Sub knpAddFile_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpAddFile.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.AddFileMenu.Enabled = True

    End Sub
End Class

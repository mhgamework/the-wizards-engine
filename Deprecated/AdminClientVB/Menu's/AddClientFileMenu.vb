Public Class AddGameFileMenu
    Inherits Panel

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.mServerComm = Client.CLMain.ServerComm

        Dim M As New DesignAddGameFileMenu()
        M.FillPanel(Me)

        Me.Align = AlignType.MiddleCenter

        Me.txtFilename = M.txtFileName.GetTextBox2D
        Me.txtDescription = M.txtDescription.GetTextBox2D
        Me.txtRelativePath = M.txtRelativePath.GetTextBox2D
        Me.txtLocalFile = M.txtLocalFile.GetTextBox2D
        Me.txtType = M.txtType.GetTextBox2D
        Me.txtEnabled = M.txtEnabled.GetTextBox2D

        Me.knpAddFile = M.knpAddFile.GetKnop001
        Me.knpCancel = M.knpCancel.GetKnop001
        Me.knpBladeren = M.knpBladeren.GetKnop001

    End Sub
    Private WithEvents mServerComm As ServerCommunication

    'Private WithEvents TickElement As New TickElement(Me)
    Private WithEvents ProcessElement As New ProcessEventElement(Me)

    Private WithEvents knpAddFile As Knop
    Private WithEvents knpCancel As Knop
    Private WithEvents knpBladeren As Knop

    Private WithEvents txtFilename As TextBox2D
    Private WithEvents txtDescription As TextBox2D
    Private WithEvents txtRelativePath As TextBox2D
    Private WithEvents txtLocalFile As TextBox2D
    Private WithEvents txtType As TextBox2D
    Private WithEvents txtEnabled As TextBox2D


    Private Sub knpCancel_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpCancel.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.Enabled = False
    End Sub

    Private Sub knpAddFile_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpAddFile.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        If System.IO.File.Exists(Me.txtLocalFile.Text) = False Then
            MsgBox("LocalFile bestaat niet!")
            Exit Sub
        End If


        Dim Bytes As Byte()
        Bytes = IO.File.ReadAllBytes(Me.txtLocalFile.Text)

        Dim P As New AddGameFilePacket(Me.txtFilename.Text, Me.txtRelativePath.Text, Bytes)

        ACLMain.ServerComm.AddGameFileAsync(P)


    End Sub

    Private Sub knpBladeren_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpBladeren.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Dim OFD As New Forms.OpenFileDialog
        Dim Result As Forms.DialogResult
        Result = OFD.ShowDialog()
        If Result = Forms.DialogResult.OK Then
            Me.txtLocalFile.Text = OFD.FileName
        End If

    End Sub
End Class

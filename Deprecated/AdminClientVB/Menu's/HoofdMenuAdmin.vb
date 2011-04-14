Public Class HoofdMenuAdmin
    Inherits HoofdMenu


    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.setFileManagerMenu(New FileManagerMenu(Me.HoofdObj))
        Me.FileManagerMenu.Enabled = False

        Me.setModelManagerMenu(New ModelManagerMenu(Me.HoofdObj))
        Me.ModelManagerMenu.Enabled = False

        Me.setWorldManagerMenu(New WorldManagerMenu(Me.HoofdObj))
        Me.WorldManagerMenu.Enabled = False


    End Sub
    Protected Overrides Sub BuildMenu()
        Dim M As New DesignHoofdMenuAdmin
        M.FillPanel(Me)

        Me.Align = AlignType.MiddleCenter


        Me.setKnopExit(M.knpQuit.GetKnop001)
        Me.setknpModelManager(M.knpModelManager.GetKnop001)
        Me.setknpFileManager(M.knpFileManager.GetKnop001)
        Me.knpWorldManager = M.knpWorldManager.GetKnop001

    End Sub


    Private mFileManagerMenu As FileManagerMenu
    Public ReadOnly Property FileManagerMenu() As FileManagerMenu
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFileManagerMenu
        End Get
    End Property
    Private Sub setFileManagerMenu(ByVal value As FileManagerMenu)
        Me.mFileManagerMenu = value
    End Sub


    Private mModelManagerMenu As ModelManagerMenu
    Public ReadOnly Property ModelManagerMenu() As ModelManagerMenu
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelManagerMenu
        End Get
    End Property
    Private Sub setModelManagerMenu(ByVal value As ModelManagerMenu)
        Me.mModelManagerMenu = value
    End Sub


    Private mWorldManagerMenu As WorldManagerMenu
    Public ReadOnly Property WorldManagerMenu() As WorldManagerMenu
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mWorldManagerMenu
        End Get
    End Property
    Private Sub setWorldManagerMenu(ByVal value As WorldManagerMenu)
        Me.mWorldManagerMenu = value
    End Sub





    Private WithEvents mknpModelManager As Knop
    Public ReadOnly Property knpModelManager() As Knop
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mknpModelManager
        End Get
    End Property
    Private Sub setknpModelManager(ByVal value As Knop)
        Me.mknpModelManager = value
    End Sub


    Private WithEvents mknpFileManager As Knop
    Public ReadOnly Property knpFileManager() As Knop
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mknpFileManager
        End Get
    End Property
    Private Sub setknpFileManager(ByVal value As Knop)
        Me.mknpFileManager = value
    End Sub

    Private WithEvents knpWorldManager As Knop



    Private Sub mknpModelManager_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mknpModelManager.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.ModelManagerMenu.Enabled = True
        Me.Enabled = False
        e.Handled = True
    End Sub

    Private Sub mknpFileManager_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mknpFileManager.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.FileManagerMenu.Enabled = True
        Me.Enabled = False
        e.Handled = True
    End Sub

    Private Sub knpWorldManager_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpWorldManager.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.WorldManagerMenu.Enabled = True
        Me.WorldManagerMenu.Activate()
        Me.Enabled = False
        e.Handled = True
    End Sub
End Class

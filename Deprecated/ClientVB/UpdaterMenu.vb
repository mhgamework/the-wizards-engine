'Imports WizUpdater
Public Class UpdaterMenu
    Inherits Panel

    Private WithEvents mUpdater As ClientUpdaterCommunication

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        Me.setRenderer(New Renderer2DContainer(Me))
        Me.Renderer.Visible = False
        'Me.setRenderer(New Renderer2D(Me))
        Me.Renderer.Size = New Vector2(500, 500)

        Me.setknpGetAll(New Knop001(Me))
        Me.knpGetAll.Positie = New Vector2(10, 10)
        Me.knpGetAll.Size = New Vector2(150, 50)
        Me.knpGetAll.Text = "Update"


        Me.setOutput(New TextList2D(Me))
        Me.Output.BackgroundColor = Color.Red
        Me.Output.Positie = New Vector2(20, 150)
        Me.Output.Size = New Vector2(300, 300)
        Me.Output.WriteLine("---The Wizards Updater---")



        Me.mUpdater = CType(Me.HoofdObj, ClientMain).Updater.Comm

        Me.LaadGameFiles()

    End Sub

    Private mScheduler As New SchedulerElement(Me)
    Public ReadOnly Property Scheduler() As SchedulerElement
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mScheduler
        End Get
    End Property
    Private Sub setScheduler(ByVal value As SchedulerElement)
        Me.mScheduler = value
    End Sub

    Private mRenderer As Renderer2DContainer
    Public ReadOnly Property Renderer() As Renderer2DContainer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRenderer
        End Get
    End Property
    Private Sub setRenderer(ByVal value As Renderer2DContainer)
        Me.mRenderer = value
    End Sub



    Public Sub LaadGameFiles()
        Dim Pad As String = Forms.Application.StartupPath & "\versions.twf"
        Dim FS As IO.FileStream = Nothing
        Dim BR As IO.BinaryReader = Nothing
        'Dim BW As IO.BinaryWriter

        Me.setGameFiles(New Dictionary(Of Integer, GameFile))

        Try
            If System.IO.File.Exists(Pad) = False Then
                Me.SaveGameFiles()

            End If



            FS = New IO.FileStream(Pad, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None)
            BR = New IO.BinaryReader(FS)

            Dim Header As String = BR.ReadString
            If Header <> "001The Wizards Version File" Then Throw New Exception("Invalid File Format")

            Dim NumFiles As Integer = BR.ReadInt32
            Dim F As GameFile
            For I As Integer = 0 To NumFiles - 1
                F = New GameFile(BR.ReadInt32, BR.ReadString, BR.ReadString, BR.ReadInt32, CType(BR.ReadInt32, GameFile.GameFileType))
                Me.GameFiles.Add(F.ID, F)
            Next
        Finally
            'If BW IsNot Nothing Then BW.Close()
            If BR IsNot Nothing Then BR.Close()
            If FS IsNot Nothing Then FS.Close()
        End Try
    End Sub

    Public Sub SaveGameFiles()
        Dim Pad As String = Forms.Application.StartupPath & "\versions.twf"
        Dim FS As IO.FileStream = Nothing
        Dim BW As IO.BinaryWriter = Nothing



        Try
            FS = New IO.FileStream(Pad, IO.FileMode.OpenOrCreate, IO.FileAccess.Write, IO.FileShare.None)
            BW = New IO.BinaryWriter(FS)
            BW.Write("001The Wizards Version File")

            BW.Write(Me.GameFiles.Count)

            For Each F As GameFile In Me.GameFiles.Values
                BW.Write(F.ID)
                BW.Write(F.FileName)
                BW.Write(F.RelativePath)
                BW.Write(F.Version)
                BW.Write(F.Type)

            Next

        Finally
            If BW IsNot Nothing Then BW.Close()
            If FS IsNot Nothing Then FS.Close()
        End Try
    End Sub


    Private mGameFiles As Dictionary(Of Integer, GameFile)
    Public ReadOnly Property GameFiles() As Dictionary(Of Integer, GameFile)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mGameFiles
        End Get
    End Property
    Private Sub setGameFiles(ByVal value As Dictionary(Of Integer, GameFile))
        Me.mGameFiles = value
    End Sub


    Private WithEvents mknpGetAll As Knop001
    Public ReadOnly Property knpGetAll() As Knop001
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mknpGetAll
        End Get
    End Property
    Private Sub setknpGetAll(ByVal value As Knop001)
        Me.mknpGetAll = value
    End Sub



    Private mOutput As TextList2D
    Public ReadOnly Property Output() As TextList2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mOutput
        End Get
    End Property
    Private Sub setOutput(ByVal value As TextList2D)
        Me.mOutput = value
    End Sub








    Private Sub mknpGetAll_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mknpGetAll.Clicked
        If e.Button = MouseOffset.Button0 And e.State = ClickedEvent.MouseState.Down Then
            Me.Output.WriteLine("Checking for Updates...")
            CLMain.Updater.Comm.GetFileVersionsAsync()
        End If
        'clmain.Updater.
    End Sub


    Protected Overrides Sub DisposeObject()
        Me.SaveGameFiles()
        MyBase.DisposeObject()

    End Sub

    Private Sub mUpdater_GetFileVersionsCompleted(ByVal sender As Object, ByVal e As ClientUpdaterCommunication.GetFileVersionsCompletedEventArgs) Handles mUpdater.GetFileVersionsCompleted

        Dim UpToDate As Boolean = True
        Me.setUpdateRequests(New List(Of Integer))
        For Each ID As Integer In e.Versions.Keys
            If Me.GameFiles.ContainsKey(ID) = False Then
                If Me.UpdateRequests.Count = 0 Then
                    Me.Output.WriteLine("Requesting file updates...")
                End If
                Me.UpdateRequests.Add(ID)
                UpToDate = False
                Me.mUpdater.RequestFileUpdateAsync(ID)

            Else
                If e.Versions(ID) > Me.GameFiles(ID).Version Then
                    If Me.UpdateRequests.Count = 0 Then
                        Me.Output.WriteLine("Requesting file updates...")
                    End If
                    Me.UpdateRequests.Add(ID)
                    UpToDate = False
                    Me.mUpdater.RequestFileUpdateAsync(ID)

                End If
            End If
        Next
        If UpToDate Then
            Me.Output.WriteLine("The Wizards is up to date!")
        End If

    End Sub


    Private mUpdateRequests As List(Of Integer)
    Public ReadOnly Property UpdateRequests() As List(Of Integer)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mUpdateRequests
        End Get
    End Property
    Private Sub setUpdateRequests(ByVal value As List(Of Integer))
        Me.mUpdateRequests = value
    End Sub


    Private mCoreFilesUpdated As Boolean
    Public ReadOnly Property CoreFilesUpdated() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCoreFilesUpdated
        End Get
    End Property
    Private Sub setCoreFilesUpdated(ByVal value As Boolean)
        Me.mCoreFilesUpdated = value
    End Sub



    Private Sub mUpdater_UpdateFile(ByVal sender As Object, ByVal e As ClientUpdaterCommunication.UpdateFileEventArgs) Handles mUpdater.UpdateFile

        Dim FS As IO.FileStream = Nothing



        Try

            If e.GameFile.Type = GameFile.GameFileType.Core Then
                Dim SavePath As String = Forms.Application.StartupPath & "\TempCoreUpdater"
                System.IO.Directory.CreateDirectory(SavePath & e.GameFile.RelativePath)
                FS = New IO.FileStream(SavePath & e.GameFile.RelativePath & "\" & e.GameFile.FileName, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
            Else
                Dim SavePath As String = Forms.Application.StartupPath
                System.IO.Directory.CreateDirectory(SavePath & e.GameFile.RelativePath)

                Dim SaveFile As String = SavePath & e.GameFile.RelativePath & "\" & e.GameFile.FileName
                If System.IO.File.Exists(SaveFile) Then
                    System.IO.File.Move(SaveFile, String.Format("{5}.{1}{0}{2}{3}{4}.B", Now.Day, Now.Month, Now.Hour, Now.Minute, Now.Second, SaveFile))
                End If
                FS = New IO.FileStream(SaveFile, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)

            End If


            FS.Write(e.FileData, 0, e.FileData.Length)

            FS.Close()

            If Me.GameFiles.ContainsKey(e.GameFile.ID) Then
                If Me.GameFiles(e.GameFile.ID).FileName <> e.GameFile.FileName Then Throw New Exception
                If Me.GameFiles(e.GameFile.ID).RelativePath <> e.GameFile.RelativePath Then Throw New Exception
                Me.GameFiles(e.GameFile.ID).UpdateVersion(e.GameFile.Version)
            Else
                Me.GameFiles.Add(e.GameFile.ID, e.GameFile.Clone)
            End If
            Me.SaveGameFiles()


        Catch ex As Exception
            Throw ex
        Finally
            If FS IsNot Nothing Then FS.Close()



        End Try

        If e.GameFile.Type = GameFile.GameFileType.Core Then Me.setCoreFilesUpdated(True)


        Me.Output.WriteLine("File '{0}' updated successfully. Type: '{1}'", e.GameFile.RelativePath & "\" & e.GameFile.FileName, e.GameFile.Type.ToString)
        Me.UpdateRequests.Remove(e.GameFile.ID)
        If Me.UpdateRequests.Count = 0 Then
            Me.Output.WriteLine("Update completed!")
            If Me.CoreFilesUpdated Then
                Me.Output.WriteLine("Core files have been updated. Restarting in 2 seconds...")
                Me.Scheduler.ScheduleAction(AddressOf Me.DoCoreUpdater, 2000)

            End If
        End If

    End Sub

    Public Sub DoCoreUpdater()
        System.Diagnostics.Process.Start(Forms.Application.StartupPath & "\CoreUpdater.exe")
        CLMain.StopSpel()
    End Sub
End Class

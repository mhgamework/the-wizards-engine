Public Class ModelManagerMenu
    Inherits Panel

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.mServerComm = DirectCast(Client.CLMain.ServerComm, AdminServerCommunication)


        Dim M As New DesignModelManager()
        M.FillPanel(Me)

        Me.Align = AlignType.MiddleCenter

        Me.lstModels = M.lstModels.GetListBox2D

        Me.knpGetModels = M.knpGetModelList.GetKnop001

        Me.knpHoofdmenu = M.knpHoofdmenu.GetKnop001

        Me.knpUpdate = M.knpUpdate.GetKnop001
        'Me.setlstGameFiles(M.lstFiles.GetListBox2D)
        'Me.setknpGetGameFiles(M.knpUpdateGameFiles.GetKnop001)


        Me.lblID = M.lblID.GetLabel
        Me.txtNaam = M.txtNaam.GetTextBox2D
        Me.txtGameFile = M.txtGameFile.GetTextBox2D
        Me.txtCollisionFile = M.txtCollisionFile.GetTextBox2D
        Me.txtCustomData = M.txtCustomData.GetTextBox2D
        Me.txtVersie = M.txtVersie.GetTextBox2D

        Me.txtLstOutput = M.txtLstOutput.GetTextList2D
        'Me.txtDescription = M.txtDescription.GetTextBox2D
        'Me.txtRelativePath = M.txtRelativePath.GetTextBox2D
        'Me.txtLocalFile = M.txtLocalFile.GetTextBox2D
        'Me.txtVersion = M.txtVersie.GetTextBox2D
        'Me.txtType = M.txtType.GetTextBox2D
        'Me.txtEnabled = M.txtEnabled.GetTextBox2D
        'Me.txtHash = M.txtHash.GetTextBox2D


        'Me.knpAddFile = M.knpAddFile.GetKnop001

        'Me.setAddFileMenu(New AddGameFileMenu(Me.HoofdObj))
        'Me.AddFileMenu.Enabled = False

        Me.lstModels.ClearItems()

    End Sub

    Private WithEvents mServerComm As AdminServerCommunication

    Private WithEvents TickElement As New TickElement(Me)
    Private WithEvents ProcessElement As New ProcessEventElement(Me)


    Private WithEvents lstModels As ListBox2D


    Private WithEvents knpGetModels As Knop
    Private WithEvents knpHoofdmenu As Knop

    Private WithEvents knpUpdate As Knop


    'Private WithEvents knpAddFile As Knop

    Private WithEvents lblID As Label
    Private WithEvents txtNaam As TextBox2D
    Private WithEvents txtGameFile As TextBox2D
    Private WithEvents txtCollisionFile As TextBox2D
    Private WithEvents txtCustomData As TextBox2D
    Private WithEvents txtVersie As TextBox2D

    Private WithEvents txtLstOutput As TextList2D
    'Private WithEvents txtDescription As TextBox2D
    'Private WithEvents txtRelativePath As TextBox2D
    'Private WithEvents txtLocalFile As TextBox2D
    'Private WithEvents txtVersion As TextBox2D
    'Private WithEvents txtType As TextBox2D
    'Private WithEvents txtEnabled As TextBox2D
    'Private WithEvents txtHash As TextBox2D

    Private Sub ProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessElement.Process
        With Me.HoofdObj.DevContainer.DX.Input
            If .KeyDown(DirectInput.Key.Escape) Then
                Me.Enabled = False
                ACLMain.HoofdMenu.Enabled = True
            End If
        End With
    End Sub


    Private Sub knpGetModels_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpGetModels.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.mServerComm.GetModelListAsync()

    End Sub


    Private Sub mServerComm_ModelListRecieved(ByVal sender As Object, ByVal e As AdminServerCommunication.ModelListRecievedEventArgs) Handles mServerComm.ModelListRecieved
        If Me.Enabled = False Then Exit Sub

        Me.lstModels.ClearItems()
        For Each AMD As AdminModelData In e.ML.Models
            Me.lstModels.AddItem(AMD)
        Next
    End Sub

    Private Sub knpHoofdmenu_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpHoofdmenu.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub

        Me.Enabled = False
        ACLMain.HoofdMenu.Enabled = True

    End Sub

    Private Sub lstModels_SelectedIndexChanged(ByVal sender As Object, ByVal e As Common.ListBox2D.SelectedIndexChangedEventArgs) Handles lstModels.SelectedIndexChanged
        If Me.lstModels.SelectedIndex = -1 Then
            Me.lblID.Text = ""
            Me.txtNaam.Text = ""
            Me.txtGameFile.Text = ""
            Me.txtCollisionFile.Text = ""
            Me.txtCustomData.Text = ""
            Me.txtVersie.Text = ""
        Else
            Dim AMD As AdminModelData = CType(Me.lstModels.Item(Me.lstModels.SelectedIndex), AdminModelData)

            Me.lblID.Text = AMD.ModelID.ToString
            Me.txtNaam.Text = AMD.Name
            Me.txtGameFile.Text = AMD.GameFile.ToString
            Me.txtCollisionFile.Text = AMD.CollisionFile.ToString
            Me.txtCustomData.Text = "Length = " & AMD.CustomData.Length.ToString
            Me.txtVersie.Text = AMD.Versie.ToString

        End If



    End Sub

    Private Sub knpUpdate_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpUpdate.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub

        Me.UpdateModelAsync()

    End Sub


    Public Enum UpdateModelStateType
        GettingGameFile = 1
        UploadingCollisionFile
        UpdatingModel
    End Enum

    Private mUpdateModelState As UpdateModelStateType
    Public ReadOnly Property UpdateModelState() As UpdateModelStateType
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mUpdateModelState
        End Get
    End Property
    Private Sub setUpdateModelState(ByVal value As UpdateModelStateType)
        Me.mUpdateModelState = value
    End Sub


    Private mUpdatingAMD As AdminModelData
    Public ReadOnly Property UpdatingAMD() As AdminModelData
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mUpdatingAMD
        End Get
    End Property
    Private Sub setUpdatingAMD(ByVal value As AdminModelData)
        Me.mUpdatingAMD = value
    End Sub


    Private Sub UpdateModelAsync()
        Dim ID As Integer
        Dim Naam As String
        Dim GameFile As Integer
        Dim CollisionFile As Integer
        Dim CustomData As Byte()



        Dim Versie As Integer = 0

        If Me.lstModels.SelectedIndex = -1 Then
            ID = -1
            CustomData = New Byte() {}
        Else
            Dim SAMD As AdminModelData = CType(Me.lstModels.Item(Me.lstModels.SelectedIndex), AdminModelData)
            ID = SAMD.ModelID

            CustomData = SAMD.CustomData




        End If


        Naam = Me.txtNaam.Text
        If Me.txtGameFile.Text = "" Then
            GameFile = -1
        Else
            GameFile = CInt(Me.txtGameFile.Text)
        End If
        If Me.txtCollisionFile.Text = "" Then
            CollisionFile = -1
        Else
            CollisionFile = CInt(Me.txtCollisionFile.Text)
        End If



        Dim AMD As New AdminModelData(ID, Naam, GameFile, CollisionFile, CustomData, Versie)

        Me.setUpdatingAMD(AMD)




        If CollisionFile = -1 And GameFile <> -1 Then
            'maak collision file

            Me.setUpdateModelState(UpdateModelStateType.GettingGameFile)
            Me.txtLstOutput.WriteLine("Getting Client File Data...")

            Me.mServerComm.GetGameFilesListAsync()
        Else
            Me.setUpdateModelState(UpdateModelStateType.UpdatingModel)
            Me.txtLstOutput.WriteLine("Updating Model Data...")
            Me.mServerComm.UpdateModelAsync(AMD)
        End If


    End Sub





    Private Sub mServerComm_GameFilesListReceived(ByVal sender As Object, ByVal e As AdminServerCommunication.GameFilesListReceivedEventArgs) Handles mServerComm.GameFilesListReceived
        If Me.UpdateModelState = UpdateModelStateType.GettingGameFile Then
            Me.txtLstOutput.WriteLine("Client File Data recieved.")

            Dim CFData As GameFile = Nothing

            For Each CF As GameFile In e.CFL.Files.Values
                If CF.ID = Me.UpdatingAMD.GameFile Then
                    CFData = CF
                    Exit For
                End If
            Next

            If CFData IsNot Nothing Then

                Me.txtLstOutput.WriteLine("Cooking Client File Mesh to Collision Data...")
                Dim CookedData As Byte() = Me.CookMesh(Forms.Application.StartupPath & CFData.RelativePath & "\" & CFData.FileName)


                Dim CollFileName As String = CFData.FileName.Substring(0, CFData.FileName.Length - 1) & "bin"

                Me.txtLstOutput.WriteLine("Uploading Collision Data File...")
                Me.setUpdateModelState(UpdateModelStateType.UploadingCollisionFile)
                Me.mServerComm.AddGameFileAsync(New AddGameFilePacket(CollFileName, CFData.RelativePath, CookedData))






            Else
                Throw New Exception("kan niet")

            End If



        End If



    End Sub


    Public Function CookMesh(ByVal nModelFilename As String) As Byte()
        Dim M As New Artificial.Heart.AHStaticModel(Me.HoofdObj.DevContainer.DX, nModelFilename)


        Dim G As GraphicsStream = Nothing
        Dim Vertex As Direct3D.CustomVertex.PositionOnly
        Dim Positions As Vector3()
        Dim TriangleIndices As Integer()
        Dim MaterialIndices As UShort() = Nothing
        Try
            G = M.Mesh.LockVertexBuffer(LockFlags.ReadOnly)
            Dim NumVs As Integer = M.Mesh.NumberVertices
            Positions = New Vector3(NumVs - 1) {}

            Dim VLength As Integer = M.Mesh.NumberBytesPerVertex

            For I As Integer = 0 To NumVs - 1
                G.Position = I * VLength
                Vertex = CType(G.Read(GetType(Direct3D.CustomVertex.PositionOnly)), Direct3D.CustomVertex.PositionOnly)
                Positions(I) = Vertex.Position
            Next

            G.Close()
            G.Dispose()
            G = Nothing

            G = M.Mesh.LockIndexBuffer(LockFlags.ReadOnly)
            Dim NumFaces As Integer = M.Mesh.NumberFaces
            TriangleIndices = New Integer(NumFaces * 3 - 1) {}
            For I As Integer = 0 To NumFaces * 3 - 1
                TriangleIndices(I) = CInt(CType(G.Read(GetType(Short)), Short))
            Next


            G.Close()
            G.Dispose()
            G = Nothing






        Finally
            If G IsNot Nothing Then
                G.Close()
                G.Dispose()
            End If

            M.Mesh.UnlockVertexBuffer()
            M.Mesh.UnlockIndexBuffer()
        End Try


        'Dim T As NxTriangleMeshDesc
        Dim TriangleMeshDesc As NxTriangleMeshDesc = NxTriangleMeshDesc.Default
        With TriangleMeshDesc
            .setPoints(Positions, True)
            .setTriangleIndices(TriangleIndices, True)
            .setMaterialIndices(MaterialIndices, True)

        End With

        Dim TriangleMesh As NxTriangleMesh = Nothing
        Dim BW As New ByteWriter

        NxCooking.InitCooking()
        Dim memStream As New NovodexMemoryStream
        If (NxCooking.CookTriangleMesh(TriangleMeshDesc, memStream)) Then
            BW.Write(memStream.Data)
            'TriangleMesh = acl.PhysicsSDK.createTriangleMesh(memStream)
        End If
        NxCooking.CloseCooking()


        Return memStream.Data
    End Function


    Private Sub mServerComm_AddGameFileCompleted(ByVal sender As Object, ByVal e As AdminServerCommunication.AddGameFileCompletedEventArgs) Handles mServerComm.AddGameFileCompleted
        If Me.UpdateModelState = UpdateModelStateType.UploadingCollisionFile Then
            Me.txtLstOutput.WriteLine("Collision File Uploaded.")
            Me.UpdatingAMD.CollisionFile = e.GameFileID

            Me.setUpdateModelState(UpdateModelStateType.UpdatingModel)
            Me.txtLstOutput.WriteLine("Updating Model Data...")
            Me.mServerComm.UpdateModelAsync(Me.UpdatingAMD)

        End If
    End Sub





    Private Sub mServerComm_UpdateModelCompleted(ByVal sender As Object, ByVal e As AdminServerCommunication.UpdateModelCompletedEventArgs) Handles mServerComm.UpdateModelCompleted
        Me.txtLstOutput.WriteLine("Model Updated Succesfull!")
        Me.mServerComm.GetModelListAsync()
    End Sub
End Class

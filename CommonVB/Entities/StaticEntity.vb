Public Class StaticEntity
    Inherits Entity

    Public Sub New(ByVal nParent As OctTree)
        MyBase.New(nParent)
        'Me.setModel(New XModel(Me))
        'Me.Model.ModelPad = Application.StartupPath & "\GameData\Models\Land001.x"
        'Me.Model.StartRootMatrix = Matrix.Scaling(10, 10, 10) * Matrix.RotationYawPitchRoll(0, Math.PI / 2, 0)
        'Me.Model.ModelPad = Application.StartupPath & "\GameData\Models\EilandEdravo001.X"
        'Me.Model.StartRootMatrix = Matrix.Scaling(0.04, 0.04, 0.04) '* Matrix.RotationYawPitchRoll(0, Math.PI / 2, 0)


    End Sub


    Public Overrides Sub Initialize()
        If Me.StaticEntityFunctions Is Nothing Then Throw New Exception("De Staticentityfunctions zijn niet ingesteld")
        MyBase.Initialize()

    End Sub



    Private mStaticEntityFunctions As StaEntFunctions
    Public ReadOnly Property StaticEntityFunctions() As StaEntFunctions
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStaticEntityFunctions
        End Get
    End Property
    Public Sub setStaticEntityFunctions(ByVal value As StaEntFunctions)
        Me.mStaticEntityFunctions = value
    End Sub





   


    'Public Sub CreateActor()
    '    If Me.Model.AHModel Is Nothing Then Exit Sub

    '    'Dim actorDesc As NxActorDesc
    '    'Dim bodyDesc As NxBodyDesc

    '    Dim G As GraphicsStream = Nothing
    '    Dim Vertex As Direct3D.CustomVertex.PositionOnly
    '    Dim Positions As Vector3()
    '    Dim TriangleIndices As Integer()
    '    Dim MaterialIndices As UShort() = Nothing
    '    Try
    '        G = Me.Model.AHModel.Mesh.LockVertexBuffer(LockFlags.ReadOnly)
    '        Dim NumVs As Integer = Me.Model.AHModel.Mesh.NumberVertices
    '        Positions = New Vector3(NumVs - 1) {}

    '        Dim VLength As Integer = Me.Model.AHModel.Mesh.NumberBytesPerVertex '= Direct3D.VertexInformation.GetFormatSize(Me.Model.AHModel.Mesh.VertexFormat)

    '        'Vs = CType(Me.Model.AHModel.Mesh.LockVertexBuffer(GetType(Direct3D.CustomVertex.PositionOnly), LockFlags.ReadOnly, NumVs), Direct3D.CustomVertex.PositionOnly())

    '        For I As Integer = 0 To NumVs - 1
    '            'Dim Vs As Direct3D.CustomVertex.PositionOnly
    '            G.Position = I * VLength
    '            Vertex = CType(G.Read(GetType(Direct3D.CustomVertex.PositionOnly)), Direct3D.CustomVertex.PositionOnly)
    '            Positions(I) = Vector3.TransformCoordinate(Vertex.Position, Me.Model.StartRootMatrix) ' Me.Model.RootMatrix)
    '        Next

    '        G.Close()
    '        G.Dispose()
    '        G = Nothing

    '        G = Me.Model.AHModel.Mesh.LockIndexBuffer(LockFlags.ReadOnly)
    '        Dim NumFaces As Integer = Me.Model.AHModel.Mesh.NumberFaces
    '        'TriangleIndices = CType(Me.Model.AHModel.Mesh.LockIndexBuffer(GetType(Short), LockFlags.ReadOnly, NumFaces * 3), Integer()) ' New Integer(NumFaces * 3 - 1) {}
    '        TriangleIndices = New Integer(NumFaces * 3 - 1) {}
    '        For I As Integer = 0 To NumFaces * 3 - 1
    '            TriangleIndices(I) = CInt(CType(G.Read(GetType(Short)), Short))
    '        Next


    '        'For I As Integer = 0 To NumFaces
    '        'g.Read (
    '        'Next

    '        G.Close()
    '        G.Dispose()
    '        G = Nothing






    '    Finally
    '        If G IsNot Nothing Then
    '            G.Close()
    '            G.Dispose()
    '        End If

    '        Me.Model.AHModel.Mesh.UnlockVertexBuffer()
    '        Me.Model.AHModel.Mesh.UnlockIndexBuffer()
    '    End Try


    '    'Dim T As NxTriangleMeshDesc
    '    Dim TriangleMeshDesc As NxTriangleMeshDesc = NxTriangleMeshDesc.Default
    '    With TriangleMeshDesc
    '        .setPoints(Positions, True)
    '        .setTriangleIndices(TriangleIndices, True)
    '        .setMaterialIndices(MaterialIndices, True)

    '    End With

    '    Dim TriangleMeshShapeDesc As NxTriangleMeshShapeDesc = NxTriangleMeshShapeDesc.Default
    '    Dim TriangleMesh As NxTriangleMesh = Nothing
    '    NxCooking.InitCooking()
    '    Dim memStream As New NovodexMemoryStream
    '    If (NxCooking.CookTriangleMesh(TriangleMeshDesc, memStream)) Then
    '        TriangleMesh = Me.BaseMain.PhysicsSDK.createTriangleMesh(memStream)
    '    End If
    '    NxCooking.CloseCooking()





    '    TriangleMeshShapeDesc.MeshData = TriangleMesh

    '    Dim actorDesc As NxActorDesc = NxActorDesc.Default
    '    'Dim bodyDesc As NxBodyDesc = NxBodyDesc.Default

    '    'actorDesc.BodyDesc = bodyDesc


    '    actorDesc.addShapeDesc(TriangleMeshShapeDesc)

    '    'actorDesc.density = 10
    '    actorDesc.globalPose = Matrix.Translation(Me.functions.Positie)
    '    Me.functions.setActor(Me.BaseMain.PhysicsScene.createActor(actorDesc))


    'End Sub

    'Public Sub DestroyActor()
    '    If Me.functions.Actor IsNot Nothing Then
    '        Me.functions.Actor.destroy()
    '        Me.functions.setActor(Nothing)
    '    End If
    'End Sub

    'Private mModel As XModel
    'Public ReadOnly Property Model() As XModel
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mModel
    '    End Get
    'End Property
    'Private Sub setModel(ByVal value As XModel)
    '    Me.mModel = value
    '    Me.Model.Positie = Me.functions.Positie
    'End Sub

    'Public Overrides Sub OnScaleVeranderd(ByVal sender As Object, ByVal e As ScaleVeranderdEventArgs)
    '    MyBase.OnScaleVeranderd(sender, e)
    '    If Me.Model IsNot Nothing Then Me.Model.Scale = Me.functions.Scale
    'End Sub


    'Public Overrides Sub OnPositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)
    '    MyBase.OnPositieVeranderd(sender, e)
    '    If Me.Model IsNot Nothing Then Me.Model.Positie = Me.functions.Positie
    'End Sub

    'Public Overrides Sub OnAhInitializeObjects(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.DeviceEventArgs)
    '    MyBase.OnAhInitializeObjects(sender, e)
    '    Me.LaadModel(Me.ModelID)
    'End Sub

    Protected Overrides Sub GetData(ByVal BW As ByteWriter)
        MyBase.GetData(BW)
        Me.StaticEntityFunctions.GetData(BW)
        'BW.Write(Me.StaticEntityFunctions.ModelID)
    End Sub

    Protected Overrides Sub Update(ByVal BR As ByteReader)
        MyBase.Update(BR)
        Me.StaticEntityFunctions.Update(BR)
        'Me.StaticEntityFunctions.LaadModel(BR.ReadInt32)
        'Me.LaadModel(BR.ReadInt32)
    End Sub


    Public Overrides Sub ReadSerializedData(ByVal nDS As DataSerializer)
        MyBase.ReadSerializedData(nDS)
        Me.StaticEntityFunctions.ReadSerializedData(nDS)
    End Sub

    Public Overrides Sub WriteSerializeData(ByVal nDS As DataSerializer)
        MyBase.WriteSerializeData(nDS)
        Me.StaticEntityFunctions.WriteSerializeData(nDS)
    End Sub

End Class

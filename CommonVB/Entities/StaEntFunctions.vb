Public Class StaEntFunctions
    Inherits SpelObject

    Public Sub New(ByVal nParent As StaticEntity)
        MyBase.New(nParent)
        Me.setStaticEntity(nParent)
        Me.StaticEntity.BaseMain.ModelManager.AddStaticEntityFunctions(Me)
    End Sub


    Private mStaticEntity As StaticEntity
    Public ReadOnly Property StaticEntity() As StaticEntity
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStaticEntity
        End Get
    End Property
    Private Sub setStaticEntity(ByVal value As StaticEntity)
        Me.mStaticEntity = value
    End Sub


    Private mModelID As Integer
    Public ReadOnly Property ModelID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelID
        End Get
    End Property
    Protected Overridable Sub setModelID(ByVal value As Integer)
        Me.mModelID = value
    End Sub


    Private mModelVersie As Integer
    Public ReadOnly Property ModelVersie() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelVersie
        End Get
    End Property
    Protected Sub setModelVersie(ByVal value As Integer)
        Me.mModelVersie = value
    End Sub


    Public Overridable Sub LaadModel(ByVal nMD As ModelData)


    End Sub


    Public Sub ClearActor()
        If Me.StaticEntity.Functions.Actor IsNot Nothing Then
            Me.StaticEntity.Functions.Actor.destroy()
        End If
    End Sub

    Public Sub CreateActor(ByVal nCollisionFileID As Integer)
        Me.ClearActor()
        If nCollisionFileID <> -1 Then
            Dim TriangleMeshShapeDesc As NovodexWrapper.NxTriangleMeshShapeDesc = NovodexWrapper.NxTriangleMeshShapeDesc.Default
            Dim TriangleMesh As NovodexWrapper.NxTriangleMesh = Nothing
            Dim CF As GameFile = Me.StaticEntity.BaseMain.GameFilesList(nCollisionFileID)
            Dim B As Byte() = IO.File.ReadAllBytes(CF.LocalFile)

            Dim memStream As New NovodexWrapper.NovodexMemoryStream(B, CUInt(B.Length))

            TriangleMesh = Me.StaticEntity.BaseMain.PhysicsSDK.createTriangleMesh(memStream)


            TriangleMeshShapeDesc.MeshData = TriangleMesh

            Dim actorDesc As NovodexWrapper.NxActorDesc = NovodexWrapper.NxActorDesc.Default
            'Dim bodyDesc As NxBodyDesc = NxBodyDesc.Default

            'actorDesc.BodyDesc = bodyDesc


            actorDesc.addShapeDesc(TriangleMeshShapeDesc)

            'actorDesc.density = 10
            actorDesc.globalPose = Matrix.Translation(Me.StaticEntity.Functions.Positie)

            Me.StaticEntity.Functions.setActor(Me.StaticEntity.BaseMain.PhysicsScene.createActor(actorDesc))
        End If

    End Sub


    Public Overridable Sub ReloadModel()

    End Sub


    'Private mLastStaticEntityDataPacket As StaticEntityDataPacket
    'Public ReadOnly Property LastStaticEntityDataPacket() As StaticEntityDataPacket
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mLastStaticEntityDataPacket
    '    End Get
    'End Property
    'Private Sub setLastStaticEntityDataPacket(ByVal value As StaticEntityDataPacket)
    '    Me.mLastStaticEntityDataPacket = value
    'End Sub


    Public Overridable Sub GetData(ByVal BW As ByteWriter)
        Dim P As New StaticEntityDataPacket
        P.ModelID = Me.ModelID
        Dim MD As ModelData = Me.StaticEntity.BaseMain.ModelManager.ModelData(Me.ModelID)
        If MD Is Nothing Then
            P.ModelVersie = -1
        Else
            P.ModelVersie = MD.Versie
        End If

        BW.Write(P)
    End Sub

    Public Overridable Sub Update(ByVal BR As ByteReader)
        Dim P As StaticEntityDataPacket = StaticEntityDataPacket.FromNetworkBytes(BR)
        'Me.setLastStaticEntityDataPacket(P)
        Me.setModelID(P.ModelID)
        Me.setModelVersie(P.ModelVersie)

        Me.ReloadModel()
    End Sub

    Protected Overrides Sub DisposeObject()
        Me.StaticEntity.BaseMain.ModelManager.RemoveStaticEntityFunctions(Me)
        MyBase.DisposeObject()
    End Sub


    Public Overridable Sub ReadSerializedData(ByVal nDS As DataSerializer)

    End Sub

    Public Overridable Sub WriteSerializeData(ByVal nDS As DataSerializer)

    End Sub
End Class

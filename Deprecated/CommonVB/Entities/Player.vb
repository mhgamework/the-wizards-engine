Public Class Player
    Inherits Entity

    Public Sub New(ByVal nParent As OctTree)
        MyBase.New(nParent)
        Me.setBoundingBoxRadius(New Vector3(0.5, 0.5, 0.5))
        Me.setBoundingSphereRadius(0.5)


        'Dim X As New XModelElement(Me)


        'X.ModelPad = Application.StartupPath & "\GameData\Models\DraakHoofd001.x"
        'X.StartRootMatrix = Matrix.Scaling(0.05, 0.05, 0.05) * Matrix.RotationYawPitchRoll(0, -Math.PI / 2, 0)

        'Dim x As New BolElement(Me)
        'x.Scale = New Vector3(2, 2, 2)





        'Me.setModelElement(x)
        'Me.setNaamText(New Text3D(Me))
        'Me.NaamText.Size = New Vector2(400, 40)
        'Me.NaamText.Text2D.FontHeight = 40
        'Me.NaamText.Scale = New Vector2(4, 0.4)
        'Me.NaamText.Text2D.TextAlign = DrawTextFormat.Center Or DrawTextFormat.VerticalCenter
        'Me.NaamText.Text2D.FontColor = Color.Yellow
        'Me.NaamText.Renderer.BackgroundColor = Color.FromArgb(150, 0, 0, 0)

        'Me.setLamp(New Lamp001(Me))
        'Me.Lamp.Enabled = False



    End Sub

    Public Overrides Sub Initialize()
        If Me.PlayerFunctions Is Nothing Then Throw New Exception("Geen playerfunctions aanwezig")
        MyBase.Initialize()

        Me.CreateActor()
    End Sub


    Private mPlayerFunctions As PlayerFunctions
    Public ReadOnly Property PlayerFunctions() As PlayerFunctions
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPlayerFunctions
        End Get
    End Property
    Public Sub setPlayerFunctions(ByVal value As PlayerFunctions)
        Me.mPlayerFunctions = value
    End Sub


    Private mHitReport As PlayerControllerHitReport
    Public ReadOnly Property HitReport() As PlayerControllerHitReport
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mHitReport
        End Get
    End Property
    Private Sub setHitReport(ByVal value As PlayerControllerHitReport)
        Me.mHitReport = value
    End Sub


    Private mController As NxCapsuleController
    Public ReadOnly Property Controller() As NxCapsuleController
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mController
        End Get
    End Property
    Private Sub setController(ByVal value As NxCapsuleController)
        Me.mController = value
    End Sub


    Public Function createSpikeShapeDesc(ByVal width As Single, ByVal height As Single, ByVal depth As Single, ByVal offset As Vector3) As NxConvexShapeDesc
        Dim localSpaceVertArray As Vector3() = New Vector3() {(offset + New Vector3(0.0!, (height / 2.0!), 0.0!)), (offset + New Vector3((width / 2.0!), (-height / 2.0!), 0.0!)), (offset + New Vector3((-width / 2.0!), (-height / 2.0!), 0.0!)), (offset + New Vector3(0.0!, 0.0!, depth))}
        Return NovodexUtil.createConvexShapeDescFromVertexCloud(localSpaceVertArray)
    End Function

    Public Sub CreateActor()
        Me.setHitReport(New PlayerControllerHitReport)
        Dim controllerDesc As NxCapsuleControllerDesc = NxCapsuleControllerDesc.Default
        controllerDesc.position = Me.Functions.Positie 'New Vector3(0, 30, 0) '
        controllerDesc.upDirection = NxHeightFieldAxis.NX_Y
        controllerDesc.radius = 1
        controllerDesc.height = 2 '(capsuleTotalHeight - (capsuleRadius * 2.0!))
        controllerDesc.stepOffset = 2 'stepOffset
        controllerDesc.Callback = Me.HitReport
        Dim controller As NxCapsuleController = DirectCast(ControllerManager.createController(Me.BaseMain.PhysicsScene, controllerDesc), NxCapsuleController)
        Dim shapeDesc As NxCapsuleShapeDesc = NxCapsuleShapeDesc.Default
        shapeDesc.height = 2.1
        shapeDesc.radius = 1

        controller.getActor.createShape(shapeDesc)

        'If (Not controller Is Nothing) Then
        '    controller.getActor.FlagFrozenRot = True
        '    controller.getActor.setName("CapsuleController")
        '    'If addArrowShapeFlag Then
        '    'controller.getActor.createShape(Me.createSpikeShapeDesc((capsuleRadius / 4.0!), (capsuleRadius / 4.0!), (capsuleRadius * 2.0!), New Vector3(0.0!, ((capsuleTotalHeight - (capsuleRadius * 2.0!)) / 2.0!), 0.0!)))
        '    controller.getActor.createShape(Me.createSpikeShapeDesc(2 / 4.0!, 2 / 4.0!, 2 / 4.0!, New Vector3(0.0!, 2 / 0.2!, 0.0!)))
        '    controller.getActor.getLastShape.FlagDisableCollision = True
        '    'End If
        'End If




        Me.setController(controller)

        Me.functions.setActor(controller.getActor)


        'Dim actorDesc As New NxActorDesc
        'Dim bodyDesc As New NxBodyDesc
        'Dim sphereDesc As New NxSphereShapeDesc
        'sphereDesc.radius = 1
        'actorDesc.addShapeDesc(sphereDesc)

        ''Dim boxDesc As New NxBoxShapeDesc
        ''boxDesc.dimensions = New Vector3(1, 1, 1)
        ''actorDesc.addShapeDesc(boxDesc)

        'actorDesc.BodyDesc = bodyDesc
        'actorDesc.density = 10
        'actorDesc.globalPose = Matrix.Translation(Me.functions.Positie)
        'Me.functions.setActor(Me.BaseMain.PhysicsScene.createActor(actorDesc))





    End Sub



    'Public Overrides Sub OnPositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)
    '    MyBase.OnPositieVeranderd(sender, e)
    '    'If e.Direct Then
    '    '    Me.Controller.setPosition(Me.functions.Positie)
    '    'End If
    '    Me.ModelElement.Positie = Me.functions.Positie
    '    'If Me.Lamp IsNot Nothing Then Me.Lamp.Positie = Me.Positie
    '    Me.NaamText.Positie = Me.functions.Positie + New Vector3(0, 4, 0)
    'End Sub



    'Private mLamp As Lamp001
    'Public ReadOnly Property Lamp() As Lamp001
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mLamp
    '    End Get
    'End Property
    'Private Sub setLamp(ByVal value As Lamp001)
    '    Me.mLamp = value
    'End Sub


    Private mObservingNode As OctTreeNode
    Public ReadOnly Property ObservingNode() As OctTreeNode
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mObservingNode
        End Get
    End Property
    Public Sub setObservingNode(ByVal value As OctTreeNode)
        Me.mObservingNode = value
    End Sub


    Private mClient As BaseClient
    Public ReadOnly Property Client() As BaseClient
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mClient
        End Get
    End Property
    Private Sub setClient(ByVal value As BaseClient)
        Me.mClient = value
    End Sub


    Public Sub LinkClient(ByVal CL As BaseClient)
        Me.setClient(CL)
        If Me.ParentNode IsNot Nothing Then
            CType(Me.Parent, OctTreeNode).OctTree.RootNode.AddPlayerObserver(Me)
        End If
    End Sub





    Private mModelElement As Model
    Public ReadOnly Property ModelElement() As Model
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelElement
        End Get
    End Property
    Private Sub setModelElement(ByVal value As Model)
        Me.mModelElement = value
    End Sub


    'Public Overrides Sub OnScaleVeranderd(ByVal sender As Object, ByVal e As ScaleVeranderdEventArgs)
    '    MyBase.OnScaleVeranderd(sender, e)
    '    Me.ModelElement.Scale = Me.functions.Scale
    'End Sub









    Protected Overrides Sub Update(ByVal BR As ByteReader)
        MyBase.Update(BR)
        Me.PlayerFunctions.Update(BR)

    End Sub

    Protected Overrides Sub GetData(ByVal BW As ByteWriter)
        MyBase.GetData(BW)
        Me.PlayerFunctions.GetData(BW)

    End Sub

    Public Overrides Sub WriteSerializeData(ByVal nDS As DataSerializer)
        MyBase.WriteSerializeData(nDS)
        Me.PlayerFunctions.WriteSerializeData(nDS)
    End Sub

    Public Overrides Sub ReadSerializedData(ByVal nDS As DataSerializer)
        MyBase.ReadSerializedData(nDS)
        Me.PlayerFunctions.ReadSerializedData(nDS)
    End Sub


    'Public Overrides Sub OnRotatieVeranderd(ByVal sender As Object, ByVal e As RotatieVeranderdEventArgs)
    '    MyBase.OnRotatieVeranderd(sender, e)
    '    Me.ModelElement.Rotatie = Me.functions.RotatieMatrix
    'End Sub
    
    'Private Sub Player_PositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs) Handles Me.PositieVeranderd
    '    If e.Direct Then
    '        If Me.Controller IsNot Nothing Then Me.Controller.setPosition(Me.Functions.Positie)

    '    End If
    'End Sub
End Class

Public Class Box001
    Inherits Entity

    Public Sub New(ByVal nParent As SpelObject, Optional ByVal Density As Integer = 10)
        MyBase.New(nParent)
        Me.setBoxModel(New Box3D(Me))
        'Me.setBoundingBoxRadius(New Vector3(1, 1, 1))
        'Me.setBoundingBoxRadius(1)
        Me.Scale = New Vector3(1, 1, 1)
        'Me.setBoxGeom(New Ode.BoxGeom(10, 10, 10)) 'New Ode.BoxGeom(0.5 * 100))
        'Me.Body.Geoms.Add(Me.BoxGeom)

        'Me.Body.GravityInfluenced = False

        Me.CreateBox(Density)




    End Sub


    Public Sub CreateBox(ByVal nDensity As Integer)
        'NovodexUtil.createBoxActor(Me.BaseMain.PhysicsScene, New Vector3(0.5!, 0.5!, 0.5!), Matrix.Translation(-3.0!, 0.25!, 0.0!), 10.0!)

        'Exit Sub
        Dim actorDesc As New NxActorDesc
        Dim bodyDesc As New NxBodyDesc
        Dim boxDesc As New NxBoxShapeDesc

        boxDesc.dimensions = New Vector3(0.5, 0.5, 0.5)
        actorDesc.addShapeDesc(boxDesc)
        If nDensity > 0 Then
            actorDesc.BodyDesc = bodyDesc
            actorDesc.density = nDensity
        Else
            actorDesc.BodyDesc = Nothing
        End If
        actorDesc.globalPose = Matrix.Translation(Me.Positie)

        Me.setActor(Me.BaseMain.PhysicsScene.createActor(actorDesc))

    End Sub

    Private mNxBox As NxBox
    Public ReadOnly Property NxBox() As NxBox
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNxBox
        End Get
    End Property
    Private Sub setNxBox(ByVal value As NxBox)
        Me.mNxBox = value
    End Sub


    'Private mBoxGeom As Ode.Geom
    'Public ReadOnly Property BoxGeom() As Ode.Geom
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mBoxGeom
    '    End Get
    'End Property
    'Private Sub setBoxGeom(ByVal value As Ode.Geom)
    '    Me.mBoxGeom = value
    'End Sub



    Private mBoxModel As Box3D
    Public ReadOnly Property BoxModel() As Box3D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoxModel
        End Get
    End Property
    Private Sub setBoxModel(ByVal value As Box3D)
        Me.mBoxModel = value
    End Sub


    Public Overrides Property Scale() As Microsoft.DirectX.Vector3
        Get
            Return MyBase.Scale
        End Get
        Set(ByVal value As Microsoft.DirectX.Vector3)
            MyBase.Scale = value
            'Dim Half As Vector3 = value * (1 / 2)
            'Me.setBoundingBoxRadius(Half)
            'Me.setBoundingBoxRadius(Half.Length)

            Me.UpdateRootMatrix()
        End Set
    End Property


    Public Sub UpdateRootMatrix()
        If Me.BoxModel Is Nothing Then Exit Sub
        If Me.BoxModel.AHModel Is Nothing Then Exit Sub
        Me.BoxModel.AHModel.RootMatrix = Matrix.Scaling(Me.Scale) * Me.RotatieMatrix * Matrix.Translation(Me.Positie)
    End Sub

    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As spelobject.deviceeventargs)
        MyBase.OnDeviceReset(sender, e)
        Me.UpdateRootMatrix()
    End Sub

    Protected Overrides Sub GetData(ByVal BW As ByteWriter)
        MyBase.GetData(BW)
        BW.Write(Me.Scale)
    End Sub

    Protected Overrides Sub Update(ByVal BR As ByteReader)
        MyBase.Update(BR)
        Me.Scale = BR.ReadVector3
    End Sub



    Public Overrides Sub OnPositieVeranderd()
        MyBase.OnPositieVeranderd()
        Me.UpdateRootMatrix()
    End Sub

    Public Overrides Sub OnRotatieVeranderd()
        MyBase.OnRotatieVeranderd()
        Me.UpdateRootMatrix()
    End Sub

    Private Sub Box001_Process(ByVal ev As MHGameWork.Game3DPlay.ProcessEvent) Handles Me.Process
        Me.BoxModel.Visible = Not Me.Actor.isSleeping

    End Sub
End Class

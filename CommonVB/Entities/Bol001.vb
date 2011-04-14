Public Class Bol001
    Inherits Entity

    Public Sub New(ByVal nParent As OctTree)
        MyBase.New(nParent)
        'Me.setBolElement(New BolElement(Me))

    End Sub
    'Public Sub New(ByVal nParent As Entity)
    '    MyBase.new(nParent)
    '    Me.Init()
    'End Sub

    Public Overrides Sub Initialize()
        MyBase.Initialize()
        Me.Functions.Scale = New Vector3(0.5, 0.5, 0.5)
        Me.CreateSphere()
    End Sub


    'Private mBolElement As BolElement
    'Public ReadOnly Property BolElement() As BolElement
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mBolElement
    '    End Get
    'End Property
    'Private Sub setBolElement(ByVal value As BolElement)
    '    Me.mBolElement = value
    'End Sub


    Public Sub CreateSphere()
        Dim actorDesc As New NxActorDesc
        Dim bodyDesc As New NxBodyDesc
        Dim sphereDesc As New NxSphereShapeDesc
        sphereDesc.radius = Me.functions.Scale.X / 2
        actorDesc.addShapeDesc(sphereDesc)


        actorDesc.BodyDesc = bodyDesc
        actorDesc.density = 10
        actorDesc.globalPose = Matrix.Translation(Me.functions.Positie)
        Me.Functions.setActor(Me.BaseMain.PhysicsScene.createActor(actorDesc))
    End Sub


    'Public Overrides Property Scale() As Microsoft.DirectX.Vector3
    '    Get
    '        Return MyBase.Scale
    '    End Get
    '    Set(ByVal value As Microsoft.DirectX.Vector3)
    '        MyBase.Scale = value
    '        Dim Half As Vector3 = value * (1 / 2)
    '        Me.setBoundingBoxRadius(Half)
    '        Me.setBoundingSphereRadius(Half.Length)

    '        Me.UpdateRootMatrix()
    '    End Set
    'End Property


    'Public Sub UpdateRootMatrix()
    '    If Me.BolModel Is Nothing Then Exit Sub
    '    If Me.BolModel.AHModel Is Nothing Then Exit Sub
    '    Me.BolModel.AHModel.RootMatrix = Matrix.Scaling(Me.Scale) * Me.RotatieMatrix * Matrix.Translation(Me.Positie)
    'End Sub


    Protected Overrides Sub GetData(ByVal BW As ByteWriter)
        MyBase.GetData(BW)
        BW.Write(Me.functions.Scale)
    End Sub

    Protected Overrides Sub Update(ByVal BR As ByteReader)
        MyBase.Update(BR)
        Me.functions.Scale = BR.ReadVector3
    End Sub



    'Public Overrides Sub OnPositieVeranderd()
    '    MyBase.OnPositieVeranderd()
    '    Me.UpdateRootMatrix()
    'End Sub

    'Public Overrides Sub OnRotatieVeranderd()
    '    MyBase.OnRotatieVeranderd()
    '    Me.UpdateRootMatrix()
    'End Sub

    'Public Overrides Sub OnScaleVeranderd(ByVal sender As Object, ByVal e As ScaleVeranderdEventArgs)
    '    MyBase.OnScaleVeranderd(sender, e)
    '    Me.BolElement.Scale = Me.functions.Scale
    'End Sub

    'Public Overrides Sub OnPositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)
    '    MyBase.OnPositieVeranderd(sender, e)
    '    Me.BolElement.Positie = Me.functions.Positie
    'End Sub

    'Public Overrides Sub OnRotatieVeranderd(ByVal sender As Object, ByVal e As RotatieVeranderdEventArgs)
    '    MyBase.OnRotatieVeranderd(sender, e)
    '    Me.BolElement.Rotatie = Me.functions.RotatieMatrix
    'End Sub

End Class

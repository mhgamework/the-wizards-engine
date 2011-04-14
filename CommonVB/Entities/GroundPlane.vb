Public Class GroundPlane
    Inherits Entity

    Public Sub New(ByVal nParent As OctTree)
        MyBase.New(nParent)

    End Sub

    Public Overrides Sub Initialize()
        MyBase.Initialize()
        Me.CreateGroundPlane()
    End Sub

    Public Sub CreateGroundPlane()
        Dim planeDesc As New NxPlaneShapeDesc
        planeDesc.materialIndex = 0
        Dim actorDesc As New NxActorDesc
        actorDesc.addShapeDesc(planeDesc)

        Me.Functions.setActor(Me.BaseMain.PhysicsScene.createActor(actorDesc))


    End Sub

End Class

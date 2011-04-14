Public Class DebugPhysicsRenderElement
    Inherits RenderElement

    Public Sub New(ByVal nParent As SpelObject, ByVal nScene As NxScene)
        MyBase.New(nParent)
        Me.setPhysicsScene(nScene)
    End Sub


    Private mPhysicsDebugRenderer As PhysicsDebugRenderer
    Public ReadOnly Property PhysicsDebugRenderer() As PhysicsDebugRenderer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPhysicsDebugRenderer
        End Get
    End Property
    Public Sub setPhysicsDebugRenderer(ByVal value As PhysicsDebugRenderer)
        Me.mPhysicsDebugRenderer = value
    End Sub


    Private mPhysicsScene As NxScene
    Public ReadOnly Property PhysicsScene() As NxScene
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPhysicsScene
        End Get
    End Property
    Private Sub setPhysicsScene(ByVal value As NxScene)
        Me.mPhysicsScene = value
    End Sub




    Public Overrides Sub OnRender(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.RenderElement.RenderEventArgs)
        MyBase.OnRender(sender, e)
        If Me.Visible Then Me.drawScene()
    End Sub

    Private Sub drawScene()
        If Me.PhysicsDebugRenderer Is Nothing Then Exit Sub

        Me.HoofdObj.DevContainer.DX.SetTexture(0, Nothing)
        Me.PhysicsDebugRenderer.renderData(Me.PhysicsScene.getDebugRenderable)
        Dim cloth As NxCloth
        For Each cloth In Me.PhysicsScene.getCloths
            Me.PhysicsDebugRenderer.drawTrianglesFromVertexTriplets(cloth.getMeshData.getTrianglesAsVertexTriplets, &HFF0000)
        Next
    End Sub
End Class

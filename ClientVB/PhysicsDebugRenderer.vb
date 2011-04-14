Public Class PhysicsDebugRenderer
    Inherits SpelObject
    Implements IRenderable


    Public Sub New(ByVal nParent As SpelObject, ByVal nDebugRenderer As NovodexDebugRenderer, ByVal nScene As NxScene)
        MyBase.New(nParent)
        Me.setDebugRenderer(nDebugRenderer)
        Me.setPhysicsScene(nScene)
    End Sub

    Public Sub OnAfterRender(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.RenderEventArgs) Implements Game3DPlay.Core.Elements.IRenderable.OnAfterRender

    End Sub

    Public Sub OnBeforeRender(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.RenderEventArgs) Implements Game3DPlay.Core.Elements.IRenderable.OnBeforeRender

    End Sub

    Public Sub OnRender(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.RenderEventArgs) Implements Game3DPlay.Core.Elements.IRenderable.OnRender
        If Me.DebugRenderer Is Nothing Then Return
        'Me.DebugRenderer.setRenderDevice(e.Graphics.GraphicsDevice)
        '' ''Me.HoofdObj.DevContainer.DX.SetTexture(0, Nothing)
        Me.DebugRenderer.renderData(Me.PhysicsScene.getDebugRenderable, e.CameraInfo.ViewMatrix, e.CameraInfo.ProjectionMatrix)
        '' ''Dim cloth As NxCloth
        '' ''For Each cloth In Me.PhysicsScene.getCloths
        '' ''    Me.PhysicsDebugRenderer.drawTrianglesFromVertexTriplets(cloth.getMeshData.getTrianglesAsVertexTriplets, &HFF0000)
        '' ''Next
    End Sub





    Private mDebugRenderer As NovodexDebugRenderer
    Public ReadOnly Property DebugRenderer() As NovodexDebugRenderer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDebugRenderer
        End Get
    End Property
    Private Sub setDebugRenderer(ByVal value As NovodexDebugRenderer)
        Me.mDebugRenderer = value
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




End Class

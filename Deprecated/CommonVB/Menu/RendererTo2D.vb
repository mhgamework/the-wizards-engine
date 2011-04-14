Public Class RendererTo2D
    Inherits PanelPart

    Public Sub New(ByVal nParent As Panel)
        MyBase.New(nParent)

    End Sub

    Private RendererTo2DContainer As New RendererTo2DContainer(Me)


    Private WithEvents RenderElement As New RenderEventElement(Me)


    Private mBackgroundColor As Color
    Public Property BackgroundColor() As Color
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.RendererTo2DContainer.BackgroundColor
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Color)
            Me.RendererTo2DContainer.BackgroundColor = value
        End Set
    End Property


    Public ReadOnly Property ViewPort() As Viewport
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.RendererTo2DContainer.ViewPort
        End Get
    End Property



    Private Sub RenderElement_AfterRender(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.AfterRender
        Me.OnAfterRender(sender, e)
    End Sub

    Private Sub RenderElement_BeforeRender(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.BeforeRender
        Me.OnBeforeRender(sender, e)
    End Sub

    Private Sub RenderElement_Render(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.Render
        Me.OnRender(sender, e)
    End Sub


    Protected Sub OnBeforeRender(ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)
        RaiseEvent BeforeRender(sender, e)
    End Sub
    Protected Sub OnRender(ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)
        RaiseEvent Render(sender, e)
    End Sub
    Protected Sub OnAfterRender(ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)
        RaiseEvent AfterRender(sender, e)
    End Sub


    Public Event BeforeRender(ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)


    Public Event Render(ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)


    Public Event AfterRender(ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)

    Private Sub RendererTo2D_PositieChanged(ByVal sender As Object, ByVal e As Game3DPlay.PanelPart.PositieChangedEventArgs) Handles Me.PositieChanged
        Me.RendererTo2DContainer.Positie = Me.Positie
    End Sub

    Private Sub RendererTo2D_SizeChanged(ByVal sender As Object, ByVal e As Game3DPlay.Size2DChangedEventArgs) Handles Me.SizeChanged
        Me.RendererTo2DContainer.Size = Me.Size

    End Sub
End Class

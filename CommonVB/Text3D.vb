Public Class Text3D
    Inherits SpelObject3D

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        Me.RendererContainer.Visible = False
        Me.setText2D(New Text2D(Me))
        Me.Text2D.Positie = Vector2.Empty
        Me.Text2D.Text = "test"
        Me.Scale = New Vector2(1, 1)
        Me.Size = New Vector2(200, 200)
    End Sub

    Private RendererContainer As New Renderer2DContainer(Me)
    Private WithEvents RenderElement As New RenderEventElement(Me)

    Private mSize As Vector2
    Public Property Size() As Vector2
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSize
        End Get
        Set(ByVal value As Vector2)
            Me.mSize = value
            Me.RendererContainer.Size = value
            Me.Text2D.Size = value
        End Set
    End Property


    Private mBackgroundColor As Color
    Public Property BackgroundColor() As Color
        Get
            Return Me.RendererContainer.BackgroundColor
        End Get
        Set(ByVal value As Color)
            Me.RendererContainer.BackgroundColor = value
        End Set
    End Property



    Private mText2D As Text2D
    Public ReadOnly Property Text2D() As Text2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mText2D
        End Get
    End Property
    Private Sub setText2D(ByVal value As Text2D)
        Me.mText2D = value
    End Sub


    Private mPositie As Vector3
    Public Property Positie() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPositie
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
            Me.mPositie = value
        End Set
    End Property


    Private mScale As Vector2
    Public Property Scale() As Vector2
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mScale
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector2)
            Me.mScale = value
        End Set
    End Property



    Private Sub Text3D_Render3D(ByVal ev As MHGameWork.Game3DPlay.Render3DEvent) Handles Me.Render3D

    End Sub

    Private Sub RenderElement_BeforeRender(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.BeforeRender
        Me.RendererContainer.OnBeforeRender(sender, e)
    End Sub

    Private Sub RenderElement_Render(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.Render
        Me.HoofdObj.DevContainer.DX.SpriteRenderer3D.Render(Me.RendererContainer.Tex, Me.Positie, Me.Scale)
    End Sub
End Class

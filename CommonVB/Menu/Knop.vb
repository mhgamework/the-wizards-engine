Public Class Knop
    Inherits PanelPart
    'Implements IPositionable2D
    'Implements IClickable


    Public Sub New(ByVal nParent As Panel, ByVal nTexturePad As String)
        MyBase.New(nParent)
        Me.setText2D(New SpelObjecten.Text2D(Me))
        Me.setImage2D(New SpelObjecten.Image2D(Me))

        Me.Text2D.Positie = Vector2.Zero

        Me.Text = "(Leeg)"
        Me.Size = New Vector2(100, 100)

        Me.Image2D.Filename = nTexturePad




        'Me.AddEventHandler(Of ClickedEvent)(AddressOf Me.OnClicked)
    End Sub



    Private mImage2D As SpelObjecten.Image2D
    Public ReadOnly Property Image2D() As SpelObjecten.Image2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mImage2D
        End Get
    End Property
    Private Sub setImage2D(ByVal value As SpelObjecten.Image2D)
        Me.mImage2D = value
    End Sub




    Private WithEvents mText2D As SpelObjecten.Text2D
    Public ReadOnly Property Text2D() As SpelObjecten.Text2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mText2D
        End Get
    End Property
    Private Sub setText2D(ByVal value As SpelObjecten.Text2D)
        Me.mText2D = value
    End Sub


    Public Overridable Property Text() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Text2D.Text
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As String)
            Me.Text2D.Text = value
        End Set
    End Property



    ' '' ''Private Sub Knop_SizeChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.Size2DChangedEventArgs) Handles Me.SizeChanged
    ' '' ''    Me.SpriteElement.Size = Me.Size
    ' '' ''    'Me.Renderer.Size = value
    ' '' ''    Me.Text2D.Size = Me.Size
    ' '' ''End Sub

    Private Sub Knop_PositieChanged1(ByVal sender As Object, ByVal e As Game3DPlay.Gui.PanelPart.PositieChangedEventArgs) Handles Me.PositieChanged
        Me.UpdateDimensions()
    End Sub

    Public Sub UpdateDimensions()
        Me.Image2D.Positie = Me.Positie
        Me.Image2D.Size = Me.Size

        If Me.Text2D.Font Is Nothing Then Exit Sub

        Dim TextSize As Vector2 = Me.Text2D.Font.MeasureString(Me.Text2D.Text)
        Me.Text2D.Positie = Me.Positie + Me.Size * 0.5 - TextSize * 0.5

    End Sub

    Private Sub mText2D_FontLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles mText2D.FontLoaded
        Me.UpdateDimensions()
    End Sub

    Private Sub mText2D_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mText2D.TextChanged
        Me.UpdateDimensions()
    End Sub

    Private Sub Knop_SizeChanged(ByVal sender As Object, ByVal e As Game3DPlay.Gui.PanelPart.SizeChangedEventArgs) Handles Me.SizeChanged
        Me.UpdateDimensions()
    End Sub
End Class

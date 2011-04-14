Public Class Label
    Inherits PanelPart

    Public Sub New(ByVal nParent As Panel)
        MyBase.New(nParent)
        Me.setText2D(New Text2D(Me))

    End Sub


    Private mText As String
    Public Property Text() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Text2D.Text
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As String)
            Me.Text2D.Text = value
        End Set
    End Property


    Private mTextAlign As DrawTextFormat
    Public Property TextAlign() As DrawTextFormat
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Text2D.TextAlign
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As DrawTextFormat)
            Me.Text2D.TextAlign = value
        End Set
    End Property



    Private mText2D As Text2D
    Public ReadOnly Property Text2D() As Text2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mText2d
        End Get
    End Property
    Private Sub setText2D(ByVal value As Text2D)
        Me.mText2d = value
    End Sub


    Private Sub Label_PositieChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.PanelPart.PositieChangedEventArgs) Handles Me.PositieChanged
        Me.Text2D.Positie = Me.Positie
    End Sub

    Private Sub Label_SizeChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.Size2DChangedEventArgs) Handles Me.SizeChanged
        Me.Text2D.Size = Me.Size
    End Sub
End Class

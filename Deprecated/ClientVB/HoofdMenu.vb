Public Class HoofdMenu
    Inherits Panel


    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.BuildMenu()



    End Sub

    Protected Overridable Sub BuildMenu()
        Me.Align = AlignType.MiddleCenter
        Dim M As New DesignHoofdMenu
        M.FillPanel(Me)

        'Me.Positie = New Vector2(50, 30)
        'Me.Size = New Vector2(Me.HoofdObj.Size.X - Me.Positie.X * 2, Me.HoofdObj.Size.Y - Me.Positie.Y * 2)
        'Me.Anchor = AnchorType.Left Or AnchorType.Right Or AnchorType.Top Or AnchorType.Bottom
        'Me.BackgroundColor = Color.BurlyWood

        Me.setKnopExit(M.knpQuit.GetKnop001)
        'Me.setKnopExit(New Knop001(Me))
        'With Me.KnopExit
        '    .Size = New Vector2(200, 50)
        '    .Positie = New Vector2(Me.Size.X / 2 - Me.KnopExit.Size.X / 2, 100)
        '    .Anchor = AnchorType.Top
        '    .Text = "Quit Game"
        'End With
    End Sub

    Private WithEvents ProcessElement As New ProcessEventElement(Me)


    Private WithEvents mKnopExit As Knop001
    Public ReadOnly Property KnopExit() As Knop001
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mKnopExit
        End Get
    End Property
    Protected Sub setKnopExit(ByVal value As Knop001)
        Me.mKnopExit = value
    End Sub

    Private Sub mKnopExit_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles mKnopExit.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.HoofdObj.StopSpel()
    End Sub


    Private Sub ProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessElement.Process
        With Me.HoofdObj.DevContainer.DX.Input
            'If .KeyDown(Key.Escape) Then
            '    Me.Enabled = False
            '    CLMain.Wereld.Enabled = True
            'End If
        End With
    End Sub
End Class

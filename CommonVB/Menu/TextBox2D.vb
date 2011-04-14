Public Class TextBox2D
    Inherits Panel
    'Implements IPositionable2D
    'Implements IClickable


    Public Sub New(ByVal nParent As Panel)
        MyBase.New(nParent)
        'Me.setRenderer(New Renderer2DContainer(Me))
        Me.setBackground(New Sprite2D(Me))
        Me.setTextObj(New Text2D(Me))
        Me.setProcessEventElement(New ProcessEventElement(Me))

        'Me.Renderer.Positie = Vector2.Empty
        Me.Background.Positie = Vector2.Empty
        Me.TextObj.Positie = Vector2.Empty

        Dim T As New Text2D(Me)
        T.Text = "|"
        T.Size = New Vector2(100, 200)
        T.Visible = False
        Me.setTextCursor(T)

        Me.setAllowedChars(CStr("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:\. _-").ToCharArray)


        'Me.AddEventHandler(Of ClickedEvent)(AddressOf Me.Clicked)

        Me.Size = New Vector2(100, 100)
        Me.TextCursorPos = 4

        Me.Blink()
    End Sub

    Private mScheduler As New SchedulerElement(Me)
    Public ReadOnly Property Scheduler() As SchedulerElement
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mScheduler
        End Get
    End Property
    Private Sub setScheduler(ByVal value As SchedulerElement)
        Me.mScheduler = value
    End Sub


    Private WithEvents mProcessEventElement As ProcessEventElement
    Public ReadOnly Property ProcessEventElement() As ProcessEventElement
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mProcessEventElement
        End Get
    End Property
    Private Sub setProcessEventElement(ByVal value As ProcessEventElement)
        Me.mProcessEventElement = value
    End Sub


    Private mTextCursorPos As Integer
    Public Property TextCursorPos() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTextCursorPos
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
            Me.mTextCursorPos = value

            Me.UpdateCursorPositie()
            Me.CheckCursorPos()
        End Set
    End Property

    Public Sub CheckCursorPos()
        If Me.TextCursorPos < 0 Then Me.TextCursorPos = 0
        If Me.TextCursorPos > Me.Text.Length Then Me.TextCursorPos = Me.Text.Length
    End Sub

    Public Sub UpdateCursorPositie()
        If Me.TextObj.Font Is Nothing Then Exit Sub
        Dim TopLeft As PointF = Me.TextObj.Font.GetPositionFromCharIndex(Me.TextCursorPos, New RectangleF(0, 0, Me.Size.X, Me.Size.Y), Me.Text, Me.TextObj.TextAlign)
        Me.TextCursor.Positie = New Vector2(TopLeft.X, TopLeft.Y - Me.TextObj.FontHeight)
    End Sub




    Private mTextCursor As Text2D
    Public ReadOnly Property TextCursor() As Text2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTextCursor
        End Get
    End Property
    Private Sub setTextCursor(ByVal value As Text2D)
        Me.mTextCursor = value
    End Sub





    Private mTextObj As Text2D
    Public ReadOnly Property TextObj() As Text2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTextObj
        End Get
    End Property
    Private Sub setTextObj(ByVal value As Text2D)
        Me.mTextObj = value
    End Sub


    Public Property Text() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.TextObj.Text
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As String)
            Me.TextObj.Text = value
            Me.OnTextChanged(Me, New TextChangedEventArgs)
        End Set
    End Property



    Public Class TextChangedEventArgs
        Inherits System.EventArgs
    End Class
    Public Sub OnTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
        RaiseEvent TextChanged(Me, e)
    End Sub
    Public Event TextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)


    Private mBackground As Sprite2D
    Public ReadOnly Property Background() As Sprite2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBackground
        End Get
    End Property
    Private Sub setBackground(ByVal value As Sprite2D)
        Me.mBackground = value
    End Sub


    Private mAllowedChars As Char()
    Public ReadOnly Property AllowedChars() As Char()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAllowedChars
        End Get
    End Property
    Private Sub setAllowedChars(ByVal value As Char())
        Me.mAllowedChars = value
    End Sub


    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As DeviceEventArgs)
        MyBase.OnDeviceReset(sender, e)
        Me.UpdateCursorPositie()
    End Sub

    Public Sub Blink()
        If Me.HasFocus = False Then Exit Sub
        Me.TextCursor.Visible = Not Me.TextCursor.Visible
        Me.Scheduler.ScheduleAction(AddressOf Blink, 700)
    End Sub

    Private Sub mProcessEventElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles mProcessEventElement.Process
        If Not Me.HasFocus Then Exit Sub 'TODO: mss handler verwijderen als geen focus?

        Dim C As Char = Me.HoofdObj.DevContainer.DX.Input.KeyChar
        If C <> Char.MinValue Then
            If Array.IndexOf(Of Char)(Me.AllowedChars, C) <> -1 Then
                Me.Text = Me.Text.Substring(0, Me.TextCursorPos) & C & Me.Text.Substring(Me.TextCursorPos, Me.Text.Length - Me.TextCursorPos)
                Me.TextCursorPos += 1
            ElseIf C = vbBack Then 'backspace
                If Me.TextCursorPos > 0 Then
                    Dim newText As String = Me.Text.Substring(0, Me.TextCursorPos - 1) & Me.Text.Substring(Me.TextCursorPos, Me.Text.Length - Me.TextCursorPos)
                    Me.TextCursorPos -= 1
                    Me.Text = newText

                End If

            End If
        End If
        With Me.HoofdObj.DevContainer.DX.Input
            If .KeyDown(DirectInput.Key.RightArrow) Then
                Me.TextCursorPos += 1
            ElseIf .KeyDown(DirectInput.Key.LeftArrow) Then
                Me.TextCursorPos -= 1
            ElseIf .KeyDown(DirectInput.Key.End) Then
                Me.TextCursorPos = Me.Text.Length
            ElseIf .KeyDown(DirectInput.Key.Home) Then
                Me.TextCursorPos = 0
            End If
        End With
    End Sub

    Private Sub TextBox2D_Focused(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Focused
        Me.TextCursor.Visible = False
        Me.Blink()

    End Sub

    Private Sub TextBox2D_FocusLost(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FocusLost
        Me.TextCursor.Visible = False

    End Sub

    'Private Sub TextBox2D_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles Me.Clicked
    '    If e.Button = DirectInput.MouseOffset.Button0 And e.State = ClickedEvent.MouseState.Down Then
    '        Me.Focus()
    '    End If
    'End Sub

    Private Sub TextBox2D_SizeChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.Size2DChangedEventArgs) Handles Me.SizeChanged
        Me.Background.Size = Me.Size
        Me.TextObj.Size = Me.Size
    End Sub

    Private Sub TextBox2D_TextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs) Handles Me.TextChanged
        Me.CheckCursorPos()
        Me.UpdateCursorPositie()
    End Sub
End Class

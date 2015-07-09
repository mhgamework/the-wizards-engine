Public Class TextList2D
    Inherits Panel

    Public Sub New(ByVal nParent As Panel)
        MyBase.New(nParent)
        Me.setLines(New Generic.List(Of String))
        Me.setText(New Text2D(Me))
        Me.Text.Positie = Vector2.Empty

        Me.Size = New Vector2(100, 100)
        Me.FontHeight = 20


    End Sub

    Public Overrides Sub Initialize()
        MyBase.Initialize()
        Me.UpdateText()
    End Sub

    Private mLines As Generic.List(Of String)
    Public ReadOnly Property Lines() As Generic.List(Of String)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mLines
        End Get
    End Property
    Private Sub setLines(ByVal value As Generic.List(Of String))
        Me.mLines = value
    End Sub


    Public ReadOnly Property Line(ByVal index As Integer) As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Lines(index)
        End Get
    End Property


    Private mText As Text2D
    Public ReadOnly Property Text() As Text2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mText
        End Get
    End Property
    Private Sub setText(ByVal value As Text2D)
        Me.mText = value
    End Sub



    Private mFontHeight As Single
    Public Property FontHeight() As Single
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Text.FontHeight
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
            Me.Text.FontHeight = value
        End Set
    End Property


    Public Sub WriteLine(ByVal nLine As String)
        Me.Lines.Add(nLine)
        If Me.Text.Font IsNot Nothing Then Me.UpdateText()
    End Sub

    Public Sub WriteLine(ByVal nLine As String, ByVal ParamArray Args As Object())
        Me.Lines.Add(String.Format(nLine, Args))
        If Me.Text.Font IsNot Nothing Then Me.UpdateText()
    End Sub

    Public Sub UpdateText()
        Dim VisibleLines As Integer = CInt(System.Math.Floor(Me.Size.Y / Me.FontHeight))

        Dim DrawLines As New Generic.List(Of String) 'String() = New String(VisibleLines - 1) {}



        Dim LoopStart As Integer = Me.Lines.Count - VisibleLines
        If LoopStart < 0 Then LoopStart = 0

        Dim LoopEnd As Integer = LoopStart + VisibleLines
        If LoopEnd > Me.Lines.Count - 1 Then LoopEnd = Me.Lines.Count - 1


        Dim TargetRect As Drawing.RectangleF
        Dim StartCharIndex As Integer
        Dim EndCharIndex As Integer

        For I As Integer = LoopStart To LoopEnd
            'TargetRect = New Drawing.RectangleF(Me.Positie.X, 0, Me.mSize.X, Me.FontHeight)
            TargetRect = New Drawing.RectangleF(Me.Positie.X, 0, Me.Size.X, Me.FontHeight)

            StartCharIndex = 0
            EndCharIndex = 0
            Do
                If Me.Line(I).Chars(StartCharIndex) = vbCr Then
                    StartCharIndex += 1
                End If
                If Me.Line(I).Chars(StartCharIndex) = vbLf Then
                    StartCharIndex += 1
                End If
                EndCharIndex = Me.Text.Font.GetLineEndCharIndex(StartCharIndex, TargetRect, Me.Line(I))

                If StartCharIndex > EndCharIndex Then

                    If StartCharIndex - 1 = Me.Line(I).Length - 1 Then
                        DrawLines.Add(Me.Line(I).Substring(EndCharIndex, StartCharIndex - EndCharIndex))
                        Exit Do
                    Else
                        StartCharIndex = StartCharIndex + 1
                    End If

                Else
                    DrawLines.Add(Me.Line(I).Substring(StartCharIndex, EndCharIndex - StartCharIndex))
                End If
                If EndCharIndex >= Me.Line(I).Length - 1 Then
                    Exit Do
                Else
                    StartCharIndex = EndCharIndex + 1
                End If

            Loop
            'Me.font.Write(Nothing, _
            '    New Drawing.RectangleF(Me.Positie.X, Me.Positie.Y + Me.FontHeight * (I - LoopStart), _
            '                            Me.mSize.X, Me.FontHeight), DrawLines(I), _
            '    DrawTextFormat.Left Or DrawTextFormat.VerticalCenter, Drawing.Color.Red.ToArgb)


        Next I

        'Dim IDrawLine As Integer = DrawLines.Length - 1
        'Dim ILine As Integer = Me.lines.Count - 1
        'Dim CurLine As String

        'Do
        '    Me.font.MeasureString()
        '    CurLine = Me.Line(ILine)

        '    Me.font.GetLineStartCharIndex ( curline.Length -1,
        '    Exit Do
        'Loop



        LoopStart = DrawLines.Count - VisibleLines
        If LoopStart < 0 Then LoopStart = 0

        LoopEnd = LoopStart + VisibleLines
        If LoopEnd > DrawLines.Count - 1 Then LoopEnd = DrawLines.Count - 1

        Me.Text.Text = ""
        If LoopEnd >= LoopStart Then
            Me.Text.Text = DrawLines(LoopStart)
        End If
        For I As Integer = LoopStart + 1 To LoopEnd
            Me.Text.Text += vbCrLf & DrawLines(I)

            'Me.font.Write(Nothing, _
            '    New Drawing.RectangleF(Me.Positie.X, Me.Positie.Y + Me.FontHeight * (I - LoopStart), _
            '                            Me.mSize.X, Me.FontHeight), DrawLines(I), _
            '    DrawTextFormat.Left Or DrawTextFormat.VerticalCenter, Drawing.Color.Red.ToArgb)

        Next I



        'Dim VisibleLines As Integer = Math.Floor(Me.Size.Y / Me.FontHeight)




        'Dim D




        'Dim LoopStart As Integer = Me.lines.Count - VisibleLines
        'If LoopStart < 0 Then LoopStart = 0

        'Dim LoopEnd As Integer = LoopStart + VisibleLines
        'If LoopEnd > Me.lines.Count - 1 Then LoopEnd = Me.lines.Count - 1
        'For I As Integer = LoopStart To LoopEnd


        '    Me.font.Write(Nothing, _
        '        New Drawing.RectangleF(Me.Positie.X, Me.Positie.Y + Me.FontHeight * (I - LoopStart), _
        '                                Me.mSize.X, Me.FontHeight), Me.Line(I), _
        '        DrawTextFormat.Left Or DrawTextFormat.VerticalCenter, Drawing.Color.Red.ToArgb)

        'Next I

    End Sub



    Private Sub TextList2D_SizeChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.Size2DChangedEventArgs) Handles Me.SizeChanged
        Me.Text.Size = Me.Size
    End Sub
End Class




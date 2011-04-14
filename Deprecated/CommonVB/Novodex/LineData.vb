Public Class LineData
    Public Sub New()
        Me.str = Nothing
        Me.offset = 0
    End Sub
    Public Function beginsWith(ByVal prefix As String) As Boolean
        Return Me.beginsWith(prefix, False)
    End Function

    Public Function beginsWith(ByVal prefix As String, ByVal caseSensitive As Boolean) As Boolean
        Dim i As Integer = Me.offset
        Do While (i < Me.str.Length)
            If Not Char.IsWhiteSpace(Me.str.Chars(i)) Then
                If ((i + prefix.Length) >= Me.str.Length) Then
                    Return False
                End If
                Dim j As Integer = 0
                Do While (j < prefix.Length)
                    Dim c As Char = Me.str.Chars((i + j))
                    Dim ch2 As Char = prefix.Chars(j)
                    If Not caseSensitive Then
                        c = Char.ToLower(c)
                        ch2 = Char.ToLower(ch2)
                    End If
                    If (c <> ch2) Then
                        Return False
                    End If
                    j += 1
                Loop
                Return True
            End If
            i += 1
        Loop
        Return False
    End Function

    Private Function parseFloat(ByVal index As Integer) As Single
        Me.offset = index
        Dim num As Single = 1.0!
        If (Me.str.Chars(Me.offset) = "-"c) Then
            num = -1.0!
            Me.offset += 1
        End If
        Dim num2 As Single = 0.0!
        Dim num3 As Single = 0.0!
        Do While (Me.offset < Me.str.Length)
            Dim ch As Char = Me.str.Chars(Me.offset)
            If ((ch < "0"c) OrElse (ch > "9"c)) Then
                Exit Do
            End If
            'TODO: num2 = ((num2 * 10.0!) + (ch - "0"c))
            num2 = ((num2 * 10.0!) + Integer.Parse(ch.ToString))
            Me.offset += 1
        Loop
        If (Me.str.Chars(Me.offset) = "."c) Then
            Me.offset += 1
            Dim num4 As Single = 0.1!
            Do While (Me.offset < Me.str.Length)
                Dim ch2 As Char = Me.str.Chars(Me.offset)
                If ((ch2 < "0"c) OrElse (ch2 > "9"c)) Then
                    Exit Do
                End If
                'TODO: num3 = (num3 + ((ch2 - "0"c) * num4))
                num3 = (num3 + (Integer.Parse(ch2.ToString) * num4))
                num4 = (num4 / 10.0!)
                Me.offset += 1
            Loop
        End If
        Return ((num2 + num3) * num)
    End Function
    Private Function parseInt(ByVal index As Integer) As Integer
        Me.offset = index
        Dim num As Integer = 1
        If (Me.str.Chars(Me.offset) = "-"c) Then
            num = -1
            Me.offset += 1
        End If
        Dim num2 As Integer = 0
        Do While (Me.offset < Me.str.Length)
            Dim ch As Char = Me.str.Chars(Me.offset)
            If ((ch < "0"c) OrElse (ch > "9"c)) Then
                Exit Do
            End If
            'TODO: num2 = ((num2 * 10) + (ch - "0"c))
            num2 = ((num2 * 10) + Integer.Parse(ch.ToString))
            Me.offset += 1
        Loop

        Return (num2 * num)
    End Function

    Public Function readFloat() As Single
        Dim i As Integer = Me.offset
        Do While (i < Me.str.Length)
            Dim c As Char = Me.str.Chars(i)
            If (Char.IsNumber(c) OrElse (c = "-"c)) Then
                Return Me.parseFloat(i)
            End If
            i += 1
        Loop
        Return 0.0!
    End Function

    Public Function readInt() As Integer
        Dim i As Integer = Me.offset
        Do While (i < Me.str.Length)
            Dim c As Char = Me.str.Chars(i)
            If (Char.IsNumber(c) OrElse (c = "-"c)) Then
                Return Me.parseInt(i)
            End If
            i += 1
        Loop
        Return 0
    End Function


    Public Sub setString(ByVal str As String)
        Me.str = str
        Me.offset = 0
    End Sub

    Public Function readVector3() As Vector3
        Return New Vector3(readFloat(), readFloat(), readFloat())
    End Function

    Public offset As Integer


    Public str As String




























End Class

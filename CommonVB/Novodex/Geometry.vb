Imports System.IO
Public Class Geometry
    ' Methods
    Public Sub New()
        Me.vertexArray = Nothing
        Me.triangleIndiceArray = Nothing
        Me.initedFlag = False
    End Sub

    Public Sub New(ByVal fileName As String)
        Me.vertexArray = Nothing
        Me.triangleIndiceArray = Nothing
        Me.initedFlag = False
        Me.initedFlag = Me.loadFromFile(fileName)
    End Sub

    Public Function loadFromFile(ByVal fileName As String) As Boolean
        Try
            Dim data As New LineData
            Dim stream As Stream = New FileStream(fileName, FileMode.Open, FileAccess.Read)
            Dim reader As New StreamReader(stream)
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim index As Integer = 0
            Dim num4 As Integer = 0
            Do While (reader.Peek <> -1)
                data.setString(reader.ReadLine)
                If data.beginsWith("NumVertices") Then
                    num = data.readInt
                    Me.vertexArray = New Vector3(num - 1) {}
                Else
                    If data.beginsWith("NumTris") Then
                        num2 = data.readInt
                        Me.triangleIndiceArray = New Integer((num2 * 3) - 1) {}
                        Continue Do
                    End If
                    If data.beginsWith("V:") Then
                        Me.vertexArray(index) = data.readVector3
                        index += 1
                        Continue Do
                    End If
                    If data.beginsWith("TI:") Then
                        Me.triangleIndiceArray((num4 * 3)) = data.readInt
                        Me.triangleIndiceArray(((num4 * 3) + 1)) = data.readInt
                        Me.triangleIndiceArray(((num4 * 3) + 2)) = data.readInt
                        num4 += 1
                    End If
                End If
            Loop
        Catch obj1 As Exception
            'TODO: SimpleTutorial.printLine(("Trouble parsing " & fileName))
            Return False
        End Try
        Return True
    End Function


    ' Properties
    Public ReadOnly Property NumTris() As Integer
        Get
            If (Me.triangleIndiceArray Is Nothing) Then
                Return 0
            End If
            Return CInt((Me.triangleIndiceArray.Length / 3))
        End Get
    End Property

    Public ReadOnly Property NumVerts() As Integer
        Get
            If (Me.vertexArray Is Nothing) Then
                Return 0
            End If
            Return Me.vertexArray.Length
        End Get
    End Property


    ' Fields
    Public initedFlag As Boolean
    Public triangleIndiceArray As Integer()
    Public vertexArray As Vector3()
End Class




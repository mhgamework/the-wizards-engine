Public Class CustomEntityReport
    Inherits NxUserEntityReport
    ' Methods
    Public Overrides Function onEvent(ByVal shapeArray As NxShape()) As Boolean
        Dim i As Integer = 0
        Do While (i < shapeArray.Length)
            'TODO: SimpleTutorial.printLine(String.Concat(New Object() { " onEvent-", Me.counter++, ": ", shapeArray(i).getShapeType }))
            i += 1
        Loop
        Return True
    End Function

    Public Sub resetCounter()
        Me.counter = 0
    End Sub


    ' Fields
    Private counter As Integer = 0
End Class


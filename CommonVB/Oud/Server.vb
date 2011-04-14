Public Class Server
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject, ByVal IP As String, ByVal Port As Integer)
        MyBase.New(nParent)
    End Sub

    Private mEndPoint As Net.IPEndPoint
    Public ReadOnly Property EndPoint() As Net.IPEndPoint
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEndPoint
        End Get
    End Property
    Private Sub setEndPoint(ByVal value As Net.IPEndPoint)
        Me.mEndPoint = value
    End Sub


End Class

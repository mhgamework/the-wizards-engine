Public Class Mineral

    Private mType As MineralType
    Public ReadOnly Property Type() As MineralType
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mType
        End Get
    End Property
    Private Sub setType(ByVal value As MineralType)
        Me.mType = value
    End Sub



    Private mAmount As Single
    Public Property Amount() As Single
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAmount
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
            Me.mAmount = value
        End Set
    End Property

    Public Sub AddAmount(ByVal nAmount As Single)
        Me.Amount += nAmount
    End Sub

    Public Sub RemoveAmount(ByVal nAmount As Single)
        Me.Amount += nAmount
    End Sub


    Public Function Split(ByVal nAmount As Single) As Mineral
        If nAmount >= Me.Amount Then
            Throw New Exception("Kan deze Mineral niet splitsen want hij bevat minder dan de gegeven nAmount.")
            Exit Function
        End If
        Dim M As New Mineral(Me.Type)
        Me.RemoveAmount(nAmount)
        M.AddAmount(nAmount)
        Return M
    End Function

    Public Sub New(ByVal nType As MineralType)
        Me.setType(nType)

    End Sub



End Class

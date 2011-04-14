Public Class VeranderingEventArgs
    Inherits System.EventArgs
    Public Sub New(ByVal nEnt As Entity, ByVal nType As Integer, ByVal nData As Byte())
        Me.setEntity(nEnt)
        Me.setType(nType)
        Me.setData(nData)
    End Sub



    Private mEntity As Entity
    Public ReadOnly Property Entity() As Entity
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntity
        End Get
    End Property
    Private Sub setEntity(ByVal value As Entity)
        Me.mEntity = value
    End Sub





    Private mType As Integer
    Public ReadOnly Property Type() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mType
        End Get
    End Property
    Private Sub setType(ByVal value As Integer)
        Me.mType = value
    End Sub


    Private mData As Byte()
    Public ReadOnly Property Data() As Byte()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mData
        End Get
    End Property
    Private Sub setData(ByVal value As Byte())
        Me.mData = value
    End Sub


End Class

Public Enum VeranderingType As Byte
    Positie = 1
    Snelheid = 2
    Rotatie = 3
    RotatieSnelheid = 4

End Enum
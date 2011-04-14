Public Class VeranderingEvent
    Inherits SpelEvent

    Public Sub New(ByVal nEnt As Entity, ByVal nVerandering As VeranderingType, ByVal nNieuweData As Byte())
        MyBase.New()
        Me.setEnt(nEnt)
        Me.setCanCallSpelEventHandlers(False)
        Me.Direction = EventDirection.Up

        Me.setVerandering(nVerandering)
        Me.setNieuweData(nNieuweData)

    End Sub


    Private mEnt As Entity
    Public ReadOnly Property Ent() As Entity
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEnt
        End Get
    End Property
    Private Sub setEnt(ByVal value As Entity)
        Me.mEnt = value
    End Sub


    Private mVerandering As VeranderingType
    Public ReadOnly Property Verandering() As VeranderingType
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mVerandering
        End Get
    End Property
    Private Sub setVerandering(ByVal value As VeranderingType)
        Me.mVerandering = value
    End Sub


    Private mNieuweData As Byte()
    Public ReadOnly Property NieuweData() As Byte()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNieuweData
        End Get
    End Property
    Private Sub setNieuweData(ByVal value As Byte())
        Me.mNieuweData = value
    End Sub


    Public Enum VeranderingType As Byte
        Positie = 1

    End Enum

End Class

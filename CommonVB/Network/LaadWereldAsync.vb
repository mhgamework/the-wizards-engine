Public Class LaadWereldCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs
    Public Sub New(ByVal nError As Exception, ByVal cancelled As Boolean, ByVal userState As Object)
        MyBase.New(nError, cancelled, userState)
    End Sub
End Class
Public Delegate Sub LaadWereldCompletedEventHandler(ByVal sender As Object, ByVal e As LaadWereldCompletedEventArgs)

Public Delegate Sub LaadWereldProgressChangedEventHandler(ByVal sender As Object, ByVal e As LaadWereldProgressChangedEventArgs)
Public Class LaadWereldProgressChangedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs
    Public Sub New(ByVal nError As Exception, ByVal cancelled As Boolean, ByVal userState As Object, ByVal nEntID As Integer, ByVal nVersie As Integer)
        MyBase.New(nError, cancelled, userState)
        Me.setEntityID(nEntID)
        'Me.setEntityType(nType)
        Me.setVersie(nVersie)
    End Sub


    Private mEntityID As Integer
    Public ReadOnly Property EntityID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntityID
        End Get
    End Property
    Private Sub setEntityID(ByVal value As Integer)
        Me.mEntityID = value
    End Sub


    'Private mEntityType As EntityType
    'Public ReadOnly Property EntityType() As EntityType
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mEntityType
    '    End Get
    'End Property
    'Private Sub setEntityType(ByVal value As EntityType)
    '    Me.mEntityType = value
    'End Sub


    Private mVersie As Integer
    Public ReadOnly Property Versie() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mVersie
        End Get
    End Property
    Private Sub setVersie(ByVal value As Integer)
        Me.mVersie = value
    End Sub




End Class
Public Class ClientUpdaterCommunicater
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        Me.setComm(New ClientUpdaterCommunication)
    End Sub

    Private WithEvents mProcessElement As New ProcessEventElement(Me)

    Private mComm As ClientUpdaterCommunication
    Public ReadOnly Property Comm() As ClientUpdaterCommunication
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mComm
        End Get
    End Property
    Private Sub setComm(ByVal value As ClientUpdaterCommunication)
        Me.mComm = value
    End Sub

    Public Overrides Sub Dispose()
        MyBase.Dispose()
        Me.Comm.Dispose()

    End Sub

    Private Sub mProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles mProcessElement.Process
        Me.Comm.Invoker.ProcessScheduledActions()
    End Sub
End Class

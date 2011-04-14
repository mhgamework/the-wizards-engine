Public Class NetworkElement
    Inherits Element

    Public Sub New(ByVal nParent As Entity)
        MyBase.New(nParent)
        Me.setParentEntity(nParent)
    End Sub


    Private mParentEntity As Entity
    Public ReadOnly Property ParentEntity() As Entity
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mParentEntity
        End Get
    End Property
    Private Sub setParentEntity(ByVal value As Entity)
        Me.mParentEntity = value
    End Sub

    Public Overridable Sub OnProcess(ByVal sender As Object, ByVal e As ProcessElement.ProcessEventArgs)

    End Sub





    Public Overridable Sub OnPositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)

    End Sub
    Public Overridable Sub OnSnelheidVeranderd(ByVal sender As Object, ByVal e As SnelheidVeranderdEventArgs)

    End Sub

    Public Overridable Sub OnRotatieVeranderd()

    End Sub



End Class

Public Class BouncingBall001
    Inherits Bol001

    Public Sub New(ByVal nParent As OctTree)
        MyBase.New(nParent)


    End Sub

    Private WithEvents mProcessElement As New ProcessEventElement(Me)

    'Private mBol As Bol001
    'Public ReadOnly Property Bol() As Bol001
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mBol
    '    End Get
    'End Property
    'Private Sub setBol(ByVal value As Bol001)
    '    Me.mBol = value
    'End Sub


    Private Sub mProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles mProcessElement.Process
        If Me.functions.Positie.Y < 1 And Math.Abs(Me.functions.Snelheid.Y) < 0.001 Then
            Me.functions.Actor.addForce(New Vector3(0, 2000 * e.Elapsed, 0), NxForceMode.NX_IMPULSE)
        End If
    End Sub
End Class

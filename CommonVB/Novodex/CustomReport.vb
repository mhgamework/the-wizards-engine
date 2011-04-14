Public Class CustomReport
    Implements NxUserContactReport, NxUserTriggerReport, NxUserNotify


    Private mBaseMain As BaseMain
    Public ReadOnly Property BaseMain() As BaseMain
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBaseMain
        End Get
    End Property
    Private Sub setBaseMain(ByVal value As BaseMain)
        Me.mBaseMain = value
    End Sub

    Public Sub New(ByVal nBaseMain As BaseMain)
        Me.setBaseMain(nBaseMain)
    End Sub

    Public Sub onContactNotify(ByVal contactPair As NxContactPair, ByVal events As UInt32) Implements NxUserContactReport.onContactNotify
        Dim num As Integer = 0
        Dim iterator As New NxContactStreamIterator(contactPair.streamPtr)
        Do While iterator.goNextPair
            Do While iterator.goNextPatch
                Do While iterator.goNextPoint
                    num += 1
                Loop
            Loop
        Loop
        Dim text As String = ""
        If ((events Or 2) = 2) Then
            [text] = "startTouch"
        ElseIf ((events Or 4) = 4) Then
            [text] = "endTouch"
        End If
        Me.BaseMain.WriteLine(String.Concat(New Object() {contactPair.Actor0.Name, " contact ", contactPair.Actor1.Name, " numPoints=", num, " ", [text]}))
    End Sub

    Public Function onJointBreak(ByVal breakingForce As Single, ByVal brokenJoint As NxJoint) As Boolean Implements NxUserNotify.onJointBreak
        Me.BaseMain.WriteLine("")
        Me.BaseMain.WriteLine(String.Concat(New Object() {"onJointBreak:", brokenJoint.Name, " breakingForce=", breakingForce}))
        Me.BaseMain.WriteLine("")
        Return True
    End Function

    Public Sub onTrigger(ByVal triggerShape As NxShape, ByVal otherShape As NxShape, ByVal status As NxShapeFlag) Implements NxUserTriggerReport.onTrigger
        If (status = NxShapeFlag.NX_TRIGGER_ON_ENTER) Then
            Me.BaseMain.WriteLine((otherShape.getActor.Name & " ON_ENTER " & triggerShape.getActor.Name))
        ElseIf (status = NxShapeFlag.NX_TRIGGER_ON_LEAVE) Then
            Me.BaseMain.WriteLine((otherShape.getActor.Name & " ON_LEAVE " & triggerShape.getActor.Name))
        End If
    End Sub

End Class




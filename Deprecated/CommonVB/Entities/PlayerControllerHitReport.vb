Public Class PlayerControllerHitReport
    Inherits NxUserControllerHitReport
    ' Methods
    Public Overrides Function onControllerHit(ByVal hit As NxControllersHit) As NxControllerAction
        Return NxControllerAction.NX_ACTION_NONE
    End Function

    Public Overrides Function onShapeHit(ByVal hit As NxControllerShapeHit) As NxControllerAction
        If hit.Shape Is Nothing Then Exit Function 'Vreemde edit in the wrapper
        Dim actor As NxActor = hit.Shape.getActor
        If actor Is Nothing Then Exit Function
        If (((hit.dir.Y = 0.0!) AndAlso actor.isDynamic) AndAlso Not actor.FlagKinematic) Then
            Dim num As Single = ((actor.Mass * hit.length) * 0.4!)
            actor.addForceAtLocalPos(DirectCast((hit.dir * num), Vector3), New Vector3(0.0!, 0.0!, 0.0!), NxForceMode.NX_IMPULSE)
            actor.addForceAtPos(DirectCast((hit.dir * num), Vector3), hit.Controller.getPosition, NxForceMode.NX_IMPULSE)
            actor.addForceAtPos(DirectCast((hit.dir * num), Vector3), hit.WorldPos, NxForceMode.NX_IMPULSE)
        End If
        Return NxControllerAction.NX_ACTION_NONE
    End Function

End Class
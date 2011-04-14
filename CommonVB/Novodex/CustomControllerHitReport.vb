Public Class CustomControllerHitReport
    Inherits NxUserControllerHitReport
    ' Methods
    Public Overrides Function onControllerHit(ByVal hit As NxControllersHit) As NxControllerAction
        Return NxControllerAction.NX_ACTION_NONE
    End Function

    Public Overrides Function onShapeHit(ByVal hit As NxControllerShapeHit) As NxControllerAction
        Dim actor As NxActor = hit.Shape.getActor
        If (((hit.dir.Y = 0.0!) AndAlso actor.isDynamic) AndAlso Not actor.FlagKinematic) Then
            Dim num As Single = ((actor.Mass * hit.length) * 0.4!)
            actor.addForceAtLocalPos(DirectCast((hit.dir * num), Vector3), New Vector3(0.0!, 0.0!, 0.0!), NxForceMode.NX_IMPULSE)
            actor.addForceAtPos(DirectCast((hit.dir * num), Vector3), hit.Controller.getPosition, NxForceMode.NX_IMPULSE)
            actor.addForceAtPos(DirectCast((hit.dir * num), Vector3), hit.WorldPos, NxForceMode.NX_IMPULSE)
        End If
        Return NxControllerAction.NX_ACTION_NONE
    End Function

End Class





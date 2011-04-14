Public Class CustomRaycastReport
    Inherits NxUserRaycastReport

    ''The counter is just for clarity in the output

    Private counter As Integer = 0

    Public Sub resetCounter()
        counter = 0
    End Sub



    'Return true to continue raycasting to other shapes
    'Return false to end raycasting
    Public Overrides Function onHit(ByRef raycastHit As NxRaycastHit) As Boolean

        Dim hitShape As NxShape = raycastHit.Shape
        Dim distance As Single = raycastHit.distance
        Dim wI As Vector3 = raycastHit.worldImpact
        'TODO: SimpleTutorial.printLine(String.Format("onHit-{0}: {1} distance={2:F3} worldImpact=({3:F3},{4:F3},{5:F3})",(counter++),hitShape.getActor().Name,distance,wI.X,wI.Y,wI.Z));
        Return True
    End Function

End Class

Public Class ClientEntityFunctions
    Inherits EntityFunctions

    Public Sub New(ByVal nParent As Entity)
        MyBase.New(nParent)

    End Sub

    Public Overridable Function AddEntityDeltaSnapshot(ByVal nTick As Integer, ByVal nEDS As EntityDeltaSnapshot) As Boolean

    End Function


End Class

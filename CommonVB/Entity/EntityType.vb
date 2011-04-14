Public Enum EntityType As Integer
    Undefined = 0
    Bol001
    Player
    Lamp001
    StaticEntity
    Box001
    GroundPlane
    BouncingBall001

End Enum
'Public Module EntityCreater
'    Public Function Create(ByVal nType As EntityType, ByVal nParent As OctTree) As Entity
'        Select Case nType
'            Case EntityType.Bol001
'                Return New Bol001(nParent)
'            Case EntityType.Player
'                Return New Player(nParent)
'            Case EntityType.Lamp001
'                Stop
'                'Return New Lamp001(nParent)
'            Case EntityType.StaticEntity
'                Return New StaticEntity(nParent)
'            Case EntityType.Box001
'                Stop
'                'Return New Box001(nParent)
'            Case EntityType.GroundPlane
'                Return New GroundPlane(nParent)
'            Case EntityType.BouncingBall001
'                Return New BouncingBall001(nParent)
'            Case Else
'                Stop
'        End Select
'        Throw New Exception
'    End Function
'End Module
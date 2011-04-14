Public Class SimpleWheel
    ' Methods
    Public Sub New(ByVal parentActor As NxActor, ByVal wheelRadius As Single, ByVal maxSuspensionTravel As Single, ByVal localPos As Vector3, ByVal springValue As Single, ByVal damperValue As Single, ByVal springTargetValue As Single)
        Dim inverseWheelMass As Single = 1.0!
        Dim shapeDesc As New NxWheelShapeDesc(wheelRadius, maxSuspensionTravel, inverseWheelMass, 0, 0.0!, 0.0!, 0.0!, Matrix.Translation(localPos))
        shapeDesc.setSuspension(New NxSpringDesc(springValue, damperValue, springTargetValue))
        Me.parentActor = parentActor
        Me.wheelShape = DirectCast(parentActor.createShape(shapeDesc), NxWheelShape)
        Dim lateralTireForceFunction As NxTireFunctionDesc = Me.wheelShape.LateralTireForceFunction
        lateralTireForceFunction.stiffnessFactor = 200000.0!
        Me.wheelShape.LateralTireForceFunction = lateralTireForceFunction
    End Sub

    Public Sub setBrakeTorque(ByVal brakeTorque As Single)
        Me.wheelShape.setBrakeTorque(brakeTorque)
    End Sub

    Public Sub setDriveTorque(ByVal driveTorque As Single)
        Me.wheelShape.setMotorTorque(driveTorque)
    End Sub

    Public Sub setProperties(ByVal wheelRadius As Single, ByVal wheelHeightPosition As Single, ByVal maxSuspensionTravel As Single, ByVal springValue As Single, ByVal damperValue As Single, ByVal springTargetValue As Single)
        Dim localPosition As Vector3 = Me.wheelShape.LocalPosition
        localPosition.Y = wheelHeightPosition
        Me.wheelShape.LocalPosition = localPosition
        Me.wheelShape.Radius = wheelRadius
        Me.wheelShape.SuspensionTravel = maxSuspensionTravel
        Me.wheelShape.setSuspension(New NxSpringDesc(springValue, damperValue, springTargetValue))
    End Sub

    Public Sub setSteerAngle(ByVal steerAngle As Single)
        Me.wheelShape.setSteerAngle((steerAngle * NovodexUtil.DEG_TO_RAD))
    End Sub


    ' Fields
    Public driveFlag As Boolean = False
    Public parentActor As NxActor = Nothing
    Public steerFlag As Boolean = False
    Public wheelShape As NxWheelShape = Nothing
End Class




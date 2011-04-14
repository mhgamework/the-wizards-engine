Public Class SimpleCar
    ' Methods
    Private Sub New()
    End Sub

    Public Sub applyBrake(ByVal brakeRatioInput As Single)
        Me.brakeRatioInput = brakeRatioInput
    End Sub

    Public Sub applySteering(ByVal steerRatioInput As Single)
        Me.steerRatioInput = steerRatioInput
    End Sub

    Public Sub applyThrottle(ByVal throttleRatioInput As Single)
        Me.throttleRatioInput = throttleRatioInput
    End Sub

    Public Shared Function createCar(ByVal scene As NxScene, ByVal length As Single, ByVal globalPose As Matrix) As SimpleCar
        Dim size As New Vector3((length / 2.0!), (length / 3.0!), length)
        Dim totalMass As Single = CSng((Math.Pow(CDbl(length), 3) * 5))
        Return SimpleCar.createCar(scene, size, totalMass, globalPose)
    End Function

    Public Shared Function createCar(ByVal scene As NxScene, ByVal size As Vector3, ByVal totalMass As Single, ByVal globalPose As Matrix) As SimpleCar
        Dim car As New SimpleCar
        car.createChasis(scene, size, totalMass)
        car.chasisActor.GlobalPose = globalPose
        car.createWheels((size.Y / 4.0!), (size.Y / 8.0!), (size.Y / 4.0!), (totalMass * 5.0!), (totalMass / 5.0!), 0.0!)
        car.setSteerProperties(30.0!, True, False)
        car.setDriveProperties((totalMass / 10.0!), (totalMass / 3.0!), True, False)
        Return car
    End Function

    Private Sub createChasis(ByVal scene As NxScene, ByVal size As Vector3, ByVal totalMass As Single)
        Dim valueY As Single = (size.Y / 4.0!)
        Dim num2 As Single = (size.Y * 0.75!)
        Dim valueX As Single = (size.X / 2.0!)
        Dim num4 As Single = (valueX * 0.8!)
        Dim valueZ As Single = (size.Z * 0.25!)
        Dim num6 As Single = (-size.Z * 0.3!)
        Dim num7 As Single = (valueZ * 0.4!)
        Dim num8 As Single = (num6 * 0.8!)
        Me.totalMass = totalMass
        Me.chasisSize = size
        Me.chasisActor = NovodexUtil.createBoxActor(scene, New Vector3(size.X, (size.Y / 2.0!), size.Z), Matrix.Identity, 1.0!)
        Dim localSpaceVertArray As Vector3() = New Vector3(8 - 1) {}
        localSpaceVertArray(0) = New Vector3(valueX, valueY, valueZ)
        localSpaceVertArray(1) = New Vector3(valueX, valueY, num6)
        localSpaceVertArray(2) = New Vector3(num4, num2, num8)
        localSpaceVertArray(3) = New Vector3(num4, num2, num7)
        Dim i As Integer = 0
        Do While (i < 4)
            localSpaceVertArray((4 + i)) = localSpaceVertArray(i)
            localSpaceVertArray((4 + i)).X = (localSpaceVertArray((4 + i)).X * -1.0!)
            i += 1
        Loop
        Me.chasisActor.createShape(NovodexUtil.createConvexShapeDescFromVertexCloud(localSpaceVertArray))
        Me.chasisActor.updateMassFromShapes(0.0!, totalMass)
        Me.chasisActor.setCMassOffsetLocalPosition(New Vector3(0.0!, (-size.Y / 10.0!), 0.0!))
        Me.chasisActor.Name = "CarChasis"
    End Sub

    Private Sub createWheels(ByVal wheelRadius As Single, ByVal wheelHeightOffset As Single, ByVal maxSuspensionTravel As Single, ByVal springValue As Single, ByVal damperValue As Single, ByVal springTargetValue As Single)
        Dim valueZ As Single = ((Me.chasisSize.Z / 2.0!) * 0.8!)
        Dim num2 As Single = (-(Me.chasisSize.Z / 2.0!) * 0.8!)
        Dim valueX As Single = (-(Me.chasisSize.X / 2.0!) * 0.9!)
        Dim num4 As Single = ((Me.chasisSize.X / 2.0!) * 0.9!)
        Dim valueY As Single = ((wheelRadius - (Me.chasisSize.Y / 2.0!)) + wheelHeightOffset)
        Me.wheelRadius = wheelRadius
        Me.maxSuspensionTravel = maxSuspensionTravel
        Me.maxDriveTorque = Me.maxDriveTorque
        Me.maxBrakeTorque = Me.maxBrakeTorque
        Me.suspensionSpringForce = springValue
        Me.suspensinoDamperValue = damperValue
        Me.suspensionSpringTargetValue = springTargetValue
        Me.frontLeftWheel = New SimpleWheel(Me.chasisActor, wheelRadius, maxSuspensionTravel, New Vector3(valueX, valueY, valueZ), springValue, damperValue, springTargetValue)
        Me.frontRightWheel = New SimpleWheel(Me.chasisActor, wheelRadius, maxSuspensionTravel, New Vector3(num4, valueY, valueZ), springValue, damperValue, springTargetValue)
        Me.rearLeftWheel = New SimpleWheel(Me.chasisActor, wheelRadius, maxSuspensionTravel, New Vector3(valueX, valueY, num2), springValue, damperValue, springTargetValue)
        Me.rearRightWheel = New SimpleWheel(Me.chasisActor, wheelRadius, maxSuspensionTravel, New Vector3(num4, valueY, num2), springValue, damperValue, springTargetValue)
    End Sub

    Public Function getMass() As Single
        Return Me.chasisActor.Mass
    End Function

    Public Sub setBrake(ByVal brakeRatio As Single)
        Me.brakeRatio = brakeRatio
        Me.brakeRatioInput = 0.0000001!
    End Sub

    Public Sub setDriveProperties(ByVal maxDriveTorque As Single, ByVal maxBrakeTorque As Single, ByVal frontWheelsDriveFlag As Boolean, ByVal rearWheelsDriveFlag As Boolean)
        Me.maxDriveTorque = maxDriveTorque
        Me.maxBrakeTorque = maxBrakeTorque
        Me.frontWheelsDriveFlag = frontWheelsDriveFlag
        Me.rearWheelsDriveFlag = rearWheelsDriveFlag
        Me.frontLeftWheel.driveFlag = frontWheelsDriveFlag
        Me.frontRightWheel.driveFlag = frontWheelsDriveFlag
        Me.rearLeftWheel.driveFlag = rearWheelsDriveFlag
        Me.rearRightWheel.driveFlag = rearWheelsDriveFlag
    End Sub

    Public Sub setSteering(ByVal steerRatio As Single)
        Me.steerRatio = steerRatio
        Me.steerRatioInput = 0.0000001!
    End Sub

    Public Sub setSteerProperties(ByVal maxSteerAngle As Single, ByVal frontWheelsSteerFlag As Boolean, ByVal rearWheelsSteerFlag As Boolean)
        Me.maxSteerAngle = maxSteerAngle
        Me.frontWheelsSteerFlag = frontWheelsSteerFlag
        Me.rearWheelsSteerFlag = rearWheelsSteerFlag
        Me.frontLeftWheel.steerFlag = frontWheelsSteerFlag
        Me.frontRightWheel.steerFlag = frontWheelsSteerFlag
        Me.rearLeftWheel.steerFlag = rearWheelsSteerFlag
        Me.rearRightWheel.steerFlag = rearWheelsSteerFlag
    End Sub

    Public Sub setThrottle(ByVal throttleRatio As Single)
        Me.throttleRatio = throttleRatio
        Me.throttleRatioInput = 0.0000001!
    End Sub

    Public Sub setWheelProperties(ByVal wheelRadius As Single, ByVal wheelHeightOffset As Single, ByVal maxSuspensionTravel As Single, ByVal springValue As Single, ByVal damperValue As Single, ByVal springTargetValue As Single)
        Me.wheelRadius = wheelRadius
        Me.maxSuspensionTravel = maxSuspensionTravel
        Me.suspensionSpringForce = springValue
        Me.suspensinoDamperValue = damperValue
        Me.suspensionSpringTargetValue = springTargetValue
        Dim wheelHeightPosition As Single = ((wheelRadius - (Me.chasisSize.Y / 2.0!)) + wheelHeightOffset)
        Me.frontLeftWheel.setProperties(wheelRadius, wheelHeightPosition, maxSuspensionTravel, springValue, damperValue, springTargetValue)
        Me.frontRightWheel.setProperties(wheelRadius, wheelHeightPosition, maxSuspensionTravel, springValue, damperValue, springTargetValue)
        Me.rearLeftWheel.setProperties(wheelRadius, wheelHeightPosition, maxSuspensionTravel, springValue, damperValue, springTargetValue)
        Me.rearRightWheel.setProperties(wheelRadius, wheelHeightPosition, maxSuspensionTravel, springValue, damperValue, springTargetValue)
    End Sub

    Public Sub tick(ByVal timeStep As Single)
        Dim speed As Single = Me.chasisActor.LinearVelocity.Length
        Me.tickSteering(timeStep, speed)
        Me.tickThrottle(timeStep)
        Me.tickBrake(timeStep)
        Me.steerRatioInput = 0.0!
        Me.throttleRatioInput = 0.0!
        Me.brakeRatioInput = 0.0!
    End Sub

    Private Sub tickBrake(ByVal timeStep As Single)
        If (Me.brakeRatioInput = 0.0!) Then
            Dim num As Single = (timeStep * 10.0!)
            Me.brakeRatio = NovodexUtil.clampFloat((Me.brakeRatio - num), 0.0!, 1.0!)
        Else
            Me.brakeRatio = NovodexUtil.clampFloat((Me.brakeRatio + Me.brakeRatioInput), 0.0!, 1.0!)
        End If
        Me.brakeTorque = ((Me.brakeRatio * Me.maxBrakeTorque) / 4.0!)
        Me.frontLeftWheel.setBrakeTorque(Me.brakeTorque)
        Me.frontRightWheel.setBrakeTorque(Me.brakeTorque)
        Me.rearLeftWheel.setBrakeTorque(Me.brakeTorque)
        Me.rearRightWheel.setBrakeTorque(Me.brakeTorque)
    End Sub

    Private Sub tickSteering(ByVal timeStep As Single, ByVal speed As Single)
        Dim num As Single = (speed / Me.chasisSize.Z)
        Dim num2 As Single = (1.5! - (num * 0.5!))
        Dim num3 As Single = NovodexUtil.clampFloat(num2, 0.25!, 1.0!)
        If (Me.steerRatioInput = 0.0!) Then
            Dim num4 As Single = (timeStep * 3.0!)
            If (Me.steerRatio > 0.0!) Then
                Me.steerRatio = NovodexUtil.clampFloat((Me.steerRatio - num4), 0.0!, 1.0!)
            Else
                Me.steerRatio = NovodexUtil.clampFloat((Me.steerRatio + num4), -1.0!, 0.0!)
            End If
        Else
            Me.steerRatio = NovodexUtil.clampFloat((Me.steerRatio + Me.steerRatioInput), -1.0!, 1.0!)
        End If
        Me.steerAngle = ((Me.steerRatio * Me.maxSteerAngle) * num3)
        Dim steerAngle As Single = 0.0!
        Dim num6 As Single = 0.0!
        If Me.frontWheelsSteerFlag Then
            steerAngle = Me.steerAngle
        End If
        If Me.rearWheelsSteerFlag Then
            num6 = -Me.steerAngle
        End If
        Me.frontLeftWheel.setSteerAngle(steerAngle)
        Me.frontRightWheel.setSteerAngle(steerAngle)
        Me.rearLeftWheel.setSteerAngle(num6)
        Me.rearRightWheel.setSteerAngle(num6)
    End Sub

    Private Sub tickThrottle(ByVal timeStep As Single)
        If (Me.throttleRatioInput = 0.0!) Then
            Dim num As Single = (timeStep * 10.0!)
            If (Me.throttleRatio > 0.0!) Then
                Me.throttleRatio = NovodexUtil.clampFloat((Me.throttleRatio - num), 0.0!, 1.0!)
            Else
                Me.throttleRatio = NovodexUtil.clampFloat((Me.throttleRatio + num), -1.0!, 0.0!)
            End If
        Else
            Me.throttleRatio = NovodexUtil.clampFloat((Me.throttleRatio + Me.throttleRatioInput), -1.0!, 1.0!)
        End If
        Me.driveTorque = (Me.throttleRatio * Me.maxDriveTorque)
        Dim num2 As Single = 1.0!
        If Me.frontWheelsDriveFlag Then
            num2 = (num2 / 2.0!)
        End If
        If Me.rearWheelsDriveFlag Then
            num2 = (num2 / 2.0!)
        End If
        Dim driveTorque As Single = 0.0!
        Dim num4 As Single = 0.0!
        If Me.frontWheelsDriveFlag Then
            driveTorque = (Me.driveTorque * num2)
        End If
        If Me.rearWheelsDriveFlag Then
            num4 = (Me.driveTorque * num2)
        End If
        Me.frontLeftWheel.setDriveTorque(driveTorque)
        Me.frontRightWheel.setDriveTorque(driveTorque)
        Me.rearLeftWheel.setDriveTorque(num4)
        Me.rearRightWheel.setDriveTorque(num4)
    End Sub


    ' Fields
    Private brakeRatio As Single = 0.0!
    Private brakeRatioInput As Single
    Private brakeTorque As Single
    Public chasisActor As NxActor
    Private chasisSize As Vector3
    Private driveTorque As Single
    Public frontLeftWheel As SimpleWheel
    Public frontRightWheel As SimpleWheel
    Private frontWheelsDriveFlag As Boolean
    Private frontWheelsSteerFlag As Boolean
    Private maxBrakeTorque As Single
    Private maxDriveTorque As Single
    Private maxSteerAngle As Single
    Private maxSuspensionTravel As Single
    Public rearLeftWheel As SimpleWheel
    Public rearRightWheel As SimpleWheel
    Private rearWheelsDriveFlag As Boolean
    Private rearWheelsSteerFlag As Boolean
    Private steerAngle As Single
    Private steerRatio As Single = 0.0!
    Private steerRatioInput As Single
    Private suspensinoDamperValue As Single
    Private suspensionSpringForce As Single
    Private suspensionSpringTargetValue As Single
    Private throttleRatio As Single = 0.0!
    Private throttleRatioInput As Single
    Private totalMass As Single
    Private wheelRadius As Single
End Class




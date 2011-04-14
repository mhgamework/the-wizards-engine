Public Class PlayerCamera
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
    End Sub

    Private WithEvents mProcessEventElement As New ProcessEventElement(Me)

    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As DeviceEventArgs)
        MyBase.OnDeviceReset(sender, e)
        Me.CreateAHCam()
    End Sub

    Public Sub CreateAHCam()
        If Me.AHCam IsNot Nothing Then Me.AHCam.Dispose()
        Me.setAHCam(New AHCamera(Me.HoofdObj.DevContainer.DX))
        Me.AHCam.Style = AHCamera.CameraStyle.TargetBased
        Me.AHCam.ZoomDistance = 10
    End Sub

    Private mAHCam As AHCamera
    Public ReadOnly Property AHCam() As AHCamera
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAHCam
        End Get
    End Property
    Private Sub setAHCam(ByVal value As AHCamera)
        Me.mAHCam = value
    End Sub



    Private mPlayerEntity As Player
    Public Property PlayerEntity() As Player
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPlayerEntity
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Player)
            Me.mPlayerEntity = value
        End Set
    End Property




    Private mTargetPositie As Vector3
    Public Property TargetPositie() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTargetPositie
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
            Me.mTargetPositie = value
            Me.AHCam.TargetPosition = value
        End Set
    End Property

    Public Sub UpdatePlayerEntity()
        If Me.PlayerEntity IsNot Nothing Then

            ' Me.PlayerEntity.Rotatiematrix = Matrix.RotationYawPitchRoll(Me.AHCam.AngleHorizontal, Me.PlayerEntity.RotatieOud.X, Me.PlayerEntity.RotatieOud.Z)


        End If

    End Sub


    Private mAngles As Vector3
    Public ReadOnly Property Angles() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAngles
        End Get
    End Property
    Private Sub setAngles(ByVal value As Vector3)
        Me.mAngles = value
    End Sub


    Public Overrides Property Enabled() As Boolean
        Get
            Return MyBase.Enabled
        End Get
        Set(ByVal value As Boolean)
            MyBase.Enabled = value
            If Me.Enabled Then Me.AHCam.ForceUpdate()
        End Set
    End Property

    Private Sub mProcessEventElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles mProcessEventElement.Process
        Me.AHCam.Process()

        With Me.HoofdObj.DevContainer.DX.Input
            If .MouseState.Z > 0 Then
                Me.AHCam.ZoomDistance -= 0.4! * Me.AHCam.ZoomDistance / 5
                If Me.AHCam.ZoomDistance < 1 Then Me.AHCam.ZoomDistance = 1
            ElseIf .MouseState.Z < 0 Then
                Me.AHCam.ZoomDistance += 0.4! * Me.AHCam.ZoomDistance / 5
                If Me.AHCam.ZoomDistance > 100 Then Me.AHCam.ZoomDistance = 100
            End If
            If .MouseState.X <> 0 Then
                Me.AHCam.AngleHorizontal += .MouseState.X * CSng(Math.PI / 180)

                Me.AHCam.AngleHorizontal = CSng(Me.AHCam.AngleHorizontal Mod (2 * Math.PI))
                If Me.AHCam.AngleHorizontal < 0 Then Me.AHCam.AngleHorizontal += CSng(2 * Math.PI)

            End If
            If .MouseState.Y <> 0 Then
                Me.AHCam.AngleVertical -= .MouseState.Y * CSng(Math.PI / 180)
                If Me.AHCam.AngleVertical > Math.PI / 2 Then
                    Me.AHCam.AngleVertical = Math.PI / 2
                ElseIf Me.AHCam.AngleVertical < -Math.PI / 2 Then
                    Me.AHCam.AngleVertical = -Math.PI / 2
                End If
            End If

        End With
        If Me.PlayerEntity IsNot Nothing Then
            Me.TargetPositie = Me.PlayerEntity.functions.Positie
        End If
        If Me.AHCam.Changing Then
            Me.setAngles(New Vector3(Me.AHCam.AngleVertical, Me.AHCam.AngleHorizontal, Me.AHCam.AngleRoll))
            Me.UpdatePlayerEntity()
        End If
    End Sub
End Class

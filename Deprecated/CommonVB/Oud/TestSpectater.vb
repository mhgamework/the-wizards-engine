'Public Class TestSpectater
'    Inherits SpelObject
'    Implements IPositionable3D

'    Public Sub New(ByVal nParent As SpelObject)
'        MyBase.New(nParent)



'        'Me.mBeweging = New Beweging(Me.mCamera)
'        With Me '.mBeweging
'            .BewegingsSnelheid = 12
'            .BewegingsSnelheidLopen = 30
'            .KnopVooruit = DirectInput.Key.W
'            .KnopRechts = DirectInput.Key.D
'            .KnopAchteruit = DirectInput.Key.S
'            .KnopLinks = DirectInput.Key.A
'            .KnopOmhoog = DirectInput.Key.Space
'            .KnopOmlaag = DirectInput.Key.LeftControl
'            .KnopLopen = DirectInput.Key.LeftShift
'        End With

'        'Me.Massa = 1

'    End Sub

'    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As spelobject.deviceeventargs)
'        MyBase.OnDeviceReset(sender, e)
'        Me.CreateCamera()
'        'Me.ScheduleAction(AddressOf Me.CreateCamera, 1000)
'    End Sub

'    Public Sub CreateCamera()
'        If Me.Camera IsNot Nothing Then Me.Camera.Dispose()

'        Me.setCamera(New AHCamera(Me.HoofdObj.DevContainer.DX))
'        Me.Camera.Style = AHCamera.CameraStyle.PositionBased
'        Me.Camera.CameraDirection = New Vector3(0, 0, 1)
'    End Sub

'    Private mmCamera As AHCamera
'    Public ReadOnly Property Camera() As AHCamera
'        <System.Diagnostics.DebuggerStepThrough()> Get
'            Return Me.mmCamera
'        End Get
'    End Property
'    Private Sub setCamera(ByVal value As AHCamera)
'        Me.mmCamera = value
'    End Sub

'    'Private mBeweging As Beweging

'    Public BewegingsSnelheid As Double
'    Public BewegingsSnelheidLopen As Double
'    '''''''''''''''''''Knoppen'''''''''''''''''''''''''''''''''''''''''
'    Public KnopVooruit As Microsoft.DirectX.DirectInput.Key
'    Public KnopAchteruit As Microsoft.DirectX.DirectInput.Key
'    Public KnopLinks As Microsoft.DirectX.DirectInput.Key
'    Public KnopRechts As Microsoft.DirectX.DirectInput.Key
'    Public KnopOmhoog As Microsoft.DirectX.DirectInput.Key
'    Public KnopOmlaag As Microsoft.DirectX.DirectInput.Key
'    Public KnopLopen As Microsoft.DirectX.DirectInput.Key
'    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'    'Public Camera As Artificial.Heart.AHCamera

'    Private mSnelheid As Vector3
'    Public ReadOnly Property Snelheid() As Vector3
'        Get
'            Return Me.mSnelheid
'        End Get
'    End Property





'    Public Overrides Property Enabled() As Boolean
'        Get
'            Return MyBase.Enabled
'        End Get
'        Set(ByVal value As Boolean)
'            MyBase.Enabled = value
'            If Me.Enabled Then
'                Me.Camera.ForceUpdate()
'            End If
'        End Set
'    End Property



'    Private Sub TestSpectater_Process(ByVal ev As MHGameWork.Game3DPlay.ProcessEvent) Handles Me.Process
'        'If Not Spel.Geef.ActieveCamera Is Me.mCam Then Spel.Geef.ActieveCamera = Me.mCam

'        'If Spel.GetInstance.DX.Input.KeyDown(DirectInput.Key.T) Then Me.mBeweging.BewegingsSnelheid = 120
'        'If Spel.GetInstance.DX.Input.KeyUp(DirectInput.Key.T) Then Me.mBeweging.BewegingsSnelheid = 12
'        'If Spel.GetInstance.DX.Input.KeyUp(DirectInput.Key.P) Then MsgBox(Me.mCamera.CameraPosition.ToString)

'        With Me.HoofdObj.DevContainer.DX.Input


'            Dim nSnelheid As Vector3
'            Dim Dir As Vector3
'            If .KeyPressed(Me.KnopVooruit) Then
'                Dir = Me.Camera.CameraDirection
'                nSnelheid.Add(Dir)
'            End If
'            If .KeyPressed(Me.KnopRechts) Then
'                Dir = Vector3.TransformCoordinate(Me.Camera.CameraDirection, Matrix.RotationY(Math.PI / 2))
'                nSnelheid.Add(Dir)
'            End If
'            If .KeyPressed(Me.KnopAchteruit) Then
'                Dir = Vector3.TransformCoordinate(Me.Camera.CameraDirection, Matrix.RotationY(Math.PI))
'                nSnelheid.Add(Dir)
'            End If
'            If .KeyPressed(Me.KnopLinks) Then
'                Dir = Vector3.TransformCoordinate(Me.Camera.CameraDirection, Matrix.RotationY(-Math.PI / 2))
'                nSnelheid.Add(Dir)
'            End If
'            If .KeyPressed(Me.KnopOmhoog) Then
'                Dir = New Vector3(0, 1, 0)
'                nSnelheid.Add(Dir)
'            End If
'            If .KeyPressed(Me.KnopOmlaag) Then
'                Dir = New Vector3(0, -1, 0)
'                nSnelheid.Add(Dir)
'            End If

'            nSnelheid.Normalize()
'            If .KeyPressed(Me.KnopLopen) Then
'                nSnelheid.Multiply(CSng(Me.BewegingsSnelheidLopen))
'            ElseIf .KeyPressed(DirectInput.Key.T) Then
'                nSnelheid.Multiply(120)
'            Else
'                nSnelheid.Multiply(CSng(Me.BewegingsSnelheid))
'            End If
'            Me.mSnelheid = nSnelheid





'            ''If Spel.GetInstance.DX.Input.MouseState.X <> 0 Then Stop

'            If .MouseState.X <> 0 Then
'                Me.Camera.AngleHorizontal -= .MouseState.X * CSng(Math.PI / 180)

'                Me.Camera.AngleHorizontal = CSng(Me.Camera.AngleHorizontal Mod (2 * Math.PI))
'                If Me.Camera.AngleHorizontal < 0 Then Me.Camera.AngleHorizontal += CSng(2 * Math.PI)

'            End If
'            If .MouseState.Y <> 0 Then
'                Me.Camera.AngleVertical -= .MouseState.Y * CSng(Math.PI / 180)
'                If Me.Camera.AngleVertical > Math.PI / 2 Then
'                    Me.Camera.AngleVertical = Math.PI / 2
'                ElseIf Me.Camera.AngleVertical < -Math.PI / 2 Then
'                    Me.Camera.AngleVertical = -Math.PI / 2
'                End If
'            End If





'            'Me.Camera.AngleHorizontal -= CSng(.MouseState.X) * 2
'            'Me.Camera.AngleVertical -= CSng(.MouseState.Y * 2)
'            Me.Camera.Process()
'            Me.Positie += Me.Snelheid * ev.Elapsed
'        End With
'    End Sub

'    Public ReadOnly Property ParentPositionable3D() As MHGameWork.Game3DPlay.IPositionable3D Implements MHGameWork.Game3DPlay.IPositionable3D.ParentPositionable3D
'        Get
'            Return Nothing
'        End Get
'    End Property



'    Private mPositie As Vector3
'    Public Property Positie() As Vector3 Implements MHGameWork.Game3DPlay.IPositionable3D.Positie
'        <System.Diagnostics.DebuggerStepThrough()> Get
'            Return Me.mPositie
'        End Get
'        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
'            Me.mPositie = value
'            Me.Camera.CameraPosition = value
'        End Set
'    End Property



'    Public Property RelatievePostie() As Microsoft.DirectX.Vector3 Implements MHGameWork.Game3DPlay.IPositionable3D.RelatievePostie
'        Get

'        End Get
'        Set(ByVal value As Microsoft.DirectX.Vector3)

'        End Set
'    End Property
'End Class

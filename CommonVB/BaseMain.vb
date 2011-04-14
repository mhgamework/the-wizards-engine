Public MustInherit Class BaseMain
    Inherits BaseHoofdObject
    Implements IProcessable
    Implements ITickable




    '' ''Public MustOverride ReadOnly Property BaseDB() As DatabaseCommunication

    Public Sub New()
        MyBase.New()
        '' ''Me.ClientGravity = New Vector3(0, -10, 0)
        '' ''Me.ClientSpeed = 10
        '' ''Me.timeStepArray = New Single(&H20 - 1) {}
        '' ''Dim i As Integer = 0
        '' ''Do While (i < Me.timeStepArray.Length)
        '' ''    Me.timeStepArray(i) = 0.01666667!
        '' ''    i += 1
        '' ''Loop

        Me.InitNx()


        '' ''Me.setModelManager(Me.CreateModelManager)
        '' ''Me.setGameFilesList(Me.CreateGameFilesList)
    End Sub


    '' ''Public MustOverride Function CreateNetworkElement(ByVal nEntity As Entity) As NetworkElement

    '' ''Public MustOverride Function Createfunctions(ByVal nEntity As Entity) As EntityFunctions



    '' ''Private mModelManager As ModelManager
    '' ''Public ReadOnly Property ModelManager() As ModelManager
    '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' ''        Return Me.mModelManager
    '' ''    End Get
    '' ''End Property
    '' ''Private Sub setModelManager(ByVal value As ModelManager)
    '' ''    Me.mModelManager = value
    '' ''End Sub

    '' ''Protected MustOverride Function CreateModelManager() As ModelManager


    '' ''Private mGameFilesList As GameFilesList
    '' ''Public ReadOnly Property GameFilesList() As GameFilesList
    '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' ''        Return Me.mGameFilesList
    '' ''    End Get
    '' ''End Property
    '' ''Private Sub setGameFilesList(ByVal value As GameFilesList)
    '' ''    Me.mGameFilesList = value
    '' ''End Sub

    '' ''Protected MustOverride Function CreateGameFilesList() As GameFilesList




    '' ''Public Sub ConsoleWriteLine(ByVal nLine As String)

    '' ''End Sub



    '' ''Private mClientGravity As Vector3
    '' ''Public Property ClientGravity() As Vector3
    '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' ''        Return Me.mClientGravity
    '' ''    End Get
    '' ''    Set(ByVal value As Vector3)
    '' ''        Me.mClientGravity = value
    '' ''    End Set
    '' ''End Property



    '' ''Private mClientSpeed As Single
    '' ''Public Property ClientSpeed() As Single
    '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' ''        Return Me.mClientSpeed
    '' ''    End Get
    '' ''    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
    '' ''        Me.mClientSpeed = value
    '' ''    End Set
    '' ''End Property







    Public Sub InitNx() 'ByVal nDevice As Direct3D.Device)
        Me.setCustomOutputStream(New CustomOutputStream(Me))
        Me.setCustomReport(New CustomReport(Me))

        Me.setPhysicsSDK(NxPhysicsSDK.Create(Me.CustomOutputStream))
        If Me.PhysicsSDK Is Nothing Then Throw New Exception("Kan Ageia physics sdk niet aanmaken")




        'These need to be set for the debugRenderer to have anything to render
        PhysicsSDK.setParameter(NxParameter.NX_VISUALIZATION_SCALE, 0.5)  'Things like actor axes and joints will be drawn to the size of visualizeScale
        PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_ACTOR_AXES, 1)              'Set to non-zero to visualize
        PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_JOINT_LIMITS, 1)
        PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_SHAPES, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_CLOTH_COLLISIONS, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_CLOTH_COLLISIONS, 1)

        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_AABBS, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_SHAPES, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_AXES, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_VNORMALS, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_FNORMALS, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_EDGES, 1)
        'PhysicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_SPHERES, 1)
        'Set the skin width to 0.01 meters
        PhysicsSDK.setParameter(NxParameter.NX_SKIN_WIDTH, 0.01F)




        Dim SDesc As New NxSceneDesc
        SDesc.gravity = New Vector3(0, -9.81, 0)
        'SDesc.FlagEnableMultiThread = True

        'SDesc.gravity = New Vector3(0, 0, 0)

        'SDesc.broadphase = NX_BROADPHASE_COHERENT
        'sdesc.collisionDetection = true
        Me.setPhysicsScene(Me.PhysicsSDK.createScene(SDesc))
        If Me.PhysicsScene Is Nothing Then Stop

        'Me.setPhysicsScene(Me.PhysicsSDK.createScene(New Vector3(0, -1, 0)))
        'If Me.PhysicsScene Is Nothing Then Stop

        Dim DefaultMaterial As NxMaterial = Me.PhysicsScene.getMaterialFromIndex(0)
        DefaultMaterial.setRestitution(0.5)
        DefaultMaterial.setStaticFriction(0.5)
        DefaultMaterial.setDynamicFriction(0.5)

        'Attach our CustomReport object to the scene
        PhysicsScene.setUserContactReport(Me.CustomReport) 'This is called when flagged actors collide
        PhysicsScene.setUserTriggerReport(Me.CustomReport) 'This is called when an actor enters or leaves a shape marked as a trigger
        PhysicsScene.setUserNotify(Me.CustomReport)            'This is called when a joint is broken by excessive force. (Manually releasing a joint will not call this)

        'Me.resetScene()

    End Sub

    Public Sub ShutdownAgeia()
        If Me.PhysicsSDK IsNot Nothing Then
            If Me.PhysicsSDK.IsReleased = False Then
                If Me.PhysicsScene IsNot Nothing Then
                    Me.PhysicsSDK.releaseScene(Me.PhysicsScene)
                End If
                If (Me.PhysicsSDK.getNbScenes() = 0) Then
                    Me.PhysicsSDK.release()
                End If
            End If
            Me.setPhysicsScene(Nothing)
            Me.setPhysicsSDK(Nothing)
        End If
    End Sub




    'Public Function InitializeGraphics() As Boolean
    '    Try
    '        Dim parameters As New PresentParameters
    '        parameters.Windowed = True
    '        parameters.SwapEffect = SwapEffect.Discard
    '        Me.renderDevice = New Device(0, DeviceType.Hardware, Me.viewport_panel, CreateFlags.MixedVertexProcessing, New PresentParameters() {parameters})
    '        Return True
    '    Catch exception As DirectXException
    '        MessageBox.Show(exception.Message, "InitializeGraphics Error")
    '        Return False
    '    End Try
    'End Function

    'Private Sub render()
    '    If (Not Me.renderDevice Is Nothing) Then
    '        Me.renderDevice.Clear(ClearFlags.Target, Color.LightBlue, 0.0!, 0)
    '        Me.renderDevice.BeginScene()
    '        Me.drawScene()
    '        Me.renderDevice.EndScene()
    '        Me.renderDevice.Present()
    '    End If
    'End Sub

    Public Sub wakeUpScene()
        Dim actor As NxActor
        For Each actor In Me.PhysicsScene.getActors
            actor.wakeUp()
        Next
    End Sub

    '    <STAThread()> _
    'Private Shared Sub Main()
    '        Using tutorial As SimpleTutorial = New SimpleTutorial
    '            If Not tutorial.InitializeGraphics Then
    '                MessageBox.Show("Could not initialize Direct3D")
    '            Else
    '                tutorial.Show()
    '                tutorial.appStarted()
    '                Do While tutorial.Created
    '                    If tutorial.startedFlag Then
    '                        tutorial.processKeys()
    '                        tutorial.tickPhysics()
    '                    End If
    '                    tutorial.render()
    '                    SimpleTutorial.purgePrintList()
    '                    Application.DoEvents()
    '                Loop
    '                tutorial.killPhysics()
    '            End If
    '        End Using
    '    End Sub

    'Protected Overrides Function ProcessKeyPreview(ByRef m As Message) As Boolean
    '    If Me.startedFlag Then
    '        Select Case m.Msg
    '            Case WM_KEYDOWN
    '                If (((CInt(m.LParam) >> 30) And 1) = 0) Then
    '                    Me.keyStates(CInt(m.WParam)) = True
    '                End If
    '                Exit Select
    '            Case WM_KEYUP
    '                Me.keyStates(CInt(m.WParam)) = False
    '                Exit Select
    '        End Select
    '    End If
    '    Return MyBase.ProcessKeyPreview((m))
    'End Function

    Public Overrides Sub Dispose()
        MyBase.Dispose()
        Me.ShutdownAgeia()
    End Sub



    Private mCustomReport As CustomReport
    Public ReadOnly Property CustomReport() As CustomReport
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCustomReport
        End Get
    End Property
    Private Sub setCustomReport(ByVal value As CustomReport)
        Me.mCustomReport = value
    End Sub






    Private mPhysicsScene As NxScene
    Public ReadOnly Property PhysicsScene() As NxScene
        Get
            Return Me.mPhysicsScene
        End Get
    End Property
    Protected Sub setPhysicsScene(ByVal value As NxScene)
        Me.mPhysicsScene = value
    End Sub


    Private mCustomOutputStream As CustomOutputStream
    Public ReadOnly Property CustomOutputStream() As CustomOutputStream
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCustomOutputStream
        End Get
    End Property
    Private Sub setCustomOutputStream(ByVal value As CustomOutputStream)
        Me.mCustomOutputStream = value
    End Sub



    Private mPhysicsSDK As NovodexWrapper.NxPhysicsSDK
    Public ReadOnly Property PhysicsSDK() As NovodexWrapper.NxPhysicsSDK
        Get
            Return Me.mPhysicsSDK
        End Get
    End Property
    Private Sub setPhysicsSDK(ByVal value As NovodexWrapper.NxPhysicsSDK)
        Me.mPhysicsSDK = value
    End Sub




    Public Sub WriteLine(ByVal S As String)

    End Sub
















    'Public Sub resetScene()
    '    Dim meshList As ArrayList = NovodexUtil.getAllMeshesAssociatedWithScene(Me.PhysicsScene, True, True, True)
    '    ControllerManager.purgeControllers()
    '    NovodexUtil.releaseAllActorsFromScene(Me.PhysicsScene)
    '    NovodexUtil.releaseAllClothsFromScene(Me.PhysicsScene)
    '    NovodexUtil.releaseAllMaterialsFromScene(Me.PhysicsScene)
    '    NovodexUtil.releaseMeshes(Me.PhysicsSDK, meshList)
    '    NovodexUtil.seedRandom(&H95B8FB)
    '    Me.randomActorCount = 0
    '    Me.unicycleActor = Nothing
    '    Me.unicycleInput = New Vector3(-1.57!, 0.0!, 0.0!)
    '    Me.boxController = Nothing
    '    Me.capsuleController = Nothing
    '    Me.car = Nothing
    '    Me.cameraRot = New Vector3(-20.0!, 0.0!, 0.0!)
    '    Me.cameraPos = New Vector3(0.0!, 5.0!, -10.0!)
    '    Me.capsuleControllerRot = New Vector3(0.0!, 0.0!, 0.0!)
    '    Me.capsuleControllerPos = New Vector3(-5.0!, 1.3!, 4.0!)
    '    Me.boxControllerRot = New Vector3(0.0!, 0.0!, 0.0!)
    '    Me.boxControllerPos = New Vector3(-3.0!, 1.3!, 4.0!)
    '    Me.deltaControllerMovement = New Vector3(0.0!, 0.0!, 0.0!)
    '    Me.createPhysicsStuff()
    '    Me.DoPhysics()
    '    'Me.car_radioButton.Checked = True
    'End Sub

    'Private Sub resetScene_button_Click(ByVal sender As Object, ByVal e As EventArgs)
    '    Me.startedFlag = False
    '    Me.resetScene()
    '    Me.setProperFocus()
    'End Sub

    'Private Sub setProperFocus()
    '    Me.listBox.Focus()
    'End Sub

    'Public Sub setupView()
    '    Dim aspectRatio As Single = 4 / 3 '(CSng(Me.viewport_panel.ClientSize.Width) / CSng(Me.viewport_panel.ClientSize.Height))
    '    Me.cameraMatrix = (Matrix.Translation(Me.cameraPos) * Matrix.RotationYawPitchRoll((Me.cameraRot.Y * NovodexUtil.DEG_TO_RAD), (-Me.cameraRot.X * NovodexUtil.DEG_TO_RAD), 0.0!))
    '    NovodexUtil.setMatrixPos((Me.cameraMatrix), Me.cameraPos)
    '    Dim vector As Vector3 = NovodexUtil.getMatrixZaxis((Me.cameraMatrix))
    '    Dim cameraTarget As Vector3 = (Me.cameraPos + vector)
    '    Dim cameraUpVector As New Vector3(0.0!, 1.0!, 0.0!)
    '    Me.renderDevice.Transform.View = Matrix.LookAtLH(Me.cameraPos, cameraTarget, cameraUpVector)
    '    Me.renderDevice.Transform.Projection = Matrix.PerspectiveFovLH((Me.verticalFOV * NovodexUtil.DEG_TO_RAD), aspectRatio, Me.nearClipDistance, Me.farClipDistance)
    'End Sub




    'Private Sub viewport_panel_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
    '    If Me.startedFlag Then
    '        Dim worldRay As NxRay = NovodexUtil.getRayFromLeftHandedPerspectiveViewport(e.X, e.Y, CSng(Me.viewport_panel.ClientSize.Width), CSng(Me.viewport_panel.ClientSize.Height), Me.verticalFOV, Me.cameraMatrix)
    '        Me.raycastTest(worldRay)
    '    End If
    '    Me.startedFlag = True
    'End Sub




    ' Fields
    Private boxController As NxBoxController = Nothing
    'Private boxController_radioButton As RadioButton
    Private boxControllerPos As Vector3
    Private boxControllerRot As Vector3
    'Private buildingGeometry As Geometry
    Private cameraMatrix As Matrix
    Private cameraPos As Vector3
    Private cameraRot As Vector3
    Private capsuleController As NxCapsuleController = Nothing
    'Private capsuleController_radioButton As RadioButton
    Private capsuleControllerPos As Vector3
    Private capsuleControllerRot As Vector3
    'Private car As SimpleCar = Nothing
    'Private car_radioButton As RadioButton
    'Private clear_button As Button
    Private clothAreaScalar As Single = 1000.0!
    'Private customControllerHitReport As CustomControllerHitReport = New CustomControllerHitReport
    'Private customEntityReport As CustomEntityReport = New CustomEntityReport
    'Private customRaycastReport As CustomRaycastReport = New CustomRaycastReport
    Private deltaControllerMovement As Vector3
    '''Private driveObjectType As DriveObjectEnum = DriveObjectEnum.Unicycle
    Private farClipDistance As Single = 2000.0!
    'Private garbageCollect_button As Button
    'Private gravity_checkBox As CheckBox
    Private keyStates As Boolean() = New Boolean(&H100 - 1) {}
    'Private label1 As Label
    'Private label2 As Label
    'Private label3 As Label
    'Private label4 As Label
    Private lastTime As Single = 0.0!
    'Private lastTimeStep As Single = 0.0!
    'Private listBox As ListBox
    Private nearClipDistance As Single = 0.01!
    'Private pause_checkBox As CheckBox
    Private pauseFlag As Boolean = False

    'TODO: Private physicsGravity As Vector3 = New Vector3(0.0!, -9.8!, 0.0!)

    'Private printInfo_button As Button
    'Private Shared printList As ArrayList = New ArrayList
    'Private printScene_button As Button
    Private randomActorCount As Integer = 0
    'Private renderDevice As Device = Nothing
    'Private resetScene_button As Button
    Private startedFlag As Boolean = False
    Private steerUnicycleWheelSeparatelyFlag As Boolean = False
    Private timeStepArray As Single() = Nothing
    Private timeStepIndex As Integer = 0
    'Private unicycle_radioButton As RadioButton
    Private unicycleActor As NxActor = Nothing
    Private unicycleInput As Vector3
    Private verticalFOV As Single = 60.0!
    Private viewport_panel As Panel
    Private visualizeScale As Single = 0.5!
    'Private wind_label As Label
    'Private wind_trackBar As TrackBar
    Private Const WM_KEYDOWN As Integer = &H100
    Private Const WM_KEYUP As Integer = &H101









    'Public Sub DoPhysics()
    '    If Not pauseFlag Then

    '        'PhysicsScene.simulate(getTimeStep())    'Run the physics for X seconds
    '        PhysicsScene.flushStream()              'Flush any commands that haven't been run yet
    '        PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()

    '        If unicycleActor IsNot Nothing Then

    '            Dim wheelShape As NxWheelShape = CType(unicycleActor.getShape(1), NxWheelShape) 'I just happen to know the shapeIndex for the wheel
    '            wheelShape.MotorTorque = unicycleInput.Z * 100

    '            If steerUnicycleWheelSeparatelyFlag Then
    '                wheelShape.SteerAngle = unicycleInput.X
    '            Else
    '                unicycleActor.GlobalOrientation = Matrix.RotationY(unicycleInput.X + 1.57F)
    '            End If

    '            'If the unicylce falls asleep applying a motor torque won't make it move, so the unicycle is forced awake if there is drive input
    '            If unicycleInput.Z <> 0 Then
    '                unicycleActor.wakeUp()
    '            End If
    '        End If

    '        If car IsNot Nothing Then

    '            car.tick(getLastTimeStep())
    '            car.chasisActor.wakeUp()
    '        End If


    '        Dim clothArray As NxCloth() = PhysicsScene.getCloths()
    '        For Each cloth As NxCloth In clothArray

    '            'Apply a mostly rightward "wind" to certain cloths.
    '            If (String.Compare(cloth.Name, "Carpet") <> 0) Then

    '                Dim clothArea As Single = cloth.UserData.ToInt32() / clothAreaScalar    'The cloth area was encoded into the UserData variable scaled by clothAreaScalar.
    '                'Dim windScalar As Single = clothArea * wind_trackBar.Value * 0.05F
    '                Dim windScalar As Single = clothArea * 5 * 0.05F
    '                cloth.ExternalAcceleration = New Vector3(NovodexUtil.randomFloat(windScalar * 2, windScalar * 4), NovodexUtil.randomFloat(-windScalar, windScalar), NovodexUtil.randomFloat(-windScalar, windScalar))
    '            End If
    '        Next

    '        If Me.DevContainer.DX.Input.MouseDown(DirectInput.MouseOffset.Button1) Then '(Form.MouseButtons==MouseButtons.Right)
    '            Dim mousePos As Point = viewport_panel.PointToClient(Form.MousePosition)
    '            Dim ray As NxRay = NovodexUtil.getRayFromLeftHandedPerspectiveViewport(mousePos.X, mousePos.Y, viewport_panel.ClientSize.Width, viewport_panel.ClientSize.Height, verticalFOV, cameraMatrix)
    '            pokeObject(ray)
    '        End If

    '        If deltaControllerMovement.Length() > 0 Then

    '            ''Dim collisionFlags As UInteger

    '            ''if driveObjectType=DriveObjectEnum.CapsuleController && capsuleController!=null)
    '            ''	{capsuleController.move(deltaControllerMovement,0xFFFFFFFF,0.001f,out collisionFlags,1.0f);}
    '            ''else if(driveObjectType==DriveObjectEnum.BoxController && boxController!=null)
    '            ''	{boxController.move(deltaControllerMovement,0xFFFFFFFF,0.001f,out collisionFlags,1.0f);}

    '            ControllerManager.updateControllers()
    '            deltaControllerMovement = New Vector3(0, 0, 0)
    '        End If

    '        'If the controllers have the little spikey nose orient in the direction the controller is facing.
    '        ''if(capsuleController!=null && capsuleController.getActor().getNbShapes()==2)
    '        ''	{capsuleController.getActor().getLastShape().LocalOrientation=Matrix.RotationY(capsuleControllerRot.Y*NovodexUtil.DEG_TO_RAD);}
    '        ''if(boxController!=null && boxController.getActor().getNbShapes()==2)
    '        ''	{boxController.getActor().getLastShape().LocalOrientation=Matrix.RotationY(boxControllerRot.Y*NovodexUtil.DEG_TO_RAD);}
    '    End If
    'End Sub













    'Public PhysicsAsync As Boolean = False

    'Private Sub BaseMain_Process(ByVal ev As MHGameWork.Game3DPlay.ProcessEvent) Handles Me.Process

    '    If Me.PhysicsAsync Then

    '        PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, False)

    '        Me.OnChildrenEvent(ev)

    '        PhysicsScene.simulate(getTimeStep())    'Run the physics for X seconds
    '        PhysicsScene.flushStream()              'Flush any commands that haven't been run yet



    '        ev.FireToChildren = False
    '    Else

    '        PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()
    '        PhysicsScene.simulate(getTimeStep())    'Run the physics for X seconds
    '        PhysicsScene.flushStream()              'Flush any commands that haven't been run yet
    '    End If






    'End Sub

    'Protected Overrides Sub HoofdObject_Render2D(ByVal ev As MHGameWork.Game3DPlay.Render2DEvent)
    '    If Not Me.DeviceReady Then Exit Sub




    '    Me.OnChildrenEvent(New BeforeRender2DEvent)
    '    Me.OnChildrenEvent(New BeforeRender3DEvent)

    '    Me.DevContainer.DX.ClearAll(Color.SkyBlue)
    '    Me.DevContainer.DX.Device.BeginScene()




    '    Me.OnChildrenEvent(New Render3DEvent)
    '    'Me.OnChildrenEvent(New Render2DEvent)


    '    Me.drawScene()

    '    If Me.Font IsNot Nothing Then Me.Font.Write(Nothing, New RectangleF(30, 30, 200, 200), Me.FPS.ToString, Direct3D.DrawTextFormat.Left Or Direct3D.DrawTextFormat.VerticalCenter, Color.Black.ToArgb)




    '    Me.DevContainer.DX.Device.EndScene()


    '    Me.OnChildrenEvent(New AfterRender2DEvent)
    '    Me.OnChildrenEvent(New AfterRender3DEvent)

    '    Me.DevContainer.DX.PresentAll()



    'End Sub

    'Protected Overrides Sub mProcessEventElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs)
    '    MyBase.mProcessEventElement_Process(sender, e)
    '    'PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()
    '    'PhysicsScene.simulate(e.Elapsed)    'Run the physics for X seconds
    '    'PhysicsScene.flushStream()              'Flush any commands that haven't been run
    'End Sub

    'Protected Overrides Sub mTickEventElement_Tick(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.TickBaseElement.TickEventArgs)
    '    MyBase.mTickEventElement_Tick(sender, e)
    '    'Beep()

    '    'PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()
    '    'NovodexWrapper.ControllerManager.updateControllers()
    '    'PhysicsScene.simulate(e.Elapsed)    'Run the physics for X seconds
    '    'PhysicsScene.flushStream()              'Flush any commands that haven't been run yet


    '    PhysicsScene.simulate(e.Elapsed)    'Run the physics for X seconds
    '    PhysicsScene.flushStream()              'Flush any commands that haven't been run yet

    '    PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()
    '    NovodexWrapper.ControllerManager.updateControllers()
    'End Sub

    Public Overridable Sub OnProcess(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.ProcessEventArgs) Implements Game3DPlay.Core.Elements.IProcessable.OnProcess




    End Sub

    Public Overridable Sub OnTick(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.TickEventArgs) Implements Game3DPlay.Core.Elements.ITickable.OnTick
        If True Then 'e.Keyboard.IsKeyDown(Input.Keys.L) Then
            PhysicsScene.simulate(e.Elapsed)    'Run the physics for X seconds
            PhysicsScene.flushStream()              'Flush any commands that haven't been run yet

            PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()
            NovodexWrapper.ControllerManager.updateControllers()
        Else
            PhysicsScene.flushStream()              'Flush any commands that haven't been run yet

            PhysicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, True)   'Get the results of the simulation which is required before the next call to simulate()
            NovodexWrapper.ControllerManager.updateControllers()



            PhysicsScene.simulate(e.Elapsed)    'Run the physics for X seconds



        End If

    End Sub
End Class






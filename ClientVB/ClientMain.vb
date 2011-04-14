Public Class ClientMain
    Inherits BaseMain




    Private mWereld As ClientWereld
    Public ReadOnly Property Wereld() As ClientWereld
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mWereld
        End Get
    End Property
    Private Sub setWereld(ByVal value As ClientWereld)
        Me.mWereld = value
    End Sub


    Protected mServer As ProxyServer
    Public ReadOnly Property Server() As ProxyServer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mServer
        End Get
    End Property
    Protected Sub setServer(ByVal value As ProxyServer)
        Me.mServer = value
    End Sub



    '' '' '' '' ''Private mUpdater As ClientUpdaterCommunicater
    '' '' '' '' ''Public ReadOnly Property Updater() As ClientUpdaterCommunicater
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mUpdater
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Private Sub setUpdater(ByVal value As ClientUpdaterCommunicater)
    '' '' '' '' ''    Me.mUpdater = value
    '' '' '' '' ''End Sub



    ' '' ''Private mServerAddress As Net.IPAddress
    ' '' ''Public ReadOnly Property ServerAddress() As Net.IPAddress
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mServerAddress
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setServerAddress(ByVal value As Net.IPAddress)
    ' '' ''    Me.mServerAddress = value
    ' '' ''End Sub

    Protected FPSText As SpelObjecten.Text2D

    Public debugRenderer As PhysicsDebugRenderer
    Public spec As SpelObjecten.Spectater

    Protected WithEvents fpsCounter As Game3DPlay.FpsCounter
    Protected Bollen As New List(Of NxActor)
    'Protected Spec As New SpelObjecten.Spectater(Me)
    'Dim Model As New SpelObjecten.XNBModel(Me)
    Public Sub New()
        CLMain = Me
        ' '' ''    '''''Me.InactiveWaitTime = 50

        ' '' ''    Me.GameFilesList.LaadGameFiles()

        ' '' ''    Me.ReadServerINI()
        'Me.setCommunication(New ClientCommunication(Me.ServerAddress, 5012, 5014))
        '''''Me.setCommunication(New ClientCommunication(Me, Net.IPAddress.Parse("127.0.0.1"), 5012, 5014))
        ' '' ''    '''''Me.setUpdater(New ClientUpdaterCommunicater(Me))

        fpsCounter = New FpsCounter(Me.XNAGame)
        fpsCounter.Enabled = True
        'fpsCounter.Updated += New EventHandler < EventArgs > (UpdateFramerate)
        XNAGame.Components.Add(fpsCounter)

        'Me.setServer(New ProxyServer(Me))
        'Me.setWereld(New ClientWereld(Me))
        'Me.LaadGUI()
        ' '' ''    '''''Me.setWereld(New Wereld(Me))
        XNAGame.IsFixedTimeStep = False
        XNAGame.Graphics.SynchronizeWithVerticalRetrace = False

        'Me.LoginMenu.Enabled = False

        ' '' ''    '''''Me.HoofdMenu.Enabled = False
        ' '' ''    '''''Me.Wereld.Enabled = False

        ' '' ''    '''''Me.Updater.Comm.TCPConn.Connect(Me.ServerAddress, 5013)

        spec = New SpelObjecten.Spectater(Me)

        Me.SetCamera(spec)



        'Model.Positie = New Vector3(1000, 0, 0)
        FPSText = New SpelObjecten.Text2D(Me)
        FPSText.Positie = New Vector2(10, 10)
        FPSText.TextColor = Framework.Graphics.Color.Red
        'FPSText.FontFilename = "Gamedata\ComicSansMS"
        FPSText.FontFilename = "Content\ComicSansMS"




        Me.CreateGroundPlane()

        Me.debugRenderer = New PhysicsDebugRenderer(Me, New NovodexDebugRenderer(XNAGame.Graphics.GraphicsDevice), Me.PhysicsScene)


        Me.CreateProxyServer()
        ''''''Me.setServer(New ProxyServer(Me))
        ''''''Me.Server.Ping()
        ' '' ''    '''''Me.setDebugPhysicsRenderElement(New DebugPhysicsRenderElement(Me, Me.PhysicsScene))




    End Sub

    Public Overridable Sub CreateProxyServer()
        Me.setServer(New ProxyServer(Me))
    End Sub



    Public Overrides Sub OnTick(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.TickEventArgs)
        MyBase.OnTick(sender, e)

    End Sub

    Public Overrides Sub OnProcess(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.ProcessEventArgs)
        MyBase.OnProcess(sender, e)
        'If e.Keyboard.IsKeyDown(Input.Keys.B) Then
        '    Me.Bollen.Add(Me.CreateSphere)
        'End If
        'If e.Mouse._lastMouseState.LeftButton = Input.ButtonState.Pressed Then
        '    Me.Bollen.Add(Me.CreateSphere)
        '    Me.Bollen(Me.Bollen.Count - 1).addForce(Me.Spec.CameraDirection * 100, NxForceMode.NX_IMPULSE)

        'End If

        FPSText.Text = Me.FPS.ToString
    End Sub

    Public Overrides Sub OnLoad(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.LoadEventArgs)
        MyBase.OnLoad(sender, e)
        Me.debugRenderer.DebugRenderer.setRenderDevice(XNAGame.Graphics.GraphicsDevice)
        'Model.RelativePath = "Content\Models\p1_wedge"
        'Model.LoadModel()
    End Sub

    Dim groundPlane As NxActor

    Public Sub CreateGroundPlane()
        Dim planeDesc As New NxPlaneShapeDesc
        planeDesc.materialIndex = 0
        Dim actorDesc As New NxActorDesc
        actorDesc.addShapeDesc(planeDesc)

        Me.groundPlane = Me.PhysicsScene.createActor(actorDesc)


    End Sub

    Public Function CreateSphere() As NxActor


        Dim actorDesc As New NxActorDesc
        Dim bodyDesc As New NxBodyDesc
        Dim sphereDesc As New NxSphereShapeDesc
        sphereDesc.radius = 1 / 2
        actorDesc.addShapeDesc(sphereDesc)


        actorDesc.BodyDesc = bodyDesc
        actorDesc.density = 10
        actorDesc.globalPose = Matrix.CreateTranslation(spec.Positie)

        Return Me.PhysicsScene.createActor(actorDesc)
    End Function

    ' '' ''Public Overrides Sub [Exit]()
    ' '' ''    Me.GameFilesList.SaveGameFiles()
    ' '' ''    MyBase.[Exit]()
    ' '' ''End Sub

    ' '' ''Protected Overrides Function CreateGameFilesList() As Common.Networking.GameFilesList
    ' '' ''    Return New ClientGameFilesList
    ' '' ''End Function

    '' '' '' '' ''Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.DeviceEventArgs)
    '' '' '' '' ''    MyBase.OnDeviceReset(sender, e)
    '' '' '' '' ''    Me.setPhysicsDebugRenderer(New PhysicsDebugRenderer(e.Device))
    '' '' '' '' ''    Me.PhysicsDebugRenderer.ZBufferEnabled = True
    '' '' '' '' ''    Me.PhysicsDebugRenderer.DrawLineShadows = False
    '' '' '' '' ''    Me.DebugPhysicsRenderElement.setPhysicsDebugRenderer(Me.PhysicsDebugRenderer)
    '' '' '' '' ''    Me.DebugPhysicsRenderElement.Visible = False
    '' '' '' '' ''End Sub

    '' '' '' '' ''Private mPhysicsDebugRenderer As PhysicsDebugRenderer
    '' '' '' '' ''Public ReadOnly Property PhysicsDebugRenderer() As PhysicsDebugRenderer
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mPhysicsDebugRenderer
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Private Sub setPhysicsDebugRenderer(ByVal value As PhysicsDebugRenderer)
    '' '' '' '' ''    Me.mPhysicsDebugRenderer = value
    '' '' '' '' ''End Sub

    '' '' '' '' ''Private mDebugPhysicsRenderElement As DebugPhysicsRenderElement
    '' '' '' '' ''Public ReadOnly Property DebugPhysicsRenderElement() As DebugPhysicsRenderElement
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mDebugPhysicsRenderElement
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Private Sub setDebugPhysicsRenderElement(ByVal value As DebugPhysicsRenderElement)
    '' '' '' '' ''    Me.mDebugPhysicsRenderElement = value
    '' '' '' '' ''End Sub

    ' '' ''Public Sub ReadServerINI()
    ' '' ''    Dim Pad As String = Forms.Application.StartupPath & "\server.ini"
    ' '' ''    Try
    ' '' ''        Dim Lines As String() = System.IO.File.ReadAllLines(Pad)
    ' '' ''        If Lines.Length = 0 Then Throw New Exception
    ' '' ''        Me.setServerAddress(Net.IPAddress.Parse(Lines(0)))

    ' '' ''    Catch ex As Exception
    ' '' ''        Throw New Exception("Kan Server.ini niet lezen")
    ' '' ''    End Try



    ' '' ''End Sub

    ' '' ''Public Overrides ReadOnly Property BaseDB() As Common.DatabaseCommunication
    ' '' ''    Get
    ' '' ''        Throw New Exception
    ' '' ''    End Get
    ' '' ''End Property


    '' '' '' '' ''Private mWereld As Wereld
    '' '' '' '' ''Public ReadOnly Property Wereld() As Wereld
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mWereld
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Private Sub setWereld(ByVal value As Wereld)
    '' '' '' '' ''    Me.mWereld = value
    '' '' '' '' ''End Sub


    '' '' '' '' ''Private mClient As Client
    '' '' '' '' ''Public ReadOnly Property Client() As Client
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mClient
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Public Sub setClient(ByVal value As Client)
    '' '' '' '' ''    Me.mClient = value
    '' '' '' '' ''End Sub




    '' '' '' '' ''Private mHoofdMenu As HoofdMenu
    '' '' '' '' ''Public ReadOnly Property HoofdMenu() As HoofdMenu
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mHoofdMenu
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Private Sub setHoofdMenu(ByVal value As HoofdMenu)
    '' '' '' '' ''    Me.mHoofdMenu = value
    '' '' '' '' ''End Sub


    Private mLoginMenu As LoginMenu
    Public ReadOnly Property LoginMenu() As LoginMenu
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mLoginMenu
        End Get
    End Property
    Private Sub setLoginMenu(ByVal value As LoginMenu)
        Me.mLoginMenu = value
    End Sub


    '' '' ''Private mUpdaterMenu As UpdaterMenu
    '' '' ''Public ReadOnly Property UpdaterMenu() As UpdaterMenu
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mUpdaterMenu
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Private Sub setUpdaterMenu(ByVal value As UpdaterMenu)
    '' '' ''    Me.mUpdaterMenu = value
    '' '' ''End Sub


    ' '' ''Public Shadows ReadOnly Property GameFilesList() As ClientGameFilesList
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return CType(MyBase.GameFilesList, ClientGameFilesList)
    ' '' ''    End Get
    ' '' ''End Property


    '' '' '' '' ''Private mCursor As Cursor001
    '' '' '' '' ''Public ReadOnly Property Cursor() As Cursor001
    '' '' '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' '' '' ''        Return Me.mCursor
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property
    '' '' '' '' ''Private Sub setCursor(ByVal value As Cursor001)
    '' '' '' '' ''    Me.mCursor = value
    '' '' '' '' ''End Sub






    Public Sub LaadGUI()
        '''''Me.setCursor(New Cursor001(Me))
        '''''Me.SetActiveCursor(Me.Cursor)
        '''''Me.setHoofdMenu(Me.CreateHoofdMenu)

        Me.setLoginMenu(New LoginMenu(Me))

    End Sub

    '' '' ''Protected Overridable Function CreateHoofdMenu() As HoofdMenu
    '' '' ''    Return New HoofdMenu(Me)
    '' '' ''End Function

    '' '' '' '' ''Protected Overridable Function CreateServerCommunication(ByVal nServerAddress As Net.IPAddress, ByVal nServerUDPPort As Integer, ByVal nServerTCPPort As Integer) As ServerCommunication
    '' '' '' '' ''    Return New ServerCommunication(Me, nServerAddress, nServerUDPPort, nServerTCPPort)

    '' '' '' '' ''End Function


    ' '' ''Protected Overrides Sub OnProcess(ByVal sender As Object, ByVal e As Game3DPlay.Core.Elements.ProcessEventArgs)
    ' '' ''    MyBase.OnProcess(sender, e)

    ' '' ''    ' '' ''With Me.DevContainer.DX.Input
    ' '' ''    ' '' ''    If .KeyDown(Key.Escape) Then
    ' '' ''    ' '' ''        If Me.LoginMenu.Enabled Then
    ' '' ''    ' '' ''            Me.StopSpel()
    ' '' ''    ' '' ''        ElseIf Me.Wereld.Enabled Then
    ' '' ''    ' '' ''            If Me.Wereld.DisableUserInteraction = False Then
    ' '' ''    ' '' ''                Me.Wereld.DisableUserInteraction = True
    ' '' ''    ' '' ''                Me.HoofdMenu.Enabled = True
    ' '' ''    ' '' ''            Else
    ' '' ''    ' '' ''                If Me.HoofdMenu.Enabled Then
    ' '' ''    ' '' ''                    Me.HoofdMenu.Enabled = False
    ' '' ''    ' '' ''                    Me.Wereld.DisableUserInteraction = False
    ' '' ''    ' '' ''                End If
    ' '' ''    ' '' ''            End If

    ' '' ''    ' '' ''        End If

    ' '' ''    ' '' ''    End If
    ' '' ''    ' '' ''End With
    ' '' ''End Sub

    ' '' ''Public Overrides Sub LoadGraphicsContent(ByVal loadAllContent As Boolean)
    ' '' ''    MyBase.LoadGraphicsContent(loadAllContent)
    ' '' ''End Sub


    '' '' ''Public Overrides Function CreateNetworkElement(ByVal nEntity As Common.Entity) As NetworkElement
    '' '' ''    Dim N As New ClientNetworkElement(nEntity)
    '' '' ''    Return N
    '' '' ''End Function

    '' '' '' '' ''Public Overrides Function Createfunctions(ByVal nEntity As Common.Entity) As Common.EntityFunctions
    '' '' '' '' ''    Return New ClientEntityFunctions(nEntity)
    '' '' '' '' ''End Function

    '' '' '' '' ''Protected Overrides Function CreateModelManager() As Common.ModelManager
    '' '' '' '' ''    Return New ClientModelManager(Me)
    '' '' '' '' ''End Function



    '' '' '' '' ''Public Shadows ReadOnly Property ModelManager() As ClientModelManager
    '' '' '' '' ''    Get
    '' '' '' '' ''        Return DirectCast(MyBase.ModelManager, ClientModelManager)
    '' '' '' '' ''    End Get
    '' '' '' '' ''End Property






    '' '' '' '' ''Public Function CreateEntity(ByVal nType As EntityType, ByVal nParent As OctTree) As Entity
    '' '' '' '' ''    Dim Ent As Entity
    '' '' '' '' ''    Select Case nType
    '' '' '' '' ''        Case EntityType.Bol001
    '' '' '' '' ''            Dim B As New Bol001(nParent)
    '' '' '' '' ''            Ent = B
    '' '' '' '' ''        Case EntityType.Player
    '' '' '' '' ''            Dim PL As New Player(nParent)
    '' '' '' '' ''            PL.setFunctions(New DynamicClientEntityFunctions(PL))
    '' '' '' '' ''            PL.setPlayerFunctions(New ClientPlayerFunctions(PL))
    '' '' '' '' ''            Ent = PL
    '' '' '' '' ''            'Case EntityType.Lamp001
    '' '' '' '' ''            '    Stop
    '' '' '' '' ''            'Return New Lamp001(nParent)
    '' '' '' '' ''        Case EntityType.StaticEntity
    '' '' '' '' ''            Dim S As New StaticEntity(nParent)
    '' '' '' '' ''            S.setStaticEntityFunctions(New ClientStaticEntityFunctions(S))
    '' '' '' '' ''            Ent = S
    '' '' '' '' ''            'Case EntityType.Box001
    '' '' '' '' ''            '    Stop
    '' '' '' '' ''            'Return New Box001(nParent)
    '' '' '' '' ''        Case EntityType.GroundPlane
    '' '' '' '' ''            Dim GP As New GroundPlane(nParent)
    '' '' '' '' ''            Ent = GP
    '' '' '' '' ''        Case EntityType.BouncingBall001
    '' '' '' '' ''            Dim BB As New BouncingBall001(nParent)
    '' '' '' '' ''            Ent = BB
    '' '' '' '' ''        Case Else
    '' '' '' '' ''            Throw New Exception("Onbekend entity type")
    '' '' '' '' ''    End Select

    '' '' '' '' ''    If Ent.Functions Is Nothing Then
    '' '' '' '' ''        Ent.setFunctions(New ClientEntityFunctions(Ent))
    '' '' '' '' ''    End If

    '' '' '' '' ''    Return Ent
    '' '' '' '' ''End Function





    Protected Overridable Sub fpsCounter_Updated(ByVal sender As Object, ByVal e As System.EventArgs) Handles fpsCounter.Updated
        XNAGame.Window.Title = Me.WindowTitle & String.Format("FPS : {0:n}", fpsCounter.FPS)
    End Sub
End Class

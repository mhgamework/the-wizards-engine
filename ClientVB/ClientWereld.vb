Public Class ClientWereld
    Inherits SpelObject

    ' '' ''Private WithEvents mServerComm As ServerCommunication


    ' '' ''Private WithEvents mProcessEventElement As New ProcessEventElement(Me)
    ' '' ''Private WithEvents RenderContainer As New RenderContainer(Me)
    ' '' ''Private WithEvents RenderElement As New RenderEventElement(Me)

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        ' '' ''mServerComm = CType(Me.HoofdObj, ClientMain).ServerComm
        '' '' ''Me.setClient(New Client(Me))

        ' '' ''Me.setPlCommandBuilder(New PlayerCommandBuilder)

        ' '' ''Me.setCam(New Spectater(Me))
        ' '' ''Me.Cam.Enabled = False

        ' '' ''Me.setPLCam(New PlayerCamera(Me))
        ' '' ''Me.PLCam.Enabled = True


        ' '' ''Me.setTree(New OctTree(Me, Vector3.Empty, New Vector3(100, 100, 100)))
        ' '' ''Me.Tree.RootNode.Split()
        ' '' ''Me.Tree.RootNode.Nodes(0).Split()


        ' '' ''Me.setPlayerCommandInterval(CInt(1000 / 33))


        '' '' ''Me.SendAngles()


        '' '' ''Dim B As New Bol001(Me.Tree)
        '' '' ''Dim EDS As EntityDeltaSnapshot
        '' '' ''With DirectCast(B.functions, ClientActor)
        '' '' ''    EDS = New EntityDeltaSnapshot(B.EntityID)
        '' '' ''    EDS.AddDeltaSnapshotChange(EntityDeltaSnapshotChange.CreatePositieChange(New Vector3(10, 0, 0)))
        '' '' ''    .AddEntityDeltaSnapshot(0, EDS)
        '' '' ''    EDS = New EntityDeltaSnapshot(B.EntityID)
        '' '' ''    EDS.AddDeltaSnapshotChange(EntityDeltaSnapshotChange.CreatePositieChange(New Vector3(0, 10, 0)))
        '' '' ''    .AddEntityDeltaSnapshot(40, EDS)
        '' '' ''End With

        ' '' ''Me.InitTerreinClient()

    End Sub

    Public Sub OnSuccessfulLogin(ByVal LoginKey As String)
        CLMain.Server.LinkUDPConnection(LoginKey)
    End Sub

    ' '' ''Public ClientRootSquare As Terrain.Client.RootSquare

    ' '' ''Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As Game3DPlay.DeviceEventArgs)
    ' '' ''    MyBase.OnDeviceReset(sender, e)
    ' '' ''    Me.ClientRootSquare.OnDeviceReset(sender, e)
    ' '' ''End Sub

    ' '' ''Public Sub InitTerreinClient()
    ' '' ''    Dim PrecompFS As New IO.FileStream(Forms.Application.StartupPath & "\TerrainPrecomputed.TWF", IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None, &H10000, IO.FileOptions.RandomAccess)
    ' '' ''    Dim BFSR As New BlockFileStreamReader(PrecompFS, PrecompBlock.BlockSize)
    ' '' ''    Me.ClientRootSquare = New Terrain.Client.RootSquare(Me.HoofdObj, BFSR, New BlockPointer(0))
    ' '' ''    Me.ClientRootSquare.LoadFromDisk()
    ' '' ''    Me.ClientRootSquare.DetailThreshold = 10
    ' '' ''    'Me.SplitRecursive(Me.ServerRootSquare, 7)

    ' '' ''    'Me.ServerRootSquare.Height(Terrain.Server.HeightDir.East) = 5
    ' '' ''    'Me.ServerRootSquare.Height(Terrain.Server.HeightDir.South) = 2

    ' '' ''End Sub

    '' '' ''Private WithEvents mClient As Client
    '' '' ''Public ReadOnly Property Client() As Client
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mClient
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Private Sub setClient(ByVal value As Client)
    '' '' ''    Me.mClient = value
    '' '' ''End Sub



    ' '' ''Private mDisableUserInteraction As Boolean
    ' '' ''Public Property DisableUserInteraction() As Boolean
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mDisableUserInteraction
    ' '' ''    End Get
    ' '' ''    Set(ByVal value As Boolean)
    ' '' ''        Me.mDisableUserInteraction = value

    ' '' ''        Me.RenderContainer.Visible = Not value
    ' '' ''        If value = True Then
    ' '' ''            Me.Cam.Enabled = False
    ' '' ''            Me.PLCam.Enabled = False
    ' '' ''        Else
    ' '' ''            Me.Cam.Enabled = False
    ' '' ''            Me.PLCam.Enabled = True
    ' '' ''        End If
    ' '' ''    End Set
    ' '' ''End Property


    ' '' ''Private mPlCommandBuilder As PlayerCommandBuilder
    ' '' ''Public ReadOnly Property PlCommandBuilder() As PlayerCommandBuilder
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mPlCommandBuilder
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setPlCommandBuilder(ByVal value As PlayerCommandBuilder)
    ' '' ''    Me.mPlCommandBuilder = value
    ' '' ''End Sub





    ' '' ''Private mCam As Spectater
    ' '' ''Public ReadOnly Property Cam() As Spectater
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mCam
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setCam(ByVal value As Spectater)
    ' '' ''    Me.mCam = value
    ' '' ''End Sub


    ' '' ''Private mPLCam As PlayerCamera
    ' '' ''Public ReadOnly Property PLCam() As PlayerCamera
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mPLCam
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setPLCam(ByVal value As PlayerCamera)
    ' '' ''    Me.mPLCam = value
    ' '' ''End Sub

    '' '' ''Private mIsLoggedIn As Boolean
    '' '' ''Public ReadOnly Property IsLoggedIn() As Boolean
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mIsLoggedIn
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Public Sub setIsLoggedIn(ByVal value As Boolean)
    '' '' ''    Me.mIsLoggedIn = value
    '' '' ''End Sub


    '' '' ''Private mLinkedPlayerEntity As Player
    '' '' ''Public ReadOnly Property LinkedPlayerEntity() As Player
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mLinkedPlayerEntity
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Private Sub setLinkedPlayerEntity(ByVal value As Player)
    '' '' ''    Me.mLinkedPlayerEntity = value
    '' '' ''End Sub


    '' '' ''Private mLinkedPlayerEntityID As Integer
    '' '' ''Public ReadOnly Property LinkedPlayerEntityID() As Integer
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mLinkedPlayerEntityID
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Public Sub setLinkedPlayerEntityID(ByVal value As Integer)
    '' '' ''    Me.mLinkedPlayerEntityID = value
    '' '' ''    If value <> -1 Then
    '' '' ''        Dim Ent As Entity = Me.Tree.RootNode.FindEntity(value)
    '' '' ''        If Ent IsNot Nothing Then Me.setLinkedPlayerEntity(CType(Ent, Player))
    '' '' ''    Else
    '' '' ''        Me.setLinkedPlayerEntity(Nothing)
    '' '' ''    End If
    '' '' ''End Sub



    ' '' ''Private mTree As OctTree
    ' '' ''Public ReadOnly Property Tree() As OctTree
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mTree
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setTree(ByVal value As OctTree)
    ' '' ''    Me.mTree = value
    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_DeltaSnapshot(ByVal sender As Object, ByVal e As ServerCommunication.DeltaSnapshotEventArgs) Handles mServerComm.DeltaSnapshot
    ' '' ''    Dim Ent As Entity
    ' '' ''    For I As Integer = 0 To e.DeltaSnapshot.EntityDeltaSnapshots.Count - 1
    ' '' ''        Ent = Me.Tree.RootNode.FindEntity(e.DeltaSnapshot.EntityDeltaSnapshots(I).EntityID)
    ' '' ''        If Ent Is Nothing Then
    ' '' ''            CLMain.ServerComm.GetEntityAsync(e.DeltaSnapshot.EntityDeltaSnapshots(I).EntityID)
    ' '' ''        Else
    ' '' ''            DirectCast(Ent.functions, ClientEntityFunctions).AddEntityDeltaSnapshot(e.Tick, e.DeltaSnapshot.EntityDeltaSnapshots(I))
    ' '' ''        End If
    ' '' ''    Next
    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_GetEntityCompleted(ByVal sender As Object, ByVal e As ServerCommunication.GetEntityCompletedEventArgs) Handles mServerComm.GetEntityCompleted
    ' '' ''    Dim Ent As Entity = Me.Tree.RootNode.FindEntity(e.EntityID)
    ' '' ''    If Ent Is Nothing Then
    ' '' ''        Ent = CLMain.CreateEntity(e.EntityType, Me.Tree)
    ' '' ''        Ent.setEntityID(e.EntityID)
    ' '' ''        Me.Tree.AddEntity(Ent)
    ' '' ''        Ent.setVersie(e.Versie)

    ' '' ''        Ent.Update(e.Data)
    ' '' ''    Else
    ' '' ''        If e.Versie > Ent.Versie Then
    ' '' ''            Ent.setVersie(e.Versie)
    ' '' ''            Ent.Update(e.Data)
    ' '' ''        End If
    ' '' ''    End If

    ' '' ''    If Me.mServerComm.Client.LoggedIn Then
    ' '' ''        If Ent.EntityID = Me.mServerComm.Client.LinkedPlayerEntityID Then
    ' '' ''            'Me.PLCam.UpdatePlayerEntity()

    ' '' ''            If Me.mServerComm.Client.LinkedPlayerEntity Is Nothing Then
    ' '' ''                Me.mServerComm.Client.TryLinkPlayerEntity()
    ' '' ''                If Me.mServerComm.Client.LinkedPlayerEntity IsNot Nothing Then
    ' '' ''                    Me.PLCam.PlayerEntity = Me.mServerComm.Client.LinkedPlayerEntity
    ' '' ''                End If
    ' '' ''                'Me.Client.setLinkedPlayerEntity(CType(Ent, Player))
    ' '' ''            End If
    ' '' ''        End If
    ' '' ''    End If
    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_LaadWereldCompleted(ByVal sender As Object, ByVal e As LaadWereldCompletedEventArgs) Handles mServerComm.LaadWereldCompleted

    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_LaadWereldProgressChanged(ByVal sender As Object, ByVal e As LaadWereldProgressChangedEventArgs) Handles mServerComm.LaadWereldProgressChanged
    ' '' ''    Dim Ent As Entity = Me.Tree.RootNode.FindEntity(e.EntityID)
    ' '' ''    If Ent Is Nothing OrElse e.Versie > Ent.Versie Then
    ' '' ''        CLMain.ServerComm.GetEntityAsync(e.EntityID)
    ' '' ''    End If


    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_SignaleerVerandering(ByVal sender As Object, ByVal e As ServerCommunication.SignaleerVeranderingEventArgs) Handles mServerComm.SignaleerVerandering
    ' '' ''    Dim Ent As Entity = Me.Tree.RootNode.FindEntity(e.EntityID)

    ' '' ''    If Ent Is Nothing OrElse e.Versie > Ent.Versie Then
    ' '' ''        CLMain.ServerComm.GetEntityAsync(e.EntityID)
    ' '' ''    End If
    ' '' ''End Sub




    '' '' ''Private mAngle As New Text2D(Me)
    '' '' ''Public ReadOnly Property Angle() As Text2D
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mAngle
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Private Sub setAngle(ByVal value As Text2D)
    '' '' ''    Me.mAngle = value
    '' '' ''End Sub

    '' '' ''Public Sub SendAngles()
    '' '' ''    If Me.LinkedPlayerEntity IsNot Nothing AndAlso Me.PLCam.AHCam.Changed Then
    '' '' ''        CLMain.ServerComm.UpdateAngles(Me.PLCam.Angles)
    '' '' ''    End If
    '' '' ''    Me.ScheduleAction(AddressOf SendAngles, 100)
    '' '' ''End Sub

    ' '' ''Private Sub mProcessEventElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles mProcessEventElement.Process


    ' '' ''    If Me.DisableUserInteraction = False Then Me.ProcessUserInput(sender, e)





    ' '' ''    If Me.HoofdObj.DevContainer.DX.Input.KeyDown(Key.B) Then
    ' '' ''        CLMain.DebugPhysicsRenderElement.Visible = Not CLMain.DebugPhysicsRenderElement.Visible

    ' '' ''    End If



    ' '' ''End Sub


    ' '' ''Public Sub ProcessUserInput(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs)
    ' '' ''    Me.ClientRootSquare.Update(Me.PLCam.AHCam.CameraPosition)

    ' '' ''    With Me.HoofdObj.DevContainer.DX.Input
    ' '' ''        If Me.PLCam.Enabled Then
    ' '' ''            If .KeyDown(Key.W) Then : Me.PlCommandBuilder.Vooruit = True
    ' '' ''            ElseIf .KeyUp(Key.W) Then : Me.PlCommandBuilder.Vooruit = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.S) Then : Me.PlCommandBuilder.Achteruit = True
    ' '' ''            ElseIf .KeyUp(Key.S) Then : Me.PlCommandBuilder.Achteruit = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.A) Then : Me.PlCommandBuilder.StrafeLinks = True
    ' '' ''            ElseIf .KeyUp(Key.A) Then : Me.PlCommandBuilder.StrafeLinks = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.D) Then : Me.PlCommandBuilder.StrafeRechts = True
    ' '' ''            ElseIf .KeyUp(Key.D) Then : Me.PlCommandBuilder.StrafeRechts = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.Space) Then : Me.PlCommandBuilder.Jump = True
    ' '' ''            ElseIf .KeyUp(Key.Space) Then : Me.PlCommandBuilder.Jump = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.LeftControl) Then : Me.PlCommandBuilder.Crouch = True
    ' '' ''            ElseIf .KeyUp(Key.LeftControl) Then : Me.PlCommandBuilder.Crouch = False
    ' '' ''            End If

    ' '' ''            If .MouseDown(MouseOffset.Button0) Then : Me.PlCommandBuilder.PrimaryAttack = True
    ' '' ''            ElseIf .MouseUp(MouseOffset.Button0) Then : Me.PlCommandBuilder.PrimaryAttack = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.LeftShift) Then : Me.PlCommandBuilder.Run = True
    ' '' ''            ElseIf .KeyUp(Key.LeftShift) Then : Me.PlCommandBuilder.Run = False
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.I) Then Me.PlCommandBuilder.NoClip = Not Me.PlCommandBuilder.NoClip





    ' '' ''        End If

    ' '' ''        'End If
    ' '' ''        If .KeyPressed(DirectInput.Key.LeftAlt) Then
    ' '' ''            If .KeyDown(DirectInput.Key.Z) Then
    ' '' ''                If Me.HoofdObj.DevContainer.DX.Device.RenderState.FillMode = FillMode.WireFrame Then
    ' '' ''                    Me.HoofdObj.DevContainer.DX.Device.RenderState.FillMode = FillMode.Solid
    ' '' ''                Else
    ' '' ''                    Me.HoofdObj.DevContainer.DX.Device.RenderState.FillMode = FillMode.WireFrame
    ' '' ''                End If
    ' '' ''            End If

    ' '' ''            If .KeyDown(Key.NumPad8) Then
    ' '' ''                Me.ClientRootSquare.DetailThreshold += 10
    ' '' ''            End If
    ' '' ''            If .KeyDown(Key.NumPad2) Then
    ' '' ''                Me.ClientRootSquare.DetailThreshold -= 10
    ' '' ''                If Me.ClientRootSquare.DetailThreshold < 1 Then Me.ClientRootSquare.DetailThreshold = 1
    ' '' ''            End If

    ' '' ''        End If

    ' '' ''    End With


    ' '' ''    If Me.HoofdObj.DevContainer.DX.Input.KeyDown(Key.C) Then
    ' '' ''        If Me.Cam.Enabled = True Then
    ' '' ''            If Me.mServerComm.Client.LinkedPlayerEntity IsNot Nothing Then
    ' '' ''                Me.Cam.Enabled = False
    ' '' ''                Me.PLCam.Enabled = True
    ' '' ''                Me.PLCam.PlayerEntity = Me.mServerComm.Client.LinkedPlayerEntity
    ' '' ''                'Me.PLCam.TargetPositie = Me.LinkedPlayerEntity.Positie
    ' '' ''            End If
    ' '' ''        Else
    ' '' ''            Me.PLCam.Enabled = False
    ' '' ''            Me.Cam.Enabled = True
    ' '' ''        End If
    ' '' ''    End If




    ' '' ''    If Me.mServerComm.Client.LoggedIn Then
    ' '' ''        If Me.LastPlayerCommand + Me.PlayerCommandInterval < Me.HoofdObj.Time Then
    ' '' ''            Me.PlCommandBuilder.CameraAngles = Me.PLCam.Angles
    ' '' ''            CLMain.ServerComm.SendPlayerCommandAsync(Me.PlCommandBuilder)

    ' '' ''            Me.LastPlayerCommand += Me.PlayerCommandInterval
    ' '' ''            If Me.LastPlayerCommand + Me.PlayerCommandInterval * 5 < Me.HoofdObj.Time Then
    ' '' ''                Me.LastPlayerCommand = Me.HoofdObj.Time
    ' '' ''            End If
    ' '' ''        End If
    ' '' ''    End If

    ' '' ''    If Me.HoofdObj.DevContainer.DX.Input.KeyDown(Key.L) Then
    ' '' ''        CLMain.ServerComm.LaadWereldAsync()
    ' '' ''    End If
    ' '' ''End Sub



    ' '' ''Private mLastPlayerCommand As Integer
    ' '' ''Public Property LastPlayerCommand() As Integer
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mLastPlayerCommand
    ' '' ''    End Get
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    ' '' ''        Me.mLastPlayerCommand = value
    ' '' ''    End Set
    ' '' ''End Property


    ' '' ''Private mPlayerCommandInterval As Integer
    ' '' ''Public ReadOnly Property PlayerCommandInterval() As Integer
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mPlayerCommandInterval
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Private Sub setPlayerCommandInterval(ByVal value As Integer)
    ' '' ''    Me.mPlayerCommandInterval = value
    ' '' ''End Sub



    ' '' ''Private Sub mServerComm_Time(ByVal sender As Object, ByVal e As ServerCommunication.TimeEventArgs) Handles mServerComm.Time
    ' '' ''    Me.HoofdObj.setTime(e.Time)
    ' '' ''    Me.HoofdObj.setLastTick(e.Tick)
    ' '' ''End Sub

    ' '' ''Private Sub mServerComm_Verandering(ByVal sender As Object, ByVal e As ServerCommunication.VeranderingEventArgs) Handles mServerComm.Verandering
    ' '' ''    Dim Ent As Entity = Me.Tree.RootNode.FindEntity(e.EntityID)
    ' '' ''    If Ent Is Nothing Then
    ' '' ''        CLMain.ServerComm.GetEntityAsync(e.EntityID)
    ' '' ''        Exit Sub
    ' '' ''    End If
    ' '' ''    Ent.Update(e.Type, e.Data)

    ' '' ''End Sub

    ' '' ''Private Sub RenderElement_Render(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.Render
    ' '' ''    Me.ClientRootSquare.Render()
    ' '' ''End Sub

    ' '' ''Protected Overrides Sub DisposeObject()
    ' '' ''    MyBase.DisposeObject()
    ' '' ''    Me.ClientRootSquare.Dispose()
    ' '' ''End Sub



























End Class

Public Class WorldManagerMenu
    Inherits Panel

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        Me.mServerComm = CType(Me.HoofdObj, AdminClientMain).ServerComm
        Dim M As New DesignWorldManager()
        M.FillPanel(Me)

        Me.Positie = New Vector2(0, 0)
        Me.Size = Me.ParentSize
        Me.Anchor = AnchorType.Top Or AnchorType.Right Or AnchorType.Bottom Or AnchorType.Left

        Me.renWorldViewport = M.renWorldViewport.GetRendererTo2D
        Me.knpGetModels = M.knpGetModelList.GetKnop001
        Me.lstModels = M.lstModels.GetListBox2D
        Me.knpHoofdMenu = M.knpHoofdmenu.GetKnop001
        Me.knpSnapToGrid = M.knpSnap.GetKnop001
        Me.txtGridX = M.txtGridX.GetTextBox2D
        Me.txtGridY = M.txtGridY.GetTextBox2D




        Me.setSnapGrid(New Grid(Me.renWorldViewport))
        Me.SnapGrid.Width = 100
        Me.SnapGrid.Height = 100
        'Me.SnapGrid.Color = Color.Red
        Me.SnapToGrid = True
        Me.GridSize = New Vector2(134, 134)




    End Sub


    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As Game3DPlay.DeviceEventArgs)
        MyBase.OnDeviceReset(sender, e)
        If Me.Cam Is Nothing Then
            Me.setCam(New Artificial.Heart.AHCamera(Me.HoofdObj.DevContainer.DX))
            Me.Cam.Style = Artificial.Heart.AHCamera.CameraStyle.PositionBased

            Me.Cam.CameraPosition = New Vector3(0, 100, 0)
            Me.Cam.CameraDirection = New Vector3(0, -0.99994, 0)
            Me.Cam.CameraUp = New Vector3(0, 0, 1)
        End If

    End Sub

    Private WithEvents ProcessElement As New ProcessEventElement(Me)

    Private WithEvents mServerComm As AdminServerCommunication




    Private WithEvents renWorldViewport As RendererTo2D
    Private WithEvents knpGetModels As Knop
    Private WithEvents lstModels As ListBox2D
    Private WithEvents knpHoofdMenu As Knop

    Private WithEvents knpSnapToGrid As Knop
    Private WithEvents txtGridX As TextBox2D
    Private WithEvents txtGridY As TextBox2D



    Private mCam As Artificial.Heart.AHCamera
    Public ReadOnly Property Cam() As Artificial.Heart.AHCamera
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCam
        End Get
    End Property
    Private Sub setCam(ByVal value As Artificial.Heart.AHCamera)
        Me.mCam = value
    End Sub




    Private mSnapGrid As Grid
    Public ReadOnly Property SnapGrid() As Grid
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSnapGrid
        End Get
    End Property
    Private Sub setSnapGrid(ByVal value As Grid)
        Me.mSnapGrid = value
    End Sub



    Private mSnapToGrid As Boolean
    Public Property SnapToGrid() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSnapToGrid
        End Get
        Set(ByVal value As Boolean)
            Me.mSnapToGrid = value
            Me.SnapGrid.Enabled = value
        End Set
    End Property


    Private mGridSize As Vector2
    Public Property GridSize() As Vector2
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mGridSize
        End Get
        Set(ByVal value As Vector2)
            Me.mGridSize = value
            Me.UpdateSnapGrid()
        End Set
    End Property


    Private mGridCenter As Vector2
    Public Property GridCenter() As Vector2
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mGridCenter
        End Get
        Set(ByVal value As Vector2)
            Me.mGridCenter = value
            Me.UpdateSnapGrid()
        End Set
    End Property

    Public Sub UpdateSnapGrid()
        Me.SnapGrid.Size = Me.GridSize
        Me.SnapGrid.WorldMatrix = Matrix.Translation(New Vector3(Me.GridCenter.X - Me.SnapGrid.Width / 2.0F * Me.GridSize.X, 0, Me.GridCenter.Y - Me.SnapGrid.Height / 2.0F * Me.GridSize.Y))
    End Sub






    Private Sub renWorldViewport_AfterRender(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles renWorldViewport.AfterRender
        ACLMain.Wereld.Tree.OnAfterRenderWorld(Me, New OctTree.RenderWorldEventArgs)
    End Sub

    Private Sub renWorldViewport_BeforeRender(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles renWorldViewport.BeforeRender
        ACLMain.Wereld.Tree.OnBeforeRenderWorld(Me, New OctTree.RenderWorldEventArgs)
    End Sub

    Private Sub renWorldViewport_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles renWorldViewport.Clicked
        If e.Button = DirectInput.MouseOffset.Button0 And e.State = Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Then
            If Me.lstModels.SelectedIndex = -1 Then Exit Sub

            e.Handled = True

            Dim Near As Vector3
            Dim Far As Vector3

            'Me.HoofdObj.DevContainer.DX.Device.Viewport = Me.renWorldViewport.ViewPort
            'Me.HoofdObj.DevContainer.DX.ComputeViewportRay(e.PointClicked.X - Me.renWorldViewport.Positie.X, e.PointClicked.Y - Me.renWorldViewport.Positie.Y, Near, Far)
            Dim Point As Vector2 = e.PointClicked - Me.renWorldViewport.Positie
            Me.ComputeViewportRay(Point.X, Point.Y, Me.renWorldViewport.ViewPort, Near, Far)



            Dim Intersection As Vector3
            Dim PlaneNormal As Vector3 = New Vector3(0, 1, 0)
            Dim PlaneOriginDist As Single = 0
            Dim RayStart As Vector3 = Near
            Dim RayDir As Vector3 = Vector3.Normalize(Far - Near)




            If CollMath.CalculateRayPlaneIntersection(RayStart, RayDir, PlaneNormal, PlaneOriginDist, Intersection) = RaycastState.Intersection Then
                Dim MD As AdminModelData = DirectCast(Me.lstModels.Item(Me.lstModels.SelectedIndex), AdminModelData)


                If Me.SnapToGrid Then
                    Dim SnapX As Single
                    Dim SnapZ As Single
                    SnapX = CSng(Math.Round((Intersection.X - Me.GridCenter.X - Me.GridSize.X / 2) / (Me.GridSize.X)))
                    SnapZ = CSng(Math.Round((Intersection.Z - Me.GridCenter.Y - Me.GridSize.Y / 2) / (Me.GridSize.Y)))

                    SnapX += 0.5F
                    SnapZ += 0.5F

                    Intersection.X = SnapX * Me.GridSize.X + Me.GridCenter.X
                    Intersection.Z = SnapZ * GridSize.Y + Me.GridCenter.Y
                End If


                ACLMain.ServerComm.PutStaticEntityAsync(New PutStaticEntityPacket(MD.ModelID, Intersection))





            End If


        End If
    End Sub


    Public Sub ComputeViewportRay(ByVal x As Single, ByVal y As Single, ByVal VP As Viewport, ByRef near As Vector3, ByRef far As Vector3) ', Optional ByVal useWorld As Boolean = False)
        Dim v As Vector3
        Dim world As Matrix
        Dim viewport As Viewport = VP
        'If useWorld Then
        '    world = Me.g_dev.Transform.World
        'Else
        world = Matrix.Identity
        'End If
        Dim view As Matrix = Me.HoofdObj.DevContainer.DX.ViewMatrix
        Dim projection As Matrix = Me.HoofdObj.DevContainer.DX.ProjectionMatrix
        v.X = x
        v.Y = y
        v.Z = 0.0!
        near = Vector3.Unproject(v, viewport, projection, view, world)
        v.Z = 1.0!
        far = Vector3.Unproject(v, viewport, projection, view, world)
    End Sub






    Private Sub renWorldViewport_Render(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles renWorldViewport.Render
        ACLMain.Wereld.Tree.OnRenderWorld(Me, New OctTree.RenderWorldEventArgs)

        'Temporary
        ACLMain.Wereld.ClientRootSquare.Render()
    End Sub



    Private Sub ProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessElement.Process
        ACLMain.Wereld.ClientRootSquare.Update(Me.Cam.CameraPosition)
        If Me.Cam IsNot Nothing Then
            Me.Cam.Process()
        End If

        With Me.HoofdObj.DevContainer.DX.Input
            If .KeyDown(DirectInput.Key.Escape) Then
                Me.Enabled = False
                ACLMain.HoofdMenu.Enabled = True
            End If

            If Me.FocusedPart Is Nothing Or Me.FocusedPart Is Me.renWorldViewport Then

                If .MouseState.Z > 0 Then
                    Me.Cam.CameraPosition += New Vector3(0, -Me.Cam.CameraPosition.Y * 0.06F, 0)
                ElseIf .MouseState.Z < 0 Then
                    Me.Cam.CameraPosition += New Vector3(0, Me.Cam.CameraPosition.Y * 0.06F, 0)
                End If


                If .KeyPressed(DirectInput.Key.LeftControl) Then
                    If .MouseState.X <> 0 Then
                        Me.Cam.AngleHorizontal -= .MouseState.X * CSng(Math.PI / 180)

                        Me.Cam.AngleHorizontal = CSng(Me.Cam.AngleHorizontal Mod (2 * Math.PI))
                        If Me.Cam.AngleHorizontal < 0 Then Me.Cam.AngleHorizontal += CSng(2 * Math.PI)

                    End If
                    If .MouseState.Y <> 0 Then
                        Me.Cam.AngleVertical -= .MouseState.Y * CSng(Math.PI / 180)
                        If Me.Cam.AngleVertical > Math.PI / 2 Then
                            Me.Cam.AngleVertical = Math.PI / 2
                        ElseIf Me.Cam.AngleVertical < -Math.PI / 2 Then
                            Me.Cam.AngleVertical = -Math.PI / 2
                        End If
                    End If
                End If

                If .KeyPressed(DirectInput.Key.UpArrow) Then
                    Me.Cam.CameraPosition += New Vector3(0, 0, 1000) * e.Elapsed
                End If
                If .KeyPressed(DirectInput.Key.DownArrow) Then
                    Me.Cam.CameraPosition += New Vector3(0, 0, -1000) * e.Elapsed
                End If
                If .KeyPressed(DirectInput.Key.LeftArrow) Then
                    Me.Cam.CameraPosition += New Vector3(-1000, 0, 0) * e.Elapsed
                End If
                If .KeyPressed(DirectInput.Key.RightArrow) Then
                    Me.Cam.CameraPosition += New Vector3(1000, 0, 0) * e.Elapsed
                End If
            End If
        End With
    End Sub

    Public Sub Activate()
        If Me.Cam IsNot Nothing Then Me.Cam.ForceUpdate()
    End Sub


    Private Sub knpGetModels_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpGetModels.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        Me.mServerComm.GetModelListAsync()
        e.Handled = True

    End Sub

    Private Sub mServerComm_ModelListRecieved(ByVal sender As Object, ByVal e As AdminServerCommunication.ModelListRecievedEventArgs) Handles mServerComm.ModelListRecieved
        If Me.Enabled = False Then Exit Sub

        Me.lstModels.ClearItems()
        For Each AMD As AdminModelData In e.ML.Models
            Me.lstModels.AddItem(AMD)
        Next
    End Sub

    Private Sub knpHoofdmenu_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpHoofdmenu.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub

        Me.Enabled = False
        ACLMain.HoofdMenu.Enabled = True

        e.Handled = True
    End Sub

    Private Sub lstModels_SelectedIndexChanged(ByVal sender As Object, ByVal e As Common.ListBox2D.SelectedIndexChangedEventArgs) Handles lstModels.SelectedIndexChanged
        If Me.lstModels.SelectedIndex = -1 Then

        Else

        End If



    End Sub

    Private Sub knpSnapToGrid_Clicked(ByVal sender As Object, ByVal e As Game3DPlay.ClickedElement.ClickedEventArgs) Handles knpSnapToGrid.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub

        Me.SnapToGrid = Not Me.SnapToGrid
        If Me.SnapToGrid Then
            Me.knpSnapToGrid.Text = "Disable Snap"
        Else
            Me.knpSnapToGrid.Text = "Enable Snap"
        End If
    End Sub

    Private Sub knpGridX_TextChanged(ByVal sender As Object, ByVal e As Common.TextBox2D.TextChangedEventArgs) Handles txtGridX.TextChanged
        Dim X As Single
        If Single.TryParse(Me.txtGridX.Text, X) Then
            Me.GridSize = New Vector2(X, Me.GridSize.Y)
        End If
    End Sub

    Private Sub knpGridY_TextChanged(ByVal sender As Object, ByVal e As Common.TextBox2D.TextChangedEventArgs) Handles txtGridY.TextChanged
        Dim Y As Single
        If Single.TryParse(Me.txtGridY.Text, Y) Then
            Me.GridSize = New Vector2(Me.GridSize.X, Y)
        End If
    End Sub
End Class

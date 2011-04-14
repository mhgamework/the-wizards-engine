Public Class ListBox2D
    Inherits PanelPart

    Public Sub New(ByVal nParent As Panel)
        MyBase.New(nParent)
        Me.mSelectedIndex = -1
        Me.setItems(New List(Of Object))
        Me.mItemHeight = 20

        Me.Items.Add("Hallo")
        Me.Items.Add("Hallo2")


        Me.setSprite2D(New Sprite2D(Me, Forms.Application.StartupPath & "\GameData\Textures\WhitePixel.png"))
        Me.Sprite2D.Visible = False
        'Me.Sprite2D.BackgroundColor = Color.Green
    End Sub

    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.DeviceEventArgs)
        MyBase.OnDeviceReset(sender, e)
        If Me.Font Is Nothing Then
            Me.setFont(AHFont.Default(Me.HoofdObj.DevContainer.DX))
        End If
    End Sub

    Private Renderer As New Renderer2DContainer(Me)

    Private WithEvents ProcessElement As New ProcessEventElement(Me)


    Private mBackgroundColor As Color
    Public Property BackgroundColor() As Color
        Get
            Return Me.Renderer.BackgroundColor
        End Get
        Set(ByVal value As Color)
            Me.Renderer.BackgroundColor = value
        End Set
    End Property


    Private mItems As List(Of Object)
    Public ReadOnly Property Items() As List(Of Object)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mItems
        End Get
    End Property
    Private Sub setItems(ByVal value As List(Of Object))
        Me.mItems = value
    End Sub


    Public ReadOnly Property Item(ByVal index As Integer) As Object
        Get
            Return Me.Items(index)
        End Get
    End Property



    Public Sub ClearItems()
        Me.Items.Clear()
    End Sub

    Public Sub AddItem(ByVal nObj As Object)
        Me.Items.Add(nObj)

    End Sub

    Public Sub AddItemRange(ByVal nColl As IEnumerable(Of Object))
        Me.Items.AddRange(nColl)

    End Sub



    Private mSelectedIndex As Integer
    Public Property SelectedIndex() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSelectedIndex
        End Get
        Set(ByVal value As Integer)
            If value < -1 Then value = -1
            If value > Me.Items.Count - 1 Then value = Me.Items.Count - 1
            If Me.mSelectedIndex = value Then Exit Property
            Me.mSelectedIndex = value
            Me.OnSelectedIndexChanged(Me, New SelectedIndexChangedEventArgs)
        End Set
    End Property


    Public Class SelectedIndexChangedEventArgs
        Inherits System.EventArgs
    End Class
    Public Sub OnSelectedIndexChanged(ByVal sender As Object, ByVal e As SelectedIndexChangedEventArgs)
        RaiseEvent SelectedIndexChanged(Me, e)
    End Sub
    Public Event SelectedIndexChanged(ByVal sender As Object, ByVal e As SelectedIndexChangedEventArgs)




    Public ReadOnly Property SelectedItem() As Object
        Get
            If Me.SelectedIndex = -1 Then Return Nothing
            Return Me.Items(Me.SelectedIndex)
        End Get
    End Property


    Private mTopItemIndex As Integer
    Public Property TopItemIndex() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTopItemIndex
        End Get
        Set(ByVal value As Integer)
            If value > Me.Items.Count - Me.NumVisibleItems Then value = Me.Items.Count - Me.NumVisibleItems
            If value < 0 Then value = 0

            Me.mTopItemIndex = value
        End Set
    End Property


    Private mSprite2D As Sprite2D
    Public ReadOnly Property Sprite2D() As Sprite2D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSprite2D
        End Get
    End Property
    Private Sub setSprite2D(ByVal value As Sprite2D)
        Me.mSprite2D = value
    End Sub





    Private mItemHeight As Integer
    Public Property ItemHeight() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mItemHeight
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
            Me.mItemHeight = value
        End Set
    End Property



    Public ReadOnly Property NumVisibleItems() As Integer
        Get
            Return CInt(Math.Floor(Me.Size.Y / Me.ItemHeight))
        End Get
    End Property



    Private mFont As AHFont
    Public ReadOnly Property Font() As AHFont
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFont
        End Get
    End Property
    Private Sub setFont(ByVal value As AHFont)
        Me.mFont = value
    End Sub



    Protected Overrides Sub OnRender2D(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.RenderElement.RenderEventArgs)
        MyBase.OnRender2D(sender, e)
        If Me.Font Is Nothing Then Exit Sub

        If Me.SelectedIndex <> -1 Then
            Me.Sprite2D.Positie = New Vector2(0, (Me.SelectedIndex - Me.TopItemIndex) * Me.ItemHeight)
            Me.Sprite2D.Size = New Vector2(Me.Size.X, Me.ItemHeight)
            Me.Sprite2D.OnRender2D(sender, e)
        End If
        Dim StartIndex As Integer = Me.TopItemIndex

        Dim EndIndex As Integer = Me.TopItemIndex + Me.NumVisibleItems - 1
        If EndIndex > Me.Items.Count - 1 Then EndIndex = Me.Items.Count - 1

        For I As Integer = 0 To EndIndex - StartIndex
            Me.Font.Write(Nothing, New RectangleF(0, I * Me.ItemHeight, Me.Size.X, Me.ItemHeight), Me.Items(StartIndex + I).ToString, DrawTextFormat.VerticalCenter, Color.Black.ToArgb)
        Next
    End Sub

    Private Sub ListBox2D_Clicked(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ClickedElement.ClickedEventArgs) Handles Me.Clicked
        If e.State <> Game3DPlay.ClickedElement.ClickedEventArgs.MouseState.Down Or e.Button <> DirectInput.MouseOffset.Button0 Then Exit Sub
        'Get the index clicked
        Dim index As Integer = CInt(Math.Floor((e.PointClicked - Me.Positie).Y / Me.ItemHeight))
        index += Me.TopItemIndex
        If index > Me.Items.Count - 1 Then Exit Sub
        Me.SelectedIndex = index
    End Sub


    Private Sub ListBox2D_PositieChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.PanelPart.PositieChangedEventArgs) Handles Me.PositieChanged
        Me.Renderer.Positie = Me.Positie
    End Sub

    Private Sub ListBox2D_SizeChanged(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.Size2DChangedEventArgs) Handles Me.SizeChanged
        Me.Renderer.Size = Me.Size
    End Sub

    Private Sub ProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessElement.Process
        If Me.HasFocus Then
            With Me.HoofdObj.DevContainer.DX.Input
                If .MouseState.Z > 0 Then
                    Me.TopItemIndex -= 1
                ElseIf .MouseState.Z < 0 Then
                    Me.TopItemIndex += 1
                End If
                If .KeyDown(DirectInput.Key.UpArrow) Then
                    Me.SelectedIndex -= 1
                End If
                If .KeyDown(DirectInput.Key.DownArrow) Then
                    Me.SelectedIndex += 1
                End If
            End With
        End If
    End Sub
End Class

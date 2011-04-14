Public Class Grid
    Inherits SpelObject
    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        Me.mHeight = 1
        Me.mWidth = 1
        Me.Size = New Vector2(1, 1)
        Me.WorldMatrix = Matrix.Identity
        Me.Color = Drawing.Color.White
    End Sub

    Private WithEvents RenderElement As New RenderEventElement(Me)





    Protected Overrides Sub DisposeObject()
        MyBase.DisposeObject()
        Device.IsUsingEventHandlers = False
        If (Not Me.gridBuffer Is Nothing) Then
            Me.gridBuffer.Dispose()
        End If
        Me.gridBuffer = Nothing
        Device.IsUsingEventHandlers = True
    End Sub

    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As Game3DPlay.DeviceEventArgs)
        MyBase.OnDeviceReset(sender, e)
        Me.dx = Me.HoofdObj.DevContainer.DX
        If Me.dx.Initialized Then
            Me.BuildGrid()
        End If
    End Sub

    Private Sub BuildGrid()
        If Me.HoofdObj.DeviceReady = False Then Exit Sub

        Device.IsUsingEventHandlers = False
        If (Not Me.gridBuffer Is Nothing) Then
            Me.gridBuffer.Dispose()
        End If
        Me.gridBuffer = New VertexBuffer(GetType(LVertex), ((2 * (Me.mHeight + 1)) + (2 * (Me.mWidth + 1))), Me.dx.Device, Usage.None, (VertexFormats.Texture1 Or (VertexFormats.Diffuse Or VertexFormats.Position)), Pool.Managed)
        Device.IsUsingEventHandlers = True
        Dim vertexArray As LVertex() = DirectCast(Me.gridBuffer.Lock(0, GetType(LVertex), LockFlags.None, New Integer() {((2 * (Me.mHeight + 1)) + (2 * (Me.mWidth + 1)))}), LVertex())
        Dim num2 As Integer = Me.mColor.ToArgb
        Dim index As Integer = 0
        Dim mWidth As Integer = Me.mWidth
        Dim i As Integer = 0
        Do While (i <= mWidth)
            vertexArray(index).v.X = (i * Me.Size.X)
            vertexArray(index).v.Z = 0.0!
            vertexArray(index).v.Y = 0.0!
            vertexArray(index).diffuse = num2
            vertexArray((index + 1)).v.X = (i * Me.Size.X)
            vertexArray((index + 1)).v.Z = (Me.Size.Y * Me.mHeight)
            vertexArray((index + 1)).v.Y = 0.0!
            vertexArray((index + 1)).diffuse = num2
            index = (index + 2)
            i += 1
        Loop
        Dim mHeight As Integer = Me.mHeight
        Dim j As Integer = 0
        Do While (j <= mHeight)
            vertexArray(index).v.X = 0.0!
            vertexArray(index).v.Z = (j * Me.Size.Y)
            vertexArray(index).v.Y = 0.0!
            vertexArray(index).diffuse = num2
            vertexArray((index + 1)).v.X = (Me.Size.X * Me.mWidth)
            vertexArray((index + 1)).v.Z = (j * Me.Size.Y)
            vertexArray((index + 1)).v.Y = 0.0!
            vertexArray((index + 1)).diffuse = num2
            index = (index + 2)
            j += 1
        Loop
        Me.gridBuffer.Unlock()
    End Sub


    ' Properties
    Public Property Height() As Integer
        Get
            Return Me.mHeight
        End Get
        Set(ByVal Value As Integer)
            If (Me.mHeight <> Value) Then
                Me.mHeight = Value

                Me.BuildGrid()

            End If
        End Set
    End Property


    Private mSize As Vector2
    Public Property Size() As Vector2
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSize
        End Get
        Set(ByVal value As Vector2)
            Me.mSize = value
            Me.BuildGrid()

        End Set
    End Property


    Public Property Width() As Integer
        Get
            Return Me.mWidth
        End Get
        Set(ByVal Value As Integer)
            If (Me.mWidth <> Value) Then
                Me.mWidth = Value
                Me.BuildGrid()

            End If
        End Set
    End Property

    Public Property WorldMatrix() As Matrix
        Get
            Return Me.mWorldMatrix
        End Get
        Set(ByVal Value As Matrix)
            Me.mWorldMatrix = Value
        End Set
    End Property


    Private mColor As Color
    Public Property Color() As Color
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mColor
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Color)
            Me.mColor = value
        End Set
    End Property



    ' Fields
    Private dx As AHDirectX
    Private gridBuffer As VertexBuffer
    Private mHeight As Integer
    Private mWidth As Integer
    Private mWorldMatrix As Matrix

    Private Sub RenderElement_Render(ByVal sender As Object, ByVal e As Game3DPlay.RenderElement.RenderEventArgs) Handles RenderElement.Render
        Me.dx.Device.SetTransform(TransformType.World, Me.mWorldMatrix)
        Me.dx.Device.RenderState.Lighting = False
        Me.dx.Device.SetStreamSource(0, Me.gridBuffer, 0)
        Me.dx.SetTexture(0, Nothing)
        Me.dx.Device.VertexFormat = (VertexFormats.Texture1 Or (VertexFormats.Diffuse Or VertexFormats.Position))
        Me.dx.Device.DrawPrimitives(PrimitiveType.LineList, 0, ((Me.mHeight + 1) + (Me.mWidth + 1)))
        Me.dx.Device.RenderState.Lighting = True
    End Sub
End Class



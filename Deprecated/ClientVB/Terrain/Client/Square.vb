Imports Microsoft.DirectX.Direct3D.CustomVertex
Imports MHGameWork.TheWizards.Common.Terrain
Namespace Terrain.Client
    Public Class Square
        Inherits Object
        Implements IDisposable


        Protected Sub New(ByVal nPointer As BlockPointer)
            Me.InitVars()
        End Sub

        Public Sub New(ByVal nParent As Square, ByVal nPointer As BlockPointer, ByVal nChildIndex As Byte)
            Me.New(nPointer)
            Me.setRootSquare(nParent.RootSquare)
            Me.setParent(nParent)

            Me.setPointer(nPointer)

            Me.setChildIndex(nChildIndex)


        End Sub

        Protected Overrides Sub Finalize()
            If Me.VertexBuffer IsNot Nothing Then Throw New Exception
            If Me.IndexBuffer IsNot Nothing Then Throw New Exception
        End Sub


        Private mRootSquare As RootSquare
        Public ReadOnly Property RootSquare() As RootSquare
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mRootSquare
            End Get
        End Property
        Protected Sub setRootSquare(ByVal value As RootSquare)
            Me.mRootSquare = value
        End Sub


        Private mParent As Square
        Public ReadOnly Property Parent() As Square
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mParent
            End Get
        End Property
        Private Sub setParent(ByVal value As Square)
            Me.mParent = value
        End Sub


        Private mPointer As BlockPointer
        Public ReadOnly Property Pointer() As BlockPointer
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mPointer
            End Get
        End Property
        Private Sub setPointer(ByVal value As BlockPointer)
            Me.mPointer = value
        End Sub


        Private mCenter As Vector2
        Public ReadOnly Property Center() As Vector2
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCenter
            End Get
        End Property
        Private Sub setCenter(ByVal value As Vector2)
            Me.mCenter = value
        End Sub


        Private mSizeRadius As Single
        Public ReadOnly Property SizeRadius() As Single
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mSizeRadius
            End Get
        End Property
        Private Sub setSizeRadius(ByVal value As Single)
            Me.mSizeRadius = value
        End Sub






        Private mVertexBuffer As VertexBuffer
        Public ReadOnly Property VertexBuffer() As VertexBuffer
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mVertexBuffer
            End Get
        End Property
        Private Sub setVertexBuffer(ByVal value As VertexBuffer)
            Me.mVertexBuffer = value
        End Sub


        Private mIndexBuffer As IndexBuffer
        Public ReadOnly Property IndexBuffer() As IndexBuffer
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mIndexBuffer
            End Get
        End Property
        Private Sub setIndexBuffer(ByVal value As IndexBuffer)
            Me.mIndexBuffer = value
        End Sub


        Public Sub OnDeviceReset(ByVal sender As Object, ByVal e As Game3DPlay.DeviceEventArgs)
            If Me.VertexBuffer IsNot Nothing Then Me.VertexBuffer.Dispose()
            If Me.IndexBuffer IsNot Nothing Then Me.IndexBuffer.Dispose()

            Me.setVertexBuffer(New VertexBuffer(GetType(Direct3D.CustomVertex.PositionNormalColored), 9, Me.RootSquare.DX.Device, Usage.None, Direct3D.CustomVertex.PositionNormalColored.Format, Pool.Managed))
            Me.setIndexBuffer(New IndexBuffer(GetType(Short), 3 * 8, Me.RootSquare.DX.Device, Usage.Dynamic Or Usage.WriteOnly, Pool.Default))

            Me.LoadFromDisk()

            For IB As Byte = 0 To 4 - 1
                If Me.ChildByte(IB) IsNot Nothing Then Me.ChildByte(IB).OnDeviceReset(sender, e)
            Next

        End Sub

        Public Function LoadPrecompBlock() As PrecompBlock
            Return PrecompBlock.FromBytes(Me.RootSquare.BFSR, Me.Pointer)
        End Function

        Public Sub LoadFromDisk()
            Dim B As PrecompBlock = Me.LoadPrecompBlock

            Me.setCenter(B.Positie)
            Me.setSizeRadius(B.SizeRadius)


            For IB As Byte = 0 To 6 - 1
                Me.ErrorByte(IB) = B.ErrorByte(IB)
            Next

            If Me.VertexBuffer IsNot Nothing Then
                Dim V As Vector3
                Dim Vertices As Direct3D.CustomVertex.PositionNormalColored() = New Direct3D.CustomVertex.PositionNormalColored(9 - 1) {}

                ' VertexDir.Center
                V.X = Me.Center.X
                V.Y = B.Height(VertexDir.Center)
                V.Z = Me.Center.Y
                Vertices(VertexDir.Center) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.East
                V.X = Me.Center.X + Me.SizeRadius
                V.Y = B.Height(VertexDir.East)
                V.Z = Me.Center.Y
                Vertices(VertexDir.East) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.North
                V.X = Me.Center.X
                V.Y = B.Height(VertexDir.North)
                V.Z = Me.Center.Y + Me.SizeRadius
                Vertices(VertexDir.North) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.West
                V.X = Me.Center.X - Me.SizeRadius
                V.Y = B.Height(VertexDir.West)
                V.Z = Me.Center.Y
                Vertices(VertexDir.West) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.South
                V.X = Me.Center.X
                V.Y = B.Height(VertexDir.South)
                V.Z = Me.Center.Y - Me.SizeRadius
                Vertices(VertexDir.South) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)



                ' VertexDir.NorthEast
                V.X = Me.Center.X + Me.SizeRadius
                V.Y = B.Height(VertexDir.NorthEast)
                V.Z = Me.Center.Y + Me.SizeRadius
                Vertices(VertexDir.NorthEast) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.NorthWest
                V.X = Me.Center.X - Me.SizeRadius
                V.Y = B.Height(VertexDir.NorthWest)
                V.Z = Me.Center.Y + Me.SizeRadius
                Vertices(VertexDir.NorthWest) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.SouthWest
                V.X = Me.Center.X - Me.SizeRadius
                V.Y = B.Height(VertexDir.SouthWest)
                V.Z = Me.Center.Y - Me.SizeRadius
                Vertices(VertexDir.SouthWest) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)

                ' VertexDir.SouthEast
                V.X = Me.Center.X + Me.SizeRadius
                V.Y = B.Height(VertexDir.SouthEast)
                V.Z = Me.Center.Y - Me.SizeRadius
                Vertices(VertexDir.SouthEast) = New Direct3D.CustomVertex.PositionNormalColored(V, New Vector3(0, 1, 0), Color.White.ToArgb)


                Dim R, G As Byte

                For I As Integer = 0 To 9 - 1
                    'R = CByte(50) + CByte((Math.Sin((FB.Verts(I).X * 3.6) / 180 * Math.PI) + 1) * 90)
                    'G = CByte(50) + CByte((Math.Sin((FB.Verts(I).Z * 3.6 - 180) / 180 * Math.PI) + 1) * 90)
                    R = CByte((Math.Sin(Vertices(I).X) + 1) * 90) + CByte(50)
                    G = CByte((Math.Sin(Vertices(I).Z) + 1) * 90) + CByte(50)

                    Vertices(I).Color = Color.FromArgb(255, R, G, 0).ToArgb
                Next



                Me.VertexBuffer.SetData(Vertices, 0, LockFlags.None)
            End If

        End Sub











        'Protected Sub New(ByVal nHoofdObj As HoofdObject, ByVal nCenter As Vector2, ByVal nHalfSize As Vector2)
        'Protected Sub New(ByVal nCenter As Vector2, ByVal nSizeRadius As Single)
        '    'Me.setHoofdObj(nHoofdObj)
        '    Me.InitVars()

        '    'Me.CreateVertices(nCenter, nHalfSize)


        'End Sub

        'Public Sub New(ByVal nCD As CornerData, ByVal nParent As Square)
        '    'Me.setHoofdObj(nParent.HoofdObj)
        '    Me.InitVars()

        '    Me.setParent(nParent)
        '    'Me.setRootSquare(nParent.RootSquare)
        '    Me.setChildIndex(nCD.ChildIndex)
        '    Me.setLevel(Me.Parent.Level + CByte(1))

        '    'Dim HalfSize As Vector2 = New Vector2(nParent.Vertex(VertexDir.East).X - nParent.Vertex(VertexDir.Center).X, _
        '    '                            nParent.Vertex(VertexDir.North).Z - nParent.Vertex(VertexDir.Center).Z)
        '    'HalfSize *= 0.5

        '    'Dim Center As Vector2 = New Vector2(nParent.Vertex(VertexDir.Center).X, nParent.Vertex(VertexDir.Center).Z)
        '    'Select Case nChildIndex
        '    '    Case ChildDir.NorthEast
        '    '        Center += HalfSize
        '    '    Case ChildDir.NorthWest
        '    '        Center += New Vector2(-HalfSize.X, HalfSize.Y)
        '    '    Case ChildDir.SouthWest
        '    '        Center -= HalfSize
        '    '    Case ChildDir.SouthEast
        '    '        Center += New Vector2(HalfSize.X, -HalfSize.Y)
        '    '    Case Else
        '    '        Stop
        '    'End Select

        '    'Me.CreateVertices(Center, HalfSize)









        '    ' Set static to true if/when this node contains real data, and
        '    ' not just interpolated values.  When static == false, a node
        '    ' can be deleted by the Update() function if none of its
        '    ' vertices or children are enabled.
        '    Me.Static = False

        '    Dim IB As Byte
        '    For IB = 0 To 4 - 1
        '        Me.ChildByte(IB) = Nothing
        '    Next

        '    Me.EnabledFlags = 0

        '    For IB = 0 To 2 - 1
        '        Me.SubEnabledCount(IB) = 0
        '    Next


        '    '' Set default vertex positions by interpolating from given corners.
        '    '' Just bilinear interpolation.
        '    'Me.Height(HeightDir.Center) = 0.25F * (nCD.CornerHeight(CornerDir.NorthEast) + nCD.CornerHeight(CornerDir.NorthWest) + nCD.CornerHeight(CornerDir.SouthWest) + nCD.CornerHeight(CornerDir.SouthEast))
        '    'Me.Height(HeightDir.East) = 0.5F * (nCD.CornerHeight(CornerDir.SouthEast) + nCD.CornerHeight(CornerDir.NorthEast))
        '    'Me.Height(HeightDir.North) = 0.5F * (nCD.CornerHeight(CornerDir.NorthEast) + nCD.CornerHeight(CornerDir.NorthWest))
        '    'Me.Height(HeightDir.West) = 0.5F * (nCD.CornerHeight(CornerDir.NorthWest) + nCD.CornerHeight(CornerDir.SouthWest))
        '    'Me.Height(HeightDir.South) = 0.5F * (nCD.CornerHeight(CornerDir.SouthWest) + nCD.CornerHeight(CornerDir.SouthEast))

        '    'For IB = 0 To 2 - 1
        '    '    Me.ErrorByte(IB) = 0
        '    'Next
        '    'For IB = 0 To 4 - 1
        '    '    'TODO: Error[i+2] = fabs((Vertex[0].Y + pcd->Verts[i].Y) - (Vertex[i+1].Y + Vertex[((i+1)&3) + 1].Y)) * 0.25;
        '    '    'wat is de &3?
        '    '    Me.ErrorByte(IB + CByte(2)) = CSng(Math.Abs((Me.Height(HeightDir.Center) + Me.HeightByte(IB)) - (Me.HeightByte(IB + CByte(1)) + Me.HeightByte(((IB + CByte(1) And CByte(3)) + CByte(1))))) * 0.25)
        '    'Next


        '    '' Compute MinY/MaxY based on corner verts.
        '    'MinY = nCD.CornerHeight(CornerDir.NorthEast)
        '    'MaxY = nCD.CornerHeight(CornerDir.NorthEast)
        '    'For IB = 1 To 4 - 1
        '    '    Dim y As Single = nCD.CornerHeightByte(IB)
        '    '    If y < Me.MinY Then Me.MinY = y
        '    '    If y > Me.MaxY Then MaxY = y
        '    'Next



        'End Sub

        Protected Overridable Sub InitVars()
            Me.setChildren(New Square(4 - 1) {})
            'Me.setVertices(New Direct3D.CustomVertex.PositionColored(9 - 1) {})
            Me.setHeights(New Single(5 - 1) {})
            Me.setErrors(New Single(6 - 1) {})
            'Me.mEnabled = True

            'Me.Height(HeightDir.Center) = 5
            'Me.Height(HeightDir.South) = 2
            'Me.Height(HeightDir.East) = 5


        End Sub




        Private mChildren As Square()
        Public ReadOnly Property Children() As Square()
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mChildren
            End Get
        End Property
        Private Sub setChildren(ByVal value As Square())
            Me.mChildren = value
        End Sub


        Private mLevel As Byte
        Public ReadOnly Property Level() As Byte
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mLevel
            End Get
        End Property
        Private Sub setLevel(ByVal value As Byte)
            Me.mLevel = value
        End Sub



        Private mHeights As Single()
        Public ReadOnly Property HeightsArray() As Single()
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mHeights
            End Get
        End Property
        Private Sub setHeights(ByVal value As Single())
            Me.mHeights = value
        End Sub


        Public Sub OnDirty()
            Me.Dirty = True
            If Me.Parent IsNot Nothing Then Me.Parent.OnDirty()
        End Sub


        Public Property ChildByte(ByVal index As Byte) As Square
            Get
                Return Me.Children(index)
            End Get
            Set(ByVal value As Square)
                Me.Children(index) = value
            End Set
        End Property

        Public Property Child(ByVal index As ChildDir) As Square
            Get
                Return Me.Children(index)
            End Get
            Set(ByVal value As Square)
                Me.Children(index) = value
            End Set
        End Property


        Public Sub CreateChild(ByVal nDir As ChildDir, ByVal nPointer As BlockPointer) 'ByVal nCD As CornerData,
            If Me.Child(nDir) Is Nothing Then
                Me.Child(nDir) = New Square(Me, nPointer, nDir)
                If Me.RootSquare.DX.Ready Then Me.ChildByte(nDir).OnDeviceReset(Nothing, Nothing)
                Me.Child(nDir).LoadFromDisk()
            End If
        End Sub
        Public Sub CreateChild(ByVal nDir As Byte, ByVal nPointer As BlockPointer) 'ByVal nCD As CornerData,
            Me.CreateChild(CType(nDir, ChildDir), nPointer) 'nCD,
        End Sub


        'Public Overridable Sub Split(ByVal nCD As CornerData)
        '    For IB As Byte = 0 To 4 - 1
        '        Dim q As New CornerData(nCD, Me, IB)
        '        Me.Children(IB) = New Square(q, Me)
        '        Me.Children(IB).Static = True
        '    Next
        'End Sub


        Private mErrors As Single()
        Public ReadOnly Property Errors() As Single()
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mErrors
            End Get
        End Property
        Private Sub setErrors(ByVal value As Single())
            Me.mErrors = value
        End Sub


        Public Property [Error](ByVal index As ErrorType) As Single
            Get
                Return Me.Errors(index)
            End Get
            Set(ByVal value As Single)
                Me.Errors(index) = value
            End Set
        End Property

        Public Property ErrorByte(ByVal index As Byte) As Single
            Get
                Return Me.Errors(index)
            End Get
            Set(ByVal value As Single)
                Me.Errors(index) = value
            End Set
        End Property


        Public MinY, MaxY As Single 'Bounds for frustum culling and error testing.
        Public SubEnabledCount(1) As Byte  'e, s enabled reference counts.


        Private mDirty As Boolean
        Public Property Dirty() As Boolean
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mDirty
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
                Me.mDirty = value
            End Set
        End Property


        Private mStatic As Boolean
        Public Property [Static]() As Boolean
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mStatic
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
                Me.mStatic = value
            End Set
        End Property


        Private mChildIndex As Byte
        Public ReadOnly Property ChildIndex() As Byte
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mChildIndex
            End Get
        End Property
        Private Sub setChildIndex(ByVal value As Byte)
            Me.mChildIndex = value
        End Sub




        Private mHeight As Single
        Public Property Height(ByVal dir As HeightDir) As Single
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mHeight
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
                Me.mHeight = value
            End Set
        End Property

        Public Property HeightByte(ByVal index As Byte) As Single
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mHeight
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
                Me.mHeight = value
            End Set
        End Property

        'Public Function CalculateSize() As Vector2
        '    Return Me.RootSquare.Size * CSng(0.5 ^ Me.Level)
        'End Function

        'Public Overridable Function CreateCornerData() As CornerData
        '    Dim ParentCD As CornerData = Me.Parent.CreateCornerData
        '    Return New CornerData(ParentCD, Me.Parent, Me.ChildIndex)
        'End Function

        Shared Indices As Short() = New Short(24 - 1) {}

        Public Sub RenderAux(ByVal Textured As Boolean) ', ByVal vis As Clip.Visibility) 'ByVal cd As QuadCornerData,

            For IB As Byte = 0 To 4 - 1
                'If Me.ChildByte(IB) IsNot Nothing AndAlso (Me.EnabledFlags And 16 << IB) > 0 Then
                If (Me.EnabledFlags And 16 << IB) > 0 Then
                    'Dim q As New CornerData(cd, Me, IB)

                    'Me.ChildByte(IB).BuildFaces(FB, q)
                    Me.ChildByte(IB).RenderAux(Textured) ', vis) 'cd,

                End If
            Next

            If Me.VertexBuffer Is Nothing Then Exit Sub
            If Me.IndexBuffer Is Nothing Then Exit Sub


            Dim vCount As Short = 0

            If (Me.EnabledFlags And CByte(1)) = 0 Then
                Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.SouthEast, VertexDir.NorthEast)
            Else
                If (Me.EnabledFlags And CByte(2 ^ 7)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.SouthEast, VertexDir.East)
                If (Me.EnabledFlags And CByte(2 ^ 4)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.East, VertexDir.NorthEast)

            End If
            If (Me.EnabledFlags And CByte(2)) = 0 Then
                Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.NorthEast, VertexDir.NorthWest)
            Else
                If (Me.EnabledFlags And CByte(2 ^ 4)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.NorthEast, VertexDir.North)
                If (Me.EnabledFlags And CByte(2 ^ 5)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.North, VertexDir.NorthWest)

            End If
            If (Me.EnabledFlags And CByte(4)) = 0 Then
                Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.NorthWest, VertexDir.SouthWest)
            Else
                If (Me.EnabledFlags And CByte(2 ^ 5)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.NorthWest, VertexDir.West)
                If (Me.EnabledFlags And CByte(2 ^ 6)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.West, VertexDir.SouthWest)

            End If
            If (Me.EnabledFlags And CByte(8)) = 0 Then
                Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.SouthWest, VertexDir.SouthEast)
            Else
                If (Me.EnabledFlags And CByte(2 ^ 6)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.SouthWest, VertexDir.South)
                If (Me.EnabledFlags And CByte(2 ^ 7)) = 0 Then Me.AddFace(Indices, vCount, VertexDir.Center, VertexDir.South, VertexDir.SouthEast)

            End If





            Me.IndexBuffer.SetData(Indices, 0, LockFlags.None)

            Me.RootSquare.DX.Device.SetStreamSource(0, Me.VertexBuffer, 0)
            Me.RootSquare.DX.Device.Indices = Me.IndexBuffer



            'Me.RootSquare.DX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vCount , 0, CInt(vCount / 3))
            Me.RootSquare.DX.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 9, 0, CInt(vCount / 3))


        End Sub

        Public Sub AddFace(ByVal indices As Short(), ByRef vcount As Short, ByVal Dir1 As VertexDir, ByVal Dir2 As VertexDir, ByVal Dir3 As VertexDir)
            indices(vcount + 0) = Dir1
            indices(vcount + 1) = Dir2
            indices(vcount + 2) = Dir3
            vcount += 3S

        End Sub

        Private mEnabledFlags As Byte
        ''' <summary>
        ''' unsigned char 'bits 0-7: e, n, w, s, ne, nw, sw, se
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EnabledFlags() As Byte
            Get
                Return Me.mEnabledFlags
            End Get
            Set(ByVal value As Byte)
                Me.mEnabledFlags = value
            End Set
        End Property


        Public Sub Update(ByVal Viewer As Vector3) 'ByVal cd As CornerData) ', ByVal ViewerLocation As Vector3, ByVal Detail As Single)
            ' Refresh the vertex enabled states in the tree, according to the
            ' location of the viewer.  May force creation or deletion of qsquares
            ' in areas which need to be interpolated.

            'DetailThreshold = Detail * VERTICAL_SCALE

            'Dim nSingles As Single() = New Single(2) {ViewerLocation.X, ViewerLocation.Y, ViewerLocation.Z}
            Me.UpdateAux(0, Viewer) 'cd, 0)
            'Me.UpdateAux(cd, nSingles, 0)

        End Sub

        Public Sub UpdateAux(ByVal CenterError As Single, ByVal Viewer As Vector3) 'ByVal CD As CornerData,
            ' Does the actual work of updating enabled states and tree growing/shrinking.

            '' Make sure error values are current.
            'If Me.Dirty Then Me.RecomputeErrorAndLighting(CD)

            Dim half As Single = Me.SizeRadius ' = 1 << CD.Level
            Dim whole As Single = Me.SizeRadius * 2 'half << 1

            ' See about enabling child verts.
            If ((Me.EnabledFlags And 1) = 0 And Me.VertexTest(Me.Center.X + whole, Me.Height(HeightDir.East), Me.Center.Y + half, Me.Error(ErrorType.East), Viewer) = True) Then EnableEdgeVertex(0, False) ' East vert.
            If ((Me.EnabledFlags And 8) = 0 And Me.VertexTest(Me.Center.X + half, Me.Height(HeightDir.South), Me.Center.Y + whole, Me.Error(ErrorType.South), Viewer) = True) Then EnableEdgeVertex(3, False) ' South vert.



            '' '' ''If (CD.Level > 0) Then
            If ((EnabledFlags And 32) = 0) Then
                If (Me.BoxTest(Me.Center.X, Me.Center.Y, half, MinY, MaxY, Me.Error(ErrorType.ChildNorthWest), Viewer) = True) Then EnableChild(1) ' nw child.er
            End If
            If ((EnabledFlags And 16) = 0) Then
                If (Me.BoxTest(Me.Center.X + half, Me.Center.Y, half, MinY, MaxY, Me.Error(ErrorType.ChildNorthEast), Viewer) = True) Then EnableChild(0) ' ne child.
            End If
            If ((EnabledFlags And 64) = 0) Then
                If (Me.BoxTest(Me.Center.X, Me.Center.Y + half, half, MinY, MaxY, Me.Error(ErrorType.ChildSouthWest), Viewer) = True) Then EnableChild(2) ' sw child.
            End If
            If ((EnabledFlags And 128) = 0) Then
                If (Me.BoxTest(Me.Center.X + half, Me.Center.Y + half, half, MinY, MaxY, Me.Error(ErrorType.ChildSouthEast), Viewer) = True) Then EnableChild(3) ' se child.
            End If

            ' Recurse into child quadrants as necessary.
            'Dim q As CornerData

            If (Me.EnabledFlags And 32) > 0 Then
                'q = New CornerData(CD, Me, ChildDir.NorthWest)
                'q = Me.CreateCornerData(CD, 1)
                'SetupCornerData(q, CD, 1)
                Me.Child(ChildDir.NorthWest).UpdateAux(Me.Error(ErrorType.ChildNorthWest), Viewer)
            End If
            If (Me.EnabledFlags And 16) > 0 Then
                'q = Me.CreateCornerData(CD, 0)
                'q = New CornerData(CD, Me, ChildDir.NorthEast)
                Me.Child(ChildDir.NorthEast).UpdateAux(Me.Error(ErrorType.ChildNorthEast), Viewer)
            End If
            If (Me.EnabledFlags And 64) > 0 Then
                'q = Me.CreateCornerData(CD, 2)
                'q = New CornerData(CD, Me, ChildDir.SouthWest)
                Me.Child(ChildDir.SouthWest).UpdateAux(Me.Error(ErrorType.ChildSouthWest), Viewer)
            End If
            If (Me.EnabledFlags And 128) > 0 Then
                'q = New CornerData(CD, Me, ChildDir.SouthEast)
                'q = Me.CreateCornerData(CD, 3)
                Me.Child(ChildDir.SouthEast).UpdateAux(Me.Error(ErrorType.ChildSouthEast), Viewer)
            End If
            ''''''End If





            ' Test for disabling.  East, South, and center.
            If ((Me.EnabledFlags And 1) > 0 And SubEnabledCount(0) = 0 And VertexTest(Me.Center.X + whole, Me.Height(HeightDir.East), Me.Center.Y + half, Me.Error(ErrorType.East), Viewer) = False) Then
                Me.EnabledFlags = CByte(Me.EnabledFlags And Not 1)
                Dim s As Square = Me.GetNeighbour(0)
                If s IsNot Nothing Then s.EnabledFlags = CByte(s.EnabledFlags And Not 4)
            End If
            If ((Me.EnabledFlags And 8) > 0 And SubEnabledCount(1) = 0 And VertexTest(Me.Center.X + half, Me.Height(HeightDir.South), Me.Center.Y + whole, Me.Error(ErrorType.South), Viewer) = False) Then
                Me.EnabledFlags = CByte(Me.EnabledFlags And Not 8)
                Dim s As Square = Me.GetNeighbour(3)
                If s IsNot Nothing Then s.EnabledFlags = CByte(s.EnabledFlags And Not 2)
            End If
            If (Me.EnabledFlags = 0 And _
                Me.Parent IsNot Nothing And _
                BoxTest(Me.Center.X, Me.Center.Y, whole, MinY, MaxY, CenterError, Viewer) = False) Then

                ' Disable ourself.
                Me.Parent.NotifyChildDisable(Me.ChildIndex)   ' nb: possibly deletes 'this'.
            End If

        End Sub

        Private Sub EnableEdgeVertex(ByVal index As Byte, ByVal IncrementCount As Boolean) ', ByVal CD As CornerData)
            ' Enable the specified edge vertex.  Indices go { e, n, w, s }.
            ' Increments the appropriate reference-count if IncrementCount is true.

            If ((Me.EnabledFlags And (1 << index)) > 0 And IncrementCount = False) Then Exit Sub

            Static Dim Inc As Integer() = New Integer(3) {1, 0, 0, 8}

            ' Turn on flag and deal with reference count.
            Me.EnabledFlags = Me.EnabledFlags Or CByte(1 << index)
            If (IncrementCount = True And (index = 0 Or index = 3)) Then
                Me.SubEnabledCount(index And 1) += CByte(1)
            End If

            ' Now we need to enable the opposite edge vertex of the adjacent square (i.e. the alias vertex).

            ' This is a little tricky, since the desired neighbor node may not exist, in which
            ' case we have to create it, in order to prevent cracks.  Creating it may in turn cause
            ' further edge vertices to be enabled, propagating updates through the tree.

            ' The sticking point is the quadcornerdata list, which
            ' conceptually is just a linked list of activation structures.
            ' In this function, however, we will introduce branching into
            ' the "list", making it in actuality a tree.  This is all kind
            ' of obscure and hard to explain in words, but basically what
            ' it means is that our implementation has to be properly
            ' recursive.

            ' Travel upwards through the tree, looking for the parent in common with our desired neighbor.
            ' Remember the path through the tree, so we can travel down the complementary path to get to the neighbor.
            Dim p As Square = Me
            'Dim pcd As CornerData = CD
            Dim ct As Integer = 0
            Dim stack(31) As Byte 'Verplaatsen naar buiten de sub om de memory allocation te verminderen!
            Do
                'Dim ci As Byte = pcd.ChildIndex
                Dim ci As Byte = p.ChildIndex

                'If (pcd.Parent Is Nothing OrElse pcd.Parent.Square Is Nothing) Then
                If p.Parent Is Nothing Then
                    ' Neighbor doesn't exist (it's outside the tree), so there's no alias vertex to enable.
                    Exit Sub
                End If
                p = p.Parent 'pcd.Parent.Square
                'pcd = pcd.Parent

                Dim SameParent As Boolean = CBool(IIf(((CInt(index) - CInt(ci)) And 2) > 0, True, False))

                ci = (ci Xor CByte(1) Xor ((index And CByte(1)) << CByte(1)))  ' Child index of neighbor node.

                stack(ct) = ci
                ct += 1

                If SameParent Then Exit Do
            Loop

            ' Get a pointer to our neighbor (create if necessary), by walking down
            ' the quadtree from our shared ancestor.
            p = p.EnableDescendant(ct, stack) ' pcd)

            '/*
            '        ' Travel down the tree towards our neighbor, enabling and creating nodes as necessary.  We'll
            '        ' follow the complement of the path we used on the way up the tree.
            '	quadcornerdata	d(16);
            '	int	i;
            '	for (i = 0; i < ct; i++) {
            '		int	ci = stack(ct-i-1);

            '		if (p.Child(ci) == NULL && CreateDepth == 0) CreateDepth = ct-i;	'xxxxxxx

            '		if ((p.EnabledFlags & (16 << ci)) == 0) {
            '			p.EnableChild(ci, *pcd);
            '		}
            '		p.SetupCornerData(&d(i), *pcd, ci);
            '		p = p.Child(ci);
            '		pcd = &d(i);
            '	}
            '*/

            ' Finally: enable the vertex on the opposite edge of our neighbor, the alias of the original vertex.


            'Dim MeHeight As Single = Me.HeightByte(index + CByte(1))


            index = index Xor CByte(2)
            p.EnabledFlags = CByte(p.EnabledFlags Or (1 << index))

            'p.HeightByte(index + CByte(1)) = MeHeight

            If (IncrementCount = True And (index = 0 Or index = 3)) Then
                p.SubEnabledCount(index And 1) += CByte(1)
            End If

        End Sub
        Public Function EnableDescendant(ByVal count As Integer, ByVal path As Byte()) As Square ', ByVal cd As CornerData
            ' This function enables the descendant node 'count' generations below
            ' us, located by following the list of child indices in path().
            ' Creates the node if necessary, and returns a pointer to it.

            count -= 1
            Dim ChildIndex As Byte = path(count)

            If ((EnabledFlags And (16 << ChildIndex)) = 0) Then
                Me.EnableChild(ChildIndex) ', cd)
            End If

            If (count > 0) Then
                'Dim q As CornerData = New CornerData(cd, Me, ChildIndex)
                Return Me.ChildByte(ChildIndex).EnableDescendant(count, path) ', q)
            Else
                Return Me.ChildByte(ChildIndex)
            End If

        End Function

        Public Sub EnableChildNoCreate(ByVal index As Byte) ', ByVal cd As CornerData)
            ' Enable the indexed child node.  { ne, nw, sw, se }
            ' Causes dependent edge vertices to be enabled.

            If Me.ChildByte(index) Is Nothing Then Exit Sub

            If ((EnabledFlags And (16 << index)) = 0) Then
                'Enabled(index + 4) = true;
                Me.EnabledFlags = CByte(Me.EnabledFlags Or (16 << index))
                EnableEdgeVertex(index, True) ', cd)
                EnableEdgeVertex((index + CByte(1)) And CByte(3), True) ', cd)

            End If

        End Sub

        Public Sub EnableChild(ByVal index As Byte) ', ByVal cd As CornerData)
            ' Enable the indexed child node.  { ne, nw, sw, se }
            ' Causes dependent edge vertices to be enabled.

            'if (Enabled(index + 4) == false) {


            If ((EnabledFlags And (16 << index)) = 0) Then
                Dim B As PrecompBlock = Me.LoadPrecompBlock
                If B.ChildPointerByte(index).IsEmpty Then Exit Sub

                'Enabled(index + 4) = true;
                Me.EnabledFlags = CByte(Me.EnabledFlags Or (16 << index))
                Me.EnableEdgeVertex(index, True) ', cd)
                Me.EnableEdgeVertex((index + CByte(1)) And CByte(3), True) ', cd)

                If Me.ChildByte(index) Is Nothing Then
                    'Me.CreateChild(cd, index)

                    Me.CreateChild(index, B.ChildPointerByte(index))
                End If
            End If

        End Sub
        Public Sub NotifyChildDisable(ByVal index As Byte) 'ByVal cd As CornerData,
            ' Marks the indexed child quadrant as disabled.  Deletes the child node
            ' if it isn't static.

            ' Clear enabled flag for the child.
            Me.EnabledFlags = CByte(Me.EnabledFlags And Not (16 << index))

            ' Update child enabled counts for the affected edge verts.
            Dim s As Square

            If (index And 2) > 0 Then
                s = Me
            Else
                s = Me.GetNeighbour(1) ', cd)
            End If
            If s IsNot Nothing Then
                s.SubEnabledCount(1) -= CByte(1)
            End If

            If (index = 1 Or index = 2) Then
                s = Me.GetNeighbour(2) ', cd)
            Else
                s = Me
            End If
            If s IsNot Nothing Then
                s.SubEnabledCount(0) -= CByte(1)
            End If

            If (Me.ChildByte(index).Static = False) Then
                'delete Child(index);
                Me.ChildByte(index).Dispose()
                Me.ChildByte(index) = Nothing
                'Child(index) = 0;

                'Me.BlockDeleteCount += 1 'xxxxx
            End If

        End Sub

        Public Function GetNeighbour(ByVal dir As Byte) As Square ', ByVal cd As CornerData
            ' Traverses the tree in search of the quadsquare neighboring this square to the
            ' specified direction.  0-3 --> { E, N, W, S }.
            ' Returns NULL if the neighbor is outside the bounds of the tree.



            ' If we don't have a parent, then we don't have a neighbor.
            ' (Actually, we could have inter-tree connectivity at this level
            ' for connecting separate trees together.)
            'If cd.Parent Is Nothing Then Return Nothing
            If Me.Parent Is Nothing Then Return Nothing

            ' Find the parent and the child-index of the square we want to locate or create.
            Dim P As Square = Nothing

            'Dim index As Byte = cd.ChildIndex Xor CByte(1) Xor ((dir And CByte(1)) << CByte(1))
            'Dim SameParent As Boolean = CBool(IIf(((CInt(dir) - CInt(cd.ChildIndex)) And 2) > 0, True, False))
            Dim index As Byte = Me.ChildIndex Xor CByte(1) Xor ((dir And CByte(1)) << CByte(1))
            Dim SameParent As Boolean = CBool(IIf(((CInt(dir) - CInt(Me.ChildIndex)) And 2) > 0, True, False))

            If (SameParent) Then
                'P = cd.Parent.Square
                P = Me.Parent
            Else
                'P = cd.Parent.Square.GetNeighbour(dir, cd.Parent)
                P = Me.Parent.GetNeighbour(dir)

                If (P Is Nothing) Then Return Nothing
            End If

            Dim n As Square = P.ChildByte(index)

            Return n

        End Function


        'Public Function RecomputeErrorAndLighting(ByVal CD As CornerData) As Single
        '    '' Recomputes the error values for this tree.  Returns the
        '    '' max error.
        '    '' Also updates MinY & MaxY.

        '    '' Measure error of center and edge vertices.
        '    'Dim maxError As Single = 0

        '    '' Compute error of center vert.
        '    'Dim e As Single
        '    'If (CD.ChildIndex And 1) > 0 Then
        '    '    e = CSng(Math.Abs(Me.Height(HeightDir.Center) - (CD.CornerHeight(CornerDir.NorthWest) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
        '    'Else
        '    '    e = CSng(Math.Abs(Me.Height(HeightDir.Center) - (CD.CornerHeight(CornerDir.NorthEast) + CD.CornerHeight(CornerDir.SouthWest)) * 0.5))
        '    'End If
        '    'If (e > maxError) Then maxError = e

        '    '' Initial min/max.
        '    'MaxY = Me.Height(HeightDir.Center)
        '    'MinY = Me.Height(HeightDir.Center)

        '    '' Check min/max of corners.
        '    'For IB As Byte = 0 To 4 - 1
        '    '    Dim Y As Single = CD.CornerHeightByte(IB)
        '    '    If (Y < MinY) Then MinY = Y
        '    '    If (Y > MaxY) Then MaxY = Y
        '    'Next

        '    '' Edge verts.
        '    'e = CSng(Math.Abs(Me.Height(HeightDir.East) - (CD.CornerHeight(CornerDir.NorthEast) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
        '    'If (e > maxError) Then maxError = e
        '    'Me.Error(ErrorType.East) = e

        '    'e = CSng(Math.Abs(Me.Height(HeightDir.South) - (CD.CornerHeight(CornerDir.SouthWest) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
        '    'If (e > maxError) Then maxError = e
        '    'Me.Error(ErrorType.South) = e

        '    '' Min/max of edge verts.
        '    'For IB As Byte = 0 To 4 - 1
        '    '    Dim y As Single = Me.HeightByte(CByte(1) + IB)
        '    '    If (y < MinY) Then MinY = y
        '    '    If (y > MaxY) Then MaxY = y
        '    'Next


        '    'Dim q As CornerData
        '    '' Check child squares.
        '    'For IB As Byte = 0 To 4 - 1
        '    '    If Me.ChildByte(IB) IsNot Nothing Then
        '    '        q = New CornerData(CD, Me, IB)
        '    '        Me.ErrorByte(IB + CByte(2)) = Me.ChildByte(IB).RecomputeErrorAndLighting(q) 'CUShort(Me.Child(I).RecomputeErrorAndLighting(q))

        '    '        If (Me.ChildByte(IB).MinY < MinY) Then MinY = Me.ChildByte(IB).MinY
        '    '        If (Me.ChildByte(IB).MaxY > MaxY) Then MaxY = Me.ChildByte(IB).MaxY
        '    '    Else
        '    '        ' Compute difference between bilinear average at child center, and diagonal edge approximation.

        '    '        'TODO: deze regel snap ik niet en lijkt vreemd

        '    '        Me.ErrorByte(IB + CByte(2)) = CSng(Math.Abs((Me.Height(HeightDir.Center) + CD.CornerHeightByte(IB)) - (Me.HeightByte(IB + CByte(1)) + Me.HeightByte(((IB + CByte(1)) And CByte(3)) + CByte(1)))) * 0.25)
        '    '    End If
        '    '    If (Me.ErrorByte(IB + CByte(2)) > maxError) Then maxError = Me.ErrorByte(IB + CByte(2))
        '    'Next



        '    '' The error, MinY/MaxY, and lighting values for this node and descendants are correct now.
        '    'Dirty = False

        '    'Return maxError
        'End Function



        ''' <summary>
        ''' returned true als de vertex niet op zijn geinterpoleerde positie ligt.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <param name="error"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VertexTest(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal [error] As Single, ByVal Viewer As Vector3) As Boolean
            ' Returns true if the vertex at (x,z) with the given world-space error between
            ' its interpolated location and its true location, should be enabled, given that
            ' the viewpoint is located at Viewer().

            Dim dx As Single = CSng(Math.Abs(x - Viewer.X))
            Dim dy As Single = CSng(Math.Abs(y - Viewer.Y))
            Dim dz As Single = CSng(Math.Abs(z - Viewer.Z))
            Dim d As Single = dx
            If (dy > d) Then d = dy
            If (dz > d) Then d = dz


            Return ([error] * Me.RootSquare.DetailThreshold) > d

        End Function

        Public Function BoxTest(ByVal x As Single, ByVal z As Single, ByVal size As Single, ByVal minY As Single, ByVal maxY As Single, ByVal [error] As Single, ByVal Viewer As Vector3) As Boolean
            ' Returns true if any vertex within the specified box (origin at x,z,
            ' edges of length size) with the given error value could be enabled
            ' based on the given viewer location.

            ' Find the minimum distance to the box.
            Dim half As Single = size * 0.5F
            Dim dx As Single = CSng(Math.Abs(x + half - Viewer.X) - half)
            Dim dy As Single = CSng(Math.Abs((minY + maxY) * 0.5F - Viewer.Y) - (maxY - minY) * 0.5F)
            Dim dz As Single = CSng(Math.Abs(z + half - Viewer.Z) - half)
            Dim d As Single = dx
            If (dy > d) Then d = dy
            If (dz > d) Then d = dz


            Return ([error] * Me.RootSquare.DetailThreshold) > d
        End Function


        'Public Sub AddHeightAtPos(ByVal CD As CornerData, ByVal nPositie As Vector2, ByVal Range As Single, ByVal value As Single)
        '    Dim HalveDiagonaal As Single = Vector2.Length(New Vector2(CD.SizeRadius, CD.SizeRadius))

        '    'Cirkel VS Cirkel Collision Check

        '    Dim Dist As Single = Math.Abs(Vector2.Length(nPositie - CD.Center))
        '    If Dist > Range + HalveDiagonaal + 0.001F Then Exit Sub 'Buiten bereik


        '    Dim FB As New FaceBuilder
        '    FB.StartBuilding(CD.Center, New Vector2(CD.SizeRadius, CD.SizeRadius), Me.HeightsArray, CD.CornerHeights)

        '    For IDir As VertexDir = VertexDir.Center To VertexDir.South
        '        Dim V As Vector3 = FB.BuildVertex(IDir)
        '        Dim VFlat As Vector2 = New Vector2(V.X, V.Z)
        '        Dist = Math.Abs(Vector2.Length(nPositie - VFlat))
        '        If Dist < Range + 0.001F Then
        '            Dim Influence As Single
        '            If Dist = 0 Then
        '                Influence = 1
        '            Else
        '                Influence = 1 - (Dist / Range)
        '            End If
        '            Me.Height(CType(IDir, HeightDir)) += value * Influence

        '        End If
        '        'Binnen bereik

        '    Next




        '    FB.EndBuilding()

        '    For IB As Byte = 0 To 4 - 1
        '        If Me.ChildByte(IB) IsNot Nothing Then
        '            Me.ChildByte(IB).AddHeightAtPos(New CornerData(CD, Me, IB), nPositie, Range, value)
        '        End If
        '    Next

        'End Sub

        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free unmanaged resources when explicitly called
                End If

                ' TODO: free shared unmanaged resources
            End If
            Me.disposedValue = True


            If Me.VertexBuffer IsNot Nothing Then
                Me.VertexBuffer.Dispose()
                Me.setVertexBuffer(Nothing)
            End If
            If Me.IndexBuffer IsNot Nothing Then
                Me.IndexBuffer.Dispose()
                Me.setIndexBuffer(Nothing)
            End If

            For IB As Byte = 0 To 4 - 1
                If Me.ChildByte(IB) IsNot Nothing Then Me.ChildByte(IB).Dispose()
            Next
        End Sub

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace
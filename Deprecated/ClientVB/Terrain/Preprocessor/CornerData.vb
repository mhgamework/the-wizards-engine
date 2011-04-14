Imports MHGameWork.TheWizards.Common.Terrain
Namespace Terrain.Preprocessor
    Public Class CornerData
        Private Sub New()
            Me.setCornerHeights(New Single(4 - 1) {})

        End Sub
        Public Sub New(ByVal nParentCD As CornerData, ByVal nParentSquareBlock As SquareBlock, ByVal nDir As Byte)
            Me.New(nParentCD, nParentSquareBlock, CType(nDir, ChildDir))
        End Sub

        Public Sub New(ByVal nParentCD As CornerData, ByVal nParentSquareBlock As SquareBlock, ByVal nDir As ChildDir)
            Me.New()
            ' Fills the given structure with the appropriate corner values for the
            ' specified child block, given our own vertex data and our corner
            ' vertex data from cd.
            '
            ' ChildIndex mapping:
            ' +-+-+
            ' |1|0|
            ' +-+-+
            ' |2|3|
            ' +-+-+
            '
            ' Verts mapping:
            ' 1-0
            ' | |
            ' 2-3
            '
            ' Vertex mapping:
            ' +-2-+
            ' | | |
            ' 3-0-1
            ' | | |
            ' +-4-+

            'Me.Parent = nParentCD
            Me.SizeRadius = nParentCD.SizeRadius * 0.5F
            Me.Center = nParentCD.Center
            Me.ChildIndex = nDir
            'Me.Square = nParentSquareBlock.Child(nDir)


            Dim HalfSize As Vector2 = New Vector2(Me.SizeRadius, Me.SizeRadius)

            Select Case CType(nDir, ChildDir)
                Case ChildDir.NorthEast
                    Me.CornerHeight(CornerDir.NorthEast) = nParentCD.CornerHeight(CornerDir.NorthEast)
                    Me.CornerHeight(CornerDir.NorthWest) = nParentSquareBlock.Height(HeightDir.North)
                    Me.CornerHeight(CornerDir.SouthWest) = nParentSquareBlock.Height(HeightDir.Center)
                    Me.CornerHeight(CornerDir.SouthEast) = nParentSquareBlock.Height(HeightDir.East)

                    Center += HalfSize

                Case ChildDir.NorthWest
                    Me.CornerHeight(CornerDir.NorthEast) = nParentSquareBlock.Height(HeightDir.North)
                    Me.CornerHeight(CornerDir.NorthWest) = nParentCD.CornerHeight(CornerDir.NorthWest)
                    Me.CornerHeight(CornerDir.SouthWest) = nParentSquareBlock.Height(HeightDir.West)
                    Me.CornerHeight(CornerDir.SouthEast) = nParentSquareBlock.Height(HeightDir.Center)

                    Center += New Vector2(-HalfSize.X, HalfSize.Y)

                Case ChildDir.SouthWest
                    Me.CornerHeight(CornerDir.NorthEast) = nParentSquareBlock.Height(HeightDir.Center)
                    Me.CornerHeight(CornerDir.NorthWest) = nParentSquareBlock.Height(HeightDir.West)
                    Me.CornerHeight(CornerDir.SouthWest) = nParentCD.CornerHeight(CornerDir.SouthWest)
                    Me.CornerHeight(CornerDir.SouthEast) = nParentSquareBlock.Height(HeightDir.South)

                    Center -= HalfSize

                Case ChildDir.SouthEast
                    Me.CornerHeight(CornerDir.NorthEast) = nParentSquareBlock.Height(HeightDir.East)
                    Me.CornerHeight(CornerDir.NorthWest) = nParentSquareBlock.Height(HeightDir.Center)
                    Me.CornerHeight(CornerDir.SouthWest) = nParentSquareBlock.Height(HeightDir.South)
                    Me.CornerHeight(CornerDir.SouthEast) = nParentCD.CornerHeight(CornerDir.SouthEast)

                    Center += New Vector2(HalfSize.X, -HalfSize.Y)

                Case Else
                    Stop
                    'TODO: eigenlijk hetzelfde als case 0?
                    'q.XOrg = cd.XOrg + half
                    'q.ZOrg = cd.ZOrg
                    'q.Verts(0) = cd.Verts(0)
                    'q.Verts(1) = Vertex(2)
                    'q.Verts(2) = Vertex(0)
                    'q.Verts(3) = Vertex(1)
            End Select


        End Sub

        Public Sub New(ByVal nCenter As Vector2, ByVal nSizeRadius As Single) ', ByVal nSquare As Square)
            Me.New()
            Me.Center = nCenter
            Me.SizeRadius = nSizeRadius
            'Me.Square = nSquare
        End Sub

        Private mCornerHeights As Single()
        Public ReadOnly Property CornerHeights() As Single()
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCornerHeights
            End Get
        End Property
        Private Sub setCornerHeights(ByVal value As Single())
            Me.mCornerHeights = value
        End Sub



        Public Property CornerHeight(ByVal Dir As CornerDir) As Single
            Get
                Return Me.CornerHeights(Dir)
            End Get
            Set(ByVal value As Single)
                Me.CornerHeights(Dir) = value
            End Set
        End Property

        Public ReadOnly Property CornerHeightByte(ByVal Dir As Byte) As Single
            Get
                Return Me.mCornerHeights(Dir)
            End Get
        End Property




        Private mSizeRadius As Single
        Public Property SizeRadius() As Single
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mSizeRadius
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
                Me.mSizeRadius = value
            End Set
        End Property



        Private mCenter As Vector2
        Public Property Center() As Vector2
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCenter
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector2)
                Me.mCenter = value
            End Set
        End Property


        'Private mParent As CornerData
        'Public Property Parent() As CornerData
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mParent
        '    End Get
        '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As CornerData)
        '        Me.mParent = value
        '    End Set
        'End Property



        Private mChildIndex As Byte
        Public Property ChildIndex() As Byte
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mChildIndex
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Byte)
                Me.mChildIndex = value
            End Set
        End Property


        'Private mSquare As Square
        'Public Property Square() As Square
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mSquare
        '    End Get
        '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Square)
        '        Me.mSquare = value
        '    End Set
        'End Property




    End Class
End Namespace
Imports Microsoft.DirectX.Direct3D.CustomVertex
Imports MHGameWork.TheWizards.Common.Terrain
Namespace Terrain.Preprocessor
    Public Class Square

        ''Protected Sub New(ByVal nHoofdObj As HoofdObject, ByVal nCenter As Vector2, ByVal nHalfSize As Vector2)
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
        '    'Me.setLevel(Me.Parent.Level + CByte(1))

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









        '    '' Set static to true if/when this node contains real data, and
        '    '' not just interpolated values.  When static == false, a node
        '    '' can be deleted by the Update() function if none of its
        '    '' vertices or children are enabled.
        '    'Me.Static = False

        '    'Dim IB As Byte
        '    'For IB = 0 To 4 - 1
        '    '    Me.ChildByte(IB) = Nothing
        '    'Next

        '    'Me.EnabledFlags = 0

        '    'For IB = 0 To 2 - 1
        '    '    Me.SubEnabledCount(IB) = 0
        '    'Next


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

        'Protected Overridable Sub InitVars()
        '    Me.setChildren(New Square(4 - 1) {})
        '    'Me.setVertices(New Direct3D.CustomVertex.PositionColored(9 - 1) {})
        '    'Me.setHeights(New Single(5 - 1) {})
        '    'Me.setErrors(New Single(6 - 1) {})
        '    'Me.mEnabled = True

        '    'Me.Height(HeightDir.Center) = 5
        '    'Me.Height(HeightDir.South) = 2
        '    'Me.Height(HeightDir.East) = 5


        'End Sub

        'Private mParent As Square
        'Public ReadOnly Property Parent() As Square
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mParent
        '    End Get
        'End Property
        'Private Sub setParent(ByVal value As Square)
        '    Me.mParent = value
        'End Sub


        ''Private mRootSquare As RootSquare
        ''Public ReadOnly Property RootSquare() As RootSquare
        ''    <System.Diagnostics.DebuggerStepThrough()> Get
        ''        Return Me.mRootSquare
        ''    End Get
        ''End Property
        ''Private Sub setRootSquare(ByVal value As RootSquare)
        ''    Me.mRootSquare = value
        ''End Sub


        'Private mChildren As Square()
        'Public ReadOnly Property Children() As Square()
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mChildren
        '    End Get
        'End Property
        'Private Sub setChildren(ByVal value As Square())
        '    Me.mChildren = value
        'End Sub


        'Private mLevel As Byte
        'Public ReadOnly Property Level() As Byte
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mLevel
        '    End Get
        'End Property
        'Private Sub setLevel(ByVal value As Byte)
        '    Me.mLevel = value
        'End Sub



        'Private mHeights As Single()
        'Public ReadOnly Property HeightsArray() As Single()
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mHeights
        '    End Get
        'End Property
        'Private Sub setHeights(ByVal value As Single())
        '    Me.mHeights = value
        'End Sub


        'Public Property HeightByte(ByVal index As Byte) As Single
        '    Get
        '        Return Me.HeightsArray(index)
        '    End Get
        '    Set(ByVal value As Single)
        '        Me.HeightsArray(index) = value
        '        Me.OnDirty()
        '    End Set
        'End Property


        'Public Property Height(ByVal index As HeightDir) As Single
        '    Get
        '        Return Me.HeightsArray(index)
        '    End Get
        '    Set(ByVal value As Single)
        '        Me.HeightsArray(index) = value
        '        Me.OnDirty()
        '    End Set
        'End Property

        'Public Sub OnDirty()
        '    Me.Dirty = True
        '    If Me.Parent IsNot Nothing Then Me.Parent.OnDirty()
        'End Sub


        'Public Property ChildByte(ByVal index As Byte) As Square
        '    Get
        '        Return Me.Children(index)
        '    End Get
        '    Set(ByVal value As Square)
        '        Me.Children(index) = value
        '    End Set
        'End Property

        'Public Property Child(ByVal index As ChildDir) As Square
        '    Get
        '        Return Me.Children(index)
        '    End Get
        '    Set(ByVal value As Square)
        '        Me.Children(index) = value
        '    End Set
        'End Property


        'Public Sub CreateChild(ByVal nCD As CornerData, ByVal nDir As ChildDir)
        '    If Me.Child(nDir) Is Nothing Then
        '        Dim Q As New CornerData(nCD, Me, nDir)
        '        Me.Child(nDir) = New Square(Q, Me)

        '    End If
        'End Sub
        'Public Sub CreateChild(ByVal nCD As CornerData, ByVal nDir As Byte)
        '    Me.CreateChild(nCD, CType(nDir, ChildDir))
        'End Sub

        'Public Sub ResetTree()
        '    ' Clear all enabled flags, and delete all non-static child nodes.

        '    For IB As Byte = 0 To 4 - 1
        '        If Me.ChildByte(IB) IsNot Nothing Then
        '            ChildByte(IB).ResetTree()
        '            If (ChildByte(IB).Static = False) Then
        '                'delete Child(i);
        '                Me.ChildByte(IB) = Nothing
        '            End If
        '        End If
        '    Next IB
        '    Me.EnabledFlags = 0
        '    Me.SubEnabledCount(0) = 0
        '    Me.SubEnabledCount(1) = 0
        '    Dirty = True
        'End Sub


        'Public Overridable Sub Split(ByVal nCD As CornerData)
        '    For IB As Byte = 0 To 4 - 1
        '        Dim q As New CornerData(nCD, Me, IB)
        '        Me.Children(IB) = New Square(q, Me)
        '        Me.Children(IB).Static = True
        '    Next
        'End Sub


        'Private mErrors As Single()
        'Public ReadOnly Property Errors() As Single()
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mErrors
        '    End Get
        'End Property
        'Private Sub setErrors(ByVal value As Single())
        '    Me.mErrors = value
        'End Sub


        'Public Property [Error](ByVal index As ErrorType) As Single
        '    Get
        '        Return Me.Errors(index)
        '    End Get
        '    Set(ByVal value As Single)
        '        Me.Errors(index) = value
        '    End Set
        'End Property

        'Public Property ErrorByte(ByVal index As Byte) As Single
        '    Get
        '        Return Me.Errors(index)
        '    End Get
        '    Set(ByVal value As Single)
        '        Me.Errors(index) = value
        '    End Set
        'End Property


        'Public MinY, MaxY As Single 'Bounds for frustum culling and error testing.
        'Public SubEnabledCount(1) As Byte  'e, s enabled reference counts.


        'Private mDirty As Boolean
        'Public Property Dirty() As Boolean
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mDirty
        '    End Get
        '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
        '        Me.mDirty = value
        '    End Set
        'End Property


        'Private mStatic As Boolean
        'Public Property [Static]() As Boolean
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mStatic
        '    End Get
        '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
        '        Me.mStatic = value
        '    End Set
        'End Property


        'Private mChildIndex As Byte
        'Public ReadOnly Property ChildIndex() As Byte
        '    <System.Diagnostics.DebuggerStepThrough()> Get
        '        Return Me.mChildIndex
        '    End Get
        'End Property
        'Private Sub setChildIndex(ByVal value As Byte)
        '    Me.mChildIndex = value
        'End Sub



        ''Public Function CalculateSize() As Vector2
        ''    Return Me.RootSquare.Size * CSng(0.5 ^ Me.Level)
        ''End Function

        'Public Overridable Function CreateCornerData() As CornerData
        '    Dim ParentCD As CornerData = Me.Parent.CreateCornerData
        '    Return New CornerData(ParentCD, Me.Parent, Me.ChildIndex)
        'End Function



        ''Private mEnabledFlags As Byte
        ''''' <summary>
        ''''' unsigned char 'bits 0-7: e, n, w, s, ne, nw, sw, se
        ''''' </summary>
        ''''' <value></value>
        ''''' <returns></returns>
        ''''' <remarks></remarks>
        ''Public Property EnabledFlags() As Byte
        ''    Get
        ''        Return Me.mEnabledFlags
        ''    End Get
        ''    Set(ByVal value As Byte)
        ''        Me.mEnabledFlags = value
        ''    End Set
        ''End Property


        ''Public Sub Update(ByVal cd As CornerData) ', ByVal ViewerLocation As Vector3, ByVal Detail As Single)
        ''    ' Refresh the vertex enabled states in the tree, according to the
        ''    ' location of the viewer.  May force creation or deletion of qsquares
        ''    ' in areas which need to be interpolated.

        ''    'DetailThreshold = Detail * VERTICAL_SCALE

        ''    'Dim nSingles As Single() = New Single(2) {ViewerLocation.X, ViewerLocation.Y, ViewerLocation.Z}
        ''    Me.UpdateAux(cd, 0)
        ''    'Me.UpdateAux(cd, nSingles, 0)

        ''End Sub

        ''Public Sub UpdateAux(ByVal CD As CornerData, ByVal CenterError As Single)
        ''    ' Does the actual work of updating enabled states and tree growing/shrinking.

        ''    ' Make sure error values are current.
        ''    If Me.Dirty Then Me.RecomputeErrorAndLighting(CD)

        ''    Dim half As Single = CD.SizeRadius ' = 1 << CD.Level
        ''    Dim whole As Single = CD.SizeRadius * 2 'half << 1

        ''    ' See about enabling child verts.
        ''    If ((Me.EnabledFlags And 1) = 0 And Me.VertexTest(CD.Center.X + whole, Me.Height(HeightDir.East), CD.Center.Y + half, Me.Error(ErrorType.East)) = True) Then EnableEdgeVertex(0, False, CD) ' East vert.
        ''    If ((Me.EnabledFlags And 8) = 0 And Me.VertexTest(CD.Center.X + half, Me.Height(HeightDir.South), CD.Center.Y + whole, Me.Error(ErrorType.South)) = True) Then EnableEdgeVertex(3, False, CD) ' South vert.



        ''    '' '' ''If (CD.Level > 0) Then
        ''    If ((EnabledFlags And 32) = 0) Then
        ''        If (Me.BoxTest(CD.Center.X, CD.Center.Y, half, MinY, MaxY, Me.Error(ErrorType.ChildNorthWest)) = True) Then EnableChildNoCreate(1, CD) ' nw child.er
        ''    End If
        ''    If ((EnabledFlags And 16) = 0) Then
        ''        If (Me.BoxTest(CD.Center.X + half, CD.Center.Y, half, MinY, MaxY, Me.Error(ErrorType.ChildNorthEast)) = True) Then EnableChildNoCreate(0, CD) ' ne child.
        ''    End If
        ''    If ((EnabledFlags And 64) = 0) Then
        ''        If (Me.BoxTest(CD.Center.X, CD.Center.Y + half, half, MinY, MaxY, Me.Error(ErrorType.ChildSouthWest)) = True) Then EnableChildNoCreate(2, CD) ' sw child.
        ''    End If
        ''    If ((EnabledFlags And 128) = 0) Then
        ''        If (Me.BoxTest(CD.Center.X + half, CD.Center.Y + half, half, MinY, MaxY, Me.Error(ErrorType.ChildSouthEast)) = True) Then EnableChildNoCreate(3, CD) ' se child.
        ''    End If

        ''    ' Recurse into child quadrants as necessary.
        ''    Dim q As CornerData

        ''    If (Me.EnabledFlags And 32) > 0 Then
        ''        q = New CornerData(CD, Me, ChildDir.NorthWest)
        ''        'q = Me.CreateCornerData(CD, 1)
        ''        'SetupCornerData(q, CD, 1)
        ''        Me.Child(ChildDir.NorthWest).UpdateAux(q, Me.Error(ErrorType.ChildNorthWest))
        ''    End If
        ''    If (Me.EnabledFlags And 16) > 0 Then
        ''        'q = Me.CreateCornerData(CD, 0)
        ''        q = New CornerData(CD, Me, ChildDir.NorthEast)
        ''        Me.Child(ChildDir.NorthEast).UpdateAux(q, Me.Error(ErrorType.ChildNorthEast))
        ''    End If
        ''    If (Me.EnabledFlags And 64) > 0 Then
        ''        'q = Me.CreateCornerData(CD, 2)
        ''        q = New CornerData(CD, Me, ChildDir.SouthWest)
        ''        Me.Child(ChildDir.SouthWest).UpdateAux(q, Me.Error(ErrorType.ChildSouthWest))
        ''    End If
        ''    If (Me.EnabledFlags And 128) > 0 Then
        ''        q = New CornerData(CD, Me, ChildDir.SouthEast)
        ''        'q = Me.CreateCornerData(CD, 3)
        ''        Me.Child(ChildDir.SouthEast).UpdateAux(q, Me.Error(ErrorType.ChildSouthEast))
        ''    End If
        ''    ''''''End If





        ''    ' Test for disabling.  East, South, and center.
        ''    If ((Me.EnabledFlags And 1) > 0 And SubEnabledCount(0) = 0 And VertexTest(CD.Center.X + whole, Me.Height(HeightDir.East), CD.Center.Y + half, Me.Error(ErrorType.East)) = False) Then
        ''        Me.EnabledFlags = CByte(Me.EnabledFlags And Not 1)
        ''        Dim s As Square = Me.GetNeighbour(0, CD)
        ''        If s IsNot Nothing Then s.EnabledFlags = CByte(s.EnabledFlags And Not 4)
        ''    End If
        ''    If ((Me.EnabledFlags And 8) > 0 And SubEnabledCount(1) = 0 And VertexTest(CD.Center.X + half, Me.Height(HeightDir.South), CD.Center.Y + whole, Me.Error(ErrorType.South)) = False) Then
        ''        Me.EnabledFlags = CByte(Me.EnabledFlags And Not 8)
        ''        Dim s As Square = Me.GetNeighbour(3, CD)
        ''        If s IsNot Nothing Then s.EnabledFlags = CByte(s.EnabledFlags And Not 2)
        ''    End If
        ''    If (Me.EnabledFlags = 0 And _
        ''        Me.Parent IsNot Nothing And _
        ''        BoxTest(CD.Center.X, CD.Center.Y, whole, MinY, MaxY, CenterError) = False) Then

        ''        ' Disable ourself.
        ''        CD.Parent.Square.NotifyChildDisable(CD.Parent, CD.ChildIndex)   ' nb: possibly deletes 'this'.
        ''    End If

        ''End Sub

        ''Private Sub EnableEdgeVertex(ByVal index As Byte, ByVal IncrementCount As Boolean, ByVal CD As CornerData)
        ''    ' Enable the specified edge vertex.  Indices go { e, n, w, s }.
        ''    ' Increments the appropriate reference-count if IncrementCount is true.

        ''    If ((Me.EnabledFlags And (1 << index)) > 0 And IncrementCount = False) Then Exit Sub

        ''    Static Dim Inc As Integer() = New Integer(3) {1, 0, 0, 8}

        ''    ' Turn on flag and deal with reference count.
        ''    Me.EnabledFlags = Me.EnabledFlags Or CByte(1 << index)
        ''    If (IncrementCount = True And (index = 0 Or index = 3)) Then
        ''        Me.SubEnabledCount(index And 1) += CByte(1)
        ''    End If

        ''    ' Now we need to enable the opposite edge vertex of the adjacent square (i.e. the alias vertex).

        ''    ' This is a little tricky, since the desired neighbor node may not exist, in which
        ''    ' case we have to create it, in order to prevent cracks.  Creating it may in turn cause
        ''    ' further edge vertices to be enabled, propagating updates through the tree.

        ''    ' The sticking point is the quadcornerdata list, which
        ''    ' conceptually is just a linked list of activation structures.
        ''    ' In this function, however, we will introduce branching into
        ''    ' the "list", making it in actuality a tree.  This is all kind
        ''    ' of obscure and hard to explain in words, but basically what
        ''    ' it means is that our implementation has to be properly
        ''    ' recursive.

        ''    ' Travel upwards through the tree, looking for the parent in common with our desired neighbor.
        ''    ' Remember the path through the tree, so we can travel down the complementary path to get to the neighbor.
        ''    Dim p As Square = Me
        ''    Dim pcd As CornerData = CD
        ''    Dim ct As Integer = 0
        ''    Dim stack(31) As Byte
        ''    Do
        ''        Dim ci As Byte = pcd.ChildIndex

        ''        'If (pcd.Parent Is Nothing OrElse pcd.Parent.Square Is Nothing) Then
        ''        If pcd.Parent Is Nothing OrElse pcd.Parent.Square Is Nothing Then
        ''            ' Neighbor doesn't exist (it's outside the tree), so there's no alias vertex to enable.
        ''            Exit Sub
        ''        End If
        ''        p = pcd.Parent.Square
        ''        pcd = pcd.Parent

        ''        Dim SameParent As Boolean = CBool(IIf(((CInt(index) - CInt(ci)) And 2) > 0, True, False))

        ''        ci = (ci Xor CByte(1) Xor ((index And CByte(1)) << CByte(1)))  ' Child index of neighbor node.

        ''        stack(ct) = ci
        ''        ct += 1

        ''        If SameParent Then Exit Do
        ''    Loop

        ''    ' Get a pointer to our neighbor (create if necessary), by walking down
        ''    ' the quadtree from our shared ancestor.
        ''    p = p.EnableDescendant(ct, stack, pcd)

        ''    '/*
        ''    '        ' Travel down the tree towards our neighbor, enabling and creating nodes as necessary.  We'll
        ''    '        ' follow the complement of the path we used on the way up the tree.
        ''    '	quadcornerdata	d(16);
        ''    '	int	i;
        ''    '	for (i = 0; i < ct; i++) {
        ''    '		int	ci = stack(ct-i-1);

        ''    '		if (p.Child(ci) == NULL && CreateDepth == 0) CreateDepth = ct-i;	'xxxxxxx

        ''    '		if ((p.EnabledFlags & (16 << ci)) == 0) {
        ''    '			p.EnableChild(ci, *pcd);
        ''    '		}
        ''    '		p.SetupCornerData(&d(i), *pcd, ci);
        ''    '		p = p.Child(ci);
        ''    '		pcd = &d(i);
        ''    '	}
        ''    '*/

        ''    ' Finally: enable the vertex on the opposite edge of our neighbor, the alias of the original vertex.


        ''    Dim MeHeight As Single = Me.HeightByte(index + CByte(1))


        ''    index = index Xor CByte(2)
        ''    p.EnabledFlags = CByte(p.EnabledFlags Or (1 << index))

        ''    p.HeightByte(index + CByte(1)) = MeHeight

        ''    If (IncrementCount = True And (index = 0 Or index = 3)) Then
        ''        p.SubEnabledCount(index And 1) += CByte(1)
        ''    End If

        ''End Sub
        ''Public Function EnableDescendant(ByVal count As Integer, ByVal path As Byte(), ByVal cd As CornerData) As Square
        ''    ' This function enables the descendant node 'count' generations below
        ''    ' us, located by following the list of child indices in path().
        ''    ' Creates the node if necessary, and returns a pointer to it.

        ''    count -= 1
        ''    Dim ChildIndex As Byte = path(count)

        ''    If ((EnabledFlags And (16 << ChildIndex)) = 0) Then
        ''        Me.EnableChild(ChildIndex, cd)
        ''    End If

        ''    If (count > 0) Then
        ''        Dim q As CornerData = New CornerData(cd, Me, ChildIndex)
        ''        Return Me.ChildByte(ChildIndex).EnableDescendant(count, path, q)
        ''    Else
        ''        Return Me.ChildByte(ChildIndex)
        ''    End If

        ''End Function

        ''Public Sub EnableChildNoCreate(ByVal index As Byte, ByVal cd As CornerData)
        ''    ' Enable the indexed child node.  { ne, nw, sw, se }
        ''    ' Causes dependent edge vertices to be enabled.

        ''    If Me.ChildByte(index) Is Nothing Then Exit Sub

        ''    If ((EnabledFlags And (16 << index)) = 0) Then
        ''        'Enabled(index + 4) = true;
        ''        Me.EnabledFlags = CByte(Me.EnabledFlags Or (16 << index))
        ''        EnableEdgeVertex(index, True, cd)
        ''        EnableEdgeVertex((index + CByte(1)) And CByte(3), True, cd)

        ''    End If

        ''End Sub

        ''Public Sub EnableChild(ByVal index As Byte, ByVal cd As CornerData)
        ''    ' Enable the indexed child node.  { ne, nw, sw, se }
        ''    ' Causes dependent edge vertices to be enabled.

        ''    'if (Enabled(index + 4) == false) {


        ''    If ((EnabledFlags And (16 << index)) = 0) Then
        ''        'Enabled(index + 4) = true;
        ''        Me.EnabledFlags = CByte(Me.EnabledFlags Or (16 << index))
        ''        EnableEdgeVertex(index, True, cd)
        ''        EnableEdgeVertex((index + CByte(1)) And CByte(3), True, cd)

        ''        If Me.ChildByte(index) Is Nothing Then
        ''            Me.CreateChild(cd, index)
        ''        End If
        ''    End If

        ''End Sub
        ''Public Sub NotifyChildDisable(ByVal cd As CornerData, ByVal index As Byte)
        ''    ' Marks the indexed child quadrant as disabled.  Deletes the child node
        ''    ' if it isn't static.

        ''    ' Clear enabled flag for the child.
        ''    Me.EnabledFlags = CByte(Me.EnabledFlags And Not (16 << index))

        ''    ' Update child enabled counts for the affected edge verts.
        ''    Dim s As Square

        ''    If (index And 2) > 0 Then
        ''        s = Me
        ''    Else
        ''        s = Me.GetNeighbour(1, cd)
        ''    End If
        ''    If s IsNot Nothing Then
        ''        s.SubEnabledCount(1) -= CByte(1)
        ''    End If

        ''    If (index = 1 Or index = 2) Then
        ''        s = Me.GetNeighbour(2, cd)
        ''    Else
        ''        s = Me
        ''    End If
        ''    If s IsNot Nothing Then
        ''        s.SubEnabledCount(0) -= CByte(1)
        ''    End If

        ''    If (Me.ChildByte(index).Static = False) Then
        ''        'delete Child(index);
        ''        Me.ChildByte(index) = Nothing
        ''        'Child(index) = 0;

        ''        'Me.BlockDeleteCount += 1 'xxxxx
        ''    End If

        ''End Sub

        ''Public Function GetNeighbour(ByVal dir As Byte, ByVal cd As CornerData) As Square
        ''    ' Traverses the tree in search of the quadsquare neighboring this square to the
        ''    ' specified direction.  0-3 --> { E, N, W, S }.
        ''    ' Returns NULL if the neighbor is outside the bounds of the tree.



        ''    ' If we don't have a parent, then we don't have a neighbor.
        ''    ' (Actually, we could have inter-tree connectivity at this level
        ''    ' for connecting separate trees together.)
        ''    If cd.Parent Is Nothing Then Return Nothing

        ''    ' Find the parent and the child-index of the square we want to locate or create.
        ''    Dim P As Square = Nothing

        ''    Dim index As Byte = cd.ChildIndex Xor CByte(1) Xor ((dir And CByte(1)) << CByte(1))
        ''    Dim SameParent As Boolean = CBool(IIf(((CInt(dir) - CInt(cd.ChildIndex)) And 2) > 0, True, False))

        ''    If (SameParent) Then
        ''        P = cd.Parent.Square
        ''    Else
        ''        P = cd.Parent.Square.GetNeighbour(dir, cd.Parent)

        ''        If (P Is Nothing) Then Return Nothing
        ''    End If

        ''    Dim n As Square = P.ChildByte(index)

        ''    Return n

        ''End Function


        ''Public Function RecomputeErrorAndLighting(ByVal CD As CornerData) As Single
        ''    ' Recomputes the error values for this tree.  Returns the
        ''    ' max error.
        ''    ' Also updates MinY & MaxY.

        ''    ' Measure error of center and edge vertices.
        ''    Dim maxError As Single = 0

        ''    ' Compute error of center vert.
        ''    Dim e As Single
        ''    If (CD.ChildIndex And 1) > 0 Then
        ''        e = CSng(Math.Abs(Me.Height(HeightDir.Center) - (CD.CornerHeight(CornerDir.NorthWest) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
        ''    Else
        ''        e = CSng(Math.Abs(Me.Height(HeightDir.Center) - (CD.CornerHeight(CornerDir.NorthEast) + CD.CornerHeight(CornerDir.SouthWest)) * 0.5))
        ''    End If
        ''    If (e > maxError) Then maxError = e

        ''    ' Initial min/max.
        ''    MaxY = Me.Height(HeightDir.Center)
        ''    MinY = Me.Height(HeightDir.Center)

        ''    ' Check min/max of corners.
        ''    For IB As Byte = 0 To 4 - 1
        ''        Dim Y As Single = CD.CornerHeightByte(IB)
        ''        If (Y < MinY) Then MinY = Y
        ''        If (Y > MaxY) Then MaxY = Y
        ''    Next

        ''    ' Edge verts.
        ''    e = CSng(Math.Abs(Me.Height(HeightDir.East) - (CD.CornerHeight(CornerDir.NorthEast) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
        ''    If (e > maxError) Then maxError = e
        ''    Me.Error(ErrorType.East) = e

        ''    e = CSng(Math.Abs(Me.Height(HeightDir.South) - (CD.CornerHeight(CornerDir.SouthWest) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
        ''    If (e > maxError) Then maxError = e
        ''    Me.Error(ErrorType.South) = e

        ''    ' Min/max of edge verts.
        ''    For IB As Byte = 0 To 4 - 1
        ''        Dim y As Single = Me.HeightByte(CByte(1) + IB)
        ''        If (y < MinY) Then MinY = y
        ''        If (y > MaxY) Then MaxY = y
        ''    Next


        ''    Dim q As CornerData
        ''    ' Check child squares.
        ''    For IB As Byte = 0 To 4 - 1
        ''        If Me.ChildByte(IB) IsNot Nothing Then
        ''            q = New CornerData(CD, Me, IB)
        ''            Me.ErrorByte(IB + CByte(2)) = Me.ChildByte(IB).RecomputeErrorAndLighting(q) 'CUShort(Me.Child(I).RecomputeErrorAndLighting(q))

        ''            If (Me.ChildByte(IB).MinY < MinY) Then MinY = Me.ChildByte(IB).MinY
        ''            If (Me.ChildByte(IB).MaxY > MaxY) Then MaxY = Me.ChildByte(IB).MaxY
        ''        Else
        ''            ' Compute difference between bilinear average at child center, and diagonal edge approximation.

        ''            'TODO: deze regel snap ik niet en lijkt vreemd

        ''            Me.ErrorByte(IB + CByte(2)) = CSng(Math.Abs((Me.Height(HeightDir.Center) + CD.CornerHeightByte(IB)) - (Me.HeightByte(IB + CByte(1)) + Me.HeightByte(((IB + CByte(1)) And CByte(3)) + CByte(1)))) * 0.25)
        ''        End If
        ''        If (Me.ErrorByte(IB + CByte(2)) > maxError) Then maxError = Me.ErrorByte(IB + CByte(2))
        ''    Next



        ''    ' The error, MinY/MaxY, and lighting values for this node and descendants are correct now.
        ''    Dirty = False

        ''    Return maxError
        ''End Function



        '''' <summary>
        '''' returned true als de vertex niet op zijn geinterpoleerde positie ligt.
        '''' </summary>
        '''' <param name="x"></param>
        '''' <param name="y"></param>
        '''' <param name="z"></param>
        '''' <param name="error"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function VertexTest(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal [error] As Single) As Boolean
        '    Return [error] > 0.01

        'End Function


        'Public Function BoxTest(ByVal x As Single, ByVal z As Single, ByVal size As Single, ByVal minY As Single, ByVal maxY As Single, ByVal [error] As Single) As Boolean
        '    '' Returns true if any vertex within the specified box (origin at x,z,
        '    '' edges of length size) with the given error value could be enabled
        '    '' based on the given viewer location.

        '    '' Find the minimum distance to the box.
        '    'Dim half As Single = size * 0.5F
        '    'Dim dx As Single = CSng(Math.Abs(x + half - Viewer(0)) - half)
        '    'Dim dy As Single = CSng(Math.Abs((minY + maxY) * 0.5F - Viewer(1)) - (maxY - minY) * 0.5F)
        '    'Dim dz As Single = CSng(Math.Abs(z + half - Viewer(2)) - half)
        '    'Dim d As Single = dx
        '    'If (dy > d) Then d = dy
        '    'If (dz > d) Then d = dz

        '    Return [error] > 0.01
        'End Function





        'Public Function WriteBlock(ByVal BFS As BlockFileStream) As BlockPointer
        '    Dim B As Block = BFS.CreateBlock


        '    For IB As Byte = 0 To 4 - 1
        '        If Me.ChildByte(IB) IsNot Nothing Then
        '            B.Write(Me.ChildByte(IB).WriteBlock(BFS).Address)
        '        Else
        '            B.Write(-1)
        '        End If
        '    Next

        '    B.Write(Me.Height(HeightDir.Center))
        '    B.Write(Me.Height(HeightDir.East))
        '    B.Write(Me.Height(HeightDir.South))

        '    BFS.WriteBlock(B)

        '    Return B.Pointer
        'End Function

        'Public Sub ReadBlock(ByVal CD As CornerData, ByVal BFS As BlockFileStreamReader, ByVal Block As BlockReader)
        '    Dim ChildPointer(4 - 1) As BlockPointer
        '    For IB As Byte = 0 To 4 - 1
        '        ChildPointer(IB) = New BlockPointer(Block.ReadInt32)
        '    Next

        '    Me.Height(HeightDir.Center) = Block.ReadSingle
        '    Me.Height(HeightDir.East) = Block.ReadSingle
        '    Me.Height(HeightDir.South) = Block.ReadSingle

        '    For IB As Byte = 0 To 4 - 1
        '        'If (Me.EnabledFlags And 16 << IB) > 0 Then
        '        If Me.ChildByte(IB) Is Nothing Then
        '            If ChildPointer(IB).Address = -1 Then
        '            Else
        '                'create node
        '                Me.CreateChild(CD, IB)

        '                Me.ChildByte(IB).ReadBlock(New CornerData(CD, Me, IB), BFS, BFS.ReadBlock(ChildPointer(IB)))
        '            End If
        '        Else
        '            If ChildPointer(IB).Address = -1 Then
        '                Me.NotifyChildDisable(New CornerData(CD, Me, IB), IB)
        '            Else
        '                Me.ChildByte(IB).ReadBlock(New CornerData(CD, Me, IB), BFS, BFS.ReadBlock(ChildPointer(IB)))
        '            End If
        '        End If
        '    Next


        'End Sub


        'Private Function CalculateError(ByVal CD As CornerData) As Single

        'End Function

        Public Structure PreprocessedData
            Public Pointer As BlockPointer
            Public MaxError As Single
            Public MinY As Single
            Public MaxY As Single
        End Structure

        Public Shared Function BuildPreprocessed(ByVal DataBFS As BlockFileStreamReader, ByVal OutBFS As BlockFileStream, ByVal ReadPointer As BlockPointer, ByVal CD As CornerData) As PreprocessedData
            Dim SQBlock As SquareBlock = SquareBlock.FromBytes(DataBFS.ReadBlockBytes(ReadPointer), ReadPointer)
            Dim PrepBlock As PrecompBlock = New PrecompBlock(OutBFS.CreateBlockPointer)
            Dim Ret As New PreprocessedData


            ' Recomputes the error values for this tree.  Returns the
            ' max error.
            ' Also updates MinY & MaxY.

            ' Measure error of center and edge vertices.
            Dim maxError As Single = 0
            Dim minY As Single = 0
            Dim maxY As Single = 0

            ' Compute error of center vert.
            Dim e As Single
            If (CD.ChildIndex And 1) > 0 Then
                e = CSng(Math.Abs(SQBlock.Height(HeightDir.Center) - (CD.CornerHeight(CornerDir.NorthWest) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
            Else
                e = CSng(Math.Abs(SQBlock.Height(HeightDir.Center) - (CD.CornerHeight(CornerDir.NorthEast) + CD.CornerHeight(CornerDir.SouthWest)) * 0.5))
            End If
            If (e > maxError) Then maxError = e

            ' Initial min/max.
            maxY = SQBlock.Height(HeightDir.Center)
            minY = SQBlock.Height(HeightDir.Center)

            ' Check min/max of corners.
            For IB As Byte = 0 To 4 - 1
                Dim Y As Single = CD.CornerHeightByte(IB)
                If (Y < minY) Then minY = Y
                If (Y > maxY) Then maxY = Y
            Next

            ' Edge verts.
            e = CSng(Math.Abs(SQBlock.Height(HeightDir.East) - (CD.CornerHeight(CornerDir.NorthEast) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
            If (e > maxError) Then maxError = e
            PrepBlock.Error(ErrorType.East) = e

            e = CSng(Math.Abs(SQBlock.Height(HeightDir.South) - (CD.CornerHeight(CornerDir.SouthWest) + CD.CornerHeight(CornerDir.SouthEast)) * 0.5))
            If (e > maxError) Then maxError = e
            PrepBlock.Error(ErrorType.South) = e

            ' Min/max of edge verts.
            For IB As Byte = 0 To 4 - 1
                Dim y As Single = SQBlock.HeightByte(CByte(1) + IB)
                If (y < minY) Then minY = y
                If (y > maxY) Then maxY = y
            Next


            Dim q As CornerData
            ' Check child squares.
            For IB As Byte = 0 To 4 - 1
                If Not SQBlock.ChildPointerByte(IB).IsEmpty Then
                    Dim ChildRet As PreprocessedData

                    q = New CornerData(CD, SQBlock, IB)
                    ChildRet = BuildPreprocessed(DataBFS, OutBFS, SQBlock.ChildPointerByte(IB), q)

                    PrepBlock.ErrorByte(IB + CByte(2)) = ChildRet.MaxError
                    PrepBlock.ChildPointerByte(IB) = ChildRet.Pointer

                    If (ChildRet.MinY < minY) Then minY = ChildRet.MinY
                    If (ChildRet.MaxY > maxY) Then maxY = ChildRet.MaxY
                Else
                    ' Compute difference between bilinear average at child center, and diagonal edge approximation.

                    'TODO: deze regel snap ik niet en lijkt vreemd

                    PrepBlock.ErrorByte(IB + CByte(2)) = CSng(Math.Abs((SQBlock.Height(HeightDir.Center) + CD.CornerHeightByte(IB)) - (SQBlock.HeightByte(IB + CByte(1)) + SQBlock.HeightByte(((IB + CByte(1)) And CByte(3)) + CByte(1)))) * 0.25)
                    PrepBlock.ChildPointerByte(IB) = BlockPointer.Empty
                End If
                If (PrepBlock.ErrorByte(IB + CByte(2)) > maxError) Then maxError = PrepBlock.ErrorByte(IB + CByte(2))
            Next



            ' The error, MinY/MaxY, and lighting values for this node and descendants are correct now.
            'Dirty = False




            PrepBlock.Positie = CD.Center
            PrepBlock.SizeRadius = CD.SizeRadius
            PrepBlock.Height(VertexDir.Center) = SQBlock.Height(HeightDir.Center)
            PrepBlock.Height(VertexDir.East) = SQBlock.Height(HeightDir.East)
            PrepBlock.Height(VertexDir.North) = SQBlock.Height(HeightDir.North)
            PrepBlock.Height(VertexDir.West) = SQBlock.Height(HeightDir.West)
            PrepBlock.Height(VertexDir.South) = SQBlock.Height(HeightDir.South)

            PrepBlock.Height(VertexDir.NorthEast) = CD.CornerHeight(CornerDir.NorthEast)
            PrepBlock.Height(VertexDir.NorthWest) = CD.CornerHeight(CornerDir.NorthWest)
            PrepBlock.Height(VertexDir.SouthWest) = CD.CornerHeight(CornerDir.SouthWest)
            PrepBlock.Height(VertexDir.SouthEast) = CD.CornerHeight(CornerDir.SouthEast)

            PrepBlock.MinY = minY
            PrepBlock.MaxY = maxY



            OutBFS.WriteBlock(PrepBlock)

            Ret.Pointer = PrepBlock.Pointer
            Ret.MinY = minY
            Ret.MaxY = maxY
            Ret.MaxError = maxError
            Return Ret
        End Function
    End Class

End Namespace
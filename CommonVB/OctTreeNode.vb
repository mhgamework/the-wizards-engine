Public Class OctTreeNode
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject, ByVal nPositie As Vector3, ByVal nSize As Vector3)
        MyBase.New(nParent)
        Me.setBoxModel(New BoundingBox3D(Me, Me))
        Me.setEntities(New List(Of Entity))
        Me.setPlayerObservers(New List(Of Player))
        Me.setNodes(Nothing)
        If nParent.GetType.Equals(GetType(OctTree)) Or nParent.GetType.IsSubclassOf(GetType(OctTree)) Then
            Me.setNodeParent(Nothing)
            Me.setOctTree(CType(nParent, OctTree))
        Else
            Me.setNodeParent(CType(nParent, OctTreeNode))
            Me.setOctTree(Me.NodeParent.OctTree)
        End If

        Me.setIsLeaf(True)
        Me.setNodeSize(nSize)
        Me.setNodePositie(nPositie)





        'Me.setEntities(New List(Of Entity))
    End Sub


    Private mOctTree As OctTree
    Public ReadOnly Property OctTree() As OctTree
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mOctTree
        End Get
    End Property
    Private Sub setOctTree(ByVal value As OctTree)
        Me.mOctTree = value
    End Sub


    Private mNodeParent As OctTreeNode
    Public ReadOnly Property NodeParent() As OctTreeNode
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNodeParent
        End Get
    End Property
    Private Sub setNodeParent(ByVal value As OctTreeNode)
        Me.mNodeParent = value
    End Sub



    Private mNodeSize As Vector3
    Public ReadOnly Property NodeSize() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNodeSize
        End Get
    End Property
    Private Sub setNodeSize(ByVal value As Vector3)
        Me.mNodeSize = value
        Me.setBoundingSphereRadius(Me.NodeSize.Length / 2)
        Me.setBoundingBoxRadius(Me.NodeSize * (1 / 2))
        UpdateBoxModelRootMatrix()
    End Sub


    Private mNodePositie As Vector3
    Public ReadOnly Property NodePositie() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNodePositie
        End Get
    End Property
    Private Sub setNodePositie(ByVal value As Vector3)
        Me.mNodePositie = value
        UpdateBoxModelRootMatrix()
    End Sub



    Private mBoxModel As BoundingBox3D
    Protected ReadOnly Property BoxModel() As BoundingBox3D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoxModel
        End Get
    End Property
    Private Sub setBoxModel(ByVal value As BoundingBox3D)
        Me.mBoxModel = value
    End Sub

    Public Sub UpdateBoxModelRootMatrix()
        If Me.BoxModel.AHModel Is Nothing Then Exit Sub

        Me.BoxModel.AHModel.RootMatrix = Matrix.Scaling(Me.NodeSize) * Matrix.Translation(Me.NodePositie)
    End Sub

    Private mNodes As OctTreeNode()
    Public ReadOnly Property Nodes() As OctTreeNode()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNodes
        End Get
    End Property
    Private Sub setNodes(ByVal value As OctTreeNode())
        Me.mNodes = value
    End Sub


    Private mEntities As List(Of Entity)
    Public ReadOnly Property Entities() As List(Of Entity)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntities
        End Get
    End Property
    Private Sub setEntities(ByVal value As List(Of Entity))
        Me.mEntities = value
    End Sub



    Public ReadOnly Property BoundingSphereCenter() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.NodePositie
        End Get
    End Property



    Private mBoundingSphereRadius As Single
    Public ReadOnly Property BoundingSphereRadius() As Single
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoundingSphereRadius
        End Get
    End Property
    Private Sub setBoundingSphereRadius(ByVal value As Single)
        Me.mBoundingSphereRadius = value
    End Sub


    Public ReadOnly Property BoundingBoxCenter() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.NodePositie
        End Get
    End Property



    Private mBoundingBoxRadius As Vector3
    Public ReadOnly Property BoundingBoxRadius() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoundingBoxRadius
        End Get
    End Property
    Private Sub setBoundingBoxRadius(ByVal value As Vector3)
        Me.mBoundingBoxRadius = value
    End Sub




    Private mIsLeaf As Boolean
    Public ReadOnly Property IsLeaf() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mIsLeaf
        End Get
    End Property
    Private Sub setIsLeaf(ByVal value As Boolean)
        Me.mIsLeaf = value
    End Sub

    Public Sub Split()
        If Me.IsLeaf = False Then Throw New Exception("Deze node is geen leaf")

        Dim ChildSize As Vector3 = Me.NodeSize * (1 / 2)
        Dim HalfChildSize As Vector3 = ChildSize * (1 / 2)

        Dim Ns As OctTreeNode() = New OctTreeNode(7) {}
        Dim T As Vector3
        T.X = Me.NodePositie.X - HalfChildSize.X
        T.Y = Me.NodePositie.Y - HalfChildSize.Y
        T.Z = Me.NodePositie.Z - HalfChildSize.Z
        Ns(0) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X - HalfChildSize.X
        T.Y = Me.NodePositie.Y - HalfChildSize.Y
        T.Z = Me.NodePositie.Z + HalfChildSize.Z
        Ns(1) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X - HalfChildSize.X
        T.Y = Me.NodePositie.Y + HalfChildSize.Y
        T.Z = Me.NodePositie.Z - HalfChildSize.Z
        Ns(2) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X - HalfChildSize.X
        T.Y = Me.NodePositie.Y + HalfChildSize.Y
        T.Z = Me.NodePositie.Z + HalfChildSize.Z
        Ns(3) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X + HalfChildSize.X
        T.Y = Me.NodePositie.Y - HalfChildSize.Y
        T.Z = Me.NodePositie.Z - HalfChildSize.Z
        Ns(4) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X + HalfChildSize.X
        T.Y = Me.NodePositie.Y - HalfChildSize.Y
        T.Z = Me.NodePositie.Z + HalfChildSize.Z
        Ns(5) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X + HalfChildSize.X
        T.Y = Me.NodePositie.Y + HalfChildSize.Y
        T.Z = Me.NodePositie.Z - HalfChildSize.Z
        Ns(6) = Me.CreateNode(Me, T, ChildSize)
        T.X = Me.NodePositie.X + HalfChildSize.X
        T.Y = Me.NodePositie.Y + HalfChildSize.Y
        T.Z = Me.NodePositie.Z + HalfChildSize.Z
        Ns(7) = Me.CreateNode(Me, T, ChildSize)

        'Next

        Me.setNodes(Ns)

        Me.setIsLeaf(False)

    End Sub

    Protected Overridable Function CreateNode(ByVal nParent As SpelObject, ByVal nPositie As Vector3, ByVal nSize As Vector3) As OctTreeNode
        Return New OctTreeNode(nParent, nPositie, nSize)
    End Function

    Public Sub Merge()
        If Me.IsLeaf = True Then Throw New Exception("Deze node is een leaf")

        For Each N As OctTreeNode In Me.Nodes
            N.MoveChildrenToParent()
            N.Dispose()
        Next
        Me.setNodes(Nothing)
        Me.setIsLeaf(True)


    End Sub

    Public Sub MoveChildrenToParent()
        If Me.IsLeaf = False Then
            For Each N As OctTreeNode In Me.Nodes
                N.MoveChildrenToParent()
            Next
        End If

        Dim Ents As Entity() = Me.Entities.ToArray
        For Each E As Entity In Ents
            Me.Parent.Move(E)
        Next
    End Sub


    Protected Overrides Sub Add(ByVal item As MHGameWork.Game3DPlay.SpelObject)
        MyBase.Add(item)
        If TypeOf item Is BoundingBox3D Then
        ElseIf TypeOf item Is OctTreeNode Then
        ElseIf TypeOf item Is Entity Then
            Me.Entities.Add(CType(item, Entity))

        Else
            Throw New Exception("Er mogen geen andere objecten als nodes of entities worden toegevoegd.")
        End If
    End Sub
    Protected Overrides Function Remove(ByVal item As MHGameWork.Game3DPlay.SpelObject) As Boolean
        Return MyBase.Remove(item)
        If item.GetType.Equals(GetType(OctTreeNode)) Or item.GetType.IsSubclassOf(GetType(OctTreeNode)) Then
        ElseIf item.GetType.Equals(GetType(Entity)) Or item.GetType.IsSubclassOf(GetType(Entity)) Then
            Me.Entities.Remove(CType(item, Entity))

        Else
            Throw New Exception("Er kunnen andere objecten als nodes of entities in deze node zitten.")
        End If
    End Function


    ''' <summary>
    ''' Kijkt of de gegeven Entity in deze node moet worden geplaatst 
    ''' en returned true as de Entity in deze node is geplaatst.
    ''' </summary>
    ''' <param name="nEnt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function OrdenEntity(ByVal nEnt As Entity) As Boolean
        If CollMath.PointInSphere(Me.BoundingSphereCenter, Me.BoundingSphereRadius, nEnt.BoundingSphereCenter) = False Then Return False

        If Me.IsLeaf = False Then
            For Each N As OctTreeNode In Me.Nodes
                If N.OrdenEntity(nEnt) Then Return True
            Next
        End If

        If CollMath.CheckCenteredBoxState(Me.BoundingBoxCenter, Me.BoundingBoxRadius, nEnt.BoundingBoxCenter, nEnt.BoundingBoxRadius) = State.Inside Then
            Me.Move(nEnt)
            Return True
        Else
            Return False

        End If
    End Function


    Protected Class BoundingBox3D
        Inherits Box3D
        Public Sub New(ByVal nParent As SpelObject, ByVal nNode As OctTreeNode)
            MyBase.New(nParent)
            Me.setNode(nNode)
        End Sub


        Private mNode As OctTreeNode
        Public ReadOnly Property Node() As OctTreeNode
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mNode
            End Get
        End Property
        Private Sub setNode(ByVal value As OctTreeNode)
            Me.mNode = value
        End Sub


        Protected Overrides Sub RenderModel()
            If Me.Node.OctTree.DrawBoundingBoxes = False Then Exit Sub

            With Me.HoofdObj.DevContainer.DX.Device.RenderState
                .SourceBlend = Blend.SourceAlpha
                .DestinationBlend = Blend.One
                .ZBufferWriteEnable = False
            End With
            MyBase.RenderModel()
            With Me.HoofdObj.DevContainer.DX.Device.RenderState
                .SourceBlend = Blend.SourceAlpha
                .DestinationBlend = Blend.InvSourceAlpha
                .ZBufferWriteEnable = True
            End With

        End Sub

    End Class

    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As DeviceEventArgs)
        MyBase.OnDeviceReset(sender, e)
        Me.UpdateBoxModelRootMatrix()
    End Sub



    Public Sub GetAllEntities(ByVal nList As List(Of Entity))
        nList.AddRange(Me.Entities)
        If Me.IsLeaf = False Then
            For Each N As OctTreeNode In Me.Nodes
                N.GetAllEntities(nList)
            Next
        End If
    End Sub



    Private mPlayerObservers As List(Of Player)
    Public ReadOnly Property PlayerObservers() As List(Of Player)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPlayerObservers
        End Get
    End Property
    Private Sub setPlayerObservers(ByVal value As List(Of Player))
        Me.mPlayerObservers = value
    End Sub


    Public Sub AddPlayerObserver(ByVal nPlayer As Player)
        If nPlayer.ObservingNode Is Me Then Exit Sub
        If nPlayer.ObservingNode IsNot Nothing Then nPlayer.ObservingNode.RemovePlayerObserver(nPlayer)
        Me.PlayerObservers.Add(nPlayer)
        nPlayer.setObservingNode(Me)

    End Sub

    Private Sub RemovePlayerObserver(ByVal nPlayer As Player)
        Me.PlayerObservers.Remove(nPlayer)
        nPlayer.setObservingNode(Nothing)

    End Sub

    Public Function FindEntity(ByVal nEntID As Integer) As Entity
        For Each Ent As Entity In Me.Entities
            If Ent.EntityID = nEntID Then Return Ent
        Next
        If Me.IsLeaf = False Then
            Dim Ent As Entity
            For Each N As OctTreeNode In Me.Nodes
                Ent = N.FindEntity(nEntID)
                If Ent IsNot Nothing Then Return Ent
            Next
        End If
        Return Nothing
    End Function


    'Public Overrides Sub DoEvent(ByVal ev As MHGameWork.Game3DPlay.SpelEvent)
    '    If ev.GetType.Equals(GetType(VeranderingEvent)) Then
    '        RaiseEvent Verandering(CType(ev, VeranderingEvent))
    '        Exit Sub 'TODO: mag dit wel?
    '    End If
    '    MyBase.DoEvent(ev)
    'End Sub

    'Public Event Verandering(ByVal ev As VeranderingEvent)


    ''' <summary>
    ''' Returns a list with all the players observing this node, include the ones in the higher nodes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlayersObservingNode() As List(Of Player)
        Dim L As New List(Of Player)
        Me.AddPlayersObservingNode(L)
        Return L
    End Function

    Protected Sub AddPlayersObservingNode(ByVal nL As List(Of Player))
        nL.AddRange(Me.PlayerObservers)
        If Me.NodeParent IsNot Nothing Then
            Me.NodeParent.AddPlayersObservingNode(nL)
        End If
    End Sub


    Public Sub OnVerandering(ByVal sender As Object, ByVal e As VeranderingEventArgs)
        For Each PL As Player In Me.PlayerObservers
            If PL.Client IsNot Nothing Then
                PL.Client.OnVerandering(sender, e)
            End If
        Next
        If Me.NodeParent IsNot Nothing Then Me.NodeParent.OnVerandering(sender, e)
    End Sub

    'Public Sub ListContactJoints(ByVal nJoints As List(Of Ode.ContactJoint))
    '    N = 0
    '    Dim Cols As New List(Of Entity)
    '    Dim Stil As New List(Of Entity)

    '    For I As Integer = 0 To Me.Entities.Count - 1
    '        If Me.Entities(I).Snelheid.Length <> 0 OrElse Me.Entities(I).RotatieSnelheid.Length <> 0 Then
    '            Cols.Add(Me.Entities(I))
    '        Else
    '            Stil.Add(Me.Entities(I))
    '        End If

    '    Next
    '    'Dim N As Integer
    '    For I As Integer = 0 To Cols.Count - 1

    '        For I2 As Integer = I + 1 To Cols.Count - 1
    '            Me.AddJoints(Cols(I), Cols(I2), nJoints)
    '            N += 1
    '        Next
    '        For I3 As Integer = 0 To Stil.Count - 1
    '            Me.AddJoints(Cols(I), Stil(I3), nJoints)
    '            N += 1
    '        Next
    '    Next
    '    Stop

    'End Sub

    'Public Sub TestIfMoving(ByVal Ent1 As Entity)
    '    Dim V As Single = Ent1.Snelheid.Length
    '    V += 1

    'End Sub


    'Private mN As Integer
    'Public Property N() As Integer
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mN
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    '        Me.mN = value
    '    End Set
    'End Property


    'Public Sub AddJoints(ByVal Ent1 As Entity, ByVal Ent2 As Entity, ByVal Joints As List(Of Ode.ContactJoint))
    '    If Ent1.Body.Geoms.Count = 0 OrElse Ent2.Body.Geoms.Count = 0 Then Exit Sub

    '    'Dim D As Decimal = Analyzer.Analyze(Of Entity)(AddressOf Me.TestIfMoving, Ent1, 100)
    '    'Stop

    '    'If Ent1.Snelheid.Length = 0 And Ent2.Snelheid.Length = 0 Then
    '    '    Exit Sub
    '    'End If
    '    'If Ent1.RotatieSnelheid.Length = 0 And Ent2.RotatieSnelheid.Length = 0 Then
    '    'Exit Sub
    '    'End If

    '    If Ent1.Body.Geoms.Count > 1 OrElse Ent2.Body.Geoms.Count > 1 Then Throw New Exception("Meer dan een Geom nog niet gemaakt")

    '    Dim CGs As Ode.ContactGeom() = Ent1.Body.Geoms.Geom(0).Collide(Ent2.Body.Geoms.Geom(0), 20)


    '    If CGs.Length > 0 Then
    '        Dim Cs As Ode.Contact() = Ode.Contact.FromContactGeomArray(CGs)
    '        Dim IJ As Ode.ContactJoint
    '        For Each IC As Ode.Contact In Cs
    '            IJ = New Ode.ContactJoint(CType(Me.HoofdObj, BaseMain).ODEWorld, IC)
    '            IJ.Attach(Ent1.Body, Ent2.Body)

    '            Joints.Add(IJ)

    '        Next
    '    End If

    'End Sub

End Class

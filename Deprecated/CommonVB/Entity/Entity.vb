Public MustInherit Class Entity
    Inherits SpelObject
    'Implements IPositionable3D

    Public Sub New(ByVal nParent As OctTree)
        MyBase.New(nParent)

        'Me.setFunctions(nFunctions)

        Me.setBaseMain(CType(Me.HoofdObj, BaseMain))
        Me.setOctTree(nParent)
        Me.setEntityID(Entity.GetNewEntityID)
        Me.setEntityType(CType(System.Enum.Parse(GetType(EntityType), Me.GetType.Name), EntityType))

        'Me.mPositie = Vector3.Empty
        'Me.mSnelheid = Vector3.Empty
        'Me.mRotatieMatrix = Matrix.Identity
        'Me.mRotatieSnelheid = Vector3.Empty
        'Me.mScale = New Vector3(1, 1, 1)


        Me.setProcessEventElement(New ProcessEventElement(Me))


        Me.OctTree.AddEntity(Me)

        'Me.setNetworkElement(Me.BaseMain.CreateNetworkElement(Me))
        'Me.setfunctions(Me.BaseMain.Createfunctions(Me))
    End Sub

    Public Overrides Sub Initialize()

        If Me.mFunctions Is Nothing Then Throw New Exception("Er zijn voor dit object geen functies gegeven")
        MyBase.Initialize()

    End Sub

    'Protected Overridable Function CheckForInitialize() As Boolean
    '    If Me.Functions IsNot Nothing Then Return True
    'End Function




    Private mFunctions As EntityFunctions
    Public ReadOnly Property Functions() As EntityFunctions
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFunctions
        End Get
    End Property
    Public Sub setFunctions(ByVal value As EntityFunctions)
        If Me.mFunctions IsNot Nothing Then Throw New Exception("De Functions zijn al ingesteld")
        Me.mFunctions = value
        'If Me.CheckForInitialize Then Me.Initialize()
    End Sub




    Private WithEvents mTickElement As TickElement

    Private WithEvents mProcessEventElement As ProcessEventElement
    Public ReadOnly Property ProcessEventElement() As ProcessEventElement
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mProcessEventElement
        End Get
    End Property
    Private Sub setProcessEventElement(ByVal value As ProcessEventElement)
        Me.mProcessEventElement = value
    End Sub


    Private mBaseMain As BaseMain
    Public ReadOnly Property BaseMain() As BaseMain
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBaseMain
        End Get
    End Property
    Private Sub setBaseMain(ByVal value As BaseMain)
        Me.mBaseMain = value
    End Sub


    Private mEntityType As EntityType
    Public ReadOnly Property EntityType() As EntityType
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntityType
        End Get
    End Property
    Private Sub setEntityType(ByVal value As EntityType)
        Me.mEntityType = value
    End Sub


    Private mEntityID As Integer
    Public ReadOnly Property EntityID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntityID
        End Get
    End Property
    Public Sub setEntityID(ByVal value As Integer)
        Me.mEntityID = value
        If value > Entity.LastEntityID Then Entity.setLastEntityID(value)
    End Sub


    Private Shared mLastEntityID As Integer
    Public Shared ReadOnly Property LastEntityID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return mLastEntityID
        End Get
    End Property
    Private Shared Sub setLastEntityID(ByVal value As Integer)
        mLastEntityID = value
    End Sub

    Public Shared Function GetNewEntityID() As Integer
        setLastEntityID(LastEntityID + 1)
        Return LastEntityID
    End Function



    Public ReadOnly Property Versie() As Integer
        Get
            Return Me.Functions.Versie
        End Get
    End Property
    Public Sub setVersie(ByVal value As Integer)
        Me.Functions.setVersie(value)
    End Sub



    Public Sub OnEntityVeranderd(ByVal sender As Object, ByVal e As EntityVeranderdEventArgs)
        RaiseEvent EntityVeranderd(Me, e)
    End Sub
    Public Event EntityVeranderd(ByVal sender As Object, ByVal e As EntityVeranderdEventArgs)





    Private mBoundingSphereCenter As Vector3
    Public ReadOnly Property BoundingSphereCenter() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoundingSphereCenter
        End Get
    End Property
    Protected Sub setBoundingSphereCenter(ByVal value As Vector3)
        Me.mBoundingSphereCenter = value
    End Sub


    Private mBoundingSphereRadius As Single
    Public ReadOnly Property BoundingSphereRadius() As Single
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoundingSphereRadius
        End Get
    End Property
    Protected Sub setBoundingSphereRadius(ByVal value As Single)
        Me.mBoundingSphereRadius = value
    End Sub


    Private mBoundingBoxCenter As Vector3
    Public ReadOnly Property BoundingBoxCenter() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoundingBoxCenter
        End Get
    End Property
    Protected Sub setBoundingBoxCenter(ByVal value As Vector3)
        Me.mBoundingBoxCenter = value
    End Sub


    Private mBoundingBoxRadius As Vector3
    Public ReadOnly Property BoundingBoxRadius() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBoundingBoxRadius
        End Get
    End Property
    Protected Sub setBoundingBoxRadius(ByVal value As Vector3)
        Me.mBoundingBoxRadius = value
    End Sub


    Public Function GetData() As Byte()
        Dim BW As New ByteWriter
        Me.GetData(BW)


        Dim B As Byte() = BW.ToBytes

        BW.Close() 'TODO: try...finally block nodig?

        Return B
    End Function
    Protected Overridable Sub GetData(ByVal BW As ByteWriter)

        BW.Write(Me.functions.Positie)
        BW.Write(Me.functions.Snelheid)
        BW.Write(Me.functions.RotatieMatrix)
        BW.Write(Me.functions.RotatieSnelheid)
        BW.Write(Me.functions.Scale)

    End Sub

    Public Sub Update(ByVal Data As Byte())
        Dim BR As New ByteReader(Data)

        Me.Update(BR)

        BR.Close() 'TODO: try...finally block nodig?

    End Sub

    Protected Overridable Sub Update(ByVal BR As ByteReader)
        Me.functions.Positie = BR.ReadVector3
        Me.functions.Snelheid = BR.ReadVector3
        Me.functions.RotatieMatrix = BR.ReadMatrix
        Me.functions.RotatieSnelheid = BR.ReadVector3
        Me.functions.Scale = BR.ReadVector3

        'Me.Scale = New Vector3(1, 1, 1)
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


    Private mParentNode As OctTreeNode
    Public ReadOnly Property ParentNode() As OctTreeNode
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mParentNode
        End Get
    End Property
    Private Sub setParentNode(ByVal value As OctTreeNode)
        Me.mParentNode = value
    End Sub

    Public Overrides Sub OnParentChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.OnParentChanged(sender, e)

        If Me.Parent Is Nothing Then
            Me.setParentNode(Nothing)
        ElseIf Me.Parent.GetType.Equals(GetType(OctTreeNode)) Or Me.Parent.GetType.IsSubclassOf(GetType(OctTreeNode)) Then
            Me.setParentNode(CType(Me.Parent, OctTreeNode))
        Else
            Me.setParentNode(Nothing)
        End If
    End Sub


    'Public Sub Save()
    '    CType(Me.HoofdObj, BaseMain).BaseDB.SaveEntity(Me)
    'End Sub


    Private mNetworkElement As NetworkElement
    Public ReadOnly Property NetworkElement() As NetworkElement
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNetworkElement
        End Get
    End Property
    Private Sub setNetworkElement(ByVal value As NetworkElement)
        Me.mNetworkElement = value
    End Sub




    Public Sub Update(ByVal nVeranderingType As Integer, ByVal nData As Byte())
        'Dim BR As New ByteReader(nData)
        'Select Case nVeranderingType
        '    Case 1
        '        Me.InterpolatePositie(BR.ReadVector3)

        '        'Me.Positie = BR.ReadVector3
        '        'Me.setRenderPositie(Me.Positie)
        '    Case 2
        '        Me.functions.Snelheid = BR.ReadVector3
        '    Case 3
        '        'Me.RotatieMatrix = BR.ReadMatrix
        '        Me.InterpolateRotatie(BR.ReadMatrix)
        '        'Me.setRenderRotatie(Me.RotatieMatrix)
        '    Case 4
        '        Me.functions.RotatieSnelheid = BR.ReadVector3
        'End Select

        'BR.Close()
    End Sub







    Public Overridable Sub OnPositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)
        RaiseEvent PositieVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnSnelheidVeranderd(ByVal sender As Object, ByVal e As SnelheidVeranderdEventArgs)
        RaiseEvent SnelheidVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnRotatieVeranderd(ByVal sender As Object, ByVal e As RotatieVeranderdEventArgs)
        RaiseEvent RotatieVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnRotatieSnelheidVeranderd(ByVal sender As Object, ByVal e As RotatieSnelheidVeranderdEventArgs)
        RaiseEvent RotatieSnelheidVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnScaleVeranderd(ByVal sender As Object, ByVal e As ScaleVeranderdEventArgs)
        RaiseEvent ScaleVeranderd(sender, e)
    End Sub

    Public Event PositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)

    Public Event SnelheidVeranderd(ByVal sender As Object, ByVal e As SnelheidVeranderdEventArgs)

    Public Event RotatieVeranderd(ByVal sender As Object, ByVal e As RotatieVeranderdEventArgs)

    Public Event RotatieSnelheidVeranderd(ByVal sender As Object, ByVal e As RotatieSnelheidVeranderdEventArgs)

    Public Event ScaleVeranderd(ByVal sender As Object, ByVal e As ScaleVeranderdEventArgs)



    Public Overridable Sub ReadSerializedData(ByVal nDS As DataSerializer)
        Me.Functions.ReadSerializedData(nDS)
    End Sub

    Public Overridable Sub WriteSerializeData(ByVal nDS As DataSerializer)
        Me.Functions.WriteSerializeData(nDS)
    End Sub






End Class

Public Class OctTree
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject, ByVal nPositie As Vector3, ByVal nSize As Vector3)
        MyBase.New(nParent)
        Me.setRootNode(Me.CreateRootNode(Me, nPositie, nSize))

        Me.setRenderContainer(New RenderContainer(Me))

    End Sub


    Private WithEvents mRenderContainer As RenderContainer
    Public ReadOnly Property RenderContainer() As RenderContainer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRenderContainer
        End Get
    End Property
    Private Sub setRenderContainer(ByVal value As RenderContainer)
        Me.mRenderContainer = value
    End Sub


    Protected Overridable Function CreateRootNode(ByVal nParent As SpelObject, ByVal nPositie As Vector3, ByVal nSize As Vector3) As OctTreeNode
        Return New OctTreeNode(nParent, nPositie, nSize)
    End Function


    Private mTreeSize As Vector3
    Public ReadOnly Property TreeSize() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTreeSize
        End Get
    End Property
    Private Sub setTreeSize(ByVal value As Vector3)
        Me.mTreeSize = value
    End Sub


    Private mRootNode As OctTreeNode
    Public ReadOnly Property RootNode() As OctTreeNode
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRootNode
        End Get
    End Property
    Private Sub setRootNode(ByVal value As OctTreeNode)
        Me.mRootNode = value
    End Sub


    Private mRenderNodes As Boolean
    Public Property RenderNodes() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRenderNodes
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mRenderNodes = value
        End Set
    End Property


    Public Sub AddEntity(ByVal nEnt As Entity)
        Me.Move(nEnt)
        Me.RootNode.OrdenEntity(nEnt)
    End Sub


    Private mDrawBoundingBoxes As Boolean
    Public Property DrawBoundingBoxes() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDrawBoundingBoxes
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mDrawBoundingBoxes = value
        End Set
    End Property

    Public Class RenderWorldEventArgs
        Inherits EventArgs

    End Class

    Public Sub OnBeforeRenderWorld(ByVal sender As Object, ByVal e As RenderWorldEventArgs)
        Me.RenderContainer.OnBeforeRender(Me, New RenderElement.RenderEventArgs)


    End Sub

    Public Sub OnRenderWorld(ByVal sender As Object, ByVal e As RenderWorldEventArgs)
        Me.RenderContainer.OnRender(Me, New RenderElement.RenderEventArgs)

    End Sub

    Public Sub OnAfterRenderWorld(ByVal sender As Object, ByVal e As RenderWorldEventArgs)
        Me.RenderContainer.OnAfterRender(Me, New RenderElement.RenderEventArgs)

    End Sub



End Class

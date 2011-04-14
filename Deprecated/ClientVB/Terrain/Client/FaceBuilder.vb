Imports Microsoft.DirectX.Direct3D.CustomVertex
Namespace ClientTerrain
    Public Class FaceBuilder

        Public Sub New()
            Me.setVerts(New List(Of Vector3))
            Me.setIndices(New List(Of Integer))
            Me.NextVertexIndex = 0
        End Sub


        Private mVerts As List(Of Vector3)
        Public ReadOnly Property Verts() As List(Of Vector3)
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mVerts
            End Get
        End Property
        Private Sub setVerts(ByVal value As List(Of Vector3))
            Me.mVerts = value
        End Sub


        Private mIndices As List(Of Integer)
        Public ReadOnly Property Indices() As List(Of Integer)
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mIndices
            End Get
        End Property
        Private Sub setIndices(ByVal value As List(Of Integer))
            Me.mIndices = value
        End Sub


        Private NextVertexIndex As Integer



        Private mCenter As Vector2
        Public ReadOnly Property Center() As Vector2
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCenter
            End Get
        End Property
        Private Sub setCenter(ByVal value As Vector2)
            Me.mCenter = value
        End Sub


        Private mSize As Vector2
        Public ReadOnly Property Size() As Vector2
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mSize
            End Get
        End Property
        Private Sub setSize(ByVal value As Vector2)
            Me.mSize = value
        End Sub


        Private mHeights As Single()
        Public ReadOnly Property Heights() As Single()
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mHeights
            End Get
        End Property
        Private Sub setHeights(ByVal value As Single())
            Me.mHeights = value
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





        Private mBuilding As Boolean
        Public Property Building() As Boolean
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mBuilding
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
                Me.mBuilding = value
            End Set
        End Property


        Public Sub StartBuilding(ByVal nCenter As Vector2, ByVal nSize As Vector2, ByVal nHeights As Single(), ByVal nCornerHeights As Single())
            Me.Building = True
            Me.setCenter(nCenter)
            Me.setSize(nSize)
            Me.setHeights(nHeights)
            Me.setCornerHeights(nCornerHeights)
        End Sub

        Public Sub EndBuilding()
            Me.Building = False
        End Sub


        Private Sub AddFace(ByVal V1 As Vector3, ByVal V2 As Vector3, ByVal V3 As Vector3)
            Me.Verts.Add(V1)
            Me.Indices.Add(Me.NextVertexIndex)
            Me.NextVertexIndex += 1S

            Me.Verts.Add(V2)
            Me.Indices.Add(Me.NextVertexIndex)
            Me.NextVertexIndex += 1S

            Me.Verts.Add(V3)
            Me.Indices.Add(Me.NextVertexIndex)
            Me.NextVertexIndex += 1S
        End Sub
        Public Sub AddFace(ByVal Dir1 As VertexDir, ByVal Dir2 As VertexDir, ByVal Dir3 As VertexDir)
            If Not Me.Building Then Throw New Exception

            Me.AddVertex(Dir1)
            Me.AddVertex(Dir2)
            Me.AddVertex(Dir3)
        End Sub

        Public Function AddVertex(ByVal Dir As VertexDir) As Integer
            Dim V As Vector3 = Me.BuildVertex(Dir)
            Me.Verts.Add(V)
            Me.Indices.Add(Me.NextVertexIndex)
            Me.NextVertexIndex += 1
            Return Me.NextVertexIndex - 1

        End Function

        Public Function BuildVertex(ByVal Dir As VertexDir) As Vector3
            If Not Me.Building Then Throw New Exception
            Dim V As Vector3
            Select Case Dir
                Case VertexDir.Center
                    V.X = Me.Center.X
                    V.Y = Me.Heights(HeightDir.Center)
                    V.Z = Me.Center.Y
                Case VertexDir.East
                    V.X = Me.Center.X + Me.Size.X
                    V.Y = Me.Heights(HeightDir.East)
                    V.Z = Me.Center.Y
                Case VertexDir.North
                    V.X = Me.Center.X
                    V.Y = Me.Heights(HeightDir.North)
                    V.Z = Me.Center.Y + Me.Size.Y
                Case VertexDir.West
                    V.X = Me.Center.X - Me.Size.X
                    V.Y = Me.Heights(HeightDir.West)
                    V.Z = Me.Center.Y
                Case VertexDir.South
                    V.X = Me.Center.X
                    V.Y = Me.Heights(HeightDir.South)
                    V.Z = Me.Center.Y - Me.Size.Y


                Case VertexDir.NorthEast
                    V.X = Me.Center.X + Me.Size.X
                    V.Y = Me.CornerHeights(CornerDir.NorthEast)
                    V.Z = Me.Center.Y + Me.Size.Y
                Case VertexDir.NorthWest
                    V.X = Me.Center.X - Me.Size.X
                    V.Y = Me.CornerHeights(CornerDir.NorthWest)
                    V.Z = Me.Center.Y + Me.Size.Y
                Case VertexDir.SouthWest
                    V.X = Me.Center.X - Me.Size.X
                    V.Y = Me.CornerHeights(CornerDir.SouthWest)
                    V.Z = Me.Center.Y - Me.Size.Y
                Case VertexDir.SouthEast
                    V.X = Me.Center.X + Me.Size.X
                    V.Y = Me.CornerHeights(CornerDir.SouthEast)
                    V.Z = Me.Center.Y - Me.Size.Y

            End Select

            Return V
        End Function
    End Class

End Namespace
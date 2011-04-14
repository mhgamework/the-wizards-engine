Imports MHGameWork.TheWizards.Common.Terrain
Namespace Terrain.Client
    Public Class RootSquare
        Inherits Square




        Private mDX As AHDirectX
        Public ReadOnly Property DX() As AHDirectX
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mDX
            End Get
        End Property
        Private Sub setDX(ByVal value As AHDirectX)
            Me.mDX = value
        End Sub



        Private mBFSR As BlockFileStreamReader
        Public ReadOnly Property BFSR() As BlockFileStreamReader
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mBFSR
            End Get
        End Property
        Private Sub setBFSR(ByVal value As BlockFileStreamReader)
            Me.mBFSR = value
        End Sub



        Private mDetailThreshold As Single
        Public Property DetailThreshold() As Single
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mDetailThreshold
            End Get
            <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
                Me.mDetailThreshold = value
            End Set
        End Property




        Public Sub Render()

            Me.DX.SetTexture(0, Nothing)
            Dim mat As New Material
            mat.Diffuse = Color.White
            mat.Ambient = Color.White
            Me.DX.SetMaterial(mat)
            Me.DX.Device.VertexFormat = Direct3D.CustomVertex.PositionNormalColored.Format
            Me.DX.Device.Transform.World = Matrix.Identity


            Me.RenderAux(True) ', Visibility.No_Clip)B

        End Sub

















        Public Sub New(ByVal nHoofdObj As HoofdObject, ByVal nBFSR As BlockFileStreamReader, ByVal nPointer As BlockPointer) ', ByVal nCenter As Vector2, ByVal nSizeRadius As Single)
            MyBase.New(nPointer)
            Me.setRootSquare(Me)
            Me.setBFSR(nBFSR)
            Me.setDX(nHoofdObj.DevContainer.DX)
        End Sub

        'Public Sub BuildActor()

        'End Sub




        'Public Overloads Sub Update()
        '    'Me.Update(Me.CreateCornerData)

        'End Sub

        'Public Overloads Function BuildFaces() As FaceBuilder
        '    Dim FB As New FaceBuilder
        '    Me.BuildFaces(FB, Me.CreateCornerData)

        '    Return FB
        'End Function

        'Public Overloads Sub Split()
        '    MyBase.Split(Me.CreateCornerData)
        'End Sub

        'Public Overrides Function CreateCornerData() As CornerData
        '    Return New CornerData(Me.Center, Me.SizeRadius, Me)
        'End Function




    End Class
End Namespace
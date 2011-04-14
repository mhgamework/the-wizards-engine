Namespace Terrain.Preprocessor
    Public Class RootSquare
        Inherits Square

        Public Sub New(ByVal nHoofdObj As HoofdObject, ByVal nCenter As Vector2, ByVal nSizeRadius As Single)
            MyBase.New(nCenter, nSizeRadius)
            Me.setSizeRadius(nSizeRadius)
            Me.setCenter(nCenter)
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


        Private mCenter As Vector2
        Public ReadOnly Property Center() As Vector2
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mCenter
            End Get
        End Property
        Private Sub setCenter(ByVal value As Vector2)
            Me.mCenter = value
        End Sub


        Public Overrides Function CreateCornerData() As CornerData
            Return New CornerData(Me.Center, Me.SizeRadius, Me)
        End Function


    End Class
End Namespace
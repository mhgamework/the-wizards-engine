Public Class PrecompBlock
    Implements IBlock

    Private Sub New()
        Me.setChildPointers(New BlockPointer(4 - 1) {})
        Me.setErrors(New Single(6 - 1) {})
        Me.setHeights(New Single(9 - 1) {})


    End Sub

    Public Sub New(ByVal nBP As BlockPointer)
        Me.New()
        Me.setPointer(nBP)
    End Sub


    Private mPointer As BlockPointer
    Public ReadOnly Property Pointer() As BlockPointer Implements IBlock.Pointer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPointer
        End Get
    End Property
    Private Sub setPointer(ByVal value As BlockPointer)
        Me.mPointer = value
    End Sub

    '4 children + positie(2) + sizeradius + 9 heights + 6 errors + MinY + MaxY
    Public Const BlockSize As Short = 4 * 4 + 4 * 2 + 4 + 4 * 9 + 4 * 6 + 4 + 4


    Public ReadOnly Property Size() As Short Implements IBlock.Size
        Get
            Return BlockSize
        End Get
    End Property



    Private mChildPointers As BlockPointer()
    Public ReadOnly Property ChildPointers() As BlockPointer()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mChildPointers
        End Get
    End Property
    Private Sub setChildPointers(ByVal value As BlockPointer())
        Me.mChildPointers = value
    End Sub


    Public Property ChildPointer(ByVal dir As Terrain.ChildDir) As BlockPointer
        Get
            Return Me.ChildPointers(dir)
        End Get
        Set(ByVal value As BlockPointer)
            Me.ChildPointers(dir) = value
        End Set
    End Property

    Public Property ChildPointerByte(ByVal index As Byte) As BlockPointer
        Get
            Return Me.ChildPointers(index)
        End Get
        Set(ByVal value As BlockPointer)
            Me.ChildPointers(index) = value
        End Set
    End Property


    Private mPositie As Vector2
    Public Property Positie() As Vector2
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPositie
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector2)
            Me.mPositie = value
        End Set
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



    Private mErrors As Single()
    Public ReadOnly Property Errors() As Single()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mErrors
        End Get
    End Property
    Private Sub setErrors(ByVal value As Single())
        Me.mErrors = value
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


    Public Property Height(ByVal dir As Terrain.VertexDir) As Single
        Get
            Return Me.Heights(dir)
        End Get
        Set(ByVal value As Single)
            Me.Heights(dir) = value
        End Set
    End Property

    Public Property [Error](ByVal type As Terrain.ErrorType) As Single
        Get
            Return Me.Errors(type)
        End Get
        Set(ByVal value As Single)
            Me.Errors(type) = value
        End Set
    End Property

    Public Property ErrorByte(ByVal index as Byte ) As Single
        Get
            Return Me.Errors(index)
        End Get
        Set(ByVal value As Single)
            Me.Errors(index) = value
        End Set
    End Property



    Private mMinY As Single
    Public Property MinY() As Single
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mMinY
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
            Me.mMinY = value
        End Set
    End Property


    Private mMaxY As Single
    Public Property MaxY() As Single
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mMaxY
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Single)
            Me.mMaxY = value
        End Set
    End Property







    Public Function ToBytes() As Byte() Implements IBlock.ToBytes
        Using BW As ByteWriter = New ByteWriter
            For IB As Byte = 0 To 4 - 1
                BW.Write(Me.ChildPointerByte(IB).Address)
            Next

            BW.Write(Me.Positie.X)
            BW.Write(Me.Positie.Y)
            BW.Write(Me.SizeRadius)
            For IB As Integer = 0 To 9 - 1
                BW.Write(Me.Heights(IB))
            Next
            For IB As Byte = 0 To 6 - 1
                BW.Write(Me.Errors(IB))
            Next

            BW.Write(Me.MinY)
            BW.Write(Me.MaxY)

            Return BW.ToBytes
        End Using
    End Function

    Public Shared Function FromBytes(ByVal BFSR As BlockFileStreamReader, ByVal BP As BlockPointer) As PrecompBlock
        Return FromBytes(BFSR.ReadBlockBytes(BP), BP)
    End Function

    Public Shared Function FromBytes(ByVal B As Byte(), ByVal BP As BlockPointer) As PrecompBlock
        Dim Bl As New PrecompBlock
        Bl.setPointer(BP)
        Using BR As New ByteReader(B)
            For IB As Byte = 0 To 4 - 1
                Dim Addr As Integer = BR.ReadInt32
                If Addr = -1 Then
                    Bl.ChildPointerByte(IB) = BlockPointer.Empty
                Else
                    Bl.ChildPointerByte(IB) = New BlockPointer(Addr)
                End If
            Next
            Bl.Positie = New Vector2(BR.ReadSingle, BR.ReadSingle)
            Bl.SizeRadius = BR.ReadSingle
            For IB As Integer = 0 To 9 - 1
                Bl.Heights(IB) = BR.ReadSingle
            Next
            For IB As Byte = 0 To 6 - 1
                Bl.Errors(IB) = BR.ReadSingle
            Next
            Bl.MinY = BR.ReadSingle
            Bl.MaxY = BR.ReadSingle

        End Using

        Return Bl
    End Function



End Class

Public Class SquareBlock
    Implements IBlock

    Private Sub New()
        Me.setChildPointers(New BlockPointer(4 - 1) {})
        Me.setHeights(New Single(5 - 1) {})


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

    '4 children + 3 heights
    Public Const BlockSize As Short = 4 * 4 + 5 * 4


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


    


    Private mHeights As Single()
    Public ReadOnly Property Heights() As Single()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mHeights
        End Get
    End Property
    Private Sub setHeights(ByVal value As Single())
        Me.mHeights = value
    End Sub


    Public Property Height(ByVal dir As Terrain.HeightDir) As Single
        Get
            Return Me.Heights(dir)
        End Get
        Set(ByVal value As Single)
            Me.Heights(dir) = value
        End Set
    End Property


    Public Property HeightByte(ByVal index As Byte) As Single
        Get
            Return Me.Heights(index)
        End Get
        Set(ByVal value As Single)
            Me.Heights(index) = value
        End Set
    End Property






    Public Function ToBytes() As Byte() Implements IBlock.ToBytes
        Using BW As ByteWriter = New ByteWriter
            For IB As Byte = 0 To 4 - 1
                BW.Write(Me.ChildPointerByte(IB).Address)
            Next

            For IB As Integer = 0 To 5 - 1
                BW.Write(Me.Heights(IB))
            Next


            Return BW.ToBytes
        End Using
    End Function

    Public Shared Function FromBytes(ByVal B As Byte(), ByVal BP As BlockPointer) As SquareBlock
        Dim Bl As New SquareBlock
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

            For IB As Integer = 0 To 5 - 1
                Bl.Heights(IB) = BR.ReadSingle
            Next



        End Using

        Return Bl
    End Function



End Class

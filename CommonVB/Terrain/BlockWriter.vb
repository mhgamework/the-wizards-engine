Public Class BlockFileStream
    Implements IDisposable


    Public Sub New(ByVal nFS As IO.FileStream, ByVal nBlockLength As Short)
        Me.setFS(nFS)
        Me.setBW(New ByteWriter(FS))
        Me.setNextBlockStart(0)
        Me.setBlockLength(nBlockLength)
    End Sub


    Private mFS As IO.FileStream
    Public ReadOnly Property FS() As IO.FileStream
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFS
        End Get
    End Property
    Private Sub setFS(ByVal value As IO.FileStream)
        Me.mFS = value
    End Sub


    Private mBW As ByteWriter
    Public ReadOnly Property BW() As ByteWriter
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBW
        End Get
    End Property
    Private Sub setBW(ByVal value As ByteWriter)
        Me.mBW = value
    End Sub



    Private mBlockLength As Short
    Public ReadOnly Property BlockLength() As Short
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBlockLength
        End Get
    End Property
    Private Sub setBlockLength(ByVal value As Short)
        Me.mBlockLength = value
    End Sub



    Private mNextBlockStart As Integer
    Public ReadOnly Property NextBlockStart() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNextBlockStart
        End Get
    End Property
    Private Sub setNextBlockStart(ByVal value As Integer)
        Me.mNextBlockStart = value
    End Sub

    Public Function CreateBlockPointer() As BlockPointer
        Dim BP As New BlockPointer(Me.NextBlockStart)
        'BW.Seek(BP.Address, IO.SeekOrigin.Begin)
        'Dim B As Byte() = New Byte(Me.BlockLength - 1) {}
        'BW.Write(B)
        Me.setNextBlockStart(Me.NextBlockStart + Me.BlockLength)


        Return BP

    End Function

    Public Function CreateBlock() As Block
        Return New Block(Me.CreateBlockPointer, Me.BlockLength)

    End Function

    Public Sub WriteBlock(ByVal nBlock As IBlock)
        If nBlock.Size <> Me.BlockLength Then Throw New Exception("Block size is niet hetzelfde als de blocklength van de BlockFileStream")
        Dim B As Byte() = nBlock.ToBytes
        If B.Length <> nBlock.Size Then Throw New Exception("This block was not entirely filled or was overfilled!")
        BW.Seek(nBlock.Pointer.Address, IO.SeekOrigin.Begin)
        BW.Write(B)
    End Sub
    

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free unmanaged resources when explicitly called
            End If

            ' TODO: free shared unmanaged resources
            
        End If
        If Me.BW IsNot Nothing Then
            Me.BW.Close()
            Me.setBW(Nothing)
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Public Structure BlockPointer

    Private mAddress As Integer
    Public ReadOnly Property Address() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAddress
        End Get
    End Property
    Private Sub setAddress(ByVal value As Integer)
        Me.mAddress = value
    End Sub



    Public Sub New(ByVal nAddress As Integer)
        If nAddress < 0 Then Throw New Exception
        Me.setAddress(nAddress)
    End Sub


    Public ReadOnly Property IsEmpty() As Boolean
        Get
            Return Me.Address = -1
        End Get
    End Property
    


    Public Shared ReadOnly Property Empty() As BlockPointer
        Get
            Dim P As New BlockPointer
            P.setAddress(-1)
            Return P

        End Get
    End Property
End Structure

'Public Structure BlockWriter
'    Public BP As BlockPointer
'    Public RelativePosition As Short

'    Public Function CalculateOffset() As Integer
'        Return BP.Address + Me.RelativePosition
'    End Function
'    Public Sub New(ByVal nBP As BlockPointer)
'        Me.BP = nBP
'    End Sub
'End Structure

Public Class Block
    Inherits ByteWriter
    Implements IBlock


    Public Sub New(ByVal nBP As BlockPointer, ByVal nBlockLength As Short)
        MyBase.New()
        Me.setPointer(nBP)
        Me.setSize(nBlockLength)
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


    Private mSize As Short
    Public ReadOnly Property Size() As Short Implements IBlock.Size
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSize
        End Get
    End Property
    Private Sub setSize(ByVal value As Short)
        Me.mSize = value
    End Sub


    Private Function ToBytes1() As Byte() Implements IBlock.ToBytes
        Return Me.ToBytes
    End Function
End Class
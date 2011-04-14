Public Class BlockFileStreamReader
    Implements IDisposable

    Public Sub New(ByVal nFS As IO.FileStream, ByVal nBlockLength As Short)
        Me.setFS(nFS)
        Me.setBR(New ByteReader(FS))
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



    Private mBR As ByteReader
    Public ReadOnly Property BR() As ByteReader
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBR
        End Get
    End Property
    Private Sub setBR(ByVal value As ByteReader)
        Me.mBR = value
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


    Public Function CreateBlock() As Block
        Dim BP As New BlockPointer(Me.NextBlockStart)
        'BW.Seek(BP.Address, IO.SeekOrigin.Begin)
        'Dim B As Byte() = New Byte(Me.BlockLength - 1) {}
        'BW.Write(B)
        Me.setNextBlockStart(Me.NextBlockStart + Me.BlockLength)


        Return New Block(BP, Me.BlockLength)

    End Function

    Public Function ReadBlockBytes(ByVal nPointer As BlockPointer) As Byte()
        BR.BaseStream.Seek(nPointer.Address, IO.SeekOrigin.Begin)
        Dim B As Byte() = BR.ReadBytes(Me.BlockLength)

        Return B
    End Function

    Public Function ReadBlock(ByVal nPointer As BlockPointer) As BlockReader
        Return New BlockReader(nPointer, Me.ReadBlockBytes(nPointer))
    End Function

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free unmanaged resources when explicitly called
            End If

            ' TODO: free shared unmanaged resources
        End If

        If Me.BR IsNot Nothing Then
            Me.BR.Close()
            Me.setBR(Nothing)
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

Public Class BlockReader
    Inherits ByteReader

    Public BP As BlockPointer


    Public Sub New(ByVal nBP As BlockPointer, ByVal nBlockData As Byte())
        MyBase.New(nBlockData)
        Me.BP = nBP

    End Sub

End Class
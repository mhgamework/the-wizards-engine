Public Class SerializedDataChunk

    Public Sub New(ByVal nName As String, ByVal nData As Byte())
        Me.setName(nName)
        Me.setData(nData)
    End Sub

    Private Sub New()

    End Sub


    Private mName As String
    Public ReadOnly Property Name() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mName
        End Get
    End Property
    Private Sub setName(ByVal value As String)
        Me.mName = value
    End Sub


    Private mData As Byte()
    Public ReadOnly Property Data() As Byte()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mData
        End Get
    End Property
    Private Sub setData(ByVal value As Byte())
        Me.mData = value
    End Sub

    Public Sub ChangeData(ByVal newData As Byte())
        Me.setData(newData)
    End Sub


    Public Function ToBytes() As Byte()
        Dim BW As New ByteWriter
        BW.Write(Me.Name)
        BW.Write(Me.Data.Length)
        BW.Write(Me.Data)


        Dim B As Byte() = BW.ToBytes
        BW.Close()

        Return B
    End Function

    Public Shared Function FromBytes(ByVal BR As ByteReader) As SerializedDataChunk
        Dim SDC As New SerializedDataChunk
        SDC.setName(BR.ReadString)
        SDC.setData(BR.ReadBytes(BR.ReadInt32))

        Return SDC
    End Function

End Class

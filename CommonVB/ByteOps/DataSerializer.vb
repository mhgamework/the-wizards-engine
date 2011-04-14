Public Class DataSerializer
    Public Sub New()
        Me.NewStandard()
    End Sub
    'Private Sub New(ByVal nBR As ByteReader)
    '    Me.NewStandard()

    'End Sub

    Private Sub NewStandard()
        Me.setChunks(New Dictionary(Of String, SerializedDataChunk))
    End Sub



    Private mBW As Type
    Public ReadOnly Property BW() As Type
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBW
        End Get
    End Property
    Private Sub setBW(ByVal value As Type)
        Me.mBW = value
    End Sub


    Private mChunks As Dictionary(Of String, SerializedDataChunk)
    Public ReadOnly Property Chunks() As Dictionary(Of String, SerializedDataChunk)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mChunks
        End Get
    End Property
    Private Sub setChunks(ByVal value As Dictionary(Of String, SerializedDataChunk))
        Me.mChunks = value
    End Sub

    Public Sub SetData(ByVal Name As String, ByVal value As Int32)
        Me.SetData(Name, BitConverter.GetBytes(value))
    End Sub

    Public Sub SetData(ByVal Name As String, ByVal value As Vector3)
        Me.SetData(Name, ByteConverter.GetBytes(value))
    End Sub

    Public Sub SetData(ByVal Name As String, ByVal value As Quaternion)
        Me.SetData(Name, ByteConverter.GetBytes(value))
    End Sub

    Public Sub SetData(ByVal Name As String, ByVal value As String)
        Me.SetData(Name, System.Text.ASCIIEncoding.ASCII.GetBytes(value))
    End Sub

    Public Sub SetData(ByVal Name As String, ByVal value As Byte())
        Dim C As SerializedDataChunk = Nothing
        If Me.Chunks.TryGetValue(Name, C) Then
            C.ChangeData(value)
        Else
            Me.AddChunck(Name, value)
        End If

    End Sub

    Public Function GetDataInt32(ByVal Name As String, ByVal nDefaultValue As Int32) As Int32
        Dim B As Byte() = Nothing
        If Me.GetData(Name, B) Then
            Return BitConverter.ToInt32(B, 0)
        Else
            Return nDefaultValue
        End If
    End Function

    Public Function GetDataVector3(ByVal Name As String, ByVal nDefaultValue As Vector3) As Vector3
        Dim B As Byte() = Nothing
        If Me.GetData(Name, B) Then
            Dim BR As New ByteReader(B)
            Dim V As Vector3 = BR.ReadVector3
            BR.Close()
            Return V
        Else
            Return nDefaultValue
        End If
    End Function

    Public Function GetDataQuaternion(ByVal Name As String, ByVal nDefaultValue As Quaternion) As Quaternion
        Dim B As Byte() = Nothing
        If Me.GetData(Name, B) Then
            Dim BR As New ByteReader(B)
            Dim Q As Quaternion = BR.ReadQuaternion
            BR.Close()
            Return Q
        Else
            Return nDefaultValue
        End If
    End Function

    Public Function GetDataString(ByVal Name As String, ByVal nDefaultValue As String) As String
        Dim B As Byte() = Nothing
        If Me.GetData(Name, B) Then

            Dim str As String = System.Text.ASCIIEncoding.ASCII.GetString(B)

            Return str
        Else
            Return nDefaultValue
        End If
    End Function



    Public Function GetData(ByVal Name As String, ByRef outData As Byte()) As Boolean
        Dim C As SerializedDataChunk = Nothing
        If Me.Chunks.TryGetValue(Name, C) Then
            outData = C.Data
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub AddChunck(ByVal Name As String, ByVal Data As Byte())
        Me.AddChunck(New SerializedDataChunk(Name, Data))
    End Sub

    Private Sub AddChunck(ByVal nChunck As SerializedDataChunk)
        Me.Chunks.Add(nChunck.Name, nChunck)
    End Sub

    Public Function ToBytes() As Byte()
        Dim BW As New ByteWriter

        BW.Write(Me.Chunks.Count)
        For Each C As SerializedDataChunk In Me.Chunks.Values
            BW.Write(C.ToBytes)
        Next




        Dim B As Byte() = BW.ToBytes
        BW.Close()

        Return B
    End Function

    Public Shared Function FromBytes(ByVal nBytes As Byte()) As DataSerializer
        Dim BR As New ByteReader(nBytes)
        Dim DS As New DataSerializer()

        Dim NumChunks As Integer
        NumChunks = BR.ReadInt32
        For I As Integer = 0 To NumChunks - 1
            DS.AddChunck(SerializedDataChunk.FromBytes(BR))
        Next

        BR.Close()

        Return DS
    End Function

End Class

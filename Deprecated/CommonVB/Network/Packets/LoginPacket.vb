Public Class LoginPacket
    Implements TheWizards.Common.Networking.INetworkSerializable


    Public Sub New(ByVal nUsername As String, ByVal nPassword As Byte())
        Me.setUsername(nUsername)
        Me.setPassword(nPassword)
    End Sub


    Private mUsername As String
    Public ReadOnly Property Username() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mUsername
        End Get
    End Property
    Private Sub setUsername(ByVal value As String)
        Me.mUsername = value
    End Sub


    Private mPassword As Byte()
    Public ReadOnly Property Password() As Byte()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPassword
        End Get
    End Property
    Private Sub setPassword(ByVal value As Byte())
        Me.mPassword = value
    End Sub



    Public Function ToNetworkBytes() As Byte() Implements Common.Networking.INetworkSerializable.ToNetworkBytes
        Dim BW As New ByteWriter
        BW.Write(Me.Username)
        BW.Write(Me.Password.Length)
        BW.Write(Me.Password)
        Return BW.ToBytesAndClose
    End Function

    Public Shared Function FromNetworkBytes(ByVal BR As ByteReader) As LoginPacket
        Return New LoginPacket(BR.ReadString, BR.ReadBytes(BR.ReadInt32))
    End Function
End Class

<Serializable()> Public Class NetworkPacket
    Implements Runtime.Serialization.ISerializable

    Public Sub New()

    End Sub

    Public Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
        Stop
    End Sub

    Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
        info.AddValue("1", "beep")
        info.AddValue("2", 4)
    End Sub
End Class

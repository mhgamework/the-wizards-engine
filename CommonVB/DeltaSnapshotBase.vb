Public MustInherit Class DeltaSnapshotBase
    Implements INetworkSerializable

    Public MustOverride Sub WriteBytes(ByVal BW As ByteWriter)
    Public MustOverride Sub ReadBytes(ByVal BR As ByteReader)




    Public Function ToNetworkBytes() As Byte() Implements Networking.INetworkSerializable.ToNetworkBytes
        Dim BW As New ByteWriter
        Me.WriteBytes(BW)

        Dim B As Byte() = BW.ToBytes
        BW.Close()
        Return B
    End Function


End Class

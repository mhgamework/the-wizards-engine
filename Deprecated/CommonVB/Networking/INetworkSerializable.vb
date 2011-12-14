'Namespace Common
Namespace Networking
    Public Interface INetworkSerializable

        'Function FromBytes(ByVal BR As ByteReader) As GameFilesList



        Function ToNetworkBytes() As Byte()


    End Interface
    'End Namespace
End Namespace
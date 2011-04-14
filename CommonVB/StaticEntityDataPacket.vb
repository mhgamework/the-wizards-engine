Public Class StaticEntityDataPacket
    Implements INetworkSerializable



    'Private mPositie As Vector3
    'Public Property Positie() As Vector3
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mPositie
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
    '        Me.mPositie = value
    '    End Set
    'End Property



    'Private mRotatieQuat As Quaternion
    'Public Property RotatieQuat() As Quaternion
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mRotatieQuat
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Quaternion)
    '        Me.mRotatieQuat = value
    '    End Set
    'End Property



    'Private mScale As Vector3
    'Public Property Scale() As Vector3
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mScale
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
    '        Me.mScale = value
    '    End Set
    'End Property


    'Private mVersie As Integer
    'Public Property Versie() As Integer
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mVersie
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    '        Me.mVersie = value
    '    End Set
    'End Property



    Private mModelID As Integer
    Public Property ModelID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelID
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
            Me.mModelID = value
        End Set
    End Property


    Private mModelVersie As Integer
    Public Property ModelVersie() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelVersie
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
            Me.mModelVersie = value
        End Set
    End Property



    Public Function ToNetworkBytes() As Byte() Implements Networking.INetworkSerializable.ToNetworkBytes
        Dim BW As New ByteWriter
        'BW.Write(Me.Positie)
        'BW.Write(Me.RotatieQuat)
        'BW.Write(Me.Scale)
        'BW.Write(Me.Versie)
        BW.Write(Me.ModelID)
        BW.Write(Me.ModelVersie)

        Dim B As Byte() = BW.ToBytes
        BW.Close()

        Return b
    End Function

    Public Shared Function FromNetworkBytes(ByVal BR As ByteReader) As StaticEntityDataPacket
        Dim P As New StaticEntityDataPacket
        'P.Positie = BR.ReadVector3
        'P.RotatieQuat = BR.ReadQuaternion
        'P.Scale = BR.ReadVector3
        'P.Versie = BR.ReadInt32
        P.ModelID = BR.ReadInt32
        P.ModelVersie = BR.ReadInt32


        Return P
    End Function
End Class

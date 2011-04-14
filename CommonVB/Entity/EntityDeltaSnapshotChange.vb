Public Class EntityDeltaSnapshotChange
    Inherits DeltaSnapshotChange

    Public Enum ChangeType As Byte
        Positie = 1
        Snelheid
        Rotatie
        RotatieSnelheid
        Scale
    End Enum

    Protected Sub New(ByVal nType As ChangeType, ByVal nData As Byte())
        Me.setType(nType)
        Me.setData(nData)
    End Sub

    Protected Sub New()

    End Sub

    Public Shared Function CreatePositieChange(ByVal nPositie As Vector3) As EntityDeltaSnapshotChange
        Return New EntityDeltaSnapshotChange(ChangeType.Positie, ByteConverter.GetBytes(nPositie))
    End Function
    Public Shared Function CreateSnelheidChange(ByVal nSnelheid As Vector3) As EntityDeltaSnapshotChange
        Return New EntityDeltaSnapshotChange(ChangeType.Snelheid, ByteConverter.GetBytes(nSnelheid))
    End Function
    Public Shared Function CreateRotatieChange(ByVal nRotatieQuat As Quaternion) As EntityDeltaSnapshotChange
        Return New EntityDeltaSnapshotChange(ChangeType.Rotatie, ByteConverter.GetBytes(nRotatieQuat))
    End Function
    Public Shared Function CreateRotatieSnelheidChange(ByVal nRotatieSnelheid As Vector3) As EntityDeltaSnapshotChange
        Return New EntityDeltaSnapshotChange(ChangeType.RotatieSnelheid, ByteConverter.GetBytes(nRotatieSnelheid))
    End Function
    Public Shared Function CreateScaleChange(ByVal nScale As Vector3) As EntityDeltaSnapshotChange
        Return New EntityDeltaSnapshotChange(ChangeType.Scale, ByteConverter.GetBytes(nScale))
    End Function

    Private mType As ChangeType
    Public ReadOnly Property Type() As ChangeType
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mType
        End Get
    End Property
    Private Sub setType(ByVal value As ChangeType)
        Me.mType = value
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




    Public Overrides Sub WriteBytes(ByVal BW As Common.ByteWriter)
        BW.Write(Me.Type)
        BW.Write(Me.Data.Length)
        BW.Write(Me.Data)
    End Sub

    Public Overrides Sub ReadBytes(ByVal BR As ByteReader)
        Me.setType(CType(BR.ReadByte, ChangeType))
        Me.setData(BR.ReadBytes(BR.ReadInt32))
    End Sub

    Public Shared Function CreateFromByteReader(ByVal BR As ByteReader) As EntityDeltaSnapshotChange
        Dim EDSC As New EntityDeltaSnapshotChange
        EDSC.ReadBytes(BR)
        Return EDSC
    End Function


    Public Function GetPositie() As Vector3
        If Me.Type = ChangeType.Positie Then
            Return New Vector3(BitConverter.ToSingle(Me.Data, 0), BitConverter.ToSingle(Me.Data, 4), BitConverter.ToSingle(Me.Data, 8))
        End If
    End Function

    Public Function GetQuaternion() As Quaternion
        If Me.Type = ChangeType.Rotatie Then
            Return New Quaternion(BitConverter.ToSingle(Me.Data, 0), BitConverter.ToSingle(Me.Data, 4), BitConverter.ToSingle(Me.Data, 8), BitConverter.ToSingle(Me.Data, 12))
        Else
            Stop
        End If
    End Function
End Class

Public Class DynamicEntityDeltaSnapshot
    Implements INetworkSerializable

    Public Sub New()

    End Sub


    Public Enum ChangeType As Byte
        None = 0
        Positie = 1
        'Snelheid=2
        'Rotatie=4
        'RotatieSnelheid=8
        'Scale=16
    End Enum

    Private mPositieChanged As Boolean
    Public ReadOnly Property PositieChanged() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPositieChanged
        End Get
    End Property
    Private Sub setPositieChanged(ByVal value As Boolean)
        Me.mPositieChanged = value
    End Sub


    Private mPositie As Vector3
    Public ReadOnly Property Positie() As Vector3
        Get
            If Me.PositieChanged = False Then Throw New Exception("Positie is niet veranderd, dus ook niet meegestuurd in de deltasnapshot")
            Return Me.mPositie
        End Get
    End Property
    Private Sub setPositie(ByVal value As Vector3)
        Me.mPositie = value
    End Sub

    Public Sub SetPositieChange(ByVal nPos As Vector3)
        Me.setPositieChanged(True)
        Me.setPositie(nPos)
    End Sub




    Public Function ToNetworkBytes() As Byte() Implements Common.Networking.INetworkSerializable.ToNetworkBytes
        Dim BW As New ByteWriter

        Dim Changes As ChangeType = ChangeType.None
        If Me.PositieChanged Then Changes = Changes Or ChangeType.Positie

        BW.Write(Changes)

        If Me.PositieChanged Then
            BW.Write(Me.Positie)
        End If





        Dim B As Byte() = BW.ToBytes
        BW.Close()

        Return B
    End Function

    Public Shared Function FromNetworkBytes(ByVal BR As ByteReader) As DynamicEntityDeltaSnapshot
        Dim DS As New DynamicEntityDeltaSnapshot

        Dim Changes As ChangeType = CType(BR.ReadByte, ChangeType)
        'MsgBox("type: " & CInt(Changes).ToString)



        If (Changes And ChangeType.Positie) <> ChangeType.None Then
            DS.setPositieChanged(True)
            DS.setPositie(BR.ReadVector3)
        End If

        Return DS

    End Function
End Class

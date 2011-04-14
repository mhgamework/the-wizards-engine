Public Class EntityDeltaSnapshot
    Inherits DeltaSnapshotBase


    Public Sub New(ByVal nEntID As Integer, ByVal nINS As INetworkSerializable)
        Me.New(nEntID, nINS.ToNetworkBytes)
    End Sub

    Public Sub New(ByVal nEntID As Integer, ByVal nData As Byte())
        Me.New()

        Me.setEntityID(nEntID)
        Me.setData(nData)
    End Sub
    Protected Sub New()
        'Me.setDeltaSnapshotChanges(New List(Of DeltaSnapshotChange))
        'Me.setEntityDeltaSnaphostChanges(New List(Of EntityDeltaSnapshotChange))
    End Sub


    Private mEntityID As Integer
    Public ReadOnly Property EntityID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntityID
        End Get
    End Property
    Private Sub setEntityID(ByVal value As Integer)
        Me.mEntityID = value
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



    'Private mDeltaSnapshotChanges As List(Of DeltaSnapshotChange)
    'Protected ReadOnly Property DeltaSnapshotChanges() As List(Of DeltaSnapshotChange)
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mDeltaSnapshotChanges
    '    End Get
    'End Property
    'Private Sub setDeltaSnapshotChanges(ByVal value As List(Of DeltaSnapshotChange))
    '    Me.mDeltaSnapshotChanges = value
    'End Sub


    'Private mEntityDeltaSnaphostChanges As List(Of EntityDeltaSnapshotChange)
    'Public ReadOnly Property EntityDeltaSnaphostChanges() As List(Of EntityDeltaSnapshotChange)
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mEntityDeltaSnaphostChanges
    '    End Get
    'End Property
    'Private Sub setEntityDeltaSnaphostChanges(ByVal value As List(Of EntityDeltaSnapshotChange))
    '    Me.mEntityDeltaSnaphostChanges = value
    'End Sub


    'Public Sub AddDeltaSnapshotChange(ByVal nC As DeltaSnapshotChange)
    '    Me.DeltaSnapshotChanges.Add(nC)
    '    If nC.GetType.Equals(GetType(EntityDeltaSnapshotChange)) Then
    '        Dim EDSC As EntityDeltaSnapshotChange = DirectCast(nC, EntityDeltaSnapshotChange)
    '        Me.EntityDeltaSnaphostChanges.Add(EDSC)
    '        Select Case EDSC.Type
    '            Case EntityDeltaSnapshotChange.ChangeType.Positie
    '                Me.setPositieChange(EDSC)
    '            Case EntityDeltaSnapshotChange.ChangeType.Snelheid
    '                Me.setSnelheidChange(EDSC)
    '            Case EntityDeltaSnapshotChange.ChangeType.Rotatie
    '                Me.setRotatieChange(EDSC)
    '            Case EntityDeltaSnapshotChange.ChangeType.RotatieSnelheid
    '                Me.setRotatieSnelheidChange(EDSC)
    '            Case EntityDeltaSnapshotChange.ChangeType.Scale
    '        End Select
    '    End If
    'End Sub

    Public Overrides Sub WriteBytes(ByVal BW As Common.ByteWriter)
        BW.WriteCompressed(Me.EntityID)
        BW.WriteCompressed(Me.Data.Length)
        BW.Write(Me.Data)
        'BW.WriteCompressed(Me.EntityDeltaSnaphostChanges.Count)
        'For I As Integer = 0 To Me.EntityDeltaSnaphostChanges.Count - 1
        '    Me.EntityDeltaSnaphostChanges(I).WriteBytes(BW)
        'Next

    End Sub

    Public Overrides Sub ReadBytes(ByVal BR As ByteReader)
        'Me.DeltaSnapshotChanges.Clear()
        'Me.EntityDeltaSnaphostChanges.Clear()
        Me.setEntityID(BR.ReadCompressedInt32)
        Me.setData(BR.ReadBytes(BR.ReadCompressedInt32))

        'Dim NumEntDSC As Integer = BR.ReadCompressedInt32
        ''Dim EntDSC As EntityDeltaSnapshotChange
        'For I As Integer = 0 To NumEntDSC - 1
        '    Me.AddDeltaSnapshotChange(EntityDeltaSnapshotChange.CreateFromByteReader(BR))
        'Next
    End Sub

    Public Shared Function FromNetworkBytes(ByVal BR As ByteReader) As EntityDeltaSnapshot
        Dim EDS As New EntityDeltaSnapshot
        EDS.ReadBytes(BR)
        Return EDS
    End Function


    'Private mPositieChange As EntityDeltaSnapshotChange
    'Public ReadOnly Property PositieChange() As EntityDeltaSnapshotChange
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mPositieChange
    '    End Get
    'End Property
    'Private Sub setPositieChange(ByVal value As EntityDeltaSnapshotChange)
    '    Me.mPositieChange = value
    'End Sub


    'Private mSnelheidChange As EntityDeltaSnapshotChange
    'Public ReadOnly Property SnelheidChange() As EntityDeltaSnapshotChange
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mSnelheidChange
    '    End Get
    'End Property
    'Private Sub setSnelheidChange(ByVal value As EntityDeltaSnapshotChange)
    '    Me.mSnelheidChange = value
    'End Sub


    'Private mRotatieChange As EntityDeltaSnapshotChange
    'Public ReadOnly Property RotatieChange() As EntityDeltaSnapshotChange
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mRotatieChange
    '    End Get
    'End Property
    'Private Sub setRotatieChange(ByVal value As EntityDeltaSnapshotChange)
    '    Me.mRotatieChange = value
    'End Sub


    'Private mRotatieSnelheidChange As EntityDeltaSnapshotChange
    'Public ReadOnly Property RotatieSnelheidChange() As EntityDeltaSnapshotChange
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mRotatieSnelheidChange
    '    End Get
    'End Property
    'Private Sub setRotatieSnelheidChange(ByVal value As EntityDeltaSnapshotChange)
    '    Me.mRotatieSnelheidChange = value
    'End Sub


End Class

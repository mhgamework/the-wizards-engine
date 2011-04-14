Public Class DeltaSnapshot
    Inherits DeltaSnapshotBase

    Public Sub New()
        'Me.setChildDeltaSnapshots(New List(Of DeltaSnapshotBase))
        Me.setEntityDeltaSnapshots(New List(Of EntityDeltaSnapshot))
    End Sub

    'Private mChildDeltaSnapshots As List(Of DeltaSnapshotBase)
    'Protected ReadOnly Property ChildDeltaSnapshots() As List(Of DeltaSnapshotBase)
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mChildDeltaSnapshots
    '    End Get
    'End Property
    'Private Sub setChildDeltaSnapshots(ByVal value As List(Of DeltaSnapshotBase))
    '    Me.mChildDeltaSnapshots = value
    'End Sub


    Private mEntityDeltaSnapshots As List(Of EntityDeltaSnapshot)
    Public ReadOnly Property EntityDeltaSnapshots() As List(Of EntityDeltaSnapshot)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEntityDeltaSnapshots
        End Get
    End Property
    Private Sub setEntityDeltaSnapshots(ByVal value As List(Of EntityDeltaSnapshot))
        Me.mEntityDeltaSnapshots = value
    End Sub


    Public Sub AddEntityDeltaSnapshot(ByVal nD As EntityDeltaSnapshot)
        'Me.ChildDeltaSnapshots.Add(nD)
        If nD.GetType.Equals(GetType(EntityDeltaSnapshot)) Then
            Me.EntityDeltaSnapshots.Add(DirectCast(nD, EntityDeltaSnapshot))
        End If
    End Sub

    Public Sub MergeDeltaSnapshot(ByVal nDS As DeltaSnapshot)
        For Each EDS As EntityDeltaSnapshot In nDS.EntityDeltaSnapshots
            Me.AddEntityDeltaSnapshot(EDS)
        Next
    End Sub



    Public Overrides Sub WriteBytes(ByVal BW As Common.ByteWriter)
        BW.Write(CInt(Me.EntityDeltaSnapshots.Count))
        For I As Integer = 0 To Me.EntityDeltaSnapshots.Count - 1
            Me.EntityDeltaSnapshots(I).WriteBytes(BW)
        Next
    End Sub

    Public Overrides Sub ReadBytes(ByVal BR As ByteReader)
        'Me.ChildDeltaSnapshots.Clear()
        Me.EntityDeltaSnapshots.Clear()
        Dim NumEntDS As Integer = Br.ReadInt32
        'Dim EntDS As EntityDeltaSnapshot
        For I As Integer = 0 To NumEntDS - 1
            Me.AddEntityDeltaSnapshot(EntityDeltaSnapshot.FromNetworkBytes(BR))
        Next


    End Sub

    Public Shared Function CreateFromByteReader(ByVal BR As ByteReader) As DeltaSnapshot
        Dim DS As New DeltaSnapshot
        DS.ReadBytes(BR)
        Return DS
    End Function
End Class

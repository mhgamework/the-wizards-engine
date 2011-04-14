Public Class DynamicClientEntityFunctions
    Inherits ClientEntityFunctions

    Public Sub New(ByVal nParent As Entity)
        MyBase.New(nParent)
        Me.setSnapshots(New SnapshotList(SnapshotBufferLength))
        Me.setPositieInterpolater(New InterpolaterLineairPositie)
        Me.PositieInterpolater.Enabled = True
        Me.setRotatieInterpolater(New InterpolaterLineairRotatie)
        Me.RotatieInterpolater.Enabled = True
    End Sub

    Shared Sub New()

    End Sub

    Shared RenderDelay As Integer = 100
    Shared SnapshotBufferLength As Integer = 100

    Protected WithEvents ProcessElement As New ProcessEventElement(Me)
    Protected WithEvents TickElement As New TickElement(Me)


    Private mSnapshots As SnapshotList
    Public ReadOnly Property Snapshots() As SnapshotList
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSnapshots
        End Get
    End Property
    Private Sub setSnapshots(ByVal value As SnapshotList)
        Me.mSnapshots = value
    End Sub




    Private mPositieInterpolater As InterpolaterLineairPositie
    Public ReadOnly Property PositieInterpolater() As InterpolaterLineairPositie
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPositieInterpolater
        End Get
    End Property
    Private Sub setPositieInterpolater(ByVal value As InterpolaterLineairPositie)
        Me.mPositieInterpolater = value
    End Sub



    Private mRotatieInterpolater As InterpolaterLineairRotatie
    Public ReadOnly Property RotatieInterpolater() As InterpolaterLineairRotatie
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRotatieInterpolater
        End Get
    End Property
    Private Sub setRotatieInterpolater(ByVal value As InterpolaterLineairRotatie)
        Me.mRotatieInterpolater = value
    End Sub




    Public Sub FindInterpolateValues(ByVal CurrTick As Integer)
        Me.PositieInterpolater.StartTime = -1
        Me.PositieInterpolater.EndTime = -1
        Me.RotatieInterpolater.StartTime = -1
        Me.RotatieInterpolater.EndTime = -1
        Me.FindInterpolateStartValues(CurrTick)
        Me.FindInterpolateEndValues(CurrTick)
        'If Me.PositieInterpolater.Enabled Then Me.PositieInterpolater.CheckIfValid()


    End Sub

    Public Sub FindInterpolateStartValues(ByVal CurrTick As Integer)
        Dim ITick As Integer
        Dim ISnap As DeltaSnapshotTick
        Dim Pos As Boolean
        Dim Snelh As Boolean
        Dim Rot As Boolean

        'vind eerste vorige snapshot
        ITick = CurrTick
        Do Until Me.Snapshots(ITick) IsNot Nothing

            ITick -= 1
            If ITick < Me.Snapshots.FirstElementTick Then
                Me.PositieInterpolater.StartTime = -1
                'TODO snelheidinterp
                Me.RotatieInterpolater.StartTime = -1
                Exit Sub
            End If
        Loop

        Me.PositieInterpolater.StartTime = ITick * Me.HoofdObj.TickDuration 'CInt(Me.HoofdObj.TickDurationInSeconds * ITick * 1000)

        'TODO snelheidinterp
        Me.RotatieInterpolater.StartTime = ITick * Me.HoofdObj.TickDuration

        Pos = False
        Snelh = True 'TODO
        Rot = False
        Do Until ITick < Me.Snapshots.FirstElementTick
            ISnap = Me.Snapshots(ITick)
            If ISnap IsNot Nothing Then

                If Pos = False And ISnap.DynamicEDS.PositieChanged Then
                    Me.PositieInterpolater.StartValue = ISnap.DynamicEDS.Positie
                    Pos = True

                End If
                '' ''If Snelh = False And ISnap.EntityDeltaSnapshot.SnelheidChange IsNot Nothing Then
                '' ''    'TODO
                '' ''    Snelh = True
                '' ''End If
                '' ''If Rot = False And ISnap.EntityDeltaSnapshot.RotatieChange IsNot Nothing Then
                '' ''    Me.RotatieInterpolater.StartValue = ISnap.EntityDeltaSnapshot.RotatieChange.GetQuaternion
                '' ''    Rot = True

                '' ''End If

                If Pos And Snelh And Rot Then Exit Do
            End If
            ITick -= 1
        Loop

        'Me.PositieInterpolater.Enabled = Pos


    End Sub

    Public Sub FindInterpolateEndValues(ByVal CurrTick As Integer)
        Dim ITick As Integer
        Dim ISnap As DeltaSnapshotTick
        Dim Pos As Boolean
        Dim Snelh As Boolean
        Dim Rot As Boolean

        ITick = CurrTick + 1
        Pos = False
        Snelh = True  'TODO: snelheidinterp
        Rot = False
        Do Until ITick > Me.Snapshots.LastElementTick
            ISnap = Me.Snapshots(ITick)
            If ISnap IsNot Nothing Then

                If Pos = False And ISnap.DynamicEDS.PositieChanged Then
                    Me.PositieInterpolater.EndTime = ITick * Me.HoofdObj.TickDuration ' CInt(ITick * Me.HoofdObj.TickDurationInSeconds * 1000)
                    Me.PositieInterpolater.EndValue = ISnap.DynamicEDS.Positie
                    Pos = True

                End If
                ' ''If Snelh = False And ISnap.EntityDeltaSnapshot.SnelheidChange IsNot Nothing Then
                ' ''    'TODO
                ' ''    Snelh = True
                ' ''End If
                ' ''If Rot = False And ISnap.EntityDeltaSnapshot.RotatieChange IsNot Nothing Then
                ' ''    Me.RotatieInterpolater.EndTime = ITick * Me.HoofdObj.TickDuration ' CInt(ITick * Me.HoofdObj.TickDurationInSeconds * 1000)
                ' ''    Me.RotatieInterpolater.EndValue = ISnap.EntityDeltaSnapshot.RotatieChange.GetQuaternion
                ' ''    Rot = True

                ' ''End If

                If Pos And Snelh And Rot Then Exit Do
            End If
            ITick += 1
        Loop

        'If Me.PositieInterpolater.Enabled Then Me.PositieInterpolater.Enabled = Pos
    End Sub

    Public Overrides Function AddEntityDeltaSnapshot(ByVal nTick As Integer, ByVal nEDS As EntityDeltaSnapshot) As Boolean
        'Return MyBase.AddEntityDeltaSnapshot(nTick, nEDS)
        Dim DEDS As DynamicEntityDeltaSnapshot
        If nEDS.Data.Length = 0 Then
            DEDS = New DynamicEntityDeltaSnapshot
        Else
            Dim BR As New ByteReader(nEDS.Data)
            DEDS = DynamicEntityDeltaSnapshot.FromNetworkBytes(BR)
            BR.Close()
        End If

        'Beep()
        'If DEDS.PositieChanged Then MsgBox(DEDS.Positie.ToString)
        Me.Snapshots.InsertDynamicEntityDeltaSnapshot(nTick, DEDS)
        'Me.FindInterpolateValues(Me.HoofdObj.LastTick - 2)


    End Function

    Private Sub TickElement_Tick(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.TickBaseElement.TickEventArgs) Handles TickElement.Tick
        'Dim DST As DeltaSnapshotTick = Me.Snapshots(e.Tick - 2)
        'If DST IsNot Nothing Then
        '    With DST.EntityDeltaSnapshot
        '        If .PositieChange IsNot Nothing Then
        '            Me.Positie = .PositieChange.GetPositie
        '        End If
        '    End With
        'End If


        'Me.FindInterpolateValues(Me.HoofdObj.LastTick - 2)




        'If e.Tick = Me.LastTick + 1 Then 'normale tick increase
        '    Me.LastTick = e.Tick
        '    Me.LastTickIndex = Me.TickToIndex(Me.LastTick)
        '    If Me.Snapshots(Me.LastTickIndex) IsNot Nothing Then
        '        Me.Snapshots(Me.IndexClosestLower) = Nothing
        '        Me.IndexClosestLower = Me.LastTickIndex
        '    End If
        '    Me.FindClosestUpper(Me.LastTick)


        'ElseIf e.Tick < Me.LastTick Then 'tick jump


        '    Me.LastTick = e.Tick
        '    Me.LastTickIndex = Me.TickToIndex(Me.LastTick)







        '    '    If Me.Snapshots(Me.IndexOldestSnapshot).Tick - Me.Snapshots.Length >= e.Tick Then
        '    Array.Clear(Me.Snapshots, 0, Me.Snapshots.Length)
        '    Me.IndexOldestSnapshot = -1
        '    Me.IndexClosestLower = -1
        '    Me.IndexClosestUpper = -1
        '    '    Else
        '    '        Me.IndexClosestUpper = Me.IndexClosestLower
        '    '        Dim I As Integer
        '    '    i =
        '    '        Do

        '    '        Loop

        '    '        Me.FindClosest(Me.LastTick)
        '    '    End If


        'ElseIf e.Tick = Me.LastTick Then 'tick jump
        '    'niks doen

        'ElseIf e.Tick > Me.LastTick Then 'tick jump

        '    '    If Me.Snapshots(Me.IndexOldestSnapshot).Tick + Me.Snapshots.Length <= e.Tick Then
        '    Array.Clear(Me.Snapshots, 0, Me.Snapshots.Length)
        '    Me.IndexOldestSnapshot = -1
        '    Me.IndexClosestLower = -1
        '    Me.IndexClosestUpper = -1
        '    '    Else
        '    '        Dim PrevIndexOldestSnapshot As Integer = Me.IndexOldestSnapshot
        '    Me.LastTick = e.Tick
        '    Me.LastTickIndex = Me.TickToIndex(Me.LastTick)
        '    '        Me.FindClosestLower(Me.LastTick)

        '    '        Dim I As Integer = PrevIndexOldestSnapshot
        '    '        Do
        '    '            If I >= Me.IndexOldestSnapshot Then Exit Do
        '    '            Me.Snapshots(I) = Nothing
        '    '            I += 1
        '    '            If I > Me.Snapshots.Length Then I -= Me.Snapshots.Length
        '    '        Loop

        '    '        Me.FindClosestUpper(Me.LastTick)

        '    '    End If

        'End If
    End Sub

    Private Sub ProcessElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessElement.Process
        Me.FindInterpolateValues(CInt(Math.Floor((Me.HoofdObj.Time - RenderDelay) / Me.HoofdObj.TickDuration)))
        If Me.PositieInterpolater.Enabled Then
            Me.Positie = Me.PositieInterpolater.Interpolate(Me.HoofdObj.Time - RenderDelay)
        End If
        If Me.RotatieInterpolater.Enabled Then
            Me.RotatieQuaternion = Me.RotatieInterpolater.Interpolate(Me.HoofdObj.Time - RenderDelay)
        End If
        'If Me.IndexClosestLower <> -1 And Me.IndexClosestUpper <> -1 Then
        '    If Me.Snapshots(Me.IndexClosestLower).EntityDeltaSnapshot.EntityDeltaSnaphostChanges.Count > 0 And _
        '     Me.Snapshots(Me.IndexClosestUpper).EntityDeltaSnapshot.EntityDeltaSnaphostChanges.Count > 0 Then
        '        Dim Pos1 As Vector3
        '        Dim Pos2 As Vector3

        '        Pos1 = Me.Snapshots(Me.IndexClosestLower).EntityDeltaSnapshot.EntityDeltaSnaphostChanges(0).GetPositie
        '        Pos2 = Me.Snapshots(Me.IndexClosestUpper).EntityDeltaSnapshot.EntityDeltaSnaphostChanges(0).GetPositie

        '        Dim Interval As Single = Me.Snapshots(Me.IndexClosestUpper).Tick - Me.Snapshots(Me.IndexClosestLower).Tick
        '        Interval *= Me.HoofdObj.TickDuration
        '        Dim CurrIntervalTime As Single = CSng(Me.HoofdObj.Time / 1000) - CSng(Me.Snapshots(Me.IndexClosestLower).Tick * Me.HoofdObj.TickDuration)


        '        Me.Positie = Pos1 * (CurrIntervalTime / Interval) + Pos2 * (1 / (CurrIntervalTime / Interval))
        '    End If
        'End If
    End Sub
End Class

Public Class DeltaSnapshotTick
    Public Sub New(ByVal nTick As Integer, ByVal nDEDS As DynamicEntityDeltaSnapshot)
        Me.setTick(nTick)
        Me.setDynamicEDS(nDEDS)
    End Sub

    Private mTick As Integer
    Public ReadOnly Property Tick() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTick
        End Get
    End Property
    Private Sub setTick(ByVal value As Integer)
        Me.mTick = value
    End Sub



    Private mDynamicEDS As DynamicEntityDeltaSnapshot
    Public ReadOnly Property DynamicEDS() As DynamicEntityDeltaSnapshot
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDynamicEDS
        End Get
    End Property
    Private Sub setDynamicEDS(ByVal value As DynamicEntityDeltaSnapshot)
        Me.mDynamicEDS = value
    End Sub



End Class
Public Class SnapshotList

    Public Sub New(ByVal nNumSnapshots As Integer)
        Me.setSnapshots(New DeltaSnapshotTick(nNumSnapshots - 1) {})

        Me.FirstElementTick = 0
    End Sub

    Private mSnapshots As DeltaSnapshotTick()
    Public ReadOnly Property Snapshots() As DeltaSnapshotTick()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSnapshots
        End Get
    End Property
    Private Sub setSnapshots(ByVal value As DeltaSnapshotTick())
        Me.mSnapshots = value
    End Sub

    Default Public ReadOnly Property Snapshot(ByVal nTick As Integer) As DeltaSnapshotTick
        Get
            If nTick < Me.FirstElementTick Or nTick > Me.LastElementTick Then Return Nothing
            Return Me.Snapshots(Me.TickToIndex(nTick))
        End Get
    End Property





    Private mIndexFirstElement As Integer
    Public ReadOnly Property IndexFirstElement() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mIndexFirstElement
        End Get
    End Property
    Private Sub setIndexFirstElement(ByVal value As Integer)
        Me.mIndexFirstElement = value
        Me.mIndexLastElement = value - 1
        If Me.mIndexLastElement < 0 Then Me.mIndexLastElement += Me.Snapshots.Length
    End Sub




    Private mFirstElement As DeltaSnapshotTick
    Public ReadOnly Property FirstElement() As DeltaSnapshotTick
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Snapshots(Me.IndexFirstElement)
        End Get
    End Property


    Private mIndexLastElement As Integer
    Public ReadOnly Property IndexLastElement() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mIndexLastElement
        End Get
    End Property
    Private Sub setIndexLastElement(ByVal value As Integer)
        Me.mIndexFirstElement = value + 1
        Me.mIndexLastElement = value - 1
        If Me.mIndexFirstElement > Me.Snapshots.Length Then Me.mIndexFirstElement -= Me.Snapshots.Length
    End Sub



    Private mFirstElementTick As Integer
    Public Property FirstElementTick() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFirstElementTick
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then value = 0

            If value > Me.FirstElementTick Then
                If Me.FirstElementTick + Me.Snapshots.Length <= value Then
                    Array.Clear(Me.Snapshots, 0, Me.Snapshots.Length)
                Else
                    For T As Integer = Me.FirstElementTick To value - 1
                        Me.Snapshots(Me.TickToIndex(T)) = Nothing
                    Next

                End If

            ElseIf value = Me.FirstElementTick Then
                'Exit Property                  ?nuttig?

            ElseIf value < Me.FirstElementTick Then
                If Me.FirstElementTick - Me.Snapshots.Length >= value Then
                    Array.Clear(Me.Snapshots, 0, Me.Snapshots.Length)
                Else
                    For T As Integer = value + 1 To Me.FirstElementTick
                        Me.Snapshots(Me.TickToIndex(T)) = Nothing
                    Next
                End If

            End If
            Me.mFirstElementTick = value
            Me.setIndexFirstElement(Me.TickToIndex(Me.mFirstElementTick))
            Me.mLastElementTick = value + Me.Snapshots.Length - 1
            Me.setIndexLastElement(Me.TickToIndex(Me.mLastElementTick))
        End Set
    End Property






    Private mLastElement As DeltaSnapshotTick
    Public Property LastElement() As DeltaSnapshotTick
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.Snapshots(Me.IndexLastElement)
        End Get
        Set(ByVal value As DeltaSnapshotTick)
            Stop 'not allowed?
            Me.Snapshots(Me.IndexLastElement) = value
        End Set
    End Property


    Private mLastElementTick As Integer
    Public Property LastElementTick() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mLastElementTick
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
            If value < Me.Snapshots.Length Then value = Me.Snapshots.Length
            'Me.mLastElementTick = value
            Me.FirstElementTick = value - Me.Snapshots.Length + 1
        End Set
    End Property



    'Private mIndexClosestLower As Integer
    'Public Property IndexClosestLower() As Integer
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mIndexClosestLower
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    '        Me.mIndexClosestLower = value
    '    End Set
    'End Property




    'Private mIndexClosestUpper As Integer
    'Public Property IndexClosestUpper() As Integer
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mIndexClosestUpper
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    '        Me.mIndexClosestUpper = value
    '    End Set
    'End Property



    'Private mLastTick As Integer
    'Public Property LastTick() As Integer
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mLastTick
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    '        Me.mLastTick = value
    '    End Set
    'End Property




    'Private mLastTickIndex As Integer
    'Public Property LastTickIndex() As Integer
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mLastTickIndex
    '    End Get
    '    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Integer)
    '        Me.mLastTickIndex = value
    '    End Set
    'End Property





    'Protected Sub InsertSnapshot(ByVal nDST As DeltaSnapshotTick)

    'End Sub

    Public Function TickToIndex(ByVal nTick As Integer) As Integer
        Return nTick Mod Me.Snapshots.Length
    End Function

    'Public Function IndexToTick(ByVal nIndex As Integer) As Integer
    '    Return Me.TickAtIndex0 + nIndex
    'End Function

    'Public Sub FindClosest(ByVal nTick As Integer)
    '    Me.FindClosestLower(nTick)
    '    Me.FindClosestUpper(nTick)

    'End Sub

    'Public Sub FindClosestLower(ByVal nTick As Integer)
    '    If Me.IndexOldestSnapshot = -1 Then
    '        Me.IndexClosestLower = -1
    '        Exit Sub
    '    End If

    '    Dim I As Integer

    '    I = Me.TickToIndex(nTick)
    '    Do
    '        If Me.Snapshots(I) IsNot Nothing Then
    '            Me.IndexClosestLower = I
    '            Exit Do
    '        End If
    '        I -= 1
    '        If I < 0 Then I += Me.Snapshots.Length
    '    Loop
    'End Sub
    'Public Sub FindClosestUpper(ByVal nTick As Integer)
    '    If Me.IndexOldestSnapshot = -1 Then
    '        Me.IndexClosestUpper = -1
    '        Exit Sub
    '    End If

    '    Dim I As Integer
    '    I = Me.TickToIndex(nTick)
    '    Do
    '        I += 1
    '        If I >= Me.Snapshots.Length Then I -= Me.Snapshots.Length
    '        If I = Me.IndexOldestSnapshot Then
    '            Me.IndexClosestUpper = -1
    '            Exit Do
    '        End If
    '        If Me.Snapshots(I) IsNot Nothing Then
    '            Me.IndexClosestUpper = I
    '            Exit Do
    '        End If

    '    Loop
    'End Sub

    Public Function InsertDynamicEntityDeltaSnapshot(ByVal nTick As Integer, ByVal nDEDS As DynamicEntityDeltaSnapshot) As Boolean
        'If nTick < Me.FirstElementTick Or nTick > Me.LastElementTick Then Return False

        If nTick < Me.FirstElementTick Then
            Me.FirstElementTick = nTick
        ElseIf nTick > Me.LastElementTick Then
            Me.LastElementTick = nTick
        Else 'ntick is in de array
            'niets doen
        End If

        Me.Snapshots(Me.TickToIndex(nTick)) = New DeltaSnapshotTick(nTick, nDEDS)

        'Dim index As Integer = Me.TickToIndex(nTick)
        'If Me.IndexOldestSnapshot <> -1 Then
        '    If nTick < Me.Snapshots(Me.IndexOldestSnapshot).Tick Then Return (False)
        '    If Me.Snapshots(Me.IndexOldestSnapshot).Tick + Me.Snapshots.Length - 1 < nTick Then Return False
        'Else
        '    Me.IndexOldestSnapshot = index
        'End If
        'Me.Snapshots(index) = New DeltaSnapshotTick(nTick, nEDS)

        'If Me.LastTick <> -1 Then Me.FindClosest(Me.LastTick)

        Return True
    End Function
End Class


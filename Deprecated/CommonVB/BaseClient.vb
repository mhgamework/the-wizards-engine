Public MustInherit Class BaseClient
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
    End Sub

    '' '' ''Private WithEvents Scheduler As New SchedulerElement(Me)
    '' '' ''Private WithEvents ProcessEventElement As New ProcessEventElement(Me)


    ' '' ''Private mTCPConn As TCPConnection
    ' '' ''Public ReadOnly Property TCPConn() As TCPConnection
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mTCPConn
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Protected Overridable Sub setTCPConn(ByVal value As TCPConnection)
    ' '' ''    Me.mTCPConn = value
    ' '' ''End Sub


    '' '' ''Private mEndPoint As Net.IPEndPoint
    '' '' ''Public Overridable ReadOnly Property EndPoint() As Net.IPEndPoint
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mEndPoint
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Protected Sub setEndPoint(ByVal value As Net.IPEndPoint)
    '' '' ''    Me.mEndPoint = value
    '' '' ''End Sub



    ' '' ''Private mUDPEndPoint As Net.IPEndPoint
    ' '' ''Public ReadOnly Property UDPEndPoint() As Net.IPEndPoint
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mUDPEndPoint
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Protected Sub setUDPEndPoint(ByVal value As Net.IPEndPoint)
    ' '' ''    Me.mUDPEndPoint = value
    ' '' ''End Sub


    ' '' ''Public Overridable ReadOnly Property GetName() As String
    ' '' ''    Get
    ' '' ''        If Me.LoggedIn = False Then
    ' '' ''            Return Me.TCPConn.EndPoint.Address.ToString & ":" & Me.TCPConn.EndPoint.Port.ToString
    ' '' ''            'Return Me.EndPoint.Address.ToString & ":" & Me.EndPoint.Port.ToString
    ' '' ''        Else
    ' '' ''            Return Me.DisplayName
    ' '' ''        End If

    ' '' ''    End Get
    ' '' ''End Property


    ' '' ''Private mLoggedIn As Boolean
    ' '' ''Public Overridable ReadOnly Property LoggedIn() As Boolean
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mLoggedIn
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Protected Sub setLoggedIn(ByVal value As Boolean)
    ' '' ''    Me.mLoggedIn = value
    ' '' ''End Sub


    ' '' ''Private mUsername As String
    ' '' ''Public Overridable ReadOnly Property Username() As String
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mUsername
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Protected Sub setUsername(ByVal value As String)
    ' '' ''    Me.mUsername = value
    ' '' ''End Sub



    ' '' ''Private mDisplayName As String
    ' '' ''Public Property DisplayName() As String
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mDisplayName
    ' '' ''    End Get
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As String)
    ' '' ''        Me.mDisplayName = value
    ' '' ''    End Set
    ' '' ''End Property


    ' '' ''Private mLinkedPlayerEntity As Player
    ' '' ''Public Overridable ReadOnly Property LinkedPlayerEntity() As Player
    ' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    ' '' ''        Return Me.mLinkedPlayerEntity
    ' '' ''    End Get
    ' '' ''End Property
    ' '' ''Protected Overridable Sub setLinkedPlayerEntity(ByVal value As Player)
    ' '' ''    Me.mLinkedPlayerEntity = value
    ' '' ''    If value IsNot Nothing Then
    ' '' ''        value.LinkClient(Me)
    ' '' ''    End If

    ' '' ''End Sub


    ' '' ''Public MustOverride Function Login(ByVal nUsername As String, ByVal nPassword As Byte()) As LoginResult

    '' '' ''Public MustOverride Sub SignaleerVerandering(ByVal sender As Object, ByVal e As VeranderingEventArgs)
    ' '' ''Public MustOverride Sub OnVerandering(ByVal sender As Object, ByVal e As VeranderingEventArgs)





    '' '' ''Private mBeweegdVooruit As Boolean
    '' '' ''Public ReadOnly Property BeweegdVooruit() As Boolean
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mBeweegdVooruit
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Private Sub setBeweegdVooruit(ByVal value As Boolean)
    '' '' ''    Me.mBeweegdVooruit = value
    '' '' ''End Sub

    '' '' ''Private mBeweegdAchteruit As Boolean
    '' '' ''Public ReadOnly Property BeweegdAchteruit() As Boolean
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mBeweegdAchteruit
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Private Sub setBeweegdAchteruit(ByVal value As Boolean)
    '' '' ''    Me.mBeweegdAchteruit = value
    '' '' ''End Sub


    '' '' ''Private mAngles As Vector3
    '' '' ''Public ReadOnly Property Angles() As Vector3
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mAngles
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Protected Sub setAngles(ByVal value As Vector3)
    '' '' ''    Me.mAngles = value

    '' '' ''    Dim sourceMatrix As Matrix = Matrix.RotationYawPitchRoll(Me.Angles.Y, -Me.Angles.X, Me.Angles.Z)
    '' '' ''    Dim source As New Vector3(0.0!, 0.0!, 1.0!)
    '' '' ''    Me.setLookDirection(Vector3.TransformNormal(source, sourceMatrix))

    '' '' ''    If Me.LinkedPlayerEntity IsNot Nothing Then
    '' '' ''        'Me.LinkedPlayerEntity.RotatieOud = New Vector3(Me.LinkedPlayerEntity.RotatieOud.X, Me.Angles.Y, Me.LinkedPlayerEntity.RotatieOud.Z)
    '' '' ''    End If


    '' '' ''End Sub


    '' '' ''Private mLookDirection As Vector3
    '' '' ''Public ReadOnly Property LookDirection() As Vector3
    '' '' ''    <System.Diagnostics.DebuggerStepThrough()> Get
    '' '' ''        Return Me.mLookDirection
    '' '' ''    End Get
    '' '' ''End Property
    '' '' ''Protected Sub setLookDirection(ByVal value As Vector3)
    '' '' ''    Me.mLookDirection = value
    '' '' ''    If Math.Round(value.Length * 10000) <> 10000 Then Stop
    '' '' ''End Sub









    '' '' ''Public Sub UpdatePlayerEntSnelheid(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs)

    '' '' ''End Sub


    '' '' ''Protected Overridable Sub ProcessEventElement_Process(ByVal sender As Object, ByVal e As MHGameWork.Game3DPlay.ProcessElement.ProcessEventArgs) Handles ProcessEventElement.Process
    '' '' ''    'Me.UpdatePlayerEntSnelheid(sender, e)
    '' '' ''End Sub


    ' '' ''Protected Overrides Sub DisposeObject()
    ' '' ''    MyBase.DisposeObject()
    ' '' ''    Me.TCPConn.Dispose()
    ' '' ''    Me.setTCPConn(Nothing)
    ' '' ''End Sub

End Class


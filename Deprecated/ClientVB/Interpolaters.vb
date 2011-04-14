Public MustInherit Class Interpolater(Of T)
    Private mStartValue As T
    Public Property StartValue() As T
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStartValue
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As T)
            Me.mStartValue = value
        End Set
    End Property



    Private mEndValue As T
    Public Property EndValue() As T
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEndValue
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As T)
            Me.mEndValue = value
        End Set
    End Property

    Private mStartTime As Integer
    Public Property StartTime() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStartTime
        End Get
        Set(ByVal value As Integer)
            Me.mStartTime = value
            Me.CalculateDuration()
            Me.UpdateInternalEnabled()
        End Set
    End Property

    Private mEndTime As Integer
    Public Property EndTime() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEndTime
        End Get
        Set(ByVal value As Integer)
            Me.mEndTime = value
            Me.CalculateDuration()
            Me.UpdateInternalEnabled()
        End Set
    End Property

    Protected Sub CalculateDuration()
        If Me.StartTime > Me.EndTime Then
            Me.setDuration(-1)
        Else
            Me.setDuration(Me.EndTime - Me.StartTime)
        End If


    End Sub

    Private mDuration As Integer
    Public ReadOnly Property Duration() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDuration
        End Get
    End Property
    Private Sub setDuration(ByVal value As Integer)
        Me.mDuration = value
    End Sub


    Private mEnabled As Boolean
    Public Property Enabled() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            If Me.InternalEnabled = False Then Return False
            Return Me.mEnabled
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mEnabled = value
        End Set
    End Property


    Private mInternalEnabled As Boolean
    Public ReadOnly Property InternalEnabled() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mInternalEnabled
        End Get
    End Property
    Private Sub setInternalEnabled(ByVal value As Boolean)
        Me.mInternalEnabled = value
    End Sub


    Public Sub UpdateInternalEnabled()
        Me.setInternalEnabled(Me.CheckIfValid)
    End Sub

    Public Function CheckIfValid() As Boolean
        'If Me.Duration <= 0 Then 'Throw New Exception("Invalid start/end time")
        If Me.Duration <= 0 Then Return False
        If Me.StartTime < 0 Then Return False
        If Me.EndTime < 0 Then Return False

        Return True
    End Function

    Public Overridable Overloads Function Interpolate(ByVal nTime As Integer) As T
        If Me.Enabled = False Then Throw New Exception
        Me.CheckIfValid()
        Dim Interp As Single = CSng((nTime - Me.StartTime) / Me.Duration)
        If Interp < 0 Or Interp > 1 Then Throw New Exception
        Return Me.Interpolate(Interp)
    End Function

    Protected MustOverride Overloads Function Interpolate(ByVal nInterp As Single) As T


End Class

Public Class InterpolaterLineairPositie
    Inherits Interpolater(Of Vector3)

    Protected Overloads Overrides Function Interpolate(ByVal nInterp As Single) As Microsoft.DirectX.Vector3
        Return Me.StartValue * (1 - nInterp) + Me.EndValue * nInterp
    End Function
End Class

Public Class InterpolaterLineairRotatie
    Inherits Interpolater(Of Quaternion)

    Protected Overloads Overrides Function Interpolate(ByVal nInterp As Single) As Quaternion
        Return Quaternion.Slerp(Me.StartValue, Me.EndValue, nInterp)
    End Function
End Class

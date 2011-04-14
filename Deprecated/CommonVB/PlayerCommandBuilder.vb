Public Class PlayerCommandBuilder

    Public Sub New()
        'Me.setCommands(New List(Of PlayerCommand))
    End Sub

    'Private mCommands As List(Of PlayerCommand)
    'Public ReadOnly Property Commands() As List(Of PlayerCommand)
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mCommands
    '    End Get
    'End Property
    'Private Sub setCommands(ByVal value As List(Of PlayerCommand))
    '    Me.mCommands = value
    'End Sub


    Private mVooruit As Boolean
    Public Property Vooruit() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mVooruit
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mVooruit = value
        End Set
    End Property



    Private mAchteruit As Boolean
    Public Property Achteruit() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mAchteruit
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mAchteruit = value
        End Set
    End Property



    Private mStrafeLinks As Boolean
    Public Property StrafeLinks() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStrafeLinks
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mStrafeLinks = value
        End Set
    End Property



    Private mStrafeRechts As Boolean
    Public Property StrafeRechts() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStrafeRechts
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mStrafeRechts = value
        End Set
    End Property



    Private mJump As Boolean
    Public Property Jump() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mJump
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mJump = value
        End Set
    End Property


    Private mCrouch As Boolean
    Public Property Crouch() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCrouch
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mCrouch = value
        End Set
    End Property



    Private mRun As Boolean
    Public Property Run() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRun
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mRun = value
        End Set
    End Property




    Private mNoClip As Boolean
    Public Property NoClip() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNoClip
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mNoClip = value
        End Set
    End Property


    Private mPrimaryAttack As Boolean
    Public Property PrimaryAttack() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mPrimaryAttack
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Boolean)
            Me.mPrimaryAttack = value
        End Set
    End Property



    Private mCameraAngles As Vector3
    Public Property CameraAngles() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCameraAngles
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
            Me.mCameraAngles = value
        End Set
    End Property






    Public Function ToBytes() As Byte()
        Dim Booleans01 As Byte = 0
        If Me.Vooruit Then
            Booleans01 = Booleans01 Or CByte(2 ^ 0)
        End If
        If Me.Achteruit Then
            Booleans01 = Booleans01 Or CByte(2 ^ 1)
        End If
        If Me.StrafeLinks Then
            Booleans01 = Booleans01 Or CByte(2 ^ 2)
        End If
        If Me.StrafeRechts Then
            Booleans01 = Booleans01 Or CByte(2 ^ 3)
        End If
        If Me.PrimaryAttack Then
            Booleans01 = Booleans01 Or CByte(2 ^ 4)
        End If
        If Me.Jump Then
            Booleans01 = Booleans01 Or CByte(2 ^ 5)
        End If
        If Me.Crouch Then
            Booleans01 = Booleans01 Or CByte(2 ^ 6)
        End If
        If Me.Run Then
            Booleans01 = Booleans01 Or CByte(2 ^ 7)
        End If

        Dim Booleans02 As Byte = 0

        If Me.NoClip Then
            Booleans02 = Booleans02 Or CByte(2 ^ 0)
        End If
        'If Me.Achteruit Then
        '    Int = Int Or CInt(2 ^ 7)
        'End If

        Dim BW As New ByteWriter
        BW.Write(Booleans01)
        BW.Write(Booleans02)

        BW.Write(Me.CameraAngles)

        Dim B As Byte() = BW.ToBytes
        BW.Close()
        Return B


    End Function

    Public Sub Read(ByVal BR As ByteReader)
        Dim B As New BitArray(New Byte() {BR.ReadByte})
        Me.Vooruit = B(0)
        Me.Achteruit = B(1)
        Me.StrafeLinks = B(2)
        Me.StrafeRechts = B(3)
        Me.PrimaryAttack = B(4)
        Me.Jump = B(5)
        Me.Crouch = B(6)
        Me.Run = B(7)

        B = New BitArray(New Byte() {BR.ReadByte})
        Me.NoClip = B(0)

        Me.CameraAngles = BR.ReadVector3

        'If Me.Vooruit Then
        '    Int = Int Or CInt(2 ^ 0)
        'End If
        'If Me.Achteruit Then
        '    Int = Int Or CInt(2 ^ 1)
        'End If
        'If Me.StrafeLinks Then
        '    Int = Int Or CInt(2 ^ 2)
        'End If
        'If Me.StrafeRechts Then
        '    Int = Int Or CInt(2 ^ 3)
        'End If
        'If Me.PrimaryAttack Then
        '    Int = Int Or CInt(2 ^ 4)
        'End If
        'If Me.Achteruit Then
        '    Int = Int Or CInt(2 ^ 5)
        'End If
        'If Me.Vooruit Then
        '    Int = Int Or CInt(2 ^ 6)
        'End If
        'If Me.Achteruit Then
        '    Int = Int Or CInt(2 ^ 7)
        'End If

        'Dim BW As New ByteWriter
        'BW.Write(Int)

        'Dim B As Byte() = BW.ToBytes
        'BW.Close()


    End Sub

End Class

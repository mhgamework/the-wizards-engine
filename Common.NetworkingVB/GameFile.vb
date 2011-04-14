Public Class GameFile
    Implements INetworkSerializable


    Public Sub New(ByVal nID As Integer, ByVal nFileName As String, ByVal nDescription As String, ByVal nRelativePath As String, ByVal nLocalFile As String, ByVal nVersion As Integer, ByVal nType As GameFileType, ByVal nEnabled As Boolean, ByVal nHash As Byte())
        Me.setID(nID)
        Me.setFileName(nFileName)
        Me.setDescription(nDescription)
        Me.setRelativePath(nRelativePath)
        Me.setLocalFile(nLocalFile)
        Me.setVersion(nVersion)
        Me.setType(nType)
        Me.setEnabled(nEnabled)
        Me.setHash(nHash)
    End Sub
    Public Sub New(ByVal nID As Integer, ByVal nFileName As String, ByVal nRelativePath As String, ByVal nVersion As Integer, ByVal nType As GameFileType)
        Me.New(nID, nFileName, "", nRelativePath, "", nVersion, nType, False, New Byte() {})
    End Sub

    Private Sub New()

    End Sub


    Private mID As Integer
    Public ReadOnly Property ID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mID
        End Get
    End Property
    Private Sub setID(ByVal value As Integer)
        Me.mID = value
    End Sub

    Private mFileName As String
    Public ReadOnly Property FileName() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFileName
        End Get
    End Property
    Private Sub setFileName(ByVal value As String)
        Me.mFileName = value
    End Sub


    Private mDescription As String
    Public ReadOnly Property Description() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDescription
        End Get
    End Property
    Private Sub setDescription(ByVal value As String)
        Me.mDescription = value
    End Sub


    Private mRelativePath As String
    Public ReadOnly Property RelativePath() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRelativePath
        End Get
    End Property
    Private Sub setRelativePath(ByVal value As String)
        Me.mRelativePath = value
    End Sub



    Private mLocalFile As String
    Public ReadOnly Property LocalFile() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mLocalFile
        End Get
    End Property
    Private Sub setLocalFile(ByVal value As String)
        Me.mLocalFile = value
    End Sub


    Private mVersion As Integer
    Public ReadOnly Property Version() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mVersion
        End Get
    End Property
    Private Sub setVersion(ByVal value As Integer)
        Me.mVersion = value
    End Sub


    Private mType As GameFileType
    Public ReadOnly Property Type() As GameFileType
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mType
        End Get
    End Property
    Private Sub setType(ByVal value As GameFileType)
        Me.mType = value
    End Sub


    Private mEnabled As Boolean
    Public ReadOnly Property Enabled() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mEnabled
        End Get
    End Property
    Private Sub setEnabled(ByVal value As Boolean)
        Me.mEnabled = value
    End Sub


    Private mHash As Byte()
    Public ReadOnly Property Hash() As Byte()
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mHash
        End Get
    End Property
    Private Sub setHash(ByVal value As Byte())
        Me.mHash = value
    End Sub




    Public Function Clone() As GameFile
        Return CType(Me.MemberwiseClone, GameFile)
    End Function

    Public Enum GameFileType
        'Alle de waarden zouden expliciet moeten gedefinieerd worden
        'zodat ze constant blijven voor in database
        NotSet = 0
        Other = 1
        Core = 2
    End Enum



    Public Shared Function FromBytes(ByVal BR As BaseByteReader) As GameFile
        Dim CL As New GameFile
        CL.setID(BR.ReadInt32)
        CL.setFileName(BR.ReadString)
        CL.setDescription(BR.ReadString)
        CL.setRelativePath(BR.ReadString)
        CL.setLocalFile(BR.ReadString)
        CL.setVersion(BR.ReadInt32)
        CL.setType(CType(BR.ReadInt32, GameFileType))
        CL.setEnabled(BR.ReadBoolean)
        CL.setHash(BR.ReadBytes(BR.ReadInt32))

        Return CL

    End Function

    Public Function ToNetworkBytes() As Byte() Implements INetworkSerializable.ToNetworkBytes
        Dim BW As New BaseByteWriter
        BW.Write(Me.ID)
        BW.Write(Me.FileName)
        BW.Write(Me.Description)
        BW.Write(Me.RelativePath)
        BW.Write(Me.LocalFile)
        BW.Write(Me.Version)
        BW.Write(Me.Type)
        BW.Write(Me.Enabled)

        BW.Write(Me.Hash.Length)
        BW.Write(Me.Hash)

        Dim B As Byte() = BW.ToBytes
        BW.Close()

        Return B

    End Function



    Public Overrides Function ToString() As String
        Return Me.RelativePath & "\" & Me.FileName

    End Function
End Class
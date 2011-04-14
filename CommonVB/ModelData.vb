Public Class ModelData
    Implements INetworkSerializable


    Public Sub New(ByVal nModelID As Integer, ByVal nCollFileID As Integer, ByVal nVersie As Integer, ByVal nFilename As String, ByVal nRelativePath As String)
        Me.setModelID(nModelID)
        Me.setCollisionFileID(nCollFileID)
        Me.setVersie(nVersie)
        Me.setFileName(nFilename)
        Me.setRelativePath(nRelativePath)
        Me.setFullPath(Forms.Application.StartupPath & nRelativePath & "\" & nFilename)
    End Sub

    Private mModelID As Integer
    Public ReadOnly Property ModelID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModelID
        End Get
    End Property
    Private Sub setModelID(ByVal value As Integer)
        Me.mModelID = value
    End Sub


    Private mCollisionFileID As Integer
    Public ReadOnly Property CollisionFileID() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mCollisionFileID
        End Get
    End Property
    Private Sub setCollisionFileID(ByVal value As Integer)
        Me.mCollisionFileID = value
    End Sub



    Private mVersie As Integer
    Public ReadOnly Property Versie() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mVersie
        End Get
    End Property
    Private Sub setVersie(ByVal value As Integer)
        Me.mVersie = value
    End Sub








    Private mFullPath As String
    Public ReadOnly Property FullPath() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFullPath
        End Get
    End Property
    Private Sub setFullPath(ByVal value As String)
        Me.mFullPath = value
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


    Private mFileName As String
    Public ReadOnly Property FileName() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mFileName
        End Get
    End Property
    Private Sub setFileName(ByVal value As String)
        Me.mFileName = value
    End Sub



    Public Function ToNetworkBytes() As Byte() Implements Networking.INetworkSerializable.ToNetworkBytes
        Dim BW As New ByteWriter
        BW.Write(Me.ModelID)
        BW.Write(Me.CollisionFileID)
        BW.Write(Me.Versie)
        BW.Write(Me.FileName)
        BW.Write(Me.RelativePath)

        Dim B As Byte() = BW.ToBytes
        BW.Close()

        Return B
    End Function

    Public Shared Function FromNetworkBytes(ByVal BR As ByteReader) As ModelData
        Dim MD As New ModelData(BR.ReadInt32, BR.ReadInt32, BR.ReadInt32, BR.ReadString, BR.ReadString)

        Return MD
    End Function
End Class

Namespace Networking
    Public Class GameFilesList
        Implements INetworkSerializable

        Public Sub New()
            Me.setFiles(New Dictionary(Of Integer, GameFile))
        End Sub





        Private mFiles As Dictionary(Of Integer, GameFile)
        Public Overridable ReadOnly Property Files() As Dictionary(Of Integer, GameFile)
            <System.Diagnostics.DebuggerStepThrough()> Get
                Return Me.mFiles
            End Get
        End Property
        Private Sub setFiles(ByVal value As Dictionary(Of Integer, GameFile))
            Me.mFiles = value
        End Sub

        Default Public Overridable ReadOnly Property GetGameFile(ByVal nID As Integer) As GameFile
            Get
                Return Me.Files(nID)
            End Get
        End Property




        Protected Sub AddFilesFromBytes(ByVal BR As BaseByteReader)
            Dim CF As GameFile

            Dim NumFiles As Integer = BR.ReadInt32
            For I As Integer = 0 To NumFiles - 1
                CF = GameFile.FromBytes(BR)
                Me.Files.Add(CF.ID, CF)
            Next

        End Sub

        Public Shared Function FromBytes(ByVal BR As BaseByteReader) As GameFilesList
            Dim CFL As New GameFilesList
            CFL.AddFilesFromBytes(BR)

            Return CFL
        End Function


        Public Function ToBytes() As Byte() Implements INetworkSerializable.ToNetworkBytes
            Dim BW As New BaseByteWriter
            BW.Write(Me.Files.Count)
            For Each GF As GameFile In Me.Files.Values
                BW.Write(GF)
            Next

            Dim B As Byte() = BW.ToBytes
            BW.Close()
            Return B
        End Function


    End Class
End Namespace